# Feature Specification: Dependency Updates ("update all if possible")

**Feature Branch**: `115-dependency-updates`
**Created**: 2026-06-13
**Status**: Draft
**Input**: User description: "update all if possible"

## Context: current vs. available (audit 2026-06-13)

| Dependency | Current pin | Latest | Gap | Disposition |
|---|---|---|---|---|
| spec-kit (`speckit_version`) | 0.8.16 | 0.10.2 | minor ×2 | **Update — safe** |
| FSharp.Core | 10.1.300 | 10.1.301 | patch | **Update — safe** |
| Microsoft.Extensions.FileSystemGlobbing | 10.0.8 | 10.0.9 | patch | **Update — safe** |
| .NET SDK | 10.0.300 | 10.0.301 | patch | **Update — safe** (no `global.json` pin; floats) |
| SkiaSharp (+ NativeAssets) | 4.147.0-preview.3.1 | 4.147.0-preview.3.1 | — | Already current (deliberate preview line) |
| Silk.NET.* | 2.23.0 | 2.23.0 | — | Already current |
| Yoga.Net | 3.2.3 | 3.2.3 | — | Already current |
| FsCheck | 3.3.3 | 3.3.3 | — | Already current |
| DiffPlex | 1.9.0 | 1.9.0 | — | Already current |
| FSharp.SystemTextJson | 1.4.36 | 1.4.36 | — | Already current |
| fsdocs-tool | 22.1.0 | 22.1.0 | — | Already current |
| XParsec | 1.0.0 | 1.0.0 | — | Already current |
| Fake.Core.Target / Fake.IO / Fake.Tools.Git | 6.1.4 | 6.1.4 | — | Pinned to `build.fsx.lock`; out of scope |
| YamlDotNet | 17.1.0 | 18.0.0 | **major** | **Hold** — breaking-change review required |
| Fable.Elmish | 4.2.0 | 5.0.2 | **major** | **Hold** — load-bearing MVU; breaking review required |
| Expecto | 10.2.2 | 11.0.0 | **major** | **Hold** — test framework; review required |
| Microsoft.NET.Test.Sdk | 17.11.1 | 18.6.0 | **major** | **Hold** — review required |
| YoloDev.Expecto.TestSdk | 0.15.3 | 1.0.0 | **major** | **Hold** — pairs with Expecto |
| FSharp.Core (11.x line) | 10.1.300 | 11.0.101-preview5 | **major** | **Hold** — tied to newer F#/SDK; not drop-in |

"Update all **if possible**" is read as: apply every update that is safe and behavior-preserving now; for each major bump, attempt it and keep it **only if it proves drop-in** under the repository's own gates, otherwise document and defer rather than force.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Maintainer brings safe dependency pins current (Priority: P1)

A maintainer wants the repository's dependency pins to be as current as they can be
without changing product behavior. They run the routed validation gates and expect
the safe patch/minor bumps (spec-kit, FSharp.Core, FileSystemGlobbing, .NET SDK) to
be applied with all gates still green and zero `.fsi` / golden / behavior delta.

**Independent test**: Apply only the four safe bumps, run the gates `Route` prints,
confirm all pass and no public-surface baseline, golden, or generated-product diff
appears. This story is independently shippable on its own.

### User Story 2 - Maintainer evaluates each major bump for drop-in safety (Priority: P2)

For each held major bump (YamlDotNet, Fable.Elmish, Expecto + test SDKs), the
maintainer attempts the update on a throwaway basis and observes whether the build,
tests, and governance gates remain green with no source changes. A bump that is
clean drop-in is adopted; one that requires code changes or breaks a gate is reverted
and recorded as deferred with the reason, so the decision is auditable.

**Independent test**: Bump one held package, run the full escalated gate set, and
verify the outcome is either "adopted, all gates green" or "reverted, reason
recorded" — never a half-applied breaking change left in the tree.

### User Story 3 - Generated template stays consistent with the libraries (Priority: P3)

After any package pin changes, the consumer-facing `dotnet new fs-skia-ui` template
and its pins must remain internally consistent with the libraries so a freshly
generated project still restores and builds. The maintainer regenerates/verifies the
template after the bumps.

**Independent test**: After the safe bumps land, run `TemplateCheck` (and generate a
project) and confirm it restores and builds against the updated pins.

### Edge cases

- A bump advertised as patch/minor that nonetheless fails a gate is treated as a
  **major-risk** bump (held), not forced through.
- The SkiaSharp `4.147.0-preview` line is the newest available; staying on it is a
  deliberate choice, not staleness — this feature does **not** move to the parallel
  stable `3.119.x` line.
- The FAKE family is pinned to `build.fsx.lock` (6.1.4) and is **not** bumped here;
  changing it is a separate, coordinated build-tooling change.
