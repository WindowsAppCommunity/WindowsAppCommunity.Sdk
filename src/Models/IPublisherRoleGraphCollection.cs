namespace WindowsAppCommunity.Sdk.Models;

/// <summary>
/// Represents a collection of publishers and their roles, where each publisher can have child and parent publishers.
/// This interface is used to manage hierarchical relationships between publishers.
/// </summary>
public interface IPublisherRoleGraphNodeCollection
{
    /// <summary>
    /// A list of other publishers who are managed under this publisher.
    /// </summary>
    public PublisherRoleCollection ParentPublishers { get; set; }

    /// <summary>
    /// A list of other publishers who are managed under this publisher.
    /// </summary>
    public PublisherRoleCollection ChildPublishers { get; set; }
}
