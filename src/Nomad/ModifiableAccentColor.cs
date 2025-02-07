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

    /// <inheritdoc />
    public string? AccentColor { get; init;  }
    
    /// <inheritdoc />
    public event EventHandler<string?>? AccentColorUpdated;

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <remarks>
    /// This method will call <see cref="ReadOnlyAccentColor.GetAsync(string, CancellationToken)"/> and create a new instance to pass to the event handlers.
    /// <para/>
    /// If already have a resolved instance of <see cref="Image"/>, you should call <see cref="ApplyEntryUpdateAsync(ValueUpdateEvent, Image, CancellationToken)"/> instead.
    /// </remarks>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public override Task ApplyEntryUpdateAsync(ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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