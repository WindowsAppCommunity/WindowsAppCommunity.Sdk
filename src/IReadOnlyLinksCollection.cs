namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a read-only collection of links.
/// </summary>
public interface IReadOnlyLinksCollection
{
    /// <summary>
    /// Represents links to external profiles or resources added by the entity.
    /// </summary>
    Link[] Links { get; }

    /// <summary>
    /// Raised when <see cref="Links"/> is updated.
    /// </summary>
    event EventHandler<Link[]>? LinksUpdated;  
}
