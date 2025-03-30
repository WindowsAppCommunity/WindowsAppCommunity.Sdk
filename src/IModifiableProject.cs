namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a project that can be modified.
/// </summary>
public interface IModifiableProject : IReadOnlyProject, IReadOnlyProject<IModifiableProjectCollection<IReadOnlyProject>>, IModifiableEntity, IModifiableImagesCollection, IModifiableUserRoleCollection, IModifiableAccentColor, IModifiableFeaturesCollection
{
    /// <summary>
    /// Updates the publisher for this project.
    /// </summary>
    /// <param name="publisher">The new publisher for this project.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task UpdatePublisherAsync(IReadOnlyPublisher publisher, CancellationToken cancellationToken);

    /// <summary>
    /// Update the category that describes this project.
    /// </summary>
    public Task UpdateCategoryAsync(string category, CancellationToken cancellationToken);
}
