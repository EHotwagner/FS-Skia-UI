# Feature Specification: V3 Stage 5 Closeout — Delete `src/Lib`, Decommission `FS.Skia.UI`, Enforce & Measure

**Feature Branch**: `053-v3-monolith-retirement`  
**Created**: 2026-06-02  
**Status**: Draft  
**Input**: User description: "@docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md implement the next part and update the plan afterwards."

## Context

The V3 modular-distribution programme retires the legacy `FS.Skia.UI` monolith
(`src/Lib`) and finishes the split-package distribution. Stages 0–4 have shipped:

- **Stage 0 (048):** the deterministic scene-output parity oracle and the
  additive `PerPackageSurfaceDiff` target + eight per-package surface baselines
  exist; ADRs 0007–0011 accepted.
- **Stage 1 (050):** the Vulkan/Skia host was extracted into
  `FS.Skia.UI.SkiaViewer/Host`, retyped onto `FS.Skia.UI.Scene`, the
  `SceneConversion.fs` bridge and `SkiaViewer → Lib` reference deleted — the
  modularity leak is closed and parity is byte-identical.
- **Stage 2 (051):** `AgentValidation` relocated `src/Lib` → `FS.Skia.UI.Build`;
  `knownGates` is now governance config rather than runtime code.
- **Stages 3–4 residual (052):** the rich `KeyboardInput` runtime rehomed into a
  new dedicated package `FS.Skia.UI.Input` (the ninth in-scope package);
  `InteractiveViewer` + `Lib.Tests` decoupled; the `Parity.Tests` old-vs-new
  bridge retired with the Scene-only scene-output oracle preserved.

This feature is the plan's **Stage 5 closeout** — the final stage. After Stages
1–4, the **only remaining consumer of `src/Lib`** is `tests/Package.Tests`, a
deliberate *packaging-contract* consumer that asserts the still-published
`FS.Skia.UI` surface via `typeof<FS.Skia.UI.ParityReport>.Assembly` (the `Parity`
evidence helper) and the `PackLocal` wiring. `src/Lib` itself is otherwise an
unreferenced husk holding only the `Parity` helper (`Library.fs(i)`),
`InternalsVisibleTo.fs`, and any `VulkanStartup`/`VulkanResources` residue.

The goal of this feature is to **complete the retirement**: decouple that last
consumer, delete `src/Lib`, stop publishing the `FS.Skia.UI` package, lock in the
per-package surface baselines as a Route-gated merge gate (now unblocked because
`knownGates` left the runtime in Stage 2), add a generated-project cleanliness
gate, publish the V2→V3 migration docs, and produce the after-measurement that
closes the programme.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - The monolith is gone (Priority: P1)

As the maintainer retiring the monolith, I need `src/Lib` deleted and the
`FS.Skia.UI` package no longer published, so that the V3 modular distribution is
the sole shipped form — no broad legacy core, no duplicate scene vocabulary, no
dependency-light promise gap. The build, the template, and all tests stay green
with nothing anywhere referencing `Lib` or the `FS.Skia.UI` monolith package.

**Independent test:** A repository-wide search for `Lib.fsproj`,
`src/Lib`, and the `FS.Skia.UI` monolith package (by `ProjectReference` /
`PackageReference` / `PackageId` / `PackLocal` entry) returns zero hits across
`src/**`, `samples/**`, `tests/**`, `template/**`, `build/**`, and
`Directory.Packages.props`. `src/Lib` no longer exists on disk and is not in the
solution. The full escalated gate set is green and `EvidenceAudit` is PASS.

### User Story 2 - Per-package surface baselines are an enforced merge gate (Priority: P2)

As a contributor changing a package's public API, I want an unrecorded
per-package `.fsi` change to be **caught by `Route` and fail the gate**, so that
each package's public contract cannot silently drift — the design's acceptance
criterion. Recording the change in that package's per-package surface baseline is
what makes the gate pass.

