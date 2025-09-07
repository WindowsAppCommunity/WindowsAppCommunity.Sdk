# Interface Graph Inventory for WindowsAppCommunity.Sdk

This document provides a comprehensive inventory of the interface hierarchy for the three main entities in the WindowsAppCommunity.Sdk: **Publisher**, **Project**, and **User**. This mapping helps developers understand the type relationships and data access patterns.

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

## Complete Interface Dependency Graphs

### IReadOnlyUser Interface Hierarchy

```
IReadOnlyUser : IReadOnlyEntity, IReadOnlyPublisherRoleCollection, IReadOnlyProjectRoleCollection, IHasId
│
├── IReadOnlyEntity
│   ├── IReadOnlyConnectionsCollection
│   │   └── IHasId
│   ├── IReadOnlyLinksCollection
│   │   └── IHasId
│   ├── IReadOnlyImagesCollection
│   │   └── IHasId
│   └── IHasId
│
├── IReadOnlyPublisherRoleCollection
│   └── IReadOnlyPublisherCollection<IReadOnlyPublisherRole>
│       ├── IReadOnlyPublisherRole : IReadOnlyPublisher
│       │   └── IReadOnlyPublisher : IReadOnlyPublisher<IReadOnlyPublisherRoleCollection>
│       │       └── IReadOnlyPublisher<TPublisherCollection> : IReadOnlyEntity, IReadOnlyAccentColor, IReadOnlyUserRoleCollection, IReadOnlyProjectCollection, IHasId
│       │           ├── IReadOnlyEntity (see above)
│       │           ├── IReadOnlyAccentColor
│       │           │   └── IHasId
│       │           ├── IReadOnlyUserRoleCollection
│       │           │   └── IReadOnlyUserCollection<IReadOnlyUserRole>
│       │           │       ├── IReadOnlyUserRole : IReadOnlyUser (circular reference)
│       │           │       └── IHasId
│       │           ├── IReadOnlyProjectCollection
│       │           │   └── IHasId
│       │           └── IHasId
│       └── IHasId
│
├── IReadOnlyProjectRoleCollection
│   └── IReadOnlyProjectCollection<IReadOnlyProjectRole>
│       ├── IReadOnlyProjectRole : IReadOnlyProject
│       │   └── IReadOnlyProject : IReadOnlyProject<IReadOnlyProjectCollection>
│       │       └── IReadOnlyProject<TDependencyCollection> : IReadOnlyEntity, IReadOnlyImagesCollection, IReadOnlyUserRoleCollection, IReadOnlyAccentColor, IReadOnlyFeaturesCollection, IHasId
│       │           ├── IReadOnlyEntity (see above)
│       │           ├── IReadOnlyImagesCollection (see above)
│       │           ├── IReadOnlyUserRoleCollection (see above)
│       │           ├── IReadOnlyAccentColor (see above)
│       │           ├── IReadOnlyFeaturesCollection
│       │           │   └── IHasId
│       │           └── IHasId
│       └── IHasId
│
└── IHasId
```

### IReadOnlyProject Interface Hierarchy

