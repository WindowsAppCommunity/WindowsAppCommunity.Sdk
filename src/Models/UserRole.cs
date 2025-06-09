using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// A container for a user and a corresponding role.
/// </summary>
public record UserRole
{
    /// <summary>
    /// The user ID.
    /// </summary>
    public required Cid UserId { get; init; }

    /// <summary>
    /// The role of the user.
    /// </summary>
    public required DagCid Role { get; init; } 
}