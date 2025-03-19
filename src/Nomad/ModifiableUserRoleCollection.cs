using System.Collections.Generic;
using System.Linq;
using Ipfs;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;

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

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyUserRole[]>? UsersAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyUserRole[]>? UsersRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => Inner.GetUsersAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken)
    {
        var valueCid = await Client.Dag.PutAsync(user.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(AddUserAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyAddUserRoleEntryAsync(appendedEntry, updateEvent, user, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemoveUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken)
    {
        var valueCid = await Client.Dag.PutAsync(user.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(RemoveUserAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyRemoveUserRoleEntryAsync(appendedEntry, updateEvent, user, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        switch (streamEntry.EventId)
        {
            case nameof(AddUserAsync):
                // TODO: Needs user repository
                IReadOnlyUserRole addedUserRole = null!;
                await ApplyAddUserRoleEntryAsync(streamEntry, updateEvent, addedUserRole, cancellationToken);
                break;
            case nameof(RemoveUserAsync):
                // TODO: Needs user repository
                IReadOnlyUserRole removedUserRole = null!;
                await ApplyRemoveUserRoleEntryAsync(streamEntry, updateEvent, removedUserRole, cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unknown event id: {streamEntry.EventId}");
        }
    }

    /// <inheritdoc/>
    public async Task ApplyAddUserRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyUserRole user, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(user.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Users = [.. Inner.Inner.Users, (user.Id, (DagCid)roleCid)];
        UsersAdded?.Invoke(this, [user]);
    }

    /// <inheritdoc/>
    public async Task ApplyRemoveUserRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyUserRole user, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(user.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Users = [.. Inner.Inner.Users.Where(x => x.Item1 != user.Id && x.Item2 != (DagCid)roleCid)];
        UsersRemoved?.Invoke(this, [user]);
    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Inner.Users = [];
        return Task.CompletedTask;
    }
}
