using System.Collections.Generic;
using Ipfs;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of key-value connection pairs.
/// </summary>
public interface IConnections
{
    /// <summary>
    /// Represents data about external application connections.
    /// </summary>
    Dictionary<string, DagCid> Connections { get; set; }
}