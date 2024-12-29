using System.Collections.Generic;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of images.
/// </summary>
public interface IReadOnlyImagesCollection
{
    /// <summary>
    /// Gets the image files in this collection.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Raised when the images in this collection are updated.
    /// </summary>
    public event EventHandler<IFile[]>? ImagesAdded;

    /// <summary>
    /// Raised when the images in this collection are removed.
    /// </summary>
    public event EventHandler<IFile[]>? ImagesRemoved;
}
