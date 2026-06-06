# Feature Specification: Design Tokens + Penpot (DTCG → Generated F# + DesignTokenDrift)

**Feature Branch**: `069-design-tokens-penpot`
**Created**: 2026-06-06
**Status**: Draft
**Input**: User description: "implement the next part of docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md and update the plan with progress" — roadmap feature **069 — Design tokens + Penpot tokens-first (DTCG JSON → generated F#, `DesignTokenDrift`)** (§13).

## Overview

Features `065` (typed controls front door), `066` (typed catalog generation), `067`
(internal keyed reconciliation), and `068` (`Controls.Elmish` command model) have all
landed on `main`. The roadmap's stated sequencing is "type the authoring layer first,
**wire tokens second**, migrate breadth last" (plan §13). This feature is the
tokens-second step.

Today every visual primitive a control renders with — colors, font family/size,
density, corner radius, the contrast ratio it must satisfy — lives as **hand-authored
literals** inside two `Theme` values: `Theme.light` and `Theme.dark`
(`src/Controls/Theme.fs`), each a record of the `Theme` type
(`src/Controls/Types.fs`). There is **no design-token layer**: a designer-facing source
of truth, a way to keep design and code in lock-step, or an interchange format a design
tool (Penpot) could read or write. Confirmed by source: the repository contains **no**
design-token, DTCG, or Penpot code or configuration of any kind.

This feature introduces a **single-source design-token pipeline**, applying the exact
generation pattern feature `066` established for the catalog (one fact source → multiple
generated artifacts → a drift gate that fails hand-edits) — now to theme primitives:

1. A canonical **DTCG-format** ([Design Tokens Community Group](https://www.designtokens.org/)
   JSON) token document, checked into the repo, becomes the **single source of truth** for
   the theme primitives. DTCG is the format Penpot (and other design tools) export and
   import, so this establishes the Penpot-interoperable interchange contract.
2. A **generated** F# design-token module ships in `FS.Skia.UI.Controls`, exposing the
   token values as a typed surface, and `Theme.light`/`Theme.dark` are **re-expressed in
   terms of those generated tokens** instead of inline literals — with values
   **byte-identical** to today's, so rendering behavior is unchanged.
3. A new **`DesignTokenDrift`** currency gate (mirroring `ControlsCatalogGenerationCheck`)
   fails the build if the generated F# is not a byte-identical regeneration of the DTCG
   source, and `RegenerateDesignTokens` is wired into `RefreshSurfaceBaselines` so the
   single edit point is the DTCG document.
4. A new **`fs-skia-design-tokens`** capability skill is authored in this same branch
   (per plan §16.4: each new skill "should land in the same feature branch that first
   needs it"), teaching the DTCG → generated-F# flow and the drift gate.

It is **additive and behavior-preserving**: the `Theme` type, `Theme.light`/`dark`
observable values, `Control.render`, the renderer, layout, and accessibility are all
unchanged in behavior. The change adds a generated public token surface and moves the
*authorship* of theme primitives from F# literals to the DTCG document. Because it edits
public `.fsi` under `src/Controls/**`, it is a consumer-contract change.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Theme primitives sourced from one DTCG document (Priority: P1)

A maintainer needs to change a theme primitive — say the accent color or the base font
size. Instead of editing F# literals in `Theme.fs` (and risking design/code drift), they
edit the single DTCG token document and regenerate; the generated F# token module and the
token-derived `Theme` values update from that one edit, and a gate guarantees the F# can
never silently diverge from the DTCG source.

**Why this priority**: This is the defining value of the feature — establishing the DTCG
document as the single source of truth for theme primitives, with generation keeping F# in
lock-step. If only this story ships, the feature delivers its core capability.

**Independent Test**: Change a token value in the DTCG document, run
`RefreshSurfaceBaselines`, and assert (a) the generated F# token module reflects the new
value and (b) the corresponding `Theme.light`/`dark` field now resolves to it — with no
hand-edit to `Theme.fs` or the generated module.

**Acceptance Scenarios**:

1. **Given** the DTCG token document and the generated F# token module are in sync,
   **When** `DesignTokenDrift` runs, **Then** it passes and reports the generated module
   is a current, byte-identical regeneration of the DTCG source.
2. **Given** a token value is edited in the DTCG document but the generated F# is **not**
   regenerated, **When** `DesignTokenDrift` runs, **Then** it **fails**, naming the stale
   token(s), the generated file, and the regenerate command (`./fake.sh build -t
   RefreshSurfaceBaselines`).
3. **Given** a token value is edited in the DTCG document, **When**
   `RefreshSurfaceBaselines` runs, **Then** the generated F# token module is rewritten from
   the DTCG source and `DesignTokenDrift` passes again, with no manual edit to the generated
   file.

### User Story 2 - Rendering behavior is unchanged (Priority: P1)

A product author who consumes `FS.Skia.UI.Controls` recompiles against the new package and
observes **no** visual or behavioral change: `Theme.light` and `Theme.dark` produce
exactly the same `Color`/size/density values they did before, every control renders
identically, and no existing code needs editing.

**Why this priority**: Theme is a shipped public contract that drives all rendering;
introducing the token layer must not change a single rendered pixel or break a consumer.
Behavior-preservation is the compatibility guarantee that makes single-sourcing safe.

**Independent Test**: For every field of `Theme.light` and `Theme.dark`, assert the
token-derived value equals the literal value the pre-feature theme produced (a frozen
expected-values table), and re-render the controls gallery to confirm node/visual output is
unchanged.

**Acceptance Scenarios**:

1. **Given** the token-derived `Theme.light` and `Theme.dark`, **When** each field
   (`Foreground`, `Background`, `Accent`, `Danger`, `Muted`, `FontFamily`, `FontSize`,
   `Density`, `CornerRadius`, `ContrastRequiredRatio`) is compared to the pre-feature
   literal value, **Then** every field is byte/value-identical.
2. **Given** an existing program that renders controls with `Theme.light`/`dark`, **When**
   compiled and run against the new package, **Then** it builds with no source edit and
   produces identical render output.

### User Story 3 - Typed token surface for direct authoring (Priority: P2)

A product or control author wants to reference a design token directly (e.g. the accent
color, the base spacing unit) when composing a view or a custom theme variant, rather than
copying a literal. The generated token module exposes the tokens as a **typed, compiler-
checked** surface they can reference, so token references are greppable and stay in sync
with the DTCG source.

**Why this priority**: Direct token authoring is the "tokens-first authoring flow" the
roadmap names, and it is what the new `fs-skia-design-tokens` skill teaches. Lower than
US1/US2 because the single-source pipeline and behavior-preservation are the load-bearing
guarantees; the typed surface is the consumer affordance built on top.

**Independent Test**: Author a small theme variant or view that references a generated token
value by name; assert it compiles, resolves to the expected value, and contains no inline
color/size literal that duplicates a token.

**Acceptance Scenarios**:

1. **Given** the generated token module, **When** a consumer references a token value by its
   typed name, **Then** it compiles and resolves to the same value the DTCG source declares.
2. **Given** the generated token surface, **When** `PackageSurfaceCheck` runs, **Then** the
   delta is **additive-only** (new token names; no removed or changed signature).

### Edge Cases

- **DTCG references/aliases**: DTCG allows a token to reference another (`"{color.base.blue}"`).
  Generation MUST resolve aliases deterministically to concrete values; an unresolvable or
  cyclic reference is a generation failure surfaced by the gate, never a partially emitted
  module.
- **Malformed or incomplete DTCG source**: if the DTCG document is invalid, or is missing a
  token the `Theme` mapping requires, generation MUST fail loudly (naming the missing/invalid
  token) and emit **no** F#, rather than producing a half-generated module.
- **Light vs. dark**: the DTCG document MUST express both the `light` and `dark` theme value
  sets so both `Theme.light` and `Theme.dark` are fully token-derived; neither retains inline
  literals for the migrated primitives.
- **Color encoding**: DTCG color values (hex/`#rrggbb[aa]`) MUST map deterministically to the
  repo's `Color` RGBA-byte representation (`src/Scene/Scene.fsi`) such that the generated
  value is byte-identical to the pre-feature literal.
- **Hand-edit of generated F#**: editing the generated token module directly MUST be caught
  by `DesignTokenDrift` (the generated file is not the source of truth), exactly as
  `ControlsCatalogGenerationCheck` catches hand-edits of `Catalog.fs`/`catalog.yml`.
- **Determinism**: generation MUST be pure — no wall-clock, randomness, or environment
  dependence; the same DTCG document always produces byte-identical F#.

> Interacting / conflicting requirements: "the DTCG document is the single source of truth
> for theme primitives" vs. "`Theme.light`/`dark` observable values are byte-unchanged".
> Resolution: in **this** feature the DTCG document is authored to **reproduce today's exact
> values**, so adopting single-sourcing is behavior-preserving — no value changes ship here.
> Any future primitive change is made by editing the DTCG document and regenerating, never by
> editing the F#. This keeps the migration purely structural and the value-change capability
> (US1) demonstrable without altering shipped appearance.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: A canonical **DTCG-format JSON** token document MUST be checked into the
  repository and serve as the **single source of truth** for the theme primitives currently
  hand-authored in `Theme.light`/`Theme.dark` (colors `Foreground`/`Background`/`Accent`/
  `Danger`/`Muted`, `FontFamily`, `FontSize`, `Density`, `CornerRadius`,
  `ContrastRequiredRatio`), for both the light and dark value sets.
- **FR-002**: A **generated** F# design-token module MUST ship in the
  `FS.Skia.UI.Controls` package, produced from the DTCG document, exposing the token values
  as a typed surface declared in a curated `.fsi`.
- **FR-003**: `Theme.light` and `Theme.dark` MUST be **re-expressed in terms of the
  generated tokens** (no inline literals for the migrated primitives), and every observable
  field value MUST be **byte/value-identical** to the pre-feature theme (behavior-preserving).
- **FR-004**: A new **`DesignTokenDrift`** gate MUST fail the build when the generated F#
  token module is not a byte-identical regeneration of the DTCG source, emitting actionable
  diagnostics that name the stale/missing/invalid token(s), the generated file, and the
  regenerate command. It MUST pass when generated F# and DTCG source are in sync.
- **FR-005**: `RegenerateDesignTokens` MUST be wired into `RefreshSurfaceBaselines` so the
  generated F# token module is rewritten from the DTCG document by the same single regenerate
  command used for the catalog, making the DTCG document the **one** edit point for token-value
  changes (adding or removing a token additionally edits the curated `DesignTokens.fsi`; no value
  change ships here, so this feature touches the DTCG document only).
- **FR-006**: Token generation MUST be **pure and deterministic** — no I/O beyond reading the
  DTCG source at the interpreter edge, no wall-clock, no randomness; identical DTCG input
  yields byte-identical F# output. DTCG alias/reference resolution MUST be deterministic, and
  cyclic/unresolvable references MUST fail generation rather than emit partial output.
- **FR-007**: The feature MUST add **no new package dependency** to `FS.Skia.UI.Controls`
  (in particular not `Fable.Elmish`); the DTCG parser/generator lives in the build/governance
  assembly (`FS.Skia.UI.Build`), as `CatalogGen` does, not in the shipped package.
- **FR-008**: The public-surface change MUST be **additive-only** — the `Theme` type and its
  module signatures are unchanged; the only public delta is the **new** generated token
  module's surface. The affected per-package surface baseline(s) are regenerated and reviewed
  in the diff.
- **FR-009**: A new routing rule (or extension of `controls-public-surface`) MUST route
  changes to the DTCG document and the generated token module through `DesignTokenDrift` plus
  the existing public-surface gates, and `validation.contract.yml` MUST stay generated from
  `Routing.fs` (no hand-sync), as governed today.
- **FR-010**: A new **`fs-skia-design-tokens`** capability skill MUST be authored in this
  branch (canonical `.agents/skills/`, `.claude` peer regenerated via
  `RefreshSurfaceBaselines`), documenting the DTCG → generated-F# flow, the `DesignTokenDrift`
  gate, and the tokens-first authoring flow; it MUST pass `SkillSyncCheck`/`SkillQualityCheck`.
- **FR-011**: Generation and the token mapping MUST carry **no `[S]` synthetic disclosure** —
  the lowering from DTCG to concrete `Color`/size values is **real** and value-parity-tested
  (US2). If any token is mapped with a placeholder, it MUST carry the `[S]` disclosure per
  Constitution Principle V.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: The active package is `FS.Skia.UI.Controls` (`src/Controls/**`), which
  gains an **additive** generated token module; `Theme` internals move from literals to
  token references with no value change. No package identity moves; no legacy Charts package
  migration is involved. The DTCG parser/generator lives in `FS.Skia.UI.Build`
  (`build/Governance/**`), not in any shipped package. Version bump/pack and template pin are
  post-merge concerns owned by the `speckit-merge` and `fs-skia-template-update` skills.
- **Public contract impact**: **Yes, additive.** A new generated token module `.fsi` is added
  under `src/Controls/`; the `Theme` type and `Theme` module signatures are unchanged. The
  affected per-package surface baseline (`FS.Skia.UI.Controls`) is regenerated; the delta is
  additive-only. Samples may be extended to demonstrate token-first authoring.
- **State workflow impact**: **None.** No stateful workflow, I/O, commands, effects,
  subscriptions, or interpreter behavior changes. Token generation is a pure build-time
  transform reading a static DTCG document.
- **Layout/rendering impact**: **None observable.** Colors/sizes/density/corner-radius/contrast
  feeding the renderer are byte-identical to today; `Control.render`, layout, charts, DataGrid,
  screenshots, Vulkan, Skia, and unsupported-environment diagnostics are unchanged. A render
  parity check confirms identical gallery output.
- **Evidence obligations**: Required real evidence paths under
  `specs/069-design-tokens-penpot/readiness/`:
  - `readiness/design-tokens.md` — the DTCG source design, the token taxonomy, the DTCG → F#
    mapping, and the tokens-first authoring flow.
  - `readiness/design-token-drift.md` — the `DesignTokenDrift` gate report (currency PASS,
    hand-edit detection).
  - `readiness/theme-token-parity.md` — the per-field `Theme.light`/`dark` value-parity table
    (token-derived ≡ pre-feature literal) and render-parity result.
  - `readiness/package-surface-expectations.md` — the additive `FS.Skia.UI.Controls` surface
    delta and regenerated-baseline rationale.
- **Unsupported scope**: No **live Penpot integration** (Penpot MCP inspect/draft/provenance,
  or any network/tool round-trip) — DTCG is established here only as the interchange format;
  live Penpot sync is the later roadmap item (§13 "Later — Penpot MCP assist"). No migration of
  the remaining 41 controls (`070`), no catalog expansion (`071+`), no motion/animation tokens,
  no new color-science/contrast computation, and no runtime theme-switching UI.
- **Build-target impact**: A **new** `DesignTokenDrift` target is added and `RefreshSurfaceBaselines`
  gains a `RegenerateDesignTokens` step; `validation.contract.yml` regenerates from `Routing.fs`.
  No semantic change to `Dev`, `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, or `EvidenceAudit`. Because the
  change edits public `src/Controls/**` `.fsi` and adds governance rules, `Route` escalates to
  the `controls-public-surface` gate set plus `DesignTokenDrift` (and the governance/skill gates
  for the routing-rule and skill edits); run **only** the gates `Route` prints, FAKE-backed
  gates sequentially.

## Key Entities

- **DTCG token document** (new): the checked-in DTCG-format JSON; the single source of truth
  for theme primitives and the Penpot-interoperable interchange artifact.
- **Generated design-token module** (new): typed F# token values generated from the DTCG
  document, shipped in `FS.Skia.UI.Controls`; a generated artifact, not hand-authored.
- **`Theme`** (existing, `src/Controls/Types.fs`): the record consumed by `Control.render`.
  Its `light`/`dark` values become token-derived; the type and observable values are unchanged.
- **`DesignTokenGen`** (new, `FS.Skia.UI.Build`): the build-assembly module that parses the
  DTCG document and renders/checks the generated F#, mirroring `CatalogGen` (`render`/`splice`/
  `currency`/`currencyDrift`).
- **`DesignTokenDrift`** (new gate): the currency gate that fails hand-edits / stale generation,
  mirroring `ControlsCatalogGenerationCheck`.
- **`fs-skia-design-tokens`** (new skill): the capability skill teaching the DTCG → F# flow.

## Success Criteria *(mandatory)*

- **SC-001**: Every theme primitive listed in FR-001 originates from the DTCG document — zero
  hand-authored color/size/density/radius/contrast literals remain in `Theme.fs` for the
  migrated fields (verified by inspection + the parity test).
- **SC-002**: For all 10 fields × 2 themes (`light`, `dark`), the token-derived value equals
  the pre-feature literal value — **100%** parity, no field divergent (US2 parity table).
- **SC-003**: Rendering the controls gallery against the token-derived themes produces output
  identical to the pre-feature themes (render parity; no visual/node diff).
- **SC-004**: Editing a DTCG token value and running `RefreshSurfaceBaselines` updates the
  generated F# and the resolved `Theme` field from that **single** edit, with no manual edit to
  the generated module (US1 demonstrated).
- **SC-005**: `DesignTokenDrift` **fails** when the generated F# is hand-edited or left stale
  relative to the DTCG source, naming the offending token(s) and the regenerate command, and
  **passes** when they are in sync.
- **SC-006**: Token generation is deterministic — regenerating from the same DTCG document
  twice yields byte-identical F# (no wall-clock/random dependence).
- **SC-007**: `FS.Skia.UI.Controls` declares **no** new package dependency; the DTCG
  parser/generator resides only in `FS.Skia.UI.Build`.
- **SC-008**: The regenerated surface baseline delta is confined to `FS.Skia.UI.Controls` and
  is **additive-only** (no removed or changed signatures), verified by `PackageSurfaceCheck` /
  `PerPackageSurfaceDiff`.
- **SC-009**: `validation.contract.yml` is a current regeneration of `Routing.fs` (the new
  rule is single-sourced, not hand-synced) and the `.claude` skill tree is a current
  regeneration of `.agents` (`fs-skia-design-tokens` passes `SkillSyncCheck`).
- **SC-010**: `./fake.sh build -t Route` over the branch diff prints the escalated gate set
  (including `DesignTokenDrift`) and **every printed gate passes**; `EvidenceAudit` verdict is
  **PASS** with no `[S]`/`[S*]` disclosures.

## Assumptions

- The theme primitives to tokenize are exactly the 10 `Theme` fields
  (`src/Controls/Types.fs`): `Foreground`, `Background`, `Accent`, `Danger`, `Muted`,
  `FontFamily`, `FontSize`, `Density`, `CornerRadius`, `ContrastRequiredRatio` — sourced from
  the current `Theme.light`/`Theme.dark` literals (`src/Controls/Theme.fs`). The `Name` field
  (`"light"`/`"dark"`) labels the theme variant and stays a code-level constant.
- DTCG is adopted as the interchange format because it is the format Penpot exports/imports;
  this feature establishes the format and the generated-F# contract so a **later** feature can
  wire live Penpot sync. No live Penpot/MCP integration ships here (§13 "Later").
- The single-source generation, drift-gate, `RefreshSurfaceBaselines` wiring, and routing/
  contract-regeneration mechanics follow the **exact** pattern feature `066` shipped for the
  catalog (`build/Governance/CatalogGen.fs`, `ControlsCatalogGenerationCheck`,
  `RegenerateCatalog`), so this feature reuses a proven, load-bearing template.
- The DTCG parser/generator is compiled F# in `FS.Skia.UI.Build`, tested with the repo's
  Expecto + FsCheck harness (per `fsharp-build-orchestration` / `fsharp-parsing` /
  `fsharp-code-generation`); no new test framework or runtime dependency is introduced.
- This feature performs the roadmap's "wire tokens second" step; the legacy ability to
  construct ad-hoc `Theme` values in code is retained (the type is unchanged) — tokens drive
  the **shipped** `light`/`dark` themes and are the preferred authoring source, mirroring the
  `065` decision to keep legacy authoring as a peer.
- The `fs-skia-design-tokens` skill lands in this branch (plan §16.4) so its guidance is
  validated against the real generation work rather than written speculatively.

## Out of Scope

- Live Penpot integration of any kind (Penpot MCP inspect/draft/provenance, network sync,
  code↔design round-trip) — deferred to the later "Penpot MCP assist" roadmap item.
- Migrating the remaining 41 controls to typed Props/MVU (`070`) and catalog expansion (`071+`).
- Motion/animation tokens, runtime theme-switching UI, new color-science or contrast-ratio
  computation, and any change to `Theme`'s field set or `Control.render` semantics.
- Changing any shipped theme **value** (the migration is value-preserving; value changes are a
  later, separate edit to the DTCG document).
- Any change to the base `FS.Skia.UI.Controls` dependency set or to other packages' surfaces.
