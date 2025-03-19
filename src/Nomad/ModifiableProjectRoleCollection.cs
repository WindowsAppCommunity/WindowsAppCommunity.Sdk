using System.Collections.Generic;
using System.Linq;
using Ipfs;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A modifiable handler for roaming project collection data.
/// </summary>
public class ModifiableProjectRoleCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableProjectRoleCollection
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required ReadOnlyProjectRoleCollection Inner { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync(CancellationToken cancellationToken) => Inner.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        var valueCid = await Client.Dag.PutAsync(project.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(AddProjectAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyAddProjectRoleEntryAsync(appendedEntry, updateEvent, project, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemoveProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        var valueCid = await Client.Dag.PutAsync(project.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(RemoveProjectAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyRemoveProjectRoleEntryAsync(appendedEntry, updateEvent, project, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        switch (streamEntry.EventId)
        {
            case nameof(AddProjectAsync):
                // TODO: Needs project repository
                IReadOnlyProjectRole addedProjectRole = null!;
                await ApplyAddProjectRoleEntryAsync(streamEntry, updateEvent, addedProjectRole, cancellationToken);
                break;
            case nameof(RemoveProjectAsync):
                // TODO: Needs project repository
                IReadOnlyProjectRole removedProjectRole = null!;
                await ApplyRemoveProjectRoleEntryAsync(streamEntry, updateEvent, removedProjectRole, cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unknown event id: {streamEntry.EventId}");
        }
    }

    /// <inheritdoc/>
    public async Task ApplyAddProjectRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(project.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Projects = [.. Inner.Inner.Projects, (project.Id, (DagCid)roleCid)];
        ProjectsAdded?.Invoke(this, [project]);
    }

    /// <inheritdoc/>
    public async Task ApplyRemoveProjectRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(project.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Projects = [.. Inner.Inner.Projects.Where(x => x.Item1 != project.Id && x.Item2 != (DagCid)roleCid)];
        ProjectsRemoved?.Invoke(this, [project]);
    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Inner.Projects = [];
        return Task.CompletedTask;
    }
}
