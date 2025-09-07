# Interface Graph Inventory for WindowsAppCommunity.Sdk

This document provides a comprehensive inventory of the interface hierarchy for the three main entities in the WindowsAppCommunity.Sdk: **Publisher**, **Project**, and **User**. This mapping is intended to help with blog generator development by clearly defining the type relationships and data access patterns.

## Core Entity Interfaces

### IReadOnlyUser
```
IReadOnlyUser : IReadOnlyEntity, IReadOnlyPublisherRoleCollection, IReadOnlyProjectRoleCollection, IHasId
```

**Purpose**: Represents a user with access to their entity data, publisher roles, and project roles.

**Key Properties**:
- Inherits all properties from `IReadOnlyEntity` (name, description, connections, links, images)
- Access to publisher roles via `IReadOnlyPublisherRoleCollection`
- Access to project roles via `IReadOnlyProjectRoleCollection`

### IReadOnlyPublisher
```
IReadOnlyPublisher : IReadOnlyPublisher<IReadOnlyPublisherRoleCollection>

IReadOnlyPublisher<TPublisherCollection> : IReadOnlyEntity, IReadOnlyAccentColor, IReadOnlyUserRoleCollection, IReadOnlyProjectCollection, IHasId
    where TPublisherCollection : IReadOnlyPublisherCollection<IReadOnlyPublisherRole>
```

**Purpose**: Represents a publisher entity with accent color, user roles, projects, and hierarchical publisher relationships.

**Key Properties**:
- Inherits all properties from `IReadOnlyEntity` (name, description, connections, links, images)
- `AccentColor` via `IReadOnlyAccentColor`
- User roles via `IReadOnlyUserRoleCollection`
- Projects via `IReadOnlyProjectCollection`
- `ParentPublishers` and `ChildPublishers` (hierarchical relationships)

### IReadOnlyProject
```
IReadOnlyProject : IReadOnlyProject<IReadOnlyProjectCollection>

IReadOnlyProject<TDependencyCollection> : IReadOnlyEntity, IReadOnlyImagesCollection, IReadOnlyUserRoleCollection, IReadOnlyAccentColor, IReadOnlyFeaturesCollection, IHasId
    where TDependencyCollection : IReadOnlyProjectCollection<IReadOnlyProject>
```

**Purpose**: Represents a project entity with dependencies, features, accent color, users, and images.

**Key Properties**:
- Inherits all properties from `IReadOnlyEntity` (name, description, connections, links, images)
- Additional images via `IReadOnlyImagesCollection`
- User roles via `IReadOnlyUserRoleCollection`
- `AccentColor` via `IReadOnlyAccentColor`
- `Features` via `IReadOnlyFeaturesCollection`
- `Dependencies` (other projects this project depends on)
- `Category` (string property for app store categorization)
- `GetPublisherAsync()` method to retrieve associated publisher

## Supporting Interface Hierarchy

### Base Entity Interface
```
IReadOnlyEntity : IReadOnlyConnectionsCollection, IReadOnlyLinksCollection, IReadOnlyImagesCollection, IHasId
```

**Properties**:
- `Name` (string)
- `Description` (string, supports markdown)
- `ExtendedDescription` (string, supports markdown)
- `ForgetMe` (bool?)
- `IsUnlisted` (bool)

**Events**:
- `NameUpdated`
- `DescriptionUpdated`
- `ExtendedDescriptionUpdated`
- `ForgetMeUpdated`
- `IsUnlistedUpdated`

### Collection Interfaces

#### IReadOnlyConnectionsCollection
```
IReadOnlyConnectionsCollection : IHasId
```
- `GetConnectionsAsync()` → `IAsyncEnumerable<IReadOnlyConnection>`
- Events: `ConnectionsAdded`, `ConnectionsRemoved`

#### IReadOnlyLinksCollection
```
IReadOnlyLinksCollection : IHasId
```
- `Links` (Link[] property)
- Events: `LinksAdded`, `LinksRemoved`

#### IReadOnlyImagesCollection
```
IReadOnlyImagesCollection : IHasId
```
- `GetImageFilesAsync()` → `IAsyncEnumerable<IFile>`
- Events: `ImagesAdded`, `ImagesRemoved`

#### IReadOnlyAccentColor
```
IReadOnlyAccentColor : IHasId
```
- `AccentColor` (string?)
- Events: `AccentColorUpdated`

#### IReadOnlyFeaturesCollection
```
IReadOnlyFeaturesCollection : IHasId
```
- `Features` (string[])
- Events: `FeaturesAdded`, `FeaturesRemoved`

### Role-Based Collections

#### IReadOnlyUserRoleCollection
```
IReadOnlyUserRoleCollection : IReadOnlyUserCollection<IReadOnlyUserRole>
IReadOnlyUserCollection<TUser> : IHasId where TUser : IReadOnlyUser
```
- `GetUsersAsync()` → `IAsyncEnumerable<TUser>`

#### IReadOnlyProjectRoleCollection
```
IReadOnlyProjectRoleCollection : IReadOnlyProjectCollection<IReadOnlyProjectRole>
```

#### IReadOnlyPublisherRoleCollection
```
IReadOnlyPublisherRoleCollection : IReadOnlyPublisherCollection<IReadOnlyPublisherRole>
```

