using Ipfs;
using OwlCore.Storage;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents image metadata.
/// </summary>
public record Image : IStorable
{
    /// <summary>
    /// A unique identifier for this Image that is persistent across runs and property updates.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// A display name for this image.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// A content identifier of a file with this image's content.
    /// </summary>
    public required DagCid Cid { get; init; }
}