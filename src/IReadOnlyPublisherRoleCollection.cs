namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of publishers with corresponding roles that cannot be modified.
/// </summary>
public interface IReadOnlyPublisherRoleCollection : IReadOnlyPublisherCollection<IReadOnlyPublisherRole>
{
}
