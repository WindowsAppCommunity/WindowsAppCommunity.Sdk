using System.Collections.Generic;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a project and role that cannot be modified.
/// </summary>
public class ReadOnlyProjectRole : IReadOnlyProjectRole
{
    /// <summary>
    /// The project this role is for.
    /// </summary>
    public required ReadOnlyProject InnerProject { get; init; }

    /// <summary>
    /// The role this project has.
    /// </summary>
    public required Role Role { get; init; }

    /// <inheritdoc/>
    public IReadOnlyProjectCollection Dependencies => InnerProject.Dependencies;

    /// <inheritdoc/>
    public string Category => InnerProject.Category;

    /// <inheritdoc/>
    public string Name => InnerProject.Name;

    /// <inheritdoc/>
    public string Description => InnerProject.Description;

    /// <inheritdoc/>
    public string ExtendedDescription => InnerProject.ExtendedDescription;

    /// <inheritdoc/>
    public bool? ForgetMe => InnerProject.ForgetMe;

    /// <inheritdoc/>
    public bool IsUnlisted => InnerProject.IsUnlisted;

    /// <inheritdoc/>
    public IReadOnlyConnection[] Connections => InnerProject.Connections;

    /// <inheritdoc/>
    public Link[] Links => InnerProject.Links;

    /// <inheritdoc/>
    public string? AccentColor => InnerProject.AccentColor;

    /// <inheritdoc/>
    public string[] Features => InnerProject.Features;

    /// <inheritdoc/>
    public string Id => InnerProject.Id;

    /// <inheritdoc/>
    public event EventHandler<string>? CategoryUpdated;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisher>? PublisherUpdated;
    
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
    public event EventHandler<string[]>? FeaturesAdded;
    
    /// <inheritdoc/>
    public event EventHandler<string[]>? FeaturesRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerProject.GetImageFilesAsync(cancellationToken);


    /// <inheritdoc/>
    public Task<IReadOnlyPublisher?> GetPublisherAsync(CancellationToken cancellationToken) => InnerProject.GetPublisherAsync(cancellationToken);


    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerProject.GetUsersAsync(cancellationToken);
}
