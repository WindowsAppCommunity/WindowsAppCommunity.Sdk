using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
using WindowsAppCommunity.Sdk.Models;
using WindowsAppCommunity.Sdk.Nomad;

namespace WindowsAppCommunity.Sdk.Tests;

public record RepositoryContainer
{
    public NomadKuboRepository<ModifiableProject, IReadOnlyProject, Project, ValueUpdateEvent>? ProjectRepository { get; set; }
    public NomadKuboRepository<ModifiablePublisher, IReadOnlyPublisher, Publisher, ValueUpdateEvent>? PublisherRepository { get; set; }
    public NomadKuboRepository<ModifiableUser, IReadOnlyUser, User, ValueUpdateEvent>? UserRepository { get; set; }
}
