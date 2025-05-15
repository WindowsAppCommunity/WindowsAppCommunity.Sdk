using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of projects and roles on each.
/// </summary>
public interface IProjectRoleCollection
{
    /// <summary>
    /// Represents a list of registered projects along with the role on each.
    /// </summary>
    ProjectRole[] Projects { get; set; }
}

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