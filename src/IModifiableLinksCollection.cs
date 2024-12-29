namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a modifiable collection of connections.
/// </summary>
public interface IModifiableLinksCollection : IReadOnlyLinksCollection
{
    /// <summary>
    /// Adds a link to this entity.
    /// </summary>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a link from this entity.
    /// </summary>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken);
}
