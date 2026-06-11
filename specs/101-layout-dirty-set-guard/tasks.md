# Tasks: Layout Dirty-Set Anti-Drift Guard (R7)

**Feature branch**: `101-layout-dirty-set-guard`
**Spec**: `specs/101-layout-dirty-set-guard/spec.md`
**Plan**: `specs/101-layout-dirty-set-guard/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` is an annotation for design-approved synthetic error-handling work; it
remains `[S]` when completed. **None planned for this feature.** R7's enforcement
is built from **pure, total** functions exercised against **real** dependencies:
`layoutDriftReport`/`formatDrift` are pure set-difference + formatting over their
natural domain (sets of attribute names), and the load-bearing probe runs the
**real** `ControlInternals.evaluateLayout` and asserts against the **real**
`layoutAffectingAttrNames` literal and the **real** `internal layoutDirtySet`.
The negative-drift tests feed literal `Set` values to the *real* pure function —
ordinary unit testing of a pure function over its natural domain, **not** a mock,
fake, stub, placeholder, or in-memory substitute for any real dependency — so
Principle V disclosure is not triggered and `EvidenceAudit` must report no
synthetic work. Any `[S]` that appears triggers the full Principle V disclosure
regime.

## Tier & MVU posture

This is a **Tier 2 (internal) change** — uniform across all tasks, so per-task
`[T2]` marks are omitted. **No public or internal `.fsi` signature change** is
expected: the drift report, the formatter, and the behavioral probe stay
**test-local** in `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs`, and
the US2 name-token `[<Literal>] private` constants stay **private** to
`src/Controls/Control.fs`. The enforcement consumes only the already-published
**internal** surface (`ControlInternals.layoutAffectingAttrNames`,
`ControlInternals.evaluateLayout`, the `internal layoutDirtySet`) reached via the
existing `InternalsVisibleTo "Controls.Tests"`.

**No MVU surface.** R7 is not stateful/IO — no `Model`/`Msg`/`Effect`/`Cmd`/
`init`/`update`/interpreter is introduced or changed. `layoutDriftReport` and the
probe are pure functions of their inputs (Constitution Principle IV is **not
applicable**). The classifier (`layoutDirtySet`) and lowering (`toLayout`) code
paths are left **byte-for-byte unchanged**; only a private constant refactor, a
comment correction (stop claiming false single-sourcing — point at the gate), and
new tests are added.

**Persistent-launch / viewer-launch rule does not apply.** R7 adds and changes
**no** default-executable, persistent-launch, or graphical entry point and
changes **no** observable rendering output (R2 INV-1 preserved byte-identically).
The user-reachable surface for this hardening feature is the **build/test gate
itself**: a contributor who introduces drift gets a fast, explicit, named Expecto
failure under `Dev`. Recorded as a **visible decision** in T003 —
`window-visibility.md`, `real-image-evidence.md`, and `visual-evidence-honesty.md`
record this as **not-applicable** with honest `key=value` values (no window, no
screenshot, byte-identical output); there is no persistent-launch / window /
screenshot obligation.

## Vertical-slice rule (US phases)

R7's `[US*]` tasks are framework-governance work, not a graphical runtime path.
A `[US*]` task is `[X]` only when its enforcement is **actually exercised against
the real dependency** under `Dev`: US1's gate runs the **real**
`evaluateLayout`/`layoutDirtySet`/`layoutAffectingAttrNames` (a deliberately
drifted set fails the report; the shipping set passes), and US3's preservation
re-runs the **existing** feature-097 evidence unchanged. Passing a test that only
exercises the pure `layoutDriftReport` with simulated sets, with the real probe
gate **not** run, does **not** satisfy `[X]` for the US1 enforcement task. The
"user-facing entry point" is the failing build/test (the developer-facing seam),
exercised by running the Controls + Layout Expecto suites under `Dev` (no FSI
host, no window — see Tier & MVU posture).

## Success-criterion → assertion mapping

- **SC-001** (100% of drift attempts caught, **both** directions, naming the
  offending attribute) → T007 failing-first negatives (`{w;h;padding}` vs `{w;h}`
  → `[Uncovered "padding"]`; `{w}` vs `{w;orientation}` → `[OverBroad
  "orientation"]`) + formatter naming each attribute & direction (FR-007) + T009
  pure `layoutDriftReport`/`formatDrift` + the **real** behavioral probe gate
  (`layoutDriftReport (discoverLayoutDrivingNames size) layoutAffectingAttrNames
  = []`) + T010 (`drift-guard.md`).
