# Phase 0 Research: Layout Dirty-Set Anti-Drift Guard (R7)

All NEEDS CLARIFICATION items from the Technical Context are resolved below. The
spec's Assumptions already bound the design space; this file records the
selected mechanism and the deferral decision.

## D1 — Single-sourcing mechanism (FR-001, SC-002)

**Decision**: A **build-time equality gate** (a behavioral probe) between the
classifier's covered name set and the layout lowering's actually-read names,
**plus** light name-token single-sourcing of the three string literals. The
runtime `layoutAffectingAttrNames` set is *kept* (the hot `layoutDirtySet`
classifier needs a cheap `Set.contains`), but it is enforced-equal to the
behaviorally-discovered truth, so it can no longer drift independently.

**Rationale**:
- The spec's own Assumptions explicitly accept "a build-time equality gate
  between the two" as a valid single-sourcing mechanism, alongside "derive" and
  "shared table."
- `toLayout`'s three reads are **semantically distinct** (`width`→`Size.Width`,
  `height`→`Size.Height`, `orientation`→`Direction`), so a single uniform table
  that `toLayout` iterates is unnatural and would obscure the lowering. Forcing
  membership single-sourcing here would trade a real drift risk for contorted
  code — exactly the cleverness Principle III discourages.
- Keeping the literal preserves R2's behavior **byte-identically** (FR-005/006):
  the classifier code path is unchanged; only a *test* now pins the literal to
  reality.
