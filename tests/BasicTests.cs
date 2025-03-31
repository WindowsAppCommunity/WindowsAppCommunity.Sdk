using System.Diagnostics;
using Ipfs;
using OwlCore.Diagnostics;
using OwlCore.Kubo;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using OwlCore.Storage;
using OwlCore.Storage.System.IO;
using WindowsAppCommunity.Sdk.Models;
using WindowsAppCommunity.Sdk.Nomad;

namespace WindowsAppCommunity.Sdk.Tests;

[TestClass]
public partial class BasicTests
{
    private void LoggerOnMessageReceived(object? sender, LoggerMessageEventArgs args) => Debug.WriteLine(args.Message);
    
    [TestMethod]
    public async Task BasicTestAsync()
    {
        Logger.MessageReceived += LoggerOnMessageReceived;
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await SafeCreateFolderAsync(temp, $"{nameof(BasicTests)}.{nameof(BasicTestAsync)}", cancellationToken);

        var kubo = await BootstrapKuboAsync(testTempFolder, 5012, 8012, cancellationToken);
        var kuboOptions = new KuboOptions
        {
            IpnsLifetime = TimeSpan.FromDays(1),
            ShouldPin = false,
            UseCache = false,
        };

        var client = kubo.Client;
        var projectRoamingKeyName = "TestProject.Roaming";
        var projectLocalKeyName = "TestProject.Local";
        var publisherRoamingKeyName = "TestPublisher.Roaming";
        var publisherLocalKeyName = "TestPublisher.Local"; 
        var userRoamingKeyName = "TestUser.Roaming";
        var userLocalKeyName = "TestUser.Local";

        NomadKuboRepository<ModifiableProject, IReadOnlyProject, Project, ValueUpdateEvent> projectRepository = null!;
        NomadKuboRepository<ModifiablePublisher, IReadOnlyPublisher, Publisher, ValueUpdateEvent> publisherRepository = null!;
        NomadKuboRepository<ModifiableUser, IReadOnlyUser, User, ValueUpdateEvent> userRepository = null!;

        projectRepository = new NomadKuboRepository<ModifiableProject, IReadOnlyProject, Project, ValueUpdateEvent>
        {
            DefaultEventStreamLabel = "Test Project",
            Client = kubo.Client,
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
            ModifiableFromHandlerConfig = config => ModifiableProject.FromHandlerConfig(config, projectRepository, publisherRepository, userRepository, client, kuboOptions),
            ReadOnlyFromHandlerConfig = config => ReadOnlyProject.FromHandlerConfig(config, projectRepository, publisherRepository, userRepository, client, kuboOptions),
        };

        publisherRepository = new NomadKuboRepository<ModifiablePublisher, IReadOnlyPublisher, Publisher, ValueUpdateEvent>
        {
            DefaultEventStreamLabel = "Test Publisher",
            Client = kubo.Client,
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
            ModifiableFromHandlerConfig = config => ModifiablePublisher.FromHandlerConfig(config, projectRepository, publisherRepository, userRepository, client, kuboOptions),
            ReadOnlyFromHandlerConfig = config => ReadOnlyPublisher.FromHandlerConfig(config, projectRepository, publisherRepository, userRepository, client, kuboOptions),
        };

        userRepository = new NomadKuboRepository<ModifiableUser, IReadOnlyUser, User, ValueUpdateEvent>
        {
            DefaultEventStreamLabel = "Test User",
            Client = kubo.Client,
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
            ModifiableFromHandlerConfig = config => ModifiableUser.FromHandlerConfig(config, projectRepository, publisherRepository, client, kuboOptions),
            ReadOnlyFromHandlerConfig = config => ReadOnlyUser.FromHandlerConfig(config, projectRepository, publisherRepository, client, kuboOptions),
        };

        var project = await projectRepository.CreateAsync(cancellationToken);
        var publisher = await publisherRepository.CreateAsync(cancellationToken);
        var user = await userRepository.CreateAsync(cancellationToken);

        await kubo.Client.ShutdownAsync();
        kubo.Dispose();
        await SetAllFileAttributesRecursive(testTempFolder, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder, cancellationToken);
        Logger.MessageReceived -= LoggerOnMessageReceived;
    }
    
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
    
    private static async Task<KuboBootstrapper> BootstrapKuboAsync(SystemFolder kuboRepo, int apiPort, int gatewayPort, CancellationToken cancellationToken)
    {
        var kubo = new KuboBootstrapper(kuboRepo.Path)
        {
            ApiUri = new Uri($"http://127.0.0.1:{apiPort}"),
            GatewayUri = new Uri($"http://127.0.0.1:{gatewayPort}"),
            RoutingMode = DhtRoutingMode.None,
            LaunchConflictMode = BootstrapLaunchConflictMode.Attach,
            ApiUriMode = ConfigMode.OverwriteExisting,
            GatewayUriMode = ConfigMode.OverwriteExisting,
        };
                
        await kubo.StartAsync(cancellationToken);
        return kubo;
    }
}