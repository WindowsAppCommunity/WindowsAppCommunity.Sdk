namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a modifiable entity with common methods.
/// </summary>
public interface IModifiableEntity : IReadOnlyEntity, IModifiableConnectionsCollection, IModifiableLinksCollection, IModifiableImagesCollection
{
    /// <summary>
    /// Updates the name of this entity.
    /// </summary>
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the description of this entity.
    /// </summary>
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the extended description of this entity.
    /// </summary>
    public Task UpdateExtendedDescriptionAsync(string extendedDescription, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the forget-me status of this entity.
    /// </summary>
    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the unlisted state of this entity.
    /// </summary>
    public Task UpdateUnlistedStateAsync(bool isUnlisted, CancellationToken cancellationToken);
}
