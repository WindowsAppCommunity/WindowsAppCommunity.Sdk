using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;

namespace WindowsAppCommunity.Sdk.Nomad
{
    /// <summary>
    /// Represents a modifiable connection.
    /// </summary>
    public class ModifiableConnection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableConnection, IDelegable<ReadOnlyConnection>
    {
        /// <inheritdoc/>
        public required ReadOnlyConnection Inner { get; init; }

        /// <inheritdoc/>
        public string Id => Inner.Id;

        /// <inheritdoc/>
        public event EventHandler<string>? ValueUpdated;

        /// <inheritdoc/>
        public async Task UpdateValueAsync(string newValue, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dagCid = (DagCid)await Client.Dag.PutAsync(newValue, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
            var updateEvent = new ValueUpdateEvent(null, dagCid, false);

            var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(UpdateValueAsync), updateEvent, DateTime.UtcNow, cancellationToken);
            await ApplyEntryUpdateAsync(appendedEntry, updateEvent, newValue, cancellationToken);

            EventStreamPosition = appendedEntry;
        }

        /// <inheritdoc/>
        public Task<string> GetValueAsync(CancellationToken cancellationToken = default) => Inner.GetValueAsync(cancellationToken);

        /// <inheritdoc />
        public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (eventStreamEntry.TargetId != Id)
                return;

            Guard.IsNotNull(updateEvent.Value);
            var updatedValue = await Client.Dag.GetAsync<string>(updateEvent.Value, cancel: cancellationToken);

            Guard.IsNotNull(updatedValue);
            await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, updatedValue, cancellationToken);
        }

        /// <summary>
        /// Applies an event stream update event and raises the relevant events.
        /// </summary>
        /// <param name="eventStreamEntry">The event stream entry to apply.</param>
        /// <param name="updateEvent">The update event to apply.</param>
        /// <param name="newValue">The resolved new value data for this event.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
        public Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, string newValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (eventStreamEntry.EventId is not nameof(UpdateValueAsync))
                return Task.CompletedTask;

            Guard.IsNotNull(updateEvent.Value);

            Inner.Inner.Value = updateEvent.Value;
            ValueUpdated?.Invoke(this, newValue);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
        {
            EventStreamPosition = null;
            return Task.CompletedTask;
        }
    }
}
