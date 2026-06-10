# Implementation Plan: Focus, Keyboard Traversal & Input Routing

**Branch**: `094-focus-keyboard-traversal` | **Date**: 2026-06-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/094-focus-keyboard-traversal/spec.md`

## Summary

E4 of the controls architecture-evolution roadmap. Generalize E1's focus-aware *text* seam
into a **full focus model for all controls**: a deterministic single **tab order** derived
purely from `AccessibilityMetadata` (`FocusOrder` ascending, `None` following in layout order,
stable tiebreak), **keyboard traversal** (Tab / Shift+Tab) that walks that order and updates the
existing `ControlRuntime.FocusedControl` via the existing `FocusControl` message, and
**focused-control key delivery** that routes a delivered key to the current `FocusedControl`'s
authored binding by matching the control's `KeyboardOperation.ActivationKeys` /
`NavigationKeys` — exactly generalizing E1's text seam to every interactive kind, with the E1
text-keystroke path preserved unchanged. Focus survives unrelated re-renders by consuming E2's
retained identity (`RetainedId`-keyed state), and the focused control is visibly indicated
through E3's `Focused` visual-state (no parallel procedural focus-paint branch).

This is **not** a redesign: key routing stays **flat per-focused-control** (consistent with the
existing flat per-`ControlId` pointer dispatch). It introduces **no** routed-event
bubbling/tunneling, command system, data binding, observable, dependency/attached property, or
lookless-template surface (permanent roadmap non-goals). Scope is **mechanism + representative
verification**: the mechanism is general, but verification is bounded to a representative set
spanning the key roles — an activation control (**`Button`**), a navigation control
(**`Slider`**, ArrowLeft/Right → value-change), and a **text** control (proving the E1 seam is
preserved). A composite control (RadioGroup/Tab/Menu/Slider) is a **single tab stop** — arrows are
delivered to its authored `NavigationKeys` binding, not an E4-owned sub-focus cursor (clarified). A
catalog-wide keyboard retrofit of all 52 typed views is explicitly out of scope.

**Technical approach**: A new pure `module Focus` (`Focus.fsi` / `Focus.fs`, inserted after
`Pointer.fs` in `Controls.fsproj`) in `FS.Skia.UI.Controls` declares the tab-order /
traversal / key-classification surface — `FocusStop`, `TabOrder`, `FocusMove`, `KeyRouting`,
and the pure totals `Focus.order` (tree → tab order), `Focus.traverse` (order + current + move →
next focus), and `Focus.route` (a focused control's `KeyboardOperation` + normalized key →
routing verdict). All three are pure, total, deterministic reductions over the lowered
`Control<'msg>` tree + `AccessibilityMetadata` + current focus + key — no live window required,
property-testable to ≥1000 generated combinations (SC-006). The **host key-routing seam** lands
in `FS.Skia.UI.Controls.Elmish` as a new internal `routeFocusedKey` (analogous to the 092
`routeFocusedText`): it resolves the focused control over the **retained** tree (E2 identity),
applies `Focus.route`, and emits the focused control's authored activation/value-change product
messages, a `FocusControl` traversal message, or a fall-through to `host.MapKey` — with the E1
`routeFocusedText` path consulted first so text delivery is unchanged. `runInteractiveApp` wires
this key path before the existing `host.MapKey` fallback.

