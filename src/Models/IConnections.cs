using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of key-value connection pairs.
/// </summary>
public interface IConnections
{
    /// <summary>
    /// Represents data about external application connections.
    /// </summary>
    Connection[] Connections { get; set; }
}

/// <summary>
/// Represents a published collection of key-value connection pairs.
/// </summary>
public record Connection
{
    /// <summary>
    /// A unique identifier for this connection.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// A <see cref="DagCid"/> of the connection value.
    /// </summary>
    public required DagCid Value { get; set; }
}