using System.Collections.Generic;
using OwlCore.ComponentModel;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A read only handler for roaming user/role collection data.
/// </summary>
public class ReadOnlyUserRoleCollection : IReadOnlyUserRoleCollection, IDelegable<IUserRoleCollection>
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required IUserRoleCollection Inner { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyUserRole[]>? UsersAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyUserRole[]>? UsersRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken)
    {
        // TODO: Needs user repository
        throw new NotImplementedException();
    }
}