```
IReadOnlyProject : IReadOnlyProject<IReadOnlyProjectCollection>
│
└── IReadOnlyProject<TDependencyCollection> : IReadOnlyEntity, IReadOnlyImagesCollection, IReadOnlyUserRoleCollection, IReadOnlyAccentColor, IReadOnlyFeaturesCollection, IHasId
    │
    ├── IReadOnlyEntity
    │   ├── IReadOnlyConnectionsCollection
    │   │   └── IHasId
    │   ├── IReadOnlyLinksCollection
    │   │   └── IHasId
    │   ├── IReadOnlyImagesCollection
    │   │   └── IHasId
    │   └── IHasId
    │
    ├── IReadOnlyImagesCollection (additional images beyond IReadOnlyEntity)
    │   └── IHasId
    │
    ├── IReadOnlyUserRoleCollection
    │   └── IReadOnlyUserCollection<IReadOnlyUserRole>
    │       ├── IReadOnlyUserRole : IReadOnlyUser
    │       │   └── IReadOnlyUser : IReadOnlyEntity, IReadOnlyPublisherRoleCollection, IReadOnlyProjectRoleCollection, IHasId
    │       │       ├── IReadOnlyEntity (see above)
    │       │       ├── IReadOnlyPublisherRoleCollection
    │       │       │   └── IReadOnlyPublisherCollection<IReadOnlyPublisherRole>
    │       │       │       └── IHasId
    │       │       ├── IReadOnlyProjectRoleCollection
    │       │       │   └── IReadOnlyProjectCollection<IReadOnlyProjectRole>
    │       │       │       └── IHasId
    │       │       └── IHasId
    │       └── IHasId
    │
    ├── IReadOnlyAccentColor
    │   └── IHasId
    │
    ├── IReadOnlyFeaturesCollection
    │   └── IHasId
    │
    ├── Dependencies (TDependencyCollection : IReadOnlyProjectCollection<IReadOnlyProject>)
    │   └── IReadOnlyProjectCollection<IReadOnlyProject>
    │       ├── IReadOnlyProject (circular reference)
    │       └── IHasId
    │
    └── IHasId
```

### IReadOnlyPublisher Interface Hierarchy

```
IReadOnlyPublisher : IReadOnlyPublisher<IReadOnlyPublisherRoleCollection>
│
└── IReadOnlyPublisher<TPublisherCollection> : IReadOnlyEntity, IReadOnlyAccentColor, IReadOnlyUserRoleCollection, IReadOnlyProjectCollection, IHasId
    │
    ├── IReadOnlyEntity
    │   ├── IReadOnlyConnectionsCollection
    │   │   └── IHasId
    │   ├── IReadOnlyLinksCollection
    │   │   └── IHasId
    │   ├── IReadOnlyImagesCollection
    │   │   └── IHasId
    │   └── IHasId
    │
    ├── IReadOnlyAccentColor
    │   └── IHasId
    │
    ├── IReadOnlyUserRoleCollection
    │   └── IReadOnlyUserCollection<IReadOnlyUserRole>
    │       ├── IReadOnlyUserRole : IReadOnlyUser
    │       │   └── IReadOnlyUser : IReadOnlyEntity, IReadOnlyPublisherRoleCollection, IReadOnlyProjectRoleCollection, IHasId
    │       │       ├── IReadOnlyEntity (see above)
    │       │       ├── IReadOnlyPublisherRoleCollection (circular reference)
    │       │       ├── IReadOnlyProjectRoleCollection
    │       │       │   └── IReadOnlyProjectCollection<IReadOnlyProjectRole>
    │       │       │       └── IHasId
    │       │       └── IHasId
    │       └── IHasId
    │
    ├── IReadOnlyProjectCollection
    │   ├── IReadOnlyProject : IReadOnlyProject<IReadOnlyProjectCollection>
    │   │   └── IReadOnlyProject<TDependencyCollection> : IReadOnlyEntity, IReadOnlyImagesCollection, IReadOnlyUserRoleCollection, IReadOnlyAccentColor, IReadOnlyFeaturesCollection, IHasId
    │   │       ├── IReadOnlyEntity (see above)
    │   │       ├── IReadOnlyImagesCollection (see above)
    │   │       ├── IReadOnlyUserRoleCollection (see above)
    │   │       ├── IReadOnlyAccentColor (see above)
    │   │       ├── IReadOnlyFeaturesCollection
    │   │       │   └── IHasId
    │   │       └── IHasId
    │   └── IHasId
    │
    ├── Publisher Hierarchy Collections (TPublisherCollection : IReadOnlyPublisherRoleCollection)
    │   └── IReadOnlyPublisherRoleCollection
    │       └── IReadOnlyPublisherCollection<IReadOnlyPublisherRole>
    │           ├── IReadOnlyPublisherRole : IReadOnlyPublisher (circular reference)
    │           └── IHasId
    │
    └── IHasId
```

This interface inventory provides a comprehensive reference for understanding the type hierarchy and relationships within the WindowsAppCommunity.Sdk.