**Independent test:** `Route` selects `PerPackageSurfaceDiff` for a change that
touches a public `.fsi` (the rule is rendered into `validation.contract.yml` and
its gate name is on the known-gate allowlist). A deliberate, reverted one-package
`.fsi` edit without a baseline update fails `PerPackageSurfaceDiff`; updating the
baseline makes it pass.

### User Story 3 - A generated app is clean (Priority: P2)

As a consumer running `dotnet new fs-skia-ui`, I get a project that **references
packages** rather than copying framework internals — no `samples/`, no framework
documentation set, no historical `specs/`, no framework README copy — so the
generated product is a clean consumer of the distribution, not a fork of the repo.

**Independent test:** A freshly generated default `app` (and the `governed`
profile) is asserted by a cleanliness gate to contain no `samples/`, no framework
docs set, no `specs/`, no framework README copy, and to reference the split
packages rather than copying framework projects. The gate fails if any of those
appear.

### User Story 4 - A V2 consumer can migrate (Priority: P3)

As an existing `FS.Skia.UI` (V2) consumer, I can follow a migration guide that
maps the old monolith surface to the split packages and tells me how to move my
package references, so that upgrading to V3 is a documented, mechanical change
rather than a reverse-engineering exercise.

**Independent test:** `docs/` contains a V2→V3 migration document with a surface
map (old `FS.Skia.UI` → `FS.Skia.UI.Scene` / `.SkiaViewer` / `.Elmish` /
`.KeyboardInput` / `.Input` / `.Layout` / `.Controls`), the package-reference
move steps, the removed-`SceneConversion` note, and the keyboard-input →
`FS.Skia.UI.Input` mapping.

### Edge Cases

- **`Package.Tests` packaging-contract assertions must be rewritten, not just
  dropped.** They currently assert the *published* `FS.Skia.UI` surface
  (`typeof<FS.Skia.UI.ParityReport>.Assembly`, the `VulkanResources`/
  `VulkanStartup` non-exports, the `PackLocal` `src/Lib/Lib.fsproj` → `FS.Skia.UI`
  entry). Once `FS.Skia.UI` is unpublished those assertions are meaningless and
  must be retired or re-pointed at the split packages — the suite must still be
  green and still assert a real packaging contract.
- **The `Parity` evidence helper retires with the monolith.** Removing it
  (`src/Lib/Library.fs(i)`) is what unblocks deleting `src/Lib`; nothing outside
  `Package.Tests` references it after Stage 4.
- **File-path references, not just symbols/namespaces.** Per the Stage 2 lesson, a
  relocated/deleted file can be referenced by **path string** in a packable-project
  fsi enumeration (e.g. `AsteroidsFeedbackSkillGuidanceTests`) — those path
  references MUST be removed too, not just symbol/namespace `open`s. A grep for the
  **path** as well as the symbol is required.
- **Route-gating the new target was deferred for a reason.** Adding the
  `PerPackageSurfaceDiff` rule to `Routing.fs` renders it into
  `validation.contract.yml`'s `required_gates`; the contract validator's
  known-gate allowlist (`knownGates`) must already recognise the gate. Stage 2
  moved `knownGates` into `FS.Skia.UI.Build`, so this is now a governance/build
  edit with **no `src/**` runtime change** — but the rule, its rendering, and the
  allowlist entry must land together or the contract currency check
  (`TargetMetadataDrift`) fails.
- **Stale `knownGates` comment.** `build/Governance/Routing.fs:214` still points at
  `src/Lib/AgentValidation.fs` `knownGates` (left deliberately in Stage 2 for the
  Routing currency contract); it MUST be corrected when `src/Lib` is deleted.
- **`ParityGallery` and the Scene-only parity oracle** must be settled: the
  scene-output oracle is preserved in the split-package suites (per Stage 4), and
  `ParityGallery` is retired or kept per ADR 0010; governance scanning lists that
  name `tests/Parity.Tests` must be cleaned.
