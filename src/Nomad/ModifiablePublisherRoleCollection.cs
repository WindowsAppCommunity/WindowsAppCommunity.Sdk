using System.Collections.Generic;
using System.Linq;
using Ipfs;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;

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

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync(CancellationToken cancellationToken) => Inner.GetPublishersAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddPublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        var valueCid = await Client.Dag.PutAsync(publisher.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(AddPublisherAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyAddPublisherRoleEntryAsync(appendedEntry, updateEvent, publisher, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemovePublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        var valueCid = await Client.Dag.PutAsync(publisher.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(RemovePublisherAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyRemovePublisherRoleEntryAsync(appendedEntry, updateEvent, publisher, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        switch (streamEntry.EventId)
        {
            case nameof(AddPublisherAsync):
                // TODO: Needs publisher repository
                IReadOnlyPublisherRole addedPublisherRole = null!;
                await ApplyAddPublisherRoleEntryAsync(streamEntry, updateEvent, addedPublisherRole, cancellationToken);
                break;
            case nameof(RemovePublisherAsync):
                // TODO: Needs publisher repository
                IReadOnlyPublisherRole removedPublisherRole = null!;
                await ApplyRemovePublisherRoleEntryAsync(streamEntry, updateEvent, removedPublisherRole, cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unknown event id: {streamEntry.EventId}");
        }
    }

    /// <inheritdoc/>
    public async Task ApplyAddPublisherRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(publisher.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Publishers = [.. Inner.Inner.Publishers, (publisher.Id, (DagCid)roleCid)];
        PublishersAdded?.Invoke(this, [publisher]);
    }

    /// <inheritdoc/>
    public async Task ApplyRemovePublisherRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(publisher.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Publishers = [.. Inner.Inner.Publishers.Where(x => x.Item1 != publisher.Id && x.Item2 != (DagCid)roleCid)];
        PublishersRemoved?.Invoke(this, [publisher]);
    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Inner.Publishers = [];
        return Task.CompletedTask;
    }
}
