namespace WindowsAppCommunity.Sdk.Nomad;

/// <summary>
/// Represents a read-only connection.
/// </summary>
public class ReadOnlyConnection : IReadOnlyConnection
{
    /// <inheritdoc />
    public required string Id { get; init; }

    /// <inheritdoc />
    public required string Value { get; init; }

    /// <inheritdoc />
    public event EventHandler<string>? ValueUpdated;
}
