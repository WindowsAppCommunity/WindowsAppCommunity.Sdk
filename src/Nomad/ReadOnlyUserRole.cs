using System.Collections.Generic;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A read-only representation of a user and the user's role.
/// </summary>
public class ReadOnlyUserRole : IReadOnlyUserRole
{
    /// <summary>
    /// The user this role is for.
    /// </summary>
    public required ReadOnlyUser InnerUser { get; init; }

    /// <summary>
    /// The role this user has.
    /// </summary>
    public required Role Role { get; init; }

    /// <inheritdoc/>
    public string Name => InnerUser.Name;

    /// <inheritdoc/>
    public string Description => InnerUser.Description;

    /// <inheritdoc/>
    public string ExtendedDescription => InnerUser.ExtendedDescription;

    /// <inheritdoc/>
    public bool? ForgetMe => InnerUser.ForgetMe;

    /// <inheritdoc/>
    public bool IsUnlisted => InnerUser.IsUnlisted;

    /// <inheritdoc/>
    public IReadOnlyConnection[] Connections => InnerUser.Connections;

    /// <inheritdoc/>
    public Link[] Links => InnerUser.Links;

    /// <inheritdoc/>
    public string Id => InnerUser.Id;

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
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersRemoved;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyProjectRole[]>? ProjectsRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken)
    {
        return InnerUser.GetImageFilesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProjectRole> GetProjectsAsync(CancellationToken cancellationToken)
    {
        return InnerUser.GetProjectsAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync(CancellationToken cancellationToken)
    {
        return InnerUser.GetPublishersAsync(cancellationToken);
    }
}
