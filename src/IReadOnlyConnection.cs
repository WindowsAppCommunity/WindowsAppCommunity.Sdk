namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a read-only connection with an ID and a value.
/// </summary>
public interface IReadOnlyConnection
{
    /// <summary>
    /// Gets the key or Id of the connection.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the value of the connection.
    /// </summary>
    Task<string> GetValueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when the value is updated.
    /// </summary>
    event EventHandler<string>? ValueUpdated;
}
