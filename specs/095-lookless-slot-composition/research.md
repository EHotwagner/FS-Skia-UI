# Phase 0 Research: Lookless Slot Composition (E5)

All Technical Context unknowns are resolved below. The spec's Clarifications session
(2026-06-10, seven questions) already fixed the highest-risk decisions — slot closure, the
static-`Control<'msg>` fill, the typed/closed-no-free-form surface, the `Attr`-category carrier,
the representative set, the folded-in consumer-skill deliverable, and the ungated
implementation status; this document records the implementation-level decisions that follow
from them, each grounded in the E3 (feature 093) precedent that E5 deliberately mirrors.

## Decision 1 — How slots are carried in the IR

**Decision**: Add a new `AttrCategory.Slot` case and a new
`AttrValue.SlotFillsValue of (string * Control<'msg>) list` case to `Types.fsi`/`Types.fs`. A
control carries **one** slot attribute (last-writer-wins) whose value is an association list
from slot name to fill sub-tree.

**Rationale**: This is the exact shape E3 used for `StyleClassesValue of StyleClass list`
under `AttrCategory.Style` (`Types.fs:231-270`). Carrying all fills in one keyed list (rather
than one attr per slot) matches the codebase's established **last-writer attribute
convention** that `ControlInternals.tryLast` / `styleClassesOf` already implement
(`Control.fs:50-71`), so the extractor is a one-line mirror. A slotted child placed into the
lowered control's `Children` then travels through the keyed reconciler diff and keeps its E2
retained identity with **no** reconciler change (FR-004) — confirmed by how `childrenFrom`
(`Control.fs:89-95`) already flattens `ChildValue`/`ChildrenValue` into `Children`.

**Alternatives considered**:
- *A new top-level `Control` record field (`Slots: ...`)* — rejected by FR-004 and the spec
  clarification: it would bypass the `Attr` mechanism, would not travel through the keyed
  reconciler without bespoke wiring, and would re-open the reconciler identity scheme E5 is
  required to leave untouched.
- *One `Attr` per slot (`slot_leading`, `slot_trailing`)* — workable, but diverges from the
  single-attr last-writer convention and complicates the "no slot attr at all ≡ byte-identical
  default" check. Rejected for the simpler single-keyed-list carrier.

## Decision 2 — How the typed/closed (no-free-form) surface is enforced

**Decision**: The **public** slot authoring surface is the **typed `Props` record fields** of
the slot-bearing kinds (`Button`: `Leading` / `Trailing`; `Panel`: `Header` / `Footer`), each
`Widget<'msg> option`. The string-keyed `Attr`-level slot builder
(`ControlInternals.slotFill`) and extractor (`ControlInternals.slotFillsOf`) are **internal**
(behind the existing `module internal ControlInternals` / `InternalsVisibleTo` seam, as
`chartValues` already is). There is **no** public free-form `Attr.slot : string -> ...`
builder.

**Rationale**: This is the deliberate divergence from E3 the spec calls out (FR-001;
Clarifications Q3): a free-form *style class* is safe (it resolves to token-derived values),
but a free-form *slot* on an arbitrary kind is exactly the lookless-`ControlTemplate` engine
the roadmap permanently rejects. Making the closed surface the **record fields** means a
consumer literally cannot reference a slot a kind does not declare — there is no field for it,
so US1 scenario 3 ("fill a slot the kind does not declare") is a **compile-time error**, not a
silent runtime drop (SC-006). The internal string key is plumbing the typed views call; it is
never a consumer affordance.

**Alternatives considered**:
- *A public `Attr.styleClasses`-style `Attr.slot : string -> Control<'msg> option -> Attr<'msg>`*
  — rejected: a public string-keyed builder **is** the free-form escape hatch FR-001 forbids.
- *A per-kind closed slot-name DU (e.g. `ButtonSlot = Leading | Trailing`) exposed publicly,
  with a typed `Attr.slot : ButtonSlot -> ...`* — viable and fully closed, but heavier than the
  record-field surface and adds N public DUs for N kinds. The record-field surface achieves
  the same compile-time closure with less public surface; chosen for idiomatic simplicity
  (Principle III). The internal carrier still uses string keys so the IR shape stays uniform
  across kinds.

## Decision 3 — Default fallback and byte-identity (the additive guarantee)

