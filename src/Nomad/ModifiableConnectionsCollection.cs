using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WindowsAppCommunity.Sdk;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a modifiable connections collection.
/// </summary>
public class ModifiableConnectionsCollection : IModifiableConnectionsCollection
{
    /// <summary>
    /// The connections associated with this entity.
    /// </summary>
    public IReadOnlyConnection[] Connections { get; private set; } = new IReadOnlyConnection[0];

    /// <summary>
    /// Raised when items are added to the <see cref="Connections"/> collection.
    /// </summary>
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;

    /// <summary>
    /// Raised when items are removed from the <see cref="Connections"/> collection.
    /// </summary>
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;

    /// <summary>
    /// The unique identifier for this collection.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The inner read-only connections collection.
    /// </summary>
    public required IReadOnlyConnectionsCollection Inner { get; init; }

    /// <summary>
    /// The roaming key for this collection.
    /// </summary>
    public required IKey RoamingKey { get; init; }

    /// <summary>
    /// The event stream handler ID for this collection.
    /// </summary>
    public required string EventStreamHandlerId { get; init; }

    /// <summary>
    /// The local event stream for this collection.
    /// </summary>
    public required EventStream<DagCid> LocalEventStream { get; init; }

    /// <summary>
    /// The local event stream key for this collection.
    /// </summary>
    public required IKey LocalEventStreamKey { get; init; }

    /// <summary>
    /// The sources for this collection.
    /// </summary>
    public required ICollection<Cid> Sources { get; init; }

    /// <summary>
    /// The Kubo options for this collection.
    /// </summary>
    public required IKuboOptions KuboOptions { get; init; }

    /// <summary>
    /// The IPFS client for this collection.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <summary>
    /// Adds a connection to this entity.
    /// </summary>
    public async Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken)
    {
        var connectionsList = new List<IReadOnlyConnection>(Connections) { connection };
        Connections = connectionsList.ToArray();
        ConnectionsAdded?.Invoke(this, new[] { connection });
        await Task.CompletedTask;
    }

    /// <summary>
    /// Removes a connection from this entity.
    /// </summary>
    public async Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken)
    {
        var connectionsList = new List<IReadOnlyConnection>(Connections);
        connectionsList.Remove(connection);
        Connections = connectionsList.ToArray();
        ConnectionsRemoved?.Invoke(this, new[] { connection });
        await Task.CompletedTask;
    }
}
