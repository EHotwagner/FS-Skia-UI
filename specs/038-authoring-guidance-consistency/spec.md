# Feature Specification: Authoring Guidance Consistency

**Feature Branch**: `038-authoring-guidance-consistency`
**Created**: 2026-05-30
**Status**: Draft
**Input**: User description: "Problems encountered while developing an app on FS Skia UI — a dangling skill id in the task-generation hints, generated skills/tests that are not domain-agnostic or consumer-facing, no local API surface for consumers (forcing DLL reflection), public names that collide on `open`, tuple-heavy inconsistent scene constructors, and an under-documented effects boundary. Mouse/pointer input, a headless raster backend, and dotnet fsi window/font usability are explicitly out of scope."

## Overview

A developer using FS.Skia.UI to build a new application (a tower-defense game)
hit a cluster of authoring-time friction points. Almost none were defects in the
running application — they were inconsistencies and gaps in the *guidance and
public surface* the project presents to an author: a skill id advertised by the
task generator that resolves to nothing, generated skills and starter tests that
carry framework-internal or demo-specific content instead of consumer-facing
guidance, no local API reference to read (so the author reflected the compiled
DLLs to recover union-case shapes), public names that collide when a consumer
`open`s a framework namespace, and an effects model documented only in passing.

This feature hardens that surface so an author who follows the project's own
hints, skills, and public API — without reflecting DLLs or reading scattered
design reports — can assign skills that resolve, discover the API locally,
understand the effects boundary, avoid name collisions, and start from
domain-agnostic scaffolding.

**Grounding note.** Several specific traps the author reported (a widgets skill
declaring `name: tddemo1-widgets`, a `fs-skia-layout` hint with no matching
skill) were properties of their *generated* project and are **not reproducible
in the current repository** — in this repo `fs-skia-layout` and
`fs-skia-ui-widgets` resolve to real declared skills (`src/Layout/skill`,
`src/Controls/skill`). The durable response is therefore a **resolution guard**
that fails when any advertised id dangles or drifts from its skill's declared
`name:`, rather than a one-off rename. One dangling id (`speckit-debug-loop`) is
live in this repository today. Likewise, the evidence-gate feature-targeting
problem the author reported was already fixed by feature 037
(`build.fsx` resolves via `.specify/feature.json` and refuses placeholder
fallback); this feature only adds a regression guard for it, not new work.

**Priority principle.** The generated consumer project is the primary purpose of
the framework. It has absolute priority: it must be feature-complete and must not
suffer at the expense of the framework repo's own development process.
Consumer-facing requirements outrank framework-repo-process requirements
throughout this spec, and the latter must never block or delay the former.

## Clarifications

### Session 2026-05-30

- Q: Guiding priority between the generated consumer project and the framework repo's dev process? → A: The generated consumer project has absolute priority — feature-complete, never sacrificed for framework-repo-process work.
- Q: What form should the local, authoritative API reference in a generated project take (FR-004)? → A: Bundle the real public `.fsi` signature files (or an `api-surface.fsi`/`.md` generated verbatim from them) into the generated project — exact signatures, no reflection.
- Q: How to treat framework-repo-only items (FR-001 dangling `speckit-debug-loop` hint, FR-011 evidence-gate regression guard) given the consumer-priority steering? → A: Keep them in this feature but at the lowest priority (P3), strictly after all consumer-facing work; they must never block or delay consumer deliverables.
- Q: Compatibility requirement for the name-collision hardening (FR-008)? → A: A breaking change is acceptable — apply the hardening (e.g. `[<RequireQualifiedAccess>]`), document a migration note, bump package versions, and update all generated samples. Existing consumers migrate on upgrade; freshly generated projects get the clean surface.
- Q: Should there be a top-level success criterion that a freshly generated project is end-to-end feature-complete using only local references? → A: Yes — add it as the primary, governing success criterion (SC-001); all other success criteria are subordinate to it.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Every advertised skill id resolves (Priority: P1)

An author (or the task generator) reads the skill-assignment hints, copies a
skill id, and emits a `skill:` annotation. The id resolves against a real skill's
declared `name:` on the first try, and the same holds for skills generated into a
consumer project: the id an author naturally reaches for (the one the hints, the
SKILL.md scan phrases, and the harness "available skills" list all advertise)
resolves and matches the skill's directory.

