# Implementation Plan: Catalog Expansion — New Typed Controls (Buttons / Pickers / Date-Time)

**Branch**: `072-typed-control-catalog-expansion` | **Date**: 2026-06-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/072-typed-control-catalog-expansion/spec.md`

## Summary

Ship the first `071+` **breadth expansion**: a representative reference slice of
**five genuinely new controls** the catalog has never had, spanning the three named
families — **buttons** (`ToggleButton`, `SplitButton`), **pickers**/`date-time`
(`DatePicker`, `TimePicker`), and a palette **picker** (`ColorPicker`). Each ships as a
**typed-first** module under `FS.Skia.UI.Controls.Typed` (immutable `Props` record +
`defaults` + `view : Props<'msg> -> Widget<'msg>`), composed **only** from existing legacy
builders so its lowered `Control<'msg>` tree uses **no new `StandardControlKind` variant** and triggers
**no renderer/layout change**. Every new control is pinned by the keystone **lowering-parity
test** (`view props |> Widget.toControl` ≡ the explicit hand-written composition) and is
brought under the **single-source catalog** (`CatalogGen.catalogFacts` →
`src/Controls/catalog.yml` + `Catalog.fs`, grown 47→52), currency-enforced by
`ControlsCatalogGenerationCheck`. The change is **additive-only** to the public surface (new
typed modules/records; one reused `Scene.Color`; BCL `DateOnly`/`TimeOnly`), adds **no
dependency**, and invents **no new MVU model** — value-bearing pickers keep their selection
in `Props` (product-owned, like `CheckBox`).

This is **Tier 1 (contracted change)** — it adds public API and catalog facts, so `Route`
escalates to `controls-public-surface`. It is the *representative-slice* expansion, not the
exhaustive family rollout; the remaining `071+` themes (full families, overlays as a
feature, virtualization, motion, Penpot/MCP) stay deferred.

**Delivery order (incremental, family-complete at MVP):**

- **P1 — MVP, covers all three families.** `DatePicker` (date-time **and** picker mechanic:
  typed `DateOnly` value + popup calendar) + `ToggleButton` (button: product-owned boolean
  + `(bool -> 'msg)`), both catalogued from the single fact source (47→49). This proves the
  whole expansion pattern end-to-end (typed value, composition, parity, catalog currency,
  render evidence).
- **P2 — button + date-time breadth.** `SplitButton` (primary action + popup `Menu` of
  secondary commands) and `TimePicker` (typed `TimeOnly` value). 49→51.
- **P3 — palette picker.** `ColorPicker` (Wrap/Grid of `Scene.Color` swatch cells). 51→52.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: **None added.** Existing only — `FS.Skia.UI.Scene` (already
referenced by `Controls.fsproj`; provides the `Color` type reused by `ColorPicker`), the BCL
(`System.DateOnly`/`System.TimeOnly`, no package), SkiaSharp (render path, unchanged),
Expecto + FsCheck (tests), the FAKE front-end `FS.Skia.UI.Build` (catalog generation,
routing, currency gate). `Directory.Packages.props` / `docs/dependencies.md` untouched.
**Testing**: Expecto over `tests/Controls.Tests/` — new lowering-parity + contract tests
(`TypedMigrationTests.fs` pattern, or a new `TypedExpansionTests.fs`), catalog cross-check
extension (`CatalogTests.fs` `typedPropsById` + `supportedCount`), interaction tests
(`InteractionTests.fs`), and rendering/accessibility coverage of the new controls at ≥2
viewports (`RenderingTests.fs`, `AccessibilityTests.fs`). FAKE targets
`RefreshSurfaceBaselines` (regenerate catalog + surface baselines) and
`ControlsCatalogGenerationCheck` (currency). Deterministic render-only evidence capture.
**Target Platform**: Windows and Linux. New controls lower to existing IR and render
headlessly through `Control.render`/`Widget.toControl` (no GPU window needed); evidence is
render-only and byte-identical on re-capture.

