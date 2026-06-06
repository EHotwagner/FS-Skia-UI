# Phase 0 Research — Catalog Expansion (New Typed Controls)

All Technical Context unknowns are resolved below; no `NEEDS CLARIFICATION` remains.

## R1 — How does a genuinely new control avoid a new `ControlKind`?

- **Decision**: Each new control is a **typed-first composition** of existing legacy
  builders. Its `view` builds a `Control<'msg>` tree whose nodes are existing kinds (root
  is an existing container kind: `Border`/`Stack`/`Toolbar`), then `Widget.ofControl`. No
  string `Kind` value is added to the IR and the renderer/layout engine is untouched.
- **Rationale**: The IR `Control.Kind` is a string and the renderer dispatches on existing
  kinds. The catalog cross-check (`CatalogTests.typedPropsById`, `TypedMigrationTests`) keys
  on the catalog row's **`Module`** (the typed module name) and its **`RequiredAttributes`**
  vs. the typed `Props` **fields** — it does **not** assert a root `ControlKind`. So a new
  control is identified by its typed module + catalog row, not by a new IR node. This keeps
  the change additive and render-path-free (spec FR-004, interacting-requirements
  resolution).
- **Alternatives considered**: (a) Add a new `ControlKind` + renderer arm per control —
  rejected: touches the render path, breaks "additive-only / no renderer change," and is a
  far larger blast radius. (b) Add legacy `*.create` builders too — rejected as unnecessary
  scope; the typed module composes existing builders directly and the legacy surface stays
  byte-frozen (FR-008).

## R2 — Where do the new controls' values and popup state live (MVU vs Props)?

- **Decision**: Values and popup open/closed are **product-owned in `Props`** — no new
  `Model`/`Msg`/`Effect`. `ToggleButton.IsOn: bool`, `DatePicker.Value: DateOnly option` +
  `IsOpen: bool`, `TimePicker.Value: TimeOnly option`, `ColorPicker.Selected: Color option`,
  `SplitButton.IsOpen: bool`, each with an optional typed callback.
- **Rationale**: This mirrors the already-shipped value-bearing controls `CheckBox`,
  `Switch`, `RadioGroup`, `Slider` (Primitives/Input) — all carry their value + an optional
  `(… -> 'msg)` callback and own no model. The product's own Elmish `update` owns the value;
  the control is a pure projection of it. Satisfies Constitution IV (no stateful
  framework-owned workflow is introduced, so no MVU ceremony is required) and FR-007.
- **Alternatives considered**: A `DatePickerModel`/`Msg` owning calendar navigation +
  open/closed — rejected: it would be a **new MVU model** (FR-007 forbids) and is not needed
  for a `Props`-projection control; the established stateful controls (`TextInput`,
  `DataGrid`) reuse *existing* models only.

## R3 — Typed value types for date, time, and color (no `obj`, no string-encoding)

- **Decision**: Date → BCL `System.DateOnly`; time → BCL `System.TimeOnly`; color → reuse
  `FS.Skia.UI.Scene.Color` (already referenced by `Controls.fsproj`). A `ColorSwatch` value
  record (`{ Name: string; Color: Color }`) is added only if a display name is needed;
  otherwise `Color` is used directly. `SplitButton` menu items use a small
  `SplitButtonItem` record (`{ Key: string; Label: string }`) consistent with the existing
  string-keyed menu/collections items.
- **Rationale**: All precise, no `obj`, no string-encoded date/time/color (FR-003, SC-007);
  all from the BCL or an already-referenced package, so **no new dependency** (FR-004).
  `DateOnly`/`TimeOnly` are unused in the repo today — purely additive.
- **Alternatives considered**: `System.DateTime` for a date — rejected (carries a spurious
  time component; `DateOnly` is the precise type). A hex `string` color — rejected
  (string-encoding where `Scene.Color` exists). A NuGet date/time-picker dependency —
  rejected (no new dependency; controls are composed in-repo).

## R4 — How is the lowering-parity test expressed when there is no single legacy builder?