- **Solution / pack-flow residue.** `Lib` must be removed from the solution, from
  `packProjects` (Helpers.fs), from the pack-version flow, and from
  `docs/reports/dependencies.md`; no `Directory.Packages.props` or template pin may
  still name `FS.Skia.UI`.
- **Revertibility.** Deletion is git-revertible until the package is unpublished;
  `Lib` is kept recoverable behind the solution until the after-measurement signs
  off.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The last `src/Lib` consumer (`tests/Package.Tests`) MUST be
  decoupled: its monolith *packaging-contract* assertions
  (`typeof<FS.Skia.UI.ParityReport>.Assembly`, the `VulkanResources`/
  `VulkanStartup` non-exports, the `PackLocal` `src/Lib/Lib.fsproj` → `FS.Skia.UI`
  entry) MUST be rewritten against the split packages or retired with
  justification, and its `Lib.fsproj` reference dropped. The suite MUST remain
  green and still assert a real packaging contract.
- **FR-002**: The `Parity` evidence helper MUST be removed from
  `src/Lib/Library.fs(i)`.
- **FR-003**: `src/Lib` MUST be deleted (`Library.fs(i)`, `InternalsVisibleTo.fs`,
  any `VulkanStartup`/`VulkanResources` residue) and removed from the solution.
- **FR-004**: The `FS.Skia.UI` monolith MUST stop being published: its
  `IsPackable`/`PackageId` removed, dropped from `PackLocal` / `packProjects`
  (Helpers.fs) / the pack-version flow / `docs/reports/dependencies.md`, and from
  any packable fsi-enumeration list (e.g. `AsteroidsFeedbackSkillGuidanceTests`).
- **FR-005**: No `Directory.Packages.props` pin and no template package pin MAY
  still name the `FS.Skia.UI` monolith package.
- **FR-006**: A repository-wide reference search MUST show **zero** remaining
  references to `Lib.fsproj` / `src/Lib` / the `FS.Skia.UI` monolith package
  across `src/**`, `samples/**`, `tests/**`, `template/**`, and `build/**`
  (excluding programme history/docs that intentionally record the retirement).
- **FR-007**: The per-package surface baselines MUST be enforced as a merge gate:
  a `Routing.fs` rule MUST Route-select `PerPackageSurfaceDiff` for changes
  touching a public `.fsi`, the rule MUST be rendered into
  `validation.contract.yml`'s `required_gates`, and the gate name MUST be on the
  `knownGates` allowlist — so an unrecorded per-package `.fsi` change fails the
  gate. `TargetMetadataDrift` (contract currency vs `Routing.fs`) MUST stay green.
- **FR-008**: A generated-project cleanliness gate (an extension of
  `GeneratedProductCheck`, per research R4) MUST assert a generated default `app`
  contains no `samples/`, no framework docs set, no historical `specs/`, no
  framework README copy, and references packages rather than copying framework
  projects.
- **FR-009**: A V2→V3 migration document MUST be published under `docs/` with: a
  table mapping the old `FS.Skia.UI` surface to the split packages
  (`FS.Skia.UI.Scene` / `.SkiaViewer` / `.Elmish` / `.KeyboardInput` /
  `.Input` / `.Layout` / `.Controls`), how to move an app's package references,
  the removed-`SceneConversion` note, and the rich keyboard-input →
  `FS.Skia.UI.Input` mapping. (`.Controls.Elmish` and `.Testing` have no monolith
  public-surface predecessor and are intentionally absent from the surface map.)
- **FR-010**: An after-measurement report MUST be written to
  `docs/reports/_baselines/2026-06-02-v3-after.md` recording: `src/Lib` LOC (→ 0),
  monolith transitive-pull (→ none), duplicate-type count (→ 0), package count,
  per-package surface baselines present, and generated-`app` cleanliness asserted —
  each metric with its reproduction command, mirroring the Stage-0 before-baseline.
- **FR-011**: A closing **ADR 0012** (programme closeout) MUST be written and
  accepted, recording the completed retirement and linking the programme ADRs.
