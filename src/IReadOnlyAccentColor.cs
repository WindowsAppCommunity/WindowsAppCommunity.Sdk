namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents an accent color that cannot be updatd.
/// </summary>
public interface IReadOnlyAccentColor
{
    /// <summary>
    /// A hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; }

    /// <summary>
    /// Raised when <see cref="AccentColor"/> is updated.
    /// </summary>
    public event EventHandler<string?>? AccentColorUpdated;
}
