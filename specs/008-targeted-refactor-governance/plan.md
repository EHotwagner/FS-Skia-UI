# Implementation Plan: Targeted Refactor and Governance Diagnostics

**Branch**: `008-targeted-refactor-governance` | **Date**: 2026-05-15 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/008-targeted-refactor-governance/spec.md`

## Summary

Deliver a bounded refactor and governance diagnostics pass that reduces review risk without changing the public library contract. The implementation keeps `src/Lib/Library.fsi` and surface baselines stable, separates the large runtime implementation into signature-governed internal responsibility areas, introduces explicit Vulkan resource ownership and staged startup cleanup evidence, strengthens generated guidance and template drift validation from shallow substring/path checks to semantic section and alignment checks, emits Yoga fallback diagnostics through the existing layout diagnostic surface when possible, and records a structured public record invariant review with follow-up proposals for any public API work.

The feature intentionally avoids broad rewrite, package identity changes, new distribution behavior, public constructor/validation APIs, new rendering capabilities, and runtime public API changes. If a required diagnostic or invariant fix cannot fit the existing public surface, the implementation must stop at evidence and a follow-up API proposal.

## Technical Context

**Language/Version**: F# on .NET `net10.0`; local SDK observed as `10.0.300`; FAKE build script in F#; Bash and Windows command wrappers
**Primary Dependencies**: Existing `Fable.Elmish`, Silk.NET Vulkan/windowing packages, SkiaSharp preview packages, Yoga.Net layout dependency, Expecto test projects, FAKE repo-local tool, Spec Kit extensions, `dotnet fsi`, and existing central package governance
**Storage**: Filesystem only: source files, `.fsi` signatures, surface baselines under `readiness/surface-baselines/`, feature readiness artifacts under `specs/008-targeted-refactor-governance/readiness/`, generated governance reports, drift reports, and follow-up proposal records
**Testing**: Expecto semantic tests, deterministic injectable native acquisition failure tests, existing real native smoke coverage where available, surface baseline checks, FSI/packed-library evidence, FAKE workflow self-checks, structured generated guidance fixtures, semantic template drift fixtures, Yoga fallback diagnostics tests or follow-up deferral evidence, and public record invariant inventory validation
**Target Platform**: Windows and Linux developer/CI environments that can run the current .NET solution, FAKE targets, and non-visual tests; real Vulkan smoke evidence remains environment-aware and must distinguish implementation defects from missing presentation/GPU setup
**Project Type**: Governed F# library and template repository with runtime source, layout/charts libraries, samples, tests, scripts, local build workflow, and Spec Kit workflow assets
**Performance Goals**: Preserve existing `Dev`, `Verify`, and `Ci` command availability and keep non-visual verification within the existing repository workflow expectations. No numeric performance gate is introduced for this refactor; readiness evidence must record any observed startup, frame smoke, layout, or governance-scan regression from existing verification commands, and reviewers decide whether the regression is avoidable before merge readiness.
**Constraints**: `src/Lib/Library.fsi`, documented public APIs, samples, and package surface baselines remain stable; no broad rewrite; no package identity/version change; no new public validation-first APIs; no new rendering capability; no template packaging expansion; result computation expressions are allowed but advanced F# features require plan/spec justification; synthetic injectable tests must be disclosed and paired with real evidence where available
**Scale/Scope**: One repository source of truth; one large runtime implementation currently around 2354 lines; one FAKE workflow script currently around 956 lines; one drift script currently around 212 lines; generated guidance and drift checks cover active and preset Spec Kit templates plus template-owned source/docs/tests/scripts/build paths

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. The feature has an explicit Tier 1 governance and observable diagnostics scope with constrained Tier 2 internal runtime refactor. `Library.fsi` is expected to remain unchanged. Any public diagnostic or constructor/validation API gap becomes a follow-up proposal instead of an implementation change in this feature. Failing-first tests must cover surface stability, staged cleanup, guidance diagnostics, drift diagnostics, Yoga fallback diagnostics or deferral, and record invariant inventory before implementation.
- **Principle II - Visibility Lives in `.fsi`**: PASS. `src/Lib/Library.fsi` remains the public consumer contract. Any new split implementation files that expose cross-file helpers must have paired `.fsi` signatures that declare only the intended internal helper contract; no top-level visibility modifiers are introduced in `.fs` files. Surface baseline evidence must prove no accidental public exports.
- **Principle III - Idiomatic Simplicity**: PASS. The design uses plain F# modules, records, discriminated unions, `Result`, a standard `result` computation expression or equivalent linear pipeline for startup, and existing Expecto/FAKE patterns. No custom operators, SRTP, reflection, type providers, or non-standard computation expressions are planned.
- **Principle IV - Elmish/MVU Boundary**: PASS. Runtime user-facing workflow remains wrapped by existing `ViewerProgram`, `ViewerEffect`, `ViewerEvent`, and `Viewer.run`. Vulkan startup and resource cleanup are represented as named internal startup stages and ownership effects interpreted at `VulkanHost.run`. Build/governance changes preserve the existing `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, and edge `interpret` boundary.
- **Principle V - Synthetic Evidence Disclosure**: PASS. Deterministic injectable native acquisition failures may use fake/instrumented acquisition to force each failure stage. Those tests must be disclosed at test/readiness surfaces and cannot be the only merge evidence for native behavior; existing real native smoke coverage remains required where available. If real native smoke cannot run in the environment, the readiness report must distinguish environment limits from implementation defects and use the repository synthetic evidence policy.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Behavior-changing refactors require failing-first tests before implementation. Existing semantic, package, layout, smoke, and governance tests must continue to pass, and new governance fixture tests must fail on seeded missing sections, missing prompts, parity mismatches, unaligned path-class drift, invalid deferrals, resource leaks, and missing record inventory entries.
- **Principle VII - Observability and Safe Failure**: PASS. Startup failures name the failing stage and preserve the original diagnostic cause. Cleanup evidence records acquired/released resources exactly once. Guidance and drift failures name missing sections, prompts, path classes, and required alignment classes. Yoga fallback evidence emits an actionable diagnostic through existing fields or records a follow-up API blocker.
- **Change Classification**: PASS. Tier 1 governance and observable diagnostics change with constrained Tier 2 internal runtime refactor; no authorized public API change.
- **Engineering Constraints**: PASS. F#/.NET remains the exclusive stack. Existing SkiaSharp/Silk.NET/Yoga dependency governance remains unchanged. Pack output conventions and Spec Kit evidence gates remain intact.