A **central design correction** is required (see Research R1): `Accessibility.defaultFor`
currently seeds **every** focusable control's `NavigationKeys` with `["Tab"; "Shift+Tab"]`, and
`Accessibility.validate` *requires* a focusable control to carry non-empty `NavigationKeys`.
Under FR-007 (a focused control's own `NavigationKeys` consumption wins per-key), that would make
**every** control consume Tab and global traversal would never fire. E4 corrects this:
traversal keys (Tab / Shift+Tab) are **engine-level**, derived from the tab order, **not**
per-control `NavigationKeys`; `NavigationKeys` is reserved for **intra-control** arrows
(slider / radio / menu); and `validate`'s over-strict "focusable ⇒ non-empty `NavigationKeys`"
rule is relaxed so an activation-only control (a `Button`) is valid. These are behavioral fixes
to `Accessibility.fs` (signatures unchanged).

This is a **Tier 1** change: it moves public surface (a new public `Focus.fsi` on
`FS.Skia.UI.Controls`, and the internal host key-routing contract + its `.fsi` doc on
`FS.Skia.UI.Controls.Elmish`), so controls-public-surface + the Controls.Elmish package-surface
+ per-package + cross-package baselines MUST be recaptured.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: existing `FS.Skia.UI.Controls` / `FS.Skia.UI.Controls.Elmish` deps
only — `Accessibility` (metadata + `validate`), `Control` (lowered tree + computed layout order),
`ControlRuntime` (`FocusedControl` / `FocusControl`), `RetainedRender` (E2 `RetainedId` identity),
`Pointer` (`FocusMovedByPointer`), `TextInput` (E1 seam), `KeyboardInput.ViewerKey` (host key
normalization). No new package dependency.
**Testing**: Expecto + FsCheck (purity / totality / determinism of `Focus.order` / `traverse` /
`route` over ≥1000 generated combinations, SC-006), deterministic offscreen route-probe results
through the real `routeFocusedKey` adapter path (no hand-seeded identity map, SC-001/SC-002/
SC-004), the reused E1 `captureRespondsProof` input→visible-change primitive for the
responds-proof (SC-002/SC-005), the unchanged E1 text-seam evidence (SC-003), `Accessibility.validate`
over the representative view (SC-007), FAKE targets per `Route`.
**Target Platform**: Windows and Linux. Logic proofs are deterministic reducer / route-probe
results; the input→visible-change responds-proof reuses the E1 evidence primitive (no live
Vulkan window required).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A to `.template.config/template.json` content — no new capability,
  sample, command, or package-policy surface ships into the `dotnet new fs-skia-ui` template.
  The template's package **pins** are refreshed on merge per the standard version-bump flow (all
  packable libraries bumped, `FsSkiaUiVersion` pin updated), not by this plan's edits.
- **Dependency impact**: N/A — no new dependency. `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` coverage are unchanged; the focus model and key
  routing use only existing Controls / Controls.Elmish / KeyboardInput types.
