# Feature Specification: Focus, Keyboard Traversal & Input Routing

**Feature Branch**: `094-focus-keyboard-traversal`
**Created**: 2026-06-10
**Status**: Draft
**Input**: User description: "create the next part of docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md" — the controls architecture-evolution roadmap defines an E1→E5 evolution. E1 (feature 090, live event dispatch + the focus-aware text seam) and E2 (features 091 + 092, the wired keyed reconciler + retained identity) have landed; E3 (feature 093, the visual-state / style layer) is specified. The next rung is **E4 — the focus / keyboard-traversal / input-routing system** that generalizes E1's focus-aware *text* seam into a full focus model for *all* controls.

## Context & Motivation *(informative)*

The controls subsystem already *tracks* focus but does not yet *deliver keyboard to it generally* or *traverse it*. The pieces in place today:

- `ControlRuntimeModel.FocusedControl: ControlId option` (`ControlRuntime.fsi:42`) durably tracks which control is focused, mutated by `ControlRuntimeMsg.FocusControl` (`ControlRuntime.fsi:54`) and surfaced as the `FocusChanged` effect.
- `Pointer` already emits `FocusMovedByPointer of control` (`Pointer.fsi:83`) so a pointer click sets focus.
- `AccessibilityMetadata` (`Types.fsi:172`) carries the metadata a focus engine needs but is **not yet driven by one**: `FocusOrder: int option`, `Role: AccessibilityRole`, and `Keyboard: KeyboardOperation` (`Types.fsi:159` — `{ Focusable; ActivationKeys; NavigationKeys }`).
- `VisualState.Focused` (`Types.fsi:192`) exists and, after E3 (feature 093), resolves through the single state→style resolver.
- E1 (feature 090) wired the **focus-aware *text* seam**: keystrokes/committed/composed text reach the focused *text* control through the `ControlRuntime.FocusedControl` + `TextInput` pipeline. E2 (features 091/092) gave every control a **stable cross-frame retained identity** (`RetainedRender` / `RetainedId`-keyed focus + text + clock state).

What is missing is the engine that ties these together for **all** controls:

1. **No keyboard traversal.** Nothing moves `FocusedControl` via Tab / Shift+Tab / arrow keys; focus only moves on a pointer click. There is no tab order.
2. **No focused-control key delivery beyond text.** A focused *button* does not activate on Space/Enter; a focused *slider* does not move on arrows. `KeyboardOperation.ActivationKeys` / `NavigationKeys` are declared in metadata but never consulted to route a key to the focused control.
3. **Focus is not yet driven from accessibility metadata.** `FocusOrder` and the role/keyboard taxonomy exist but no engine derives a deterministic traversal order from them, and `Accessibility.validate` is not yet exercised against a live focus order.

E4 closes exactly these gaps: a **deterministic focus model over the E2 retained tree** with a single tab order derived from `AccessibilityMetadata`, **keyboard traversal** that updates `FocusedControl`, and **focused-control key delivery for all interactive kinds** — generalizing E1's text seam. Focus visuals tie into E3's `Focused` visual-state. This is incremental MVU-core evolution toward declarative-retained (SwiftUI/Compose-class) capability parity per the maintainer architecture decision recorded 2026-06-10. It is **not** a redesign: key routing stays **flat per-focused-control** (consistent with the existing flat per-`ControlId` event model), introducing **no** routed-event bubbling/tunneling, command system, data binding, or dependency-property surface.

## Clarifications

### Session 2026-06-10

- Q: How should the tab order treat a composite control (RadioGroup/Tab/Menu) — one focus stop or one stop per child? → A: A single focus stop; the composite's `NavigationKeys` (arrows) fire its authored value-change/selection binding and the consumer's `update` changes the selection — E4 owns no intra-group sub-focus cursor (consistent with flat per-control routing).
- Q: Which navigation-key control is THE representative verified for `NavigationKeys` delivery (FR-010/SC-002)? → A: `Slider` (ArrowLeft/Right → value-change), the clean non-composite navigation proof.
- Q: When the focused control is removed between frames, where should focus land? → A: The next control in tab order at the removed control's former position (or `None` if the order is empty), reusing E2 stale-target recovery.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A keyboard user traverses controls in a predictable order (Priority: P1)

