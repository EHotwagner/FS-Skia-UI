# Implementation Plan: Runtime Visual-State Bridge (R1)

**Branch**: `096-runtime-visual-state-bridge` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/096-runtime-visual-state-bridge/spec.md`

## Summary

R1 builds the one missing wire between the interaction state the framework already
tracks and the style resolver that already consumes it. E3 (093) gave us a pure
`Style.resolve` that reads a control's `VisualState`; E2 (091+092) gave every
control a stable retained identity so its `visualState` attribute survives a
re-render. Nothing converts the live `ControlRuntime` interaction state
(focus/hover/press/selection) into that attribute — so hover/press/focus styling
exists only as a manual consumer opt-in.

This feature adds:

1. **`ControlRuntime.deriveVisualState : ControlRuntimeModel -> ControlId -> VisualState`**
   — a pure, total, deterministic projection from interaction state to a single
   `VisualState` under a fixed closed precedence. **New Tier-1 public surface.**
2. **`applyRuntimeVisualState : ControlRuntimeModel -> Control<'msg> -> Control<'msg>`**
   — an **internal** host bridge that walks the lowered `Control<'msg>` tree and
   stamps each control's derived state, **preserving a consumer-set non-`Normal`
   attribute** and **emitting nothing at `Normal`** (byte-identity at rest).
3. A `renderRetained` call site in `ControlsElmish.fs` that builds a
   `ControlRuntimeModel` from the host's live `pointerState` (hover/press, already
   `ControlId`-keyed) + `focused` (`RetainedId`, resolved back to `ControlId` via
   the prior retained tree) and applies the bridge to the freshly-produced tree
   **before** `RetainedRender.step`, in the **`ControlId` domain** (pre-reconcile).
   On the first frame there is no prior retained tree, so `focused` resolves to `None`
   (research §D5) and focus indication begins only once focus is established by
   post-render interaction.
4. **Widened migrated geometry**: `slider`, `text-box`, `radio-group`, `switch`
   join `button`/`check-box` in routing their paint through `Style.resolve` with
   the threaded `state`, so focus/restyle is visible on a representative focusable
   surface. Byte-identical to today at `Normal`.

Because the stamp lands pre-diff, a hover/focus change becomes an `Update` patch on
exactly that subtree (composes with E2 partial repaint); because `Normal`+unset
stamps nothing, the un-bridged build stays `Scene`-byte-identical and E2/E3 fast
paths are untouched. No data binding, observable graph, dependency property,
selector engine, or lookless template is introduced — this is wiring, not
architecture.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Existing `FS.Skia.UI.Controls`
(types, `Style.resolve`, `ControlInternals.visualStateOf`, `ControlRuntimeModel`,
`PointerState`) and `FS.Skia.UI.Controls.Elmish` (`runInteractiveApp` retained
host, `RetainedRender`).
**Testing**: Expecto (`Controls.Tests`, `Elmish.Tests`), FsCheck property tests for
totality/determinism/precedence, FSI transcript for the public projection, real
in-repo readiness artifacts under `specs/096-runtime-visual-state-bridge/readiness/`.
**Target Platform**: Windows and Linux (`net10.0`).

**Key grounding facts (verified in source):**

- `ControlRuntimeModel` (the actual type name; spec prose abbreviates to
  `ControlRuntime`) holds `FocusedControl: ControlId option`,
  `HoveredControl: ControlId option`, `PressedControls: Set<ControlId>`,
  `Selection: ControlSelection option` (a **text-range** selection carrying
  `ControlId`) — `src/Controls/ControlRuntime.fs:35`–`44`.
- `ControlId = string`, `ControlKind = string`; the established identity scheme is
  `control.Key |> Option.defaultValue control.Kind` (used by
  `retainedIdOfControl`, `ControlsElmish.fs:581`). The bridge reuses it verbatim.
- `Control<'msg>` has a **structural** `Children: Control<'msg> list` field
  (`Types.fsi:289`) — the canonical child channel the reconciler and geometry walk;
  the bridge recurses it.
- `ControlInternals.visualStateOf : Attr<'msg> list -> VisualState` reads the
  last `visualState` attribute (absent ≡ `Normal`); it is `module internal` and
  exposed in `Control.fsi:61`. `ControlRuntime.fs` compiles **after** `Control.fs`
  (`Controls.fsproj:55` then `:65`), so the bridge reuses it directly.
- `Style.applyState` already maps every `VisualState` case to a delta
  (`Style.fs:71`); `Style.resolve` is unchanged.
- The host's `renderRetained` (`ControlsElmish.fs:555`) already owns `pointerState`
  (`PointerState.Hover`/`.Presses`, `ControlId`-keyed) and `focused` (`RetainedId`).
- Migrated geometry today: `button`/`icon-button` (`buttonGeom`) and `check-box`
  (`checkboxGeom`) take `classes`/`state` and call `Style.resolve`
  (`Control.fs:707,600`). `sliderGeom`/`textFieldGeom`/`radioGeom`/`switchGeom` do
  not yet — widening adds the `classes`/`state` params and the `Style.resolve` call,
  byte-identical at `Normal`.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Tier**: **Tier 1 (contracted change)** — adds one public function
