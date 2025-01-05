namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a modifiable connections collection.
/// </summary>
public interface IReadOnlyConnectionsCollection
{
    /// <summary>
    /// The connections associated with this entity.
    /// </summary>
    IReadOnlyConnection[] Connections { get; }

    /// <summary>
    /// Raised when items are added to the <see cref="Connections"/> collection.
    /// </summary>
    event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;

    /// <summary>
    /// Raised when items are removed from the <see cref="Connections"/> collection.
    /// </summary>
    event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;
}
