namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a user that can be modified. 
/// </summary>
public interface IModifiableUser : IReadOnlyUser, IModifiableEntity, IModifiablePublisherRoleCollection, IModifiableProjectRoleCollection
{
}
