using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents the data for a link.
/// </summary>
public record Link : IStorable
{
    /// <summary>
    /// A unique identifier for this Link that is persistent across runs and property updates.
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// The external url this link points to.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// A display name for this link.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A description of this link (for accessibility or display).
    /// </summary>
    public required string Description { get; set; }
}