A user pressing Tab (and Shift+Tab) moves focus forward (and backward) through the interactive controls of a running view in a deterministic, accessible order — without touching the pointer. The focused control is visibly indicated.

**Why this priority**: Keyboard traversal is the headline E4 capability and the precondition for keyboard-only and assistive-technology use. Without a tab order, a keyboard user cannot reach most controls at all.

**Independent Test**: Build a view with several focusable controls carrying mixed `FocusOrder` (some explicit, some `None`); apply a sequence of Tab presses and assert `FocusedControl` advances through the controls in the expected (FocusOrder-then-layout) order, wraps at the end, and reverses under Shift+Tab — using the pure traversal reducer, no live window required.

**Acceptance Scenarios**:

1. **Given** a view with focusable controls of mixed `FocusOrder`, **When** Tab is pressed repeatedly from no-focus, **Then** focus advances through them in ascending `FocusOrder`, with `FocusOrder = None` controls following in layout order, and wraps cyclically to the first after the last.
2. **Given** focus on some control, **When** Shift+Tab is pressed, **Then** focus moves to the previous control in the same order (wrapping to the last from the first).
3. **Given** a view containing non-focusable controls (e.g. static text whose `KeyboardOperation.Focusable` is false), **When** Tab traverses, **Then** those controls are skipped and never receive focus.

---

### User Story 2 - A focused control responds to its activation and navigation keys (Priority: P1)

A user who has focused a control operates it by keyboard: Space/Enter activates a focused button, arrow keys move a focused slider or change a focused radio selection, and a focused text control still receives typed text (the E1 behavior, preserved). The key goes to the focused control's authored binding, generalizing E1's text seam to every interactive kind.

**Why this priority**: Traversal without activation is half a focus system. Delivering keys to the focused control by its declared `ActivationKeys`/`NavigationKeys` is what makes keyboard operation actually *do* something, and is the direct generalization the roadmap names for E4.

**Independent Test**: Focus a button and deliver a key in its `KeyboardOperation.ActivationKeys`; assert its authored activation binding (the same one a pointer click dispatches) fires exactly once. Repeat for a control with `NavigationKeys` (e.g. a slider) asserting its value-change binding fires. Confirm a focused text control still receives typed characters through the E1 pipeline unchanged.

**Acceptance Scenarios**:

1. **Given** a focused button, **When** a key in its `ActivationKeys` (e.g. Space/Enter) is delivered, **Then** the control's authored activation binding dispatches once — the same message a pointer click would produce — and a key outside its activation/navigation set is a no-op for that control.
2. **Given** a focused control with `NavigationKeys` (e.g. a slider/radio group), **When** a navigation key (arrow) is delivered, **Then** the control's value-change/selection binding dispatches deterministically.
3. **Given** a focused text control, **When** a printable key is delivered, **Then** it reaches the `TextInput` pipeline exactly as in E1 (text key delivery is unchanged, not regressed by the generalized routing).

---

### User Story 3 - Focus survives unrelated re-renders and is driven by accessibility metadata (Priority: P2)

A maintainer (and an end user) sees focus stay put when an unrelated part of the model changes: a background update that shifts sibling controls does not reset which control is focused, and the focused control keeps its visible indicator. The traversal order and key semantics come from the existing accessibility metadata, validated, not from hand-rolled per-control tables.

**Why this priority**: Focus stability across re-renders is the concrete fix for the ControlsShowcase2 "shortcuts blocked after clicks" / focus-reset-on-rebuild symptom, and metadata-driven correctness is what keeps the system accessible. It depends on E2's identity and E3's `Focused` style, so it sequences after the two P1 capabilities.

**Independent Test**: Focus a control, then apply an unrelated model update that reorders its siblings (the 092 sibling-shift case); assert `FocusedControl` still resolves to the same control via E2's retained identity and that its `Focused` visual-state is still applied. Separately, run `Accessibility.validate` over a constructed traversal order and assert it reports no focus-order defects.