- **FR-012**: The `Parity.Tests` / `ParityGallery` residue MUST be settled per ADR
  0010 (the Scene-only scene-output oracle kept where valuable, `ParityGallery`
  kept-or-retired with the decision recorded), and governance scanning lists that
  name `tests/Parity.Tests` cleaned.
- **FR-013**: The stale `knownGates` comment at `build/Governance/Routing.fs:214`
  pointing at `src/Lib/AgentValidation.fs` MUST be corrected.
- **FR-014**: The generated-consumer contract MUST stay green — `TemplateCheck`,
  `GeneratedProductCheck`, and `GeneratedGuidanceCheck` pass; a generated default
  `app` continues to restore/build/run and references split packages only.
- **FR-015**: The package dependency graph MUST remain acyclic and
  `FS.Skia.UI.Scene` MUST remain FSharp.Core-only; no stage edit may introduce a
  back-edge or a new heavy dependency into a base package.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: The `FS.Skia.UI` monolith package identity is **removed**
  (unpublished; dropped from pack flow, CPM pins, template pins, dependency docs).
  No other package identity changes; the eight/nine split packages keep their
  identities. The standard post-merge two-commit version-bump + template-pin flow
  applies to any packable project whose contents change. No `Charts` package
  migration is in scope (the design's separate `Charts` package remains future
  work).
- **Public contract impact**: The `FS.Skia.UI` per-package and aggregate surface
  baselines are **removed** with the package. `validation.contract.yml` **changes**
  this stage — the new `PerPackageSurfaceDiff` `Routing.fs` rule is rendered into
  `required_gates` and the gate is added to the `knownGates` allowlist
  (`FS.Skia.UI.Build`, not runtime). `TargetMetadataDrift` currency vs `Routing.fs`
  MUST hold. `Package.Tests` packaging-contract assertions are rewritten against
  the split packages.
- **State workflow impact**: No stateful workflow / I/O / command / effect /
  subscription / interpreter behaviour changes — all runtime moved in Stages 1–4;
  this stage deletes the now-dead monolith and adds governance/enforcement. The
  generated `app` runtime behaviour is unchanged (parity already proven).
- **Layout/rendering impact**: No rendering-architecture change. No runtime scene
  code moves this stage. Screenshot/visual re-capture remains
  headless-GPU-infeasible (disclosed; the deterministic scene-output oracle is
  authoritative and already preserved in the split-package suites).
- **Evidence obligations**: The no-consumer grep proof (zero `Lib`/`FS.Skia.UI`
  monolith references repo-wide); the after-measurement report
  (`2026-06-02-v3-after.md`) with reproduction commands; `PerPackageSurfaceDiff`
  enforcement evidence (an unrecorded `.fsi` change fails the gate); the
  generated-project cleanliness gate green on a generated `app`; the V2→V3
  migration doc; ADR 0012; the standard repo-root governance readiness docs and
  per-feature readiness notes for `Route --enforce`; `EvidenceGraph` valid and
  `EvidenceAudit` PASS with zero synthetic.
- **Unsupported scope**: The `Charts`/`DataGrid` package split; new template
  profiles (`headless-scene`, `full-governed`, `sample-pack` as first-class
  switches); any new rendering architecture or dynamic/plugin loader (no FCS, no
  runtime script loading); reference-screenshot re-capture (headless-GPU-infeasible).
