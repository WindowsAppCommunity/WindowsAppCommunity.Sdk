namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of features that can be modified.
/// </summary>
public interface IModifiableFeaturesCollection : IReadOnlyFeaturesCollection
{
    /// <summary>
    /// Adds a feature to the collection.
    /// </summary>
    /// <param name="feature">The feature to add.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    Task AddFeatureAsync(string feature, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a feature from the collection.
    /// </summary>
    /// <param name="feature">The feature to remove.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    Task RemoveFeatureAsync(string feature, CancellationToken cancellationToken);
}
