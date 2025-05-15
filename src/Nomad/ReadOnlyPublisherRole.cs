using System.Collections.Generic;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A read-only representation of a publisher and the publisher's role.
/// </summary>
public class ReadOnlyPublisherRole : IReadOnlyPublisherRole
{
    /// <summary>
    /// The publisher this role is for.
    /// </summary>
    public required ReadOnlyPublisher InnerPublisher { get; init; }

    /// <summary>
    /// The role this publisher has.
    /// </summary>
    public required Role Role { get; init; }

    /// <inheritdoc/>
    public string Name => InnerPublisher.Name;

    /// <inheritdoc/>
    public string Description => InnerPublisher.Description;

    /// <inheritdoc/>
    public string ExtendedDescription => InnerPublisher.ExtendedDescription;

    /// <inheritdoc/>
    public bool? ForgetMe => InnerPublisher.ForgetMe;

    /// <inheritdoc/>
    public bool IsUnlisted => InnerPublisher.IsUnlisted;

    /// <inheritdoc/>
    public Link[] Links => InnerPublisher.Links;

    /// <inheritdoc/>
    public string Id => InnerPublisher.Id;

    /// <inheritdoc/>
    public IReadOnlyPublisherCollection ParentPublishers => InnerPublisher.ParentPublishers;

    /// <inheritdoc/>
    public IReadOnlyPublisherCollection ChildPublishers => InnerPublisher.ChildPublishers;

    /// <inheritdoc/>
    public string? AccentColor => InnerPublisher.AccentColor;

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

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyConnection> GetConnectionsAsync(CancellationToken cancellationToken = default) => InnerPublisher.GetConnectionsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerPublisher.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken) => InnerPublisher.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerPublisher.GetUsersAsync(cancellationToken);
}
