using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of images that can be modified.
/// </summary>
public interface IModifiableImagesCollection : IReadOnlyImagesCollection
{
    /// <summary>
    /// Adds an image to the collection.
    /// </summary>
    /// <param name="imageFile">The image file to add to the collection.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken);

    /// <summary>
    /// Removes an image from the collection.
    /// </summary>
    /// <param name="imageFile">The image file to remove from the collection.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken);
}
