namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents role data for a user, project or publisher.
/// </summary>
public class Role
{
    /// <summary>
    /// The name of the role.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// A description for the role.
    /// </summary>
    public required string Description { get; init; }
}
