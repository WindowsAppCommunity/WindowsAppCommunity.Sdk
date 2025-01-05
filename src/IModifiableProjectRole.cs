namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a project with a corresponding role that can be modified.
/// </summary>
public interface IModifiableProjectRole : IReadOnlyProjectRole, IModifiableProject
{
}