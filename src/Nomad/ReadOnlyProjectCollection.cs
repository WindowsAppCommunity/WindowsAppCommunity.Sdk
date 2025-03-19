using System.Collections.Generic;
using OwlCore.ComponentModel;
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

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken)
    {
        // TODO: Needs project repository
        throw new NotImplementedException();
    }
}
