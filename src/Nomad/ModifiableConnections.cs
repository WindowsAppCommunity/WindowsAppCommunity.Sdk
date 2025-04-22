using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad
{
    public class ModifiableConnections : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableConnection
    {
        private readonly Dictionary<string, IModifiableConnection> _connections;

        public ModifiableConnections()
        {
            _connections = new Dictionary<string, IModifiableConnection>();
        }

        public Task AddConnectionAsync(IModifiableConnection connection, CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            _connections[connection.Id] = connection;
            return Task.CompletedTask;
        }

        public Task RemoveConnectionAsync(IModifiableConnection connection, CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            _connections.Remove(connection.Id);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyDictionary<string, IModifiableConnection>> GetConnectionsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IReadOnlyDictionary<string, IModifiableConnection>)_connections);
        }

        public string Id { get; private set; }
        public string Value { get; private set; }
        public event EventHandler<string>? ValueUpdated;

        public Task UpdateValueAsync(string newValue, CancellationToken cancellationToken = default)
        {
            Value = newValue;
            ValueUpdated?.Invoke(this, newValue);
            return Task.CompletedTask;
        }
    }
}
