using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of projects.
/// </summary>
public interface IProjectCollection
{
    /// <summary>
    /// Represents a list of registered projects.
    /// </summary>
    Cid[] Projects { get; set; }
}