- **Decision**: The parity fixture is the **explicit hand-written composition of existing
  legacy builders** the `view` re-expresses (e.g. `DatePicker` ≡
  `Border.create [ … Stack.create [ TextBox/Label field; trigger Button; Overlay.create [ calendar Stack of day Buttons ] ] ]`).
  The test asserts `view props |> Widget.toControl` is structurally equal to that
  composition, order-normalized with events canonicalized to the message they produce —
  identical discipline to `065`/`070` (`TypedMigrationTests`).
- **Rationale**: The keystone proof (spec FR-005/SC-002) does not require a *single* legacy
  builder; it requires the typed `view` to be a faithful façade over **existing IR**. Pinning
  to an explicit composition gives the same protection (every downstream render/a11y test
  stays valid) and makes the lowering greppable.
- **Alternatives considered**: Snapshot-only golden IR with no hand-written reference —
  rejected: a hand-written composition documents *what* the control lowers to and catches
  silent drift better than an opaque snapshot.

## R5 — Extending the single-source catalog from 47 to 52

- **Decision**: Add 5 facts to `CatalogGen.catalogFacts` (`toggle-button`, `split-button`,
  `date-picker`, `time-picker`, `color-picker`), each with its `Module`/`RequiredAttributes`/
  `Events`/`AccessibilityRole`; add the 5 `BEGIN/END GENERATED: typed-catalog/<id>` marker
  regions to `catalog.yml` and `Catalog.fs`; regenerate via
  `./fake.sh build -t RefreshSurfaceBaselines`; bump the `catalog.yml` header
  `supportedCount` 47→52 and the matching `CatalogTests.fs` assertion; capture the 5 per-fact
  parity fixtures the `066` cross-check reads. No generator-mechanism change — none of the
  new ids is evidence-carrying, so the chart/data-grid special-case is untouched.
- **Rationale**: Reuses the `066`/`071` mechanism exactly (FR-006, spec Assumptions). The
  `typedPropsById` cross-check extends naturally: each new id maps to its new typed `Props`
  type and the required attribute(s) must appear as `Props` fields. `Catalog.supportedCount ()`
  counts rows and updates automatically; only the YAML header constant + its test assertion
  are hand-bumped.
- **Alternatives considered**: Leaving the new rows hand-maintained outside the markers —
  rejected: it would regress the single-source guarantee `071` just completed (the very
  thing the currency gate enforces).

## R6 — Catalog `RequiredAttributes` for optional-valued pickers

- **Decision**: Pickers whose selection is genuinely optional carry **`RequiredAttributes =
  []`** (following `switch`/`spinner`/`separator` precedent): `date-picker`, `time-picker`.
  Controls with a required content set carry it: `color-picker → ["swatches"]`,
  `toggle-button → ["text"]`, `split-button → ["text"]`. Each required attribute (PascalCased)
  is present as a `Props` field, satisfying the cross-check.
- **Rationale**: A required attribute asserts the field is part of the authoring contract,
  not that its value is non-`None`. An empty no-selection date/time is a valid edge case
  (spec Edge Cases), so its value field is `… option` and the row declares no required
  attribute — exactly how `switch` (a value-bearing control) declares `[]`.
- **Alternatives considered**: Declaring `value` required for the pickers — rejected: it
  implies a mandatory selection and conflicts with the empty-state edge case.

## R7 — Accessibility roles for controls without a dedicated role case

- **Decision**: Map to the nearest existing `AccessibilityRole`/catalog role string:
  `toggle-button → Button`, `split-button → Menu`, `date-picker`/`time-picker → TextBox`,
  `color-picker → List`. The lowered tree carries the role via the same accessibility
  attributes the composed legacy builders already emit.
- **Rationale**: No new role is introduced (additive, no IR change); the chosen roles are
  the closest semantic match among shipped roles and follow existing catalog precedent
  (e.g. compound controls reuse `List`/`Menu`). Render/accessibility tests assert the role
  on the lowered tree (FR-009).
- **Alternatives considered**: New role cases (`Toggle`, `DatePicker`) — rejected as a
  surface/IR addition beyond the slice's additive intent.
