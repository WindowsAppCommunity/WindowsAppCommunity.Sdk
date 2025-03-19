using OwlCore.ComponentModel;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a project.
/// </summary>
public interface IReadOnlyProject : IReadOnlyProject<IReadOnlyProjectCollection>
{
}

/// <summary>
/// Represents a project.
/// </summary>
public interface IReadOnlyProject<out TDependencyCollection> : IReadOnlyEntity, IReadOnlyImagesCollection, IReadOnlyUserRoleCollection, IReadOnlyAccentColor, IReadOnlyFeaturesCollection, IHasId
    where TDependencyCollection : IReadOnlyProjectCollection<IReadOnlyProject>
{
    /// <summary>
    /// The projects that this project depends on.
    /// </summary>
    public TDependencyCollection Dependencies { get; }

    /// <summary>
    /// The category defining this project, as found in an app store.
    /// </summary>
    public string Category { get; }
    
    /// <summary>
    /// Raised when <see cref="Category"/> is updated.
    /// </summary>
    public event EventHandler<string>? CategoryUpdated;

    /// <summary>
    /// Raised when <see cref="Dependencies"/> are updated. 
    /// </summary>
    public event EventHandler<IReadOnlyPublisher>? PublisherUpdated;

    /// <summary>
    /// Gets the publisher for this project.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IReadOnlyPublisher> GetPublisherAsync(CancellationToken cancellationToken);
}
