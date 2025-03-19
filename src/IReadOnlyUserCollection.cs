using System.Collections.Generic;
using OwlCore.ComponentModel;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of users.
/// </summary>
public interface IReadOnlyUserCollection : IReadOnlyUserCollection<IReadOnlyUser>
{
}

/// <summary>
/// Represents a collection of users with a corresponding role that can be modified.
/// </summary>
/// <typeparam name="TUser">The type of user in this collection.</typeparam>
public interface IReadOnlyUserCollection<out TUser> : IHasId
    where TUser : IReadOnlyUser
{
    /// <summary>
    /// Get the users in this collection.
    /// </summary>
    public IAsyncEnumerable<TUser> GetUsersAsync(CancellationToken cancellationToken);
}
