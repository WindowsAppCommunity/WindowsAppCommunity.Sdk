using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IModifiableAccentColor" />
public class ModifiableAccentColor : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableAccentColor, IDelegable<ReadOnlyAccentColor>
{
    /// <inheritdoc />
    public required ReadOnlyAccentColor Inner { get; init; }

    /// <summary>
    /// A unique identifier for this instance, persistent across machines and reruns.
    /// </summary>
    public required string Id { get; init; }

    /// <inheritdoc />
    public string? AccentColor => Inner.AccentColor;

    /// <inheritdoc />
    public event EventHandler<string?>? AccentColorUpdated;

    /// <inheritdoc />
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (eventStreamEntry.TargetId != Id)
            return;

        string? accentColor = null;

        if (updateEvent is { Unset: false, Value: not null })
        {
            (accentColor, _) = await Client.ResolveDagCidAsync<string>(updateEvent.Value, nocache: !KuboOptions.UseCache, cancellationToken);
        }

        await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, accentColor, cancellationToken);
    }

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <param name="eventStreamEntry">The event stream entry to apply.</param>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="accentColor">The resolved accent color data for this event.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, string? accentColor, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (eventStreamEntry.EventId is not nameof(UpdateAccentColorAsync))
            return Task.CompletedTask;

        Inner.Inner.AccentColor = accentColor;
        AccentColorUpdated?.Invoke(this, accentColor);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        EventStreamPosition = null;
        Inner.Inner.AccentColor = null;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken)
    {
        DagCid? valueCid = null;
        if (accentColor is not null)
        {
            Cid cid = await Client.Dag.PutAsync(accentColor, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
            valueCid = (DagCid)cid;
        }

        var updateEvent = new ValueUpdateEvent(null, valueCid, accentColor is null);

        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(UpdateAccentColorAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, accentColor, cancellationToken);

        EventStreamPosition = appendedEntry;
    }
}