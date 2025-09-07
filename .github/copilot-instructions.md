# WindowsAppCommunity.Sdk

The WindowsAppCommunity.Sdk is a .NET library for managing membership, projects, and publisher data in the Windows App Community using IPFS (InterPlanetary File System) for distributed content addressing.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Setup
- Install .NET 9.0 SDK:
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0
  export PATH="$HOME/.dotnet:$PATH"
  ```
- Install IPFS Kubo (required for tests):
  ```bash
  wget https://github.com/ipfs/kubo/releases/download/v0.37.0/kubo_v0.37.0_linux-amd64.tar.gz -O kubo.tar.gz
  tar -xzf kubo.tar.gz
  sudo install kubo/ipfs /usr/local/bin/
  ```

### Build and Test Commands
- **Bootstrap the repository:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH"
  dotnet restore
  ```
- **Build the project:**
  ```bash
  dotnet build
  ```
  - Build time: ~8 seconds. NEVER CANCEL - Set timeout to 60+ seconds minimum
  - Produces many warnings (expected in current development state)
  - Targets both netstandard2.0 and net9.0 frameworks
  
- **Run tests:**
  ```bash
  dotnet test --verbosity normal
  ```
  - **CRITICAL**: Tests require internet connectivity to dist.ipfs.tech for IPFS Kubo binary downloads
  - Test execution time: 1-2 minutes if network connectivity is available, immediate failure without connectivity
  - NEVER CANCEL: Set timeout to 30+ minutes for full test execution with network dependencies
  - All 16 tests will fail in network-restricted environments due to IPFS dependency downloads

- **Generate NuGet packages:**
  ```bash
  dotnet build -c Release
  dotnet pack -c Release
  ```
  - Creates packages in `src/bin/Release/` directory
  - Generates both main package and symbols package (.symbols.nupkg)

## Validation

### Build Validation
- Always run `dotnet restore` before any build operations
- Expect build warnings (433+ warnings are normal for current development state)
- Build output should produce DLLs in `src/bin/Debug/` for both target frameworks

### Test Scenarios
**Note**: Tests require external IPFS infrastructure connectivity and will fail in network-restricted environments.

When network connectivity is available, tests validate:
- IPFS Kubo bootstrapping and repository creation
- Project creation and management via distributed storage
- Publisher and user repository operations
- Image and link collection management
- Cross-repository data synchronization

### Manual Validation Scenarios
Since automated tests may fail due to network restrictions:
1. **Build Verification**: Ensure `dotnet build` completes successfully
2. **Package Creation**: Verify NuGet package generation in bin/Debug folders
3. **API Surface**: Confirm key types are available (RepositoryContainer, Project, Publisher, User models)

## Common Tasks

### Repository Structure
```
WindowsAppCommunity.Sdk/
├── .github/workflows/           # CI/CD pipelines
│   ├── build.yml               # Build validation (Ubuntu, .NET 9.0)
│   └── publish.yml             # Package publishing
├── src/                        # Main SDK source code
│   ├── Models/                 # Data models (Project, Publisher, User, etc.)
│   ├── Nomad/                  # IPFS/distributed storage implementations
│   └── WindowsAppCommunity.Sdk.csproj  # Main project file
├── tests/                      # MSTest-based test suite
│   ├── BasicTests.cs          # Basic repository operations
│   ├── ProjectTests.cs        # Project management tests
│   ├── PublisherTests.cs      # Publisher operations tests
│   ├── UserTests.cs           # User management tests
│   └── TestSetupHelpers.cs    # IPFS Kubo bootstrapping utilities
└── WindowsAppCommunity.Sdk.sln # Solution file
```

### Key Components
- **RepositoryContainer**: Central container managing Project, Publisher, and User repositories
- **Models**: Data structures for Projects, Publishers, Users with IPFS content addressing
- **Nomad Layer**: IPFS-backed implementations providing distributed storage capabilities
- **Test Infrastructure**: MSTest framework with IPFS Kubo integration for distributed testing

### Frequent Commands Reference
```bash
# View repository root structure
ls -la

# Check .NET version
dotnet --version

# Clean previous builds
dotnet clean

# Restore packages
dotnet restore

# Build with detailed output
dotnet build --verbosity detailed

# Build for Release
dotnet build -c Release

# Run specific test class
dotnet test --filter "ClassName=BasicTests"

# Generate NuGet package (requires Release build first)
dotnet build -c Release
dotnet pack -c Release

# Check IPFS version
ipfs version
```

### Project Dependencies
Key NuGet packages:
- **OwlCore.Nomad.Kubo**: IPFS/Kubo integration for distributed storage
- **Microsoft.Bcl.AsyncInterfaces**: Async programming support
- **System.Linq.Async**: Async LINQ operations
- **MSTest.TestFramework**: Testing framework

### Development Notes
- **Early Development**: Project is version 0.0.0 and marked as "work in progress"
- **Multi-Targeting**: Supports both .NET Standard 2.0 and .NET 9.0
- **IPFS Dependency**: Core functionality relies on IPFS for distributed content addressing
- **Network Requirements**: Full functionality requires internet connectivity for IPFS operations
- **No Linting**: No specific linting configuration currently in place

### CI/CD Pipeline
The GitHub Actions workflow (`.github/workflows/build.yml`) performs:
1. .NET 9.0 SDK installation
2. Repository checkout
3. `dotnet build /r` (recursive build)
4. `dotnet test` execution

### Common Issues
- **Network Connectivity**: Tests fail without access to IPFS infrastructure (dist.ipfs.tech)
- **.NET Version**: Requires .NET 9.0 SDK specifically (will not work with .NET 8.0 or earlier)
- **IPFS Binary**: Tests automatically download IPFS Kubo binary on first run if network allows
- **Long Test Times**: IPFS-based tests can take 30+ minutes when fully functional

### Performance Expectations
- **Restore**: ~1 second
- **Build (Debug)**: ~3 seconds (NEVER CANCEL - set 60+ second timeout)
- **Build (Release)**: ~3 seconds (NEVER CANCEL - set 60+ second timeout) 
- **Test**: 1-2 minutes (failure due to network) or 30+ minutes (full execution)
- **Package**: ~2-3 seconds (after Release build)

Always allow adequate time for operations and avoid canceling builds or tests prematurely.