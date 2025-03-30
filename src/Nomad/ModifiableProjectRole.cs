using System.Collections.Generic;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a project with a corresponding role that can be modified.
/// </summary>
public class ModifiableProjectRole : IModifiableProjectRole
{
    /// <summary>
    /// The modifiable project this role is for.
    /// </summary>
    public required ModifiableProject InnerProject { get; init; }

    /// <summary>
    /// The role this project has.
    /// </summary>
    public required Role Role { get; init; }

    /// <inheritdoc/>
    public IModifiableProjectCollection<IReadOnlyProject> Dependencies => InnerProject.Dependencies;

    /// <inheritdoc/>
    IReadOnlyProjectCollection IReadOnlyProject<IReadOnlyProjectCollection>.Dependencies => (IReadOnlyProjectCollection)InnerProject.Dependencies;

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
    public string Id => InnerProject.Id;

    /// <inheritdoc/>
    public string? AccentColor => InnerProject.AccentColor;

    /// <inheritdoc/>
    public string[] Features => InnerProject.Features;

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
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerProject.AddConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task AddFeatureAsync(string feature, CancellationToken cancellationToken) => InnerProject.AddFeatureAsync(feature, cancellationToken);

    /// <inheritdoc/>
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerProject.AddImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) => InnerProject.AddLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task AddUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerProject.AddUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerProject.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyPublisher> GetPublisherAsync(CancellationToken cancellationToken) => InnerProject.GetPublisherAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerProject.GetUsersAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerProject.RemoveConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveFeatureAsync(string feature, CancellationToken cancellationToken) => InnerProject.RemoveFeatureAsync(feature, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerProject.RemoveImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken) => InnerProject.RemoveLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerProject.RemoveUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken) => InnerProject.UpdateAccentColorAsync(accentColor, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateCategoryAsync(string category, CancellationToken cancellationToken) => InnerProject.UpdateCategoryAsync(category, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken) => InnerProject.UpdateDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateExtendedDescriptionAsync(string extendedDescription, CancellationToken cancellationToken) => InnerProject.UpdateExtendedDescriptionAsync(extendedDescription, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken) => InnerProject.UpdateForgetMeStatusAsync(forgetMe, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken) => InnerProject.UpdateNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task UpdatePublisherAsync(IReadOnlyPublisher publisher, CancellationToken cancellationToken) => InnerProject.UpdatePublisherAsync(publisher, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateUnlistedStateAsync(bool isUnlisted, CancellationToken cancellationToken) => InnerProject.UpdateUnlistedStateAsync(isUnlisted, cancellationToken);
}
