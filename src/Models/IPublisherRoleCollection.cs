using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of publishers and roles on each.
/// </summary>
public interface IPublisherRoleCollection
{
    /// <summary>
    /// Represents a list of registered publishers along with the role on each.
    /// </summary>
    public PublisherRole[] Publishers { get; set; }
}

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