**Routing note**: `./fake.sh build -t Route` on the *spec/plan-only* working tree prints the
docs/specify tier (`Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph`). Once the
implementation diff lands (new `src/Controls/Widgets/*.fsi`/`.fs`, `Controls.fsproj` compile
entries, `build/Governance/CatalogGen.fs` facts, regenerated `catalog.yml`/`Catalog.fs`,
`tests/Controls.Tests/**`, `samples/ControlsGallery/Program.fs`, surface baselines,
readiness), **re-run `Route`** and run exactly the gates it then prints — public `.fsi` +
catalog facts escalate to `controls-public-surface`
(`ControlsCatalogCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`,
`PackageSurfaceCheck`, `FsiTranscripts`, `GeneratedProductCheck`) plus
`ControlsCatalogGenerationCheck` (catalog currency) and `DesignTokenDrift` (must stay green);
`after_implement` runs `EvidenceAudit`. All Technical Context items are **fully resolved** —
no open clarifications remain (see `research.md`).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` change. This feature
  adds package-internal typed control modules (`src/Controls/Widgets/**`), build-side
  catalog facts (`build/Governance/CatalogGen.fs`), regenerated governance artifacts
  (`catalog.yml`/`Catalog.fs`), tests, and the repo sample — none is a generated-project
  template fragment, package-policy, or command-surface change the template must mirror. The
  post-merge version bump/pack and template-pin refresh are **out of scope** here, owned by
  `speckit-merge` / `fs-skia-template-update` (spec Package impact).
- **Dependency impact**: N/A — **no dependency added** (FR-004/SC-007). `ColorPicker` reuses
  the existing `FS.Skia.UI.Scene.Color`; date/time use BCL `DateOnly`/`TimeOnly`.
  `Directory.Packages.props`, `docs/dependencies.md`, generated template inclusion, and
  `DependencyReport` are unchanged.
- **Command-surface impact**: No `build.fsx`/`Routing.fs`/wrapper change — the catalog
  expansion reuses the existing `RefreshSurfaceBaselines` / `ControlsCatalogGenerationCheck`
  path and the standard `controls-public-surface` gates; `validation.contract.yml` stays
  generated from `Routing.fs`. **Run `./fake.sh build -t Route` first on the implementation
  diff and run only the gates it prints.** FAKE-backed targets share `.fake` state and are
  **not** safe to run concurrently — run them sequentially in the escalated order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  Safe non-FAKE reads/checks may still run in parallel.
- **Generated project impact**: N/A — no change to default/minimal generated contents,
  selected-Controls guidance, local skills, validation logs, placeholder/excluded-history
  scans, or generated `Dev` behavior. `catalog.yml` / `Catalog.fs` are framework governance
  artifacts (catalog cross-check inputs), not generated-project output. The new typed
  modules become available to consumers additively once the package version bumps
  post-merge.
- **Evidence paths**: All under `specs/072-typed-control-catalog-expansion/readiness/`:
  - `typed-controls-front-door.md` — the five new controls, their `Props`/composition
    design, the lowering-parity matrix, and the explicit statement that lowering is **real**
    (no `[S]`).
  - `package-surface-expectations.md` — the additive public-surface delta (new modules /
    records / `ColorSwatch` value type if any) and the regenerated-baseline rationale.
  - `controls-rendering.md` — deterministic ≥2-viewport render evidence for the new controls
    (render-only, **no** `[S]`).
  - `typed-lowering-parity.md` — the per-control parity fixture matrix (5 controls ×
    typed≡composition).
  - Per-fact catalog parity fixtures the `066` cross-check reads land under
    `specs/066-typed-catalog-generation/readiness/parity-fixtures/`
    (`Catalog.fs.<id>.txt` + `catalog.yml.<id>.txt` for each of the 5 new ids).
  - Gate evidence (`evidence-graph.md`, `evidence-audit.md`, per-package surface diff,
    skill-loading) lands under `readiness/` per the printed gates.
- **`.fsi` / contract impact**: **Tier 1, additive-only.** New public `.fsi` declarations
  under `FS.Skia.UI.Controls.Typed` (five new modules + their `Props` records; possibly a
  small `SplitButtonItem`/`ColorSwatch` value record). **No existing signature changes**
  (SC-004). Both the controls public-surface baseline and the per-package surface baseline
  are regenerated to reflect **additions only**, verified by
  `PackageSurfaceCheck`/`PerPackageSurfaceDiff`. Compatibility: purely additive, legacy
  `*.create`/`Attr` peers untouched and not deprecated (FR-008).
- **MVU/effect boundary**: **No new MVU model, no I/O in any `update`.** The new controls
  are stateless from the framework's view — selection/toggle values are **product-owned** in
  `Props` (mirroring `CheckBox`/`Switch`/`RadioGroup`, which carry their value and an
  optional callback). Popup open/closed (`DatePicker`/`SplitButton`/`ColorPicker`) is an
  authored `Props` flag (`IsOpen`), not framework-owned ephemeral state — so no
  `Model`/`Msg`/`Effect` is introduced (FR-007). Catalog generation stays pure
  text/splice/currency with file I/O at the `Engine/Interpret.fs` edge (Principle IV).
- **Synthetic evidence**: **None planned — no `[S]`/`[S*]`/`[SEH]`.** Lowering parity is
  proven against real composed IR; catalog rows are generated from the real fact table and
  cross-checked against the real typed `Props` types; render evidence is real render-only
  output through the existing IR path; parity fixtures are golden bytes captured from the
  generator, not fabricated literals. `EvidenceAudit` must be PASS with no disclosures
  (SC-006).
- **Test evidence**: **Failing-first then green.** Contract tests assert the five typed
  modules + `defaults`/`view` exist (red before the modules land). Parity tests assert
  `view ≡ composition` per control (the keystone, red before `view` bodies). Catalog tests:
  the `typedPropsById` cross-check and `supportedCount` assertion fail on a 52-fact table
  without the new typed `Props` types / fixtures / count, then pass once added; a deliberate
  hand-edit to a generated row makes `ControlsCatalogGenerationCheck` fail (SC-003).
  Interaction tests assert `OnToggle`/`OnChange`/`OnSelected`/`OnClick` dispatch the typed
  message. Rendering/accessibility tests cover the new controls at ≥2 viewports.
- **Observability**: The currency gate already emits an actionable diagnostic naming the
  stale `typed-catalog/<id>` region and the regeneration command
  (`./fake.sh build -t RefreshSurfaceBaselines`); extending to 52 facts preserves that
  per-region diagnostic for the new ids. The fixture cross-check names a missing fixture id
  on a gap. `PackageSurfaceCheck` names any non-additive surface delta. Render evidence
  records captured viewports and the byte-identical re-capture claim.
- **Deferred scope**: Exhaustive family coverage (every button variant, picker, and
  date-time control beyond the five-control slice), overlays as a standalone feature,
  list/grid virtualization, motion/animation, live Penpot/MCP design-sync, and any legacy-API
  deprecation all remain deferred to later `071+` features (spec Out of Scope / FR-001). A
  full color-wheel/gradient `ColorPicker` (which would need new rendering) is out of scope —
  this feature ships the palette/swatch picker only.

## Project Structure

```
src/Controls/Widgets/Buttons.fsi / Buttons.fs      # NEW — ToggleButton, SplitButton (+ SplitButtonItem)
src/Controls/Widgets/Pickers.fsi / Pickers.fs      # NEW — DatePicker, TimePicker, ColorPicker (+ ColorSwatch)
src/Controls/Controls.fsproj                       # MODIFIED — add the 4 new compile entries
                                                   #   (after the existing Widgets/* block)

build/Governance/CatalogGen.fs                     # MODIFIED — catalogFacts 47 -> 52 (5 new facts);
                                                   #   no generator-mechanism change (markers reused)
src/Controls/catalog.yml                           # REGENERATED — supportedCount 47->52; 5 new
                                                   #   typed-catalog/<id> regions
src/Controls/Catalog.fs                            # REGENERATED — 5 new typed-catalog/<id> regions

tests/Controls.Tests/TypedExpansionTests.fs        # NEW — contract + lowering-parity for the 5
                                                   #   new controls (keystone)
tests/Controls.Tests/InteractionTests.fs           # MODIFIED — per-control event dispatch
                                                   #   (OnToggle / OnChange / OnSelected / OnClick)
tests/Controls.Tests/CatalogTests.fs               # MODIFIED — typedPropsById += 5; supportedCount 47->52
tests/Controls.Tests/RenderingTests.fs             # MODIFIED — new controls @ >=2 viewports
tests/Controls.Tests/AccessibilityTests.fs         # MODIFIED — new controls @ >=2 viewports
samples/ControlsGallery/Program.fs                 # MODIFIED — typedAuthoringPanel gains the 5 new controls

specs/066-typed-catalog-generation/readiness/parity-fixtures/
    Catalog.fs.<id>.txt                            # NEW — one per new id (5)
    catalog.yml.<id>.txt                           # NEW — one per new id (5)

specs/072-typed-control-catalog-expansion/
    spec.md  plan.md  research.md  data-model.md  quickstart.md
    checklists/requirements.md
    contracts/
        typed-control-surface.contract.md
        catalog-expansion.contract.md
    readiness/
        typed-controls-front-door.md
        package-surface-expectations.md
        controls-rendering.md
        typed-lowering-parity.md
```

> Naming note: the new modules live under the existing `FS.Skia.UI.Controls.Typed`
> namespace (clean names `ToggleButton`/`SplitButton`/`DatePicker`/`TimePicker`/`ColorPicker`,
> no legacy collision because none of these exist legacy-side). Grouping into `Buttons.*`
> and `Pickers.*` files mirrors the existing per-theme grouping (`Primitives`, `Display`,
> `Input`, `Containers`, `Navigation`, `Overlay`). Exact file split is a tasks-phase detail.
```
