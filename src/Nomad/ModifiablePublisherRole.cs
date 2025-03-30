using System.Collections.Generic;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a publisher and role that can be modified.
/// </summary>
public class ModifiablePublisherRole : IModifiablePublisherRole
{
    /// <summary>
    /// The modifiable publisher this role is for.
    /// </summary>
    public required ModifiablePublisher InnerPublisher { get; init; }

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
    public IReadOnlyConnection[] Connections => InnerPublisher.Connections;

    /// <inheritdoc/>
    public Link[] Links => InnerPublisher.Links;

    /// <inheritdoc/>
    public string Id => InnerPublisher.Id;

    /// <inheritdoc/>
    public string? AccentColor => InnerPublisher.AccentColor;

    /// <inheritdoc/>
    public IModifiablePublisherCollection<IReadOnlyPublisher> ParentPublishers => InnerPublisher.ParentPublishers;

    /// <inheritdoc/>
    public IModifiablePublisherCollection<IReadOnlyPublisher> ChildPublishers => InnerPublisher.ChildPublishers;

    /// <inheritdoc/>
    IReadOnlyPublisherCollection IReadOnlyPublisher<IReadOnlyPublisherCollection>.ParentPublishers => (IReadOnlyPublisherCollection)InnerPublisher.ParentPublishers;

    /// <inheritdoc/>
    IReadOnlyPublisherCollection IReadOnlyPublisher<IReadOnlyPublisherCollection>.ChildPublishers => (IReadOnlyPublisherCollection)InnerPublisher.ChildPublishers;

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
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerPublisher.AddConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerPublisher.AddImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) => InnerPublisher.AddLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task AddUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerPublisher.AddUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerPublisher.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken) => InnerPublisher.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerPublisher.GetUsersAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerPublisher.RemoveConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerPublisher.RemoveImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken) => InnerPublisher.RemoveLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerPublisher.RemoveUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken) => InnerPublisher.UpdateAccentColorAsync(accentColor, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken) => InnerPublisher.UpdateDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateExtendedDescriptionAsync(string extendedDescription, CancellationToken cancellationToken) => InnerPublisher.UpdateExtendedDescriptionAsync(extendedDescription, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken) => InnerPublisher.UpdateForgetMeStatusAsync(forgetMe, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken) => InnerPublisher.UpdateNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateUnlistedStateAsync(bool isUnlisted, CancellationToken cancellationToken) => InnerPublisher.UpdateUnlistedStateAsync(isUnlisted, cancellationToken);
}