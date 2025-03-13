using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IModifiableEntity" />
public class ModifiableEntity : NomadKuboEventStreamHandler<ValueUpdateEvent>, IDelegable<ReadOnlyEntity>, IModifiableEntity
{
    /// <inheritdoc />
    public required ReadOnlyEntity Inner { get; init; }

    /// <summary>
    /// The connections associated with this entity.
    /// </summary>
    public required IModifiableConnectionsCollection InnerConnections { get; init; }

    /// <summary>
    /// The images associated with this entity.
    /// </summary>
    public required ModifiableImagesCollection InnerImages { get; init; }

    /// <summary>
    /// The links associated with this entity.
    /// </summary>
    public required IModifiableLinksCollection InnerLinks { get; init; }

    /// <inheritdoc />
    public string Name => Inner.Inner.Name;

    /// <inheritdoc />
    public string Description => Inner.Inner.Description;

    /// <inheritdoc />
    public string ExtendedDescription => Inner.Inner.ExtendedDescription;

    /// <inheritdoc />
    public bool? ForgetMe => Inner.Inner.ForgetMe;

    /// <inheritdoc />
    public bool IsUnlisted => Inner.Inner.IsUnlisted;

    /// <inheritdoc />
    public IReadOnlyConnection[] Connections => InnerConnections.Connections;

    /// <inheritdoc />
    public Link[] Links => InnerLinks.Links;

    /// <inheritdoc />
    public event EventHandler<string>? NameUpdated;

    /// <inheritdoc />
    public event EventHandler<string>? DescriptionUpdated;

    /// <inheritdoc />
    public event EventHandler<string>? ExtendedDescriptionUpdated;

    /// <inheritdoc />
    public event EventHandler<bool?>? ForgetMeUpdated;

    /// <inheritdoc />
    public event EventHandler<bool>? IsUnlistedUpdated;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksUpdated;

    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesAdded;

    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesRemoved;

    /// <inheritdoc />
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerImages.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc />
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerConnections.AddConnectionAsync(connection, cancellationToken);

    /// <inheritdoc />
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerConnections.RemoveConnectionAsync(connection, cancellationToken);

    /// <inheritdoc />
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerImages.AddImageAsync(imageFile, cancellationToken);

    /// <inheritdoc />
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerImages.RemoveImageAsync(imageFile, cancellationToken);

