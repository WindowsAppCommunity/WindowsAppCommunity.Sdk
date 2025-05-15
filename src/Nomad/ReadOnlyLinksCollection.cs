using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IReadOnlyLinksCollection" />
public class ReadOnlyLinksCollection : IReadOnlyLinksCollection, IDelegable<ILinkCollection>, IReadOnlyNomadKuboRegistry<Link>
{
    /// <summary>
    /// The client to use for communicating with ipfs.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <inheritdoc/>
    public required ILinkCollection Inner { get; init; }

    /// <summary>
    /// The links in this collection.
    /// </summary>
    public Link[] Links => Inner.Links.Select(link => new Link
    {
        Id = link.Id,
        Url = link.Url,
        Name = link.Name,
        Description = link.Description,
    }).ToArray();

    /// <inheritdoc/>
    public event EventHandler<Link[]>? LinksAdded;

    /// <inheritdoc/>
    public event EventHandler<Link[]>? LinksRemoved;

    /// <inheritdoc/>
    public event EventHandler<Link[]>? ItemsAdded
    {
        add => LinksAdded += value;
        remove => LinksAdded -= value;
    }

    /// <inheritdoc/>
    public event EventHandler<Link[]>? ItemsRemoved
    {
        add => LinksRemoved += value;
        remove => LinksRemoved -= value;
    }

    /// <inheritdoc/>
    public Task<Link> GetAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var link = Inner.Links.First(link => link.Id == id);
        return Task.FromResult(new Link
        {
            Id = link.Id,
            Name = link.Name,
            Url = link.Url,
            Description = link.Description
        });
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<Link> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Yield();
        foreach (var link in Inner.Links)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new Link
            {
                Id = link.Id,
                Name = link.Name,
                Url = link.Url,
                Description = link.Description
            };
        }
    }
}