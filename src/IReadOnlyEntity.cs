namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a read-only entity with common properties and events.
/// </summary>
public interface IReadOnlyEntity : IReadOnlyConnectionsCollection, IReadOnlyLinksCollection, IReadOnlyImagesCollection
{
    /// <summary>
    /// The name of the entity.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A description of the entity. Supports markdown.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// An extended description of the entity. Supports markdown.
    /// </summary>
    string ExtendedDescription { get; }

    /// <summary>
    /// A flag that indicates whether the entity has requested to be forgotten.
    /// </summary>
    bool? ForgetMe { get; }

    /// <summary>
    /// A flag indicating whether this is a non-public project.
    /// </summary>
    public bool IsUnlisted { get; }

    /// <summary>
    /// Raised when <see cref="Name"/> is updated.
    /// </summary>
    event EventHandler<string>? NameUpdated;

    /// <summary>
    /// Raised when <see cref="Description"/> is updated.
    /// </summary>
    event EventHandler<string>? DescriptionUpdated;

    /// <summary>
    /// Raised when <see cref="ExtendedDescription"/> is updated.
    /// </summary>
    event EventHandler<string>? ExtendedDescriptionUpdated;

    /// <summary>
    /// Raised when <see cref="ForgetMe"/> is updated.
    /// </summary>
    event EventHandler<bool?>? ForgetMeUpdated;

    /// <summary>
    /// Raised when <see cref="IsUnlisted"/> is updated.
    /// </summary>
    public event EventHandler<bool>? IsUnlistedUpdated;
}
