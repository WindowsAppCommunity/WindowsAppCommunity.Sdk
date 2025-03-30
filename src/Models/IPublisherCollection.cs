using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of publishers.
/// </summary>
public interface IPublisherCollection
{
    /// <summary>
    /// Represents a list of registered publishers.
    /// </summary>
    Cid[] Publishers { get; set; }
}