# Phase 0 Research: Declarative Visual-State & Style-Class Layer

All four spec `## Clarifications` (resolution model, class-vs-state precedence, migration
scope, typed-vs-free-form classes) were resolved in the 2026-06-10 clarify session. No
`NEEDS CLARIFICATION` markers remain in the spec. The remaining unknowns are *implementation
shape* decisions, resolved below against the actual codebase.

## R1 — How is styling done today, and where does the resolver insert?

**Decision**: The migrated controls' paint is refactored inside `module internal
ControlInternals` (Control.fs) to call a new `Style.resolve` and read `ResolvedStyle` fields,
replacing inline `theme`-and-boolean-attribute branching. The resolver lives in a new
`module Style` compiled after `Theme.fs`, before `Attributes`/`Control`.

**Rationale**: Today there is **no** state→style resolver and **no** `VisualState` threaded
into rendering. `ControlInternals.renderNode`/`paintNode`/`faithfulContent`/`renderScene` style
purely from `theme: Theme`, reading per-kind boolean attributes inline (`boolValue "selected"`,
`"on"`, `theme.Accent` vs `theme.Muted` — Control.fs:432,472,488,515,889). `VisualState`
itself is only *catalog metadata* (`Catalog.fs:22`) and runtime focus/animation state
(`ControlRuntime.FocusedControl`, `RetainedRender.StateByIdentity`); it never reaches the paint
code. So E3's resolver is the **first** place `(tokens, theme, classes, state)` is composed.
Inserting it as a sibling module keeps `Control.fs` the single caller and preserves the 091/092
`ControlInternals` factoring that `RetainedRender` reuses (so the retained path inherits the
resolver for free).

**Alternatives considered**: (a) Resolve inside each `Widgets/*.fs` typed `view` before
lowering — rejected: the legacy `Control<'msg>` render path (`renderNode`) is where paint is
actually produced and where byte-identity must be proven; resolving in the typed front door
would leave the legacy path procedural and split the styling logic in two. (b) A resolver in a
brand-new top-level package — rejected: no package-identity change is warranted (spec
Package-impact), and it would force a cross-package dependency cycle with `Types`.

## R2 — How does a consumer attach style classes, and what is the typed-closed surface?

**Decision**: Add to `Types.fsi`:
- `type StyleVariant` — a typed, closed union of the built-in semantic variants
  (`Primary | Danger | Ghost | Neutral | Success | Warning` — exact set fixed in
  data-model.md), `[<RequireQualifiedAccess>]`.
- `type StyleClass = Variant of StyleVariant | Custom of string` — the per-entry carrier,
  unifying the typed path and the free-form escape hatch.
- a new `AttrValue<'msg>` case `StyleClassesValue of StyleClass list` so an ordered class list
  rides the existing `Attr` mechanism under the existing `AttrCategory.Style`.

`Attributes.fs[i]` gains `styleClasses : StyleClass list -> Attr<'msg>`. The typed `Props` of
the migrated controls gain a `Classes: StyleClass list` field (default `[]`) that lowers to that
attribute.

**Rationale**: FR-001 demands *both* a compiler-checked common path and a free-form hatch; a
single `StyleClass` union with a `Variant`/`Custom` split gives both while flowing both through
one resolver (FR-001, FR-002). Reusing `Attr`/`AttrCategory.Style` (which already exists,
`Types.fsi`) avoids inventing a parallel attachment mechanism and keeps `Control<'msg>` shape
stable (classes are just attributes, ordered by list position = attach order, FR-003). A new
`AttrValue` case (rather than encoding into `StringListValue`) keeps the typed variants
non-stringly-typed end-to-end and keeps the resolver total over a closed type.

**Alternatives considered**: (a) Encode classes as `StringListValue` of variant names —
rejected: loses compiler-checking, makes the resolver partial over arbitrary strings, and
conflicts with FR-001's "typed, closed" requirement. (b) A dedicated `Classes` field on the
`Control<'msg>` record — rejected: a Tier-1 record-shape change with broader blast radius than
an additive `AttrValue` case, and it would not flow through the existing attribute plumbing the
typed front door already uses.

## R3 — What is `ResolvedStyle`, and which properties does the resolver own?

**Decision**: `ResolvedStyle` is a flat record of the concrete visual properties the migrated
kinds consume from the resolver: foreground/text color, fill/background color, stroke color +
width, and font (family option, size, weight option). Each is the *last writer* under the fixed
precedence. The resolver returns one `ResolvedStyle`; the migrated paint reads its fields where
it previously read `theme.Accent`/`theme.Muted`/`theme.Foreground` etc.

**Rationale**: The procedural code's actual degrees of freedom are color (fill/stroke/text) and
typography — exactly the properties listed (matching the `Paint.fill`/`Paint.stroke`/`textRun`
usage in Control.fs). A flat record makes last-writer-wins per-property trivial and total, and
makes structural equality (the parity proof) a plain record comparison. Properties the migrated
kinds don't vary (geometry) stay computed as today — the resolver governs *paint/typography
only*, matching the spec's "resolved paint/typography" language (US1, FR-004).

**Alternatives considered**: A `Map<string, value>` property bag — rejected: stringly-typed,
non-total, and defeats compiler-checked structural equality. Per-kind `ResolvedStyle` variants —
rejected: re-introduces per-kind branching the feature exists to remove.

## R4 — How is the fixed precedence implemented as a pure, total fold?

**Decision**: `resolve tokens theme classes state : ResolvedStyle` = start from the
**token/theme base** (the current default kind styling expressed as a `ResolvedStyle`), fold the
attached `classes` left-to-right (each class overwrites the properties it sets, later wins), then
apply the `state` layer last (a `VisualState → ResolvedStyle-delta` total mapping that overwrites
the properties it sets). Each layer is a `ResolvedStyle -> ResolvedStyle` that overwrites only
the properties it owns; "last writer wins per property" falls out of ordered record updates.

**Rationale**: This is FR-003 verbatim — `base < classes (earlier<later) < state`,
last-writer-wins, no selectors/specificity/cascade. A left fold over `classes` then a single
`state` application is the plainest total expression (Principle III). Totality: `StyleVariant`
is closed and `VisualState` has eight cases, so both layer-maps are exhaustive `match`es with no
fallthrough; `Custom name` with an unknown token resolves to the base (identity delta), which is
deterministic, not a failure. Determinism: no clock/randomness/Map-ordering — pure record
updates over an ordered list (SC-004).

**Alternatives considered**: A priority-sorted merge with explicit specificity weights —
rejected: that *is* a specificity algebra, a permanent non-goal (FR-003). A right fold or
Map-merge — rejected: obscures the "later wins" semantics and risks nondeterministic Map
iteration.

## R5 — How is byte-identity to the prior procedural output proven (no live window)?

**Decision**: Parity is asserted as **structural `Scene` equality**: capture the current
procedural render (the `Scene list` from `ControlInternals.renderScene`/`renderNode`) for each
migrated `(kind, theme, state, no-class)` into a baseline file under `readiness/parity/`, then
assert the post-refactor resolver-driven render is structurally equal to that baseline. The
resolver's default (no-class) `ResolvedStyle` must reproduce the exact colors/typography the
procedural code produced.

**Rationale**: `SceneEvidence.renderPng`/`renderReadbackEvidence` are deterministic
*capability-hash* functions, not pixel encoders (established in features 091/082 and the spec's
Evidence-obligations note), so the authoritative parity proof is structural `Scene` /
resolved-style equality, which is exact and environment-independent. `Scene` values are
comparable records, so equality is a total, deterministic check — and capturing the baseline
*before* the refactor and diffing *after* makes the parity test failing-first (Principle VI).

**Alternatives considered**: Pixel-PNG diffing via a live Vulkan window — rejected: explicitly
out of scope (spec Unsupported-scope), non-deterministic across hosts, and unnecessary since
`Scene` equality is exact. Capability-hash equality alone — rejected: a hash collision could
mask a real delta; structural `Scene` equality is stronger and is what 091 established.

## R6 — Which controls are the "representative set" to migrate?

**Decision**: Migrate **Button** (box+label family) and **one rich-geometry family control**
(candidate: `progress`/`gauge` or `chart`-class kind that visibly branches color on
state/selection). The exact rich-family pick is finalized in tasks against which kind most
clearly exercises accent-vs-muted state branching, but the set spans both families per FR-005.

**Rationale**: FR-005 requires the representative set to span the rich-geometry and box+label
families and to prove byte-identity for the migrated kinds while leaving the rest procedural
(SC-007). Button is the canonical box+label kind and the natural carrier for the
primary/danger/ghost variants (US1's headline example). A rich-geometry kind proves the resolver
generalizes beyond simple box+label paint. Keeping the set small honors the explicit
"all-52 migration is out of scope" boundary.

**Alternatives considered**: Migrate only Button — rejected: doesn't span both families
(FR-005). Migrate a large set — rejected: out of scope and inflates parity-baseline surface
without proving anything new.

## R7 — Token sourcing for variants (DTCG authority)

**Decision**: Variants map to **existing** generated `DesignTokens` values where possible
(`accent` → Primary, `danger` → Danger, `muted`/`background` → Ghost/Neutral). If a variant
needs a value with no existing token (e.g. a distinct success/warning hue), the token is added
to `design-tokens.tokens.json` (DTCG source) and `DesignTokens.fs` regenerated via
`RefreshSurfaceBaselines`; `DesignTokens.fsi` gets the additive `val`. No color/size literal is
inlined in the resolver.

**Rationale**: FR-008 + the spec assumptions make the DTCG source the sole token origin, kept
authoritative by `DesignTokenDrift`. Mapping to existing tokens first minimizes surface delta;
adding a token through the DTCG pipeline (never inline) keeps the drift gate authoritative and
`ContrastCheck` the sole contrast authority (FR-007). Whether any new token is needed is settled
in tasks once the variant set's target values are fixed against the existing token inventory.

**Alternatives considered**: Inline new color literals for success/warning — rejected: bypasses
`DesignTokenDrift` (FR-008). A second token file for "variant tokens" — rejected: splits the
single source of truth.

## R8 — How does the state-driven look survive a re-render via the live retained path (SC-005)?

**Decision**: E3 adds nothing to identity tracking. The resolver is invoked inside the same
`ControlInternals` paint functions that `RetainedRender` already drives per frame; because the
`VisualState`/animation clock are keyed to the stable `RetainedId` (092's
`StateByIdentity`), a sibling-shifting model update that re-keys positionally still resolves the
same state for "the same control". SC-005 evidence drives a real two-frame sequence through the
live retained path (the 092 wiring) and asserts the resolved look persists — not a hand-seeded
`StateByIdentity` map.

**Rationale**: FR-006 forbids re-deriving the 067/091/092 identity scheme; the resolver only
*reads* it. Proving SC-005 through the live path (not a seeded map) is the explicit lesson from
the 092 review (the E2 wiring gap memory): hand-seeded state proved nothing about the real host.

**Alternatives considered**: Add a style-specific identity cache — rejected: re-derives identity
(FR-006 violation) and duplicates 092's `StateByIdentity`.
