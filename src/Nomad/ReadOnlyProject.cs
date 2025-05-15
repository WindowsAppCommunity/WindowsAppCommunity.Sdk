using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A read only handler for roaming project data.
/// </summary>
public class ReadOnlyProject : IReadOnlyProject, IDelegable<Project>
{
    /// <summary>
    /// Creates a new instance of <see cref="ReadOnlyProject"/> from the specified handler configuration.
    /// </summary>
    /// <param name="handlerConfig">The handler configuration to create the instance from.</param>
    /// <param name="projectDependencyRepository">The repository to use for returning project dependencies.</param>
    /// <param name="publisherRepository">The repository to use for returning publishers.</param>
    /// <param name="userRepository">>The repository to use for returning users.</param>
    /// <param name="kuboOptions">The options used to read and write data to and from Kubo.</param>
    /// <param name="client">The client used to interact with the ipfs network.</param>
    /// <returns>A new instance of <see cref="ReadOnlyProject"/>.</returns>
    public static ReadOnlyProject FromHandlerConfig(NomadKuboEventStreamHandlerConfig<Project> handlerConfig, INomadKuboRepositoryBase<ModifiableProject, IReadOnlyProject> projectDependencyRepository, INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> publisherRepository, INomadKuboRepositoryBase<ModifiableUser, IReadOnlyUser> userRepository, ICoreApi client, IKuboOptions kuboOptions)
    {
        Guard.IsNotNull(handlerConfig.RoamingValue);
        Guard.IsNotNull(handlerConfig.RoamingId);

        ReadOnlyImagesCollection readOnlyImagesCollection = new ReadOnlyImagesCollection
        {
            Inner = handlerConfig.RoamingValue,
            Client = client,
        };

        ReadOnlyConnectionCollection readOnlyConnectionCollection = new ReadOnlyConnectionCollection
        {
            Inner = handlerConfig.RoamingValue,
            Client = client,
        };

        IReadOnlyLinksCollection readOnlyLinksCollection = null!;

        ReadOnlyEntity readOnlyEntity = new ReadOnlyEntity
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            InnerConnections = readOnlyConnectionCollection,
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

        ReadOnlyProjectCollection dependencies = new ReadOnlyProjectCollection()
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            Client = client,
            KuboOptions = kuboOptions,
            ProjectDependencyRepository = projectDependencyRepository,
        };

        return new ReadOnlyProject
        {
            Id = handlerConfig.RoamingId,
            Inner = handlerConfig.RoamingValue,
            InnerEntity = readOnlyEntity,
            InnerAccentColor = readOnlyAccentColor,
            InnerUserRoleCollection = readOnlyUserRoleCollection,
            Dependencies = dependencies,
            Client = client,
            PublisherRepository = publisherRepository,
        };
    }

    /// <summary>
    /// The client to use for communicating with ipfs.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <summary>
    /// The roaming project data that this handler reads.
    /// </summary>
    public required Project Inner { get; init; }

    /// <summary>
    /// The read only entity handler for this project.
    /// </summary>
    public required ReadOnlyEntity InnerEntity { get; init; }

    /// <summary>
    /// The read only accent color handler for this project.
    /// </summary>
    public required ReadOnlyAccentColor InnerAccentColor { get; init; }

    /// <summary>
    /// The read only user role handler for this project.
    /// </summary>
    public required ReadOnlyUserRoleCollection InnerUserRoleCollection { get; init; }

    /// <summary>
    /// The readonly dependency collection handler for this project.
    /// </summary>
    public required IReadOnlyProjectCollection Dependencies { get; init; }

    /// <summary>
    /// A repository to get modifiable or readonly project instances from.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> PublisherRepository { get; init; }

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
    public Link[] Links => InnerEntity.Links;

    /// <inheritdoc/>
    public string? AccentColor => InnerAccentColor.AccentColor;

    /// <inheritdoc/>
    public string Category => Inner.Category;

    /// <inheritdoc/>
    public string[] Features => Inner.Features;

    /// <inheritdoc/>
    public event EventHandler<string>? CategoryUpdated;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisher>? PublisherUpdated;

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
    public event EventHandler<string?>? AccentColorUpdated;

    /// <inheritdoc/>
    public event EventHandler<string[]>? FeaturesAdded;

    /// <inheritdoc/>
    public event EventHandler<string[]>? FeaturesRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerEntity.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IReadOnlyPublisher?> GetPublisherAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Inner.Publisher is null)
            return null;

        return await PublisherRepository.GetAsync(Inner.Publisher, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyConnection> GetConnectionsAsync(CancellationToken cancellationToken = default) => InnerEntity.GetConnectionsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerUserRoleCollection.GetUsersAsync(cancellationToken);
}
