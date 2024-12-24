namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents an accent color.
/// </summary>
public interface IAccentColor
{
    /// <summary>
    /// A hex-encoded accent color.
    /// </summary>
    public string? AccentColor { get; set; }
}