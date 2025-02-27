using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage;
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

        Inner.AccentColor = updateEvent.Value;
    }

    /// <inheritdoc />
    public override Task<EventStreamEntry<Cid>> AppendNewEntryAsync(ValueUpdateEvent updateEvent, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc />
    public Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}