- **SC-002** (exactly **one** authoritative definition; **zero** independent
  hand-maintained second lists) → T011 name-token `[<Literal>] private` constants
  (one token per name, shared by `nodeWidth`/`nodeHeight`/`orientationOf` and
  `layoutAffectingAttrNames`) + the T009 probe gate enforcing **membership**
  equality (drift becomes impossible to ship) + T012 (`single-source.md`
  inspection: no second free-to-drift literal).
- **SC-003** (a content-only / style / state / visual-state edit re-measures the
  **same** node count as pre-R7) → T014 re-running the **unchanged**
  `WorkReductionRecord.RemeasuredNodeCount` assertions in
  `tests/Controls.Tests/Feature097WiringTests.fs` (`r2-preservation.md`).
- **SC-004** (incremental bounds **byte-identical** to full evaluation over
  ≥1000 randomized edit sequences) → T013 re-running the **unchanged**
  `tests/Layout.Tests/Feature097IncrementalTests.fs` property (`r2-preservation.md`).
- **SC-005** (no public surface / consumer-observable behavior change; routed
  gate set green; `EvidenceAudit` reports no synthetic work) → T015 zero
  surface-drift confirmation (`PerPackageSurface.captureCurrent` vs the T006
  reference) + T016 `Dev` green (`validation-log.md`) + T018 audit PASS with no
  synthetic.
- **SC-006** (the intrinsic-size-memo decision (FR-008) explicitly recorded so
  R8 can reconcile §10.4 without ambiguity) → T005 (`runtime-limitations.md`
  records the **deferral** + the R8 §10.4 wording handoff).

## Non-SC requirement traceability

- **FR-001** (classifier name set derived-from / gated-against the lowering's
  single source so the two cannot silently diverge) → T009 (the behavioral-probe
  **equality gate** between `discoverLayoutDrivingNames` and
  `layoutAffectingAttrNames`).
- **FR-002** (build fails on **under-coverage** — an un-covered layout input
  cannot ship silently) → T007 (`Uncovered` negative) + T009 (the gate fails the
  instant `toLayout` reads a corpus name absent from the literal).
- **FR-003** (build fails on **over-coverage**; the two sets enforced **exactly
  equal**) → T007 (`OverBroad` negative) + T009 (set-difference both directions).
- **FR-004** (classifier honors any `AttrCategory.Layout`-tagged attribute
  independent of the name set; enforcement asserts the honoring) → T008
  (category-honoring units on the **real** `layoutDirtySet`: `AttrSet` Layout-
  category name **absent** from the set still dirties; `AttrRemoved` of a prev-
  node Layout-category attr dirties; a `Visual`-category change does **not**
  dirty; the name-set gate does **not** demand the category-only name appear).
- **FR-005** (no incremental-result change; INV-1 over the ≥1000-case property) →
  T013 (re-run unchanged).
- **FR-006** (no additional re-measure vs the pre-R7 baseline) → T014 (re-run
  unchanged).
- **FR-007** (human-legible failure naming the drifting attribute(s) + direction)
  → T007 (formatter assertions) + T009 (`formatDrift` impl; documents the
  corpus-bounded coverage boundary at the test site).
- **FR-008** (optional intrinsic-size memo OUT of default scope; record an
  explicit decision; §10.4 reconciliation delegated to R8) → T005 (deferral +
  R8 handoff in `runtime-limitations.md`).
- **FR-009** (preserve all permanent non-goals — no data binding, dependency
  properties, CSS selectors, or template engine; internal wiring + gate only) →
  T003 (record) + T005 (non-goals in `runtime-limitations.md`).

## Governance risk levels

- **Small** — the pure `layoutDriftReport`/`formatDrift` (totality, both
  drift directions, the deterministic order-stable findings, the human-legible
  formatter): focused validation is `Dev` + the targeted
  `Controls.Tests/Feature101*` drift-report unit tests.
- **Medium** — the behavioral **probe** over the real `evaluateLayout`
  (corpus × fixtures discovery, structural `LayoutNode` equality, union) and the
  FR-004 category-honoring units on the real `layoutDirtySet`: `Dev` + the
  `Controls.Tests/Feature101*` probe-gate + category suites; plus the re-run of
  the **unchanged** `Layout.Tests/Feature097IncrementalTests.fs` and
  `Controls.Tests/Feature097WiringTests.fs`.
