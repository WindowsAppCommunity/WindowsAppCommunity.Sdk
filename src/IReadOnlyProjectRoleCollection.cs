namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a collection of projects with corresponding roles that cannot be modified.
/// </summary>
public interface IReadOnlyProjectRoleCollection : IReadOnlyProjectCollection<IReadOnlyProjectRole>
{
}
