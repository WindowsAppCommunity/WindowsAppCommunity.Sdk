namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published collection of links.
/// </summary>
public interface ILinkCollection
{
    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    Link[] Links { get; set; }
}