using System.Collections.Generic;
using OwlCore.ComponentModel;
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

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync(CancellationToken cancellationToken)
    {
        // TODO: Needs project repository
        throw new NotImplementedException();
    }
}
