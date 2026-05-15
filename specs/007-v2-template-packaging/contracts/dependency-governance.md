# Contract: Dependency Governance

This contract defines how V2 governs direct package versions and dependency metadata.

## Central Package Policy

`Directory.Packages.props` must enable NuGet Central Package Management and declare direct package versions with `<PackageVersion />` entries.

Project files should use versionless package references:

```xml
<PackageReference Include="Expecto" />
```

Project files must not declare inline external dependency versions:

```xml
<PackageReference Include="Expecto" Version="10.2.2" />
```

## Required Metadata

`docs/dependencies.md` must list every centrally governed direct dependency.

| Field | Required | Notes |
|---|---|---|
| Package ID | Yes | Must match `Directory.Packages.props`. |
| Version | Yes | Must match central policy. |
| Purpose | Yes | Why the package is required. |
| Owner | Yes | Maintainer or responsibility group. |
| License posture | Yes | Accepted license or review note. |
| Upgrade expectation | Yes | Review cadence or command/process. |
| Preview risk | Conditional | Required for preview packages such as SkiaSharp preview builds. |

## Validation-Only Exceptions

Inline or property-driven package versions are allowed only when all conditions hold:

- The reference validates a locally packed package or generated package-smoke path.
- The exception is documented in `docs/dependencies.md`.
- `DependencyReport` includes the exception in its output.
- The exception does not govern an external runtime/test dependency.

Current expected exception category:

```text
FsSkiaUiPackageVersion for local package smoke/sample package-reference validation
```

## Required Target

`./fake.sh build -t DependencyReport`

The target must:

- scan all repository-owned `.fsproj` files,
- fail on unmanaged inline external dependency versions,
- confirm central package entries exist for direct package references,
- confirm dependency metadata exists for every governed dependency,
- confirm validation-only exceptions are documented,
- write `specs/007-v2-template-packaging/readiness/dependencies.md`.

## Pass Criteria

- 100% of direct external dependency versions are governed centrally.
- 100% of validation-only exceptions are documented.
- 100% of governed dependencies have required metadata.
- Preview packages have explicit preview-risk notes.
