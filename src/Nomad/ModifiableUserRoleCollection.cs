using System.Collections.Generic;
using System.Linq;
using Ipfs;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using CommunityToolkit.Diagnostics;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A modifiable handler for roaming user collection data.
/// </summary>
public class ModifiableUserRoleCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableUserRoleCollection
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required ReadOnlyUserRoleCollection Inner { get; init; }

    /// <summary>
    /// The repository to use for getting modifiable or readonly user instances.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiableUser, IReadOnlyUser> UserRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyUserRole[]>? UsersAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyUserRole[]>? UsersRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => Inner.GetUsersAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(user.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(user.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: (DagCid)keyCid, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(AddUserAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyAddUserRoleEntryAsync(appendedEntry, updateEvent, user, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemoveUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(user.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(user.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: (DagCid)keyCid, Value: (DagCid)valueCid, true);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(RemoveUserAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyRemoveUserRoleEntryAsync(appendedEntry, updateEvent, user, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        if (updateEvent.Key == null || updateEvent.Value == null)
        {
            throw new ArgumentNullException("Key or Value in updateEvent cannot be null.");
        }

        switch (streamEntry.EventId)
        {
            case nameof(AddUserAsync):
                var userId = await Client.Dag.GetAsync<string>(updateEvent.Key, cancel: cancellationToken);
                var user = await UserRepository.GetAsync(userId, cancellationToken);
                Guard.IsNotNull(user);

                var role = await Client.Dag.GetAsync<Role>(updateEvent.Value, cancel: cancellationToken);
                Guard.IsNotNull(role);

                if (user is ModifiableUser modifiableUser)
                {
                    ModifiableUserRole addedUserRole = new ModifiableUserRole
                    {
                        InnerUser = modifiableUser,
                        Role = role
                    };

                    await ApplyAddUserRoleEntryAsync(streamEntry, updateEvent, addedUserRole, cancellationToken);
                }
                else if (user is ReadOnlyUser readOnlyUser)
                {
                    IReadOnlyUserRole addedUserRole = new ReadOnlyUserRole
                    {
                        InnerUser = readOnlyUser,
                        Role = role
                    };

                    await ApplyAddUserRoleEntryAsync(streamEntry, updateEvent, addedUserRole, cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException("User is of an unsupported type.");
                }

                break;
            case nameof(RemoveUserAsync):
                var removedUserId = await Client.Dag.GetAsync<string>(updateEvent.Key, cancel: cancellationToken);
                var removedUser = await UserRepository.GetAsync(removedUserId, cancellationToken);
                Guard.IsNotNull(removedUser);

                var removedRole = await Client.Dag.GetAsync<Role>(updateEvent.Value, cancel: cancellationToken);
                Guard.IsNotNull(removedRole);

                if (removedUser is ModifiableUser modifiableRemovedUser)
                {
                    ModifiableUserRole removedUserRole = new ModifiableUserRole
                    {
                        InnerUser = modifiableRemovedUser,
                        Role = removedRole
                    };

                    await ApplyRemoveUserRoleEntryAsync(streamEntry, updateEvent, removedUserRole, cancellationToken);
                }
                else if (removedUser is ReadOnlyUser readOnlyRemovedUser)
                {
                    IReadOnlyUserRole removedUserRole = new ReadOnlyUserRole
                    {
                        InnerUser = readOnlyRemovedUser,
                        Role = removedRole
                    };

                    await ApplyRemoveUserRoleEntryAsync(streamEntry, updateEvent, removedUserRole, cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException("User is of an unsupported type.");
                }

                break;
            default:
                throw new InvalidOperationException($"Unknown event id: {streamEntry.EventId}");
        }
    }

    /// <inheritdoc/>
    public async Task ApplyAddUserRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyUserRole user, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(user.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Users = [.. Inner.Inner.Users, new UserRole { UserId = user.Id, Role = (DagCid)roleCid }];
        UsersAdded?.Invoke(this, [user]);
    }

    /// <inheritdoc/>
    public async Task ApplyRemoveUserRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyUserRole user, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(user.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Users = [.. Inner.Inner.Users.Where(x => x.UserId != user.Id && x.Role != (DagCid)roleCid)];
        UsersRemoved?.Invoke(this, [user]);
    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Inner.Users = [];
        return Task.CompletedTask;
    }
}
