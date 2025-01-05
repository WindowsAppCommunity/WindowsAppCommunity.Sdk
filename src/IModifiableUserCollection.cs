namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of users with a corresponding role that can be modified.
/// </summary>
public interface IModifiableUserCollection<TUser> : IReadOnlyUserCollection<TUser>
    where TUser : IReadOnlyUser
{
    /// <summary>
    /// Adds a user to this collection along with their role.
    /// </summary>
    public Task AddUserAsync(TUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the user from this collection.
    /// </summary>
    public Task RemoveUserAsync(TUser user, CancellationToken cancellationToken);
}