- **Command-surface impact**: No new gate or build target. `build.fsx` / `Routing.fs` are
  unchanged — the change routes through the **existing** controls-public-surface escalation
  (a public `src/Controls/*.fsi` add) and the package-surface escalation (`src/Controls.Elmish/*.fsi`
  edit). FAKE-backed targets run sequentially in the deterministic escalated order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  Surface baselines recaptured via `RefreshSurfaceBaselines` and `PerPackageSurface.captureCurrent`.
  `ContrastCheck` applies **only if** the focus indicator introduces a new token-derived color
  (the indicator is expected to resolve through E3's existing `Focused` style, so no new token).
- **Generated project impact**: None. No change to default/minimal generated contents, selected
  Controls guidance, local skills, validation logs, or generated `Dev` behavior. The focus model
  is additive: a generated project whose consumer adds no keyboard interaction renders and
  behaves identically (FR-009).
- **Evidence paths**: All under `specs/094-focus-keyboard-traversal/readiness/`:
  - `us1-tab-traversal.md` — Tab / Shift+Tab advance `FocusedControl` through focusable controls
    of mixed `FocusOrder` in `FocusOrder`-then-layout order, wrap cyclically at both ends, and
    skip non-focusable controls; pure `Focus.order` + `Focus.traverse` results (US1 / SC-001).
  - `us2-focused-key-delivery.md` — a focused `Button` activates on each `ActivationKey`
    producing exactly the pointer-equivalent message once (no double-dispatch); a focused
    `Slider` changes value on its `NavigationKeys` (ArrowLeft/Right); through the real
    `routeFocusedKey` adapter path (US2 / SC-002).
  - `us2-text-seam-preserved.md` — a focused text control still receives typed/committed/composed
    text via the unchanged E1 `routeFocusedText` pipeline (US2 / SC-003).
  - `us3-focus-stability.md` — focus survives a sibling-shifting re-render via the **live**
    retained path (`RetainedRender.step`), resolving to the same control, not a hand-seeded map
    (US3 / SC-004).
  - `us3-focus-indicator.md` — the focused control renders E3's `Focused` visual-state and the
    indicator moves with focus (previously-focused control loses it), with no procedural per-kind
    focus-paint branch (US3 / SC-005).
  - `sc006-determinism-property.md` — purity / totality / determinism of `Focus.order` /
    `traverse` / `route` over ≥1000 generated combinations; an unmatched key is a defined no-op
    that falls through (never a throw) (SC-006).
  - `sc007-validate-order.md` — the computed traversal order for the representative view passes
    `Accessibility.validate`, and inspection confirms tab order + key semantics derive solely
    from `AccessibilityMetadata` (no parallel hand-rolled table); the `view` contract is
    unchanged for keyboard-free consumers (SC-007).
  - `responds-proof.md` — the reused E1 `captureRespondsProof` input→visible-change proof for a
    key-driven focus change (an inert host yields identical frames + `Inert`).
  - `fsi-transcript.md` — FSI exercise of the public `Focus` surface through the packed library
    (Principle I).
  - `surface-baselines.md` — recaptured controls-public-surface / Controls.Elmish package-surface
    / per-package / cross-package baseline diffs.
- **`.fsi` / contract impact**: **Tier 1, surface moves.** New public `src/Controls/Focus.fsi`
  (`FocusStop`, `TabOrder`, `FocusMove`, `KeyRouting`; `Focus.order` / `Focus.traverse` /
  `Focus.route`). `src/Controls.Elmish/ControlsElmish.fsi` gains the internal `routeFocusedKey`
  contract (and `runInteractiveApp`'s `.fsi` doc is updated to honestly describe key routing —
  echoing the E1 lesson that the contract doc must match the code). `Accessibility.fsi`
  signatures are **unchanged** — the `defaultFor` / `validate` corrections (R1) are behavioral.
  The representative typed `Props` (`Widgets/Buttons.fsi`, `Widgets/Input.fsi`) change **only if**
  they need to expose `KeyboardOperation` they do not already carry via `Accessibility.defaultFor`
  (expected: no `.fsi` delta — defaults already supply the metadata; corrected per R1).
  controls-public-surface + Controls.Elmish package-surface + per-package + cross-package
  baselines recaptured. Compatibility: purely additive to consumers — the
  `view : 'model -> Control<'msg>` contract is unchanged; a consumer adding no keyboard
  interaction sees no behavior change.
- **MVU/effect boundary**: Satisfied through the **existing** `ControlRuntime` MVU boundary —
  `Model` = `ControlRuntimeModel` (`FocusedControl`), `Msg` = `ControlRuntimeMsg.FocusControl`,
  `update` = the existing pure `ControlRuntime.update`. E4 adds **pure reducers** (`Focus.order`,
  `Focus.traverse`, `Focus.route` — functions of tree + metadata + current focus + key, no I/O,
  no new `Effect`/`Cmd`/subscription model) and the **interpreter-edge** key routing at the host
  (`routeFocusedKey` in `Controls.Elmish`, wired by `runInteractiveApp`). Traversal produces
  `FocusControl` messages the existing `ControlRuntime.update` consumes; the engine **reads** —
  never duplicates — `FocusedControl`. A removed focused control reuses E2's stale-target recovery
  (`RecoverStaleTarget` / `StaleTarget`). No new effect/command/interpreter *model* is introduced
  beyond key-delivery routing.
- **Synthetic evidence**: None planned. Traversal / routing proofs are authoritative deterministic
  reducer + route-probe results from the **real** `Focus` functions and the **real** `routeFocusedKey`
  adapter path. SC-004 (focus stability) is proven through the **live** `RetainedRender.step` path,
  **not** a hand-seeded `StateByIdentity` map (the 092 gap this explicitly avoids repeating).
  SC-003 reuses the unchanged E1 text-seam evidence; the responds-proof reuses the real E1
  `captureRespondsProof` primitive. No mocks/stubs/fakes are anticipated; if any `[S]` appears it
  triggers the full Principle V disclosure regime.
- **Test evidence**: Failing-first semantic tests in `Controls.Tests`: (1) a tab-order test
  asserting `Focus.order` yields the `FocusOrder`-then-layout order and excludes non-focusable
  controls — fails before derivation matches; (2) a traversal test asserting Tab / Shift+Tab
  advance/reverse with cyclic wrap; (3) an FsCheck property asserting purity/totality/determinism
  of `order`/`traverse`/`route` over ≥1000 inputs and that an unmatched key never throws; (4) a
  key-routing test asserting a focused `Button` activates once (pointer-equivalent, no
  double-dispatch) and a focused `Slider` navigates (ArrowLeft/Right), through the real adapter path;
  (5) an E1-text-seam regression test asserting unchanged text delivery; (6) a focus-stability
  test over the live retained path (sibling shift); (7) a `validate`-passes-order test plus the
  R1 correction test (a focusable activation-only `Button` is valid; Tab is not consumed by a
  default control). Governance: surface-baseline tests (controls-public-surface / Controls.Elmish
  package-surface / per-package / cross-package).
- **Observability**: The `Focus` reducers are pure and total — `order`/`traverse`/`route` cover
  the closed `FocusMove` / `KeyRouting` sets with no partial match or exception path; an unmatched
  key resolves deterministically to `Fallthrough` (a defined no-op, documented, surfaced via the
  host fallback `host.MapKey`, never silent). A focused control removed between frames is handled
  by E2 stale-target recovery and surfaces the existing `StaleTarget` effect / diagnostic. Diff
  diagnostics (`KeyCollision`) continue to surface through the host diagnostics channel. No new
  structured-log surface; the existing `ControlDiagnostic` / `PointerDiagnostic` channels remain
  authoritative.
- **Deferred scope**: Out of scope and bounded as follow-ups — a catalog-wide keyboard retrofit of
  all 52 controls' authored binding surfaces (representative roles only here); a full text-editor /
  IME UX, selection gestures, undo/redo (text domain begun in E1); routed-event bubbling/tunneling,
  a command system, accelerator/mnemonic/global-hotkey tables; lookless template / slot composition
  (E5, demand-driven); data binding / observables / dependency properties (permanent non-goal); a
  screen-reader / AT-bridge integration (metadata stays the contract — no platform automation peer).

**Initial Constitution Check: PASS** — Tier 1 with `.fsi` adds + baseline recapture planned; the
stateful focus workflow rides the existing `ControlRuntime` MVU boundary with pure new reducers
and an interpreter-edge host routing seam (Principle IV); no synthetic evidence (Principle V);
idiomatic simplicity respected (the reducers are plain folds / list walks over records and closed
unions — no SRTP, reflection, type providers, custom operators, non-trivial CEs, or multi-case
active patterns; Principle III).

## Project Structure

```
src/Controls/
  Types.fsi / Types.fs             # unchanged (AccessibilityMetadata / KeyboardOperation already present)
  Accessibility.fsi / .fs          # .fs ONLY: R1 corrections to defaultFor (Tab out of NavigationKeys,
                                    #   intra-control arrows in) + validate (relax focusable⇒NavigationKeys);
                                    #   .fsi signatures unchanged
  Control.fsi / Control.fs         # source of the lowered tree + computed layout order Focus.order walks
  ControlRuntime.fsi / .fs         # unchanged (FocusedControl / FocusControl reused)
  RetainedRender.fsi / .fs         # unchanged (E2 RetainedId identity consumed by the host seam)
  Pointer.fsi / Pointer.fs         # unchanged (FocusMovedByPointer reused for pointer focus)
  Focus.fsi / Focus.fs             # NEW — FocusStop, TabOrder, FocusMove, KeyRouting + pure
                                    #   order / traverse / route (Tier 1, public)
  Widgets/Buttons.fsi / .fs        # representative activation control (Button) — metadata only if needed
  Widgets/Input.fsi / .fs          # representative navigation control (Slider, ArrowLeft/Right) — metadata only if needed
  Widgets/TextBoxWidget.fsi / .fs  # representative text control (E1 seam preserved)

src/Controls/Controls.fsproj       # insert Focus.fsi / Focus.fs after Pointer.fs (line 76)

src/Controls.Elmish/
  ControlsElmish.fsi / .fs         # + internal routeFocusedKey; runInteractiveApp wires the key path;
                                    #   .fsi host-contract doc updated to describe key routing (package surface)

test/Controls.Tests/               # tab-order, traversal, routing, property (FsCheck), E1-regression,
                                    #   stability (live retained), validate-order + R1-correction tests

specs/094-focus-keyboard-traversal/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/focus-model.md         # the pure tab-order / traversal / key-classification contract
  contracts/key-routing-surface.md # the host key-routing seam contract
  checklists/requirements.md
  readiness/                       # evidence artifacts (paths above)
```

**Insertion point**: `Focus.fsi` / `Focus.fs` go after `Pointer.fs` (line 76 of `Controls.fsproj`)
so the module is in scope for the host adapter and depends only on already-compiled `Accessibility`,
`Control`, `ControlRuntime`, and `RetainedRender`. `Focus` references `ControlId` and the
`AccessibilityMetadata` / `KeyboardOperation` types only — it does **not** depend on `RetainedRender`'s
internal structure (the `ControlId`↔`RetainedId` binding lives at the host seam), keeping the pure
reducers free of the internal retained types.

## Complexity Tracking

No constitution deviations requiring justification. The focus reducers are plain list walks and a
match over a closed key-routing union; no Principle III escape (SRTP, reflection, type providers,
custom operators, non-trivial CEs, multi-case active patterns) is used. The one non-obvious change
— the R1 `defaultFor`/`validate` correction — is a behavioral fix disclosed in Research and covered
by a dedicated failing-first test, not a hidden complexity.
