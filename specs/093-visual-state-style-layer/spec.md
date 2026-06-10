# Feature Specification: Declarative Visual-State & Style-Class Layer

**Feature Branch**: `093-visual-state-style-layer`
**Created**: 2026-06-10
**Status**: Planned
**Input**: User description: "next part in controls roadmap in report in docs/reports from today" — the controls architecture-evolution roadmap (`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`) defines an E1→E5 evolution. E1 (feature 090) and E2 (features 091 + 092, wire the keyed reconciler + retained identity) have landed; the next rung is **E3 — the visual-state / style layer over design tokens**.

## Context & Motivation *(informative)*

The controls subsystem styles controls **procedurally, per kind**: each control kind computes
its own paint/typography inline at render time, branching on `VisualState`
(Normal/Hover/Pressed/Focused/Selected/Disabled/Loading/Validation, `Types.fsi:187`) against the
`Theme` primitives (`Types.fsi:198`) and the generated `DesignTokens` (`DesignTokens.fsi`). There
is **no styling layer**: no consumer-attachable style classes or variants, and no single
state→style resolver. A control's appearance is hard-coded in its kind's render branch.

The E3 step makes styling **declarative and state-driven** without introducing a CSS-selector
engine or any of the permanently-rejected XAML pillars (data binding, dependency properties,
lookless `ControlTemplate`s, selector matching). It introduces two consumer-facing concepts —
**style classes / variants** a consumer attaches to a control (e.g. "primary", "danger",
"ghost") — and one internal concept — a **single, ordered state→style resolver** that takes
(tokens + theme + attached classes + current `VisualState`) and produces the control's resolved
visual properties. The procedural per-kind styling is replaced by this one resolver for the
migrated controls.

E3 depends on E2's retained identity: a control's `VisualState` is only meaningful across frames
because the reconciler now gives that control a stable identity (so a hover/press/focus state —
and, with E2's per-control animation clock, an *animated* transition between states — attaches to
"the same control" rather than being rebuilt and lost every frame). E3 builds the styling on top
of that identity; it does not re-derive it.

This feature is incremental MVU-core evolution toward declarative-retained (SwiftUI/Compose-class)
capability parity, per the maintainer architecture decision recorded 2026-06-10. It is **not** a
redesign and introduces no data-binding/observable surface.

## Clarifications

### Session 2026-06-10

- Q: What is the resolution model for style classes — arbitrary selectors, or a closed/ordered
  scheme? → A: A **closed, deterministically-ordered** scheme. A control carries an ordered list
  of attached style classes; resolution is a pure fold over (theme/token base → each class in
  attach order → current visual state), last-writer-wins per property. No selector matching, no
  specificity algebra, no cascade across unrelated controls — the model is closed and ordered so
  it can never grow into a CSS engine.
- Q: When a consumer-attached class and the control's visual state both set the same visual
  property, which wins? → A: **Visual state wins over class**, and within each, later-attached
  wins over earlier. The fixed precedence order is: token/theme base < attached classes
  (in attach order) < current visual state. This keeps interactive feedback (hover/press/focus)
  always visible regardless of which classes a consumer attached.
- Q: Must every one of the 52 catalog controls be migrated off procedural styling in this
  feature? → A: No. E3 delivers the **resolver mechanism + the declarative class/state surface +
  a representative migration** (a small set spanning the rich and box+label families) proving the
  resolver produces byte-identical output to the prior procedural path for the migrated controls.
  A catalog-wide migration of all 52 is a separate follow-up pass, explicitly out of scope here.
- Q: Are style classes a typed surface or free-form strings? → A: A **typed, closed variant set
  plus an open user-class escape hatch**: the built-in semantic variants (primary/danger/ghost/…)
  are a typed union so the common path is compiler-checked, and an additional free-form class is
  accepted for consumer-defined token-derived styles. Both flow through the same ordered resolver.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A consumer styles a control by intent, not by hand-painting (Priority: P1)

A consumer authoring a `view : 'model -> Control<'msg>` attaches a semantic style class to a
control — for example marks a button as the "primary" action or a "danger" action — and the
control renders with the token-derived paint/typography for that intent, without the consumer
computing any colors or branching on theme.

