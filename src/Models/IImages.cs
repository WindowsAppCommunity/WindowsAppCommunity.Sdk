using Ipfs;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of images.
/// </summary>
public interface IImages
{
    /// <summary>
    /// A list of <see cref="Image"/> objects.
    /// </summary>
    Image[] Images { get; set; }
}