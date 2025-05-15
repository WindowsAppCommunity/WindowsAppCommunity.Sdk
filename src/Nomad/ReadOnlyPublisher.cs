using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk;
using WindowsAppCommunity.Sdk.Models;
using WindowsAppCommunity.Sdk.Nomad;
using Link = WindowsAppCommunity.Sdk.Link;

/// <summary>
/// A read-only event stream handler for reading roaming publisher data.
/// </summary>
public class ReadOnlyPublisher : IReadOnlyPublisher, IDelegable<Publisher>
{
    /// <summary>
    /// Creates a new instance of <see cref="ReadOnlyPublisher"/> from the specified handler configuration.
    /// </summary>
    /// <param name="handlerConfig">The handler configuration to create the instance from.</param>
    /// <param name="userRepository">The repository to use for returning modifiable or readonly users.</param>
    /// <param name="publisherRepository">The repository to use for returning modifiable or readonly publishers.</param>
    /// <param name="projectDependencyRepository">The repository to use for returning modifiable or readonly projects.</param>
    /// <param name="client">The client used to interact with the ipfs network.</param>
    /// <param name="kuboOptions">The options used to read and write data to and from Kubo.</param>
    public static ReadOnlyPublisher FromHandlerConfig(NomadKuboEventStreamHandlerConfig<Publisher> handlerConfig, INomadKuboRepositoryBase<ModifiableProject, IReadOnlyProject> projectDependencyRepository, INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> publisherRepository, INomadKuboRepositoryBase<ModifiableUser, IReadOnlyUser> userRepository, ICoreApi client, IKuboOptions kuboOptions)
    {
        Guard.IsNotNull(handlerConfig.RoamingValue);
        Guard.IsNotNull(handlerConfig.RoamingId);

        ReadOnlyImagesCollection readOnlyImagesCollection = new ReadOnlyImagesCollection
        {
            Inner = handlerConfig.RoamingValue,
            Client = client,
        };

        IReadOnlyConnectionsCollection readOnlyConnectionsCollection = new ReadOnlyConnectionsCollection
        {
            Inner = handlerConfig.RoamingValue,
            Client = client,
        };

        IReadOnlyLinksCollection readOnlyLinksCollection = null!;

        ReadOnlyEntity readOnlyEntity = new ReadOnlyEntity
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            InnerConnections = readOnlyConnectionsCollection,
            InnerImages = readOnlyImagesCollection,
            InnerLinks = readOnlyLinksCollection,
            Client = client,
        };

        ReadOnlyAccentColor readOnlyAccentColor = new ReadOnlyAccentColor
        {
            Inner = handlerConfig.RoamingValue,
            Client = client,
        };

        ReadOnlyUserRoleCollection readOnlyUserRoleCollection = new()
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            UserRepository = userRepository,
            Client = client,
        };

        var parentPublishers = new ReadOnlyPublisherCollection
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue.ParentPublishers,
            Client = client,
            PublisherRepository = publisherRepository,
        };

        var childPublishers = new ReadOnlyPublisherCollection
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue.ChildPublishers,
            Client = client,
            PublisherRepository = publisherRepository,
        };

        var projectCollection = new ReadOnlyProjectCollection
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            Client = client,
            KuboOptions = kuboOptions,
            ProjectDependencyRepository = projectDependencyRepository,
        };

        return new ReadOnlyPublisher
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            InnerEntity = readOnlyEntity,
            InnerAccentColor = readOnlyAccentColor,
            InnerUserRoleCollection = readOnlyUserRoleCollection,
            InnerProjectCollection = projectCollection,
            ParentPublishers = parentPublishers,
            ChildPublishers = childPublishers,
        };
    }

    /// <summary>
    /// The read only entity handler for this publisher.
    /// </summary>
    public required ReadOnlyEntity InnerEntity { get; init; }

    /// <summary>
    /// The read only accent color handler for this publisher.
    /// </summary>
    public required ReadOnlyAccentColor InnerAccentColor { get; init; }

    /// <summary>
    /// The read only user role handler for this publisher.
    /// </summary>
    public required ReadOnlyUserRoleCollection InnerUserRoleCollection { get; init; }

    /// <summary>
    /// The handler for reading projects and roles on this publisher.
    /// </summary>
    public required ReadOnlyProjectCollection InnerProjectCollection { get; init; }

    /// <inheritdoc/>
    public required IReadOnlyPublisherCollection ParentPublishers { get; init; }
    
    /// <inheritdoc/>
    public required IReadOnlyPublisherCollection ChildPublishers { get; init; }

    /// <inheritdoc/>
    public required Publisher Inner { get; init; }
    
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public string Name => InnerEntity.Name;

    /// <inheritdoc/>
    public string Description => InnerEntity.Description;

    /// <inheritdoc/>
    public string ExtendedDescription => InnerEntity.ExtendedDescription;

    /// <inheritdoc/>
    public bool? ForgetMe => InnerEntity.ForgetMe;

    /// <inheritdoc/>
    public bool IsUnlisted => InnerEntity.IsUnlisted;

    /// <inheritdoc/>
    public IReadOnlyConnection[] Connections => InnerEntity.Connections;

    /// <inheritdoc/>
    public Link[] Links => InnerEntity.Links;

    /// <inheritdoc/>
    public string? AccentColor => InnerAccentColor.AccentColor;

    /// <inheritdoc/>
    public event EventHandler<string>? NameUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<string>? DescriptionUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<string>? ExtendedDescriptionUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<bool?>? ForgetMeUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<bool>? IsUnlistedUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;
    
    /// <inheritdoc/>
    public event EventHandler<Link[]>? LinksAdded;
    
    /// <inheritdoc/>
    public event EventHandler<Link[]>? LinksRemoved;
    
    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ImagesAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ImagesRemoved;
    
    /// <inheritdoc/>
    public event EventHandler<string?>? AccentColorUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerEntity.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken) => InnerProjectCollection.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerUserRoleCollection.GetUsersAsync(cancellationToken);
}