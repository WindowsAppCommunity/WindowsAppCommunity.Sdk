namespace WindowsAppCommunity.Sdk;

/// <summary>
/// An in-memory implementation of <see cref="IReadOnlyConnection"/>.
/// </summary>
/// <remarks>
/// This class is used to create a basic connection value to be added to a collection.
/// </remarks>
public class InMemoryConnection : IReadOnlyConnection
{
    /// <summary>
    /// An Id for the connection.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The value of the connection.
    /// </summary>
    public required string Value { get; init; }

    /// <inheritdoc/>
    public Task<string> GetValueAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Value);
    }

    /// <inheritdoc/>
    public event EventHandler<string>? ValueUpdated;
}