- Name-token `[<Literal>]` constants (`AttrWidth`/`AttrHeight`/`AttrOrientation`)
  shared by `nodeWidth`/`nodeHeight`/`orientationOf` and the literal remove
  *typo* drift cheaply, giving one authoritative token per name (the "one
  authoritative definition" of SC-002), while the gate enforces *membership*
  equality. Together they satisfy SC-002's intent: no independent,
  free-to-drift second list.

**Alternatives considered**:
- *Derive the literal at runtime from the probe.* Rejected — it would run the
  probe on the live hot path (perf regression, and a behavior change that risks
  FR-005/006), and make the runtime set depend on fixture completeness.
- *Source/AST static analysis of `toLayout`'s string reads.* Rejected — no
  lightweight, deterministic F# AST tool is wired into the gate path; a regex
  over source would be brittle and is not in-process-pure in the intended sense.
- *Shared single table `toLayout` iterates.* Rejected — unnatural given the
  per-name distinct semantics (see Rationale).

## D2 — How the probe discovers layout-driving names (FR-002, US1)

**Decision**: For each candidate attribute `name` in a representative **corpus**
and each fixture in a representative **fixture set**, build `fixture` with and
without an attribute named `name`, run the real
`ControlInternals.evaluateLayout`, and compare the resulting root `LayoutNode`
trees by structural equality. `name` is *layout-driving* iff toggling it changes
the `LayoutNode` on **any** fixture. The discovered set is the union over the
corpus.

**Rationale**:
- `LayoutNode` is a plain record with structural equality; `evaluateLayout` is
  already on the internal surface and is the exact function the live path
  measures with — so the probe observes *real* lowering behavior, not a model of
  it. A future `toLayout` that reads `floatValue "padding"` makes a padding
  toggle change the `LayoutNode`, so `padding` is discovered and the gate fails
  naming it (US1 scenario 1).
- Toggling an attribute *directly onto* the fixture (rather than relying on a
  control builder to emit it) means corpus completeness reduces to "is the name
  in the corpus," independent of which control kinds emit it.

**Corpus & fixtures**:
- **Fixtures** must let each real layout input take effect: at minimum (a) a
  plain container (non-`grid`/`toolbar`/`dock` kind, so `orientation` actually
  drives `Direction`) with ≥1 child, and (b) a leaf content control. This
  guarantees `width`/`height`/`orientation` are each observable.
- **Corpus** = the union of three **concrete, traceable** sources (no
  hand-curated free list): (1) `ControlInternals.layoutAffectingAttrNames`
  itself; (2) the attribute-name vocabulary the controls layer actually emits —
  the names produced by the `Attr` builder functions and attribute-name string
  literals in `src/Controls/Control.fs` (e.g. `orientation`, `value`, `text`,
  the geometry names) — so the corpus tracks the real control vocabulary and a
  future attribute auto-enrolls when its builder is added; and (3) a few explicit
  non-layout names (`background`, `foreground`, `text`, a style/visual-state
  class) to exercise the over-coverage direction and prove non-layout names are
  *not* discovered. The probe attaches each corpus name directly onto the fixture
  (D2 rationale), so corpus completeness reduces to "is the name in the
  vocabulary above," independent of which control kind would normally emit it.

**Documented boundary (observability)**: the gate proves equality over names
*reachable in the corpus*. A layout-driving attribute is only meaningful if some
control can carry it, so deriving the corpus from a representative gallery keeps
the under-coverage guarantee strong in practice. This residual is the same
"representative" discipline feature 097 used for its ≥1000-case property and is
documented at the test site (FR-007) rather than implied.

## D3 — Negative evidence without mutating `toLayout` (FR-002/003/007, SC-001)

**Decision**: Split the gate into (i) the pure `layoutDriftReport discovered
covered : DriftFinding list` and (ii) the probe that produces `discovered`. The
negative tests call `layoutDriftReport` with **simulated** sets
(`{width;height;padding}` vs `{width;height}` → `[Uncovered "padding"]`;
`{width}` vs `{width;orientation}` → `[OverBroad "orientation"]`) and assert the
formatter names the attribute and direction. The positive gate calls
`layoutDriftReport (probe()) layoutAffectingAttrNames` and asserts `[]`.

**Rationale**: gives clean, deterministic negative evidence for *both*
directions (under- and over-coverage) and exercises the human-legible formatter
(FR-007) without needing to perturb production `toLayout`. The pure function is
the unit under test; its inputs are its natural domain (sets of names), so this
is ordinary unit testing — **not** synthetic evidence (no real dependency is
mocked; the real dependency is exercised by the positive gate).

## D4 — Category-honoring assertion (FR-004)

**Decision**: Assert directly on the real `internal layoutDirtySet`: (a) an
`Update` whose `AttrChanges` contains an `AttrSet` with
`Category = AttrCategory.Layout` and a name **absent** from
`layoutAffectingAttrNames` still dirties the node; (b) an `AttrRemoved` of a name
that was Layout-category on the *prev* node dirties; (c) the name-set equality
gate (D2/D3) operates on names only and does **not** require a category-only
attribute to appear in the name list. (a)+(b) confirm the category channel;
(c) confirms the two channels are independent — the spec's stated resolution of
the FR-003↔FR-004 interaction.

**Rationale**: `layoutDirtySet` already reads `attr.Category` independently of
the name set (`src/Controls/RetainedRender.fs:268-272`); R7 only *asserts* this
holds so a future categorized attribute needs no name-set edit (forward
compatibility, spec Assumption).

## D5 — R2 invariant preservation (FR-005/006, SC-003/004)

**Decision**: Re-run, **unchanged**, the existing evidence:
- `tests/Layout.Tests/Feature097IncrementalTests.fs` — incremental-≡-full
  byte-identity over ≥1000 randomized edit sequences (INV-1 / SC-004).
- `tests/Controls.Tests/Feature097WiringTests.fs` — the `WorkReductionRecord`
  content-only-edit `RemeasuredNodeCount` assertions (SC-003).

**Rationale**: R7 changes no classifier or lowering behavior (the literal and
`layoutDirtySet` logic are byte-for-byte the same; only a private constant
refactor + comment fix + new tests are added), so these stay green and are cited
as the preservation proof rather than re-implemented.

## D6 — Intrinsic-size memo deferral (FR-008, SC-006)

**Decision**: **Deferred.** R2 shipped a computed-`Bounds` cache only; the
optional intrinsic-size memo named in roadmap §10.4 is **not** landed in R7. No
measured workload shows the fixed-size-ancestor boundary re-measure is hot, and
R7's charter is anti-drift hardening with zero behavior change — adding a memo
would introduce a new cache to validate against the incremental-≡-full property,
widening scope beyond the guard. The §10.4 wording reconciliation (R2 cached
`Bounds` only, memo optional/deferred) is **delegated to R8** per FR-008. This
decision is recorded here and surfaced as a tasks-level note so R8 can reconcile
the roadmap text without ambiguity.

**Rationale**: §11.6 recommends the low-risk hardening rungs before
behavior-changing work; landing an un-profiled cache contradicts that and risks
the R7 zero-delta guarantee. If a future profile shows the boundary re-measure
hot, the memo lands keyed by retained identity per §10.4 and gated by the same
incremental-≡-full property — as its own change, not R7.

## D7 — Route / gate posture (build-target decision deferred to plan, now resolved)

**Decision**: Land the enforcement as **ordinary Controls Expecto tests**, not a
new FAKE/Governance gate. Consequence: `Route` runs **inner-loop** (`Dev`);
`AgentValidation.knownGates` and `validation.contract.yml` are untouched (no
`TargetMetadataDrift` escalation). Keep all new helpers private/test-local so no
`.fsi` or per-package internal baseline moves. The feature's evidence
obligations still run `EvidenceGraph` + `EvidenceAudit` (sequentially) to produce
the verdict-token artifacts. Always run `./fake.sh build -t Route` first and obey
the printed gate list if it differs.

**Rationale**: the drift is a Controls-internal coupling; an in-suite test that
runs under `Dev` is the minimal, deterministic mechanism and matches the R2
evidence style. Promoting it to a FAKE gate would add governance surface for no
guarantee the Expecto test does not already provide.
