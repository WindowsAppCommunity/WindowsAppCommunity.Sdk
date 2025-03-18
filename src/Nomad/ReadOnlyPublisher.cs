using System.Collections.Generic;
using OwlCore.ComponentModel;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk;
using WindowsAppCommunity.Sdk.Models;
using WindowsAppCommunity.Sdk.Nomad;
using Link = WindowsAppCommunity.Sdk.Link;

/// <summary>
/// A read-only event stream handler for reading roaming publisher data.
/// </summary>
public class ReadOnlyPublisher : IReadOnlyPublisher, IDelegable<Publisher>
{
    /// <summary>
    /// The read only entity handler for this publisher.
    /// </summary>
    public required ReadOnlyEntity InnerEntity { get; init; }

    /// <summary>
    /// The read only accent color handler for this publisher.
    /// </summary>
    public required IReadOnlyAccentColor InnerAccentColor { get; init; }

    /// <summary>
    /// The read only user role handler for this publisher.
    /// </summary>
    public required IReadOnlyUserRoleCollection InnerUserRoleCollection { get; init; }

    /// <summary>
    /// The handler for reading projects and roles on this publisher.
    /// </summary>
    public required IReadOnlyProjectCollection InnerProjectCollection { get; init; }

    /// <inheritdoc/>
    public required IReadOnlyPublisherCollection ParentPublishers { get; init; }
    
    /// <inheritdoc/>
    public required IReadOnlyPublisherCollection ChildPublishers { get; init; }

    /// <inheritdoc/>
    public required Publisher Inner { get; init; }
    
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public string Name => InnerEntity.Name;

    /// <inheritdoc/>
    public string Description => InnerEntity.Description;

    /// <inheritdoc/>
    public string ExtendedDescription => InnerEntity.ExtendedDescription;

    /// <inheritdoc/>
    public bool? ForgetMe => InnerEntity.ForgetMe;

    /// <inheritdoc/>
    public bool IsUnlisted => InnerEntity.IsUnlisted;

    /// <inheritdoc/>
    public IReadOnlyConnection[] Connections => InnerEntity.Connections;

    /// <inheritdoc/>
    public Link[] Links => InnerEntity.Links;

    /// <inheritdoc/>
    public string? AccentColor => throw new NotImplementedException();

    /// <inheritdoc/>
    public event EventHandler<string>? NameUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<string>? DescriptionUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<string>? ExtendedDescriptionUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<bool?>? ForgetMeUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<bool>? IsUnlistedUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyConnection[]>? ConnectionsRemoved;
    
    /// <inheritdoc/>
    public event EventHandler<Link[]>? LinksAdded;
    
    /// <inheritdoc/>
    public event EventHandler<Link[]>? LinksRemoved;
    
    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ImagesAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IFile[]>? ImagesRemoved;
    
    /// <inheritdoc/>
    public event EventHandler<string?>? AccentColorUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProject[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerEntity.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken) => InnerProjectCollection.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerUserRoleCollection.GetUsersAsync(cancellationToken);
}