- **Build-target impact**: `validation.contract.yml` + `Routing.fs` change
  (the `PerPackageSurfaceDiff` rule); a new/extended generated-project cleanliness
  gate is added (`GeneratedProjectCheck` or `GeneratedProductCheck` extension);
  `PackLocal` / `packProjects` change (drop `FS.Skia.UI`). `Route` selects the
  escalated gate set (the change touches `template/**`, governance paths, and
  public `.fsi`). `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck` /
  `TemplateDrift`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit`
  MUST be green; `TargetMetadataDrift` / `SkillSyncCheck` currency MUST hold.

## Success Criteria *(mandatory)*

- **SC-001**: `src/Lib` no longer exists on disk and is not in the solution; a
  named repo-wide reference search shows **zero** `Lib.fsproj` / `src/Lib` /
  `FS.Skia.UI` monolith references across `src/**`, `samples/**`, `tests/**`,
  `template/**`, and `build/**`.
- **SC-002**: The `FS.Skia.UI` monolith package is no longer packed or published —
  it is absent from `PackLocal` / `packProjects`, `Directory.Packages.props` pins,
  template pins, and `docs/reports/dependencies.md`.
- **SC-003**: `tests/Package.Tests` is green with no `Lib` reference, asserting a
  real packaging contract against the split packages.
- **SC-004**: The per-package surface baselines are enforced — `Route` selects
  `PerPackageSurfaceDiff` on a public-`.fsi` change, a deliberate unrecorded
  one-package `.fsi` edit fails the gate, and recording the baseline makes it pass;
  `validation.contract.yml` reflects the rule and `TargetMetadataDrift` is green.
- **SC-005**: The generated-project cleanliness gate is present and green on a
  freshly generated default `app` (and rejects a planted `samples/` / framework
  docs / `specs/` / README copy).
- **SC-006**: A generated default `app` restores/builds/runs referencing split
  packages only and does not pull any monolith transitively
  (`TemplateCheck`/`GeneratedProductCheck` green).
- **SC-007**: The V2→V3 migration doc and ADR 0012 are published and linked from
  the implementation plan; the after-measurement report records `src/Lib` → 0 LOC,
  duplicate-type count → 0, and monolith transitive-pull → none.
- **SC-008**: The package dependency graph is acyclic and `FS.Skia.UI.Scene` is
  still FSharp.Core-only (verified from project references).
- **SC-009**: The full escalated gate sequence is green and `EvidenceAudit`
  returns PASS with zero synthetic tasks.

## Assumptions

- After Stage 4 (feature 052), the **only** remaining `src/Lib` consumer is
  `tests/Package.Tests` (the packaging-contract suite); every other sample/test is
  already off the monolith. This feature's first job is to decouple that last
  consumer.
- `knownGates` lives in `FS.Skia.UI.Build` (relocated in Stage 2), so adding the
  `PerPackageSurfaceDiff` `Routing.fs` rule and rendering it into
  `validation.contract.yml` is a governance/build edit with **no `src/**` runtime
  change** — the Stage-0 §0.3 deferral is now clean to pick up.
- The nine in-scope split packages are `FS.Skia.UI.Scene`, `.SkiaViewer`,
  `.Elmish`, `.KeyboardInput`, `.Input` (new in 052), `.Layout`, `.Controls`,
  `.Controls.Elmish`, `.Testing`; `Charts`/`DataGrid` stay in `src/Controls`
  (out of scope).
- The deterministic scene-output parity oracle (preserved in the split-package
  suites in Stage 4) remains authoritative; reference-screenshot re-capture is
  headless-GPU-infeasible and corroboration-only (carried from Stages 0–4).
- The post-merge version-bump + template-pin flow (per Stage 2's outcome) applies
  on merge for any package whose contents change.
- Deletion is git-revertible until the package is unpublished; `Lib` is kept
  recoverable behind the solution until the after-measurement signs off.

## Dependencies

- **Stages 1–4 (features 050, 051, 052)** — host extracted, leak closed,
  `AgentValidation` relocated, rich keyboard input rehomed to `FS.Skia.UI.Input`,
  `Parity.Tests` bridge retired, every consumer but `Package.Tests` off `Lib`.
  Done.
- **Stage 0 (feature 048)** — the `PerPackageSurfaceDiff` target + per-package
  baselines and ADRs 0007–0011 this stage enforces and closes. Done.
- **Stage 2 (feature 051)** — `knownGates` relocated; precondition for the
  `PerPackageSurfaceDiff` Route-gating rule (FR-007). Done.
- This is the **final** programme stage — its completion is the whole-programme
  definition of done.
