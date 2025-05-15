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
public partial class ProjectTests
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
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(ProjectTests)}.{nameof(BasicPropertyTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(ProjectTests)}.{nameof(BasicPropertyTestAsync)}.1", cancellationToken);

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

        Guard.IsNotNull(repositoryContainer.ProjectRepository);
        var project = await repositoryContainer.ProjectRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(project);
        Guard.IsNotNull(project.Id);
        Guard.IsNotNull(project.Name);
        Guard.IsNotNull(project.Description);
        
        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);

        // Test updating the name and description
        var newName = "New Project Name";
        var newDescription = "New Project Description";
        var newAccentColor = "#000000";
        var newCategory = "New Category";
        var newFeature = "New feature";
        
        await project.UpdateNameAsync(newName, cancellationToken);
        await project.UpdateDescriptionAsync(newDescription, cancellationToken);
        await project.UpdateAccentColorAsync(newAccentColor, cancellationToken);
        await project.UpdateCategoryAsync(newCategory, cancellationToken);
        await project.AddFeatureAsync(newFeature, cancellationToken);
        await project.UpdatePublisherAsync(publisher, cancellationToken);

        await project.PublishRoamingAsync<ModifiableProject, ValueUpdateEvent, Project>(cancellationToken);
        await project.PublishLocalAsync<ModifiableProject, ValueUpdateEvent>(cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);

        // Data should match the updated values
        Guard.IsEqualTo(project.Name, newName);
        Guard.IsEqualTo(project.Description, newDescription);
        Guard.IsNotNull(project.AccentColor);
        Guard.IsEqualTo(project.AccentColor, newAccentColor);
        Guard.IsEqualTo(project.Category, newCategory);
        Guard.IsEqualTo(project.Features.First(), newFeature);
        
        var retrievedPublisher = await project.GetPublisherAsync(cancellationToken);
        Guard.IsNotNull(retrievedPublisher);
        Guard.IsNotNull(retrievedPublisher.Id, publisher.Id);
        Guard.IsNotNull(retrievedPublisher.Name, publisher.Name);

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.ProjectRepository);

        var project1 = await repositoryContainer2.ProjectRepository.GetAsync(project.Id, cancellationToken);
        Guard.IsNotNull(project1);

        // Data should match the updated values
        Guard.IsEqualTo(project1.Name, newName);
        Guard.IsEqualTo(project1.Description, newDescription);
        Guard.IsEqualTo(project1.Id, project.Id);
        Guard.IsNotNull(project1.AccentColor);
        Guard.IsEqualTo(project1.AccentColor, newAccentColor);
        Guard.IsEqualTo(project1.Category, newCategory);
        Guard.IsEqualTo(project1.Features.First(), newFeature);
        
        var retrievedPublisher1 = await project1.GetPublisherAsync(cancellationToken);
        Guard.IsNotNull(retrievedPublisher1);
        Guard.IsNotNull(retrievedPublisher1.Id, publisher.Id);
        Guard.IsNotNull(retrievedPublisher1.Name, publisher.Name);

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
    [Ignore]
    public async Task LinksTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(ProjectTests)}.{nameof(BasicPropertyTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(ProjectTests)}.{nameof(BasicPropertyTestAsync)}.1", cancellationToken);

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

        Guard.IsNotNull(repositoryContainer.ProjectRepository);

        var project = await repositoryContainer.ProjectRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(project);
        Guard.IsNotNull(project.Id);
        Guard.IsNotNull(project.Name);
        Guard.IsNotNull(project.Description);

        // Test adding a link
        var newLink = new Link
        {
            Uri = "http://example.com/",
            Name = "Test Link",
            Description = "Just a test"
        };
        await project.AddLinkAsync(newLink, cancellationToken);

        await project.PublishRoamingAsync<ModifiableProject, ValueUpdateEvent, Project>(cancellationToken);
        await project.PublishLocalAsync<ModifiableProject, ValueUpdateEvent>(cancellationToken);

        // Data should match the updated values
        Guard.IsEqualTo(project.Links.First().Name, newLink.Name);
        Guard.IsEqualTo(project.Links.First().Description, newLink.Description);
        Guard.IsEqualTo(project.Links.First().Uri, newLink.Uri);

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.ProjectRepository);

        var project1 = await repositoryContainer2.ProjectRepository.GetAsync(project.Id, cancellationToken);
        Guard.IsNotNull(project1);

        // Data should match the updated values
        Guard.IsEqualTo(project1.Links.First().Name, newLink.Name);
        Guard.IsEqualTo(project1.Links.First().Description, newLink.Description);
        Guard.IsEqualTo(project1.Links.First().Uri, newLink.Uri);

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
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(ProjectTests)}.{nameof(ImagesTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(ProjectTests)}.{nameof(ImagesTestAsync)}.1", cancellationToken);

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

        Guard.IsNotNull(repositoryContainer.ProjectRepository);

        var project = await repositoryContainer.ProjectRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(project);
        Guard.IsNotNull(project.Id);
        Guard.IsNotNull(project.Name);
        Guard.IsNotNull(project.Description);

        // Test adding an image file
        var mockImageFile = new MemoryFile(id: "test id", name: "test name", new MemoryStream());
        await project.AddImageAsync(mockImageFile, cancellationToken);

        await project.PublishRoamingAsync<ModifiableProject, ValueUpdateEvent, Project>(cancellationToken);
        await project.PublishLocalAsync<ModifiableProject, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added image
        await foreach (var image in project.GetImageFilesAsync(cancellationToken))
        {
            Guard.IsNotNull(image);
            Guard.IsEqualTo(image.Id, mockImageFile.Id);
            Guard.IsEqualTo(image.Name, mockImageFile.Name);
        }

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.ProjectRepository);

        var project1 = await repositoryContainer2.ProjectRepository.GetAsync(project.Id, cancellationToken);
        Guard.IsNotNull(project1);
        
        // Iterating should yield the added image
        await foreach (var image in project1.GetImageFilesAsync(cancellationToken))
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
    public async Task UserRoleTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(ProjectTests)}.{nameof(UserRoleTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(ProjectTests)}.{nameof(UserRoleTestAsync)}.1", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5017, 8017, cancellationToken);
        var kubo2 = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder2, 5018, 8018, cancellationToken);
        var kuboOptions = new KuboOptions
        {
            IpnsLifetime = TimeSpan.FromDays(1),
            ShouldPin = false,
            UseCache = false,
        };

        await TestSetupHelpers.ConnectSwarmsAsync([kubo, kubo2], cancellationToken);

        var managedKeysEnumerable = await kubo.Client.Key.ListAsync(cancellationToken);
        var managedKeys = new List<IKey>(managedKeysEnumerable);
        RepositoryContainer repositoryContainer = new(kuboOptions, kubo.Client, managedKeys);

        Guard.IsNotNull(repositoryContainer.ProjectRepository);
        var project = await repositoryContainer.ProjectRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(project);
        Guard.IsNotNull(project.Id);
        Guard.IsNotNull(project.Name);
        Guard.IsNotNull(project.Description);
        
        Guard.IsNotNull(repositoryContainer.UserRepository);
        var user = await repositoryContainer.UserRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(user);
        Guard.IsNotNull(user.Id);
        Guard.IsNotNull(user.Name);

        // Create role
        var role = new Role
        {
            Id = "test role id",
            Name = "test role name",
            Description = "test role desc"
        };

        var newUserRole = new ModifiableUserRole
        {
            InnerUser = user,
            Role = role,
        };
        
        // Test adding user/role to project
        await project.AddUserAsync(newUserRole, cancellationToken);

        await project.PublishRoamingAsync<ModifiableProject, ValueUpdateEvent, Project>(cancellationToken);
        await project.PublishLocalAsync<ModifiableProject, ValueUpdateEvent>(cancellationToken);
        
        await user.PublishRoamingAsync<ModifiableUser, ValueUpdateEvent, User>(cancellationToken);
        await user.PublishLocalAsync<ModifiableUser, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added user and role
        await foreach (var userRole in project.GetUsersAsync(cancellationToken))
        {
            Guard.IsEqualTo(userRole.Role.Id, role.Id);
            Guard.IsEqualTo(userRole.Role.Name, role.Name);
            Guard.IsEqualTo(userRole.Role.Description, role.Description);
        }

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.ProjectRepository);
    
        var project1 = await repositoryContainer2.ProjectRepository.GetAsync(project.Id, cancellationToken);
        Guard.IsNotNull(project1);

        // Iterating should yield the added user and role
        await foreach (var userRole in project1.GetUsersAsync(cancellationToken))
        {
            Guard.IsEqualTo(userRole.Role.Id, role.Id);
            Guard.IsEqualTo(userRole.Role.Name, role.Name);
            Guard.IsEqualTo(userRole.Role.Description, role.Description);
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
