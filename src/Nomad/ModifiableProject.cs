using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a project that can be modified.
/// </summary>
public class ModifiableProject : NomadKuboEventStreamHandler<ValueUpdateEvent>, IDelegable<Project>, IModifiableProject
{
    /// <summary>
    /// Creates a new instance of <see cref="ModifiableProject"/> from the specified handler configuration.
    /// </summary>
    /// <param name="handlerConfig">The handler configuration to create the instance from.</param>
    /// <param name="projectDependencyRepository">The repository to use for getting modifiable or readonly project dependency instances.</param>
    /// <param name="publisherRepository">>The repository to use for getting modifiable or readonly publisher instances.</param>
    /// <param name="userRepository">The repository to use for getting modifiable or readonly user instances.</param>
    /// <param name="client">The client used to interact with the ipfs network.</param>
    /// <param name="kuboOptions">The options used to read and write data to and from Kubo.</param>
    /// <returns>A new instance of <see cref="ModifiableProject"/>.</returns>
    public static ModifiableProject FromHandlerConfig(NomadKuboEventStreamHandlerConfig<Project> handlerConfig, INomadKuboRepositoryBase<ModifiableProject, IReadOnlyProject> projectDependencyRepository, INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> publisherRepository, INomadKuboRepositoryBase<ModifiableUser, IReadOnlyUser> userRepository, ICoreApi client, IKuboOptions kuboOptions)
    {
        Guard.IsNotNull(handlerConfig.RoamingValue);
        Guard.IsNotNull(handlerConfig.RoamingKey);
        Guard.IsNotNull(handlerConfig.LocalValue);
        Guard.IsNotNull(handlerConfig.LocalKey);

        // Root read-only project handler
        ReadOnlyProject readOnlyProject = ReadOnlyProject.FromHandlerConfig(handlerConfig, projectDependencyRepository, publisherRepository, userRepository, client, kuboOptions);
        ReadOnlyEntity readOnlyEntity = readOnlyProject.InnerEntity;

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
        
        ModifiableAccentColor modifiableAccentColor = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            Inner = readOnlyProject.InnerAccentColor,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Client = client,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStreamKey = handlerConfig.LocalKey,
            LocalEventStream = handlerConfig.LocalValue,
            KuboOptions = kuboOptions,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        ModifiableUserRoleCollection modifiableUserRoleCollection = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            Inner = readOnlyProject.InnerUserRoleCollection,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            Client = client,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStreamKey = handlerConfig.LocalKey,
            LocalEventStream = handlerConfig.LocalValue,
            KuboOptions = kuboOptions,
            Sources = handlerConfig.RoamingValue.Sources,
            UserRepository = userRepository,
        };

        ModifiableProjectCollection dependencies = new()
        {
            Id = handlerConfig.RoamingKey.Id,
            Inner = (ReadOnlyProjectCollection)readOnlyProject.Dependencies,
            Client = client,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            RoamingKey = handlerConfig.RoamingKey,
            LocalEventStreamKey = handlerConfig.LocalKey,
            LocalEventStream = handlerConfig.LocalValue,
            KuboOptions = kuboOptions,
            ProjectRepository = projectDependencyRepository,
            Sources = handlerConfig.RoamingValue.Sources,
        };