**Why this priority**: This is the headline E3 capability — declarative styling by intent. Without
it, consumers either accept the one built-in look per kind or hand-paint, which is exactly the gap
E3 closes.

**Independent Test**: Author the same control twice — once with a "primary" class, once with a
"danger" class — lower both and assert the resolved visual properties differ in the
token-appropriate way (accent vs danger family) and that each matches the tokens for that class,
with no consumer-side color math.

**Acceptance Scenarios**:

1. **Given** a control with an attached semantic variant (e.g. "primary"), **When** it is lowered
   and resolved, **Then** its resolved paint/typography are derived from the tokens for that
   variant, not the default kind styling.
2. **Given** the same control kind with two different variants, **When** both are resolved under
   one theme, **Then** their resolved visual properties differ in the variant-appropriate way.
3. **Given** a consumer-defined free-form class mapped to token-derived values, **When** it is
   attached, **Then** it resolves through the same path as the built-in variants.

---

### User Story 2 - Interactive states render distinctly and survive re-renders (Priority: P1)

A control changes appearance as the user interacts with it — hover, press, focus, selected,
disabled, validation — and (because E2 gives it stable identity) that state-driven appearance is
consistent frame to frame rather than reset on every re-render. The state-driven look composes
with any attached style class.

**Why this priority**: State-driven visuals are the second half of "visual-state layer" and the
direct fix for the procedural-per-kind styling. It is the rung the roadmap names as E3's core
("hover/press/focus/selected/disabled render distinctly").

**Independent Test**: Resolve one control across each `VisualState` value and assert each produces
a distinct, token-appropriate resolved style; then resolve a control that has both an attached
class and a non-Normal state and assert the state's property wins where they overlap while the
class's other properties remain.

**Acceptance Scenarios**:

1. **Given** one control kind, **When** it is resolved under all eight states (Normal, Hover,
   Pressed, Focused, Selected, Disabled, Loading, Validation), **Then** each state the procedural
   baseline differentiates yields a visibly distinct, token-derived resolved style (states the
   baseline paints identically stay identical, preserving parity).
2. **Given** a control with an attached class **and** a non-Normal visual state, **When** both set
   the same visual property, **Then** the visual state's value wins (per the fixed precedence) and
   the class's non-overlapping properties are retained.
3. **Given** a control whose visual state changes between two frames, **When** E2's retained
   identity matches it across the frames, **Then** the resolved style transitions with the state
   (and, where an animation clock is attached, may animate) rather than being rebuilt from scratch.

---

### User Story 3 - One declarative resolver replaces procedural per-kind styling (Priority: P2)

A maintainer reads the styling path and finds a **single** state→style resolver fed by
(tokens + theme + classes + state) for the migrated controls, instead of per-kind inline color
branching scattered across each control's render code. The migrated controls produce
byte-identical render output to the prior procedural path under the default (no-class, Normal-or-
procedural-state) case, proving the refactor is behavior-preserving.

**Why this priority**: Consolidating the styling logic is the structural goal; it is lower urgency
than the two user-facing capabilities but is what makes the layer maintainable and what the exit
criteria measure.

**Independent Test**: For the representative migrated controls, render via the new resolver and via
a captured baseline of the prior procedural output for the same (kind, theme, state, no-class)
inputs; assert byte/structural-`Scene` equality. Assert no procedural per-kind color branch remains
for the migrated kinds.

**Acceptance Scenarios**:

1. **Given** a migrated control with no attached class in its default state, **When** it is
   resolved through the new resolver, **Then** its output is byte-identical to the prior procedural
   styling for that (kind, theme, state).
2. **Given** the migrated kinds, **When** the styling code is inspected, **Then** no per-kind inline
   visual-state color branching remains — styling flows through the one resolver.
3. **Given** an unmigrated control kind, **When** it renders, **Then** it is unchanged (the
   migration is additive and partial; no regression for kinds left on the procedural path).

---

### Edge Cases

