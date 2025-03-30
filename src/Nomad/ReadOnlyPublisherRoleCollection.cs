using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A read only handler for roaming publisher/role collection data.
/// </summary>
public class ReadOnlyPublisherRoleCollection : IReadOnlyPublisherRoleCollection, IDelegable<IPublisherRoleCollection>
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required IPublisherRoleCollection Inner { get; init; }

    /// <summary>
    /// The client to use for communicating with IPFS.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <summary>
    /// The repository to use for getting modifiable or readonly publisher instances.
    /// </summary>
    public required NomadKuboRepository<ModifiablePublisher, IReadOnlyPublisher, Publisher, ValueUpdateEvent> PublisherRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersRemoved;

    /// <inheritdoc/>
    public async IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var publisherRole in Inner.Publishers)
        {
            var role = await Client.Dag.GetAsync<Role>(publisherRole.RoleCid, cancel: cancellationToken);
            var publisher = await PublisherRepository.GetAsync(publisherRole.PublisherId, cancellationToken);

            yield return new ReadOnlyPublisherRole
            {
                InnerPublisher = (publisher as ModifiablePublisher)?.InnerPublisher ?? (ReadOnlyPublisher)publisher,
                Role = role,
            };
        }
    }
}
