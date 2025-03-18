using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
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
    public required IModifiableUserRoleCollection InnerUserRoleCollection { get; init; }

    /// <summary>
    /// The roaming project data that this handler modifies.
    /// </summary>
    public required Project Inner { get; init; }

    /// <inheritdoc/>
    public required IModifiableProjectCollection<IReadOnlyProject> Dependencies { get; init; }

    /// <inheritdoc/>
    public string Category => InnerProject.Category;

    /// <inheritdoc/>
    public string Name => InnerProject.Category;

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

    /// <inheritdoc/>
    public Task<IReadOnlyPublisher> GetPublisherAsync(CancellationToken cancellationToken) => InnerProject.GetPublisherAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerUserRoleCollection.GetUsersAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task UpdatePublisherAsync(IReadOnlyPublisher publisher, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var valueCid = await Client.Dag.PutAsync(publisher.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(UpdatePublisherAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new PublisherUpdateEvent(publisher.Id), cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public Task UpdateCategoryAsync(string category, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task AddFeatureAsync(string feature, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var valueCid = await Client.Dag.PutAsync(feature, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(AddFeatureAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new FeatureAddEvent(feature), cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemoveFeatureAsync(string feature, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var valueCid = await Client.Dag.PutAsync(feature, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(RemoveFeatureAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new FeatureRemoveEvent(feature), cancellationToken);

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
                IReadOnlyPublisher publisher = null!; // todo, get from repo
                await ApplyEntryUpdateAsync(streamEntry, updateEvent, new PublisherUpdateEvent(publisher.Id), publisher, cancellationToken);
                break;
            case nameof(UpdateCategoryAsync):
                Guard.IsNotNull(updateEvent.Value);
                var category = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyEntryUpdateAsync(streamEntry, updateEvent, new CategoryUpdateEvent(category), cancellationToken);
                break;
            case nameof(AddFeatureAsync):
                Guard.IsNotNull(updateEvent.Value);
                var addedFeature = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyEntryUpdateAsync(streamEntry, updateEvent, new FeatureAddEvent(addedFeature), cancellationToken);
                break;
            case nameof(RemoveFeatureAsync):
                Guard.IsNotNull(updateEvent.Value);
                var removedFeature = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyEntryUpdateAsync(streamEntry, updateEvent, new FeatureRemoveEvent(removedFeature), cancellationToken);
                break;
            default:
                throw new NotImplementedException();
        };
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, FeatureAddEvent addedFeature, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(AddFeatureAsync));
        var feature = addedFeature.Feature;

        Inner.Features = [.. Inner.Features, feature];
        FeaturesAdded?.Invoke(this, [feature]);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, FeatureRemoveEvent removedFeature, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(RemoveFeatureAsync));
        var feature = removedFeature.Feature;

        Inner.Features = Inner.Features.Where(f => f != feature).ToArray();
        FeaturesRemoved?.Invoke(this, [feature]);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, PublisherUpdateEvent publisherUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdatePublisherAsync));
        var publisherId = publisherUpdate.PublisherId;

        IReadOnlyPublisher publisher = null!; // todo, get from repo

        return ApplyEntryUpdateAsync(streamEntry, updateEvent, publisherUpdate, publisher, cancellationToken);
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, PublisherUpdateEvent publisherUpdate, IReadOnlyPublisher updatedPublisher, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdatePublisherAsync));
        var publisherId = publisherUpdate.PublisherId;

        Inner.Publisher = publisherId;
        PublisherUpdated?.Invoke(this, updatedPublisher);
        return Task.CompletedTask;
    }

    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CategoryUpdateEvent categoryUpdateEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdateCategoryAsync));
        var category = categoryUpdateEvent.Category;

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

        // TODO, needs implementation instead of interface.
        // await InnerUserRoleCollection.ResetEventStreamPositionAsync(cancellationToken);
        // await Dependencies.ResetEventStreamPositionAsync(cancellationToken);
    }

    /// <summary>
    /// Event data for an added feature.
    /// </summary>
    /// <param name="Feature">The feature description added.</param>
    internal record FeatureAddEvent(string Feature);

    /// <summary>
    /// Event data for a removed feature.
    /// </summary>
    /// <param name="Feature">The feature description removed.</param>
    internal record FeatureRemoveEvent(string Feature);

    /// <summary>
    /// Event data for a publisher update.
    /// </summary>
    /// <param name="PublisherId">The Id of the publisher to set on this project.</param>
    internal record PublisherUpdateEvent(string PublisherId);

    /// <summary>
    /// Event data for a category update.
    /// </summary>
    /// <param name="Category">The category to set on this project.</param>
    internal record CategoryUpdateEvent(string Category);
}
