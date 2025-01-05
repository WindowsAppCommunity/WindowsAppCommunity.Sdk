namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a publisher with a corresponding role that can be modified.
/// </summary>
public interface IModifiablePublisherRole : IReadOnlyPublisherRole, IModifiablePublisher
{
}