**Why this priority**: This is the highest-frequency trap. `speckit-debug-loop`
is advertised in the hints (`.agents/skills/speckit-tasks/SKILL.md:145,149` and
the `.claude/` mirror) but no skill declares that `name:` anywhere — it dangles
today. In the author's generated project the same class of failure appeared as a
skill whose directory and declared `name:` disagreed. A guard that fails on any
unresolved or drifted id removes the whole class.

**Independent Test**: Run a check that, for every id advertised in the hints and
scan phrases (and for every skill generated into a consumer project), asserts the
id resolves to a declared `name:` and that a skill's directory, declared `name:`,
and advertised id agree. Introduce a deliberately dangling id and confirm the
check fails.

**Acceptance Scenarios**:

1. **Given** the current hints, **When** the resolution guard runs, **Then** it
   flags `speckit-debug-loop` as unresolved (and passes once the reference is
   corrected).
2. **Given** a skill whose directory name and declared `name:` disagree, **When**
   the guard runs, **Then** it fails.
3. **Given** the `.agents/` and `.claude/` copies of a skill, **When** they are
   compared, **Then** they declare the same `name:` and advertise the same id.

### User Story 2 - A consumer can read the API without reflecting DLLs (Priority: P1)

An author needs the exact shape of an API element (for example, a `SceneNode`
union case's field order). They read a reference that is present **in their
generated project**, rather than reflecting the compiled DLLs or learning the
shape through compiler errors.

**Why this priority**: The template bundles no local API reference into generated
projects, and the generated skills point at framework-internal `src/.../X.fsi`
paths that do not exist in a consumer project. Although the framework packages
are built with `GenerateDocumentationFile` and compile `.fsi` signatures, none of
that is surfaced as something an author can open and read locally — which is why
the author resorted to DLL reflection to pin down union-case shapes.

**Independent Test**: In a freshly generated project, locate a local,
human-readable, authoritative reference for the public surface an author is
expected to use, and confirm an author can determine an element's exact shape
from it alone, without reflection.

**Acceptance Scenarios**:

1. **Given** a freshly generated project, **When** the author looks for an API
   reference, **Then** a local authoritative reference is present and does not
   point at a path absent from the project.
2. **Given** that reference, **When** the author needs a specific element's
   shape, **Then** they can determine it without reflecting DLLs.

### User Story 3 - Public names do not collide on `open` (Priority: P2)

A consumer `open`s a framework namespace and defines ordinary names — an
`update`, an `init`, a `Normal` case of their own enum — without the framework's
public names silently shadowing theirs or producing confusing type errors.

**Why this priority**: `ViewerWindowStartupState` exposes a bare `Normal` case
with no `[<RequireQualifiedAccess>]` (`src/SkiaViewer/SkiaViewer.fsi:44-48`),
which collides with a consumer's own `Normal`; viewer/input surfaces expose
`update`/`init` that shadow a consumer's MVU `update`/`init`. The author hit
exactly these collisions. The framework source lives in this repository, so the
public surface can be hardened directly.

**Independent Test**: In a consumer module that `open`s the relevant framework
namespace and defines its own `Normal`, `update`, and `init`, confirm the
consumer's names resolve to the consumer's definitions and the framework's
collide-prone names require qualification.

**Acceptance Scenarios**:

1. **Given** a consumer enum with a `Normal` case and an `open` of the viewer
   namespace, **When** the consumer references `Normal`, **Then** it resolves to
   the consumer's case (the framework's is qualified).
2. **Given** a consumer that defines `update`/`init` after `open`, **When** they
   reference `update`/`init`, **Then** the framework's do not shadow them.

### User Story 4 - Generated guidance is consumer-facing and domain-agnostic (Priority: P2)

An author reads the skills and starter test scaffolding in their generated
project. The skills give consumer-facing usage guidance (how to build a scene,
wire the host, produce evidence) rather than pointing at framework-internal paths
and build targets that do not exist in a consumer project. The starter tests are
written for a generic application, with no leftover demo-specific identifiers.

**Why this priority**: The generated starter tests assert against a "Tetris-style
board" (`template/base/tests/Product.Tests/Tests.fs:430-431`), which misleads an
author about what to edit; generated skills reference framework-internal paths
and targets (`CapabilityCheck`/`PackLocal`/`src/.../X.fsi`) absent from a
consumer project.

**Independent Test**: In a generated project, confirm the starter tests contain
no demo-specific identifiers and the skills contain at least one
consumer-runnable usage snippet and no references to paths or build targets
absent from the generated project.

