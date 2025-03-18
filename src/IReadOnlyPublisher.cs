namespace WindowsAppCommunity.Sdk;

/// <summary>
/// Represents a publisher, a collection of projects and collaborators who publish content to users.
/// </summary>
public interface IReadOnlyPublisher : IReadOnlyPublisher<IReadOnlyPublisherCollection>
{
}

/// <summary>
/// Represents a publisher, a collection of projects and collaborators who publish content to users.
/// </summary>
public interface IReadOnlyPublisher<TPublisherCollection> : IReadOnlyEntity, IReadOnlyAccentColor, IReadOnlyUserRoleCollection, IReadOnlyProjectCollection
    where TPublisherCollection : IReadOnlyPublisherCollection<IReadOnlyPublisher>
{
    /// <summary>
    /// The collection of publishers that this publisher belongs to.
    /// </summary>
    public TPublisherCollection ParentPublishers { get; }

    /// <summary>
    /// The collection of publishers that belong to this publisher.
    /// </summary>
    public TPublisherCollection ChildPublishers { get; }
}
