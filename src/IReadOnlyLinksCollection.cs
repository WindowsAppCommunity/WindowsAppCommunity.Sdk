using OwlCore.ComponentModel;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a read-only collection of links.
/// </summary>
public interface IReadOnlyLinksCollection : IHasId
{
    /// <summary>
    /// Represents links to external profiles or resources added by the entity.
    /// </summary>
    Link[] Links { get; }

    /// <summary>
    /// Raised when <see cref="Links"/> are added.
    /// </summary>
    event EventHandler<Link[]>? LinksAdded;  

    /// <summary>
    /// Raised when <see cref="Links"/> are removed.
    /// </summary>
    event EventHandler<Link[]>? LinksRemoved;  
}
