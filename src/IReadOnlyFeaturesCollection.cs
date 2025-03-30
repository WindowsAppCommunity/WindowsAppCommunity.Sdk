namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of features that can be read but not modified.
/// </summary>
public interface IReadOnlyFeaturesCollection
{
    /// <summary>
    /// A list of features provided by this project.
    /// </summary>
    public string[] Features { get; }
    
    /// <summary>
    /// Raised when <see cref="Features"/> is updated.
    /// </summary>
    public event EventHandler<string[]>? FeaturesAdded;
    
    /// <summary>
    /// Raised when <see cref="Features"/> is updated.
    /// </summary>
    public event EventHandler<string[]>? FeaturesRemoved;
}