#### IReadOnlyProjectCollection
```
IReadOnlyProjectCollection<TProject> : IHasId where TProject : IReadOnlyProject
```
- `GetProjectsAsync()` → `IAsyncEnumerable<TProject>`
- Events: `ProjectsAdded`, `ProjectsRemoved`

#### IReadOnlyPublisherCollection
```
IReadOnlyPublisherCollection<TPublisher> : IHasId where TPublisher : IReadOnlyPublisher
```
- `GetPublishersAsync()` → `IAsyncEnumerable<TPublisher>`
- Events: `PublishersAdded`, `PublishersRemoved`

### Role Interfaces

#### IReadOnlyUserRole
```
IReadOnlyUserRole : IReadOnlyUser
```
- `Role` (Role property)

#### IReadOnlyProjectRole
```
IReadOnlyProjectRole : IReadOnlyProject
```
- `Role` (Role property)

#### IReadOnlyPublisherRole
```
IReadOnlyPublisherRole : IReadOnlyPublisher
```
- `Role` (Role property)

## Supporting Data Classes

### Role
```csharp
public class Role
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}
```

### Link
```csharp
public class Link : IStorable
{
    public required string Id { get; init; }
    public required string Url { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}
```

### IReadOnlyConnection
```csharp
public interface IReadOnlyConnection
{
    string Id { get; }
    Task<string> GetValueAsync(CancellationToken cancellationToken = default);
    event EventHandler<string>? ValueUpdated;
}
```

## Data Access Patterns for Blog Generator

### For User Profiles
1. **Basic Info**: `IReadOnlyEntity` → name, description, extendedDescription
2. **Visual Elements**: `IReadOnlyImagesCollection` → profile images
3. **External Links**: `IReadOnlyLinksCollection` → social/external profiles
4. **App Connections**: `IReadOnlyConnectionsCollection` → connected applications
5. **Project Involvement**: `IReadOnlyProjectRoleCollection` → projects and roles
6. **Publisher Involvement**: `IReadOnlyPublisherRoleCollection` → publishers and roles

### For Publisher Profiles
1. **Basic Info**: `IReadOnlyEntity` → name, description, extendedDescription
2. **Branding**: `IReadOnlyAccentColor` → brand color
3. **Visual Elements**: `IReadOnlyImagesCollection` → logos, banners
4. **External Links**: `IReadOnlyLinksCollection` → websites, social media
5. **App Connections**: `IReadOnlyConnectionsCollection` → connected applications
6. **Team Members**: `IReadOnlyUserRoleCollection` → users and their roles
7. **Published Projects**: `IReadOnlyProjectCollection` → all projects under publisher
8. **Publisher Hierarchy**: `ParentPublishers`, `ChildPublishers` → organizational structure

### For Project Profiles
1. **Basic Info**: `IReadOnlyEntity` → name, description, extendedDescription
2. **Categorization**: `Category` → app store category
3. **Branding**: `IReadOnlyAccentColor` → project theme color
4. **Visual Elements**: `IReadOnlyImagesCollection` → screenshots, icons
5. **External Links**: `IReadOnlyLinksCollection` → project website, documentation
6. **App Connections**: `IReadOnlyConnectionsCollection` → integrations
7. **Features**: `IReadOnlyFeaturesCollection` → feature list
8. **Team Members**: `IReadOnlyUserRoleCollection` → contributors and roles
9. **Dependencies**: `Dependencies` → other projects this depends on
10. **Publisher**: `GetPublisherAsync()` → owning publisher

## Interface Dependency Graph

```
IHasId (base interface with Id property)
├── IReadOnlyEntity
│   ├── IReadOnlyConnectionsCollection
│   ├── IReadOnlyLinksCollection  
│   └── IReadOnlyImagesCollection
├── IReadOnlyAccentColor
├── IReadOnlyFeaturesCollection
├── IReadOnlyUserCollection<T>
│   └── IReadOnlyUserRoleCollection
├── IReadOnlyProjectCollection<T>
│   └── IReadOnlyProjectRoleCollection
├── IReadOnlyPublisherCollection<T>
│   └── IReadOnlyPublisherRoleCollection
├── IReadOnlyUser
│   └── IReadOnlyUserRole
├── IReadOnlyProject<T>
│   └── IReadOnlyProjectRole
└── IReadOnlyPublisher<T>
    └── IReadOnlyPublisherRole
```

## Recommendations for Blog Generator Implementation

1. **Start with Core Entities**: Implement converters for `IReadOnlyUser`, `IReadOnlyPublisher`, and `IReadOnlyProject` first
2. **Implement Supporting Collections**: Handle each collection interface as a separate template component
3. **Handle Async Data**: Many collections use `IAsyncEnumerable<T>` - ensure proper async handling in templates
4. **Link Resolution**: Pre-resolve relationships (like publisher for projects) to avoid async calls in templates
5. **Role Handling**: Create specialized templates for role-based views vs direct entity views
6. **Image Processing**: Handle `IFile` objects from `IReadOnlyImagesCollection` for proper image rendering
7. **Connection Values**: Cache `IReadOnlyConnection.GetValueAsync()` results before template rendering

This interface inventory provides the foundation for systematically implementing blog generation for each entity type while maintaining clear separation of concerns.