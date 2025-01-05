namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of projects with corresponding roles that can be modified.
/// </summary>
public interface IModifiableProjectRoleCollection : IModifiableProjectCollection<IReadOnlyProjectRole>
{
}