using System.Collections.Generic;
using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a list of registered users along with the role on each.
/// </summary>
public interface IUserRoleCollection
{
    /// <summary>
    /// Represents a list of registered users along with the role on each.
    /// </summary>
    public (Cid, DagCid)[] Users { get; set; }
}