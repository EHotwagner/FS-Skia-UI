# Phase 1 Data Model: Dependency Updates

This feature has no runtime entities. Its only "data" is the set of dependency pins and
their dispositions — modeled here as the authoritative before/after table that the tasks
act on and the evidence verifies against.

## Entity: Dependency Pin

| Field | Meaning |
|---|---|
| `id` | Package / tool identifier (or `speckit_version`, `dotnet-sdk`). |
| `location` | Where the pin lives (`Directory.Packages.props`, `.specify/init-options.json`, installed SDK). |
| `current` | Version pinned as of 2026-06-13. |
| `target` | Version to move to (or `=` if already current). |
| `class` | `safe` (apply now) \| `held` (adopt-iff-drop-in) \| `current` (no-op) \| `out-of-scope`. |
| `outcome` | Filled during implementation: `applied` \| `adopted` \| `deferred(reason)` \| `unchanged`. |

### Validation rules

- A `safe` pin MUST end `applied` with all routed gates green and zero `.fsi`/golden/
  generated-product diff (FR-002, FR-003).
- A `held` pin MUST end either `adopted` (gates green, no source change) or
  `deferred(reason)` — never half-applied (FR-004, FR-005).
- `speckit_version` in `.specify/init-options.json` MUST equal the spec-kit version
  actually in use after the change (FR-007).
- `current` and `out-of-scope` pins MUST end `unchanged`.

## Pin table (state transitions)

| id | location | current | target | class | expected outcome |
|---|---|---|---|---|---|
| spec-kit (`speckit_version`) | `.specify/init-options.json` | 0.8.16 | 0.10.2 | safe | applied |
| FSharp.Core | `Directory.Packages.props` | 10.1.300 | 10.1.301 | safe | applied |
| Microsoft.Extensions.FileSystemGlobbing | `Directory.Packages.props` | 10.0.8 | 10.0.9 | safe | applied |
| .NET SDK | installed toolchain (no `global.json`) | 10.0.300 | 10.0.301 | safe | applied (floats) |
| YamlDotNet | `Directory.Packages.props` | 17.1.0 | 18.0.0 | held | adopted \| deferred |
| Fable.Elmish | `Directory.Packages.props` | 4.2.0 | 5.0.2 | held | adopted \| deferred |
| Expecto | `Directory.Packages.props` | 10.2.2 | 11.0.0 | held | adopted \| deferred (cluster) |
| Microsoft.NET.Test.Sdk | `Directory.Packages.props` | 17.11.1 | 18.6.0 | held | adopted \| deferred (cluster) |
| YoloDev.Expecto.TestSdk | `Directory.Packages.props` | 0.15.3 | 1.0.0 | held | adopted \| deferred (cluster) |
| FSharp.Core (11.x) | `Directory.Packages.props` | 10.1.300 | 11.0.101-preview5 | out-of-scope | unchanged |
| SkiaSharp (+ NativeAssets ×2) | `Directory.Packages.props` | 4.147.0-preview.3.1 | = | current | unchanged |
| Silk.NET.* (5) | `Directory.Packages.props` | 2.23.0 | = | current | unchanged |
| Yoga.Net | `Directory.Packages.props` | 3.2.3 | = | current | unchanged |
| FsCheck | `Directory.Packages.props` | 3.3.3 | = | current | unchanged |
| DiffPlex | `Directory.Packages.props` | 1.9.0 | = | current | unchanged |
| FSharp.SystemTextJson | `Directory.Packages.props` | 1.4.36 | = | current | unchanged |
| XParsec | `Directory.Packages.props` | 1.0.0 | = | current | unchanged |
| fsdocs-tool | `.config/dotnet-tools.json` | 22.1.0 | = | current | unchanged |
| Fake.* (3) | `Directory.Packages.props` / `build.fsx.lock` | 6.1.4 | = | out-of-scope | unchanged (lock-pinned) |

> Expecto / Microsoft.NET.Test.Sdk / YoloDev.Expecto.TestSdk are a **cluster** — adopt all
> three or none, since the test runner/host/adapter interlock.

## Final dispositions (filled during implementation — T020)

| id | current → target | class | **outcome** | evidence |
|---|---|---|---|---|
| FSharp.Core | 10.1.300 → 10.1.301 | safe | **applied** | Dev green; zero `.fsi`/golden/generated-product diff |
| Microsoft.Extensions.FileSystemGlobbing | 10.0.8 → 10.0.9 | safe | **applied** | Dev green (build-tooling adopt-set, `build/**` only) |
| spec-kit (`speckit_version`) | 0.8.16 → 0.10.2 | safe | **applied** | routed gates green; bumped `init-options.json` (the only FR-007 field) + tracked the governance-test constant. `.specify` install-provenance manifests left at `0.8.16` (no actual 0.10.2 install) — see us1-validation.md |
| .NET SDK | 10.0.300 → (10.0.301) | safe | **unchanged (floats)** | no `global.json`; installed SDK is `10.0.300` (not `10.0.301` as the plan assumed); float follows the installed SDK — honesty-corrected in us1-validation.md |
| YamlDotNet | 17.1.0 → 18.0.0 | held | **adopted** | Dev + GeneratedGuidanceCheck + TemplateDrift green, zero source change |
| Fable.Elmish | 4.2.0 → 5.0.2 | held | **adopted** | Dev + Elmish.Tests/Parity.Tests green, zero source change |
| Expecto | 10.2.2 → 11.0.0 | held (cluster) | **deferred** | `NU1608`: YoloDev 1.0.0 caps Expecto `<10.0.0`, conflicts with Expecto 11 — cluster reverted whole |
| Microsoft.NET.Test.Sdk | 17.11.1 → 18.6.0 | held (cluster) | **deferred** | reverted with the cluster (never partial) |
| YoloDev.Expecto.TestSdk | 0.15.3 → 1.0.0 | held (cluster) | **deferred** | YoloDev 1.0.0 requires Expecto `>=9.0.0 && <10.0.0` — incompatible with both Expecto 11 and the current 10.2.2 |
| FSharp.Core (11.x) | 10.1.300 → 11.0.101-preview5 | out-of-scope | **unchanged** | not attempted — tied to a newer F#/SDK, not drop-in on `net10.0` |
| SkiaSharp (+ NativeAssets ×2) | 4.147.0-preview.3.1 → = | current | **unchanged** | deliberate preview line; next candidates are older stable 3.119.x |
| Silk.NET.* / Yoga.Net / FsCheck / DiffPlex / FSharp.SystemTextJson / XParsec / fsdocs-tool | = | current | **unchanged** | already current |
| Fake.* (3) | 6.1.4 → = | out-of-scope | **unchanged** | `build.fsx.lock`-pinned |

**No half-applied breaking bump remains** (FR-005): `git diff Directory.Packages.props` shows only the two
applied safe pins + the two adopted held pins (YamlDotNet 18, Fable.Elmish 5); the deferred cluster is back
at its current pins.
