namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a user with a corresponding role that can be modified.
/// </summary>
public interface IModifiableUserRole : IReadOnlyUserRole, IModifiableUser
{
}
