using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
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

    /// <summary>
    /// The client to use for communicating with IPFS.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <summary>
    /// The repository to use for getting modifiable or readonly user instances.
    /// </summary>
    public required NomadKuboRepository<ModifiableUser, IReadOnlyUser, User, ValueUpdateEvent> UserRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyUserRole[]>? UsersAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyUserRole[]>? UsersRemoved;

    /// <inheritdoc/>
    public async IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var userRole in Inner.Users)
        {
            var role = await Client.Dag.GetAsync<Role>(userRole.RoleCid, cancel: cancellationToken);
            var user = await UserRepository.GetAsync(userRole.UserId, cancellationToken);

            yield return new ReadOnlyUserRole
            {
                InnerUser = (user as ModifiableUser)?.InnerUser ?? (ReadOnlyUser)user,
                Role = role,
            };
        }
    }
}
