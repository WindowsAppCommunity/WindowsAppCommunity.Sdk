using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A modifiable handler for roaming publisher collection data.
/// </summary>
public class ModifiablePublisherCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiablePublisherCollection<IReadOnlyPublisher>, IReadOnlyPublisherCollection
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <summary>
    /// The read-only handler for this publisher collection.
    /// </summary>
    public required ReadOnlyPublisherCollection Inner { get; init; }

    /// <summary>
    /// A unique identifier for add events, persistent across machines and reruns.
    /// </summary>
    public string AddPublisherEventId { get; init; } = nameof(AddPublisherAsync);

    /// <summary>
    /// A unique identifier for remove events, persistent across machines and reruns.
    /// </summary>
    public string RemovePublisherEventId { get; init; } = nameof(RemovePublisherAsync);

    /// <summary>
    /// The repository to use for getting modifiable or readonly publisher instances.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> PublisherRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisher[]>? PublishersAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisher[]>? PublishersRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyPublisher> GetPublishersAsync(CancellationToken cancellationToken) => Inner.GetPublishersAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddPublisherAsync(IReadOnlyPublisher publisher, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(publisher.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)keyCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: AddPublisherEventId, updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyAddPublisherEntryAsync(appendedEntry, updateEvent, publisher, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemovePublisherAsync(IReadOnlyPublisher publisher, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(publisher.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)keyCid, true);

        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: RemovePublisherEventId, updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyRemovePublisherEntryAsync(appendedEntry, updateEvent, publisher, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        if (streamEntry.EventId == AddPublisherEventId)
        {
            Guard.IsNotNull(updateEvent.Value);
            var publisherId = await Client.Dag.GetAsync<Cid>(updateEvent.Value, cancel: cancellationToken);
            var publisher = await PublisherRepository.GetAsync(publisherId, cancellationToken);

            await ApplyAddPublisherEntryAsync(streamEntry, updateEvent, publisher, cancellationToken);
        }
        else if (streamEntry.EventId == RemovePublisherEventId)
        {
            Guard.IsNotNull(updateEvent.Value);
            var publisherId = await Client.Dag.GetAsync<Cid>(updateEvent.Value, cancel: cancellationToken);
            var publisher = await PublisherRepository.GetAsync(publisherId, cancellationToken);

            await ApplyRemovePublisherEntryAsync(streamEntry, updateEvent, publisher, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"Unknown event id: {streamEntry.EventId}");
        }
    }

    /// <inheritdoc/>
    public Task ApplyAddPublisherEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyPublisher publisher, CancellationToken cancellationToken)
    {
        Inner.Inner.Publishers = [.. Inner.Inner.Publishers, publisher.Id];
        PublishersAdded?.Invoke(this, [publisher]);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ApplyRemovePublisherEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyPublisher publisher, CancellationToken cancellationToken)
    {
        Inner.Inner.Publishers = [.. Inner.Inner.Publishers.Where(id => id != publisher.Id)];
        PublishersRemoved?.Invoke(this, [publisher]);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Inner.Publishers = [];
        return Task.CompletedTask;
    }
}
