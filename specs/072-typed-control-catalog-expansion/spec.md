# Feature Specification: Catalog Expansion — New Typed Controls (Buttons / Pickers / Date-Time)

**Feature Branch**: `072-typed-control-catalog-expansion`
**Created**: 2026-06-06
**Status**: Draft
**Input**: User description: "implement the next part of the typed-controls front-door implementation plan" — the next part, selected as the **`071+` breadth expansion** (catalog expansion: buttons / pickers / date-time), distinct from the housekeeping `071` that already merged.

## Overview

Features `065`–`071` built and proved the typed authoring front door: a sealed
`Widget<'msg>`, immutable per-control `Props` records under the
`FS.Skia.UI.Controls.Typed` namespace, optional reused-MVU façades, lowering-parity
tests, and a single-source catalog (`CatalogGen.catalogFacts` →
`src/Controls/catalog.yml` + `Catalog.fs`, currency-enforced by
`ControlsCatalogGenerationCheck`). `070` migrated all 41 remaining controls onto that
front door and `071` brought every one of the **47** catalog rows under single-source
generation. That work was uniformly *re-expression* — it added no control the catalog
did not already list.

This feature is the first **breadth expansion** the roadmap deferred to `071+`: it
introduces a small reference slice of **genuinely new controls** that the catalog has
never had, drawn from the three families the plan names — **buttons**, **pickers**, and
**date-time**. The current catalog has no date-or-time control and no dedicated picker
beyond `ComboBox`. The new controls ship as typed-first modules under
`FS.Skia.UI.Controls.Typed`, grow the catalog from 47 rows under the **same** single-source
generation, and are each proven by the same lowering-parity discipline established in
`065`.

Crucially, every new control is a **composition of existing controls** — it lowers to a
tree of `Control<'msg>` IR nodes whose `Kind`s already exist (e.g. `Button`, `TextBox`,
`Overlay`, `Stack`, `Border`). This feature introduces **no new `StandardControlKind` variant**, **no
renderer/layout change**, **no new MVU model primitive**, and **no new package
dependency**. It is additive to the public surface only. The breadth beyond this slice
(the full button/picker/date-time families, overlays as a feature, virtualization,
motion) stays deferred; this is the *representative-slice* expansion, not the exhaustive
one — exactly as `065` was a six-control slice rather than all 47.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A product author uses a new date-time control through the typed front door (Priority: P1)

A product developer building a generated FS.Skia.UI app needs the user to enter a date.
No date control exists today, so they hand-build one from a `TextBox` and parse strings.
With this feature they author `Typed.DatePicker.view { Typed.DatePicker.defaults with Value = Some today; OnChange = Some DateChosen }` — a compiler-checked `Props` record with a
typed date value (a BCL `System.DateOnly`, not a string), an optional message callback, and
a popup calendar — and it composes into their existing typed `view` like any other
`Widget<'msg>`.

**Why this priority**: The date-time family is the largest concrete gap in the catalog and
the clearest user-visible value of "expansion." Proving one date-time control end-to-end
through the typed front door (typed value, popup composition, parity, render evidence)
validates the whole expansion pattern for every later date-time control.

**Independent Test**: Author a panel using `Typed.DatePicker.view`, render it at ≥2
viewports, confirm the lowered control is structurally equal to the explicit composition
of existing legacy builders (parity), and confirm selecting a day dispatches the typed
`OnChange` message carrying the chosen `DateOnly`.

### User Story 2 - A maintainer adds a new catalog control from the single fact source (Priority: P1)

A framework maintainer adds each new control's catalog facts to **one** source —
`CatalogGen.catalogFacts` — and regenerates via `./fake.sh build -t RefreshSurfaceBaselines`.
`catalog.yml` (`supportedCount` grows from 47) and `Catalog.fs` are produced from that
single table for the new rows exactly as for the existing 47; no new row is hand-edited;
and `ControlsCatalogGenerationCheck` fails if any generated row — old or new — drifts from
the fact table.

