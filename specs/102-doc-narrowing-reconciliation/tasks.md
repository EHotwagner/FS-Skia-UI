# Tasks: Documented-Narrowing Reconciliation (R8)

**Feature branch**: `102-doc-narrowing-reconciliation`
**Spec**: `specs/102-doc-narrowing-reconciliation/spec.md`
**Plan**: `specs/102-doc-narrowing-reconciliation/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. **R8 expects zero `[S]`/`[S*]`/`[SEH]` rows** — it is a pure
documentation/internal-comment honesty pass that introduces no synthetic
evidence; `EvidenceAudit` MUST report **0 synthetic**.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase). FAKE-backed gate
  tasks are never `[P]`: `./fake.sh` shares `.fake` state and MUST run
  sequentially in the deterministic order printed below.
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T2]** — Tier 2 (internal/documentation) change. Per the constitution and
  plan, R8 is wholly Tier 2: no public `.fsi` surface is added, removed, or
  modified under the recorded default choices (annotate, document).

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors its
structured `skillist` via `[skillist: ...]` (`[skillist: []]` when empty).

## Reconciliation decisions (recorded up front — SC-006)

- **FR-002 → annotate (option b), not remove.** Lowest-risk, zero-surface-delta;
  the public `deriveVisualState` is exercised by tests that may seed a
  `Selection`, so annotation guarantees no test moves. Recorded in T004.
- **FR-005 → document/annotate, not drop and not enable routing.** Keeps
  arrow-key behavior for `Chart`/`Graph`/`Progress` byte-identical (FR-008 wins
  the conflicting-requirements note). Recorded in T005.

## Routing reality (read before validating)

Run `./fake.sh build -t Route` first and run **only** the gates it prints.
Feature 101 (R7) established that **any `src/Controls/**/*.fs` edit — even a
pure comment — escalates `Route` to the `controls-public-surface` gate set**,
regardless of whether a `.fsi` changes. R8 touches
`src/Controls/ControlRuntime.fs`, `src/Controls/Focus.fs`, and
`src/Controls/Control.fs`, so **expect escalation**. No public-surface baseline
recapture is required under the recorded default choices (no `.fsi` signature
moves). FAKE-backed commands run **sequentially** in this order when escalated:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

## Governance risk levels

- **Small**: roadmap prose edits (`docs/reports/…-roadmap.md`) and the
  `src/Layout/Layout.fs` comment — route per `Route` (doc subtree / inner-loop).
- **Medium / broad**: the `src/Controls/**/*.fs` comment edits, which escalate to
  the controls-public-surface set (feature 101 rule). Focused validation is the
  escalated set above. Broad validation is required only if `Route --enforce`
  names a missing evidence artifact. Non-authoritative aggregate results (e.g.
  `GeneratedProductCheck` environment failures) are recorded as
  environment-class, not product defects, in `readiness/`.

---

## Phase 1: Setup

- [X] T001 [T2] [skillist: []] Confirm the feature directory links spec + plan and the working tree matches the plan's verified-source-sites table (six cited narrowings present at the cited lines)
- [X] T002 [P] [T2] [skillist: []] Scaffold `specs/102-doc-narrowing-reconciliation/readiness/` audit-enforced placeholders discoverable before implementation: `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `window-visibility.md` (not-applicable — non-visual, no screenshots), `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [T2] [skillist: []] Record feature classification: Tier 2 (internal/documentation), affected layers `FS.Skia.UI.Controls` + `FS.Skia.UI.Layout` + repo report, public-API impact = none (zero `.fsi` delta under default choices), **Principle IV (MVU/effect) is not applicable** (no `Model`/`Msg`/`Effect`/`update` added or altered), and evidence obligations = routed gate set green + parity/golden unchanged + existing suites unchanged + `EvidenceGraph`/`EvidenceAudit` with 0 synthetic

---

## Phase 2: Foundation — record the two reconciliation decisions

- [X] T004 [T2] [skillist: []] Record the FR-002 decision = **annotate (not remove)** in `research.md`/`plan.md` with rationale (signature finding: the dead `Selected` branch drops no parameter, so removal would also be zero-`.fsi`-delta; annotate chosen as lowest-risk so no `deriveVisualState` test moves) — SC-006
- [X] T005 [T2] [skillist: []] Record the FR-005 decision = **document/annotate (not drop, not enable routing)** in `research.md`/`plan.md` with rationale (FR-008 banner constraint wins; enabling default `NavRange`s would move a parity row and is out of R8 scope) — SC-006
- [X] T006 [P] [T2] [skillist: []] Confirm each of the six cited sites against the working tree and record the verification in `research.md` (roadmap §10.3/§10.4 + "segmented"; `ControlRuntime.fs` dead branch; `Layout.fs` Yoga comment; `Focus.fs` value-role branch; `Control.fs:1131` preview id)

**Checkpoint**: Decisions recorded — reconciliation edits may begin.

---

## Phase 3: User Story 1 (US1) — the roadmap report matches what ships

- [X] T007 [P] [US1] [T2] [skillist: []] FR-001: reconcile roadmap §10.3 to describe `deriveVisualState` as realizing only the 5-level runtime tail (`Pressed > Selected > Focused > Hover > Normal`), attributing the head semantic states and consumer-out-ranks-derived arbitration to `applyRuntimeVisualState` (the two-function split the `.fsi` already documents)
- [X] T008 [P] [US1] [T2] [skillist: []] FR-003: reconcile roadmap §10.4 to describe the shipped R2 cache — a computed-`Bounds` cache keyed by structural `LayoutNodeId` — and remove the "intrinsic-size memo keyed by retained identity" claim, cross-referencing feature 101's recorded intrinsic-size-memo deferral (FR-008 of 101)
- [X] T009 [P] [US1] [T2] [skillist: []] FR-006: correct every roadmap "segmented" selection-role mention (`:938`, `:1041`) to name the `AccessibilityRole`s that actually exist (no nonexistent `Segmented` role implied)
- [X] T010 [US1] [T2] [skillist: []] US1 independent test: read each reconciled roadmap section against its cited source lines and confirm zero remaining prose-vs-implementation contradiction for the three report items (SC-002); record the diff as the reconciliation evidence

**Checkpoint**: The roadmap report is an accurate map of the shipped code.

---

## Phase 4: User Story 2 (US2) — in-source narrowings are honest at the point of use

- [X] T011 [P] [US2] [T2] [skillist: []] FR-002b: annotate the dead `Selected`-from-`Selection` derivation in `src/Controls/ControlRuntime.fs` (`deriveVisualState`, branch at `:206-207`) as forward-looking, stating the live host (`ControlsElmish`) does not populate `Selection`, so only consumer-set `Selected` fires today — annotation only, no logic change
- [X] T012 [P] [US2] [T2] [skillist: []] FR-004: add the maintainer's blast-radius approval rationale ("blast-radius nil, Controls integer geometry unaffected") to the Yoga point-scale-rounding disable comment in `src/Layout/Layout.fs:7-12`, alongside the existing INV-1 correctness motive
- [X] T013 [P] [US2] [T2] [skillist: []] FR-005: annotate the `navIntentFor` `Chart`/`Graph`/`Progress` value-role branch in `src/Controls/Focus.fs:123-129` as classed-but-not-routed-by-default (because `Accessibility.defaultFor` gives those roles no `NavRange`) — note only, routing unchanged
- [X] T014 [P] [US2] [T2] [skillist: []] FR-007: annotate the residual `Key ?? Kind` at `src/Controls/Control.fs:1131` as the legacy 080 single-control **preview** path, distinct from the R3-unified `Key ?? path` dispatch/recovery id (feature 098), so it is not mistaken for the divergence R3 removed
- [X] T015 [US2] [T2] [skillist: []] FR-010: verify every comment/annotation added in T011–T014 is purely descriptive and carries no gate-significant token or literal evidence filename that could trip the window-visibility or diff-scan audits
- [X] T016 [US2] [T2] [skillist: []] US2 independent test: grep each cited site and confirm the annotation is present and accurate (SC-001) — each independently inspectable; record the source diffs as evidence

**Checkpoint**: Every in-source narrowing is honest at the point of use.

---

## Phase 5: User Story 3 (US3) — zero behavior change — & evidence

- [X] T017 [US3] [T2] [skillist: []] Run `./fake.sh build -t Route` and record the printed tier + minimal gate list in `readiness/generated-validation.md` (expect escalation to controls-public-surface per feature 101); run only the gates it prints
- [X] T018 [US3] [T2] [skillist: []] Run the routed gate set **sequentially** (deterministic order, no concurrent FAKE); confirm rendering output, parity/golden evidence, and the R1/R2/R4/R5 property + unit suites (Controls / Elmish / Layout) are green and unchanged, and that no public `.fsi`/surface baseline moved (SC-003, SC-005). A moved or edited test is a red flag that a comment was parsed as a behavior token (FR-010) — investigate, do not accept
- [X] T019 [US3] [T2] [skillist: []] Confirm arrow-key routing for `Chart`/`Graph`/`Progress` is unchanged (still not routed by default) — the existing navigation suite passes without modification (SC-004)
- [X] T020 [T2] [skillist: []] Record the governance risk level, the focused validation run for it, whether broad validation was required, and any non-authoritative aggregate result (e.g. `GeneratedProductCheck` environment-class failure) in `readiness/governance-risk-levels.md` + `readiness/aggregate-hang-diagnostics.md`
- [X] T021 [T2] [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory`/`tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises; write `readiness/evidence-graph.md`
- [X] T022 [T2] [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with **0 synthetic**; write `readiness/evidence-audit.md` with a verdict token and ensure `readiness/generated-validation.md` records package-resolution=resolved / package-mismatch=false

---

## Synthetic-Evidence Inventory

R8 introduces **no** synthetic evidence. No `[S]`/`[S*]`/`[SEH]` task exists;
`EvidenceAudit` MUST report 0 synthetic. This table is intentionally empty.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