**Acceptance Scenarios**:

1. **Given** a generated project's starter tests, **When** an author inspects
   them, **Then** they are domain-agnostic with no leftover demo names.
2. **Given** a generated project's skills, **When** an author reads them,
   **Then** they find consumer-facing usage guidance and no references to
   nonexistent framework-internal paths or build targets.

### User Story 5 - The effects boundary is documented canonically (Priority: P3)

An author wiring `update` to the host finds one canonical page that explains the
two effect categories — application commands at the MVU edge versus viewer
effects at the host boundary — and shows the `update`→host wiring.

**Why this priority**: The concept exists only scattered across design reports
(`docs/reports/*`), and one abbreviation involved is invisible to reflection, so
an author cannot recover the flow from the DLLs and must cross-reference source.

**Independent Test**: Locate a single documentation page that names both effect
categories, explains the boundary, and shows the canonical wiring; confirm an
author can follow it without reading scattered reports or source.

**Acceptance Scenarios**:

1. **Given** the documentation set, **When** an author searches for the effects
   boundary, **Then** a single canonical page covers both categories and the
   wiring.
2. **Given** that page, **When** an author follows the wiring example, **Then**
   it matches how a generated project wires effects.

### User Story 6 - Scene constructors are consistent and hard to misuse (Priority: P3)

An author constructs scene nodes using a consistent, self-describing API rather
than positional 4-tuples whose arity errors are confusing.

**Why this priority**: `Rectangle of (float*float*float*float)*Color`,
`Text of (float*float)*string*Color` (`src/Scene/Scene.fsi:322-332`) are
tuple-heavy and inconsistent with `PaintedRectangle of Rect*Paint` and with the
safer `rectangle` helper (`Scene.fsi:410`); an arity slip yields a confusing
"tuple of length 5" error. The author made this mistake twice.

**Independent Test**: Confirm a consistent, self-describing way to construct each
of `Rectangle`/`PaintedRectangle`/`Text` exists (e.g. `Rect`-based or
named-argument helpers) such that an arity slip is prevented or produces a clear
error, without removing the existing constructors.

**Acceptance Scenarios**:

1. **Given** the scene API, **When** an author constructs a rectangle and a text
   node, **Then** a consistent self-describing constructor/helper is available
   for both.
2. **Given** the existing positional constructors, **When** this feature ships,
   **Then** they still compile (additive change), so existing generated code is
   unaffected.

### Edge Cases

- A hint advertises an id that resolves in this repo but not in the skill set a
  generated project receives: the guard must cover generated-project skills, not
  only this repo's skills.
- The `.agents/` and `.claude/` copies of a skill drift in `name:` or advertised
  id: validation must treat them as synchronized peers.
- A public name is collision-prone but already qualified in some modules and not
  others: hardening must be applied consistently across the public surface, not
  case by case.
- The evidence-gate feature targeting (already fixed in 037) regresses: a guard
  should catch a return to placeholder-fallback or filename-mention triggering.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001** *(framework-repo dev-process item — Priority P3; must not block or
  delay any consumer-facing requirement)*: A repeatable check MUST fail when any
  skill id advertised in the task-generation hints or scan phrases does not
  resolve to a declared skill `name:`. The currently-dangling `speckit-debug-loop`
  reference MUST be removed or repointed to a real skill so the check passes.
- **FR-002**: The check MUST fail when a skill's directory name, declared
  `name:`, and advertised id are not mutually consistent — for skills in this
  repository and for skills generated into a consumer project.
- **FR-003**: The `.agents/` and `.claude/` copies of each skill MUST declare the
  same `name:` and advertise the same id, validated as synchronized peers.
