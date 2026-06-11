# Tasks: True Visual-State Cross-Fade (R6)

**Feature branch**: `103-visual-state-cross-fade`
**Spec**: `specs/103-visual-state-cross-fade/spec.md`
**Plan**: `specs/103-visual-state-cross-fade/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. **R6 plans zero `[S]`/`[S*]`/`[SEH]` rows** — every proof drives
the real `RetainedRender` assemble path with real `Style.resolve`-painted
snapshots and injected deltas; `EvidenceAudit` MUST report **0 synthetic**.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase). FAKE-backed gate
  tasks are never `[P]`: `./fake.sh` shares `.fake` state and MUST run
  sequentially in the deterministic order printed below.
- **[US1]**, **[US2]**, **[US3]** — user-story scope.
- **[T1]** — Tier 1 (contracted) change. R6 alters observable behavior
  (mid-transition paint) and touches an internal-but-baselined `.fsi`
  (`AnimationClock` gains a field; doc reconciled). The **public**
  `runInteractiveApp`/consumer surface is **unchanged**.

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors its
structured `skillist` via `[skillist: ...]` (`[skillist: []]` when empty).

## Success-criterion → assertion mapping

- **SC-001** (mid-flight strictly-between) → T006 (failing-first) / T010 (green) →
  `readiness/mid-flight-interpolation.md`.
- **SC-002** (at-rest byte-identity, no animation attribute) → T011 → INV-1.
- **SC-003** (final frame == snapped static, all channels) → T011 → INV-2.
- **SC-004** (determinism under injected deltas) → T012 → INV-4.
- **SC-005** (doc↔behavior agreement) → T015 → INV-7.
- **SC-006** (suites green; held state single scoped repaint) → T014 / T018 → INV-6.

## Routing reality (read before validating)

Run `./fake.sh build -t Route` first and run **only** the gates it prints.
Features 101/102 established that **any `src/Controls/**/*.fs` edit — even a pure
comment — escalates `Route` to the `controls-public-surface` gate set**,
regardless of `.fsi` delta. R6 edits `src/Controls/RetainedRender.fs` and
`src/Controls/RetainedRender.fsi`, so **expect escalation**. The internal
`.fsi` field/doc move requires a **per-package** surface baseline recapture
(`PerPackageSurface.captureCurrent`) — `RefreshSurfaceBaselines` does **not**
regenerate per-package snapshots (`[[per-package-baseline-not-in-refresh-target]]`).
FAKE-backed commands run **sequentially** in this order when escalated:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

The actual minimal list is whatever `Route` prints for this diff.

## Governance risk levels

- **Small**: the `RetainedRender.fsi` internal doc-comment reconciliation (FR-009)
  — doc-only, zero public signature delta.
- **Medium / broad**: the `RetainedRender.fs` behavior change (prior-snapshot
  capture + composite) plus the internal `AnimationClock` field, which escalate to
  the controls-public-surface set (feature 101 rule). Focused validation is the
  escalated set above. Broad validation is required only if `Route --enforce`
  names a missing evidence artifact. Non-authoritative aggregate results (e.g. a
  `GeneratedProductCheck` environment failure — `[[generated-product-check-env-failure]]`)
  are recorded as environment-class, not product defects, in `readiness/`.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Confirm the feature directory links spec + plan, then confirm the four root-cause sites are present at the cited lines: `fadeAnimation` (fixed opacity-only `Animation`, `src/Controls/RetainedRender.fs:~94`), `updateClockForState` (state-change detector with no endpoint knowledge, `:~123`), `sampleOnPaint` (opacity-only overlay, `:~153`), and the `AnimationClock` type doc that over-advertises a color channel (`src/Controls/RetainedRender.fsi:~40-51`)
- [X] T002 [P] [T1] [skillist: fs-skia-evidence-mode] Scaffold `specs/103-visual-state-cross-fade/readiness/` audit-enforced placeholders discoverable before implementation — `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `visual-evidence-honesty.md`, `window-visibility.md` (not-applicable — assembly is GPU-free byte-identity/interpolation evidence, no persistent window or screenshots), `real-image-evidence.md` (not-applicable — deterministic scene assembly, no captured images), `at-rest-byte-identity.md`, `final-frame-identity.md`, `mid-flight-interpolation.md`, `determinism.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [T1] [skillist: []] Record feature classification: **Tier 1** (contracted), affected layer `FS.Skia.UI.Controls` (`RetainedRender` internals), public-API impact = **none** (public `runInteractiveApp`/consumer surface unchanged; only the internal `AnimationClock` `.fsi` field + doc), **Principle IV (MVU/effect) not applicable** (no `Model`/`Msg`/`Effect`/`update` added — reuses the feature-099 host-tick → `advance` → assemble seam), and the five evidence obligations (at-rest, final-frame, mid-flight, determinism, graph+audit with 0 synthetic)

---

## Phase 2: Foundation — internal contract + resolved design fork

- [X] T004 [T1] [skillist: fs-skia-reconciliation] Draft the internal `.fsi` change in `src/Controls/RetainedRender.fsi`: the **internal** `AnimationClock` type gains a `From : FS.Skia.UI.Scene.Scene list` prior-snapshot field, and its doc-comment is reconciled to describe the **snapshot-composite** cross-fade and drop the unfulfilled standalone Scene-`Color`-tween claim (FR-009). The **public** surface stays byte-identical
- [X] T005 [P] [T1] [skillist: fs-skia-reconciliation, fs-skia-scene] Confirm the Phase-0 design fork in `research.md`: `Animation.applyAt` **never applies the `Color` tween** (samples opacity/transform only) and a single Scene `Color` tween cannot represent the multi-channel `Foreground`/`Fill`/`Stroke` paint `Style.resolve` produces — so the cross-fade is realized by **compositing the two cached static own-scene snapshots** via the public opacity tween, and record the retarget-on-second-change and doc-reconciliation (FR-009) decisions

**Checkpoint**: Internal contract drafted and the design fork is resolved on paper — implementation may begin.

---

## Phase 3: User Story 1 (US1) — a state transition visibly cross-fades its colors

### Tests First (Principle I, Principle VI)

- [X] T006 [P] [US1] [T1] [skillist: fs-skia-reconciliation, fs-skia-testing] Add the **failing-first** semantic test via the internal `RetainedRender.step` surface (`<InternalsVisibleTo>`): drive a control whose `Style.resolve` output differs between `Normal` and `Hover`/`Focused` in a token-derived color channel through `Normal → Hover` with a fixed injected-delta sequence, sample an intermediate frame, and assert a color channel value lies **strictly between** the prior and next resolved-style endpoints. Red initially (the new appearance only fades in from transparent) (SC-001 / INV-3)

### Implementation

- [X] T007 [US1] [T1] [skillist: fs-skia-reconciliation] Extend `updateClockForState` to capture `From` from the **matched prior retained node's `Fragment.OwnScene`** at transition start, and on a **mid-flight retarget** seed `From` from the previous target's own snapshot with `Elapsed = 0` (FR-001, FR-007)
- [X] T008 [US1] [T1] [skillist: fs-skia-scene, fs-skia-reconciliation] Rebuild `sampleOnPaint` to composite two opacity-driven layers via the **public** `Animation.applyAt`: the `From` prior layer fades out (`1 → 0`) **under** the next own-scene fading in (`0 → 1`); `From = []` degenerates to today's fade-in (a safe degenerate case, not a special path) (FR-002)
- [X] T009 [US1] [T1] [skillist: fs-skia-reconciliation] Thread the prior own-scene snapshot by `RetainedId` through the assemble walk; the composite branch is entered **only** when `clockActive clock` is true (a settled clock paints `ownStatic` verbatim) (FR-002, preserves the settle path for INV-2)
- [X] T010 [US1] [T1] [skillist: fs-skia-evidence-mode] US1 independent test green; write `readiness/mid-flight-interpolation.md` — an intermediate composited color of a both-states-painted region strictly between the `Normal` and `Hover` endpoints (with `Animation.lerpColor` endpoints as the strictly-between reference; mid-flight is animation, not golden) (SC-001 / INV-3)

**Checkpoint**: A live transition genuinely interpolates token-derived paint mid-flight.

---

## Phase 4: User Story 2 (US2) — at-rest and settled output is unchanged

### Tests First

- [X] T011 [P] [US2] [T1] [skillist: fs-skia-reconciliation, fs-skia-testing] Add the byte-identity tests via `RetainedRender.step`: (a) **at-rest** — with no clock in flight the assembled scene equals the cached `SubtreeScene` and **no** animation attribute is emitted (SC-002 / INV-1); (b) **final-frame** — advance a transition past its duration with a large injected delta and assert the frame is byte-identical to the statically snapped render of the new state for **every** animated channel (SC-003 / INV-2)
- [X] T012 [P] [US2] [T1] [skillist: fs-skia-testing] Add the **determinism** test: replay a fixed injected-delta sequence (repo has no `testProperty` — use `Check.One`, `[[feature-099-live-animation-clock]]`) and assert an identical sampled-frame sequence; a non-positive delta is a no-op (never rewinds) and a past-duration delta settles canonically with no overshoot in any channel (SC-004 / INV-4)

### Implementation / verify

- [X] T013 [US2] [T1] [skillist: fs-skia-evidence-mode] Confirm the settle / fast path is **unchanged** so FR-004/FR-005 hold by construction (the cross-fade is an assembly-time overlay gated to mid-flight frames only); write `readiness/at-rest-byte-identity.md`, `readiness/final-frame-identity.md`, and `readiness/determinism.md`
- [X] T014 [US2] [T1] [skillist: fs-skia-reconciliation, fs-skia-testing] Edge-case tests: **no channel differs** collapses the tween to a no-op with no spurious repaint; a **held** state stays a `Keep` after settle (the `Reconcile.attrValueEqual` `VisualStateValue` equality case from feature 099 stays intact — single scoped repaint, not per-frame); a settled **return-to-`Normal`** clock is still **dropped** so the identity returns to byte-identical at-rest output, now also discarding `From` (FR-008, INV-5/INV-6, SC-006)

**Checkpoint**: The two stable points (at-rest, final frame) are byte-identical and the clock is deterministic.

---

## Phase 5: User Story 3 (US3) — the advertised channels and the driven channels agree

- [X] T015 [US3] [T1] [skillist: fs-skia-reconciliation] Read `src/Controls/RetainedRender.fsi`; confirm the reconciled `AnimationClock` doc names **exactly** the channels the implementation drives (the opacity tween + the snapshot composite), that every advertised channel is exercised by a test in this feature, and that the dropped standalone color-tween claim is gone — no doc-advertised channel left undriven (FR-009 / SC-005 / INV-7)

**Checkpoint**: The doc is an accurate map of what a live clock drives.

---

## Phase 6: Integration & Polish

- [X] T016 [T1] [skillist: []] Recapture the **per-package** surface baseline via `PerPackageSurface.captureCurrent` for the moved internal `AnimationClock` `.fsi` field/doc (`RefreshSurfaceBaselines` does **not** regenerate per-package snapshots — `[[per-package-baseline-not-in-refresh-target]]`)
- [X] T017 [T1] [skillist: []] Run `./fake.sh build -t Route` and record the printed tier + minimal gate list in `readiness/generated-validation.md` (expect escalation to `controls-public-surface` per feature 101); run only the gates it prints
- [X] T018 [T1] [skillist: []] Run the routed gate set **sequentially** (deterministic order, no concurrent FAKE); confirm rendering output, the Controls + Elmish suites, and the 099/101 property + unit suites are green and unchanged, the held-state single-repaint invariant holds, and no **public** `.fsi`/surface baseline moved; record the governance risk level, focused validation run, whether broad validation was required, and any non-authoritative aggregate result in `readiness/governance-risk-levels.md` + `readiness/aggregate-hang-diagnostics.md` (SC-006)
- [X] T019 [T1] [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory`/`tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises; write `readiness/evidence-graph.md`
- [X] T020 [T1] [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with **0 synthetic**; write `readiness/evidence-audit.md` with a verdict token and ensure `readiness/generated-validation.md` records `package-resolution=resolved` / `package-mismatch=false`

---

## Synthetic-Evidence Inventory

R6 introduces **no** synthetic evidence. No `[S]`/`[S*]`/`[SEH]` task exists;
every proof drives the real `RetainedRender` assemble path with injected deltas.
`EvidenceAudit` MUST report 0 synthetic. This table is intentionally empty.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
