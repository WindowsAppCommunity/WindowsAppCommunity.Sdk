using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A read only handler for roaming project collection data.
/// </summary>
public class ReadOnlyProjectCollection : IReadOnlyProjectCollection, IDelegable<IProjectCollection>
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required IProjectCollection Inner { get; init; }

    /// <summary>
    /// The client used to interact with the ipfs network.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <inheritdoc />
    public required IKuboOptions KuboOptions { get; set; }

    /// <summary>
    /// The repository used to get and create project instances.
    /// </summary>
    public required NomadKuboRepository<ModifiableProject, IReadOnlyProject, Project, ValueUpdateEvent> ProjectDependencyRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public async IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var id in Inner.Projects)
        {
            // Get read-only or modifiable based on provided repository.
            yield return await ProjectDependencyRepository.GetAsync(id, cancellationToken);
        }
    }
}
