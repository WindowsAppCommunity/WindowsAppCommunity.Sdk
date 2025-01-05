namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of users with a corresponding role that cannot be modified.
/// </summary>
public interface IReadOnlyUserRoleCollection : IReadOnlyUserCollection<IReadOnlyUserRole>
{
}

/// <summary>
/// Represents a collection of users with a corresponding role that cannot be modified.
/// </summary>
public interface IReadOnlyUserRoleCollection<TUserRoleCollection> : IReadOnlyUserCollection<TUserRoleCollection>
    where TUserRoleCollection : IReadOnlyUserRole
{
}
