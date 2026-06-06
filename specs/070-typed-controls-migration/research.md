# Phase 0 Research: Migrate Remaining 41 Controls to the Typed Front Door

All facts below were read from source on 2026-06-06. The five `065` decisions
(Q1–Q5: legacy kept as peer; `FS.Skia.UI.Controls.Typed` namespace; adapter
unchanged; sealed `Widget`; reuse existing models) are **settled and not
reopened** — they are baked into the spec's Assumptions. This file resolves only
the genuinely new questions `070` introduces. No `NEEDS CLARIFICATION` remains.

## Grounding (verified current state)

| Fact | Evidence |
| --- | --- |
| Six controls already typed under `FS.Skia.UI.Controls.Typed` | `src/Controls/Widget.fsi`, `src/Controls/Widgets/{Primitives,TextBoxWidget,DataGridWidget}.fsi` (TextBlock, Button, CheckBox, Stack, TextBox, DataGrid) |
| Catalog lists 47 controls; 41 remain legacy-only | `src/Controls/catalog.yml` (`supportedCount: 47`); the six typed ids are `text-block`, `button`, `check-box`, `text-box`, `stack`, `data-grid` |
| `066` single source is keyed by id, currently 6 facts | `build/Governance/CatalogGen.fs:44` `catalogFacts`; `currency`/`spliceWith` validate/splice **only** ids present in `catalogFacts` |
| The typed cross-check binds catalog id → typed `Props` type, not module name | `tests/Controls.Tests/CatalogTests.fs:104` `typedPropsById` (hand-maintained 6-entry map); asserts `catalogFacts` ids == typed ids and each `requiredAttribute` PascalCased is a `Props` field |
| Several catalog ids share one backing legacy module | `catalog.yml`: `Collections` backs `list-view`/`list-box`/`multi-select-list`/`combo-box`/`tree-view`/`scroll-viewer`/`split-view`; `Menu` backs `menu`/`context-menu` |
| Collections exposes only an MVU model — no `*.create` builder | `src/Controls/Collections.fsi` (`visibleRange`/`init`/`update`; `CollectionModel`/`Msg`/`Effect`) |
| Most other controls have a dedicated legacy `*.create` | `src/Controls/Control.fsi` (Label, Image, Icon, Separator, Badge, IconButton, NumericInput, Switch, Slider, RadioGroup, Grid, Dock, Wrap, Border, Panel, ProgressBar, Spinner, ValidationMessage, Tabs, Menu, Toolbar, Tooltip, Dialog, Toast, Overlay, TextArea); charts in `Charts.fsi` (`LineChart`/`BarChart`/`PieChart`/`ScatterPlot`/`GraphView` `.create`); `CustomControl.fsi` (`create`/`validate`) |
| Generic kind builders exist for controls with no dedicated `*.create` | `src/Controls/Control.fsi:6` `Control.create: ControlKind -> Attr<'msg> list -> Control<'msg>`, `:8` `Control.standard: StandardControlKind -> Attr<'msg> list -> Control<'msg>` |
| Routing already escalates this path and names the evidence | `build/Governance/Routing.fs` `controls-public-surface` (paths `src/Controls/**`); the spec's Evidence obligations name the four readiness files |

## Decision R1 — One typed module per catalog **id**, named by PascalCase id

**Decision**: Each of the 41 remaining catalog ids gets its **own** typed module
under `FS.Skia.UI.Controls.Typed`, named the PascalCase of the id (`rich-text` →
`RichText`, `icon-button` → `IconButton`, `list-view` → `ListView`,
`context-menu` → `ContextMenu`, `scroll-viewer` → `ScrollViewer`, `line-chart` →
`LineChart`, …), each exposing `Props` + `defaults` + `view` (+ `init`/`update`
for stateful ones). This holds even where several ids share a backing legacy
module/model.

