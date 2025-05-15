using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a user that cannot be modified.
/// </summary>
public class ReadOnlyUser : IReadOnlyUser, IDelegable<User>
{
    /// <summary>
    /// Creates a new instance of <see cref="ReadOnlyUser"/> from the specified handler configuration.
    /// </summary>
    /// <param name="handlerConfig">The handler configuration to create the instance from.</param>
    /// <param name="projectDependencyRepository">The repository to use for returning modifiable or readonly projects.</param>
    /// <param name="publisherRepository">The repository to use for returning modifiable or readonly publishers.</param>
    /// <param name="client">The client used to interact with the ipfs network.</param>
    /// <param name="kuboOptions">The options used to read and write data to and from Kubo.</param>
    public static ReadOnlyUser FromHandlerConfig(NomadKuboEventStreamHandlerConfig<User> handlerConfig, INomadKuboRepositoryBase<ModifiableProject, IReadOnlyProject> projectDependencyRepository, INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> publisherRepository, ICoreApi client, IKuboOptions kuboOptions)
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

        var projectRoles = new ReadOnlyProjectRoleCollection
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            ProjectRepository = projectDependencyRepository,
            Client = client,
        };

        var publisherRoles = new ReadOnlyPublisherRoleCollection
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            PublisherRepository = publisherRepository,
            Client = client,
        };

        return new ReadOnlyUser
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            InnerEntity = readOnlyEntity,
            InnerPublisherRoles = publisherRoles,
            InnerProjectRoles = projectRoles,
        };
    }

    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <summary>
    /// The roaming user data.
    /// </summary>
    public required User Inner { get; init; }

    /// <summary>
    /// The handler for reading entity data on this user.
    /// </summary>
    public required ReadOnlyEntity InnerEntity { get; init; }

    /// <summary>
    /// The handler for reading publisher roles on this user.
    /// </summary>
    public required ReadOnlyPublisherRoleCollection InnerPublisherRoles { get; init; }

    /// <summary>
    /// The handler for reading project roles on this user.
    /// </summary>
    public required ReadOnlyProjectRoleCollection InnerProjectRoles { get; init; }

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

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksRemoved;

    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ImagesAdded;

    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ImagesRemoved;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersRemoved;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerEntity.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync(CancellationToken cancellationToken) => InnerProjectRoles.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync(CancellationToken cancellationToken) => InnerPublisherRoles.GetPublishersAsync(cancellationToken);
}
