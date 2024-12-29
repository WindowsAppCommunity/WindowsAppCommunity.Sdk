using System.Collections.Generic;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of publishers that can be read but not modified.
/// </summary>
public interface IReadOnlyPublisherCollection : IReadOnlyPublisherCollection<IReadOnlyPublisher>
{
} 

/// <summary>
/// Represents a collection of publishers that can be read but not modified.
/// </summary>
public interface IReadOnlyPublisherCollection<TPublisher>
    where TPublisher : IReadOnlyPublisher
{
    /// <summary>
    /// Gets the publishers in this collection.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<TPublisher> GetPublishersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Raised when publishers are added to the collection.
    /// </summary>
    public event EventHandler<TPublisher[]>? PublishersAdded;
    
    /// <summary>
    /// Raised when publishers are removed from the collection.
    /// </summary>
    public event EventHandler<TPublisher[]>? PublishersRemoved;
}
