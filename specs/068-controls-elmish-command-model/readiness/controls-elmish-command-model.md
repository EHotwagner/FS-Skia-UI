# Controls.Elmish command model — evidence (068)

Feature-specific evidence (spec §Evidence obligations): the Widget-view path, the
`AdapterCommand`↔`Cmd<'msg>` mapping rule, the lowering-parity result, and the command
round-trip property results. The change is **additive** and confined to
`src/Controls.Elmish/`.

## 1. Widget-returning view path (US1, FR-001/FR-004)

- `ControlsElmish.widgetView : ('model -> Widget<'msg>) -> ('model -> Control<'msg>)`
  = `view >> Widget.toControl` (pure composition).
- `ControlsElmish.programOfWidget init update view subscriptions`
  = `program init update (widgetView view) subscriptions`. The adapter lowers the
  `Widget<'msg>` internally, so a product authored entirely with
  `FS.Skia.UI.Controls.Typed.*` needs **no** `Widget.toControl` shim in its own code
  (SC-001). The `AdapterProgram` record is unchanged — `View` stays `'model -> Control<'msg>`.
- **Lowering parity (SC-002):** for the same logical view authored two ways,
  `program.View model` (Widget path) and `view model |> Widget.toControl` (legacy boundary)
  render to the same node count and dispatch identically.
  - Evidence: `tests/Elmish.Tests/TypedControlsAdapterTests.fs` →
    "programOfWidget runs a Widget-returning view with no Widget.toControl in product code".

## 2. AdapterCommand ↔ Cmd<'msg> mapping rule (US2, FR-003/FR-008)

The bridge module `AdapterCmd` aligns the adapter's effect-list command model with the
Elmish standard `Cmd<'msg>` under a single documented, **total** rule:

| Function | Rule |
| --- | --- |
| `none` | `= Cmd.none` (the empty Elmish effect list). |
| `ofMessage msg` | `= [ DispatchProductMessage msg ]`. Law: `productMessages (ofMessage m) = [ m ]`. |
| `productMessages command` | ordered `List.choose` of `DispatchProductMessage` payloads; no other case contributes. |
| `toCmd route command` | one Elmish effect per `AdapterEffect`, **in list order**, each dispatching `route effect`; `[] -> Cmd.none`. **Total** over every `AdapterEffect` case (product and non-product), pure to construct, never throws. |

- **Empty edge:** `toCmd route [] = none` and dispatches nothing.
- **Order:** dispatch order = `List.map route command` order — deterministic.
- **Totality / non-product effects:** `DispatchControlRuntimeMessage`,
  `DispatchKeyboardMessage`, `DispatchHostCommand`, and `ReportAdapterDiagnostic` are
  **carried** by `route`, not silently dropped — the alignment is total over the union.

## 3. Round-trip property result (SC-003/FR-008)

- Product-message round-trip: dispatching `toCmd projectProduct command` through a recording
  dispatcher yields exactly `productMessages command` (same multiset, same order, none
  dropped or duplicated), property-tested over **≥1,000** generated commands with no
  counterexample.
- Totality/order: over a generator spanning **every** `AdapterEffect` case, dispatch order
  equals `List.map route command`, ≥1,000 cases, no counterexample.
- Evidence: `tests/Elmish.Tests/AdapterCmdTests.fs` (FsCheck, `Config…WithMaxTest 1000`).

## 4. Test result

`dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj` → **17 passed, 0 failed** (was 6
before this feature). All real evidence — no synthetic fixtures, mocks, or placeholders
(Principle V: no `[S]`).

## 5. Compatibility note (interacting requirements)

The package now offers **two** view return types. They are **peers**: the `Widget<'msg>`
path is documented as **preferred**; the legacy `Control<'msg>` path is retained as a
**frozen peer** (mirrors the 065 Q1 decision). Neither path is removed or behaviorally
changed by introducing the other (FR-010). Existing `Control`-view programs compile with no
source edit and behave identically (US3, SC-004), and the base `FS.Skia.UI.Controls`
package keeps its `Fable.Elmish`-free dependency split (FR-006, SC-005).

## 6. Governance / risk

- Risk level: **small** — additive public surface confined to one package.
- Required evidence: this file + `package-surface-expectations.md` + regenerated baselines.
- Broad validation: the serialized maintainer-verify order is run because this is a
  consumer-contract (`src/**/*.fsi`) change; `Route` over the branch diff is the
  authoritative gate selector and prints the `package-surface` set
  (`PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff`). Aggregate broad-run
  results are non-authoritative and recorded here.