## Project Structure

### Documentation (this feature)

```text
specs/008-targeted-refactor-governance/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- public-surface-stability.md
|   |-- native-startup-cleanup.md
|   |-- governance-diagnostics.md
|   `-- layout-diagnostics-and-record-invariants.md
|-- checklists/
|   `-- requirements.md
`-- readiness/
    |-- public-surface.txt
    |-- semantic-tests.txt
    |-- native-startup-cleanup.md
    |-- native-startup-cleanup-tests.txt
    |-- native-smoke.txt
    |-- build-organization.md
    |-- generated-guidance.md
    |-- template-drift.md
    |-- yoga-fallback-diagnostics.txt
    |-- record-invariants.md
    `-- follow-ups.md
```

### Source Code (repository root)

```text
src/Lib/
|-- Library.fsi                         # stable public consumer signature
|-- Library.fs                          # public facade and unchanged public type/function contract
|-- RuntimeDiagnostics.fsi              # internal helper contract if split needs cross-file diagnostics
|-- RuntimeDiagnostics.fs
|-- SceneModel.fsi                      # internal helper contract if scene representation moves
|-- SceneModel.fs
|-- VulkanResources.fsi                 # resource ownership helpers and cleanup rules
|-- VulkanResources.fs
|-- VulkanStartup.fsi                   # named startup stages and acquisition pipeline
|-- VulkanStartup.fs
|-- VulkanFrame.fsi                     # frame/screenshot helpers if moved out of Library.fs
`-- VulkanFrame.fs

src/Layout/
|-- Types.fsi                           # existing diagnostic public surface remains stable if possible
|-- Types.fs
|-- Layout.fsi
`-- Layout.fs                           # Yoga failure fallback diagnostics through existing LayoutDiagnostic

build.fsx                               # attempt physical split; otherwise named concern sections
fake.sh
fake.cmd

scripts/
`-- template-drift.fsx                  # semantic path-class and alignment validation

tests/Lib.Tests/
|-- Tests.fs
`-- NativeStartupCleanupTests.fs

tests/Layout.Tests/
|-- Tests.fs
`-- YogaFallbackDiagnosticsTests.fs

tests/Governance.Tests/
|-- GeneratedGuidanceTests.fs
|-- TemplateDriftTests.fs
|-- CommandContractTests.fs
`-- PublicRecordInvariantTests.fs
```

