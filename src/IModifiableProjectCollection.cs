namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a modifiable collection of projects.
/// </summary>
public interface IModifiableProjectCollection<TProject> : IReadOnlyProjectCollection<TProject>
    where TProject : IReadOnlyProject
{
    /// <summary>
    /// Adds a project to the collection.
    /// </summary>
    Task AddProjectAsync(TProject project, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a project from the collection.
    /// </summary>
    Task RemoveProjectAsync(TProject project, CancellationToken cancellationToken);
}