(`ControlRuntime.deriveVisualState`) to `FS.Skia.UI.Controls`. Requires `.fsi`
update + surface-baseline recapture. The `applyRuntimeVisualState` bridge and the
widened geometry are internal/behavioral (no public type added).

**Principle compliance:**

- **I (Spec→FSI→Tests→Impl)**: The public `deriveVisualState` signature is drafted
  in `ControlRuntime.fsi`, exercised via an FSI transcript, and covered by semantic
  tests before the `.fs` body. ✅
- **II (Visibility in `.fsi`)**: `deriveVisualState` appears in `ControlRuntime.fsi`;
  `applyRuntimeVisualState` is **omitted** from the `.fsi` (automatically internal)
  and reached by tests via `InternalsVisibleTo` (`Controls.Tests`, `Elmish.Tests`
  already declared, `Controls.fsproj:19,29`). No access modifiers in `.fs`. ✅
- **III (Idiomatic simplicity)**: A `match`-based total precedence and a structural
  recursion over `Control.Children`. No SRTP/reflection/custom operators/type
  providers. The precedence reads top-down as a plain ordered `if`/`match`. ✅
- **IV (MVU boundary)**: The bridge **reads** `ControlRuntimeModel` (already an MVU
  model owned by the pointer/focus reducers) and the host applies it at the
  **interpreter edge** (`renderRetained` closure). `deriveVisualState` and
  `applyRuntimeVisualState` are **pure**; no new `Msg`/`Effect`/`update` and no
  mutation of runtime state. ✅
- **V (Synthetic disclosure)**: None planned. The live responds-proof drives the
  real retained host; precedence is property-tested over generated (not canned)
  combinations; byte-identity is structural `Scene` equality. No `[S]` expected.
- **VI (Test evidence)**: Failing-first tests — precedence/totality property suite,
  byte-identity-at-rest `Scene` equality, focus-survives-reshuffle via real
  identity, widened-kind restyle parity. ✅