**Why this priority**: The expansion must *extend* the single-source guarantee `071`
completed, not regress it by re-introducing hand-maintained rows. Adding a row must remain
a one-edit-plus-regenerate operation.

**Independent Test**: Add the new ids to `catalogFacts`, regenerate, confirm the new rows
appear in both artifacts; hand-edit one generated new row and confirm
`ControlsCatalogGenerationCheck` fails; revert and confirm it passes.

### User Story 3 - A new button-family control carries state without a new model (Priority: P2)

A product author needs a button that reflects an on/off state (a `ToggleButton`) and a
button that offers a primary action plus a dropdown of secondary actions (a `SplitButton`).
They author both through typed `Props` — boolean-valued and command-list-valued
respectively — with optional typed message callbacks, and the controls reuse the existing
control/composition machinery (no new `Model`/`Msg`/`Effect` type is invented).

**Why this priority**: The button family demonstrates the two remaining new-control
mechanics (a product-owned boolean toggle, and a command-list with a popup menu) and
proves the expansion needs no new state primitive — value-carrying controls stay
product-owned like `CheckBox`, popup controls reuse the existing overlay/menu composition.

**Independent Test**: `Typed.ToggleButton` with `IsOn = true` lowers to the pressed-state
composition and dispatches `(bool -> 'msg)` on toggle; `Typed.SplitButton` lowers to a
primary action plus a popup menu whose items dispatch their typed messages.

### Edge Cases

- **Empty / no selection**: a `DatePicker`/`TimePicker` with no chosen value renders an
  empty field and dispatches no message; `defaults` define the no-selection state.
- **Picker with empty option set**: a palette/swatch `ColorPicker` (or `SplitButton` with
  an empty command list) renders the trigger but an empty/disabled popup — it must not
  fail to lower.
- **Optional callback is `None`**: every typed event callback lowers to **no binding** when
  `None` (the established front-door rule), so a display-only instance is valid.
- **Out-of-range value**: a `TimePicker` value outside 00:00–23:59 or a malformed date is
  unrepresentable because the typed value is a BCL `TimeOnly`/`DateOnly`, not a string —
  the compiler prevents the invalid state rather than a runtime guard.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The catalog MUST gain a bounded reference slice of new controls beyond the
  existing 47, spanning the three named families — **buttons**, **pickers**, and
  **date-time** — with at least one control representing each family.
- **FR-002**: Each new control MUST ship a typed module under the
  `FS.Skia.UI.Controls.Typed` namespace with an immutable `Props` record, a `defaults`
  value, and a `view : Props<'msg> -> Widget<'msg>`, following the front-door pattern
  established in `065`/`070`.
- **FR-003**: Each new control's typed value MUST use a precise type, never `obj` and never
  a string-encoded value where a structured type exists — date and time values use the BCL
  `System.DateOnly` / `System.TimeOnly`; events are optional typed callbacks.