**Structure Decision**: Keep `Library.fsi` as the stable public facade and split implementation only where F# compile order, signature pairing, and surface baseline evidence prove no accidental public exports. Any split helper `.fsi` files are compile-order contracts, not new public consumer surface. They MUST either be assembly-internal in the signature file and excluded from public surface baselines, or the split MUST fall back to named sections inside `Library.fs`. Surface tests must fail on any unapproved new package-visible module, value, or type. If F# signature/file constraints make a proposed split brittle, keep the affected code in `Library.fs` under clear internal responsibility sections and record fallback evidence. Build script splitting follows the same rule: attempt a physical concern split, accept only if `Dev`, `Verify`, and `Ci` load cross-platform, otherwise keep one canonical `build.fsx` with named sections and fallback evidence.

## Complexity Tracking

No constitution violations require justification. The only conditional complexity is the physical split of `build.fsx` and `Library.fs`; both have explicit fallback rules and evidence requirements.

## Phase 0: Research

See [research.md](./research.md). Decisions cover signature-governed runtime splitting, native ownership helpers, staged startup flow, deterministic failure injection with synthetic disclosure, FAKE physical split fallback, structured generated guidance validation, semantic template drift validation, Yoga fallback diagnostics through existing `LayoutDiagnostic` fields, and public record invariant inventory.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/public-surface-stability.md](./contracts/public-surface-stability.md), [contracts/native-startup-cleanup.md](./contracts/native-startup-cleanup.md), [contracts/governance-diagnostics.md](./contracts/governance-diagnostics.md), [contracts/layout-diagnostics-and-record-invariants.md](./contracts/layout-diagnostics-and-record-invariants.md), and [quickstart.md](./quickstart.md).

Design summary:

- `Library.fsi` is the stable contract; public records and modules remain source-compatible. Surface baselines and package checks fail the feature if public exports change without a separate approved spec.
- Runtime implementation is split by responsibility only behind signature-governed internal helper contracts or, if physical split is not compile-stable, grouped into named sections inside `Library.fs`.
- Vulkan native resources use small ownership helpers that record owner, acquire stage, transfer point, release action, and disposal state. Deterministic staged acquisition tests prove each acquired resource is released exactly once after later-stage failures.
- `VulkanHost.run` startup becomes a named sequence of startup stages using `Result` or a standard `result` computation expression so failure propagation is linear, diagnostic causes are preserved, and cleanup obligations are clear.
- `build.fsx` keeps the existing `BuildModel`/`BuildMsg`/`BuildEffect`/`update`/`interpret` boundary. A physical split is attempted only if the FAKE entry script remains canonical and `Dev`, `Verify`, and `Ci` load on Windows and Linux; otherwise named sections are the accepted design.
- `GeneratedGuidanceCheck` validates Markdown section headings, required prompts within the correct sections, deferred-scope placement, and active/preset parity instead of substring-only presence.
- `TemplateDrift` classifies changed template-owned paths, maps each path class to required alignment classes, and validates same-diff alignment files plus active feature spec/plan/readiness evidence naming the changed path or affected feature area, or an accepted deferral record.
- Yoga execution failure in layout evaluation emits a structured fallback diagnostic through existing `LayoutDiagnostic` fields when sufficient. The planned encoding is `Code = FallbackBoundsApplied`, `Severity = Warning`, `Constraint = Some "yoga"`, `FallbackApplied = true`, with an actionable message and node/context where available. If this is insufficient without changing `.fsi`, implementation records a follow-up API proposal and does not alter the signature.
- Public record invariants are reviewed in `specs/008-targeted-refactor-governance/readiness/record-invariants.md`; helper constructor or validation-first recommendations require follow-up IDs and remain out of implementation scope.

## Constitution Check - Post Design

- **Principle I**: PASS. Contracts define public surface stability, native cleanup, governance diagnostics, Yoga fallback behavior, and invariant review before tasks/implementation.
- **Principle II**: PASS. The design requires paired signatures or single-file fallback for split internals and explicit surface baseline evidence.
- **Principle III**: PASS. The staged startup approach uses standard `Result`/`result` patterns and avoids non-standard abstractions.
- **Principle IV**: PASS. Runtime and build I/O remain modeled behind existing or internal stage/effect boundaries, with interpreters at `VulkanHost.run` and FAKE `interpret`.
- **Principle V**: PASS. Synthetic/instrumented native failure tests are disclosed and paired with real smoke evidence where available.
- **Principle VI**: PASS. Failing-first tests are required for every behavior or governance change.
- **Principle VII**: PASS. All failure modes require actionable diagnostics and readiness artifacts.
- **Engineering Constraints**: PASS. F#/.NET stack, package governance, and local workflow conventions remain unchanged.