- A control carries **no** attached class and is in `Normal` state: it MUST resolve to exactly the
  current default kind styling (the migration's behavior-preserving baseline case).
- **Conflicting classes**: two attached classes set the same property — the later-attached wins
  (deterministic, attach-order fold), and a subsequent visual-state value still overrides both.
- A class references a token that is **contrast-insufficient** against the resolved background:
  resolution still produces a value (it does not silently drop the class), and the existing
  contrast gate is the authority that flags it — E3 does not add a second contrast policy.
- A control is `Disabled` **and** has a "danger" class: the precedence (state over class) governs
  which property each side controls; disabled de-emphasis and danger intent compose per the fixed
  order rather than one silently erasing the other.
- A `Validation` state carries a `ValidationState` payload: the resolver maps the validation
  severity to token-derived styling deterministically (no per-kind validation branch).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: A control MUST be able to carry an ordered set of attached **style classes/variants**.
  The common semantic variants (e.g. primary/danger/ghost — the exact closed set is a plan detail)
  MUST be a **typed, closed** surface so the common path is compiler-checked; an additional
  **free-form** user class MUST be accepted for consumer-defined, token-derived styling. Both kinds
  flow through the same resolver.
- **FR-002**: Style resolution MUST be a **pure, total, deterministic** fold producing a control's
  resolved visual properties from inputs `(design tokens + theme + attached classes in attach
  order + current VisualState)`. Identical inputs MUST always produce an identical resolved style.
- **FR-003**: Resolution precedence MUST be the fixed, closed order **token/theme base < attached
  classes (earlier < later) < current visual state**, applied last-writer-wins per visual property.
  A visual state's value for a property MUST override any class's value for the same property; a
  later-attached class MUST override an earlier one. There MUST be **no** selector matching, no
  specificity algebra, and no cross-control cascade.
- **FR-004**: Each `VisualState` (Normal, Hover, Pressed, Focused, Selected, Disabled, Loading,
  Validation) MUST resolve through the single resolver — not via per-kind inline branching — and
  `Validation` MUST map its `ValidationState` severity to styling deterministically. Every state
  the prior procedural path visually differentiates MUST yield a distinct, token-derived style;
  where the procedural baseline treats two states identically (e.g. `Loading` inheriting `Normal`
  paint), the resolver MUST preserve that identity so byte-identical parity (FR-005) holds —
  parity wins over manufactured distinctness.
- **FR-005**: The procedural, per-kind visual-state styling MUST be replaced by the single resolver
  for a **representative set** of controls spanning the rich-geometry and box+label families —
  concretely **`Button`** (box+label) and **`CheckBox`** (rich-geometry). For
  the migrated kinds, the resolver's output for the default (no-class) case in each state MUST be
  **byte-identical / structurally-equal** to the prior procedural output (behavior-preserving
  refactor). Unmigrated kinds MUST be unchanged. A catalog-wide migration of all 52 controls is
  explicitly **out of scope** for this feature.
- **FR-006**: A control's resolved style MUST attach to its **E2 stable retained identity**, so a
  state-driven appearance is consistent across an unrelated re-render and (where E2's per-control
  animation clock is present) a state transition can animate rather than being rebuilt each frame.
  E3 MUST NOT re-derive or alter the reconciler identity scheme established by features 067/091/092.
- **FR-007**: Style resolution MUST remain **contrast-validated by the existing gate**: it MUST NOT
  introduce a second/parallel contrast policy. A class whose token yields an insufficient contrast
  ratio MUST still resolve to a concrete value (no silent drop); the existing contrast gate remains
  the single authority that flags it.
- **FR-008**: Token references in the resolver MUST stay sourced from the DTCG single source
  (`design-tokens.tokens.json` → generated `DesignTokens`); E3 MUST NOT hard-code new color/size
  literals that bypass `DesignTokenDrift`. Any new token needed by a variant is added to the DTCG
  source and regenerated, not inlined.
- **FR-009**: The styling layer MUST be **additive** to the MVU consumer surface: a consumer who
  attaches no class sees no behavior change, and the `view : 'model -> Control<'msg>` contract is
  unchanged. No data-binding, observable, dependency/attached-property, lookless-template, or
  CSS-selector capability is introduced (permanent roadmap non-goals).

> Interacting / conflicting requirements: FR-003 (state overrides class) vs FR-001 (consumer
> attaches a class to control appearance) — resolution: the consumer chooses the *base* intent via
> classes, but interactive feedback (hover/press/focus/disabled) always remains visible by winning
> the per-property contest; a consumer cannot suppress a focus/disabled cue with a class, by design.
> FR-005 (replace procedural styling) vs FR-005 (byte-identical output) — resolution: the refactor
> must preserve output exactly for the migrated default case; if a token-vs-literal rounding
> difference is unavoidable it is treated as a defect in the migration, not an accepted change, and
> output fidelity wins.
> FR-004 (each state distinct) vs FR-005/SC-003 (byte-identical to procedural) — resolution: a
> state yields a distinct style only where the procedural baseline already differentiates it; where
> the baseline paints two states identically the resolver preserves that identity, and parity wins
> over manufactured distinctness.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).**

