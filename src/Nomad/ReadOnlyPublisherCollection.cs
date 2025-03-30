using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// A read only handler for roaming publisher collection data.
/// </summary>
public class ReadOnlyPublisherCollection : IReadOnlyPublisherCollection, IDelegable<IPublisherCollection>
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required IPublisherCollection Inner { get; init; }

    /// <summary>
    /// The client to use for communicating with IPFS.
    /// </summary>
    public required ICoreApi Client { get; init; }

    /// <summary>
    /// The repository to use for getting modifiable or readonly publisher instances.
    /// </summary>
    public required NomadKuboRepository<ModifiablePublisher, IReadOnlyPublisher, Publisher, ValueUpdateEvent> PublisherRepository { get; init; }

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisher[]>? PublishersAdded;

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisher[]>? PublishersRemoved;

    /// <inheritdoc/>
    public async IAsyncEnumerable<IReadOnlyPublisher> GetPublishersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var publisherId in Inner.Publishers)
        {
            var publisher = await PublisherRepository.GetAsync(publisherId, cancellationToken);
            yield return publisher;
        }
    }
}