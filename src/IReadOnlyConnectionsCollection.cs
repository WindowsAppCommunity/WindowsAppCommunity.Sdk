using System.Collections.Generic;
using OwlCore.ComponentModel;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a modifiable connections collection.
/// </summary>
public interface IReadOnlyConnectionsCollection : IHasId
{
    /// <summary>
    /// Gets the connections associated with this entity.
    /// </summary>
    IAsyncEnumerable<IReadOnlyConnection> GetConnectionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when items are added to the collection.
    /// </summary>
    event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;

    /// <summary>
    /// Raised when items are removed from the collection.
    /// </summary>
    event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;
}
