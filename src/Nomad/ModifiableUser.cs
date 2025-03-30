using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a user that can be modified.
/// </summary>
public class ModifiableUser : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableUser
{
    /// <summary>
    /// Creates a new instance of the <see cref="ModifiableUser"/> class from the given handler configuration.
    /// </summary>
    /// <param name="handlerConfig">The handler configuration to create the instance from.</param>
    /// <param name="projectDependencyRepository">The repository to use for getting modifiable or readonly project dependency instances.</param>
    /// <param name="publisherRepository">>The repository to use for getting modifiable or readonly publisher instances.</param>
    /// <param name="client">The client used to interact with the ipfs network.</param>
    /// <param name="kuboOptions">The options used to read and write data to and from Kubo.</param>
    /// <returns>A new instance of <see cref="ModifiableUser"/>.</returns>
    public static ModifiableUser FromHandlerConfig(NomadKuboEventStreamHandlerConfig<User> handlerConfig, NomadKuboRepository<ModifiableProject, IReadOnlyProject, Project, ValueUpdateEvent> projectDependencyRepository, NomadKuboRepository<ModifiablePublisher, IReadOnlyPublisher, Publisher, ValueUpdateEvent> publisherRepository, ICoreApi client, IKuboOptions kuboOptions)
    {
        Guard.IsNotNull(handlerConfig.RoamingValue);
        Guard.IsNotNull(handlerConfig.RoamingKey);
        Guard.IsNotNull(handlerConfig.LocalValue);
        Guard.IsNotNull(handlerConfig.LocalKey);

        // Root read-only user handler
        var readOnlyUser = ReadOnlyUser.FromHandlerConfig(handlerConfig, projectDependencyRepository, publisherRepository, client, kuboOptions);
        var readOnlyEntity = readOnlyUser.InnerEntity;

        // Modifiable virtual event stream handlers
        IModifiableConnectionsCollection modifiableConnectionsCollection = null!;
        IModifiableLinksCollection modifiableLinksCollection = null!;
        ModifiableImagesCollection modifiableImagesCollection = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            Inner = readOnlyEntity.InnerImages,
            RoamingKey = handlerConfig.RoamingKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            Sources = handlerConfig.RoamingValue.Sources,
            KuboOptions = kuboOptions,
            Client = client,
        };

        ModifiableEntity modifiableEntity = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Inner = readOnlyEntity,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            InnerConnections = modifiableConnectionsCollection,
            InnerImages = modifiableImagesCollection,
            InnerLinks = modifiableLinksCollection,
            Sources = handlerConfig.RoamingValue.Sources,
            KuboOptions = kuboOptions,
            Client = client,
        };

        ModifiablePublisherRoleCollection modifiablePublisherRoleCollection = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Inner = readOnlyUser.InnerPublisherRoles,
            PublisherRepository = publisherRepository,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            Sources = handlerConfig.RoamingValue.Sources,
            KuboOptions = kuboOptions,
            Client = client,
        };

        ModifiableProjectRoleCollection modifiableProjectRoleCollection = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Inner = readOnlyUser.InnerProjectRoles,
            ProjectRepository = projectDependencyRepository,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            Sources = handlerConfig.RoamingValue.Sources,
            KuboOptions = kuboOptions,
            Client = client,
        };

        // Modifiable user root event stream handler.
        return new ModifiableUser
        {
            Id = handlerConfig.RoamingKey.Id,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            InnerUser = readOnlyUser,
            InnerEntity = modifiableEntity,
            InnerPublisherRoles = modifiablePublisherRoleCollection,
            InnerProjectRoles = modifiableProjectRoleCollection,
            RoamingKey = handlerConfig.RoamingKey,
            Sources = handlerConfig.RoamingValue.Sources,
            LocalEventStreamKey = handlerConfig.LocalKey,
            LocalEventStream = handlerConfig.LocalValue,
            KuboOptions = kuboOptions,
            Client = client,
        };
    }

    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <summary>
    /// The handler for reading user data.
    /// </summary>
    public required ReadOnlyUser InnerUser { get; init; }

    /// <summary>
    /// The handler for modifying entity data on this user.
    /// </summary>
    public required ModifiableEntity InnerEntity { get; init; }

    /// <summary>
    /// The handler for modifying publisher roles on this user.
    /// </summary>
    public required ModifiablePublisherRoleCollection InnerPublisherRoles { get; init; }

    /// <summary>
    /// The handler for modifying project roles on this user.
    /// </summary>
    public required ModifiableProjectRoleCollection InnerProjectRoles { get; init; }

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
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerEntity.AddConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerEntity.AddImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) => InnerEntity.AddLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task AddProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken) => InnerProjectRoles.AddProjectAsync(project, cancellationToken);

    /// <inheritdoc/>
    public Task AddPublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken) => InnerPublisherRoles.AddPublisherAsync(publisher, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerEntity.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync(CancellationToken cancellationToken) => InnerProjectRoles.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync(CancellationToken cancellationToken) => InnerPublisherRoles.GetPublishersAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerEntity.RemoveConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerEntity.RemoveImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken) => InnerEntity.RemoveLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken) => InnerProjectRoles.RemoveProjectAsync(project, cancellationToken);

    /// <inheritdoc/>
    public Task RemovePublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken) => InnerPublisherRoles.RemovePublisherAsync(publisher, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken) => InnerEntity.UpdateDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateExtendedDescriptionAsync(string extendedDescription, CancellationToken cancellationToken) => InnerEntity.UpdateExtendedDescriptionAsync(extendedDescription, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken) => InnerEntity.UpdateForgetMeStatusAsync(forgetMe, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken) => InnerEntity.UpdateNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateUnlistedStateAsync(bool isUnlisted, CancellationToken cancellationToken) => InnerEntity.UpdateUnlistedStateAsync(isUnlisted, cancellationToken);

    /// <inheritdoc />
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        switch (streamEntry.EventId)
        {
            case nameof(UpdateNameAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateDescriptionAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateExtendedDescriptionAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateForgetMeStatusAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateUnlistedStateAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddConnectionAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveConnectionAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddLinkAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveLinkAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddImageAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveImageAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddPublisherAsync):
                await InnerPublisherRoles.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemovePublisherAsync):
                await InnerPublisherRoles.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddProjectAsync):
                await InnerProjectRoles.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveProjectAsync):
                await InnerProjectRoles.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    public override async Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Entity, PublisherRoles, ProjectRoles
        await InnerEntity.ResetEventStreamPositionAsync(cancellationToken);
        await InnerPublisherRoles.ResetEventStreamPositionAsync(cancellationToken);
        await InnerProjectRoles.ResetEventStreamPositionAsync(cancellationToken);
    }
}
