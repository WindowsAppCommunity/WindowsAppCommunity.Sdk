namespace WindowsAppCommunity.Sdk.Models;

/// <inheritdoc/>
public record PublisherRoleCollection : IPublisherRoleCollection
{
    /// <inheritdoc />
    public PublisherRole[] Publishers { get; set; } = [];
}
