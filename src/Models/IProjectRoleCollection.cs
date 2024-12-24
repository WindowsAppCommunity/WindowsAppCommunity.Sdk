using System.Collections.Generic;
using Ipfs;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of projects and roles on each.
/// </summary>
public interface IProjectRoleCollection
{
    /// <summary>
    /// Represents a list of registered projects along with the role on each.
    /// </summary>
    Dictionary<DagCid, Role> Projects { get; set; }
}