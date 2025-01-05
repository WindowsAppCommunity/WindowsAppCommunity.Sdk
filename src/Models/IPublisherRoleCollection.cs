using System.Collections.Generic;
using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of publishers and roles on each.
/// </summary>
public interface IPublisherRoleCollection
{
    /// <summary>
    /// Represents a list of registered publishers along with the role on each.
    /// </summary>
    public Dictionary<DagCid, Role> Publishers { get; set; }
}