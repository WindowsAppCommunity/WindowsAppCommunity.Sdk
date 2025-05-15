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
public class ModifiableProjectRoleCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableProjectRoleCollection
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required ReadOnlyProjectRoleCollection Inner { get; init; }

    /// <summary>
    /// The repository to use for getting modifiable or readonly project instances.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiableProject, IReadOnlyProject> ProjectRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync(CancellationToken cancellationToken) => Inner.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(project.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(project.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: (DagCid)keyCid, Value: (DagCid)valueCid, false);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: "AddProjectRoleAsync", updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyAddProjectRoleEntryAsync(appendedEntry, updateEvent, project, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public async Task RemoveProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        var keyCid = await Client.Dag.PutAsync(project.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(project.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent(Key: (DagCid)keyCid, Value: (DagCid)valueCid, true);
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: "RemoveProjectRoleAsync", updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyRemoveProjectRoleEntryAsync(appendedEntry, updateEvent, project, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        switch (streamEntry.EventId)
        {
            case  "AddProjectRoleAsync":
                {
                    Guard.IsNotNull(updateEvent.Key);
                    Guard.IsNotNull(updateEvent.Value);
                    var projectId = await Client.Dag.GetAsync<Cid>(updateEvent.Key, cancel: cancellationToken);
                    var role = await Client.Dag.GetAsync<Role>(updateEvent.Value, cancel: cancellationToken);
                    var project = await ProjectRepository.GetAsync(projectId, cancellationToken);

                    var addedProjectRole = new ReadOnlyProjectRole
                    {
                        InnerProject = (project as ModifiableProject)?.InnerProject ?? (ReadOnlyProject)project,
                        Role = role,
                    };

                    await ApplyAddProjectRoleEntryAsync(streamEntry, updateEvent, addedProjectRole, cancellationToken);
                    break;
                }
            case "RemoveProjectRoleAsync":
                {
                    Guard.IsNotNull(updateEvent.Key);
                    Guard.IsNotNull(updateEvent.Value);
                    var projectId = await Client.Dag.GetAsync<Cid>(updateEvent.Key, cancel: cancellationToken);
                    var role = await Client.Dag.GetAsync<Role>(updateEvent.Value, cancel: cancellationToken);
                    var project = await ProjectRepository.GetAsync(projectId, cancellationToken);

                    var removedProjectRole = new ReadOnlyProjectRole
                    {
                        InnerProject = (project as ModifiableProject)?.InnerProject ?? (ReadOnlyProject)project,
                        Role = role,
                    };

                    await ApplyRemoveProjectRoleEntryAsync(streamEntry, updateEvent, removedProjectRole, cancellationToken);
                    break;
                }
            default:
                throw new InvalidOperationException($"Unknown event id: {streamEntry.EventId}");
        }
    }

    /// <inheritdoc/>
    public async Task ApplyAddProjectRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(project.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Projects = [.. Inner.Inner.Projects, new ProjectRole { ProjectId = project.Id, Role = (DagCid)roleCid }];
        ProjectsAdded?.Invoke(this, [project]);
    }

    /// <inheritdoc/>
    public async Task ApplyRemoveProjectRoleEntryAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        var roleCid = await Client.Dag.PutAsync(project.Role, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        Inner.Inner.Projects = [.. Inner.Inner.Projects.Where(x => x.ProjectId != project.Id && x.Role != (DagCid)roleCid)];
        ProjectsRemoved?.Invoke(this, [project]);
    }

    /// <inheritdoc/>
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Inner.Projects = [];
        return Task.CompletedTask;
    }
}
