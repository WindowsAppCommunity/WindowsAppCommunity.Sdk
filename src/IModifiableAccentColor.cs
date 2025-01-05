namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents an accent color that can be modified.
/// </summary>
public interface IModifiableAccentColor : IReadOnlyAccentColor
{
    /// <summary>
    /// Updates the accent color for this project.
    /// </summary>
    /// <param name="accentColor">The new hex-encoded accent color for this project.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task UpdateAccentColorAsync(string? accentColor, CancellationToken cancellationToken);
}