**Decision**: Each slot-bearing kind's geometry in `ControlInternals` is refactored so that,
**per declared named region**, it places the slot fill's sub-tree if present, else renders the
region's **default content** — which is *exactly today's chrome for that region*. The
flanking/peripheral default regions (`Button.Leading`/`Trailing`, `Panel.Header`/`Footer`)
default to **zero geometry** (empty), so an unfilled control does not shift its label/content
and is byte-identical. When **no** slot attribute is present at all, the extractor returns the
empty fill set and the kind renders its pre-slot output verbatim.

**Rationale**: FR-003 / SC-002 require an unfilled slot-bearing control to be
byte-identical / structurally-`Scene`-equal to its pre-slot render across the kind's states,
and FR-007/SC-007 require kinds *not* given slots to be unchanged. The E3 parity precedent
(`Feature093ParityTests.fs`, the `frozenButtonGeom` oracle at lines 27-33) is the exact proof
technique: freeze the pre-slot geometry as an oracle, assert the refactored path equals it for
the unfilled case. The "default contributes zero geometry" rule for peripheral regions is what
makes the label/content position invariant under the refactor.

**Alternatives considered**:
- *Always reserve leading/trailing region space even when unfilled* — rejected: it would shift
  the label and break byte-identity (SC-002). Filled regions shift layout (consumer opted in,
  output legitimately changes); unfilled regions must not.
- *Distinguish "unfilled" only by `None` and treat empty-content as default too* — rejected by
  the spec edge case: filling with an explicitly empty sub-tree renders empty **by the
  consumer's choice**, distinct from "unfilled falls back to default." The carrier therefore
  distinguishes "slot name absent from the fill list" (→ default) from "slot name present with
  empty content" (→ empty).

## Decision 4 — How slotted content composes with E1–E4 (free, no new machinery)

