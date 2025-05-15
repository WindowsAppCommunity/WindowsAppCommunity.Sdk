using Ipfs;
using OwlCore.ComponentModel;
using System.Collections.Generic;

namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a content publisher.
/// </summary>
public record Publisher : IEntity, ILinkCollection, IProjectCollection, IUserRoleCollection, IConnections, IAccentColor, ISources<Cid>
{
    /// <summary>
    /// The name of the publisher.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A description of the publisher.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// An extended description of the publisher.
    /// </summary>
    public string ExtendedDescription { get; set; } = string.Empty;

    /// <summary>
    /// A hex-encoded accent color for this publisher.
    /// </summary>
    public string? AccentColor { get; set; }

    /// <summary>
    /// Represents links to external profiles or resources added by the publisher.
    /// </summary>
    public Link[] Links { get; set; } = [];

    /// <summary>
    /// Represents images that demonstrate this publisher.
    /// </summary>
    public Image[] Images { get; set; } = [];

    /// <summary>
    /// Users who are registered to participate in this publisher, along with their roles.
    /// </summary>
    public UserRole[] Users { get; set; } = [];

    /// <summary>
    /// Projects who are registered under this publisher.
    /// </summary>
    public Cid[] Projects { get; set; } = [];

    /// <summary>
    /// A list of other publishers who are managed under this publisher.
    /// </summary>
    public PublisherCollection ParentPublishers { get; set; } = new();

    /// <summary>
    /// A list of other publishers who are managed under this publisher.
    /// </summary>
    public PublisherCollection ChildPublishers { get; set; } = new();

    /// <summary>
    /// Holds information about publisher assets that have been published for consumption by an end user, such as a Microsoft Store app, a package on nuget.org, a git repo, etc.
    /// </summary>
    public Dictionary<string, DagCid> Connections { get; set; } = [];

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; set; }

    /// <summary>
    /// A flag indicating whether this is a non-public publisher.
    /// </summary>
    public bool IsUnlisted { get; set; }

    /// <inheritdoc/>
    public required ICollection<Cid> Sources { get; init; }
}
