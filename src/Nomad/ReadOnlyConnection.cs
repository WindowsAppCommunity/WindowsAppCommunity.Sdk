using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a read-only connection.
/// </summary>
public class ReadOnlyConnection : IReadOnlyConnection, IDelegable<Connection>
{
    /// <inheritdoc />
    public string Id => Inner.Id;

    /// <summary>
    /// The client to use for communicating with ipfs.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <summary>
    /// The roaming value for this connection.
    /// </summary>
    public required Connection Inner { get; init; }

    /// <inheritdoc />
    public event EventHandler<string>? ValueUpdated;

    /// <inheritdoc/>
    public Task<string> GetValueAsync(CancellationToken cancellationToken = default)
    {
        return Client.Dag.GetAsync<string>(Inner.Value, cancel: cancellationToken);
    }
}
