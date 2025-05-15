using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using OwlCore.Storage.Memory;
using OwlCore.Storage.System.IO;
using WindowsAppCommunity.Sdk.Models;
using WindowsAppCommunity.Sdk.Nomad;

namespace WindowsAppCommunity.Sdk.Tests;

[TestClass]
public partial class UserTests
{
    // Tests to write:
    // - Updating and reading name and description
    // - Updating and reading links
    // - Updating and reading images
    // - Updating and reading accent color
    // - Updating and reading category
    // - Updating and reading user role list

    [TestMethod]
    public async Task BasicPropertyTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(UserTests)}.{nameof(BasicPropertyTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(UserTests)}.{nameof(BasicPropertyTestAsync)}.1", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5013, 8013, cancellationToken);
        var kubo2 = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder2, 5014, 8014, cancellationToken);

        await TestSetupHelpers.ConnectSwarmsAsync([kubo, kubo2], cancellationToken);
        
        var kuboOptions = new KuboOptions
        {
            IpnsLifetime = TimeSpan.FromDays(1),
            ShouldPin = false,
            UseCache = false,
        };

        var managedKeysEnumerable = await kubo.Client.Key.ListAsync(cancellationToken);
        var managedKeys = new List<IKey>(managedKeysEnumerable);
        RepositoryContainer repositoryContainer = new(kuboOptions, kubo.Client, managedKeys);
        
        Guard.IsNotNull(repositoryContainer.UserRepository);
        var user = await repositoryContainer.UserRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(user);
        Guard.IsNotNull(user.Id);
        Guard.IsNotNull(user.Name);
        Guard.IsNotNull(user.Description);
        
        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);

        // Test updating the name and description
        var newName = "New User Name";
        var newDescription = "New User Description";
        var newAccentColor = "#000000";
        var newCategory = "New Category";
        var newFeature = "New feature";
        
        await user.UpdateNameAsync(newName, cancellationToken);
        await user.UpdateDescriptionAsync(newDescription, cancellationToken);

        await user.PublishRoamingAsync<ModifiableUser, ValueUpdateEvent, User>(cancellationToken);
        await user.PublishLocalAsync<ModifiableUser, ValueUpdateEvent>(cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);

        // Data should match the updated values
        Guard.IsEqualTo(user.Name, newName);
        Guard.IsEqualTo(user.Description, newDescription);

        // Read user from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.UserRepository);

        var user1 = await repositoryContainer2.UserRepository.GetAsync(user.Id, cancellationToken);
        Guard.IsNotNull(user1);

        // Data should match the updated values
        Guard.IsEqualTo(user1.Name, newName);
        Guard.IsEqualTo(user1.Description, newDescription);
        Guard.IsEqualTo(user1.Id, user.Id);

        await kubo.Client.ShutdownAsync();
        kubo.Dispose();
    
        await kubo2.Client.ShutdownAsync();
        kubo2.Dispose();
    
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder, cancellationToken);
    
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder2, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder2, cancellationToken);
    }
    
    [TestMethod]
    public async Task LinksTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(UserTests)}.{nameof(BasicPropertyTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(UserTests)}.{nameof(BasicPropertyTestAsync)}.1", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5013, 8013, cancellationToken);
        var kubo2 = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder2, 5014, 8014, cancellationToken);

        await TestSetupHelpers.ConnectSwarmsAsync([kubo, kubo2], cancellationToken);
        
        var kuboOptions = new KuboOptions
        {
            IpnsLifetime = TimeSpan.FromDays(1),
            ShouldPin = false,
            UseCache = false,
        };

        var managedKeysEnumerable = await kubo.Client.Key.ListAsync(cancellationToken);
        var managedKeys = new List<IKey>(managedKeysEnumerable);
        RepositoryContainer repositoryContainer = new(kuboOptions, kubo.Client, managedKeys);

        Guard.IsNotNull(repositoryContainer.UserRepository);

        var user = await repositoryContainer.UserRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(user);
        Guard.IsNotNull(user.Id);
        Guard.IsNotNull(user.Name);
        Guard.IsNotNull(user.Description);

        // Test adding a link
        var newLink = new Link
        {
            Id = "test id",
            Url = "http://example.com/",
            Name = "Test Link",
            Description = "Just a test"
        };
        await user.AddLinkAsync(newLink, cancellationToken);

        await user.PublishRoamingAsync<ModifiableUser, ValueUpdateEvent, User>(cancellationToken);
        await user.PublishLocalAsync<ModifiableUser, ValueUpdateEvent>(cancellationToken);

        // Data should match the updated values
        Guard.IsEqualTo(user.Links.First().Name, newLink.Name);
        Guard.IsEqualTo(user.Links.First().Description, newLink.Description);
        Guard.IsEqualTo(user.Links.First().Url, newLink.Url);

        // Read user from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.UserRepository);

        var user1 = await repositoryContainer2.UserRepository.GetAsync(user.Id, cancellationToken);
        Guard.IsNotNull(user1);

        // Data should match the updated values
        Guard.IsEqualTo(user1.Links.First().Name, newLink.Name);
        Guard.IsEqualTo(user1.Links.First().Description, newLink.Description);
        Guard.IsEqualTo(user1.Links.First().Url, newLink.Url);

        await kubo.Client.ShutdownAsync();
        kubo.Dispose();
    
        await kubo2.Client.ShutdownAsync();
        kubo2.Dispose();
    
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder, cancellationToken);
    
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder2, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder2, cancellationToken);
    }
    
    [TestMethod]
    public async Task ImagesTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(UserTests)}.{nameof(ImagesTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(UserTests)}.{nameof(ImagesTestAsync)}.1", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5015, 8015, cancellationToken);
        var kubo2 = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder2, 5016, 8016, cancellationToken);

        await TestSetupHelpers.ConnectSwarmsAsync([kubo, kubo2], cancellationToken);
        
        var kuboOptions = new KuboOptions
        {
            IpnsLifetime = TimeSpan.FromDays(1),
            ShouldPin = false,
            UseCache = false,
        };

        var managedKeysEnumerable = await kubo.Client.Key.ListAsync(cancellationToken);
        var managedKeys = new List<IKey>(managedKeysEnumerable);
        RepositoryContainer repositoryContainer = new(kuboOptions, kubo.Client, managedKeys);

        Guard.IsNotNull(repositoryContainer.UserRepository);

        var user = await repositoryContainer.UserRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(user);
        Guard.IsNotNull(user.Id);
        Guard.IsNotNull(user.Name);
        Guard.IsNotNull(user.Description);

        // Test adding an image file
        var mockImageFile = new MemoryFile(id: "test id", name: "test name", new MemoryStream());
        await user.AddImageAsync(mockImageFile, cancellationToken);

        await user.PublishRoamingAsync<ModifiableUser, ValueUpdateEvent, User>(cancellationToken);
        await user.PublishLocalAsync<ModifiableUser, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added image
        await foreach (var image in user.GetImageFilesAsync(cancellationToken))
        {
            Guard.IsNotNull(image);
            Guard.IsEqualTo(image.Id, mockImageFile.Id);
            Guard.IsEqualTo(image.Name, mockImageFile.Name);
        }

        // Read user from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.UserRepository);

        var user1 = await repositoryContainer2.UserRepository.GetAsync(user.Id, cancellationToken);
        Guard.IsNotNull(user1);
        
        // Iterating should yield the added image
        await foreach (var image in user1.GetImageFilesAsync(cancellationToken))
        {
            Guard.IsNotNull(image);
            Guard.IsEqualTo(image.Id, mockImageFile.Id);
            Guard.IsEqualTo(image.Name, mockImageFile.Name);
        }

        await kubo.Client.ShutdownAsync();
        kubo.Dispose();
    
        await kubo2.Client.ShutdownAsync();
        kubo2.Dispose();
    
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder, cancellationToken);
    
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder2, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder2, cancellationToken);
    }
    
    [TestMethod]
    public async Task ProjectRoleTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(UserTests)}.{nameof(ProjectRoleTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(UserTests)}.{nameof(ProjectRoleTestAsync)}.1", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5017, 8017, cancellationToken);
        var kubo2 = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder2, 5018, 8018, cancellationToken);

        await TestSetupHelpers.ConnectSwarmsAsync([kubo, kubo2], cancellationToken);
        var kuboOptions = new KuboOptions
        {
            IpnsLifetime = TimeSpan.FromDays(1),
            ShouldPin = false,
            UseCache = false,
        };

        var managedKeysEnumerable = await kubo.Client.Key.ListAsync(cancellationToken);
        var managedKeys = new List<IKey>(managedKeysEnumerable);
        RepositoryContainer repositoryContainer = new(kuboOptions, kubo.Client, managedKeys);

        Guard.IsNotNull(repositoryContainer.UserRepository);
        var user = await repositoryContainer.UserRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(user);
        Guard.IsNotNull(user.Id);
        Guard.IsNotNull(user.Name);
        Guard.IsNotNull(user.Description);
        
        Guard.IsNotNull(repositoryContainer.ProjectRepository);
        var project = await repositoryContainer.ProjectRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(project);
        Guard.IsNotNull(project.Id);
        Guard.IsNotNull(project.Name);

        // Create role
        var role = new Role
        {
            Id = "test role id",
            Name = "test role name",
            Description = "test role desc"
        };

        var newProjectRole = new ModifiableProjectRole
        {
            InnerProject = project,
            Role = role,
        };
        
        // Test adding user/role to user
        await user.AddProjectAsync(newProjectRole, cancellationToken);

        await user.PublishRoamingAsync<ModifiableUser, ValueUpdateEvent, User>(cancellationToken);
        await user.PublishLocalAsync<ModifiableUser, ValueUpdateEvent>(cancellationToken);
        
        await project.PublishRoamingAsync<ModifiableProject, ValueUpdateEvent, Project>(cancellationToken);
        await project.PublishLocalAsync<ModifiableProject, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added user and role
        await foreach (var projectRole in user.GetProjectsAsync(cancellationToken))
        {
            Guard.IsEqualTo(projectRole.Role.Id, role.Id);
            Guard.IsEqualTo(projectRole.Role.Name, role.Name);
            Guard.IsEqualTo(projectRole.Role.Description, role.Description);
        }

        // Read user from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.UserRepository);

        var user1 = await repositoryContainer2.UserRepository.GetAsync(user.Id, cancellationToken);
        Guard.IsNotNull(user1);

        // Iterating should yield the added user and role
        await foreach (var projectRole in user1.GetProjectsAsync(cancellationToken))
        {
            Guard.IsEqualTo(projectRole.Role.Id, role.Id);
            Guard.IsEqualTo(projectRole.Role.Name, role.Name);
            Guard.IsEqualTo(projectRole.Role.Description, role.Description);
        }

        await kubo.Client.ShutdownAsync();
        kubo.Dispose();
    
        await kubo2.Client.ShutdownAsync();
        kubo2.Dispose();
    
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder, cancellationToken);
    
        await TestSetupHelpers.SetAllFileAttributesRecursive(testTempFolder2, attributes => attributes & ~FileAttributes.ReadOnly);
        await temp.DeleteAsync(testTempFolder2, cancellationToken);
    }
}
