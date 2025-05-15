using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk;
using WindowsAppCommunity.Sdk.Models;
using WindowsAppCommunity.Sdk.Nomad;
using Link = WindowsAppCommunity.Sdk.Link;

/// <summary>
/// A modifiable event stream handler for modifying roaming publisher data.
/// </summary>
public class ModifiablePublisher : NomadKuboEventStreamHandler<ValueUpdateEvent>, IDelegable<Publisher>, IModifiablePublisher
{
    /// <summary>
    /// Creates a new instance of <see cref="ModifiablePublisher"/> from the specified handler configuration.
    /// </summary>
    /// <param name="handlerConfig">The handler configuration to create the instance from.</param>
    /// <param name="userRepository">The repository to use for returning modifiable or readonly users.</param>
    /// <param name="publisherRepository">The repository to use for returning modifiable or readonly publishers.</param>
    /// <param name="projectDependencyRepository">The repository to use for returning modifiable or readonly projects.</param>
    /// <param name="client">The client used to interact with the ipfs network.</param>
    /// <param name="kuboOptions">The options used to read and write data to and from Kubo.</param>
    /// <returns>A new instance of <see cref="ModifiablePublisher"/>.</returns>
    public static ModifiablePublisher FromHandlerConfig(NomadKuboEventStreamHandlerConfig<Publisher> handlerConfig, INomadKuboRepositoryBase<ModifiableProject, IReadOnlyProject> projectDependencyRepository, INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> publisherRepository, INomadKuboRepositoryBase<ModifiableUser, IReadOnlyUser> userRepository, ICoreApi client, IKuboOptions kuboOptions)
    {
        Guard.IsNotNull(handlerConfig.RoamingValue);
        Guard.IsNotNull(handlerConfig.RoamingKey);
        Guard.IsNotNull(handlerConfig.LocalValue);
        Guard.IsNotNull(handlerConfig.LocalKey);

        var readonlyPublisher = ReadOnlyPublisher.FromHandlerConfig(handlerConfig, projectDependencyRepository, publisherRepository, userRepository, client, kuboOptions);

        ModifiableImagesCollection modifiableImagesCollection = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            Client = client,
            KuboOptions = kuboOptions,
            Inner = readonlyPublisher.InnerEntity.InnerImages,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        IModifiableConnectionsCollection modifiableConnectionsCollection = null!;
        IModifiableLinksCollection modifiableLinksCollection = null!;

        ModifiableEntity modifiableEntity = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            Client = client,
            KuboOptions = kuboOptions,
            Inner = readonlyPublisher.InnerEntity,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            InnerConnections = modifiableConnectionsCollection,
            InnerImages = modifiableImagesCollection,
            InnerLinks = modifiableLinksCollection,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        ModifiableAccentColor modifiableAccentColor = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            Client = client,
            KuboOptions = kuboOptions,
            Inner = readonlyPublisher.InnerAccentColor,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        ModifiableUserRoleCollection modifiableUserRoleCollection = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            Client = client,
            KuboOptions = kuboOptions,
            Inner = readonlyPublisher.InnerUserRoleCollection,
            UserRepository = userRepository,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        var parentPublishers = new ModifiablePublisherCollection
        {
            Id = handlerConfig.RoamingKey.Id,
            Inner = (ReadOnlyPublisherCollection)readonlyPublisher.ParentPublishers,
            Client = client,
            KuboOptions = kuboOptions,
            PublisherRepository = publisherRepository,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        var childPublishers = new ModifiablePublisherCollection
        {
            Id = handlerConfig.RoamingKey.Id,
            Inner = (ReadOnlyPublisherCollection)readonlyPublisher.ChildPublishers,
            Client = client,
            KuboOptions = kuboOptions,
            PublisherRepository = publisherRepository,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        var projectCollection = new ModifiableProjectCollection
        {
            Id = handlerConfig.RoamingKey.Id,
            Inner = readonlyPublisher.InnerProjectCollection,
            Client = client,
            KuboOptions = kuboOptions,
            ProjectRepository = projectDependencyRepository,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        return new ModifiablePublisher
        {
            Id = handlerConfig.RoamingKey.Id,
            InnerPublisher = readonlyPublisher,
            InnerEntity = modifiableEntity,
            InnerAccentColor = modifiableAccentColor,
            InnerUserRoleCollection = modifiableUserRoleCollection,
            InnerProjectCollection = projectCollection,
            ParentPublishers = parentPublishers,
            ChildPublishers = childPublishers,
            Client = client,
            KuboOptions = kuboOptions,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStream = handlerConfig.LocalValue,
            LocalEventStreamKey = handlerConfig.LocalKey,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Sources = handlerConfig.RoamingValue.Sources,
        };
    }

    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <summary>
    /// The read only handler for this project.
    /// </summary>
    public required ReadOnlyPublisher InnerPublisher { get; init; }

    /// <summary>
    /// The read only entity handler for this publisher.
    /// </summary>
    public required ModifiableEntity InnerEntity { get; init; }

    /// <summary>
    /// The read only accent color handler for this publisher.
    /// </summary>
    public required ModifiableAccentColor InnerAccentColor { get; init; }

    /// <summary>
    /// The modifiable project collection handler for this publisher.
    /// </summary>
    public required ModifiableProjectCollection InnerProjectCollection { get; init; }

    /// <summary>
    /// The read only user role handler for this publisher.
    /// </summary>
    public required ModifiableUserRoleCollection InnerUserRoleCollection { get; init; }

    /// <summary>
    /// The roaming project data that this handler modifies.
    /// </summary>
    public Publisher Inner => InnerPublisher.Inner;

    /// <inheritdoc/>
    public required IModifiablePublisherCollection<IReadOnlyPublisher> ParentPublishers { get; init; }

    /// <inheritdoc/>
    public required IModifiablePublisherCollection<IReadOnlyPublisher> ChildPublishers { get; init; }

    /// <inheritdoc/>
    IReadOnlyPublisherCollection IReadOnlyPublisher<IReadOnlyPublisherCollection>.ParentPublishers => (IReadOnlyPublisherCollection)ParentPublishers;

    /// <inheritdoc/>
    IReadOnlyPublisherCollection IReadOnlyPublisher<IReadOnlyPublisherCollection>.ChildPublishers => (IReadOnlyPublisherCollection)ChildPublishers;

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
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerEntity.AddConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerEntity.AddImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) => InnerEntity.AddLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task AddUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerUserRoleCollection.AddUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public Task AddProjectAsync(IReadOnlyProject project, CancellationToken cancellationToken) => InnerProjectCollection.AddProjectAsync(project, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerEntity.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken) => InnerProjectCollection.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerUserRoleCollection.GetUsersAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveProjectAsync(IReadOnlyProject project, CancellationToken cancellationToken) => InnerProjectCollection.RemoveProjectAsync(project, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerEntity.RemoveConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerEntity.RemoveImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken) => InnerEntity.RemoveLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerUserRoleCollection.RemoveUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken) => InnerAccentColor.UpdateAccentColorAsync(accentColor, cancellationToken);

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

    /// <inheritdoc/>
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
            case nameof(UpdateAccentColorAsync):
                await InnerAccentColor.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddUserAsync):
                await InnerUserRoleCollection.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveUserAsync):
                await InnerUserRoleCollection.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(InnerProjectCollection.AddProjectAsync):
                await InnerProjectCollection.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(InnerProjectCollection.RemoveProjectAsync):
                await InnerProjectCollection.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case $"{nameof(ParentPublishers)}.{nameof(IModifiablePublisherCollection<IReadOnlyPublisher>.AddPublisherAsync)}":
                await ((ModifiablePublisherCollection)ParentPublishers).ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case $"{nameof(ParentPublishers)}.{nameof(IModifiablePublisherCollection<IReadOnlyPublisher>.RemovePublisherAsync)}":
                await ((ModifiablePublisherCollection)ParentPublishers).ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case $"{nameof(ChildPublishers)}.{nameof(IModifiablePublisherCollection<IReadOnlyPublisher>.AddPublisherAsync)}":
                await ((ModifiablePublisherCollection)ChildPublishers).ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case $"{nameof(ChildPublishers)}.{nameof(IModifiablePublisherCollection<IReadOnlyPublisher>.RemovePublisherAsync)}":
                await ((ModifiablePublisherCollection)ChildPublishers).ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            default:
                throw new NotImplementedException();
        };
    }

    /// <inheritdoc/>
    public override async Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        EventStreamPosition = null;
        await InnerEntity.ResetEventStreamPositionAsync(cancellationToken);
        await InnerAccentColor.ResetEventStreamPositionAsync(cancellationToken);

        await InnerUserRoleCollection.ResetEventStreamPositionAsync(cancellationToken);
        await InnerProjectCollection.ResetEventStreamPositionAsync(cancellationToken);
        await ((ModifiablePublisherCollection)ParentPublishers).ResetEventStreamPositionAsync(cancellationToken);
        await ((ModifiablePublisherCollection)ChildPublishers).ResetEventStreamPositionAsync(cancellationToken);
    }
}