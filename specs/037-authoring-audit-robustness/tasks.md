# Tasks: Fail-Loud Authoring & Audit Robustness

**Feature branch**: `037-authoring-audit-robustness`
**Spec**: `specs/037-authoring-audit-robustness/spec.md`
**Plan**: `specs/037-authoring-audit-robustness/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed by the evidence audit, never written by hand.
No `[SEH]` is anticipated for this feature: the US1 unresolved-feature path,
the US2 prose/violation fixtures, and the US3 mixed-open compile all use real,
feasible inputs (a renamed `feature.json`, real markdown fixtures, a real
compile), so error behavior is exercised with real evidence rather than
synthetic substitution.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task has a matching entry in `tasks.deps.yml`; every line mirrors its
structured `skillist` as `[skillist: ...]` (`[skillist: []]` when empty).

## Validator pitfall guidance (read before `EvidenceGraph`)

- This feature legitimately edits the evidence graph/audit tooling, so several
  titles carry `EvidenceGraph` / `EvidenceAudit` trigger phrases **and** the
  matching `speckit-evidence-graph` / `speckit-evidence-audit` skill. That is
  intentional, not an accidental trigger.
- Readiness-aggregation tasks that cite required filenames use the exact
  `Complete readiness notes` prefix to suppress capability-expectation checks.
- This is **not** a viewer / persistent-GUI / window-visibility feature. Titles
  and prose deliberately avoid those trigger phrases — and avoid naming the
  viewer/window-visibility/package-resolution readiness filenames verbatim — so
  the audit's conditional GUI/runtime/window scans stay dormant. (Writing those
  filenames literally here would itself trip the scans, which is the very
  prose-substring over-match this feature removes.)
- `tasks.deps.yml` uses one object-shaped entry per task id with indented
  `deps` and `skillist` fields; dependency lists use exact `Tnnn` ids; the
  visible mirror matches the structured list exactly and in order.

## Governance risk level

Medium. The single contract change (US3 `[<RequireQualifiedAccess>]` on
`ControlEventOrigin`) is the focused-validation target — `PackageSurfaceCheck`
plus refreshed surface baselines. Broad validation (the full sequential FAKE
order) is required only at integration because US1/US2 alter the audit/graph
gates themselves. Non-authoritative aggregate results are recorded in
`readiness/logs/` and never treated as a substitute for the focused gates.

## Canonical Verification Targets (FAKE-backed — run sequentially, never concurrently)

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Plus `./fake.sh build -t PackageSurfaceCheck` for the US3 baseline refresh. If a
failure looks race-like, rerun the affected FAKE-backed commands sequentially
before product debugging.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Create placeholder evidence files listed by the plan: scaffold `specs/037-authoring-audit-robustness/readiness/` with `logs/`, `audit-fixtures/`, and `fsi/` subdirectories
- [X] T002 [P] [skillist: []] Record feature Tier (Tier 1, contract change isolated to `ControlEventOrigin`), affected layers (Controls `.fsi`, governance tooling, template), public-contract impact, Elmish/MVU applicability (not applicable — no stateful or I/O-bearing runtime workflow changes), and the evidence obligations from the plan's Evidence Plan

---

## Phase 2: Foundation

- [X] T003 [skillist: []] Complete readiness notes for `readiness/governance-risk-levels.md` naming the small, medium, and broad governance risk levels, the focused validation required for the selected level, when broad validation is required, and required evidence
- [X] T004 [P] [skillist: []] Complete readiness notes for `readiness/aggregate-hang-diagnostics.md` recording verdict, stage, elapsed duration, last observed command, focused rerun, and the non-authoritative aggregate policy
- [X] T005 [P] [skillist: []] Complete readiness notes for `readiness/runtime-limitations.md` covering .NET 10 desktop, Vulkan, SkiaSharp preview, unsupported macOS/mobile/browser, and no software-renderer fallback
- [X] T006 [skillist: []] Confirm the three contract files (`contracts/audit-status-region-contract.md`, `contracts/control-event-origin-contract.md`, `contracts/fsi-load-script-contract.md`) capture the authoritative-region grammar and deterministic resolution rule, the `.fsi` surface delta and baseline lines, and the generated `.fsx` shape and in-sync derivation

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) — audit never silently passes against the wrong feature [P1]

### Tests First (Principle I, Principle VI)

- [X] T007 [P] [US1] [skillist: speckit-evidence-graph, speckit-evidence-audit] Add a feature-resolution test (failing-first): a real `feature.json` resolves the active feature and reports its real task count, while an unreadable/empty/missing `feature.json` produces a non-zero/blocking result — asserting the current hardcoded fallback is gone (FR-001, FR-002, FR-003)

### Implementation

- [X] T008 [US1] [skillist: speckit-evidence-graph, speckit-evidence-audit] Remove the hardcoded `"007-v2-template-packaging"` fallback in `build.fsx` `activeFeatureId`; resolve authoritatively from `.specify/feature.json`; on missing/unreadable/empty, hard-fail with a prominent non-suppressible warning naming the expected source (FR-001, FR-002)
- [X] T009 [US1] [skillist: speckit-evidence-graph] Echo the resolved feature id and real task count from `compute-task-graph.py`, surface any recorded-feature-vs-scanned-directory mismatch in the output, and when the resolved feature's task file is empty or unparseable, report that explicitly (non-zero/blocking) rather than falling back to a stub count (FR-003, US1 scenario 3, spec Edge Cases)
- [X] T010 [US1] [skillist: speckit-evidence-audit] Align `.specify/scripts/bash/common.sh` `get_feature_paths` resolution order (env override → `feature.json` → branch-prefix) so the no-real-feature state is terminal-fail, never a stub fallback (FR-002)

### Evidence

- [X] T011 [US1] [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `EvidenceGraph` then `EvidenceAudit`; record the resolved id and real task count in `readiness/feature-resolution.md` and `readiness/logs/evidence-graph.txt` / `readiness/logs/evidence-audit.txt`, plus a transcript of the unresolved-feature run showing the non-zero exit and warning, and a transcript of the empty/unparseable-task-file run showing the explicit report (no stub fallback)

**Checkpoint**: US1 is independently testable — real feature resolves to real task count; unresolved hard-fails.

---

## Phase 4: User Story 2 (US2) — audit blocks only on real violations, not prose mentions [P1]

### Tests First

- [X] T012 [P] [US2] [skillist: speckit-evidence-audit] Add `readiness/audit-fixtures/prose-negation-clean.md` (blocker terms only inside prose/negation/illustrative text, plus a clean `audit-status` region) and `readiness/audit-fixtures/genuine-violation.md` (a violating value inside the region); demonstrate failing-first that the prose fixture blocks under today's substring scanner (FR-004)
- [X] T013 [US2] [skillist: speckit-evidence-audit] Add verification asserting the prose fixture resolves to PASS after the fix and the genuine-violation fixture BLOCKS both before and after (no true-positive regression) (FR-006)

### Implementation

- [X] T014 [US2] [skillist: speckit-evidence-audit] Restrict machine-readable status reads in `run-audit.sh` to the designated `audit-status` fenced region; drop the bare substring blockers (`taskbar-only` / `mismatch` / `nu1603` in text); first declared region wins, a duplicate key within it is a surfaced parse error, and a malformed key/value (present but unparseable) is surfaced as a parse error rather than silently treated as passing or failing (FR-004, FR-005, spec Edge Cases)
- [X] T015 [US2] [skillist: speckit-evidence-graph, speckit-evidence-audit] Document the deterministic resolution rule (authoritative region wins; prose never read) in the `speckit-evidence-graph` and `speckit-evidence-audit` SKILL docs and their synchronized `.agents/skills` peers (FR-005)

### Evidence

- [X] T016 [US2] [skillist: speckit-evidence-audit] Audit both fixtures and record prose→PASS and genuine→BLOCK results in `readiness/logs/evidence-audit.txt`, plus the duplicate-key parse-error, malformed-key parse-error, and prose-bullet-does-not-override checks (US2 scenarios 2 & 4, spec Edge Cases)

**Checkpoint**: US2 is independently testable — prose/negation fixtures pass; genuine violations still block.

---

## Phase 5: User Story 3 (US3) — mixed Scene/Controls resolves predictably [P2] [T1]

### Tests First

- [X] T017 [P] [US3] [skillist: fs-skia-scene, fs-skia-ui-widgets] Add a mixed Scene/Controls compile fixture under `readiness/fsi/` that opens `FS.Skia.UI.Scene` then `FS.Skia.UI.Controls` (Controls last) and constructs an unqualified scene text node plus a bounds literal; failing-first, it reproduces the opaque `ControlEventOrigin` error pre-fix (FR-007, SC-004)

### Implementation

- [X] T018 [US3] [T1] [skillist: fs-skia-ui-widgets] Add `[<RequireQualifiedAccess>]` to `ControlEventOrigin` in `src/Controls/Types.fs` and the matching `src/Controls/Types.fsi`, and qualify any repo usages of its unqualified cases so the `Text` case stops shadowing the scene text construct (FR-007)
- [X] T019 [US3] [skillist: fs-skia-scene, fs-skia-ui-widgets] Document the predictable pattern for shared structurally-typed types (reuse the shared bounds type to avoid record-field inference hijack) at the point of use in authoring guidance (FR-008)
- [X] T020 [US3] [T1] [skillist: fs-skia-template-update] Refresh `readiness/surface-baselines/FS.Skia.UI.Controls.txt` and `FS.Skia.UI.txt` via `scripts/refresh-surface-baselines.fsx` and confirm `./fake.sh build -t PackageSurfaceCheck` passes with the qualified-access marker
- [X] T021 [US3] [skillist: []] Record the spec 035 reversal and rationale in `specs/035-api-discovery-names/readiness/name-collision-safety.md` — guidance-over-attributes is reversed for `ControlEventOrigin` only — and document the consumer compatibility impact + migration guidance: code referencing `ControlEventOrigin` cases unqualified (`Text`, `Pointer`, …) must now qualify them (`ControlEventOrigin.Text`), with a before/after snippet (FR-010; public-API changes document compatibility impact and migration guidance)

### Evidence

- [X] T022 [US3] [skillist: fs-skia-scene, fs-skia-ui-widgets] Build with `./fake.sh build -t Dev`, compile the mixed-open fixture, and record the transcript under `readiness/fsi/` confirming it resolves to the scene construct (or fails naming the colliding symbols) — never the opaque error (SC-004)

**Checkpoint**: US3 is independently testable — the previously-failing open order now compiles or fails with an actionable diagnostic.

---

## Phase 6: User Story 4 (US4) — load a generated app into FSI from a documented entry point [P3]

### Tests First

- [X] T023 [P] [US4] [skillist: fs-skia-template-update] Add a generated-product expectation that the emitted `.fsx` load script appears in the generated file list and references the app plus its transitive `FS.Skia.UI.*` set (FR-009)

### Implementation

- [X] T024 [US4] [skillist: fs-skia-template-update] Emit the generated `.fsx` load script from `template/base/` via `GenerateV3Products` in `build.fsx`, derived from the pinned `Directory.Packages.props` set and the generated `Product` output assembly so it stays in sync without being a hand-maintained reference list (FR-009)
- [X] T025 [US4] [skillist: fs-skia-template-update] Register the new `.fsx` in `.template.config/template.json` generated content and add the FSI-load entry-point guidance to `template/base/README.md` and `template/base/docs/product.md`
- [X] T026 [US4] [skillist: fs-skia-template-update, fs-skia-layout-evidence] Preserve benign host-warning classification on the load path per the spec 021 host-warning contract — benign headless/host warnings stay classified benign while real failures stay fatal
- [X] T027 [US4] [skillist: fs-skia-template-update] Generate products, run `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` (sequential), run the emitted `.fsx` in FSI for a generated app, and record the transcript in `readiness/fsi-load-script.md` showing zero manual reference edits (SC-005)

**Checkpoint**: US4 is independently testable — one documented step loads a generated app into FSI.

---

## Phase 7: Integration & Polish

- [X] T028 [skillist: []] Run the full sequential FAKE validation order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`), plus `PackageSurfaceCheck`, and record the non-authoritative aggregate results in `readiness/logs/`
- [X] T029 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and that the resolved feature id and real task count are echoed
- [X] T030 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS (no false blocks on the prose fixture; the genuine-violation fixture still blocks) or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — all evidence is real per the plan's Synthetic-evidence decision)_ | | | | | | | | |
