using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of publishers.
/// </summary>
public record PublisherCollection : IPublisherCollection
{
    /// <summary>
    /// Represents a list of registered publishers.
    /// </summary>
    public Cid[] Publishers { get; set; } = [];
}