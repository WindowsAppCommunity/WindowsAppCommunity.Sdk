using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a read-only collection of connections.
/// </summary>
public class ReadOnlyConnectionsCollection : IReadOnlyConnectionsCollection
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
    public IReadOnlyConnection[] Connections => GetConnections();

    /// <inheritdoc />
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;

    private IReadOnlyConnection[] GetConnections()
    {
        var connections = new List<IReadOnlyConnection>();

        foreach (var kvp in Inner.Connections)
        {
            connections.Add(new ReadOnlyConnection
            {
                Id = kvp.Key,
                Value = kvp.Value.ToString()
            });
        }

        return connections.ToArray();
    }

    /// <summary>
    /// Handles the addition of connections.
    /// </summary>
    /// <param name="connections">The connections to add.</param>
    public void OnConnectionsAdded(IReadOnlyConnection[] connections)
    {
        ConnectionsAdded?.Invoke(this, connections);
    }

    /// <summary>
    /// Handles the removal of connections.
    /// </summary>
    /// <param name="connections">The connections to remove.</param>
    public void OnConnectionsRemoved(IReadOnlyConnection[] connections)
    {
        ConnectionsRemoved?.Invoke(this, connections);
    }
}