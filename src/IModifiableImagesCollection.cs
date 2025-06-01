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
    /// Adds an image to the collection with a specified ID and name.
    /// </summary>
    /// <remarks>
    /// If the ID is not provided, the image's own ID will be used. If the name is not provided, the image's own name will be used.
    /// </remarks>
    /// <param name="imageFile">The image file to add to the collection.</param>
    /// <param name="id">An optional ID to assign to the image.</param>
    /// <param name="name">An optional name to assign to the image.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    Task AddImageAsync(IFile imageFile, string? id, string? name, CancellationToken cancellationToken);

    /// <summary>
    /// Removes an image from the collection.
    /// </summary>
    /// <param name="imageId">The ID of the image file to remove from the collection.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    Task RemoveImageAsync(string imageId, CancellationToken cancellationToken);
}
