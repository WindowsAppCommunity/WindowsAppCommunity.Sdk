namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a modifiable connections collection.
/// </summary>
public interface IModifiableConnectionsCollection : IReadOnlyConnectionsCollection
{
    /// <summary>
    /// Adds a connection to this entity.
    /// </summary>
    Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a connection from this entity.
    /// </summary>
    Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken);
}
