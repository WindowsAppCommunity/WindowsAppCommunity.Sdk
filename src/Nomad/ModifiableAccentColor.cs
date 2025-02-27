using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using WindowsAppCommunity.Sdk.Models;

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
    public override async Task ApplyEntryUpdateAsync(ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (updateEvent.TargetId != Id)
            return;

        string? accentColor = null;
        
        if (updateEvent is { Unset: false, Value: not null })
        {
            (accentColor, _) = await Client.ResolveDagCidAsync<string>(updateEvent.Value!,  nocache: !KuboOptions.UseCache, cancellationToken);
        }
        
        await ApplyEntryUpdateAsync(updateEvent, accentColor, cancellationToken);
    }
    
    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="accentColor">The resolved accent color data for this event.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task ApplyEntryUpdateAsync(ValueUpdateEvent updateEvent, string? accentColor, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (updateEvent.EventId is not nameof(UpdateAccentColorAsync))
            return Task.CompletedTask;
        
        Inner.Inner.AccentColor = accentColor;
        AccentColorUpdated?.Invoke(this, accentColor);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc cref="INomadKuboEventStreamHandler{TEventEntryContent}.AppendNewEntryAsync" />
    public override async Task<EventStreamEntry<Cid>> AppendNewEntryAsync(ValueUpdateEvent updateEvent, CancellationToken cancellationToken = default)
    {
        // Use extension method for code deduplication (can't use inheritance).
        var localUpdateEventCid = await Client.Dag.PutAsync(updateEvent, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var newEntry = await this.AppendEventStreamEntryAsync(localUpdateEventCid, updateEvent.EventId, updateEvent.TargetId, cancellationToken);
        return newEntry;
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
        bool unset = accentColor is null;

        DagCid? valueCid = null;
        if (!unset)
        {
            Cid cid = await Client.Dag.PutAsync(accentColor!, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
            valueCid = (DagCid)cid;
        }

        var updateEvent = new ValueUpdateEvent(Id, nameof(UpdateAccentColorAsync), null, valueCid, unset);

        await ApplyEntryUpdateAsync(updateEvent, accentColor, cancellationToken);
        var appendedEntry = await AppendNewEntryAsync(updateEvent, cancellationToken);

        EventStreamPosition = appendedEntry;
    }
}