namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of users with a corresponding role that can be modified.
/// </summary>
public interface IModifiableUserRoleCollection : IModifiableUserCollection<IReadOnlyUserRole>
{
}
