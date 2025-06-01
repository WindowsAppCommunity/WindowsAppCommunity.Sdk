using CommunityToolkit.Diagnostics;
using OwlCore.Kubo;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage.System.IO;
using WindowsAppCommunity.Sdk.Nomad;

namespace WindowsAppCommunity.Sdk.Tests;

[TestClass]
public partial class BasicTests
{
    
    [TestMethod]
    public async Task BasicTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(BasicTests)}.{nameof(BasicTestAsync)}", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5012, 8012, cancellationToken);
        var kuboOptions = new KuboOptions
        {
            IpnsLifetime = TimeSpan.FromDays(1),
            ShouldPin = false,
            UseCache = false,
        };

        var managedKeysEnumerable = await kubo.Client.Key.ListAsync(cancellationToken);
        var managedKeys = new List<Key>(managedKeysEnumerable.Select(k => new Key(k)));

        RepositoryContainer repositoryContainer = new(kuboOptions, kubo.Client, managedKeys, [], [], []);

        Guard.IsNotNull(repositoryContainer.ProjectRepository);
        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        Guard.IsNotNull(repositoryContainer.UserRepository);

        var project = await repositoryContainer.ProjectRepository.CreateAsync(new(KnownId: "Test"), cancellationToken);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new(KnownId: "Test"), cancellationToken);
        var user = await repositoryContainer.UserRepository.CreateAsync(new(KnownId: "Test"), cancellationToken);

        await kubo.Client.ShutdownAsync();
        kubo.Dispose();
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder, cancellationToken);
    }
}