        // Modifiable project root event stream handler.
        return new ModifiableProject
        {
            Id = handlerConfig.RoamingKey.Id,
            EventStreamHandlerId = handlerConfig.RoamingKey.Id,
            InnerProject = readOnlyProject,
            InnerEntity = modifiableEntity,
            InnerAccentColor = modifiableAccentColor,
            InnerUserRoleCollection = modifiableUserRoleCollection,
            Dependencies = dependencies,
            PublisherRepository = publisherRepository,
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
    /// The read only handler for this project.
    /// </summary>
    public required ReadOnlyProject InnerProject { get; init; }

    /// <summary>
    /// The read only entity handler for this project.
    /// </summary>
    public required ModifiableEntity InnerEntity { get; init; }

    /// <summary>
    /// The read only accent color handler for this project.
    /// </summary>
    public required ModifiableAccentColor InnerAccentColor { get; init; }

    /// <summary>
    /// The read only user role handler for this project.
    /// </summary>
    public required ModifiableUserRoleCollection InnerUserRoleCollection { get; init; }

    /// <summary>
    /// The roaming project data that this handler modifies.
    /// </summary>
    public Project Inner => InnerProject.Inner;

    /// <inheritdoc/>
    public required IModifiableProjectCollection<IReadOnlyProject> Dependencies { get; init; }

    /// <inheritdoc/>
    IReadOnlyProjectCollection IReadOnlyProject<IReadOnlyProjectCollection>.Dependencies => (IReadOnlyProjectCollection)Dependencies;

    /// <summary>
    /// A repository to get modifiable or readonly project instances from.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> PublisherRepository { get; init; }

    /// <inheritdoc/>
    public string Category => InnerProject.Category;

    /// <inheritdoc/>
    public string Name => InnerProject.Name;

    /// <inheritdoc/>
    public string Description => InnerProject.Description;

    /// <inheritdoc/>
    public string ExtendedDescription => InnerProject.ExtendedDescription;

    /// <inheritdoc/>
    public bool? ForgetMe => InnerProject.ForgetMe;

    /// <inheritdoc/>
    public bool IsUnlisted => InnerProject.IsUnlisted;

    /// <inheritdoc/>
    public IReadOnlyConnection[] Connections => InnerProject.Connections;

    /// <inheritdoc/>
    public Link[] Links => InnerProject.Links;

    /// <inheritdoc/>
    public string? AccentColor => InnerProject.AccentColor;

    /// <inheritdoc/>
    public string[] Features => InnerProject.Features;

    /// <inheritdoc cref="WindowsAppCommunity.Sdk.IReadOnlyProject.CategoryUpdated" />
    public event EventHandler<string>? CategoryUpdated;

    /// <inheritdoc cref="WindowsAppCommunity.Sdk.IReadOnlyProject.PublisherUpdated" />
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

    /// <inheritdoc/>
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

    /// <inheritdoc cref="IReadOnlyProject{TDependencyCollection}.GetPublisherAsync" />
    public Task<IReadOnlyPublisher?> GetPublisherAsync(CancellationToken cancellationToken) => InnerProject.GetPublisherAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerUserRoleCollection.GetUsersAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task UpdatePublisherAsync(IReadOnlyPublisher publisher, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var valueCid = await Client.Dag.PutAsync(publisher.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(UpdatePublisherAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyPublisherUpdateEntryUpdateAsync(appendedEntry, updateEvent, publisher.Id, publisher, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task UpdateCategoryAsync(string category, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var valueCid = await Client.Dag.PutAsync(category, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(UpdateCategoryAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyCategoryEntryUpdateAsync(appendedEntry, updateEvent, category, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task AddFeatureAsync(string feature, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var valueCid = await Client.Dag.PutAsync(feature, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(AddFeatureAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyFeatureAddEntryUpdateAsync(appendedEntry, updateEvent, feature, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemoveFeatureAsync(string feature, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var valueCid = await Client.Dag.PutAsync(feature, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(RemoveFeatureAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyFeatureRemoveEntryUpdateAsync(appendedEntry, updateEvent, feature, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public Task AddUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerUserRoleCollection.AddUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerUserRoleCollection.RemoveUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerEntity.AddConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerEntity.RemoveConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) => InnerEntity.AddLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken) => InnerEntity.RemoveLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerEntity.AddImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerEntity.RemoveImageAsync(imageFile, cancellationToken);

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
            case nameof(UpdatePublisherAsync):
                Guard.IsNotNull(updateEvent.Value);
                var publisherId = await Client.Dag.GetAsync<Cid>(updateEvent.Value, cancel: cancellationToken);
                await ApplyPublisherUpdateEntryUpdateAsync(streamEntry, updateEvent, publisherId, cancellationToken);
                break;
            case nameof(UpdateCategoryAsync):
                Guard.IsNotNull(updateEvent.Value);
                var category = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyCategoryEntryUpdateAsync(streamEntry, updateEvent, category, cancellationToken);
                break;
            case nameof(AddFeatureAsync):
                Guard.IsNotNull(updateEvent.Value);
                var addedFeature = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyFeatureAddEntryUpdateAsync(streamEntry, updateEvent, addedFeature, cancellationToken);
                break;
            case nameof(RemoveFeatureAsync):
                Guard.IsNotNull(updateEvent.Value);
                var removedFeature = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyFeatureRemoveEntryUpdateAsync(streamEntry, updateEvent, removedFeature, cancellationToken);
                break;
            default:
                throw new NotImplementedException();
        }
        ;
    }

    internal Task ApplyFeatureAddEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, string addedFeature, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(AddFeatureAsync));

        Inner.Features = [.. Inner.Features, addedFeature];
        FeaturesAdded?.Invoke(this, [addedFeature]);
        return Task.CompletedTask;
    }
    
    internal Task ApplyFeatureRemoveEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, string removedFeature, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(RemoveFeatureAsync));

        Inner.Features = Inner.Features.Where(f => f != removedFeature).ToArray();
        FeaturesRemoved?.Invoke(this, [removedFeature]);
        return Task.CompletedTask;
    }

    internal async Task ApplyPublisherUpdateEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, string publisherId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdatePublisherAsync));

        var publisher = await PublisherRepository.GetAsync(publisherId, cancellationToken);
        await ApplyPublisherUpdateEntryUpdateAsync(streamEntry, updateEvent, publisherId, publisher, cancellationToken);
    }

    internal Task ApplyPublisherUpdateEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, string publisherId, IReadOnlyPublisher updatedPublisher, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdatePublisherAsync));

        Inner.Publisher = publisherId;
        PublisherUpdated?.Invoke(this, updatedPublisher);
        return Task.CompletedTask;
    }

    internal Task ApplyCategoryEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, string category, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdateCategoryAsync));

        Inner.Category = category;
        CategoryUpdated?.Invoke(this, category);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override async Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        EventStreamPosition = null;
        await InnerEntity.ResetEventStreamPositionAsync(cancellationToken);
        await InnerAccentColor.ResetEventStreamPositionAsync(cancellationToken);

        await InnerUserRoleCollection.ResetEventStreamPositionAsync(cancellationToken);
        await ((ModifiableProjectCollection)Dependencies).ResetEventStreamPositionAsync(cancellationToken);
    }
}
