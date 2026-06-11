# Feature Specification: Runtime Visual-State Bridge

**Feature Branch**: `096-runtime-visual-state-bridge`
**Created**: 2026-06-10
**Status**: Draft
**Input**: User description: "create the next part of `docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`" — the controls architecture-evolution roadmap. Steps E1–E5 (features 090, 091+092, 093, 094, 095) have landed; the roadmap's §10 post-implementation audit then defines five live-path remediation features **R1–R5** with the recommended order **R1 → {R3, R2} → R4 → R5**. This feature is **R1 — the runtime visual-state bridge** (roadmap §10.3), the highest-value remediation because it closes two roadmap exit criteria at once (E3 "states render distinctly", E4 "focus is visibly indicated") and is the prerequisite for R4's animated transitions.

## Context & Motivation *(informative)*

E3 (feature 093) delivered a single pure `Style.resolve` that consumes a control's `VisualState`, and E2 (features 091 + 092) gave every control a stable retained identity so the reconciler **carries** a control's `Attr.visualState` attribute across frames. But the framework has no code that converts the **live interaction state it already tracks** into that attribute.

`ControlRuntime` holds the interaction state — `FocusedControl`, `HoveredControl`, `PressedControls`, `Selection` (`src/Controls/ControlRuntime.fs:36`–`40`; `.fsi:42`–`46`) — updated by the pointer/focus reducers. The render path faithfully *consumes* a `VisualState` attribute when one is present (`Style.resolve`, `src/Controls/Style.fs:83`). **The conversion between the two does not exist.** The only producer of `Attr.visualState` in product source is the attribute's own constructor (`src/Controls/Attributes.fs:72`); its only callers are tests. The E4 focus responds-proof makes the gap explicit: the *consumer's* view mirrors focus by hand — `if focusedKey = "btn" then [ Attr.visualState Focused ] else []` (`tests/Elmish.Tests/Feature094FocusRoutingTests.fs:257`–`258`).

The consequence is that the roadmap's headline live-window promises — "hover/press/focus/selected/disabled render distinctly" (E3) and "focus is visibly indicated" (E4) — are delivered today as *capabilities a consumer can opt into by hand*, not as *working live behavior*. A running app does not restyle on hover or show a focus ring unless the consumer manually threads `VisualState` attributes from interaction state they would have to reconstruct themselves.

R1 builds the missing bridge once: it derives a per-control `VisualState` from `ControlRuntime` and injects it into the `Control<'msg>` tree **before** the reconciler diffs, so the bridge composes with E2's partial repaint for free (a hover change becomes an `Update` patch on exactly that subtree) and with E3's resolver (which already consumes the attribute). It is architecture-preserving and non-goal-preserving: it introduces no data binding, no observable property graph, no dependency properties, no selector engine — it is wiring, not architecture.

## Clarifications

### Session 2026-06-11

- Q: Which concrete selection-category kinds are in the widened migrated set this feature
  (FR-006/SC-006)? → A: **`RadioGroup` and `Switch`** (the two single-focusable Input selection
  kinds). The widened set is exactly **`Button`, `CheckBox`, `Slider`, `TextBox`, `RadioGroup`,
  `Switch`**. The collection selection controls (`ToggleButton`, `ListBox`, `MultiSelectList`,
  `ComboBox`) are **out of scope** here — their item-level / virtualized selection geometry is
  better matched to R5 navigation.
- Q: Is the host bridge entry (`applyRuntimeVisualState`) public API or internal? → A: The pure
  projection **`ControlRuntime.deriveVisualState` is public** (testable, consumer-reusable); the
  **`applyRuntimeVisualState` host bridge is internal** (invoked by `renderRetained`, tested via
  `InternalsVisibleTo`). This keeps the new Tier-1 public surface to the single projection;
  consumers on the built-in host get the behavior automatically without calling the bridge.
- Q: What is the authoritative source the bridge reads as the consumer's pre-set semantic state
  for the FR-003 "fill only the `Normal` slot" rule? → A: The control's **pre-existing
  `Attr.visualState` attribute** on the lowered `Control<'msg>`. Typed semantic Props
  (Disabled/Selected/…) already lower to that attribute, so there is **one channel**: a present
  non-`Normal` attribute is the consumer's intent and is preserved; an absent/`Normal` attribute
  is the slot the derived interaction state fills.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A running control restyles on interaction with zero consumer code (Priority: P1)