- **VII (Observability)**: No new failure path; the bridge is total (every
  `ControlId` → a `VisualState`) and emits no diagnostics. Existing host diagnostic
  surfacing is untouched. ✅

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` or
  command-surface change. The `dotnet new` template only refreshes its package
  **pins** on merge via the standard version-bump flow (separate track); no template
  content/sample/skill change. The two shipping consumer skills already teach
  E1–E5; R1 needs no new skill (the behavior is automatic on the built-in host).
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are unaffected (no dependency
  added or version-floated).
- **Command-surface impact**: No new gate, target, or wrapper. A public
  `src/Controls/ControlRuntime.fsi` signature change escalates to the
  **controls-public-surface** rule, so `Route` prints the serialized
  `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck →
  EvidenceGraph → EvidenceAudit` path **plus `ContrastCheck`**. FAKE-backed targets
  run sequentially in that deterministic order (shared `.fake` state). Surface
  baselines are recaptured (`RefreshSurfaceBaselines` for controls-public-surface +
  cross-package; `PerPackageSurface.captureCurrent` for the per-package snapshot).
  Run `./fake.sh build -t Route` first and run exactly the gates it prints.
- **Generated project impact**: None to default/minimal generated contents or
  generated `Dev` behavior. The bridge is internal to the built-in retained host,
  so a generated project consuming `runInteractiveApp` gains live restyle/focus
  **automatically** with no scaffold change, no new selected-Controls guidance, and
  no placeholder/excluded-history scan delta.
- **Evidence paths**: All under `specs/096-runtime-visual-state-bridge/readiness/`:
  - `derive-precedence.md` — property-test transcript: totality + determinism over
    ≥1000 generated `(ControlRuntimeModel, ControlId, consumer-state)` combos; fixed
    order holds; consumer non-`Normal` preserved 100% (SC-004, US3).
  - `live-restyle.md` — US1: a migrated control hovered/pressed/selected in a
    `ControlRuntimeModel` resolves to the matching style with a **no-attribute**
    consumer `view`; a non-interacted sibling resolves `Normal` (SC-001).
  - `focus-survives-reshuffle.md` — US2: a focused control shows its `Focused`
    indicator and the indicator stays on the same control across a sibling-shifting
    re-render **via real retained identity**, not a hand-seeded map (SC-002).
  - `byte-identity-at-rest.md` — FR-005/SC-003: a `Normal`+unset control emits no
    `visualState` attribute and renders `Scene`-byte-identical to the un-bridged
    build; `RecomputedNodeCount` unchanged at rest.
  - `partial-repaint.md` — SC-005: a single hover entering one control surfaces a
    single reconciler `Update` patch; repaint is O(hovered-subtree) via the existing
    `WorkReduction` metric.
  - `widened-kinds.md` — SC-006: each of `button`/`check-box`/`slider`/`text-box`/
    `radio-group`/`switch` restyles + focus-indicates; unmigrated kinds (incl.
    `toggle-button`/`list-box`/`multi-select-list`/`combo-box`) show no render delta.
  - `responds-proof.md` — input → visible restyle on the live retained path that an
    inert/un-bridged build fails (distinct from a render-only screenshot).
  - `contrast.md` — SC-007: `ContrastCheck` still passes; no second contrast policy,
    no new token literal.
  - Recaptured surface baselines (controls-public-surface, per-package,
    cross-package) committed as the public-contract evidence.
- **`.fsi` / contract impact**: `src/Controls/ControlRuntime.fsi` gains **one** new
  `val deriveVisualState`. `applyRuntimeVisualState` is **not** added to any `.fsi`
  (internal). `Control.fsi`, `Style.fsi`, and `Types.fsi` are unchanged (the
  widened geometry reuses the existing `VisualState`-threaded private render path;
  no new public control type, no new `VisualState` case). Compatibility: purely
  additive — no existing signature changes; migration guidance = "none required; new
  projection is opt-in for direct callers, automatic on the built-in host".
- **MVU/effect boundary**: `Model` = the existing `ControlRuntimeModel` (read-only
  here). `Msg`/`Effect`/`init`/`update` = **unchanged** (the pointer/focus reducers
  already own state mutation). New code is **pure**: `deriveVisualState`
  (model→state) and `applyRuntimeVisualState` (model+tree→tree). Interpreter edge =
  the `renderRetained` closure assembles the read-only `ControlRuntimeModel` from
  `pointerState`+`focused` and applies the bridge. No new effect/command/subscription.
- **Synthetic evidence**: None planned. Real retained host drives the responds-proof;
  property tests generate (not hardcode) combinations; parity is structural `Scene`
  equality. If any real-evidence path proves infeasible at implementation time, the
  task is marked `[S]` with full Principle-V disclosure — not expected.
- **Test evidence**: Failing-first: (a) precedence/totality/determinism FsCheck
  suite; (b) consumer-state-preserved + derived-fills-`Normal` unit cases; (c)
  byte-identity-at-rest `Scene` equality vs un-bridged build; (d) focus-survives a
  sibling-shifting re-render through real identity; (e) per-widened-kind restyle
  parity + unmigrated-kind no-delta; (f) FSI transcript exercising the public
  `deriveVisualState`. Governance: recaptured surface baselines; `ContrastCheck`.
- **Observability**: No new diagnostics or log paths; the bridge is total and
  silent by design (a state change is observable as the resolved style / `Update`
  patch). Missing-artifact failure classes are the existing readiness-gate ones.
- **Deferred scope**: Out — incremental measure/partial re-layout (R2),
  binding-aware unkeyed dispatch (R3), the live animation clock + animated
  transitions (R4; R1 only enables the trigger), general navigation-key delivery
  (R5), a catalog-wide migration of all 52 controls off `Normal`-only geometry, any
  new `VisualState` case, and the permanent non-goals (CSS selectors,
  attached/dependency properties, lookless templates, data binding).

**Post-design re-check**: No new violation introduced by Phase 1. The design adds
one pure public function + one internal pure bridge + a host call site + four
geometry widenings; all principle checks above still hold. ✅

## Project Structure

```
specs/096-runtime-visual-state-bridge/
├── spec.md
├── plan.md                      # this file
├── research.md                  # Phase 0 — decisions & rationale
├── data-model.md                # Phase 1 — entities & precedence model
├── quickstart.md                # Phase 1 — how to exercise the bridge
├── contracts/
│   └── control-runtime-bridge.md  # public deriveVisualState + internal bridge contract
├── checklists/
│   └── requirements.md          # (existing) spec quality checklist
└── readiness/                   # evidence artifacts (created during implement)

src/Controls/                    # FS.Skia.UI.Controls (Tier-1 surface lands here)
├── ControlRuntime.fsi           # + val deriveVisualState   (PUBLIC, new)
├── ControlRuntime.fs            # + deriveVisualState + applyRuntimeVisualState (internal)
└── Control.fs                   # widen sliderGeom/textFieldGeom/radioGeom/switchGeom
                                 #   to thread classes+state through Style.resolve

src/Controls.Elmish/            # FS.Skia.UI.Controls.Elmish
└── ControlsElmish.fs            # renderRetained: build ControlRuntimeModel from
                                 #   pointerState+focused, apply bridge pre-step

tests/Controls.Tests/           # precedence/totality property suite, byte-identity,
                                 #   widened-kind parity, FSI transcript
tests/Elmish.Tests/             # focus-survives-reshuffle, responds-proof, partial-repaint
```

## Phase 2 (next): `/speckit-tasks`

Phase 2 produces `tasks.md` + `tasks.deps.yml` (story-grouped, `skillist`-tagged,
acyclic). This plan stops after Phase 1 design.
