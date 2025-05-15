using System.Collections.Generic;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a user with a corresponding role that can be modified.
/// </summary>
public class ModifiableUserRole : IModifiableUserRole
{
    /// <inheritdoc />
    public required ModifiableUser InnerUser { get; init; }

    /// <inheritdoc />
    public required Role Role { get; init; }

    /// <inheritdoc />
    public string Name => InnerUser.Name;

    /// <inheritdoc />
    public string Description => InnerUser.Description;

    /// <inheritdoc />
    public string ExtendedDescription => InnerUser.ExtendedDescription;

    /// <inheritdoc />
    public bool? ForgetMe => InnerUser.ForgetMe;

    /// <inheritdoc />
    public bool IsUnlisted => InnerUser.IsUnlisted;

    /// <inheritdoc />
    public Link[] Links => InnerUser.Links;

    /// <inheritdoc />
    public string Id => InnerUser.Id;

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

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksRemoved;

    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesAdded;

    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesRemoved;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersAdded;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersRemoved;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsAdded;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsRemoved;

    /// <inheritdoc />
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerUser.AddConnectionAsync(connection, cancellationToken);

    /// <inheritdoc />
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerUser.AddImageAsync(imageFile, cancellationToken);

    /// <inheritdoc />
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) => InnerUser.AddLinkAsync(link, cancellationToken);

    /// <inheritdoc />
    public Task AddProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken) => InnerUser.AddProjectAsync(project, cancellationToken);

    /// <inheritdoc />
    public Task AddPublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken) => InnerUser.AddPublisherAsync(publisher, cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyConnection> GetConnectionsAsync(CancellationToken cancellationToken = default) => InnerUser.GetConnectionsAsync(cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerUser.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync(CancellationToken cancellationToken) => InnerUser.GetProjectsAsync(cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync(CancellationToken cancellationToken) => InnerUser.GetPublishersAsync(cancellationToken);

    /// <inheritdoc />
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerUser.RemoveConnectionAsync(connection, cancellationToken);

    /// <inheritdoc />
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerUser.RemoveImageAsync(imageFile, cancellationToken);

    /// <inheritdoc />
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken) => InnerUser.RemoveLinkAsync(link, cancellationToken);

    /// <inheritdoc />
    public Task RemoveProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken) => InnerUser.RemoveProjectAsync(project, cancellationToken);

    /// <inheritdoc />
    public Task RemovePublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken) => InnerUser.RemovePublisherAsync(publisher, cancellationToken);

    /// <inheritdoc />
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken) => InnerUser.UpdateDescriptionAsync(description, cancellationToken);

    /// <inheritdoc />
    public Task UpdateExtendedDescriptionAsync(string extendedDescription, CancellationToken cancellationToken) => InnerUser.UpdateExtendedDescriptionAsync(extendedDescription, cancellationToken);

    /// <inheritdoc />
    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken) => InnerUser.UpdateForgetMeStatusAsync(forgetMe, cancellationToken);

    /// <inheritdoc />
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken) => InnerUser.UpdateNameAsync(name, cancellationToken);

    /// <inheritdoc />
    public Task UpdateUnlistedStateAsync(bool isUnlisted, CancellationToken cancellationToken) => InnerUser.UpdateUnlistedStateAsync(isUnlisted, cancellationToken);
}
