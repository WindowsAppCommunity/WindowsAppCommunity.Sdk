namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a user with a corresponding role that cannot be modified.
/// </summary>
public interface IReadOnlyUserRole : IReadOnlyUser
{
    /// <summary>
    /// Gets the role for this user.
    /// </summary>
    public Role Role { get; }
}
