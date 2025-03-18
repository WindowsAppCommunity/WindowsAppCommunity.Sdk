using System.Collections.Generic;
using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk;
using WindowsAppCommunity.Sdk.Models;
using WindowsAppCommunity.Sdk.Nomad;
using Link = WindowsAppCommunity.Sdk.Link;

/// <summary>
/// A modifiable event stream handler for modifying roaming publisher data.
/// </summary>
public class ModifiablePublisher : NomadKuboEventStreamHandler<ValueUpdateEvent>, IDelegable<Project>, IModifiablePublisher
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <summary>
    /// The read only handler for this project.
    /// </summary>
    public required ReadOnlyPublisher InnerPublisher { get; init; }

    /// <summary>
    /// The read only entity handler for this publisher.
    /// </summary>
    public required ModifiableEntity InnerEntity { get; init; }

    /// <summary>
    /// The read only accent color handler for this publisher.
    /// </summary>
    public required ModifiableAccentColor InnerAccentColor { get; init; }

    /// <summary>
    /// The modifiable project collection handler for this publisher.
    /// </summary>
    public required IModifiableProjectCollection<IReadOnlyProject> InnerProjectCollection { get; init; }

    /// <summary>
    /// The read only user role handler for this publisher.
    /// </summary>
    public required IModifiableUserRoleCollection InnerUserRoleCollection { get; init; }

    /// <summary>
    /// The roaming project data that this handler modifies.
    /// </summary>
    public required Project Inner { get; init; }

    /// <inheritdoc/>
    public required IModifiablePublisherCollection<IReadOnlyPublisher> ParentPublishers { get; init; }

    /// <inheritdoc/>
    public required IModifiablePublisherCollection<IReadOnlyPublisher> ChildPublishers { get; init; }

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
    public string? AccentColor => InnerAccentColor.AccentColor;

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
    public Task AddConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerEntity.AddConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerEntity.AddImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken) => InnerEntity.AddLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task AddUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerUserRoleCollection.AddUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => InnerEntity.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken) => InnerProjectCollection.GetProjectsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyUserRole> GetUsersAsync(CancellationToken cancellationToken) => InnerUserRoleCollection.GetUsersAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveConnectionAsync(IReadOnlyConnection connection, CancellationToken cancellationToken) => InnerEntity.RemoveConnectionAsync(connection, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken) => InnerEntity.RemoveImageAsync(imageFile, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken) => InnerEntity.RemoveLinkAsync(link, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveUserAsync(IReadOnlyUserRole user, CancellationToken cancellationToken) => InnerUserRoleCollection.RemoveUserAsync(user, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken) => InnerAccentColor.UpdateAccentColorAsync(accentColor, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken) => InnerEntity.UpdateDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateExtendedDescriptionAsync(string extendedDescription, CancellationToken cancellationToken) => InnerEntity.UpdateExtendedDescriptionAsync(extendedDescription, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken) => InnerEntity.UpdateForgetMeStatusAsync(forgetMe, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken) => InnerEntity.UpdateNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task UpdateUnlistedStateAsync(bool isUnlisted, CancellationToken cancellationToken) => InnerEntity.UpdateUnlistedStateAsync(isUnlisted, cancellationToken);

    /// <inheritdoc/>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> streamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        switch (streamEntry.EventId)
        {
            case nameof(UpdateNameAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateDescriptionAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateExtendedDescriptionAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateForgetMeStatusAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateUnlistedStateAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddConnectionAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveConnectionAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddLinkAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveLinkAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddImageAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveImageAsync):
                await InnerEntity.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(UpdateAccentColorAsync):
                await InnerAccentColor.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(AddUserAsync):
                // TODO: Needs implementation instead of interface
                // await InnerUserRoleCollection.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case nameof(RemoveUserAsync):
                // TODO: Needs implementation instead of interface
                // await InnerUserRoleCollection.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case $"{nameof(ParentPublishers)}.{nameof(IModifiablePublisherCollection<IReadOnlyPublisher>.AddPublisherAsync)}":
                // TODO: Needs implementation instead of interface
                // await ParentPublishers.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case $"{nameof(ParentPublishers)}.{nameof(IModifiablePublisherCollection<IReadOnlyPublisher>.RemovePublisherAsync)}":
                // TODO: Needs implementation instead of interface
                // await ParentPublishers.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case $"{nameof(ChildPublishers)}.{nameof(IModifiablePublisherCollection<IReadOnlyPublisher>.AddPublisherAsync)}":
                // TODO: Needs implementation instead of interface
                // await ChildPublishers.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            case $"{nameof(ChildPublishers)}.{nameof(IModifiablePublisherCollection<IReadOnlyPublisher>.RemovePublisherAsync)}":
                // TODO: Needs implementation instead of interface
                // await ChildPublishers.ApplyEntryUpdateAsync(streamEntry, updateEvent, cancellationToken);
                break;
            default:
                throw new NotImplementedException();
        }
        ;
    }

    /// <inheritdoc/>
    public override async Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        EventStreamPosition = null;
        await InnerEntity.ResetEventStreamPositionAsync(cancellationToken);
        await InnerAccentColor.ResetEventStreamPositionAsync(cancellationToken);

        // TODO, needs implementation instead of interface.
        // await InnerUserRoleCollection.ResetEventStreamPositionAsync(cancellationToken);
        // await Dependencies.ResetEventStreamPositionAsync(cancellationToken);
    }
}