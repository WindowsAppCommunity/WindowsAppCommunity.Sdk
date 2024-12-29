namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents the data for a link.
/// </summary>
public class Link
{
    /// <summary>
    /// The uri this link points to.
    /// </summary>
    public required string Uri { get; set; }

    /// <summary>
    /// A display name for this link.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A description of this link (for accessibility or display).
    /// </summary>
    public required string Description { get; set; }
}