A consumer authoring a plain `view : 'model -> Control<'msg>` runs their app and **does nothing special**. When the user hovers, presses, or selects a migrated control, the control visibly changes appearance — hover lightens, press darkens, selected/disabled render distinctly — because the host derives that visual state from its own interaction tracking and feeds it to the style resolver. The consumer writes no `Attr.visualState`, reconstructs no focus/hover bookkeeping, and reads nothing from `ControlRuntime`.

**Why this priority**: This is the headline R1 capability and the direct fix for the audit finding that E3's resolver is "present but not live-driven". Without it, declarative state-styling exists only as a manual opt-in, which is the gap R1 closes.

**Independent Test**: Drive the live host (or its pure render-step equivalent) with a `ControlRuntime` whose `HoveredControl`/`PressedControls`/`Selection` name a migrated control, render through `renderRetained`, and assert the resolved style for that control matches its `Hover`/`Pressed`/`Selected` resolution — with a consumer `view` that attaches **no** visual-state attribute. Assert a sibling that is not interacted resolves to its `Normal` style.

**Acceptance Scenarios**:

1. **Given** a migrated control and a `ControlRuntime` whose `HoveredControl` is that control, **When** the host renders, **Then** the control resolves to its `Hover` style with no consumer-authored attribute.
2. **Given** the same control pressed (in `PressedControls`), **When** the host renders, **Then** it resolves to `Pressed`, overriding the hover look per the closed precedence.
3. **Given** a control that is neither hovered, pressed, focused, nor selected, **When** the host renders, **Then** it resolves to `Normal` and emits no visual-state attribute (byte-identical to the un-bridged build).

---

### User Story 2 - A focused control shows a focus indicator that survives unrelated re-renders (Priority: P1)

A user tabs to (or clicks) an interactive control and sees a **focus indicator** appear automatically. When an unrelated part of the model changes and the tree re-renders — even shifting the focused control's sibling position — the focus indicator stays on the same control, because the bridge stamps focus before the reconciler diffs and E2's identity carries it across the frame.

**Why this priority**: This closes E4's unmet "focus is visibly indicated" exit criterion and validates that the bridge composes with E2 retained identity (the focus-survives-re-render guarantee). It is equal-priority with US1 because focus indication and interaction restyle are the two roadmap exit criteria R1 exists to satisfy.

**Independent Test**: With a focused migrated control, render two frames whose only difference is an unrelated sibling-shifting model change; assert the focus indicator resolves on the same control in both frames (via retained identity, not a hand-seeded map), and that the consumer `view` authored no focus attribute.

**Acceptance Scenarios**:

1. **Given** a `ControlRuntime` whose `FocusedControl` is a migrated focusable control, **When** the host renders, **Then** that control resolves with its `Focused` indicator and no consumer-authored focus attribute.
2. **Given** the focused control, **When** an unrelated model update shifts its sibling order and the tree re-renders, **Then** the focus indicator remains on the same control across the re-render via E2 identity.
3. **Given** focus moves to a different control, **When** the host renders, **Then** the previously-focused control returns to its prior (non-focused) resolution and the newly-focused one gains the indicator.

---

### User Story 3 - Consumer-set semantic state composes with derived interaction state by a closed order (Priority: P2)

A consumer who *does* set a semantic state from their `'model` — `Disabled`, `Validation`, `Loading`, or `Selected` — keeps that state; the bridge never silently erases it. Derived interaction states (hover/press/focus) only fill the slot the consumer left at `Normal`. A disabled control stays disabled-looking even while hovered; a consumer-marked `Selected` item stays selected. The arbitration is one fixed, closed, ordered model — not ad-hoc per-kind logic.

**Why this priority**: It makes the bridge predictable and safe to adopt — a consumer's authored intent is never overwritten by an incidental pointer hover. It is lower urgency than the two live-behavior stories but is what makes the precedence trustworthy and is what the determinism exit criterion measures.

**Independent Test**: For a control the consumer marked `Disabled`, drive a `ControlRuntime` that also reports it hovered/pressed/focused; assert it resolves `Disabled` (consumer state wins over derived). For a control the consumer left at `Normal`, assert the derived interaction state fills it. Property-test the full precedence for totality and determinism over generated combinations.

**Acceptance Scenarios**:

