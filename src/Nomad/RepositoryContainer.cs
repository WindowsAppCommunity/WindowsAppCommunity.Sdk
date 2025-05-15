using System.Collections.Generic;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.Nomad.Kubo;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A simple container for the three repositories used in the SDK.
/// </summary>
/// <remarks>
/// Since the three repositories are interdependent, this container is used to create them in a single place.
/// <para/>
/// DI may not be able to construct the repositories due to the circular dependencies, but they can be constructed via this class.
/// </remarks>
public class RepositoryContainer
{
    /// <summary>
    /// Creates a new instance of the <see cref="RepositoryContainer"/> class.
    /// </summary>
    /// <param name="kuboOptions">The options to use when publishing or retrieving content via Kubo.</param>
    /// <param name="client">The Kubo client to use for API calls to IPFS.</param>
    /// <param name="managedKeys">The collection of keys available to this repository.</param>
    public RepositoryContainer(IKuboOptions kuboOptions, ICoreApi client, ICollection<IKey> managedKeys)
    {
        var userRepository = new UserRepository()
        {
            KuboOptions = kuboOptions,
            Client = client,
            ProjectRepository = null!,
            PublisherRepository = null!,
            ManagedKeys = managedKeys,
        };

        var projectRepository = new ProjectRepository()
        {
            KuboOptions = kuboOptions,
            Client = client,
            UserRepository = userRepository,
            PublisherRepository = null!,
            ManagedKeys = managedKeys,
        };

        userRepository.ProjectRepository = projectRepository;

        var publisherRepository = new PublisherRepository()
        {
            KuboOptions = kuboOptions,
            Client = client,
            UserRepository = userRepository,
            ProjectRepository = projectRepository,
            ManagedKeys = managedKeys,
        };

        projectRepository.PublisherRepository = publisherRepository;
        projectRepository.UserRepository = userRepository; 

        UserRepository = userRepository;
        ProjectRepository = projectRepository;
        PublisherRepository = publisherRepository;
    }

    /// <summary>
    /// The repository to use for returning modifiable or readonly projects.
    /// </summary>
    public ProjectRepository ProjectRepository { get; }

    /// <summary>
    /// The repository to use for returning modifiable or readonly publishers.
    /// </summary>
    public PublisherRepository PublisherRepository { get; }

    /// <summary>
    /// The repository to use for returning modifiable or readonly users.
    /// </summary>
    public UserRepository UserRepository { get; }
}