- **Broad** — escalation **does not apply**: this is a framework-internal
  `src/Controls/**` + `tests/**` change with **no** `.fsi`/template/governance
  surface move, so `Route` routes **inner-loop → `Dev` only**, and no new
  FAKE/Governance gate is registered (`AgentValidation.knownGates` /
  `validation.contract.yml` untouched). **`Route` is authoritative** — run
  `./fake.sh build -t Route` against the actual diff and run exactly the gates it
  prints (`--enforce` for missing required evidence); if it unexpectedly
  escalates (e.g. an unintended per-package internal baseline shift), run exactly
  what it prints, **sequentially**. The feature's evidence obligations
  additionally run `EvidenceGraph` then `EvidenceAudit` — FAKE-backed, run
  **sequentially** (shared `.fake` state, never concurrently); aggregate results
  are recorded as **non-authoritative** unless re-confirmed sequentially.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Every task has a matching `tasks.deps.yml` entry; every line mirrors the
  structured `skillist` via `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory artifacts are present and linked (`spec.md`, `plan.md`, `research.md`, `data-model.md`, `quickstart.md`, `contracts/layout-drift-guard.md`, `checklists/`) and that `.specify/feature.json` resolves `specs/101-layout-dirty-set-guard`
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold audit-discoverable readiness placeholders under `readiness/`: the feature-specific `drift-guard.md`, `category-honoring.md`, `single-source.md`, `r2-preservation.md`, `surface-baseline.md`, `validation-log.md`, plus the audit-enforced `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `evidence-graph.md`, `evidence-audit.md`, and the not-applicable `window-visibility.md`, `real-image-evidence.md`, `visual-evidence-honesty.md` — each naming its authoritative command, artifact path, failure class, and next action using `key=value` lines (not bare image-filename claims); `window-visibility.md` / `real-image-evidence.md` / `visual-evidence-honesty.md` record the **not-applicable** decision with honest values per T003 (no window, no screenshot, byte-identical rendering output — R7 changes no rendering)
- [X] T003 [P] [skillist: []] Record feature **Tier 2 (internal)**, affected layers (`FS.Skia.UI.Controls` — `src/Controls/Control.fs` `toLayout` ↔ `layoutAffectingAttrNames` coupling + the US2 name-token constants; `src/Controls/RetainedRender.fs` `layoutDirtySet` comment correction only; `tests/Controls.Tests/**`; `tests/Layout.Tests/**` re-run), public-API impact (**none** — no public/internal `.fsi` signature change; all new symbols private/test-local), MVU applicability (**N/A** — not stateful/IO; `layoutDriftReport`/probe are pure; Principle IV does not apply), and the evidence obligations from the plan; record as a **visible decision** that the persistent-launch / viewer-launch task-generation rule does **not** apply (no default-exe / persistent-launch entry point; rendering output byte-identical; `window-visibility.md` / `real-image-evidence.md` / `visual-evidence-honesty.md` are not-applicable with honest values) and that FR-009's permanent non-goals are preserved (no data binding, dependency properties, CSS selectors, or template engine)
- [X] T004 [skillist: []] Run `./fake.sh build -t Route` against the working-tree diff and confirm it routes **inner-loop → `Dev` only** (a framework-internal `src/Controls/**` + `tests/**` change with no `.fsi`/template/governance surface move, no new FAKE gate); record the authoritative gate list plus the small/medium/broad governance risk levels into `readiness/governance-risk-levels.md`; note that the feature's evidence obligations additionally run `EvidenceGraph` then `EvidenceAudit` sequentially

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-evidence-mode] Record unsupported-scope, permanent non-goals, and the **FR-008 intrinsic-size-memo deferral** into `readiness/runtime-limitations.md` (research D6 / SC-006): R2 shipped a computed-`Bounds` cache only; the optional intrinsic-size memo named in roadmap §10.4 is **deferred** (no profiled workload shows the fixed-size-ancestor boundary re-measure is hot; adding an un-profiled cache would widen scope and risk the zero-delta guarantee), and the §10.4 wording reconciliation (R2 cached `Bounds` only, memo optional/deferred) is **delegated to R8**; also out of scope — R6 visual-state cross-fade, the R8 doc-narrowing reconciliations (Yoga point-scale rationale, R1/R5 surface notes), collection virtualization, and any expansion of the layout-driving attribute set (R7 guards un-guarded additions, it does not make them)
- [X] T006 [skillist: fs-skia-ui-widgets] Capture the current `FS.Skia.UI.Controls` per-package internal `.fsi.txt` baseline (`PerPackageSurface.captureCurrent`) as the **pre-change reference** for the Phase-6 zero-surface-drift confirmation (SC-005), and confirm `tests/Controls.Tests` already reaches `ControlInternals.layoutAffectingAttrNames` / `evaluateLayout` / `layoutDirtySet` via the existing `InternalsVisibleTo "Controls.Tests"` (no new visibility grant needed)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — a layout input cannot be added without updating the dirty classifier