**Acceptance Scenarios**:

1. **Given** a focused control, **When** an unrelated model update shifts sibling controls, **Then** focus remains on the same control via its E2 retained identity (not reset to none and not jumped to a positional neighbor).
2. **Given** the focused control, **When** it renders, **Then** its appearance reflects E3's `Focused` visual-state and a focus indicator is visible; when focus moves away, the indicator is removed from the previously-focused control.
3. **Given** a view's controls and their `AccessibilityMetadata`, **When** the traversal order is computed, **Then** it is derived from `FocusOrder` + role/layout and passes `Accessibility.validate`, with no separate per-control focus table introduced.

---

### Edge Cases

- **No focusable controls** in the view: Tab is a no-op; `FocusedControl` stays `None`; key delivery has no target and falls through to the host's key fallback (consistent with E1's binding-wins / `MapPointer`-fallback precedence).
- **Focused control removed between frames**: when a control that held focus is gone after a re-render, focus resolves to the **next control in tab order at the removed control's former position** (or `None` if the order is now empty) rather than throwing or pointing at a stale id — reusing E2's stale-target recovery.
- **A focused control claims the traversal key itself** (e.g. a multi-line text area that inserts a Tab character): the focused control's own `ActivationKeys`/`NavigationKeys`/text-consumption wins for that key; only a Tab/Shift+Tab the focused control does **not** consume drives global traversal — so traversal never steals a key the focused control legitimately uses.
- **Pointer focus then keyboard traversal**: a pointer click sets focus to the hit focusable control (or its nearest focusable keyed ancestor, reusing E1's keyed-ancestor recovery); subsequent Tab continues from that control's position in the order.
- **Clicking a non-focusable region**: leaves the current focus unchanged (does not silently clear it), so a keyboard user does not lose their place on an incidental click.
- **Arrow keys inside a composite role** (RadioGroup/Tab/Menu/Slider): the composite is a **single tab stop**; its `NavigationKeys` (arrows) are delivered to the composite's authored value-change/selection binding, which the consumer's `update` interprets to move *intra-group* — E4 owns no sub-focus cursor. Tab still moves *between* groups — the two do not conflict because arrows are claimed by the focused group and Tab is not.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST compute a **deterministic single tab order** over the controls of the current view, derived from `AccessibilityMetadata`: focusable controls (`KeyboardOperation.Focusable = true`) ordered by `FocusOrder` ascending, with `FocusOrder = None` controls following in layout/document order, and a stable deterministic tiebreak. Non-focusable controls MUST NOT appear in the order. A **composite control** (RadioGroup/Tab/Menu/Slider) is a **single focus stop** — the tab order does not emit one stop per child; intra-group movement is delivered to the composite's authored `NavigationKeys` binding (FR-003), not handled by an E4-owned sub-focus cursor.
- **FR-002**: **Keyboard traversal** MUST move focus through the tab order: Tab advances to the next focusable control, Shift+Tab to the previous, and traversal MUST wrap cyclically at both ends. Traversal MUST update `ControlRuntime.FocusedControl` (via the existing `FocusControl` message) and MUST be a **pure, total, deterministic** reduction of (tree/order + current focus + key) → next focus.
- **FR-003**: A delivered key MUST be routed to the current `FocusedControl` and matched against that control's `KeyboardOperation`: a key in `ActivationKeys` MUST trigger the control's authored **activation** binding (the same message a pointer activation dispatches), and a key in `NavigationKeys` MUST trigger its authored **value-change/selection** binding. This generalizes E1's text seam to all interactive kinds; **focused text controls MUST keep their E1 keystroke/composition delivery unchanged**.
- **FR-004**: Focus MUST be **stable across unrelated re-renders** via E2's retained identity: `FocusedControl` MUST track a control's stable retained identity so a sibling-shifting or otherwise-unrelated model update does not reset or misdirect focus. E4 MUST NOT re-derive or alter the reconciler identity scheme established by features 067/091/092; it consumes that identity.
- **FR-005**: The focused control MUST render its **E3 `Focused` visual-state** and show a visible focus indicator; when focus moves, the indicator MUST be removed from the previously-focused control. E4 MUST drive the `Focused` state through E3's resolver and MUST NOT introduce a second/parallel styling path for focus visuals.
- **FR-006**: **Pointer and keyboard focus MUST compose.** A pointer click MUST set focus to the hit focusable control or its nearest focusable keyed ancestor (reusing E1's keyed-ancestor recovery and `Pointer.FocusMovedByPointer`); subsequent keyboard traversal MUST continue from that control's position. A click on a non-focusable region MUST leave the current focus unchanged.
- **FR-007**: Traversal and key routing MUST be **deterministic and total with a defined fallback**: a key with no focused control, or no matching `ActivationKeys`/`NavigationKeys`/text consumption on the focused control, MUST be a no-op for the control and fall through to the host's key fallback (binding-wins, host-fallback-second — the same precedence shape E1 established for pointer). A focused control that legitimately consumes a Tab/arrow key MUST win that key over global traversal; only unconsumed Tab/Shift+Tab drive traversal.
- **FR-008**: Tab order and key semantics MUST be **derived from the existing `AccessibilityMetadata`** (`Role`, `FocusOrder`, `KeyboardOperation`) and MUST be validatable by `Accessibility.validate`. E4 MUST NOT introduce a separate, hand-rolled per-control focus/keyboard table parallel to the accessibility metadata.
- **FR-009**: The focus/traversal/key-routing system MUST be **additive** to the MVU consumer surface: the `view : 'model -> Control<'msg>` contract is unchanged, a consumer who provides no keyboard interaction sees no behavior change, and **no** routed-event bubbling/tunneling, command system, data-binding/observable, dependency/attached-property, or lookless-template capability is introduced (permanent roadmap non-goals). Key routing stays **flat per-focused-control**, consistent with the existing flat per-`ControlId` event model.
- **FR-010**: E4 delivers the **focus-model mechanism + representative verification**, not a catalog-wide retrofit. The mechanism (tab-order derivation, traversal, focused-control key delivery) MUST be general, but verification is bounded to a **representative set spanning the key roles**: an activation-key control (**`Button`**), a navigation-key control (**`Slider`**, ArrowLeft/Right → value-change), and a **text** control (proving E1's seam is preserved under the generalized routing). A keyboard retrofit of all 52 typed views' authored binding surfaces is explicitly **out of scope** and is a separate fitness pass.

> Interacting / conflicting requirements: FR-002 (Tab drives global traversal) vs FR-003/FR-007 (a focused control may consume Tab/arrow keys itself) — resolution: the focused control's own key consumption wins per-key; only a key the focused control does **not** consume reaches global traversal, so a multi-line text area's Tab and a radio group's arrows are never stolen by traversal.
> FR-005 (focus visual via E3 `Focused`) vs the E3 precedence that *visual state wins over class* (feature 093, FR-003) — resolution: this is consistent by design; the focus indicator is always visible because `Focused` overrides any consumer class, exactly as E3 specifies, so E4 adds no new precedence rule.
> FR-006 (click sets focus) vs FR-004 (focus stable across re-renders) — resolution: an explicit pointer/keyboard focus change is intentional and moves focus; only *unrelated* model updates must preserve it. Intentional focus moves are never suppressed by stability.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section is expected to name concrete packages, `.fsi` signatures, build targets, effects, and evidence paths — that is its purpose.

- **Package impact**: No package identity or set changes. Behavioral + surface changes land in the existing **`FS.Skia.UI.Controls`** package (the pure focus model / tab-order derivation + traversal reducer over `AccessibilityMetadata`, sited near `ControlRuntime`/`Pointer`/`Accessibility`) and the **`FS.Skia.UI.Controls.Elmish`** package (the interactive host's key routing — the `InteractiveViewerHost.MapKey -> 'msg list` seam from feature 092 and `SkiaViewer` key delivery). The migrated representative controls' typed `Props` front doors under `src/Controls/Widgets/*.fs` gain (or simply expose) their `KeyboardOperation` activation/navigation surface. All packable libraries are version-bumped and the template pins refreshed on merge per the standard flow.
- **Public contract impact**: This feature **moves public surface (Tier 1)**. New public signatures appear on `FS.Skia.UI.Controls` (the focus-model / traversal entry points and any focus-order/key-routing types) and on `FS.Skia.UI.Controls.Elmish` (the host key-routing contract), so `src/Controls/*.fsi` and `src/Controls.Elmish/*.fsi` change and **controls-public-surface + the Controls.Elmish package-surface + per-package + cross-package surface baselines MUST be recaptured** (`RefreshSurfaceBaselines` / `PerPackageSurface.captureCurrent`). The `.fsi` host-contract doc MUST honestly describe key routing (echoing the E1 lesson that the contract doc must match the code).
- **State workflow impact**: Extends the existing `ControlRuntime` focus handling — traversal produces `FocusControl` messages and the focus engine reads (does not duplicate) `FocusedControl`; key routing is added at the host (`Controls.Elmish`) seam. No new effect/command/subscription/interpreter *model* is introduced beyond key-delivery routing; the traversal/routing reducers are pure functions of the tree + metadata + current focus + key. Reuses E2's stale-target recovery for a removed focused control.
- **Layout/rendering impact**: The only render change is the **focus indicator** for the focused control, resolved through E3's `Focused` visual-state (feature 093) — no new Skia/Vulkan surface and no procedural focus-paint branch. Tab order consumes the existing computed layout order as a tiebreak (`Layout`/`Bounds`), it does not change layout. A **responds-vs-renders proof** for key-driven focus change reuses the E1 responds-evidence primitive (input→visible-change), and offscreen route probes plus the pure traversal/routing reducers cover the deterministic logic without requiring a live window.
- **Evidence obligations**: Real, in-repo readiness artifacts under `specs/094-focus-keyboard-traversal/readiness/` proving: (a) Tab/Shift+Tab traverse focusable controls in the `FocusOrder`-then-layout order, wrapping, skipping non-focusable controls (US1); (b) a focused control responds to its `ActivationKeys`/`NavigationKeys` (button activation, slider/radio navigation) and a focused text control still receives keystrokes via the E1 pipeline (US2); (c) focus survives an unrelated sibling-shifting re-render via E2 retained identity and the focused control shows the E3 `Focused` style (US3); (d) the traversal order passes `Accessibility.validate`. Logic proofs are authoritative as deterministic reducer/route-probe results; the input→visible-change responds-proof reuses the E1 evidence primitive.
- **Unsupported scope**: Out of scope — a full text editor / IME UX, selection gestures, undo/redo (that remains the text domain begun in E1); routed-event **bubbling/tunneling**, a command system, accelerator/mnemonic/global-hotkey tables; a catalog-wide keyboard retrofit of all 52 controls (representative roles only here); lookless template / slot composition (E5, demand-driven); data binding / observables / dependency properties (permanent non-goal); a screen-reader/AT-bridge integration (metadata stays the contract, no platform automation peer is built here).
- **Build-target impact**: Run `Route` first and run only the gates it prints. Public `src/Controls/*.fsi` and `src/Controls.Elmish/*.fsi` signature changes escalate to the **controls-public-surface** / **package-surface** rules, so the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit` path applies. `ContrastCheck` applies if the focus indicator introduces a new token-derived color. No new gate is added; surface baselines are recaptured rather than hand-edited.

## Success Criteria *(mandatory)*

- **SC-001**: From a running view, a keyboard-only user reaches every focusable control via Tab/Shift+Tab in a deterministic order that matches the `FocusOrder`-then-layout specification and wraps at both ends, while every non-focusable control is skipped — verified for 100% of the focusable controls in the representative view.
- **SC-002**: A focused control responds to its declared keys: a focused button activates on each of its `ActivationKeys` producing exactly the pointer-equivalent message (once, no double-dispatch), and a focused `Slider` changes value on its `NavigationKeys` (ArrowLeft/Right) — verified for the representative activation and navigation controls.
- **SC-003**: A focused text control receives typed/committed/composed text through the E1 pipeline with **zero regression** under the generalized key routing — the E1 text-seam evidence still passes unchanged.
- **SC-004**: Focus is stable across an unrelated re-render: a sibling-shifting model update (the 092 case) leaves `FocusedControl` resolving to the same control via E2 retained identity, demonstrated through the live retained path rather than a hand-seeded focus map.
- **SC-005**: The focused control is visibly indicated through E3's `Focused` visual-state, and the indicator moves with focus (the previously-focused control loses it) — with no procedural per-kind focus-paint branch.
- **SC-006**: Traversal and key routing are pure and deterministic: identical (order/tree, current focus, key) inputs produce identical (next focus, dispatched binding) across at least 1000 generated input combinations, and an unmatched key is a defined no-op that falls through to the host fallback (never a throw).
- **SC-007**: The computed traversal order passes `Accessibility.validate` for the representative view, and inspection confirms tab order and key semantics derive solely from `AccessibilityMetadata` (no parallel hand-rolled focus/keyboard table) — and the `view` consumer contract is unchanged for consumers who add no keyboard interaction.

## Assumptions

- E1 (feature 090) has landed: the focus-aware text seam delivers keystrokes to a focused text control through `ControlRuntime.FocusedControl` + the `TextInput` pipeline, and the keyed-ancestor recovery + binding-wins/host-fallback precedence exist. E4 generalizes this seam; it does not re-establish it.
- E2 (features 091 + 092) has landed: the keyed reconciler is on the live render path and confers the stable cross-frame retained identity (`RetainedId`-keyed focus/text/clock state) that E4's focus stability attaches to. E4 consumes that identity and does not re-implement it.
- E3 (feature 093) provides the single state→style resolver and the `Focused` visual-state path that E4's focus indicator renders through. If E3 has not yet landed when E4 is implemented, the focus indicator is resolved through whatever path renders the `Focused` state at that time, still without a parallel procedural branch.
- The existing `AccessibilityMetadata` (`Role`, `FocusOrder`, `KeyboardOperation` with `Focusable`/`ActivationKeys`/`NavigationKeys`) is the complete and authoritative source for focusability, tab order, and key semantics; E4 derives traversal from it and adds no new accessibility primitives.
- The event model stays **flat per-control** (no bubbling/tunneling); key routing targets the single `FocusedControl`, mirroring the flat per-`ControlId` pointer dispatch from E1. This is a deliberate non-goal boundary, not an omission.
- Per the architecture-evolution decision (2026-06-10), this is incremental MVU-core evolution toward declarative-retained parity, not a redesign; no data-binding, command, or property-system surface is introduced.
- Like E1 and E3, E4 is scoped to **mechanism + representative verification**; a catalog-wide keyboard retrofit of all 52 controls is a separate follow-up fitness pass.

## Key Entities

- **Focus model / tab order**: the deterministic ordering of the view's focusable controls, derived purely from `AccessibilityMetadata` (`FocusOrder` + layout/role) over the E2 retained tree; the substrate traversal walks.
- **Keyboard traversal**: the pure reduction (order + current focus + Tab/Shift+Tab/arrow) → next `FocusedControl`, wrapping at the ends and skipping non-focusable controls; emits the existing `FocusControl` message.
- **Focused-control key routing**: the host-seam mapping of a delivered key to the current `FocusedControl`'s authored binding, matched against its `KeyboardOperation.ActivationKeys` / `NavigationKeys`, with text controls preserving the E1 keystroke path; falls through to the host key fallback when unmatched.
- **`KeyboardOperation`**: the existing per-control accessibility record (`Focusable`, `ActivationKeys`, `NavigationKeys`) that is the sole source of a control's focusability and key semantics.
- **Focus indicator**: the visible focus cue rendered for the `FocusedControl` through E3's `Focused` visual-state — not a separate procedural paint path.
- **Retained identity (E2)**: the `RetainedId`-keyed stable identity that lets `FocusedControl` survive an unrelated re-render; consumed by E4, owned by features 067/091/092.
