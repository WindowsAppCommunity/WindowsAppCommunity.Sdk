namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a modifiable connection with a key and a value.
/// </summary>
public interface IModifiableConnection : IReadOnlyConnection
{
    /// <summary>
    /// Updates the value of the connection.
    /// </summary>
    Task UpdateValueAsync(string value, CancellationToken cancellationToken);
}
