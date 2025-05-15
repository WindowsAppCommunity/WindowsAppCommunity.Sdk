using System.Linq;
using Ipfs;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
///  A repository that can be used to create (with parameter), read, update, and delete Publisher items under a specific context in Kubo.
/// </summary>
public class PublisherRepository : NomadKuboRepository<ModifiablePublisher, IReadOnlyPublisher, Publisher, ValueUpdateEvent, PublisherCreateParam>
{
    /// <summary>
    /// The prefix used to build local and roaming key names
    /// </summary>
    public string KeyNamePrefix { get; set; } = "WindowsAppCommunity.Sdk.Publisher";

    /// <inheritdoc/> 
    public override ModifiablePublisher ModifiableFromHandlerConfig(NomadKuboEventStreamHandlerConfig<Publisher> handlerConfig) => ModifiablePublisher.FromHandlerConfig(handlerConfig, ProjectRepository, this, UserRepository, Client, KuboOptions);

    /// <inheritdoc/> 
    public override IReadOnlyPublisher ReadOnlyFromHandlerConfig(NomadKuboEventStreamHandlerConfig<Publisher> handlerConfig) => ReadOnlyPublisher.FromHandlerConfig(handlerConfig, ProjectRepository, this, UserRepository, Client, KuboOptions);

    /// <inheritdoc/> 
    protected override NomadKuboEventStreamHandlerConfig<Publisher> GetEmptyConfig() => new();

    /// <summary>
    /// The repository to use for returning modifiable or readonly users.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiableUser, IReadOnlyUser> UserRepository { get; init; }

    /// <summary>
    /// The repository to use for returning modifiable or readonly projects.
    /// </summary>
    public required INomadKuboRepositoryBase<ModifiableProject, IReadOnlyProject> ProjectRepository { get; init; }

    /// <inheritdoc/> 
    public override Task<(string LocalKeyName, string RoamingKeyName)?> GetExistingKeyNamesAsync(string roamingId, CancellationToken cancellationToken)
    {
        var existingRoamingKey = ManagedKeys.FirstOrDefault(x => $"{x.Id}" == $"{roamingId}");
        if (existingRoamingKey is null)
            return Task.FromResult<(string LocalKeyName, string RoamingKeyName)?>(null);

        // Transform roaming key name into local key name
        // This repository implementation doesn't do anything fancy for this,
        // the names are basically hardcoded to the KeyNamePrefix and roaming vs local.
        var localKeyName = existingRoamingKey.Name.Replace(".Roaming.", ".Local.");
        return Task.FromResult<(string LocalKeyName, string RoamingKeyName)?>((localKeyName, existingRoamingKey.Name));
    }

    /// <inheritdoc/> 
    public override (string LocalKeyName, string RoamingKeyName) GetNewKeyNames(PublisherCreateParam createParam)
    {
        return (LocalKeyName: $"{KeyNamePrefix}.Local.{createParam.KnownId}", RoamingKeyName: $"{KeyNamePrefix}.Roaming.{createParam.KnownId}");
    }

    /// <inheritdoc/> 
    public override string GetNewEventStreamLabel(PublisherCreateParam createParam, IKey roamingKey, IKey localKey) => $"Publisher {createParam.KnownId}";

    /// <inheritdoc/> 
    public override Publisher GetInitialRoamingValue(PublisherCreateParam createParam, IKey roamingKey, IKey localKey) => new()
    {
        Sources = [localKey.Id],
    };
}

/// <summary>
/// A record of the required fields for creating a new <see cref="Publisher"/> via <see cref="PublisherRepository"/>.
/// </summary>
/// <param name="KnownId">A pre-existing unique identifier for this Publisher. Used to uniquely name the created Nomad keys in Kubo. Necessary for identifying keys for during pairing and retrieval. If unknown or arbitrary, use integer increments or GUID.</param>
public record PublisherCreateParam(string KnownId);