### Tests First (Principle I, Principle VI)

- [X] T007 [P] [US1] [skillist: fs-skia-testing, fs-skia-layout] Add the failing-first **drift-report** + **probe-gate** suite (`tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs`; fails to compile/red until T009 adds the helpers; SC-001/FR-002/FR-003/FR-007). Negative directions over the pure `layoutDriftReport` with **simulated** sets: `({width;height;padding},{width;height})` → `[Uncovered "padding"]`; `({width},{width;orientation})` → `[OverBroad "orientation"]`; `({a;b},{b;c})` → `[Uncovered "a"; OverBroad "c"]` (both directions, sorted/order-stable); `({width;height;orientation},{width;height;orientation})` → `[]` (shipping state passes); and assert `formatDrift` names **each** attribute **and** its direction in human-legible text (FR-007), empty list → an explicit "no drift" string. Plus the load-bearing **positive gate**: `layoutDriftReport (discoverLayoutDrivingNames size) ControlInternals.layoutAffectingAttrNames = []` exercising the **real** `evaluateLayout` over the representative fixtures + corpus (data-model + research D2). Deterministic, in-process (`Check.One`-style, no repo-absent `testProperty`)
- [X] T008 [P] [US1] [skillist: fs-skia-testing, fs-skia-reconciliation] Add the **FR-004 category-honoring** units asserted directly on the **real** `internal layoutDirtySet prev patch next` (`tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs`; contracts C3): (a) an `Update` whose `AttrChanges` set an `AttrSet { Category = AttrCategory.Layout }` with a name **absent** from `layoutAffectingAttrNames` puts the node id in the dirty set (category channel dirties); (b) an `AttrRemoved` of a name that was `Category = Layout` on the **prev** node dirties (the category-recovered-from-prev edge case); (c) an `AttrSet { Category = Visual }` content/style change does **not** dirty the node (SC-004 — no extra re-measure); and assert the name-set equality gate (T007) operates on **names only** and does **not** demand a category-only attribute appear in `layoutAffectingAttrNames` (the FR-003↔FR-004 independence resolution). These assert existing `layoutDirtySet` behavior (it already reads `attr.Category` independently) so they pin forward-compatibility without changing it

### Implementation

- [X] T009 [US1] [skillist: fs-skia-reconciliation, fs-skia-ui-widgets] Add, **test-local** in `Feature101LayoutDriftGuardTests.fs`, the pure `layoutDriftReport (discovered: Set<string>) (covered: Set<string>) : DriftFinding list` (`DriftFinding = Uncovered of string | OverBroad of string`; exact set-difference both directions, sorted/total/never-throws) and `formatDrift` (human-legible, names attribute + direction; empty → "no drift"), plus the probe seam (`ProbeFixture`, `probeCorpus`, `nameDrivesLayout`, `discoverLayoutDrivingNames`) that toggles each corpus name on representative fixtures and compares the **real** `ControlInternals.evaluateLayout` root `LayoutNode` by structural equality (data-model §probe). `probeCorpus` MUST be built from the **concrete, traceable** source named in research D2 — the `Attr` builder vocabulary + attribute-name literals in `src/Controls/Control.fs`, unioned with `ControlInternals.layoutAffectingAttrNames` and the explicit non-layout names (`background`/`foreground`/`text`/a visual-state class) — **not** a hand-curated free list, so the under-coverage guarantee tracks the real control vocabulary. This makes T007 GREEN (and keeps T008 green; FR-001/FR-002/FR-003/FR-007). Correct the **false single-sourcing comment** at `src/Controls/Control.fs:1207` (and the mirroring note near `layoutDirtySet` in `src/Controls/RetainedRender.fs`) to stop claiming the literal and `toLayout` are single-sourced and instead point at the gate — **no behavior change** to `toLayout`/`layoutDirtySet`/the literal. Document the corpus-bounded coverage boundary at the test site (FR-007 observability)
- [X] T010 [US1] [skillist: fs-skia-evidence-mode] Capture US1 to `readiness/drift-guard.md` and `readiness/category-honoring.md`: the two negative directions named by `formatDrift` (under-coverage `padding`, over-coverage `orientation`); the positive probe gate passing today (discovered = `{width;height;orientation}` = covered) and the named failure it would produce the instant `toLayout` reads an un-covered corpus name; the FR-004 category channel proven independent of the name set; and the documented corpus/fixture discipline that bounds the guarantee (read from the T007/T008 suite, not assumed) (SC-001)

**Checkpoint**: User Story 1 is functional and testable independently.

