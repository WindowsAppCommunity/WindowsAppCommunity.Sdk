using System.Collections.Generic;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of projects.
/// </summary>
public interface IReadOnlyProjectCollection : IReadOnlyProjectCollection<IReadOnlyProject>
{
}

/// <summary>
/// Represents a collection of projects.
/// </summary>
public interface IReadOnlyProjectCollection<TProject>
    where TProject : IReadOnlyProject
{
    /// <summary>
    /// Enumerates the projects in this collection.
    /// </summary>
    IAsyncEnumerable<TProject> GetProjectsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Raised when projects are added to this collection.
    /// </summary>
    event EventHandler<TProject[]>? ProjectsAdded;

    /// <summary>
    /// Raised when projects are removed from this collection.
    /// </summary>
    event EventHandler<TProject[]>? ProjectsRemoved;
}
