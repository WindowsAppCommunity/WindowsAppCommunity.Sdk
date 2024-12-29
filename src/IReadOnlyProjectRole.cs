namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a project with a corresponding role that cannot be modified.
/// </summary>
public interface IReadOnlyProjectRole : IReadOnlyProject
{
    /// <summary>
    /// Gets the role for this project.
    /// </summary>
    public Role Role { get; }
}