---

## Phase 4: User Story 2 (US2) — the classifier and the lowering share one definition

### Implementation

- [X] T011 [US2] [skillist: fs-skia-ui-widgets] Add the **name-token single-sourcing** in `src/Controls/Control.fs` (data-model §shared name-token constants): `let [<Literal>] private AttrWidth = "width"`, `AttrHeight = "height"`, `AttrOrientation = "orientation"`, referenced by `nodeWidth`/`nodeHeight` (`hasAttr`), `orientationOf`, **and** `layoutAffectingAttrNames`, so no string literal of a layout-driving name is duplicated — one authoritative token per name (SC-002). These are `private` to the `.fs`: **no `Control.fsi` change**, **no behavior change** (the same three strings, byte-identically), **no** per-package internal surface move expected
- [X] T012 [US2] [skillist: fs-skia-evidence-mode] Capture US2 to `readiness/single-source.md` (SC-002): record that after R7 exactly **one** authoritative definition of each layout-driving attribute name exists (the `[<Literal>]` token), that `nodeWidth`/`nodeHeight`/`orientationOf` and `layoutAffectingAttrNames` all resolve to it with **zero** independent hand-maintained second list, and that the T009 behavioral-probe gate enforces **membership** equality so adding a name to the lowering without the classifier is now impossible to ship — by inspection plus the gate, the "make the comment's claim actually true" outcome

**Checkpoint**: User Story 2 is functional and testable independently.

---

## Phase 5: User Story 3 (US3) — current behavior and determinism are fully preserved

### Tests First (Principle I, Principle VI)

- [X] T013 [P] [US3] [skillist: fs-skia-testing, fs-skia-layout] Re-run, **unchanged**, the existing `tests/Layout.Tests/Feature097IncrementalTests.fs` incremental-≡-full **byte-identity** property over ≥1000 randomized edit sequences and confirm it stays GREEN (R2 INV-1 / SC-004 / FR-005). R7 adds no code on the lowering/classifier path, so this is cited as the preservation proof, not re-implemented; record the result into `readiness/r2-preservation.md`
- [X] T014 [US3] [skillist: fs-skia-testing, fs-skia-reconciliation] Re-run, **unchanged**, the existing `tests/Controls.Tests/Feature097WiringTests.fs` `WorkReductionRecord.RemeasuredNodeCount` assertions for a content-only / style / state / visual-state edit and confirm the re-measure count is **identical** to the pre-R7 baseline (no extra re-measure introduced; SC-003 / FR-006); record the result into `readiness/r2-preservation.md` alongside the T013 incremental-property outcome

**Checkpoint**: User Story 3 is functional and testable independently.

---

## Phase 6: Integration & Polish

- [X] T015 [P] [skillist: fs-skia-ui-widgets] Confirm **zero** surface drift: recapture the `FS.Skia.UI.Controls` per-package internal `.fsi.txt` baseline (`PerPackageSurface.captureCurrent`) and diff vs the T006 pre-change reference — confirm **no** public/internal `.fsi` signature change (the name-token constants are `private`, the report/probe are test-local); if an unintended internal-surface move is detected, recapture and note it explicitly; record to `readiness/surface-baseline.md` (SC-005)
- [X] T016 [skillist: fs-skia-testing] Run exactly the gate `Route` printed (T004) — `./fake.sh build -t Dev` — and confirm the full Controls + Layout Expecto suites are green (the new `Feature101LayoutDriftGuardTests` drift-report + probe-gate + category units, plus the re-run unchanged `Feature097IncrementalTests` and `Feature097WiringTests`); record the aggregate as **non-authoritative** into `readiness/validation-log.md`; rerun any race-like FAKE failure **sequentially** before any product-regression claim; if an aggregate hangs, record the diagnosis in `readiness/aggregate-hang-diagnostics.md` (SC-005)
- [X] T017 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory` + `tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises; record to `readiness/evidence-graph.md`
- [X] T018 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic-propagation + diff-scan; **no** synthetic/stub work, no `[S]`/`[S*]`) or document every `--accept-synthetic` override; record to `readiness/evidence-audit.md` with the verdict token

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. **None planned** — see
the Status Legend rationale (`layoutDriftReport`/`formatDrift` are pure functions
tested over their natural domain; the load-bearing probe gate, the category-
honoring units, and the R2 preservation tests all exercise the **real**
`evaluateLayout` / `layoutDirtySet` / `layoutAffectingAttrNames` and the existing
feature-097 evidence — no mock, fake, stub, placeholder, canned response, or
in-memory substitute for any real dependency exists). For any `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