- **FR-004**: A freshly generated project MUST contain, locally, the real public
  `.fsi` signature files for the packages it consumes — or an `api-surface.fsi`/
  `api-surface.md` generated verbatim from those signatures — as its
  authoritative API reference, so an author can determine any element's exact
  shape (e.g. a union case's field order) without reflecting DLLs. A derived
  human-readable summary is acceptable only if it is generated from the real
  signatures and kept in lockstep with them, never hand-maintained.
- **FR-005**: Generated guidance MUST NOT direct authors to API references,
  paths, or build targets that do not exist in a generated consumer project.
- **FR-006**: Generated skills MUST contain at least one consumer-facing,
  consumer-runnable usage snippet (scene construction, host wiring, or evidence
  production) rather than only scope/governance text.
- **FR-007**: Generated starter test scaffolding MUST be domain-agnostic, with no
  leftover demo-specific (e.g. game-title) identifiers such as the current
  "Tetris-style board" assertions.
- **FR-008**: Collision-prone public names MUST stop shadowing consumer code so
  that a consumer who `open`s a framework namespace can define their own
  `Normal`, `update`, and `init` — at minimum `ViewerWindowStartupState` and the
  viewer/input surfaces that expose `update`/`init`. The hardening (e.g.
  `[<RequireQualifiedAccess>]`) MAY be a breaking change to existing consumer
  source; when it is, a migration note MUST be documented, package versions MUST
  be bumped, and all generated samples MUST be updated so a freshly generated
  project compiles and ships with the clean, non-colliding surface.
- **FR-009**: The documentation set MUST include a single canonical page
  describing the two effect categories, the boundary between them, and the
  canonical `update`→host wiring, readable without consulting scattered reports
  or source.
- **FR-010**: The scene-construction API MUST offer a consistent, self-describing
  way to build `Rectangle`, `PaintedRectangle`, and `Text` nodes (reducing the
  tuple-arity footgun), added without removing the existing constructors so
  generated code keeps compiling.
- **FR-011** *(framework-repo dev-process item — Priority P3; must not block or
  delay any consumer-facing requirement)*: A regression guard MUST assert the
  evidence gates continue to target the feature in `.specify/feature.json` and do
  not trigger required evidence solely from filename mentions in `tasks.md`
  (behavior established by feature 037).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package *identities* are unchanged. Package *versions* will
  bump on merge per repository policy if any packable project changes (FR-008,
  FR-010 touch `src/` public surface). Package *contents* of generated projects
  change: a local API reference (FR-004) and consumer-facing skill snippets
  (FR-006) are added via the template. For controls/charts/graph/DataGrid
  authoring, the active widgets skill (`src/Controls/skill`, name
  `fs-skia-ui-widgets`) is the path; this feature does not change Charts package
  migration guidance.
- **Public contract impact**: `.fsi` signatures change. FR-008 removes name
  collisions (e.g. `[<RequireQualifiedAccess>]` on `ViewerWindowStartupState` and
  the viewer/input `update`/`init` surfaces) and **may be breaking** for existing
  consumers; when so, it requires a documented migration note, a version bump,
  and updated generated samples (per Clarifications). FR-010 adds scene
  constructors/helpers additively (no removals). FR-004 bundles the real `.fsi`
  signatures into generated projects as the local API reference. These require
  `.fsi` updates and surface-baseline review.
- **State workflow impact**: No stateful workflow, command, effect, subscription,
  or interpreter *behavior* changes. The effects boundary is documented (FR-009),
  not modified.
- **Layout/rendering impact**: None to rendering behavior. Layout, charts,
  DataGrid, screenshots, and Vulkan/Skia visual output are unchanged. Pointer
  input, a headless raster backend, and dotnet fsi window/font behavior are
  explicitly out of scope.
- **Evidence obligations**: Required real evidence: the skill-resolution guard
  output (FR-001/FR-002/FR-003); a generated project showing a usable local API
  reference (FR-004), consumer-facing skills (FR-006), and domain-agnostic starter
  tests (FR-007); a compile demonstrating a consumer's `Normal`/`update`/`init`
  no longer collide after `open` (FR-008); the canonical effects doc reachable
  (FR-009); a compile showing both new and existing scene constructors work
  (FR-010); and an evidence-gate run proving feature.json targeting (FR-011).
- **Unsupported scope**: Out of scope — mouse/pointer host input; a headless /
  software raster backend; dotnet fsi window/font usability. Also out of scope:
  removing or breaking any existing public constructor (FR-010 is additive only).
- **Build-target impact**: `GeneratedGuidanceCheck` and `TemplateCheck` change to
  assert id resolution (FR-001/FR-002), the generated API reference (FR-004),
  consumer-facing snippets (FR-006), and domain-agnostic tests (FR-007). `Dev`
  and surface-baseline checks change with the `.fsi` updates (FR-008/FR-010).
  `EvidenceGraph`/`EvidenceAudit` gain or keep a feature.json-targeting guard
  (FR-011). FAKE-backed targets are run sequentially per repository policy.

## Success Criteria *(mandatory)*

- **SC-001 (Primary, governing)**: A freshly generated consumer project is
  end-to-end feature-complete using only local references — it builds, runs its
  tests, and produces its evidence with zero DLL reflection and zero dependence
  on framework-repo-only paths, targets, or skills. Every other success criterion
  below is subordinate to this one; no framework-repo-process work may be
  considered done if it has degraded this outcome.
- **SC-002**: An author can determine any expected-public API element's exact
  shape using only the `.fsi` signatures present in a freshly generated project,
  with zero need to reflect DLLs.
- **SC-003**: A freshly generated project whose code `open`s the framework
  namespace and defines its own `Normal`, `update`, and `init` compiles with
  those names resolving to the project's definitions; where the hardening is
  breaking, a migration note and version bump are published and all generated
  samples are updated.
- **SC-004**: A freshly generated project contains zero leftover demo-specific
  identifiers in starter tests and zero generated references to paths/build
  targets absent from the project.
- **SC-005**: An author can locate and follow the effects boundary from a single
  documentation page without consulting scattered reports or source.
- **SC-006**: Existing generated code that uses the positional scene constructors
  still compiles after FR-010 (no breaking removals), and a consistent
  self-describing constructor/helper exists for `Rectangle`/`PaintedRectangle`/
  `Text`.
- **SC-007**: 100% of skill ids advertised by hints and scan phrases resolve to a
  declared `name:`; the resolution guard fails on an introduced dangling/drifted
  id and passes on the corrected repository.
- **SC-008**: The evidence gates demonstrably target the feature in
  `.specify/feature.json` and do not fire on an incidental filename mention.

## Assumptions

- The repository contains the framework source (`src/*.fsi`), the template
  (`template/`), the skills, and the evidence gates, so all listed requirements
  are actionable here (this corrects an earlier assumption that framework source
  was upstream-only).
- "Local authoritative API reference" (FR-004) is satisfied by bundling the real
  public `.fsi` signatures into generated projects (or an `api-surface.fsi`/`.md`
  generated verbatim from them); a hand-maintained summary is not acceptable. The
  generation mechanism is a planning decision; the outcome is exact, local,
  reflection-free signatures.
- FR-008 may be a breaking change: the listed names stop colliding (e.g. via
  `[<RequireQualifiedAccess>]`), and when existing consumer source would break, a
  migration note is documented, package versions are bumped, and all generated
  samples are updated so a freshly generated project compiles cleanly. The
  governing concern is the freshly generated project's completeness, not
  shielding already-published consumers from a one-time migration.
- Per the priority principle, framework-repo-process requirements (FR-001,
  FR-011) are P3 and sequenced after all consumer-facing work; they must never
  block or delay it, and SC-001 governs all other criteria.
- FR-010 is additive: new constructors/helpers are added and existing positional
  constructors are retained, so no generated code breaks and no migration is
  forced. A full breaking redesign of the scene DU is out of scope.
- The author-reported `tddemo1-widgets` name mismatch and `fs-skia-layout`
  dangling hint were generated-project artifacts not reproducible in current
  source; FR-001/FR-002 address them as a guard against the class rather than as
  one-off renames.
- The evidence-gate feature-targeting fix already shipped in feature 037; FR-011
  is a regression guard, not new behavior.
- `GeneratedGuidanceCheck`, `TemplateCheck`, `Dev`, `EvidenceGraph`, and
  `EvidenceAudit` are the FAKE-backed targets that will assert these outcomes,
  run sequentially per repository policy.

## Key Entities

- **Skill**: A unit with a declared `name:`, scan phrases, and guidance, present
  under `src/<Module>/skill/`, `.agents/skills/`, `.claude/skills/`, or bundled in
  `template/`. Its identity has three facets that must agree: directory name,
  declared `name:`, and every advertised id.
- **Skill-assignment hints**: The task-generation guidance
  (`speckit-tasks/SKILL.md`) that advertises skill ids; every id must resolve.
- **Generated API reference**: The local, human-readable description of the
  public surface shipped into a generated project.
- **Public surface (`.fsi`)**: The framework signatures under `src/`; the locus
  of the name-collision hardening (FR-008) and the additive scene constructors
  (FR-010).
- **Effects-boundary documentation**: The canonical page describing the two
  effect categories and the `update`→host wiring.
- **Evidence gate**: The check (`build.fsx` `EvidenceGraph`/`EvidenceAudit`) that
  evaluates a feature's real evidence, targeting `.specify/feature.json`.
