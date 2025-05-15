using System.Collections.Generic;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IReadOnlyEntity" />
public class ReadOnlyEntity : IDelegable<IEntity>, IReadOnlyEntity
{
    /// <inheritdoc/>
    public required string Id { get; init; }
    
    /// <summary>
    /// The client to use for communicating with ipfs.
    /// </summary>
    public required ICoreApi Client { get; init; }
    
    /// <summary>
    /// Handles the connections for this entity.
    /// </summary>
    public required ReadOnlyConnectionCollection InnerConnections { get; init; }

    /// <summary>
    /// Handles the images collection for this entity.
    /// </summary>
    public required ReadOnlyImagesCollection InnerImages { get; init; }

    /// <summary>
    /// Handles the links collection for this entity.
    /// </summary>
    public required IReadOnlyLinksCollection InnerLinks { get; init; }

    /// <inheritdoc />
    public required IEntity Inner { get; init; }

    /// <inheritdoc />
    public string Name => Inner.Name;

    /// <inheritdoc />
    public string Description => Inner.Description;

    /// <inheritdoc />
    public string ExtendedDescription => Inner.ExtendedDescription;

    /// <inheritdoc />
    public bool? ForgetMe => Inner.ForgetMe;

    /// <inheritdoc />
    public bool IsUnlisted => Inner.IsUnlisted;

    /// <inheritdoc />
    public Link[] Links => InnerLinks.Links;

    /// <inheritdoc />
    public event EventHandler<string>? NameUpdated;

    /// <inheritdoc />
    public event EventHandler<string>? DescriptionUpdated;

    /// <inheritdoc />
    public event EventHandler<string>? ExtendedDescriptionUpdated;

    /// <inheritdoc />
    public event EventHandler<bool?>? ForgetMeUpdated;

    /// <inheritdoc />
    public event EventHandler<bool>? IsUnlistedUpdated;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksAdded;

    /// <inheritdoc/>
    public event EventHandler<Link[]>? LinksRemoved;

    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesAdded;

    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesRemoved;

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyConnection> GetConnectionsAsync(CancellationToken cancellationToken = default) => InnerConnections.GetConnectionsAsync(cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerImages.GetImageFilesAsync(cancellationToken);
}