- **Package impact**: No package identity or set changes. Behavioral + surface changes land in the
  existing **`FS.Skia.UI.Controls`** package: the style-class/variant authoring surface and the
  resolver (`src/Controls/Types.fs[i]`, `src/Controls/Control.fs`, and the
  `src/Controls/DesignTokens.*` / `design-tokens.tokens.json` DTCG source if a variant needs a new
  token), plus the typed `Props` front doors of the migrated controls under
  `src/Controls/Widgets/*.fs`. The typed front door (`FS.Skia.UI.Controls.Typed`) gains an
  attach-class affordance for the migrated controls. All packable libraries are version-bumped and
  the template pins refreshed on merge per the standard flow.
- **Public contract impact**: This feature **moves public surface (Tier 1)**. New public types/
  signatures appear on `FS.Skia.UI.Controls` — the typed closed variant union, the attach-class
  affordance on `Control`/typed `Props`, and the resolver's public entry if exposed — so
  `src/Controls/Types.fsi` (and any touched `*.fsi`) change and **controls-public-surface +
  per-package + cross-package surface baselines MUST be recaptured**. `DesignTokens.fsi` changes
  only if a variant adds a token (additive). `Theme` and the existing `DesignTokens` value surface
  stay value-identical for the migrated default case.
- **State workflow impact**: None to the consumer state model. Style resolution is a pure function
  of (tokens/theme/classes/state); it reads — but does not own or mutate — the `VisualState` that
  `ControlRuntime` already tracks. No new effect/command/subscription/interpreter behavior.
- **Layout/rendering impact**: Rendering output MUST be byte-identical / structurally-equal to the
  prior procedural styling for every migrated kind in its default (no-class) case across all
  `VisualState` values (FR-005). New class/variant inputs change the produced scene only when a
  consumer opts in by attaching a class. No new Skia/Vulkan surface; deterministic render-only
  evidence (no live window required). Existing fidelity decoding (ControlFidelity) and contrast
  gates apply.
- **Evidence obligations**: Real, in-repo readiness artifacts under
  `specs/093-visual-state-style-layer/readiness/` proving: (a) a semantic variant resolves to its
  token-derived style and two variants differ appropriately (US1); (b) each `VisualState` resolves
  to a distinct token-derived style and the fixed class-vs-state precedence holds (US2); (c) the
  migrated kinds' resolver output is byte-identical / structurally-`Scene`-equal to a captured
  baseline of the prior procedural output for the (kind, theme, state, no-class) inputs, and no
  per-kind color branch remains for them (US3); (d) the contrast gate still governs (no second
  policy). Parity proofs are authoritative as structural `Scene` / resolved-style equality
  (SceneEvidence render functions are deterministic capability-hash functions, not pixel encoders).
- **Unsupported scope**: Out of scope — CSS-selector matching, specificity/cascade, attached/
  dependency properties, lookless `ControlTemplate`/slot composition (that is E5, demand-driven),
  data binding/observables (permanent non-goal); a catalog-wide migration of all 52 controls off
  procedural styling (only a representative set here); a theme-switching *UI*; focus/keyboard
  traversal delivery (E4); a live windowed pixel-PNG capture path.
