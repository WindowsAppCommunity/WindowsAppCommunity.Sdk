using OwlCore.ComponentModel;

namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a user.
/// </summary>
public interface IReadOnlyUser : IReadOnlyEntity, IReadOnlyPublisherRoleCollection, IReadOnlyProjectRoleCollection, IHasId
{
}