- **FR-004**: Each new control's `view` MUST lower to a `Control<'msg>` tree built **only**
  from existing composition builders (`Stack`/`Border`/`Overlay`/`Grid`/`Wrap`/`Menu`/
  `Button`/…); this feature MUST NOT add a new `StandardControlKind` variant — the closed
  control-kind enum (`ControlKind` itself is a `string` alias) — nor change the renderer or
  layout engine, nor add a package dependency.
- **FR-005**: Each new control MUST have a **lowering-parity test** asserting its lowered IR
  is structurally equal (order-normalized, events canonicalized to the message they
  produce) to the explicit, hand-written composition of existing legacy builders it
  re-expresses — the keystone proof from `065`/§10.3.
- **FR-006**: Each new control MUST be added to `CatalogGen.catalogFacts` as the single
  source, with `catalog.yml` (updated `supportedCount`) and `Catalog.fs` **generated** from
  it; no new catalog row may be hand-maintained, and `ControlsCatalogGenerationCheck` MUST
  enforce currency over the grown set.
- **FR-007**: New controls that carry persistent UI state MUST reuse existing
  model/effect machinery (no new `Model`/`Msg`/`Effect` primitive); product-owned values (a
  toggle's boolean, a picker's selection) stay in the `Props` as the author's value,
  mirroring `CheckBox`.
- **FR-008**: The public surface change MUST be **additive only** — no existing `.fsi`
  signature changes and no legacy `*.create`/`Attr` builder is removed or deprecated; the
  surface baselines are regenerated to reflect only additions.
- **FR-009**: Each new control MUST carry accessibility metadata (role/name/keyboard
  affordance) on its lowered tree and MUST be exercised by the rendering/accessibility
  suites at ≥2 viewports, with captured deterministic render evidence.
- **FR-010**: Each new control MUST appear in the persistent `samples/ControlsGallery`
  typed-authoring panel so the expansion is dogfooded end-to-end.
- **FR-011**: The required routing evidence artifacts for the escalated
  `controls-public-surface` path MUST exist and be populated for this feature.

> Interacting / conflicting requirements: "introduce genuinely new controls (FR-001)" vs.
> "no new `StandardControlKind` variant, no renderer change, additive-only (FR-004/FR-008)". Resolution:
> new controls are **typed-first compositions** — they exist as new `FS.Skia.UI.Controls.Typed`
> modules that build their lowered tree from existing legacy builders/`ControlKind`s
> (e.g. `DatePicker` = a field control + an `Overlay` popup containing a `Stack`/`Grid` of
> day `Button`s). The parity test (FR-005) pins each new control to the explicit existing
> composition, so "new control" never means "new IR node" or "new renderer." A new
> top-level legacy single-control `*.create` builder is **not** required and is out of
> scope; the typed module is the authoring surface and composes the existing builders
> internally.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: `FS.Skia.UI.Controls` gains additive public API (new typed modules
  and `Props` records); package identity is unchanged; the next post-merge bump/pack and
  template-pin flow follows the usual `speckit-merge` / `fs-skia-template-update` path. No
  legacy Charts package migration is involved.
- **Public contract impact**: New `.fsi` declarations under `FS.Skia.UI.Controls.Typed`
  (the new modules/records) — additive only; both the controls public-surface baseline and
  the per-package surface baseline are regenerated to reflect additions, with no signature
  change to any existing declaration.
- **State workflow impact**: No new MVU model/effect/interpreter is introduced; stateful
  new controls reuse existing machinery and product-owned `Props` values; no I/O in any
  `update`.
- **Layout/rendering impact**: No renderer, layout-engine, Skia, or Vulkan change. New
  controls lower to compositions of existing `ControlKind`s and are exercised by the
  existing rendering/accessibility suites at ≥2 viewports; deterministic render evidence is
  captured via evidence mode.
- **Evidence obligations**: Real evidence paths under
  `specs/072-typed-control-catalog-expansion/readiness/` —
  `typed-controls-front-door.md` (front-door/parity statement, explicitly **no `[S]`**
  synthetic lowering), `package-surface-expectations.md` (additive surface delta),
  `controls-rendering.md` (viewport render evidence), and a parity matrix for the new
  controls. Lowering is **real**, so no `[S]` disclosure is expected.
- **Unsupported scope**: Exhaustive coverage of the button/picker/date-time families,
  overlays as a standalone feature, list virtualization, motion/animation, and any
  Penpot/MCP design-sync work are out of scope (see Out of Scope).
- **Build-target impact**: No new build target. Routing escalates to the
  `controls-public-surface` gate set plus `ControlsCatalogGenerationCheck` (catalog
  currency) and `DesignTokenDrift` (must stay green); the serialized escalated six-target
  order applies. `Route` is authoritative — run it and run only the gates it prints.

## Success Criteria *(mandatory)*

- **SC-001**: A product author can place each new control in a typed `view` using only its
  `Props` record + `defaults`, with no string keys and no `obj`, and the project compiles.
- **SC-002**: For every new control, the typed `view`'s lowered IR is structurally equal to
  the explicit existing-builder composition it re-expresses (100% of new controls pass the
  lowering-parity test).
- **SC-003**: The catalog `supportedCount` reflects the new total and every new row is
  generated from `CatalogGen.catalogFacts`; hand-editing any generated row (old or new)
  fails `ControlsCatalogGenerationCheck`.
- **SC-004**: Every existing test remains green and every existing public `.fsi` signature
  is byte-unchanged; the only surface-baseline delta is additions.
- **SC-005**: Each new control renders at ≥2 viewports with stable node counts and carries
  accessibility metadata, with captured render evidence.
- **SC-006**: `./fake.sh build -t Route --enforce` over the branch diff prints the escalated
  `controls-public-surface` path and every printed gate passes, with all required evidence
  artifacts present and populated.
- **SC-007**: No new package dependency, no new `StandardControlKind` variant (the closed
  control-kind enum; `ControlKind` is a `string` alias), and no new MVU model primitive
  appear in the diff.

## Key Entities

- **New typed control module** (`FS.Skia.UI.Controls.Typed.<Control>`): the additive public
  authoring surface — an immutable `Props<'msg>` record drawn from the variable taxonomy, a
  `defaults` value, and a `view` returning `Widget<'msg>`.
