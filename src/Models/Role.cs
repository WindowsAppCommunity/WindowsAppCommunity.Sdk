using Newtonsoft.Json;
using OwlCore.Storage;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents the role of a user.
/// </summary>
public record Role : IStorable
{
    /// <summary>
    /// A unique identifier for this Role.
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// The name of the role.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// A description for the role.
    /// </summary>
    public required string Description { get; init; }

    /// <inheritdoc />
    public virtual bool Equals(Role? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Description == other.Description;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Name.GetHashCode() * 397) ^ Description.GetHashCode();
        }
    }
}
