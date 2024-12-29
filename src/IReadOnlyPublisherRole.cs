namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a publisher with a corresponding role that cannot be modified.
/// </summary>
public interface IReadOnlyPublisherRole : IReadOnlyPublisher
{
    /// <summary>
    /// Gets the role for this publisher.
    /// </summary>
    public Role Role { get; }
}