    /// <inheritdoc />
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) => InnerLinks.AddLinkAsync(link, cancellationToken);

    /// <inheritdoc />
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken) => InnerLinks.RemoveLinkAsync(link, cancellationToken);

    /// <inheritdoc />
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (eventStreamEntry.EventId)
        {
            case nameof(AddImageAsync):
            {
                await InnerImages.ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, cancellationToken);
                break;
            }
            case nameof(RemoveImageAsync):
            {
                await InnerImages.ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, cancellationToken);
                break;
            }
            case nameof(AddConnectionAsync):
            {
                // TODO: Needs implementation, not interface
                // await InnerConnections.ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, cancellationToken);
                break;
            }
            case nameof(RemoveConnectionAsync):
            {
                // TODO: Needs implementation, not interface
                // await InnerConnections.ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, cancellationToken);
                break;
            }
            case nameof(AddLinkAsync):
            {
                // TODO: Needs implementation, not interface
                // await InnerLinks.ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, cancellationToken);
                break;
            }
            case nameof(RemoveLinkAsync):
            {
                // TODO: Needs implementation, not interface
                // await InnerLinks.ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, cancellationToken);
                break;
            }
            case nameof(UpdateNameAsync):
            {
                Guard.IsNotNull(updateEvent.Value);

                var name = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, new NameUpdateEvent(name), cancellationToken);
                break;
            }
            case nameof(UpdateDescriptionAsync):
            {
                Guard.IsNotNull(updateEvent.Value);

                var description = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, new DescriptionUpdateEvent(description), cancellationToken);
                break;
            }
            case nameof(UpdateExtendedDescriptionAsync):
            {
                Guard.IsNotNull(updateEvent.Value);

                var extendedDescription = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);
                await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, new ExtendedDescriptionUpdateEvent(extendedDescription), cancellationToken);
                break;
            }
            case nameof(UpdateForgetMeStatusAsync):
            {
                Guard.IsNotNull(updateEvent.Value);

                var forgetMe = await Client.Dag.GetAsync<bool?>(updateEvent.Value, cancel: cancellationToken);
                await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, new ForgetMeUpdateEvent(forgetMe), cancellationToken);
                break;
            }
            case nameof(UpdateUnlistedStateAsync):
            {
                Guard.IsNotNull(updateEvent.Value);

                var isUnlisted = await Client.Dag.GetAsync<bool>(updateEvent.Value, cancel: cancellationToken);
                await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, new IsUnlistedUpdateEvent(isUnlisted), cancellationToken);
                break;
            }
        }
    }

    /// <inheritdoc />
    public async Task UpdateNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var valueCid = await Client.Dag.PutAsync(name, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(UpdateNameAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new NameUpdateEvent(name), cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc />
    public async Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var valueCid = await Client.Dag.PutAsync(description, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(UpdateDescriptionAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new DescriptionUpdateEvent(description), cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc />
    public async Task UpdateExtendedDescriptionAsync(string extendedDescription, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var valueCid = await Client.Dag.PutAsync(extendedDescription, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(UpdateExtendedDescriptionAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new ExtendedDescriptionUpdateEvent(extendedDescription), cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc />
    public async Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        DagCid? valueCid = null;
        if (forgetMe is not null)
        {
            Cid cid = await Client.Dag.PutAsync(forgetMe, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
            valueCid = (DagCid)cid;
        }

        var updateEvent = new ValueUpdateEvent(null, valueCid, forgetMe is null);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(UpdateForgetMeStatusAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new ForgetMeUpdateEvent(forgetMe), cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc />
    public async Task UpdateUnlistedStateAsync(bool isUnlisted, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var valueCid = await Client.Dag.PutAsync(isUnlisted, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var updateEvent = new ValueUpdateEvent(null, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: EventStreamHandlerId, eventId: nameof(UpdateUnlistedStateAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new IsUnlistedUpdateEvent(isUnlisted), cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, NameUpdateEvent nameUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdateNameAsync));
        
        Inner.Inner.Name = nameUpdate.Name;
        NameUpdated?.Invoke(this, nameUpdate.Name);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, DescriptionUpdateEvent descriptionUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdateDescriptionAsync));
        var description = descriptionUpdate.Description;
        
        Inner.Inner.Description = description;
        DescriptionUpdated?.Invoke(this, description);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, ExtendedDescriptionUpdateEvent extendedDescriptionUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdateExtendedDescriptionAsync));
        var extendedDescription = extendedDescriptionUpdate.ExtendedDescription;
        
        Inner.Inner.ExtendedDescription = extendedDescription;
        ExtendedDescriptionUpdated?.Invoke(this, extendedDescription);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, ForgetMeUpdateEvent forgetMeUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdateForgetMeStatusAsync));
        var forgetMe = forgetMeUpdate.ForgetMe;
        
        Inner.Inner.ForgetMe = forgetMe;
        ForgetMeUpdated?.Invoke(this, forgetMe);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    internal Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IsUnlistedUpdateEvent isUnlistedUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Guard.IsEqualTo(streamEntry.EventId, nameof(UpdateUnlistedStateAsync));
        var isUnlisted = isUnlistedUpdate.IsUnlisted;
        
        Inner.Inner.IsUnlisted = isUnlisted;
        IsUnlistedUpdated?.Invoke(this, isUnlisted);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        // TODO: Reset inner virtual event stream handlers
        // Connections, Links, Images
        await InnerImages.ResetEventStreamPositionAsync(cancellationToken);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Event data for name updates.
    /// </summary>
    /// <param name="Name">The updated name value.</param>
    internal record NameUpdateEvent(string Name);

    /// <summary>
    /// Event data for description updates.
    /// </summary>
    /// <param name="Description">The updated description value.</param>
    internal record DescriptionUpdateEvent(string Description);

    /// <summary>
    /// Event data for extended description updates.
    /// </summary>
    /// <param name="ExtendedDescription">The updated extended description value.</param>
    internal record ExtendedDescriptionUpdateEvent(string ExtendedDescription);

    /// <summary>
    /// Event data for forget-me updates.
    /// </summary>
    /// <param name="ForgetMe">The updated forget-me value.</param>
    internal record ForgetMeUpdateEvent(bool? ForgetMe);

    /// <summary>
    /// Event data for unlisted state updates.
    /// </summary>
    /// <param name="IsUnlisted">The updated unlisted state value.</param>
    internal record IsUnlistedUpdateEvent(bool IsUnlisted);
}
