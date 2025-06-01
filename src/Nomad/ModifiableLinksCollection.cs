using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IModifiableLinksCollection" />
public class ModifiableLinksCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableLinksCollection, IDelegable<ReadOnlyLinksCollection>
{
    /// <inheritdoc />
    public required ReadOnlyLinksCollection Inner { get; init; }

    /// <summary>
    /// A unique identifier for this instance, persistent across machines and reruns.
    /// </summary>
    public required string Id { get; init; }

    /// <inheritdoc />
    public Link[] Links => Inner.Links;

    /// <inheritdoc />
    public async Task AddLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var newLink = new Models.Link
        {
            Id = link.Id,
            Name = link.Name,
            Description = link.Description,
            Url = link.Url,
        };

        var keyCid = await Client.Dag.PutAsync(link.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(newLink, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent((DagCid)keyCid, (DagCid)valueCid, false);

        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(AddLinkAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, newLink, link, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc />
    public async Task RemoveLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var Link = Inner.Inner.Links.FirstOrDefault(img => img.Id == link.Id);
        if (Link == null)
        {
            throw new ArgumentException("Link not found in the collection.", nameof(link));
        }

        var keyCid = await Client.Dag.PutAsync(Link.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(Link, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent((DagCid)keyCid, (DagCid)valueCid, true);

        var appendedEntry = await AppendNewEntryAsync(Id, nameof(RemoveLinkAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, Link, link, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksAdded;

    /// <inheritdoc />
    public event EventHandler<Link[]>? LinksRemoved;

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <remarks>
    /// This method will call <see cref="ReadOnlyLinksCollection.GetAsync(string, CancellationToken)"/> and create a new instance to pass to the event handlers.
    /// <para/>
    /// If already have a resolved instance of <see cref="Link"/>, you should call <see cref="ApplyEntryUpdateAsync(EventStreamEntry{DagCid}, ValueUpdateEvent, Models.Link, Link, CancellationToken)"/> instead.
    /// </remarks>
    /// <param name="eventStreamEntry">The event stream entry to apply.</param>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (eventStreamEntry.TargetId != Id)
            return;

        Guard.IsNotNull(updateEvent.Value);
        var (link, _) = await Client.ResolveDagCidAsync<Models.Link>(updateEvent.Value.Value, nocache: !KuboOptions.UseCache, cancellationToken);

        Guard.IsNotNull(link);
        await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, link, null, cancellationToken);
    }

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <param name="eventStreamEntry">The event stream entry to apply.</param>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="linkModel">The deserialized Link model data for this event.</param>
    /// <param name="linkAppModel">The runtime application model for this event.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, Models.Link linkModel, Link? linkAppModel = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (eventStreamEntry.EventId)
        {
            case nameof(AddLinkAsync):
                {
                    Inner.Inner.Links = [.. Inner.Inner.Links, linkModel];
                    linkAppModel ??= new Link { Description = linkModel.Description, Id = linkModel.Id, Name = linkModel.Name, Url = linkModel.Url, };
                    LinksAdded?.Invoke(this, [linkAppModel]);
                    break;
                }
            case nameof(RemoveLinkAsync):
                {
                    Inner.Inner.Links = [.. Inner.Inner.Links.Except([linkModel])];
                    linkAppModel ??= new Link { Description = linkModel.Description, Id = linkModel.Id, Name = linkModel.Name, Url = linkModel.Url, };
                    LinksRemoved?.Invoke(this, [linkAppModel]);
                    break;
                }
        }
    }

    /// <inheritdoc />
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        EventStreamPosition = null;
        Inner.Inner.Links = [];

        return Task.CompletedTask;
    }
}
