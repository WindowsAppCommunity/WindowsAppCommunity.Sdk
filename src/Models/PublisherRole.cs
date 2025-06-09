using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// A container for a Publisher and a corresponding role.
/// </summary>
public record PublisherRole
{
    /// <summary>
    /// The publisher ID.
    /// </summary>
    public required Cid PublisherId { get; init; }

    /// <summary>
    /// The role of the publisher.
    /// </summary>
    public required DagCid Role { get; init; }
}