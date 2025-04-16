using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage.System.IO;

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

        var projectRoamingKeyName = "TestProject.Roaming";
        var projectLocalKeyName = "TestProject.Local";
        var publisherRoamingKeyName = "TestPublisher.Roaming";
        var publisherLocalKeyName = "TestPublisher.Local";
        var userRoamingKeyName = "TestUser.Roaming";
        var userLocalKeyName = "TestUser.Local";

        RepositoryContainer repositoryContainer = TestSetupHelpers.CreateTestRepositories(kuboOptions, kubo.Client, projectRoamingKeyName, projectLocalKeyName, publisherRoamingKeyName, publisherLocalKeyName, userRoamingKeyName, userLocalKeyName);

        Guard.IsNotNull(repositoryContainer.ProjectRepository);
        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        Guard.IsNotNull(repositoryContainer.UserRepository);

        var project = await repositoryContainer.ProjectRepository.CreateAsync(cancellationToken);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(cancellationToken);
        var user = await repositoryContainer.UserRepository.CreateAsync(cancellationToken);

        await kubo.Client.ShutdownAsync();
        kubo.Dispose();
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder, cancellationToken);
    }
}