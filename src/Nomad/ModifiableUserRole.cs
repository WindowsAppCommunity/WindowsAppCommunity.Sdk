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
    public IReadOnlyConnection[] Connections => InnerUser.Connections;

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
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken)
    {
        return InnerUser.AddConnectionAsync(connection, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken)
    {
        return InnerUser.AddImageAsync(imageFile, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken)
    {
        return InnerUser.AddLinkAsync(link, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        return InnerUser.AddProjectAsync(project, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddPublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        return InnerUser.AddPublisherAsync(publisher, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken)
    {
        return InnerUser.GetImageFilesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync(CancellationToken cancellationToken)
    {
        return InnerUser.GetProjectsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync(CancellationToken cancellationToken)
    {
        return InnerUser.GetPublishersAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken)
    {
        return InnerUser.RemoveConnectionAsync(connection, cancellationToken);
    }

    /// <inheritdoc />
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken)
    {
        return InnerUser.RemoveImageAsync(imageFile, cancellationToken);
    }

    /// <inheritdoc />
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken)
    {
        return InnerUser.RemoveLinkAsync(link, cancellationToken);
    }

    /// <inheritdoc />
    public Task RemoveProjectAsync(IReadOnlyProjectRole project, CancellationToken cancellationToken)
    {
        return InnerUser.RemoveProjectAsync(project, cancellationToken);
    }

    /// <inheritdoc />
    public Task RemovePublisherAsync(IReadOnlyPublisherRole publisher, CancellationToken cancellationToken)
    {
        return InnerUser.RemovePublisherAsync(publisher, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken)
    {
        return InnerUser.UpdateDescriptionAsync(description, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateExtendedDescriptionAsync(string extendedDescription, CancellationToken cancellationToken)
    {
        return InnerUser.UpdateExtendedDescriptionAsync(extendedDescription, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken)
    {
        return InnerUser.UpdateForgetMeStatusAsync(forgetMe, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken)
    {
        return InnerUser.UpdateNameAsync(name, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateUnlistedStateAsync(bool isUnlisted, CancellationToken cancellationToken)
    {
        return InnerUser.UpdateUnlistedStateAsync(isUnlisted, cancellationToken);
    }
}
