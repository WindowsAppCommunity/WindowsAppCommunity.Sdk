using System.Collections.Generic;
using OwlCore.ComponentModel;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IReadOnlyEntity" />
public class ReadOnlyEntity : IDelegable<IEntity>, IReadOnlyEntity
{
    /// <summary>
    /// Handles the connections for this entity.
    /// </summary>
    public required IReadOnlyConnectionsCollection InnerConnections { get; init; }

    /// <summary>
    /// Handles the images collection for this entity.
    /// </summary>
    public required IReadOnlyImagesCollection InnerImages { get; init; }

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
    IReadOnlyConnection[] IReadOnlyConnectionsCollection.Connections => InnerConnections.Connections;

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
    public event EventHandler<Link[]>? LinksUpdated;
    
    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesAdded;
    
    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesRemoved;
    
    /// <inheritdoc />
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerImages.GetImageFilesAsync(cancellationToken);
}
