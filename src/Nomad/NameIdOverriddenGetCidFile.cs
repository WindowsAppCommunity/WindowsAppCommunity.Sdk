using System.IO;
using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A file with an overridden ID and name.
/// </summary>
/// <typeparam name="TInner">The inner file type to delegate to.</typeparam>
public class NameIdOverriddenGetCidFile<TInner> : IFile, IDelegable<TInner>, IGetCid
    where TInner : class, IFile, IGetCid
{
    /// <inheritdoc />
    public required string Id { get; init; }

    /// <inheritdoc />
    public required TInner Inner { get; init; }

    /// <inheritdoc />
    public required string Name { get; init; }

    /// <inheritdoc />
    public Task<Cid> GetCidAsync(CancellationToken cancellationToken)
    {
        return Inner.GetCidAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<Stream> OpenStreamAsync(FileAccess accessMode, CancellationToken cancellationToken = default)
    {
        return Inner.OpenStreamAsync(accessMode, cancellationToken);
    }
}