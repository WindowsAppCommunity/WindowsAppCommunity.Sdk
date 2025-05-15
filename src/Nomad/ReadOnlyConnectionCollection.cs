using System.Collections.Generic;
using System.Linq;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a read-only collection of connections.
/// </summary>
public class ReadOnlyConnectionCollection : IReadOnlyConnectionsCollection, IDelegable<IConnections>, IReadOnlyNomadKuboRegistry<IReadOnlyConnection>
{
    /// <summary>
    /// The client to use for communicating with ipfs.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <summary>
    /// The connections associated with this collection.
    /// </summary>
    public required IConnections Inner { get; init; }

    /// <inheritdoc />
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyConnection[]>? ItemsAdded
    {
        add => ConnectionsRemoved += value;
        remove => ConnectionsRemoved -= value;
    }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyConnection[]>? ItemsRemoved
    {
        add => ConnectionsRemoved += value;
        remove => ConnectionsRemoved -= value;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyConnection> GetAsync(string id, CancellationToken cancellationToken)
    {
        var existingConnection = Inner.Connections.FirstOrDefault(c => c.Id == id);
        if (existingConnection == null)
            throw new KeyNotFoundException($"No connection found with ID {id}");

        return Task.FromResult<IReadOnlyConnection>(new ReadOnlyConnection
        {
            Inner = existingConnection,
            Client = Client,
        });
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyConnection> GetAsync(CancellationToken cancellationToken) => GetConnectionsAsync(cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyConnection> GetConnectionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Inner.Connections
            .Select(x => new ReadOnlyConnection
            {
                Inner = x,
                Client = Client,
            })
            .Cast<IReadOnlyConnection>()
            .ToAsyncEnumerable();
    }
}