- **Typed value types**: `System.DateOnly` (date pickers), `System.TimeOnly` (time
  pickers), `bool` (toggle), and a typed command/option list (split button / palette
  picker) — all precise, no `obj`.
- **Catalog fact row** (`CatalogGen.catalogFacts` entry): the single source for each new
  control's id, module name, required attribute(s), and evidence pointer, from which
  `catalog.yml` and `Catalog.fs` rows are generated.
- **Lowering-parity fixture**: per new control, the explicit hand-written composition of
  existing legacy builders the typed `view` must equal.

## Assumptions

- **Reference slice, representative not exhaustive.** Like `065`'s six-control slice, this
  feature ships a small set spanning the three families — proposed: `ToggleButton` and
  `SplitButton` (buttons), `ColorPicker` as a palette/swatch picker (pickers), and
  `DatePicker` + `TimePicker` (date-time). The exact membership is a plan-phase detail; the
  spec requires ≥1 per family (FR-001). Full-family coverage is deferred.
- **Typed-first composition, no new IR.** New controls are compositions of existing
  controls and lower to existing control kinds; no new legacy single-control `*.create`
  builder, no new `StandardControlKind` variant, no renderer change (FR-004, interacting-requirements
  resolution). Popup-bearing controls reuse the existing `Overlay`/`Menu` composition.
- **BCL value types only.** Date/time values use `System.DateOnly`/`System.TimeOnly` from
  the BCL — no new package dependency (FR-003, SC-007).
- **`ColorPicker` is a palette/swatch picker**, composed from existing colored
  `Border`/`Button` cells — a full color-wheel/gradient picker (which would need new
  rendering) is out of scope.
- **Single-source pattern reused as-is.** The `066`/`071` catalog generation mechanism
  (per-row generation markers, the chart/data-grid evidence special-case, one fixture per
  fact) is extended to the new rows, not replaced.
- **Escalated path.** Because public `.fsi` and catalog facts change, `Route` escalates to
  `controls-public-surface`; the serialized six-target order is run sequentially per
  AGENTS.md.

## Out of Scope

- **Exhaustive family coverage** — every button variant, every picker, every date-time
  control beyond the reference slice (deferred to later `071+` features).
- **Overlays as a standalone feature, list/grid virtualization, and motion/animation** —
  the other `071+` themes, each its own later feature.
- **Any new `StandardControlKind` variant, renderer/layout change, or new MVU model primitive** — new
  controls are compositions of existing IR.
- **New package dependencies** (including for date/time — BCL types only).
- **Removal or deprecation of any legacy `*.create`/`Attr` builder** — the legacy surface
  stays a byte-frozen peer.
- **Penpot / MCP design-sync and code→design catalog sync** — the plan's "Later" item.
