using Ipfs;
using System.Collections.Generic;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a user.
/// </summary>
public record User : IEntity, IConnections, ILinkCollection, IProjectRoleCollection, IPublisherRoleCollection
{
    /// <summary>
    /// The name of the user.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A description of the user. Supports markdown.
    /// </summary>
    public required string Description { get; set;  }

    /// <summary>
    /// An extended description of the user. Supports markdown.
    /// </summary>
    public required string ExtendedDescription { get; set; }

    /// <summary>
    /// Represents application connections added by the user.
    /// </summary>
    public Dictionary<string, DagCid> Connections { get; set; } = [];

    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    public Link[] Links { get; set; } = [];

    /// <summary>
    /// Represents images that demonstrate this user.
    /// </summary>
    public Image[] Images { get; set; } = [];

    /// <summary>
    /// A list of all the projects the user is registered with, along with their role on the project.
    /// </summary>
    public Dictionary<DagCid, Role> Projects { get; set; } = [];

    /// <summary>
    /// Represents all publishers the user is registered with, along with their roles.
    /// </summary>
    public Dictionary<DagCid, Role> Publishers { get; set; } = [];

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; set; }

    /// <summary>
    /// A flag indicating whether this is an unlisted project.
    /// </summary>
    public bool IsUnlisted { get; set; }
}
