using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IReadOnlyImagesCollection" />
public class ReadOnlyImagesCollection : IReadOnlyImagesCollection, IDelegable<IImages>, IReadOnlyNomadKuboRegistry<IFile>
{
    /// <inheritdoc />
    public required string Id { get; init; }

    /// <summary>
    /// The client to use for communicating with ipfs.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <inheritdoc/>
    public required IImages Inner { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ImagesAdded;

    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ImagesRemoved;

    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ItemsAdded
    {
        add => ImagesAdded += value;
        remove => ImagesAdded -= value;
    }

    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ItemsRemoved
    {
        add => ImagesRemoved += value;
        remove => ImagesRemoved -= value;
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => GetAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<IFile> GetAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var image = Inner.Images.First(image => image.Id == id);
        return Task.FromResult(ImageToFile(image));
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<IFile> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Yield();
        foreach (var image in Inner.Images)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return ImageToFile(image);
        }
    }

    /// <summary>
    /// Create an <see cref="IFile"/> from an <see cref="Image"/>.
    /// </summary>
    /// <param name="image">The image to convert.</param>
    /// <returns>The created <see cref="IFile"/>.</returns>
    public IFile ImageToFile(Image image)
    {
        return new NameIdOverriddenGetCidFile<IpfsFile>
        {
            Id = image.Id,
            Name = image.Name,
            Inner = new IpfsFile(image.Cid, image.Name, Client)
        };
    }
}