using System.Collections.Generic;
using System.Linq;
using Ipfs;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A modifiable handler for roaming publisher collection data.
/// </summary>
public class ModifiablePublisherRoleCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiablePublisherRoleCollection
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required ReadOnlyPublisherRoleCollection Inner { get; init; }

    /// <summary>
    /// A unique identifier for add events, persistent across machines and reruns.
    /// </summary>
    public string AddPublisherRoleEventId { get; init; } = "AddPublisherRoleAsync";

    /// <summary>
    /// A unique identifier for remove events, persistent across machines and reruns.
    /// </summary>
    public string RemovePublisherRoleEventId { get; init; } = "RemovePublisherRoleAsync";

    /// <summary>
    /// The repository to use for getting modifiable or readonly publisher instances.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> PublisherRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync(CancellationToken cancellationToken) => Inner.GetPublishersAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddPublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(publisher.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(publisher.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: (DagCid)keyCid, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: "AddPublisherRoleAsync", updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyAddPublisherRoleEntryAsync(appendedEntry, updateEvent, publisher, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemovePublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(publisher.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(publisher.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: (DagCid)keyCid, Value: (DagCid)valueCid, true);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: "RemovePublisherRoleAsync", updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyRemovePublisherRoleEntryAsync(appendedEntry, updateEvent, publisher, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        if (updateEvent.Key == null || updateEvent.Value == null)
        {
            throw new ArgumentNullException("Key or Value in updateEvent cannot be null.");
        }

        if (streamEntry.EventId == AddPublisherRoleEventId)
        {
            var publisherId = await Client.Dag.GetAsync<string>(updateEvent.Key, cancel: cancellationToken);
            var publisher = await PublisherRepository.GetAsync(publisherId, cancellationToken);
            if (publisher is ModifiablePublisher modifiablePublisher)
            {
                var publisherRole = await Client.Dag.GetAsync<Role>(updateEvent.Value, cancel: cancellationToken);
                var addedPublisherRole = new ModifiablePublisherRole
                {
                    InnerPublisher = modifiablePublisher,
                    Role = publisherRole
                };

                await ApplyAddPublisherRoleEntryAsync(streamEntry, updateEvent, addedPublisherRole, cancellationToken);
            }
            else if (publisher is ReadOnlyPublisher readOnlyPublisher)
            {
                var publisherRole = await Client.Dag.GetAsync<Role>(updateEvent.Value, cancel: cancellationToken);
                var addedPublisherRole = new ReadOnlyPublisherRole
                {
                    InnerPublisher = readOnlyPublisher,
                    Role = publisherRole
                };

                await ApplyAddPublisherRoleEntryAsync(streamEntry, updateEvent, addedPublisherRole, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException("Publisher is of an unsupported type.");
            }

            return;
        }

        if (streamEntry.EventId == RemovePublisherRoleEventId)
        {
            var removedPublisherId = await Client.Dag.GetAsync<string>(updateEvent.Key, cancel: cancellationToken);
            var removedPublisher = await PublisherRepository.GetAsync(removedPublisherId, cancellationToken);
            if (removedPublisher is ModifiablePublisher modifiableRemovedPublisher)
            {
                var removedPublisherRole = await Client.Dag.GetAsync<Role>(updateEvent.Value, cancel: cancellationToken);
                var removedPublisherRoleInstance = new ModifiablePublisherRole
                {
                    InnerPublisher = modifiableRemovedPublisher,
                    Role = removedPublisherRole
                };

                await ApplyRemovePublisherRoleEntryAsync(streamEntry, updateEvent, removedPublisherRoleInstance, cancellationToken);
            }
            else if (removedPublisher is ReadOnlyPublisher readOnlyRemovedPublisher)
            {
                var removedPublisherRole = await Client.Dag.GetAsync<Role>(updateEvent.Value, cancel: cancellationToken);
                var removedPublisherRoleInstance = new ReadOnlyPublisherRole
                {
                    InnerPublisher = readOnlyRemovedPublisher,
                    Role = removedPublisherRole
                };

                await ApplyRemovePublisherRoleEntryAsync(streamEntry, updateEvent, removedPublisherRoleInstance, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException("Publisher is of an unsupported type.");
            }
            return;
        }

        throw new InvalidOperationException($"Unknown event id: {streamEntry.EventId}");
    }

    /// <inheritdoc/>
    public async Task ApplyAddPublisherRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(publisher.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner = [.. Inner.Inner, new PublisherRole { PublisherId = publisher.Id, Role = (DagCid)roleCid }];
        PublishersAdded?.Invoke(this, [publisher]);
    }

    /// <inheritdoc/>
    public async Task ApplyRemovePublisherRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(publisher.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner = [.. Inner.Inner.Where(x => x.PublisherId != publisher.Id && x.Role != (DagCid)roleCid)];
        PublishersRemoved?.Invoke(this, [publisher]);
    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Inner = [];
        return Task.CompletedTask;
    }
}
