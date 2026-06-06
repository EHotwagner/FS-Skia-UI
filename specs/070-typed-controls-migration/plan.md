# Implementation Plan: Migrate Remaining 41 Controls to the Typed Props/MVU Front Door

**Branch**: `070-typed-controls-migration` | **Date**: 2026-06-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/070-typed-controls-migration/spec.md`

## Summary

Complete the typed front door started in `065`: every catalog control not in the
six-control reference slice — the **41 remaining** of 47 — gains an additive,
compiler-checked authoring surface under `FS.Skia.UI.Controls.Typed.*` (an
immutable `Props` record + `defaults` + a `view` returning `Widget<'msg>`, plus
typed `init`/`update` for stateful controls). Each typed `view` lowers to a
`Control<'msg>` **structurally equal** to the legacy `*.create`/`Attr` or
`Control.standard` builder it faces, proven by a per-control lowering-parity test
(the keystone `065` discipline). Stateful controls (`text-area`, the selection
collections, charts/graph) **delegate to the existing MVU models** (`TextInput`,
`Collections`, `DataGrid`, chart/graph) rather than forking them; `custom-control`
is "typed" via the existing `Widget.ofControl` bridge, not a fabricated schema.
The legacy string-keyed API stays **byte-frozen**, so the public-surface delta is
additive-only. No new dependency (in particular no `Fable.Elmish`) is added. The
`066` catalog single source (`CatalogGen.catalogFacts`) is **regenerated** from 6
to all 47 rows so `ControlsCatalogGenerationCheck` stays green. The new
**`fs-skia-typed-controls`** capability skill lands in this branch (plan §16.4) —
`070` is "run that skill 41 times," so the skill is validated against the real
migration. The change is confined to `src/Controls/**` (escalates to
`controls-public-surface`) plus the skill paths (skill gate set).

## Technical Context

