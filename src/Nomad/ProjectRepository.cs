using System.Linq;
using Ipfs;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
///  A repository that can be used to create (with parameter), read, update, and delete Project items under a specific context in Kubo.
/// </summary>
public class ProjectRepository : NomadKuboRepository<ModifiableProject, IReadOnlyProject, Project, ValueUpdateEvent, ProjectCreateParam>
{
    /// <summary>
    /// The prefix used to build local and roaming key names
    /// </summary>
    public string KeyNamePrefix { get; set; } = "WindowsAppCommunity.Sdk.Project";
    
    /// <inheritdoc/> 
    public override ModifiableProject ModifiableFromHandlerConfig(NomadKuboEventStreamHandlerConfig<Project> handlerConfig) => ModifiableProject.FromHandlerConfig(handlerConfig, this, PublisherRepository, UserRepository, Client, KuboOptions);
    
    /// <inheritdoc/> 
    public override IReadOnlyProject ReadOnlyFromHandlerConfig(NomadKuboEventStreamHandlerConfig<Project> handlerConfig) => ReadOnlyProject.FromHandlerConfig(handlerConfig, this, PublisherRepository, UserRepository, Client, KuboOptions);
    
    /// <inheritdoc/> 
    protected override NomadKuboEventStreamHandlerConfig<Project> GetEmptyConfig() => new();

    /// <summary>
    /// The repository to use for returning modifiable or readonly users.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiableUser, IReadOnlyUser> UserRepository { get; set; }

    /// <summary>
    /// The repository to use for returning modifiable or readonly publishers.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiablePublisher, IReadOnlyPublisher> PublisherRepository { get; set; }

    /// <inheritdoc/> 
    public override Task<(string LocalKeyName, string RoamingKeyName)?> GetExistingKeyNamesAsync(string roamingId, CancellationToken cancellationToken)
    {
        var existingRoamingKey = ManagedKeys.FirstOrDefault(x=> $"{x.Id}" == $"{roamingId}");
        if (existingRoamingKey is null)
            return Task.FromResult<(string LocalKeyName, string RoamingKeyName)?>(null);
        
        // Transform roaming key name into local key name
        // This repository implementation doesn't do anything fancy for this,
        // the names are basically hardcoded to the KeyNamePrefix and roaming vs local.
        var localKeyName = existingRoamingKey.Name.Replace(".Roaming.", ".Local.");
        return Task.FromResult<(string LocalKeyName, string RoamingKeyName)?>((localKeyName, existingRoamingKey.Name));
    }

    /// <inheritdoc/> 
    public override (string LocalKeyName, string RoamingKeyName) GetNewKeyNames(ProjectCreateParam createParam)
    {
        return (LocalKeyName: $"{KeyNamePrefix}.Local.{createParam.KnownId}", RoamingKeyName: $"{KeyNamePrefix}.Roaming.{createParam.KnownId}");
    }
    
    /// <inheritdoc/> 
    public override string GetNewEventStreamLabel(ProjectCreateParam createParam, IKey roamingKey, IKey localKey) => $"Project {createParam.KnownId}";
    
    /// <inheritdoc/> 
    public override Project GetInitialRoamingValue(ProjectCreateParam createParam, IKey roamingKey, IKey localKey) => new()
    {
        Sources = [localKey.Id],
    };
}

/// <summary>
/// A record of the required fields for creating a new <see cref="Project"/> via <see cref="ProjectRepository"/>.
/// </summary>
/// <param name="KnownId">A pre-existing unique identifier for this Project. Used to uniquely name the created Nomad keys in Kubo. Necessary for identifying keys for during pairing and retrieval. If unknown or arbitrary, use integer increments or GUID.</param>
public record ProjectCreateParam(string KnownId);