**Decision**: Slot-fill sub-trees are injected into the **lowered control's `Children`** at
their region positions (alongside, and ordered with, the kind's default-region children). No
E1/E2/E3/E4 code changes.

**Rationale**: A slot fill is a real `Control<'msg>`. Once it is in `Children` it is
indistinguishable from any other child to the rest of the framework: the keyed reconciler
diff matches it key-first-then-positional and confers E2 retained identity (features
067/091/092); its `EventValue` bindings dispatch through the existing flat per-`ControlId`
mechanism (E1); its `StyleClassesValue`/`VisualStateValue` resolve through `Style.resolve`
(E3); and `Focus.order`/`traverse`/`route` (E4) include it when its `KeyboardOperation`
declares it focusable. This is precisely why FR-005 can assert "compose with E1–E4 unchanged"
— the composition is structural, not bespoke. The same-kind-sibling collision risk (the
086/092 learning) is already handled by the reconciler's key-first rule, so two same-kind
slot fills in different regions keep distinct identity (Edge Cases).

**Alternatives considered**:
- *A parallel slot-fill render/focus/dispatch path* — rejected: it would make slotted content
  a "second-class sub-tree" / dead-zone (the exact anti-goal FR-005 forbids) and duplicate
  E1–E4. Injecting into `Children` reuses everything.

## Decision 5 — The representative two-kind set

**Decision**: **`Button`** (leaf-with-regions) with `Leading` / `Trailing` slots flanking its
label; **`Panel`** (composite-container) with `Header` / `Footer` chrome regions around its
existing children (`Content`).

**Rationale**: FR-007 requires a representative set spanning a leaf-with-regions kind and a
composite-container kind, with the exact container a plan detail. `Button`'s box+label is the
canonical leaf-with-regions case (an icon-leading button is the spec's headline example) and
its legacy geometry (`buttonGeom`, `Control.fs:653-685`) is small and already parity-tested by
E3 — the cheapest faithful refactor. `Panel` is the simplest composite-container (`Panel.view`
just passes children through, `Containers.fs:121-127`), so adding `Header`/`Footer` regions
whose unfilled default is empty keeps the existing pass-through byte-identical while
demonstrating the "replace a container's header region" scenario. Both already have typed
`Props` front doors (`Primitives.fsi` `ButtonProps`; `Containers.fsi` `PanelProps`) to extend
additively.

**Alternatives considered**:
- *`SplitView` / `Dock` as the container* — viable, but heavier chrome and less obviously a
  "named header/footer region" demonstration than `Panel`. Deferred to the catalog-wide
  follow-up.

## Decision 6 — Purity, determinism, and the property surface (SC-005)

**Decision**: Slot lowering is a pure, total function; an FsCheck property generates ≥1000
`(kind, slot fills)` combinations and asserts (a) identical inputs lower to an identical
`Control<'msg>` IR and (b) lowering never throws (unfilled → default, filled → placed).

**Rationale**: Directly encodes SC-005 and FR-006, and mirrors E3's determinism property. The
generator produces both representative kinds with arbitrary subsets of their declared slots
filled (including the empty-content case), exercising the absent-vs-empty distinction from
Decision 3. Determinism is straightforward because there is no `Date.now`/random/IO in the
lowering path — it is a fold over the slot-fill list and the kind's region defaults.

## Decision 7 — Consumer-capability skill: expand the two shipping consumer skills (FR-010/FR-011)

**Decision**: Deliver the E1–E5 capability guidance by **expanding** the package-owned
`src/Controls/skill/SKILL.md` (`fs-skia-ui-widgets`) and the template-fragment
`template/fragments/controls/skill/SKILL.md` (`fs-skia-generated-controls-guidance`) — each
names and shows a **runnable consumer example** for all five rungs: live event dispatch (E1),
retained identity (E2 — why focus/text survive a sibling-shifting re-render), style
class/variant + visual state (E3), focus/keyboard traversal (E4), and slot composition (E5).
**No** new `.agents/skills/<id>` governance skill is added.

**Rationale**: FR-010/FR-011 + Clarifications Q5/Q6. The landed E1–E4 rungs (090–094) shipped
without updating consumer guidance — both skills today still teach the pre-E1 "build
`Control<'msg>` with `create` + attrs, keep state in the model" story and mention none of style
classes, visual state, focus traversal, or retained identity, so a template consumer cannot
discover capabilities the framework already has (US4). These two skills are the **channel that
ships into generated products**: the package-owned skill is the agent/package-facing one and the
template-fragment skill is selected into a `dotnet new fs-skia-ui` project's Controls capability
(SC-009). A repo-facing `.agents/skills/<id>` would not reach a generated consumer, so it is
explicitly **not** the vehicle. The `.claude` peer is regenerated from the canonical `.agents`
source by `RefreshSurfaceBaselines`, never hand-edited (`SkillSyncCheck`), and the content must
pass `SkillQualityCheck` (rubric) and keep `GeneratedGuidanceCheck` green.

**Honesty constraint (FR-010)**: a slot lowers to `Control<'msg>` and is **not** a data-bound
template; retained identity is a property of the keyed tree, not a binding. Because E1–E5 are
**all shipped by this feature** (Decision under FR-009), the skill carries **no** Principle V
synthetic-evidence disclosure — every rung it documents is a real, landed capability.

**Alternatives considered**:
- *A new standalone `.agents/skills/fs-skia-architecture-evolution` governance skill* — rejected
  by FR-011: that audience is repo/agent-facing and does **not** ship into generated projects,
  so it would miss the actual `dotnet new fs-skia-ui` consumer (the gap US4 closes).
- *Documenting only E5 (the new rung)* — rejected: the spec scopes the deliverable to the full
  E1–E5 surface precisely because E1–E4 shipped without consumer guidance (FR-010).

## Cross-cutting confirmations

- **No new effect/command/subscription/interpreter** (FR-006, MVU N/A): the lowering is pure
  structure; slotted content's state uses the existing E1–E4 mechanisms. Confirmed against
  Constitution Principle IV — no stateful/I/O workflow is introduced.
- **Tier 1 surface recapture**: new `Types.fsi` cases + typed-`Props` deltas move public
  surface; controls-public-surface + per-package + cross-package baselines are recaptured via
  `RefreshSurfaceBaselines` / `PerPackageSurface.captureCurrent`, never hand-edited (the
  per-package-not-in-RefreshSurfaceBaselines gotcha applies — regenerate per-package snapshots
  via `PerPackageSurface.captureCurrent`).
- **Ungated implementation** (FR-009): the roadmap originally **demand-gated** E5's scheduling,
  but the spec's clarification (Q7) folds the FR-010/FR-011 consumer-capability-skill deliverable
  into this feature, and that is the concrete trigger that **ungates implementation** — the slot
  mechanism is **implemented as part of feature 095, no longer deferred**. The skill therefore
  documents E1–E5 as all-shipped capabilities with **no** Principle V synthetic-evidence
  disclosure. The bounded scope fixed by FR-001…FR-008 still pins the mechanism against
  scope-creep toward the rejected templating engine.
