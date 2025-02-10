using Ipfs;
using Ipfs.CoreApi;
using OwlCore.Kubo;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A wrapper that overrides the <see cref="IStorable.Id"/> of an <see cref="IpfsFile"/>.
/// </summary>
public class IdOverriddenIpfsFile : IpfsFile
{
    /// <summary>
    /// Creates a new instance of <see cref="IdOverriddenIpfsFile"/>.
    /// </summary>
    public IdOverriddenIpfsFile(Cid cid, string name, string id, ICoreApi client)
        : base(cid, name, client)
    {
        Id = id;
    }
    
    /// <inheritdoc />
    public override string Id { get; }
}