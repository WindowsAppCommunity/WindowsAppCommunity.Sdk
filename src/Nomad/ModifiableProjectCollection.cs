using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A modifiable handler for roaming project collection data.
/// </summary>
public class ModifiableProjectCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableProjectCollection<IReadOnlyProject>
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required ReadOnlyProjectCollection Inner { get; init; }

    /// <summary>
    /// The repository to use for getting modifiable or readonly project instances.
    /// </summary>
    public required NomadKuboRepository<ModifiableProject, IReadOnlyProject, Project, ValueUpdateEvent> ProjectRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken) => Inner.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddProjectAsync(IReadOnlyProject project, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(project.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)keyCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(AddProjectAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyAddProjectEntryAsync(appendedEntry, updateEvent, project, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemoveProjectAsync(IReadOnlyProject project, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(project.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: null, Value: (DagCid)keyCid, true);

        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(RemoveProjectAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyRemoveProjectEntryAsync(appendedEntry, updateEvent, project, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        switch (streamEntry.EventId)
        {
            case nameof(AddProjectAsync):
                {
                    Guard.IsNotNull(updateEvent.Value);
                    var projectId = await Client.Dag.GetAsync<Cid>(updateEvent.Value, cancel: cancellationToken);
                    var project = await ProjectRepository.GetAsync(projectId, cancellationToken);

                    await ApplyAddProjectEntryAsync(streamEntry, updateEvent, project, cancellationToken);
                }
                break;
            case nameof(RemoveProjectAsync):
                {
                    Guard.IsNotNull(updateEvent.Value);
                    var projectId = await Client.Dag.GetAsync<Cid>(updateEvent.Value, cancel: cancellationToken);
                    var project = await ProjectRepository.GetAsync(projectId, cancellationToken);

                    await ApplyRemoveProjectEntryAsync(streamEntry, updateEvent, project, cancellationToken);
                }
                break;
            default:
                throw new InvalidOperationException($"Unknown event id: {streamEntry.EventId}");
        }
    }

    /// <inheritdoc/>
    public Task ApplyAddProjectEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyProject project, CancellationToken cancellationToken)
    {
        Inner.Inner.Projects = [.. Inner.Inner.Projects, project.Id];
        ProjectsAdded?.Invoke(this, [project]);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ApplyRemoveProjectEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyProject project, CancellationToken cancellationToken)
    {
        Inner.Inner.Projects = [.. Inner.Inner.Projects.Where(id => id != project.Id)];
        ProjectsRemoved?.Invoke(this, [project]);
        return Task.CompletedTask;

    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Inner.Projects = [];
        return Task.CompletedTask;
    }
}
