namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents role data for a user, project or publisher.
/// </summary>
public class Role
{
    /// <summary>
    /// A unique identifier for this Role.
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// The name of the role.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// A description for the role.
    /// </summary>
    public required string Description { get; init; }
}
