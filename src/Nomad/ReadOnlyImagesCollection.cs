using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IReadOnlyImagesCollection" />
public class ReadOnlyImagesCollection : IReadOnlyImagesCollection, IDelegable<IImages>, IReadOnlyNomadKuboRegistry<IFile>
{
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
        return Task.FromResult<IFile>(new IdOverriddenIpfsFile(image.Cid, image.Name, image.Id, Client));
    }
    
    /// <inheritdoc/>
    public async IAsyncEnumerable<IFile> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Yield();
        foreach (var image in Inner.Images)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new IdOverriddenIpfsFile(image.Cid, image.Name, image.Id, Client);
        }
    }
}