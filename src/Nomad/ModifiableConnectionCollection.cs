using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a modifiable connection collection.
/// </summary>
public class ModifiableConnectionCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableConnectionsCollection, IDelegable<ReadOnlyConnectionCollection>
{
    /// <summary>
    /// Raised when items are added to the collection.
    /// </summary>
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;

    /// <summary>
    /// Raised when items are removed from the collection.
    /// </summary>
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;

    /// <summary>
    /// The unique identifier for this collection.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The inner read-only connections collection.
    /// </summary>
    public required ReadOnlyConnectionCollection Inner { get; init; }

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyConnection> GetConnectionsAsync(CancellationToken cancellationToken = default) => Inner.GetConnectionsAsync(cancellationToken);

    /// <summary>
    /// Adds a connection to this entity.
    /// </summary>
    public async Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken)
    {
        var value = await connection.GetValueAsync(cancellationToken);

        var keyCid = (DagCid)await Client.Dag.PutAsync(connection.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = (DagCid)await Client.Dag.PutAsync(value, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(keyCid, valueCid, Unset:false);

        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(AddConnectionAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new Connection { Id = connection.Id, Value = valueCid }, connection, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <summary>
    /// Removes a connection from this entity.
    /// </summary>
    public async Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken)
    {
        var existing = Inner.Inner.Connections.FirstOrDefault(x => x.Id == connection.Id);
        if (existing == null)
            throw new ArgumentException("Connection not found in the collection.", nameof(connection));

        var value = await connection.GetValueAsync(cancellationToken);

        var keyCid = (DagCid)await Client.Dag.PutAsync(connection.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = (DagCid)await Client.Dag.PutAsync(value, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(keyCid, valueCid, Unset: true);

        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(RemoveConnectionAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, new Connection { Id = connection.Id, Value = valueCid }, connection, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <remarks>
    /// This method will call <see cref="ReadOnlyConnectionCollection.GetAsync(string, CancellationToken)"/> and create a new instance to pass to the event handlers.
    /// <para/>
    /// If already have a resolved instance of <see cref="Connection"/>, you should call <see cref="ApplyEntryUpdateAsync(EventStreamEntry{DagCid}, ValueUpdateEvent, Connection, IReadOnlyConnection?, CancellationToken)"/> instead.
    /// </remarks>
    /// <param name="eventStreamEntry">The event stream entry to apply.</param>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (eventStreamEntry.TargetId != Id)
            return;

        Guard.IsNotNull(updateEvent.Value);
        var (connection, _) = await Client.ResolveDagCidAsync<Connection>(updateEvent.Value.Value, nocache: !KuboOptions.UseCache, cancellationToken);

        Guard.IsNotNull(connection);
        await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, connection, null, cancellationToken);
    }

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <param name="eventStreamEntry">The event stream entry to apply.</param>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="connection">The resolved connection data for this event.</param>
    /// <param name="connectionInstance">The existing instance of <see cref="IReadOnlyConnection"/> to emit on raised collection add/remove events.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, Connection connection, IReadOnlyConnection? connectionInstance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (eventStreamEntry.EventId)
        {
            case nameof(AddConnectionAsync):
                {
                    Inner.Inner.Connections = [.. Inner.Inner.Connections, connection];
                    var connectionFile = connectionInstance ??= await Inner.GetAsync(connection.Id, cancellationToken);
                    ConnectionsAdded?.Invoke(this, [connectionFile]);
                    break;
                }
            case nameof(RemoveConnectionAsync):
                {
                    Inner.Inner.Connections = [.. Inner.Inner.Connections.Except([connection])];
                    var connectionFile = connectionInstance ??= await Inner.GetAsync(connection.Id, cancellationToken);
                    ConnectionsRemoved?.Invoke(this, [connectionFile]);
                    break;
                }
        }
    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // TODO: Reset inner virtual event stream handlers
        // Connections, Links, Images
        throw new NotImplementedException();
    }
}