- The FSharp.Core 11.x line is a major jump coupled to a newer F#/SDK and is **not**
  in scope.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The four safe bumps MUST be applied: spec-kit `0.8.16 → 0.10.2`,
  FSharp.Core `10.1.300 → 10.1.301`, Microsoft.Extensions.FileSystemGlobbing
  `10.0.8 → 10.0.9`, and acknowledgement of .NET SDK `10.0.300 → 10.0.301`.
- **FR-002**: After applying the safe bumps, every gate that `./fake.sh build -t Route`
  prints for the change MUST pass.
- **FR-003**: The safe bumps MUST introduce **zero** public-contract delta — no `.fsi`
  signature change, no surface-baseline change, no golden/screenshot diff, and no
  generated-product behavior change.
- **FR-004**: Each held major bump (YamlDotNet, Fable.Elmish, Expecto,
  Microsoft.NET.Test.Sdk, YoloDev.Expecto.TestSdk, FSharp.Core 11.x) MUST be either
  (a) adopted only if it is verified drop-in with all gates green and no source change,
  or (b) left at its current pin with the deferral reason recorded.
- **FR-005**: No partially-applied breaking bump may remain in the working tree — a
  major bump that is not adopted MUST be fully reverted to its current pin.
- **FR-006**: The consumer-facing template pins MUST remain consistent with the
  libraries after the change such that a generated project restores and builds.
- **FR-007**: The spec-kit version recorded in `.specify/init-options.json`
  (`speckit_version`) MUST match the version actually in use after the bump.

> Interacting / conflicting requirements: "update all" vs. "preserve behavior and a
> green tree" — resolution: **behavior preservation and green gates win**. A bump is
> adopted only when it is both newer and proven non-breaking; otherwise it is deferred.
> Completeness of the update is bounded by safety, not the reverse.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package **versions** of external dependencies change
  (`Directory.Packages.props`); repo-owned `FS.Skia.UI.*` package identities and
  contents do **not** change as part of this feature. Template package pins
  (`template/**`) may be refreshed for consistency (US3).
- **Public contract impact**: **None intended.** No `.fsi` signature, documented API,
  sample contract, or per-package surface baseline changes. This is asserted, not
  assumed — surface gates must confirm zero delta.
- **State workflow impact**: None. No effects, commands, subscriptions, or interpreter
  behavior changes. (A held Fable.Elmish major bump would touch this surface — which is
  exactly why it is held behind a drop-in check.)
- **Layout/rendering impact**: None expected. SkiaSharp/Silk/Yoga pins are unchanged,
  so layout, charts, DataGrid, Vulkan/Skia output, and screenshots are unaffected.
- **Evidence obligations**: Real evidence under `specs/115-dependency-updates/` —
  the routed gate output proving green gates, a before/after pin diff, and a recorded
  adopt/defer decision per major bump. `EvidenceAudit` must pass with zero synthetic
  markers.
- **Unsupported scope**: No FAKE-family bump (`build.fsx.lock`-pinned), no SkiaSharp
  line change (stay on `4.147 preview`), no FSharp.Core 11.x adoption, no new feature
  behavior, no platform/distribution/release changes.
- **Build-target impact**: `Route` determines the gate set. Expected:
  `GeneratedGuidanceCheck` and `TemplateCheck` engage if `.specify/**` or `template/**`
  change; `EvidenceGraph` / `EvidenceAudit` run as the merge gate. `DependencyReport`
  may need to reflect the new pins. No new build target is added.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of the four identified safe bumps are applied and every gate
  `Route` prints passes.
- **SC-002**: Public-surface, golden, and generated-product diff is **zero** after the
  safe bumps (measurably: surface baseline and golden gates report no change).
- **SC-003**: Every held major bump has a recorded, auditable disposition —
  "adopted (gates green, no source change)" or "deferred (reason)" — with none left
  half-applied.
- **SC-004**: A freshly generated `dotnet new fs-skia-ui` project restores and builds
  against the updated pins.
- **SC-005**: `.specify/init-options.json` `speckit_version` equals the spec-kit
  version actually in use.

## Assumptions

- "Update all if possible" means *adopt every update that is provably safe now*, not
  *force every available version regardless of breakage*.
- The SkiaSharp `4.147 preview` line is an intentional choice and is to be preserved.
- The FAKE family stays lock-pinned at 6.1.4 (build-tooling, coordinated separately).
- No `global.json` SDK pin exists, so the .NET SDK patch follows the installed
  toolchain rather than a committed pin change.
- Latest versions are as observed from NuGet / GitHub on 2026-06-13; the actual bump
  applies whatever is current at implementation time within the same safe/major
  classification.