- **Build-target impact**: Run `Route` first and run only the gates it prints. A public
  `src/Controls/*.fsi` signature change escalates to the **controls-public-surface** rule, so the
  serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph
  → EvidenceAudit` path applies, plus `DesignTokenDrift` and `ContrastCheck`. No new gate is added;
  surface baselines are recaptured via `RefreshSurfaceBaselines` / `PerPackageSurface.captureCurrent`.

## Success Criteria *(mandatory)*

- **SC-001**: A consumer attaches a semantic style class to a control with zero color/theme math;
  the resolved visual properties are derived from that variant's tokens, and two different variants
  on the same kind under one theme produce distinguishably different resolved styles — verified for
  100% of the built-in variant set.
- **SC-002**: Each `VisualState` the procedural baseline visually differentiates resolves to a
  distinct, token-derived style for a representative control (states the baseline treats
  identically stay identical, preserving FR-005 parity), and when an attached class and a
  non-Normal state set the same property, the visual state's value wins while the class's
  non-overlapping properties are retained — verified across all eight states.
- **SC-003**: For every migrated control kind, the resolver's output for the default (no-class)
  case is byte-identical / structurally-`Scene`-equal to the prior procedural output across all
  `VisualState` values, and inspection confirms no per-kind inline visual-state color branch remains
  for the migrated kinds.
- **SC-004**: Style resolution is pure and deterministic: identical (tokens, theme, classes, state)
  inputs produce an identical resolved style across at least 1000 generated input combinations, and
  the fixed precedence (base < classes-in-order < state, last-writer-wins) holds for every generated
  combination.
- **SC-005**: A control's state-driven appearance is consistent across an unrelated re-render under
  E2's retained identity (a hover/focus/selected look is not reset by a sibling-shifting model
  update), demonstrated through the live-path identity rather than a hand-seeded map.
- **SC-006**: The contrast gate remains the single contrast authority — no migrated control's
  default styling regresses its contrast result, and a deliberately contrast-insufficient class is
  flagged by the existing gate (not silently dropped by the resolver).
- **SC-007**: Unmigrated control kinds are unchanged (no render-output delta), proving the migration
  is additive and partial, and the `view` consumer contract is unchanged for consumers who attach no
  class.

## Assumptions

- E2 (features 091 + 092) has landed: the keyed reconciler is on the live render path and confers a
  stable cross-frame identity and per-control animation clock that E3's state-driven styling attaches
  to. E3 consumes that identity and does not re-implement it.
- The existing `VisualState` enum (`Types.fsi:187`) is the complete set of states E3 resolves; this
  feature does not add new states, only a single resolver over the existing ones plus the new class
  surface.
- The DTCG token source (`design-tokens.tokens.json`) and `DesignTokens` generated module remain the
  single source of token values; any variant-specific token is added there and regenerated, keeping
  `DesignTokenDrift` authoritative.
- The design-system policy direction (WCAG/Ant, per the 2026-06-09 Ant Design adoption analysis)
  informs *which* token values variants map to but is decided in the token work, not re-litigated
  here; E3 consumes whatever tokens exist.
- Per the architecture-evolution decision, this is incremental MVU-core evolution toward
  declarative-retained parity (E-series), not a redesign; no data-binding or property-system surface
  is introduced.

## Key Entities

- **Style class / variant**: a consumer-attachable label — a typed closed set of semantic variants
  (primary/danger/ghost/…) plus a free-form user-class escape hatch — that maps to token-derived
  visual properties. Carried as an ordered list on a control.
- **Visual state**: the existing `VisualState` (Normal/Hover/Pressed/Focused/Selected/Disabled/
  Loading/Validation) tracked by `ControlRuntime`, consumed (not owned) by the resolver.
- **Resolved style**: the per-control output of resolution — the concrete paint/typography
  properties the renderer applies, produced purely from (tokens + theme + classes + state).
- **State→style resolver**: the single pure, total, deterministic fold that replaces procedural
  per-kind styling for migrated controls, applying the fixed precedence order.
- **Design tokens**: the generated `DesignTokens` values (from the DTCG source) that are the sole
  origin of color/size primitives the resolver composes; governed by `DesignTokenDrift`.
