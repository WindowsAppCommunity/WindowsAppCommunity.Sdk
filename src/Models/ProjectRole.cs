using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// A container for a project and a corresponding role.
/// </summary>
public record ProjectRole
{
    /// <summary>
    /// The project ID.
    /// </summary>
    public required Cid ProjectId { get; init; }

    /// <summary>
    /// The role of the project.
    /// </summary>
    public required DagCid Role { get; init; }
}