namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of publishers that can be modified.
/// </summary>
public interface IModifiablePublisherCollection<TPublisher> : IReadOnlyPublisherCollection<TPublisher>
    where TPublisher : IReadOnlyPublisher
{
    /// <summary>
    /// Adds a publisher to the collection.
    /// </summary>
    public Task AddPublisherAsync(TPublisher publisher, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a publisher from the collection.
    /// </summary>
    public Task RemovePublisherAsync(TPublisher publisher, CancellationToken cancellationToken);
}
