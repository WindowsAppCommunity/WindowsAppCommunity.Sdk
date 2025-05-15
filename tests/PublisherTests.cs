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
public partial class PublisherTests
{
    // Tests to write:
    // - Updating and reading name and description
    // - Updating and reading links
    // - Updating and reading images
    // - Updating and reading accent color
    // - Updating and reading user role list
    // - Updating and reading project list
    // - Updating and reading child and parent publishers

    [TestMethod]
    public async Task BasicPropertyTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(BasicPropertyTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(BasicPropertyTestAsync)}.1", cancellationToken);

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
        
        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);

        // Test updating the name and description
        var newName = "New Publisher Name";
        var newDescription = "New Publisher Description";
        var newAccentColor = "#000000";
        
        await publisher.UpdateNameAsync(newName, cancellationToken);
        await publisher.UpdateDescriptionAsync(newDescription, cancellationToken);
        await publisher.UpdateAccentColorAsync(newAccentColor, cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);

        // Data should match the updated values
        Guard.IsEqualTo(publisher.Name, newName);
        Guard.IsEqualTo(publisher.Description, newDescription);
        Guard.IsNotNull(publisher.AccentColor);
        Guard.IsEqualTo(publisher.AccentColor, newAccentColor);

        // Read publisher from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.PublisherRepository);

        var publisher1 = await repositoryContainer2.PublisherRepository.GetAsync(publisher.Id, cancellationToken);
        Guard.IsNotNull(publisher1);

        // Data should match the updated values
        Guard.IsEqualTo(publisher1.Name, newName);
        Guard.IsEqualTo(publisher1.Description, newDescription);
        Guard.IsEqualTo(publisher1.Id, publisher.Id);
        Guard.IsNotNull(publisher1.AccentColor);
        Guard.IsEqualTo(publisher1.AccentColor, newAccentColor);

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
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(BasicPropertyTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(BasicPropertyTestAsync)}.1", cancellationToken);

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

        Guard.IsNotNull(repositoryContainer.PublisherRepository);

        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);

        // Test adding a link
        var newLink = new Link
        {
            Id = "test id",
            Url = "http://example.com/",
            Name = "Test Link",
            Description = "Just a test"
        };
        await publisher.AddLinkAsync(newLink, cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);

        // Data should match the updated values
        Guard.IsEqualTo(publisher.Links.First().Name, newLink.Name);
        Guard.IsEqualTo(publisher.Links.First().Description, newLink.Description);
        Guard.IsEqualTo(publisher.Links.First().Url, newLink.Url);

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.PublisherRepository);

        var publisher1 = await repositoryContainer2.PublisherRepository.GetAsync(publisher.Id, cancellationToken);
        Guard.IsNotNull(publisher1);

        // Data should match the updated values
        Guard.IsEqualTo(publisher1.Links.First().Name, newLink.Name);
        Guard.IsEqualTo(publisher1.Links.First().Description, newLink.Description);
        Guard.IsEqualTo(publisher1.Links.First().Url, newLink.Url);

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
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(ImagesTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(ImagesTestAsync)}.1", cancellationToken);

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

        Guard.IsNotNull(repositoryContainer.PublisherRepository);

        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);

        // Test adding an image file
        var mockImageFile = new MemoryFile(id: "test id", name: "test name", new MemoryStream());
        await publisher.AddImageAsync(mockImageFile, cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added image
        await foreach (var image in publisher.GetImageFilesAsync(cancellationToken))
        {
            Guard.IsNotNull(image);
            Guard.IsEqualTo(image.Id, mockImageFile.Id);
            Guard.IsEqualTo(image.Name, mockImageFile.Name);
        }

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.PublisherRepository);

        var publisher1 = await repositoryContainer2.PublisherRepository.GetAsync(publisher.Id, cancellationToken);
        Guard.IsNotNull(publisher1);
        
        // Iterating should yield the added image
        await foreach (var image in publisher1.GetImageFilesAsync(cancellationToken))
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
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(UserRoleTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(UserRoleTestAsync)}.1", cancellationToken);

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

        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);
        
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
        await publisher.AddUserAsync(newUserRole, cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);
        
        await user.PublishRoamingAsync<ModifiableUser, ValueUpdateEvent, User>(cancellationToken);
        await user.PublishLocalAsync<ModifiableUser, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added user and role
        await foreach (var userRole in publisher.GetUsersAsync(cancellationToken))
        {
            Guard.IsEqualTo(userRole.Role.Id, role.Id);
            Guard.IsEqualTo(userRole.Role.Name, role.Name);
            Guard.IsEqualTo(userRole.Role.Description, role.Description);
        }

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.PublisherRepository);

        var publisher1 = await repositoryContainer2.PublisherRepository.GetAsync(publisher.Id, cancellationToken);
        Guard.IsNotNull(publisher1);

        // Iterating should yield the added user and role
        await foreach (var userRole in publisher1.GetUsersAsync(cancellationToken))
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
    
    [TestMethod]
    public async Task ProjectsListTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(ProjectsListTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(ProjectsListTestAsync)}.1", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5019, 8019, cancellationToken);
        var kubo2 = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder2, 5020, 8020, cancellationToken);

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

        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);
        
        Guard.IsNotNull(repositoryContainer.ProjectRepository);
        var project = await repositoryContainer.ProjectRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(project);
        Guard.IsNotNull(project.Id);
        Guard.IsNotNull(project.Name);
        
        // Test adding user/role to project
        await publisher.AddProjectAsync(project, cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);
        
        await project.PublishRoamingAsync<ModifiableProject, ValueUpdateEvent, Project>(cancellationToken);
        await project.PublishLocalAsync<ModifiableProject, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added project
        await foreach (var project0 in publisher.GetProjectsAsync(cancellationToken))
        {
            Guard.IsEqualTo(project0.Id, project.Id);
            Guard.IsEqualTo(project0.Name, project.Name);
        }

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.PublisherRepository);

        var publisher1 = await repositoryContainer2.PublisherRepository.GetAsync(publisher.Id, cancellationToken);
        Guard.IsNotNull(publisher1);

        // Iterating should yield the added project
        await foreach (var project0 in publisher1.GetProjectsAsync(cancellationToken))
        {
            Guard.IsEqualTo(project0.Id, project.Id);
            Guard.IsEqualTo(project0.Name, project.Name);
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
    public async Task PublisherParentTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(PublisherParentTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(PublisherParentTestAsync)}.1", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5021, 8021, cancellationToken);
        var kubo2 = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder2, 5022, 8022, cancellationToken);

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
        RepositoryContainer repositoryContainer1 = new(kuboOptions, kubo.Client, managedKeys);

        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);
        
        Guard.IsNotNull(repositoryContainer1.PublisherRepository);
        var otherPublisher = await repositoryContainer1.PublisherRepository.CreateAsync(new("Test2"), cancellationToken);
        Guard.IsNotNull(otherPublisher);
        Guard.IsNotNull(otherPublisher.Id);
        Guard.IsNotNull(otherPublisher.Name);

        Guard.IsNotEqualTo(publisher.Id, otherPublisher.Id);
        
        // Test adding parent publisher to publisher
        await publisher.ParentPublishers.AddPublisherAsync(otherPublisher, cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);
        
        await otherPublisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await otherPublisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added project
        await foreach (var publisher0 in publisher.ParentPublishers.GetPublishersAsync(cancellationToken))
        {
            Guard.IsEqualTo(publisher0.Id, otherPublisher.Id);
            Guard.IsEqualTo(publisher0.Name, otherPublisher.Name);
        }

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.PublisherRepository);

        var publisher1 = await repositoryContainer2.PublisherRepository.GetAsync(publisher.Id, cancellationToken);
        Guard.IsNotNull(publisher1);

        // Iterating should yield the added project
        await foreach (var publisher0 in publisher1.ParentPublishers.GetPublishersAsync(cancellationToken))
        {
            Guard.IsEqualTo(publisher0.Id, otherPublisher.Id);
            Guard.IsEqualTo(publisher0.Name, otherPublisher.Name);
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
    public async Task PublisherChildTestAsync()
    {
        var cancellationToken = CancellationToken.None;

        var temp = new SystemFolder(Path.GetTempPath());
        var testTempFolder = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(PublisherChildTestAsync)}.0", cancellationToken);
        var testTempFolder2 = await TestSetupHelpers.SafeCreateFolderAsync(temp, $"{nameof(PublisherTests)}.{nameof(PublisherChildTestAsync)}.1", cancellationToken);

        var kubo = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder, 5023, 8023, cancellationToken);
        var kubo2 = await TestSetupHelpers.BootstrapKuboAsync(testTempFolder2, 5024, 8024, cancellationToken);

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
        RepositoryContainer repositoryContainer1 = new(kuboOptions, kubo.Client, managedKeys);

        Guard.IsNotNull(repositoryContainer.PublisherRepository);
        var publisher = await repositoryContainer.PublisherRepository.CreateAsync(new("Test"), cancellationToken);
        Guard.IsNotNull(publisher);
        Guard.IsNotNull(publisher.Id);
        Guard.IsNotNull(publisher.Name);
        Guard.IsNotNull(publisher.Description);
        
        Guard.IsNotNull(repositoryContainer1.PublisherRepository);
        var otherPublisher = await repositoryContainer1.PublisherRepository.CreateAsync(new("Test2"), cancellationToken);
        Guard.IsNotNull(otherPublisher);
        Guard.IsNotNull(otherPublisher.Id);
        Guard.IsNotNull(otherPublisher.Name);

        Guard.IsNotEqualTo(publisher.Id, otherPublisher.Id);
        
        // Test adding child publisher to publisher
        await publisher.ChildPublishers.AddPublisherAsync(otherPublisher, cancellationToken);

        await publisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await publisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);
        
        await otherPublisher.PublishRoamingAsync<ModifiablePublisher, ValueUpdateEvent, Publisher>(cancellationToken);
        await otherPublisher.PublishLocalAsync<ModifiablePublisher, ValueUpdateEvent>(cancellationToken);

        // Iterating should yield the added project
        await foreach (var publisher0 in publisher.ChildPublishers.GetPublishersAsync(cancellationToken))
        {
            Guard.IsEqualTo(publisher0.Id, otherPublisher.Id);
            Guard.IsEqualTo(publisher0.Name, otherPublisher.Name);
        }

        // Read project from secondary kubo client
        var managedKeysEnumerable2 = await kubo2.Client.Key.ListAsync(cancellationToken);
        var managedKeys2 = new List<IKey>(managedKeysEnumerable2);
        RepositoryContainer repositoryContainer2 = new(kuboOptions, kubo2.Client, managedKeys2);
        Guard.IsNotNull(repositoryContainer2.PublisherRepository);

        var publisher1 = await repositoryContainer2.PublisherRepository.GetAsync(publisher.Id, cancellationToken);
        Guard.IsNotNull(publisher1);

        // Iterating should yield the added project
        await foreach (var publisher0 in publisher1.ChildPublishers.GetPublishersAsync(cancellationToken))
        {
            Guard.IsEqualTo(publisher0.Id, otherPublisher.Id);
            Guard.IsEqualTo(publisher0.Name, otherPublisher.Name);
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
