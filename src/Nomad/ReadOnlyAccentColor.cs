using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IReadOnlyAccentColor" />
public class ReadOnlyAccentColor : IReadOnlyAccentColor, IDelegable<IAccentColor>
{
    /// <summary>
    /// The client to use for communicating with ipfs.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <inheritdoc />
    public required IAccentColor Inner { get; init; }

    /// <inheritdoc />
    public string? AccentColor => Inner.AccentColor;

    /// <inheritdoc />
    public event EventHandler<string?>? AccentColorUpdated;
}