**Rationale**: FR-001 and SC-001 require *each of the 41 controls* / *all 47
catalog controls* to have a typed module exposing `defaults` + `view`. A per-id
module is the only reading that satisfies "every catalog control is authorable
through the typed front door" (US1) — an author types `Typed.ListView.view` or
`Typed.ComboBox.view`, not a generic collection module with a mode flag. It also
keeps the `065` invariant uniform ("the catalog `Module` fact names the typed
module"), so the `066` cross-check stays mechanical.

**Alternatives considered**: One typed module per *legacy backing module* (e.g. a
single `Typed.Collections` with a variant enum) — rejected: it collapses five
distinct catalog ids into one module, fails the per-id authoring goal, and would
make the `id → Props type` cross-check ambiguous. A `*Widget` suffix — rejected
for the same reason `065` rejected it (noisier at every call site; the namespace
segment already disambiguates from the legacy modules).

## Decision R2 — Shared models are reused, never forked; per-id modules delegate

**Decision**: Where multiple per-id typed modules sit over one legacy model, every
one of those modules **delegates to the same existing model** rather than
introducing a parallel type. The five selection collections (`list-view`,
`list-box`, `multi-select-list`, `combo-box`, `tree-view`) reuse the existing
`Collections` model (`CollectionModel`/`CollectionMsg`/`CollectionEffect`) via
`Collections.init`/`update`. `text-area` reuses `TextInput`
(`TextInputModel`/`Msg`/`Effect`) exactly as the typed `TextBox` does in `065`.
Chart/graph ids reuse the existing chart/graph models. `data-grid` is already
typed over `DataGrid` from `065`.

**Rationale**: FR-004/SC-003 forbid a forked model and require the typed `update`
to produce model/effects identical to the reused model's `update`. Delegation
keeps the façade thin and the behavior provably identical (the interaction tests
assert equality against the existing model directly). It mirrors the `065`/`068`
discipline and keeps the migration additive at the model layer (Principle IV — no
new effect types, no I/O in `update`).

**Alternatives considered**: A fresh per-id model for each collection variant —
rejected (FR-004 violation; doubles the state + test surface; invites drift).

## Decision R3 — Lowering-parity target = whatever legacy path authors that control today

**Decision**: Each typed `view`'s lowering-parity target is the **existing legacy
authoring path** for that control:

- Controls with a dedicated legacy `*.create` (most of `Control.fsi`, the charts
  in `Charts.fsi`) lower structurally equal to that `*.create [ … ]` call.
- Controls whose only legacy authoring path is the generic kind builder (the
  selection collections / containers with no dedicated `*.create`, e.g. the
  `Collections`-backed ids) lower structurally equal to `Control.standard kind
  [ … ]` (or `Control.create kind [ … ]`) with the same `ControlKind`,
  attributes, children, content, and accessibility the legacy path emits.
- `custom-control` has no schema; see R4.

The per-control parity test (extending `TypedLoweringTests.fs`) pins the exact
target and asserts structural `Control<'msg>` equality after normalizing
attribute order — exactly the `065` keystone method.

**Rationale**: FR-002/SC-002 require structural equality to *the equivalent legacy
builder for the same logical inputs*, so the downstream render/layout/
diagnostics/a11y/event pipeline is exercised byte-unchanged. Pinning to the actual
legacy path (per-`*.create` where it exists, the generic kind builder otherwise)
is the faithful target; parity is asserted on the **lowered** `Control<'msg>`, not
the typed field shape, so a richer typed field can still lower to an `obj`-carrying
legacy attribute without breaking the no-`obj`-in-`Props` rule (the `065`
parity-vs-no-`obj` resolution carries over).

**Alternatives considered**: Inventing a new uniform builder for the
no-`*.create` controls — rejected (would change the legacy surface and the parity
target; the generic `Control.standard`/`create` is the real legacy path).

## Decision R4 — `custom-control` is typed via `Widget.ofControl`, not a fabricated schema

**Decision**: `custom-control`'s typed affordance is the existing public
`Widget.ofControl : Control<'msg> -> Widget<'msg>` bridge (lift an author-built
`Control<'msg>` into the typed tree). It does **not** get a synthetic `Props`
record of made-up fields. It is "typed" in that it produces a `Widget<'msg>`
through the front door.

**Rationale**: FR-006 mandates this; the spec's interacting-requirements note
resolves SC-001 honestly ("all 47 typed") without fabricating a schema for a
deliberate escape hatch that has no fixed attribute set. `Widget.ofControl` is
already public from `065`, so this adds no signature.

**Catalog/cross-check consequence**: because the `066` `typedPropsById` map keys
each catalog id to a `Props` **type**, `custom-control` is the one id with no
`Props` record. The cross-check is extended to treat `custom-control` as the
`Widget.ofControl`-satisfied row (it has no `requiredAttributes` to map, or its
row is marked as bridge-typed) so `ControlsCatalogGenerationCheck` and the
`CatalogTests` correspondence stay green without a fake type.

## Decision R5 — Catalog single source regenerated from 6 → 47 (never hand-edited)

**Decision**: `CatalogGen.catalogFacts` is extended from the current 6 entries to
all 47, and `catalog.yml` + `Catalog.fs` are **regenerated** from it via
`RegenerateCatalog` / `./fake.sh build -t RefreshSurfaceBaselines` — never
hand-edited. The hand-maintained `tests/Controls.Tests/CatalogTests.fs`
`typedPropsById` map is extended in lockstep to all 47 typed `Props` types, so the
"`catalogFacts` ids == typed ids" and "each `requiredAttribute` PascalCased is a
`Props` field" assertions hold for the full set.

**Rationale**: FR-012/SC-007 require `ControlsCatalogGenerationCheck` to stay
green with the catalog facts and the typed surface consistent, and any
`Module`-fact update to be a regeneration. The `066` `currency` function only
validates ids present in `catalogFacts`; expanding the fact table is the
mechanism that brings the other 41 rows under generation. The `requiredAttribute →
PascalCase → Props field` invariant constrains `Props` field naming (e.g. a row
whose `requiredAttributes` includes `text` must have a `Text` field), which is a
real authoring constraint each control's `Props` must satisfy.

**Alternatives considered**: Leaving the 41 rows hand-authored and only generating
the 6 — rejected: it leaves the typed surface and the catalog out of sync once the
41 are typed, and the spec explicitly forbids hand-edited catalog facts.

## Decision R6 — The `fs-skia-typed-controls` skill lands in this branch, validated against the migration

**Decision**: Author the new canonical `.agents/skills/fs-skia-typed-controls/
SKILL.md` capability skill **in this branch, before/while doing the migration**,
and regenerate its `.claude` peer via `RefreshSurfaceBaselines`. The skill teaches
the add-a-typed-control flow (pick taxonomy fields → write `Props` + `defaults` +
`view` → add the mandatory lowering-parity test → reuse the existing MVU model for
stateful controls → keep the surface additive). `070`'s 41 migrations are the
skill's validation corpus.

**Rationale**: FR-013 and plan §16.4 require it ("each new skill should land in the
same feature branch that first needs it" so its guidance is validated against real
work, not written speculatively). It must pass `SkillSyncCheck`/`SkillQualityCheck`
(SC-008). Never hand-edit the `.claude` copy — `SkillSyncCheck` flags the drift.

## Cross-cutting confirmations (carried from `065`, unchanged)

- **No new dependency** (FR-008/SC-006): typed modules depend only on `Widget` +
  existing control/model modules. The `Controls.fsproj`-has-no-`Fable.Elmish`
  guard stays green.
- **Legacy frozen** (FR-007/SC-009): no existing `.fsi` signature is added,
  removed, or changed; the `FS.Skia.UI.Controls.Typed` namespace isolates the new
  clean names from the legacy modules; `PackageSurfaceCheck` sees additions only.
- **Parity vs no-`obj`**: parity is asserted on the lowered `Control<'msg>`
  (normalized attribute order), so a strongly typed `Props` field may lower into a
  legacy `UntypedValue of obj` attribute without exposing `obj` on the typed
  surface (FR-003/SC-005 remain satisfied).

## Outcome

All `070`-specific design questions are resolved (R1–R6). The `065` decisions are
unchanged. No `NEEDS CLARIFICATION` remains; Phase 1 design proceeds.
