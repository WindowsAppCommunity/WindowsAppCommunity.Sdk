using Ipfs;
using System.Collections.Generic;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a project.
/// </summary>
public record Project : IEntity, IUserRoleCollection, IAccentColor, IProjectRoleCollection
{
    /// <summary>
    /// The canonical publisher for this project.
    /// </summary>
    public required DagCid Publisher { get; set; }

    /// <summary>
    /// The name of this project.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A description of this project.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// An extended description of this project.
    /// </summary>
    public required string ExtendedDescription { get; set; }

    /// <summary>
    /// A list of <see cref="Image"/>s demonstrating this project.
    /// </summary>
    public Image[] Images { get; set; } = [];

    /// <summary>
    /// A list of features provided by this project.
    /// </summary>
    public string[] Features { get; set; } = [];

    /// <summary>
    /// A hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; set; }

    /// <summary>
    /// The category defining this project, as found in an app store.
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Other projects which this project may depend on for various reasons.
    /// </summary>
    public Dictionary<DagCid, Role> Projects { get; set; } = [];

    /// <summary>
    /// The <see cref="User"/>s who collaborate on this project, and their corresponding roles.
    /// </summary>
    public Dictionary<DagCid, Role> Users { get; set; } = [];

    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    public Link[] Links { get; set; } = [];

    /// <summary>
    /// Holds information about project assets that have been published for consumption by an end user, such as a Microsoft Store app, a package on nuget.org, a git repo, etc.
    /// </summary>
    public Dictionary<string, DagCid> Connections { get; set; } = new();

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; set; }

    /// <summary>
    /// A flag indicating whether this is a non-public project.
    /// </summary>
    public bool IsUnlisted { get; set; }
}
