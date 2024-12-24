namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a published entity.
/// </summary>
public interface IEntity : IConnections, IImages
{
    /// <summary>
    /// The name of the entity.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// A description of the entity. Supports markdown.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// An extended description of the entity. Supports markdown.
    /// </summary>
    string ExtendedDescription { get; set; }

    /// <summary>
    /// A flag that indicates whether the entity has requested to be forgotten.
    /// </summary>
    bool? ForgetMe { get; set; }

    /// <summary>
    /// A flag indicating whether this is an unlisted project.
    /// </summary>
    public bool IsUnlisted { get; set; }
}