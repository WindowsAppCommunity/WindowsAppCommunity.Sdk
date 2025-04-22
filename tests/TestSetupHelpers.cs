using System.Diagnostics;
using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using OwlCore.Storage;
using OwlCore.Storage.System.IO;
using WindowsAppCommunity.Sdk.Models;
using WindowsAppCommunity.Sdk.Nomad;

namespace WindowsAppCommunity.Sdk.Tests;

public static class TestSetupHelpers
{
    public static void LoggerOnMessageReceived(object? sender, LoggerMessageEventArgs args) => Debug.WriteLine(args.Message);

    /// <summary>
    /// Creates a temp folder for the test fixture to work in, safely unlocking and removing existing files if needed.
    /// </summary>
    /// <returns>The folder that was created.</returns>
    public static async Task<SystemFolder> SafeCreateFolderAsync(SystemFolder rootFolder, string name, CancellationToken cancellationToken)
    {
        // When Kubo is stopped unexpectedly, it may leave some files with a ReadOnly attribute.
        // Since this folder is created every time tests are run, we need to clean up any files leftover from prior runs.
        // To do that, we need to remove the ReadOnly file attribute.
        var testTempRoot = (SystemFolder)await rootFolder.CreateFolderAsync(name, overwrite: false, cancellationToken: cancellationToken);
        await SetAllFileAttributesRecursive(testTempRoot, attributes => attributes & ~FileAttributes.ReadOnly);

        // Delete and recreate the folder.
        return (SystemFolder)await rootFolder.CreateFolderAsync(name, overwrite: true, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Changes the file attributes of all files in all subfolders of the provided <see cref="SystemFolder"/>.
    /// </summary>
    /// <param name="rootFolder">The folder to set file permissions in.</param>
    /// <param name="transform">This function is provided the current file attributes, and should return the new file attributes.</param>
    public static async Task SetAllFileAttributesRecursive(SystemFolder rootFolder, Func<FileAttributes, FileAttributes> transform)
    {
        await foreach (var childFile in rootFolder.GetFilesAsync())
        {
            var file = (SystemFile)childFile;
            file.Info.Attributes = transform(file.Info.Attributes);
        }

        await foreach (var childFolder in rootFolder.GetFoldersAsync())
        {
            var folder = (SystemFolder)childFolder;
            await SetAllFileAttributesRecursive(folder, transform);
        }
    }

    public static async Task<KuboBootstrapper> BootstrapKuboAsync(SystemFolder kuboRepo, int apiPort, int gatewayPort, CancellationToken cancellationToken)
    {
        var kubo = new KuboBootstrapper(kuboRepo.Path)
        {
            ApiUri = new Uri($"http://127.0.0.1:{apiPort}"),
            GatewayUri = new Uri($"http://127.0.0.1:{gatewayPort}"),
            RoutingMode = DhtRoutingMode.None,
            LaunchConflictMode = BootstrapLaunchConflictMode.Throw,
            ApiUriMode = ConfigMode.OverwriteExisting,
            GatewayUriMode = ConfigMode.OverwriteExisting,
        };

        await kubo.StartAsync(cancellationToken);
        return kubo;
    }

    public static async Task ConnectSwarmsAsync(KuboBootstrapper[] bootstrappers, CancellationToken cancellationToken)
    {
        // Get ID and multiaddr from each bootstrapper
        var ids = await bootstrappers.InParallel(x => x.Client.IdAsync(cancel: cancellationToken));

        // Add each multiaddr to each client, except self
        foreach (var id in ids)
        {
            Guard.IsNotNull(id);
            foreach (var bootstrapper in bootstrappers)
            {
                var currentId = await bootstrapper.Client.IdAsync(cancel: cancellationToken);
                if (currentId == id)
                    continue;
                
                foreach (var address in id.Addresses)
                {
                    try
                    {
                        await bootstrapper.Client.Swarm.ConnectAsync(address, cancel: cancellationToken);
                    }
                    catch
                    {
                        continue;
                    }

                    break;
                }
            }
        }
    }

    public static RepositoryContainer CreateTestRepositories(KuboOptions kuboOptions, ICoreApi client, string projectRoamingKeyName, string projectLocalKeyName, string publisherRoamingKeyName, string publisherLocalKeyName, string userRoamingKeyName, string userLocalKeyName)
    {
        var repositoryContainer = new RepositoryContainer();

        repositoryContainer.ProjectRepository = new NomadKuboRepository<ModifiableProject, IReadOnlyProject, Project, ValueUpdateEvent>
        {
            DefaultEventStreamLabel = "Test Project",
            Client = client,
            KuboOptions = kuboOptions,
            GetEventStreamHandlerConfigAsync = async (roamingId, cancellationToken) =>
            {
                var (localKey, roamingKey, foundRoamingId) = await NomadKeyHelpers.RoamingIdToNomadKeysAsync(roamingId, projectRoamingKeyName, projectLocalKeyName, client, cancellationToken);
                return new NomadKuboEventStreamHandlerConfig<Project>
                {
                    RoamingId = roamingKey?.Id ?? (foundRoamingId is not null ? Cid.Decode(foundRoamingId) : null),
                    RoamingKey = roamingKey,
                    RoamingKeyName = projectRoamingKeyName,
                    LocalKey = localKey,
                    LocalKeyName = projectLocalKeyName,
                };
            },
            GetDefaultRoamingValue = (localKey, roamingKey) => new Project
            {
                Name = "Test Project",
                Description = "This is a test project.",
                ExtendedDescription = "This is a test project. It is used to test the Windows App Community SDK.",
                Category = "Test",
                Sources = [localKey.Id],
            },
            ModifiableFromHandlerConfig = config =>
            {
                Guard.IsNotNull(repositoryContainer.ProjectRepository);
                Guard.IsNotNull(repositoryContainer.PublisherRepository);
                Guard.IsNotNull(repositoryContainer.UserRepository);
                return ModifiableProject.FromHandlerConfig(config, repositoryContainer.ProjectRepository, repositoryContainer.PublisherRepository, repositoryContainer.UserRepository, client, kuboOptions);
            },
            ReadOnlyFromHandlerConfig = config =>
            {
                Guard.IsNotNull(repositoryContainer.ProjectRepository);
                Guard.IsNotNull(repositoryContainer.PublisherRepository);
                Guard.IsNotNull(repositoryContainer.UserRepository);
                return ReadOnlyProject.FromHandlerConfig(config, repositoryContainer.ProjectRepository, repositoryContainer.PublisherRepository, repositoryContainer.UserRepository, client, kuboOptions);
            },
        };

        repositoryContainer.PublisherRepository = new NomadKuboRepository<ModifiablePublisher, IReadOnlyPublisher, Publisher, ValueUpdateEvent>
        {
            DefaultEventStreamLabel = "Test Publisher",
            Client = client,
            KuboOptions = kuboOptions,
            GetEventStreamHandlerConfigAsync = async (roamingId, cancellationToken) =>
            {
                var (localKey, roamingKey, foundRoamingId) = await NomadKeyHelpers.RoamingIdToNomadKeysAsync(roamingId, publisherRoamingKeyName, publisherLocalKeyName, client, cancellationToken);
                return new NomadKuboEventStreamHandlerConfig<Publisher>
                {
                    RoamingId = roamingKey?.Id ?? (foundRoamingId is not null ? Cid.Decode(foundRoamingId) : null),
                    RoamingKey = roamingKey,
                    RoamingKeyName = publisherRoamingKeyName,
                    LocalKey = localKey,
                    LocalKeyName = publisherLocalKeyName,
                };
            },
            GetDefaultRoamingValue = (localKey, roamingKey) => new Publisher
            {
                Name = "Test Publisher",
                Description = "This is a test publisher.",
                ExtendedDescription = "This is a test publisher. It is used to test the Windows App Community SDK.",
                Sources = [localKey.Id],
            },
            ModifiableFromHandlerConfig = config =>
            {
                Guard.IsNotNull(repositoryContainer.ProjectRepository);
                Guard.IsNotNull(repositoryContainer.PublisherRepository);
                Guard.IsNotNull(repositoryContainer.UserRepository);
                return ModifiablePublisher.FromHandlerConfig(config, repositoryContainer.ProjectRepository, repositoryContainer.PublisherRepository, repositoryContainer.UserRepository, client, kuboOptions);
            },
            ReadOnlyFromHandlerConfig = config =>
            {
                Guard.IsNotNull(repositoryContainer.ProjectRepository);
                Guard.IsNotNull(repositoryContainer.PublisherRepository);
                Guard.IsNotNull(repositoryContainer.UserRepository);
                return ReadOnlyPublisher.FromHandlerConfig(config, repositoryContainer.ProjectRepository, repositoryContainer.PublisherRepository, repositoryContainer.UserRepository, client, kuboOptions);
            },
        };

        repositoryContainer.UserRepository = new NomadKuboRepository<ModifiableUser, IReadOnlyUser, User, ValueUpdateEvent>
        {
            DefaultEventStreamLabel = "Test User",
            Client = client,
            KuboOptions = kuboOptions,
            GetEventStreamHandlerConfigAsync = async (roamingId, cancellationToken) =>
            {
                var (localKey, roamingKey, foundRoamingId) = await NomadKeyHelpers.RoamingIdToNomadKeysAsync(roamingId, userRoamingKeyName, userLocalKeyName, client, cancellationToken);
                return new NomadKuboEventStreamHandlerConfig<User>
                {
                    RoamingId = roamingKey?.Id ?? (foundRoamingId is not null ? Cid.Decode(foundRoamingId) : null),
                    RoamingKey = roamingKey,
                    RoamingKeyName = userRoamingKeyName,
                    LocalKey = localKey,
                    LocalKeyName = userLocalKeyName,
                };
            },
            GetDefaultRoamingValue = (localKey, roamingKey) => new User
            {
                Name = "Test User",
                Description = "This is a test user.",
                ExtendedDescription = "This is a test user. It is used to test the Windows App Community SDK.",
                Sources = [localKey.Id],
            },
            ModifiableFromHandlerConfig = config =>
            {
                Guard.IsNotNull(repositoryContainer.ProjectRepository);
                Guard.IsNotNull(repositoryContainer.PublisherRepository);
                return ModifiableUser.FromHandlerConfig(config, repositoryContainer.ProjectRepository, repositoryContainer.PublisherRepository, client, kuboOptions);
            },
            ReadOnlyFromHandlerConfig = config =>
            {
                Guard.IsNotNull(repositoryContainer.ProjectRepository);
                Guard.IsNotNull(repositoryContainer.PublisherRepository);
                return ReadOnlyUser.FromHandlerConfig(config, repositoryContainer.ProjectRepository, repositoryContainer.PublisherRepository, client, kuboOptions);
            },
        };
        return repositoryContainer;
    }
}
