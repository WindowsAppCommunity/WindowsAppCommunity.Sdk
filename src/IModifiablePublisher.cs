namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a content publisher that can be modified.
/// </summary>
public interface IModifiablePublisher : IReadOnlyPublisher, IModifiableEntity, IModifiableAccentColor, IModifiableUserRoleCollection
{
}