**Language/Version**: F# / .NET `net10.0` (matches `Controls.fsproj`)
**Primary Dependencies**: existing only — `Scene`, `Layout`, `KeyboardInput`. **No new dependency** (FR-008); explicitly not `Fable.Elmish` (SC-006).
**Testing**: Expecto (`tests/Controls.Tests/`, `tests/Elmish.Tests/`), FSI transcripts, the escalated FAKE six-target order, and the `Route`-printed `controls-public-surface` + skill gates.
**Target Platform**: Windows and Linux (library; no platform narrowing).
**Change Tier**: Tier 1 (contracted change — adds public `.fsi` surface for 41 controls; additive-only).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial evaluation: PASS.** The feature is additive-only, reuses the existing
IR and the existing stateful models, adds no dependency, and introduces no new
stateful workflow or I/O. Stateful typed controls satisfy Principle IV by
delegating to the existing pure `update` functions. Lowering is real and
parity-tested for every control, so Principle V (synthetic disclosure) is not
engaged (FR-011 — intent is zero `[S]`). It re-uses the settled `065` decisions
(Q1–Q5) and the `066` single-source generation pattern without reopening them.

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` change. The
  feature adds source under `src/Controls/**`, tests under `tests/**`, a repo
  sample panel in `samples/ControlsGallery/`, and one new **governance** skill
  under `.agents/skills/fs-skia-typed-controls/` (with its regenerated `.claude`
  peer). None of these is a generated-project template fragment, package-policy,
  or command-surface change the template must mirror. Version bump/pack and
  template-pin refresh are explicitly **post-merge**, owned by `speckit-merge`
  and `fs-skia-template-update` (out of scope here).
- **Dependency impact**: N/A — no dependency added (FR-008, SC-006).
  `Directory.Packages.props`, `docs/dependencies.md`, generated template
  inclusion, and `DependencyReport` are unchanged. The existing dependency-
  governance guard (`tests/Elmish.Tests/`) asserting `Controls.fsproj` references
  no `Fable.Elmish` is kept green and re-run.
- **Command-surface impact**: No `build.fsx`/`Routing.fs`/wrapper change for the
  controls migration — `src/Controls/**` already matches the
  `controls-public-surface` rule, so no routing edit is required (the spec's
  Build-target impact confirms this). The new skill edit routes through the skill
  gate set (`SkillSyncCheck`, `SkillQualityCheck`, `SkillContractPathCheck`,
  `TemplateUpdateSkillPackageCheck`; FR-013 / SC-008) and `RefreshSurfaceBaselines`
  regenerates the `.claude` peer and the catalog. `validation.contract.yml` stays generated from
  `Routing.fs`. Run `./fake.sh build -t Route` **first** and run only the gates it
  prints; FAKE-backed targets run **sequentially** (shared `.fake` state — never
  concurrently) in the escalated deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A — no change to default/minimal generated
  contents, validation logs, placeholder/excluded-history scans, or generated
  `Dev` behavior. `GeneratedProductCheck` is a printed gate and is expected to
  pass unchanged: the typed surface is additive and the generated product does
  not consume the 41 new modules in this feature. The new skill becomes available
  to maintainers/products but ships no new selected-Controls guidance to the
  default generated project.
- **Evidence paths**: All under `specs/070-typed-controls-migration/readiness/`.
  Routing-required (`Route --enforce`): `typed-controls-migration.md`,
  `package-surface-expectations.md`. Supporting per the spec's Evidence
  obligations: `typed-lowering-parity.md` (the 41-control parity matrix — the
  keystone proof), `controls-rendering.md` (typed gallery-panel viewport render
  evidence). FSI transcripts land under the same `readiness/` tree per the
  `FsiTranscripts` gate; skill-loading and selected-skills evidence under
  `readiness/` per the Local Agent Skills gate.
- **`.fsi` / contract impact**: Yes — Tier 1, additive. New public `.fsi` under
  `src/Controls/Widgets/` declares the 41 typed modules in the
  `FS.Skia.UI.Controls.Typed` namespace, grouped by mechanic (Display, Input,
  Stateful, Containers, Navigation, Overlay, Collections, Charts/Graph). **No**
  existing `.fsi` signature changes (FR-007 — legacy frozen). The
  `FS.Skia.UI.Controls` package per-package surface baseline is regenerated and
  reviewed in the diff (FR-010); `PackageSurfaceCheck`/`PerPackageSurfaceDiff`
  gate that the delta is additive-only (SC-004). Compatibility: legacy API is the
  permanent peer; `custom-control`'s typed affordance is the existing
  `Widget.ofControl` bridge (FR-006), not a new signature.
- **MVU/effect boundary**: No **new** MVU model. Stateful typed façades reuse the
  existing models and delegate to their pure `update`: `text-area` →
  `TextInput`; the selection collections (`list-view`, `list-box`,
  `multi-select-list`, `combo-box`, `tree-view`) → `Collections`
  (`CollectionModel`/`CollectionMsg`/`CollectionEffect`); charts/graph → the
  existing chart/graph models; (`data-grid` already typed in `065` via
  `DataGrid`). Each stateful typed `init`/`update` returns the existing model and
  effect types (FR-004, SC-003) with no I/O in `update`; effect/model equality vs.
  the reused model is asserted per control. Edge interpreters are reused
  unchanged. Pure-display/input/container controls are pure `Props -> Widget`
  (no model).
- **Synthetic evidence**: None intended (FR-011). Every lowering is real and
  verified by structural parity against the legacy builder. `typed-controls-
  migration.md` states this explicitly. If any single control cannot achieve real
  parity in this feature, that control alone carries the `[S]` disclosure per
  Principle V, is named in the evidence, and the rest proceed (per the spec's
  per-mechanic grouping resolution) — the target remains zero `[S]`/`[S*]` and a
  PASS `EvidenceAudit` (SC-010).
- **Test evidence**: Failing-first contract tests assert the 41 typed modules
  exist before production code (extend `TypedControlContractTests.fs`). The
  keystone is the per-control structural lowering-parity matrix (extend
  `TypedLoweringTests.fs`): typed `view |> Widget.toControl` ≡ normalized legacy
  builder output for all 41 (SC-002). Interaction tests cover optional-event →
  no-binding (FR-005) and stateful `update` delegation equality (SC-003).
  Accessibility + rendering tests cover a representative typed gallery panel at
  ≥2 viewports. The `066` catalog cross-check tests (`CatalogTests.fs`,
  `typedPropsById`) are extended to all 47 and kept green (SC-007). The `no obj /
  no string-keyed event` grep guard over the new `.fsi` enforces FR-003/SC-005.
- **Observability**: No new diagnostics path. Typed views lower to the same IR,
  so existing `Control.diagnostics`/`ControlDiagnostic` reporting, accessibility
  metadata, and unsupported-environment messages are reused byte-for-byte. No new
  actionable-failure or unsupported-environment message is introduced.
- **Deferred scope**: Out of scope and sequenced later — catalog **expansion**
  (new buttons/pickers/date-time), overlays/virtualization, motion/animation
  (`071+`); live Penpot/MCP integration; design-token **value** changes (`069`
  shipped the layer); legacy-API deprecation (a later, separate decision); any
  change to keyed-reconciliation internals (`067`) or the `Controls.Elmish`
  adapter signature / command model (`068`). Version bump/pack and template-pin
  refresh are post-merge (`speckit-merge` / `fs-skia-template-update`).

## Project Structure

New compile units land in `src/Controls/Widgets/` (so they ship in
`FS.Skia.UI.Controls`, no project moves). `<Compile>` order in `Controls.fsproj`
is significant — every typed module is inserted **after** `Widget.fs` and after
the legacy module(s) it lowers to / the model(s) it reuses. The 41 controls are
grouped by mechanic into a small number of new files (per the spec's "grouped by
mechanic into the existing `Widgets/*` file structure" resolution), so the diff is
reviewable per group:

```
src/Controls/
  Widget.fsi / Widget.fs                  (existing — the lowering seam, unchanged)
  Widgets/Primitives.fsi / .fs            (existing — TextBlock, Button, CheckBox, Stack)
  Widgets/TextBoxWidget.fsi / .fs         (existing — typed TextBox over TextInput)
  Widgets/DataGridWidget.fsi / .fs        (existing — typed DataGrid)
  Widgets/Display.fsi / .fs               <- NEW: rich-text, label, image, icon, separator,
                                                  badge, progress-bar, spinner, validation-message
  Widgets/Input.fsi / .fs                 <- NEW: icon-button, numeric-input, radio-group,
                                                  switch, slider
  Widgets/TextAreaWidget.fsi / .fs        <- NEW: text-area (reuses TextInput model)
  Widgets/Containers.fsi / .fs            <- NEW: grid, dock, wrap, border, panel,
                                                  scroll-viewer, split-view
  Widgets/Navigation.fsi / .fs            <- NEW: tabs, menu, context-menu, toolbar
  Widgets/Overlay.fsi / .fs               <- NEW: tooltip, dialog, toast, overlay
  Widgets/CollectionsWidgets.fsi / .fs    <- NEW: list-view, list-box, multi-select-list,
                                                  combo-box, tree-view (reuse Collections model)
  Widgets/ChartsWidgets.fsi / .fs         <- NEW: line-chart, bar-chart, pie-chart,
                                                  scatter-plot, graph-view (reuse chart/graph models)
  Widgets/CustomControlWidget.fsi / .fs   <- NEW: custom-control via Widget.ofControl (no Props schema)

build/Governance/
  CatalogGen.fs                           (regenerate catalogFacts 6 -> 47; no hand-edit)
src/Controls/
  catalog.yml, Catalog.fs                 (REGENERATED via RefreshSurfaceBaselines, never hand-edited)

tests/Controls.Tests/
  TypedControlContractTests.fs            (extend: assert 41 typed modules exist; no `obj` in new .fsi)
  TypedLoweringTests.fs                   (extend: 41-control structural parity matrix — keystone)
  InteractionTests.fs                     (extend: optional-event -> no binding; stateful update delegation)
  RenderingTests.fs, AccessibilityTests.fs (extend: representative typed panel at >=2 viewports)
  CatalogTests.fs                         (extend typedPropsById 6 -> 47; required-attr -> Props field)
tests/Elmish.Tests/                       (keep dependency guard green; Widget.toControl through adapter)

samples/ControlsGallery/Program.fs        (extend: representative typed-authoring panel — render smoke; FR-014)

.agents/skills/fs-skia-typed-controls/SKILL.md   <- NEW canonical capability skill
.claude/skills/fs-skia-typed-controls/...         <- REGENERATED peer (RefreshSurfaceBaselines)

specs/070-typed-controls-migration/readiness/
  typed-controls-migration.md             (routing-required)
  package-surface-expectations.md         (routing-required)
  typed-lowering-parity.md                (supporting — the 41-control parity matrix)
  controls-rendering.md                   (supporting — typed gallery viewport render)
```

**Per-id typed module decision** (resolved in research.md, R1): each of the 41
catalog **ids** gets its own typed module named by the PascalCase of its id
(`RichText`, `Label`, `IconButton`, `ListView`, `ComboBox`, `ContextMenu`,
`ScrollViewer`, `LineChart`, …), uniformly with the `065` invariant "catalog
`Module` fact names the typed module." Where several ids share a backing legacy
model (the `Collections` model backs the five selection collections; the chart
models back the chart ids), the per-id typed modules are **distinct** but
**delegate to the same shared model** and **lower to the legacy builder/kind** for
that id — so SC-001's "all 47 have a typed module" is satisfied per id without
forking any model.

## Phase 0: Research

See [research.md](./research.md). It resolves the genuinely new design questions
this feature introduces (per-id typed modules over shared legacy models; the
lowering-parity target where no per-control `*.create` exists; the catalog
single-source regeneration from 6 → 47; the `custom-control` honesty resolution;
skill-lands-in-branch sequencing). The five `065` decisions (Q1–Q5) are settled
and **not reopened** (the spec's Assumptions bake them in); no
`NEEDS CLARIFICATION` remains.

## Phase 1: Design & Contracts

- Data model: [data-model.md](./data-model.md) — the 41 control entities grouped
  by mechanic, each control's taxonomy field choices, the reused models, and the
  validation rules (no `obj`, required→non-optional, optional→`defaults`,
  optional-event→no-binding).
- Contracts: [contracts/](./contracts/) — representative `.fsi` surface sketches
  per mechanic group (Display, Input, Stateful/TextArea, Containers, Navigation,
  Overlay, Collections, Charts/Graph, CustomControl) and the lowering-parity
  contract that every control must satisfy.
- Quickstart: [quickstart.md](./quickstart.md) — author a migrated control through
  the typed surface, compose containers over `Widget` children, and finish at the
  Elmish adapter with `Widget.toControl`.

**Post-design Constitution re-check: PASS.** The contracts keep every `Props`
field strongly typed (no `obj`, no string-named events — FR-003/SC-005), expose
only the typed modules + records on the public surface (Principle II), route every
stateful `init`/`update` through the existing pure models (Principle IV), and pin
real lowering per control with a parity test (Principle V — no synthetic). The
additive-only namespace isolation (`FS.Skia.UI.Controls.Typed`) keeps the legacy
peer frozen (FR-007/SC-009). No new violation surfaced.

## Phase 2 (planning complete)

Tasks are deferred to `/speckit-tasks`. The expected dependency-ordered shape:
author the `fs-skia-typed-controls` skill first (it gates the migration work) →
failing-first contract tests for the 41 modules → per-mechanic group
implementation (Display → Input → Stateful/TextArea → Containers → Navigation →
Overlay → Collections → Charts/Graph → CustomControl), each control with its
mandatory lowering-parity test → regenerate `catalogFacts` 6→47 and the catalog
artifacts; extend `CatalogTests.typedPropsById` → interaction + a11y/render tests
→ gallery panel → regenerate the package-surface baseline → write the four
readiness artifacts → run the `Route`-printed gates + the escalated six-target
order to green with a PASS `EvidenceAudit` and zero `[S]`.