1. **Given** a control the consumer set to `Disabled` and a runtime reporting it hovered, **When** the bridge derives state, **Then** the result is `Disabled` (consumer semantic state out-ranks derived interaction state).
2. **Given** a control the consumer left at `Normal` and a runtime reporting it focused, **When** the bridge derives state, **Then** the result is `Focused` (derived state fills the `Normal` slot).
3. **Given** a runtime reporting one control simultaneously pressed and (consumer-)selected, **When** the bridge derives state, **Then** exactly one state is chosen by the fixed closed order, deterministically, with no per-kind branching.

---

### Edge Cases

- **Control absent from runtime state**: a control named by neither `Focused`/`Hovered`/`Pressed`/`Selection` resolves to `Normal` and emits **no** visual-state attribute — preserving the un-bridged byte-identical output and E2's `Keep → reuse` / E3's `[] → control` fast paths.
- **Non-migrated kind interacted**: a control whose kind does not read `VisualState` in its geometry (outside the migrated set) is **not** restyled by the bridge — interaction state derives but produces no visible change, exactly as today in **rendered output**, with no error. The bridge is **kind-agnostic**: it may stamp an inert `visualState` attribute on a non-migrated *interacted* node, which the geometry ignores. This does not affect the at-rest byte-identity guarantee, which is defined for non-interacted `Normal`-and-unset controls (FR-005). (Alternative considered: gate the stamp on migrated-kind membership; rejected to keep the bridge a total, kind-agnostic tree walk with a single source of truth for the migrated set — the geometry dispatch.)
- **Multiple interaction flags on one control** (e.g. hovered *and* pressed *and* focused): the closed precedence picks a single winning state deterministically; there is no blended or ambiguous result.
- **Consumer semantic state already non-`Normal`**: derived interaction state does **not** override it; the consumer's authored `Disabled`/`Validation`/`Loading`/`Selected` is preserved (the derived state only fills a `Normal` slot).
- **Stamp domain mismatch**: the bridge keys controls in the **`ControlId` domain** (pre-reconcile, the domain `ControlRuntime` already uses); it never operates in the `RetainedId` domain, so a control's runtime state and its stamped attribute always refer to the same node.
- **Unkeyed same-kind siblings**: the bridge keys a control by `Key |> Option.defaultValue Kind` — the same scheme `ControlRuntime` uses. Two same-kind siblings with **no** `Key` collapse to one `ControlId`, so interacting with one derives the state for **both**. Per-instance restyle therefore requires the consumer to assign distinct `Key`s to same-kind siblings; closing this for unkeyed dispatch is **R3** scope, not R1.
- **First-frame focus**: on the first rendered frame there is no prior retained tree to resolve the host's `focused` `RetainedId` back to a `ControlId`, so `focused` resolves to `None` and no `Focused` indicator is derived until focus is established by post-render interaction (research §D5).
- **Hover churn**: a per-frame hover change touches only the hovered subtree's attribute, so the reconciler scopes the repaint to that subtree (O(hovered-subtree)), not the whole tree.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide a **pure, total, deterministic projection** from live interaction state to a single visual state — `ControlRuntime.deriveVisualState : ControlRuntime -> ControlId -> VisualState` — selecting a control's state from `FocusedControl`/`HoveredControl`/`PressedControls`/`Selection`. Identical inputs MUST always yield an identical result, and the function MUST be defined for every `ControlId` (a control named by no interaction state yields `Normal`).
- **FR-002**: Derivation precedence MUST be a **fixed, closed, ordered model** (highest wins): `Disabled` > `Validation` > `Loading` > `Pressed` > `Selected` > `Focused` > `Hover` > `Normal`. There MUST be no per-kind branching, no open/extensible precedence surface, and no ambiguity when multiple interaction flags apply to one control. The **runtime-derivable tail** (`Pressed` > `Selected` > `Focused` > `Hover` > `Normal`) is what `deriveVisualState` itself emits; the higher `Disabled`/`Validation`/`Loading` ranks are never produced from runtime state and are realized only by the consumer-preservation rule (FR-003), which out-ranks any derived state. The full order is thus the composition of the derived tail with consumer preservation, not a single function that can emit all eight cases.
- **FR-003**: A consumer-supplied semantic state (one of `Disabled`/`Validation`/`Loading`/`Selected`, set from the consumer's `'model` on the `Control<'msg>`) MUST be **preserved and out-rank** any derived interaction state: derived interaction states (`Pressed`/`Focused`/`Hover` — and runtime-`Selected`) MUST only fill the slot a consumer left at `Normal`. The bridge MUST NEVER silently erase a consumer's authored non-`Normal` state. The **single authoritative source** of consumer intent is the control's **pre-existing `Attr.visualState` attribute** on the lowered `Control<'msg>` (typed semantic Props lower to that attribute): a present non-`Normal` attribute is preserved; an absent or `Normal` attribute is the slot the derived state fills. The bridge MUST NOT read a second/parallel channel for consumer state.
- **FR-004**: The system MUST provide a **host bridge** — `applyRuntimeVisualState : ControlRuntime -> Control<'msg> -> Control<'msg>` — that stamps each control's derived `VisualState` onto the `Control<'msg>` tree, applied in `renderRetained` (`src/Controls.Elmish/ControlsElmish.fs:555`) **before** `RetainedRender.step` and in the **`ControlId` domain** (pre-reconcile). The bridge is an **internal** host detail (invoked automatically by the built-in retained host, tested via `InternalsVisibleTo`); it is not public surface. Because the stamp lands before the diff, a state change MUST surface as an attribute change the reconciler turns into an `Update` patch on exactly that control's subtree, composing with E2's partial repaint.
- **FR-005**: The bridge MUST emit **no attribute** when a control's derived state is `Normal` and the consumer set none, so non-interacted controls stay **byte-identical** to the un-bridged build and E3's `[] → control` and E2's `Keep → reuse` fast paths are untouched. (Identity-at-rest is preserved.)
- **FR-006**: The bridge MUST restyle a control only if its kind reads `VisualState` in its geometry (the migrated set). This feature MUST **widen the E3-migrated set** (today `Button`, `CheckBox`) to the **focusable interactive kinds that most need a focus indicator** — exactly **`Button`, `CheckBox`, `Slider`, `TextBox`, `RadioGroup`, and `Switch`** — so focus indication and interaction restyle are visible on a representative interactive surface rather than two kinds. The collection selection controls (`ToggleButton`, `ListBox`, `MultiSelectList`, `ComboBox`) and a catalog-wide migration of all 52 controls remain **out of scope** (tracked with E3 / R5).
- **FR-007**: A bridged control's resolved style MUST attach to its **E2 stable retained identity**, so a focus/hover/selected look is consistent across an unrelated re-render (the focus indicator survives a sibling-shifting model update). R1 MUST NOT re-derive or alter the reconciler identity scheme established by features 067/091/092; it consumes that identity.
- **FR-008**: The bridge MUST remain **contrast-validated by the existing gate** and MUST NOT introduce a second contrast policy. It introduces no new token literals; any styling it surfaces flows through E3's `Style.resolve` over DTCG-sourced tokens, so `DesignTokenDrift` and the contrast gate stay authoritative.
- **FR-009**: The feature MUST be **additive** to the MVU consumer surface: a consumer who interacts with nothing sees no behavior change, the `view : 'model -> Control<'msg>` contract is unchanged, and no data-binding, observable, dependency/attached-property, lookless-template, or CSS-selector capability is introduced (permanent roadmap non-goals). The **only** new public surface is the `ControlRuntime.deriveVisualState` projection; the `applyRuntimeVisualState` bridge stays internal (FR-004).

> Interacting / conflicting requirements: FR-002 (closed precedence order, where `Pressed` out-ranks
> `Selected`) vs FR-003 (consumer-set semantic state is preserved and out-ranks *derived* state) —
> resolution: the precedence order arbitrates among states **of the same origin**; a consumer-set
> non-`Normal` state always wins over a *derived* interaction state regardless of where the two sit in
> the order, because derived states only fill a `Normal` slot. Concretely, a consumer-`Selected`
> control that the runtime also reports `Pressed` resolves `Selected` (consumer state preserved), not
> `Pressed` — FR-003 governs the consumer-vs-derived contest, FR-002 governs the derived-vs-derived
> contest. FR-004 (stamp before reconcile to drive partial repaint) vs FR-005 (emit nothing at
> `Normal` to preserve byte-identity) — resolution: the bridge stamps an attribute **only** for
> non-`Normal` derived/consumer state; the `Normal`-and-unset case is a no-op, so partial repaint and
> identity-at-rest are both satisfied without conflict.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** Unlike the rest of the
> spec, this section is *expected* to name concrete packages, `.fsi` signatures, build targets,
> effects, and evidence paths — that is its purpose.

- **Package impact**: No package identity or set changes. Behavioral + surface changes land in the existing **`FS.Skia.UI.Controls`** package (the `deriveVisualState` projection and `applyRuntimeVisualState` bridge in `src/Controls/ControlRuntime.fs[i]`, the widened-kind geometry in `src/Controls/Control.fs`) and the **`FS.Skia.UI.Controls.Elmish`** package (the `renderRetained` bridge call in `src/Controls.Elmish/ControlsElmish.fs`). No new token is required, so the DTCG source and `DesignTokens` are untouched. All packable libraries are version-bumped and template pins refreshed on merge per the standard flow.
- **Public contract impact**: This feature **moves public surface (Tier 1)**. A **single** new public projection — `ControlRuntime.deriveVisualState` — appears on `FS.Skia.UI.Controls`, so `src/Controls/ControlRuntime.fsi` changes and **controls-public-surface + per-package + cross-package surface baselines MUST be recaptured** (`RefreshSurfaceBaselines` / `PerPackageSurface.captureCurrent`). The `applyRuntimeVisualState` bridge is **internal** (exposed to tests via `InternalsVisibleTo`, not on the public baseline). The widened-kind geometry change reuses the existing `VisualState`-threaded render path (no new control public type). `Style.fs` is unchanged (it already consumes `VisualState`).
- **State workflow impact**: None to the consumer state model. The bridge **reads** `ControlRuntime` interaction state that the pointer/focus reducers already own; it does not mutate, own, or add to that state, and introduces no new effect/command/subscription/interpreter behavior. The derivation is a pure function of the existing runtime model.
- **Layout/rendering impact**: Rendering output MUST be byte-identical to the un-bridged build for every control whose derived state is `Normal` and unset (FR-005). For an interacted migrated control, the produced scene changes only on that control's subtree, surfaced as a reconciler `Update` patch (composes with E2 partial repaint). No new Skia/Vulkan surface. The live focus-indication / restyle path additionally yields a **responds-proof** runtime artifact (input → visible restyle) distinct from a render-only screenshot; deterministic render-only `Scene`-equality evidence covers the pure derivation and the byte-identity-at-rest claims.
- **Evidence obligations**: Real, in-repo readiness artifacts under `specs/096-runtime-visual-state-bridge/readiness/` proving: (a) a migrated control hovered/pressed/selected in `ControlRuntime` resolves to the matching state's style with a no-attribute consumer `view` (US1); (b) a focused migrated control shows its `Focused` indicator and that indicator survives a sibling-shifting unrelated re-render via E2 identity, not a hand-seeded map (US2); (c) the closed precedence and consumer-vs-derived rule hold, property-tested for totality/determinism over generated combinations (US3); (d) a `Normal`-and-unset control emits no attribute and stays `Scene`-byte-identical to the un-bridged build (FR-005); (e) a responds-proof captures input → visible restyle that an inert/unbridged build fails; (f) the contrast gate still governs. Parity proofs are authoritative as structural `Scene` / resolved-style equality (the SceneEvidence render functions are deterministic capability-hash functions, not pixel encoders).
- **Unsupported scope**: Out of scope — incremental measure / partial re-layout (R2), binding-aware unkeyed dispatch (R3), the live animation clock and animated state transitions (R4 — R1 only enables the trigger), general navigation-key delivery (R5); a catalog-wide migration of all 52 controls off the `Normal`-only geometry; any new visual state beyond the existing `VisualState` enum; CSS selectors, attached/dependency properties, lookless templates, data binding (permanent non-goals).
- **Build-target impact**: Run `Route` first and run only the gates it prints. A public `src/Controls/*.fsi` signature change escalates to the **controls-public-surface** rule, so the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit` path applies, plus `ContrastCheck`. No new gate is added; surface baselines are recaptured.

## Success Criteria *(mandatory)*

- **SC-001**: On the live host, hovering, pressing, or selecting a migrated control **visibly changes its rendering with zero consumer code** (no `Attr.visualState` authored, nothing read from `ControlRuntime`) — verified for hover, press, and selected against each migrated interactive kind.
- **SC-002**: A focused migrated control shows its `Focused` indicator automatically, and that indicator **survives an unrelated sibling-shifting re-render** via E2 retained identity (demonstrated through the live-path identity, not a hand-seeded attribute map).
- **SC-003**: A control whose derived state is `Normal` and which carries no consumer-set state emits **no** visual-state attribute and renders **`Scene`-byte-identical** to the un-bridged build, preserving E2's `Keep → reuse` and E3's `[] → control` fast paths (`RecomputedNodeCount` unchanged at rest).
- **SC-004**: The derivation precedence is a **tested closed model**: `deriveVisualState` is pure, total, and deterministic across at least 1000 generated `(ControlRuntime, ControlId, consumer-state)` combinations, the fixed order (`Disabled > Validation > Loading > Pressed > Selected > Focused > Hover > Normal`) holds for every combination, and a consumer-set non-`Normal` state is preserved over any derived interaction state in 100% of cases.
- **SC-005**: A localized interaction (e.g. a hover entering one control) touches only that control's subtree — the bridged attribute change surfaces as a single reconciler `Update` patch and the repaint is O(hovered-subtree), measured via the existing `WorkReduction` metric, not a whole-tree repaint.
- **SC-006**: The migrated interactive set is widened to a **representative focusable surface** — exactly `Button`, `CheckBox`, `Slider`, `TextBox`, `RadioGroup`, and `Switch` — each of which restyles on interaction and shows a focus indicator on the live host; unmigrated kinds (including `ToggleButton`/`ListBox`/`MultiSelectList`/`ComboBox`) remain unchanged (no render-output delta).
- **SC-007**: The contrast gate remains the single contrast authority — no migrated control's bridged styling regresses its contrast result and the bridge adds no second contrast policy or new token literal.
- **SC-008**: The `view : 'model -> Control<'msg>` consumer contract is unchanged and the feature is additive: a consumer who interacts with nothing observes no behavior change, and no data-binding/observable/dependency-property/selector/template surface is introduced.

## Assumptions

- E2 (features 091 + 092) and E3 (feature 093) have landed: the keyed reconciler is on the live render path (`RetainedRender.step`), confers a stable cross-frame `ControlId`/`RetainedId` identity, and carries the `Attr.visualState` attribute across frames; `Style.resolve` already consumes `VisualState`. R1 consumes these and re-implements none of them.
- `ControlRuntime` is the authoritative, already-populated source of live interaction state (`FocusedControl`/`HoveredControl`/`PressedControls`/`Selection`), updated by the existing pointer/focus reducers; R1 only reads it.
- The existing `VisualState` enum (Normal/Hover/Pressed/Focused/Selected/Disabled/Loading/Validation) is the complete set the bridge derives into; this feature adds no new state.
- The E3-migrated geometry (controls that read `VisualState` in their render) is `Button` + `CheckBox` today; widening to the focusable interactive set reuses that same `VisualState`-threaded render path rather than introducing a new styling mechanism.
- Per the architecture-evolution decision, this is incremental MVU-core evolution toward declarative-retained parity (the R-series finishing the E-series live path), not a redesign; no data-binding or property-system surface is introduced.
- R1 is independent of R2–R5 (roadmap §10.8): it neither depends on nor blocks incremental layout (R2), binding-aware recovery (R3), or general navigation (R5), and it is the prerequisite *trigger* for R4's animated transitions (which is a separate feature).

## Key Entities

- **Interaction state**: the live `ControlRuntime` fields `FocusedControl`, `HoveredControl`, `PressedControls`, `Selection` — owned and updated by the pointer/focus reducers, read (not mutated) by the bridge.
- **Derived visual state**: the single `VisualState` the projection selects for a control from its interaction state, under the fixed closed precedence; `Normal` when no interaction (and no consumer state) applies.
- **Consumer semantic state**: a non-`Normal` `VisualState` a consumer sets from their `'model` (`Disabled`/`Validation`/`Loading`/`Selected`) — preserved and out-ranking derived interaction state.
- **Runtime visual-state bridge**: the host step (`applyRuntimeVisualState`) that stamps the derived state onto the `Control<'msg>` tree in the `ControlId` domain before the reconciler diffs, so the change becomes a scoped `Update` patch.
- **Migrated interactive set**: the focusable kinds whose geometry reads `VisualState` and therefore restyle/focus-indicate live — widened here to exactly `Button`, `CheckBox`, `Slider`, `TextBox`, `RadioGroup`, and `Switch`.
- **Resolved style**: the per-control output of E3's `Style.resolve`, now driven automatically by the bridge-supplied state rather than a consumer-authored attribute.
