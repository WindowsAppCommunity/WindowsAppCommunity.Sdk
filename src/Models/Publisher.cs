using Ipfs;
using System.Collections.Generic;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a content publisher.
/// </summary>
public record Publisher : IName
{
    /// <summary>
    /// The name of the publisher.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A description of the publisher.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// An extended description of the publisher.
    /// </summary>
    public string ExtendedDescription { get; set; }

    /// <summary>
    /// A hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; set; }

    /// <summary>
    /// Represents links to external profiles or resources added by the publisher.
    /// </summary>
    public Link[] Links { get; set; } = [];

    /// <summary>
    /// Users who are registered to participate in this publisher, along with their roles.
    /// </summary>
    public Dictionary<DagCid, Role> Users { get; set; } = new();

    /// <summary>
    /// A list of other publishers who are managed under this publisher.
    /// </summary>
    public DagCid[] ParentPublishers { get; set; } = [];

    /// <summary>
    /// A list of other publishers who are managed under this publisher.
    /// </summary>
    public DagCid[] ChildPublishers { get; set; } = [];

    /// <summary>
    /// Holds information about publisher assets that have been published for consumption by an end user, such as a Microsoft Store app, a package on nuget.org, a git repo, etc.
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
