using System.Collections.Generic;
using OwlCore.ComponentModel;
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

    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersAdded;
    
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyPublisherRole[]>? PublishersRemoved;

    /// <inheritdoc/>
    public IAsyncEnumerable<IReadOnlyPublisherRole> GetPublishersAsync(CancellationToken cancellationToken)
    {
        // TODO: Needs publisher repository
        throw new NotImplementedException();
    }
}
