using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A read only handler for roaming project/role collection data.
/// </summary>
public class ReadOnlyProjectRoleCollection : IReadOnlyProjectRoleCollection, IDelegable<IProjectRoleCollection>
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required IProjectRoleCollection Inner { get; init; }
    
    /// <summary>
    /// The client to use for communicating with IPFS.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <summary>
    /// The repository to use for getting modifiable or readonly project instances.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiableProject, IReadOnlyProject> ProjectRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public async IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var projectRole in Inner.Projects)
        {
            var role = await Client.Dag.GetAsync<Role>(projectRole.Role, cancel: cancellationToken);
            var project = await ProjectRepository.GetAsync(projectRole.ProjectId, cancellationToken);

            yield return new ReadOnlyProjectRole
            {
                InnerProject = (project as ModifiableProject)?.InnerProject ?? (ReadOnlyProject)project,
                Role = role,
            };
        }
    }
}
