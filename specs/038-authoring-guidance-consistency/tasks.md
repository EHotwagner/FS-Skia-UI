# Tasks: Authoring Guidance Consistency

**Feature branch**: `038-authoring-guidance-consistency`
**Spec**: `specs/038-authoring-guidance-consistency/spec.md`
**Plan**: `specs/038-authoring-guidance-consistency/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

**Path convention:** `readiness/surface-baselines/…` is the **repo-root**
baseline tree (shared, per-module, committed). All other `readiness/…` paths in
this file are under `specs/038-authoring-guidance-consistency/readiness/`
(this feature's evidence). Surface baselines are never written under the
feature's evidence directory.

The `[S*]` marker is computed by the evidence audit, never written by hand.
No `[SEH]` is anticipated for this feature: the US1 dangling/drift/peer-mismatch
fixtures, the US2 missing/drifted-reference check, the US3 mixed-`open` compile,
the US4 generated-project scan, the US6 constructor compile, and the FR-011
filename-mention fixture all use real, feasible inputs (real markdown/skill
fixtures, real compiles, real generated-project runs), so every error behavior
is exercised with real evidence rather than synthetic substitution.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US6]** — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task has a matching entry in `tasks.deps.yml`; every line mirrors its
structured `skillist` as `[skillist: ...]` (`[skillist: []]` when empty).

## Validator pitfall guidance (read before `EvidenceGraph`)

- **This is NOT a graphical viewer feature.** US3 hardens the *naming/visibility*
  of the viewer/input/Elmish public surface (`[<RequireQualifiedAccess>]`); it
  changes no runtime, window, or rendering behavior. Titles deliberately avoid
  the audit's GUI/window trigger wording (persistent-viewer-launch and
  window-visibility-validation phrasing, and persistent graphical-window
  runtime phrasing) so the audit's conditional GUI/window scans stay dormant —
  and no persistent-launch task is required or generated.
- The US3/US6 contract changes are Tier 1 (`.fsi` surface): the focused
  validation target is `PackageSurfaceCheck` plus refreshed surface baselines.
- US1 work is the *skill-id* resolution guard (advertised id ↔ declared `name:`
  ↔ directory, plus `.agents`↔`.claude` peer sync). It is unrelated to the task
  DAG / `tasks.deps.yml` validator; titles avoid that validator's trigger
  vocabulary.
- FR-011 (Phase 9) legitimately touches the evidence gates, so its titles carry
  `EvidenceGraph`/`EvidenceAudit` trigger phrases **and** the matching
  `speckit-evidence-graph` / `speckit-evidence-audit` skills. That is
  intentional, not an accidental trigger.
- `tasks.deps.yml` uses one object-shaped entry per task id with indented `deps`
  and `skillist` fields; dependency lists use exact `Tnnn` ids; the visible
  mirror matches the structured list exactly and in order.

## Priority & consumer-precedence note

Per the spec's Priority Principle and Clarifications, the generated consumer
project has absolute priority (SC-001 governing). Consumer-facing stories
(US1–US6) precede the framework-repo dev-process item (FR-011, Phase 9), which
is P3 and must never block or delay any consumer deliverable. The dangling
`speckit-debug-loop` hint removal (FR-001) is folded into US1 because the
resolution guard cannot pass while it dangles.

Scheduling invariant: although the FR-001 debug-loop removal is folded forward
into US1 (P1) for the practical reason above, it is a single P3 framework-repo
edit that neither produces nor gates any consumer artifact. The P3
feature-targeting guard (FR-011, Phase 9) is **not** a dependency of the SC-001
governing integration task (T036); T036 depends only on the consumer stories
(US1–US6). FR-011 is exercised solely by the final evidence chain (T035 →
EvidenceGraph/EvidenceAudit), so no P3 item can block or delay a consumer
deliverable.

## Principle IV (MVU/effect boundary) applicability

**Not applicable.** This feature introduces no stateful workflow, command,
effect, subscription, or interpreter *behavior*. FR-009 (US5) *documents* the
existing effects boundary; FR-008 (US3) is a naming/visibility change; FR-010
(US6) adds pure scene constructors. No `Model`/`Msg`/`Effect` contract tasks are
generated.

## Governance risk level

**Medium.** The contract changes (US3 RQA on `ViewerWindowStartupState` and the
enumerated `update`/`init` surfaces; US6 additive scene constructors) are the
focused-validation targets — `PackageSurfaceCheck` plus refreshed surface
baselines. Broad validation (the full sequential FAKE order) is required only at
integration because US2/US4/US5 alter generated output that
`GeneratedGuidanceCheck`/`TemplateCheck`/`GeneratedProductCheck` assert.
Non-authoritative aggregate results are recorded in `readiness/logs/` and never
treated as a substitute for the focused gates.

## Canonical Verification Targets (FAKE-backed — run sequentially, never concurrently)

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Plus `./fake.sh build -t PackageSurfaceCheck` for the US3/US6 `.fsi` baseline
refreshes. If a failure looks race-like, rerun the affected FAKE-backed commands
sequentially before product debugging.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Create placeholder evidence files listed by the plan: scaffold `specs/038-authoring-guidance-consistency/readiness/` with `logs/`, `skill-resolution-fixtures/`, and `fsi/` subdirectories, plus empty placeholders for `skill-resolution.md`, `generated-api-reference.md`, `name-collision-migration.md`, `generated-guidance.md`, `effects-boundary.md`, and `feature-targeting-regression.md`
- [X] T002 [P] [skillist: []] Record feature Tier (Tier 1 — contract changes isolated to `ViewerWindowStartupState`/viewer-input `update`/`init` surfaces and additive `Scene` constructors), affected layers (`src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput`, `src/Scene` `.fsi`; governance tooling; template/generated output), public-contract impact, Elmish/MVU applicability (not applicable — no stateful/I-O runtime behavior change), and the evidence obligations from the plan's Evidence Plan

---

## Phase 2: Foundation

- [X] T003 [skillist: []] Complete readiness notes for `readiness/governance-risk-levels.md` naming the small, medium, and broad governance risk levels, the focused validation required for the selected level, when broad validation is required, and the non-authoritative aggregate policy
- [X] T004 [P] [skillist: []] Complete readiness notes for `readiness/aggregate-hang-diagnostics.md` recording verdict, stage, elapsed duration, last observed command, focused rerun, and the non-authoritative aggregate policy
- [X] T005 [P] [skillist: []] Complete readiness notes for `readiness/runtime-limitations.md` covering .NET 10 desktop, Vulkan, SkiaSharp preview, unsupported macOS/mobile/browser, and no software-renderer fallback
- [X] T006 [skillist: []] Confirm the six contract files (`contracts/skill-resolution-contract.md`, `generated-api-reference-contract.md`, `name-collision-hardening-contract.md`, `generated-guidance-contract.md`, `effects-boundary-contract.md`, `scene-constructor-contract.md`) each name the exact rule, the failing-first fixture, and the FR/SC they satisfy
- [X] T007 [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Enumerate the collision-prone public-name set per research R3 (`ViewerWindowStartupState.Normal`, plus every `update`/`init`-bearing viewer/Elmish/input surface a consumer could `open` into collision) and record, for each, whether it is already module-qualified or needs `[<RequireQualifiedAccess>]`, together with the surface-baseline files to refresh (`FS.Skia.UI.SkiaViewer.txt`, `FS.Skia.UI.KeyboardInput.txt`, merged `FS.Skia.UI.txt`, plus `FS.Skia.UI.Elmish.txt` if and only if the Elmish surface is hardened), and explicitly record the Elmish decision (hardened vs already module-qualified, no change) so its baseline is added or intentionally omitted

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) — every advertised skill id resolves [P1]

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: []] Add a failing-first id-resolution check with three fixtures under `readiness/skill-resolution-fixtures/`: a dangling advertised id, a skill whose directory and declared `name:` disagree, and an `.agents`↔`.claude` peer that declares a different `name:`; demonstrate the guard FAILS on each, naming the offending id and the advertising file:line (FR-001, FR-002, FR-003, SC-007)

### Implementation

- [X] T009 [US1] [skillist: []] Implement the resolution guard in `build.fsx` `GeneratedGuidanceCheck`: build the advertised-id set from the repo-file inputs — the hint/scan-phrase lines in `speckit-tasks/SKILL.md` (both `.agents` and `.claude` copies) — resolve each against the declared `name:` of every skill under `src/*/skill`, `.agents/skills/*`, `.claude/skills/*`, and `template/fragments/*/skill`, and fail on any unresolved id, any directory/`name:`/advertised-id disagreement, or any `.agents`↔`.claude` peer drift. The guard reads only repository files; the runtime "available skills" harness surface is not an input because a FAKE target cannot enumerate it (FR-001, FR-002, FR-003)
- [X] T010 [US1] [skillist: fs-skia-template-update] Extend the guard so the skills generated into a consumer project are validated the same way (advertised id ↔ declared `name:` ↔ directory), covering the edge case where an id resolves in this repo but not in the skill set a generated project receives (FR-002, spec Edge Cases)
- [X] T011 [US1] [skillist: []] Remove the dangling `speckit-debug-loop` reference everywhere it is advertised in the hints/scan phrases — `.agents/skills/speckit-tasks/SKILL.md` and its synchronized `.claude/skills/speckit-tasks/SKILL.md` peer (no such skill `name:` exists to repoint to) — so the resolution guard passes on the corrected repository (FR-001, SC-007)
- [X] T012 [US1] [skillist: []] Run `./fake.sh build -t GeneratedGuidanceCheck`; record the PASS on the corrected repository and the FAIL transcripts against each fixture in `readiness/skill-resolution.md`, including the `.agents`↔`.claude` peer-comparison output (SC-007, FR-003)

**Checkpoint**: US1 is independently testable — every advertised id resolves; the guard fails on an introduced dangling/drifted/peer-mismatched id.

---

## Phase 4: User Story 2 (US2) — read the API without reflecting DLLs [P1]

### Tests First

- [X] T013 [P] [US2] [skillist: fs-skia-template-update] Add a generated-project expectation (failing-first) that `docs/api-surface/` is present and contains the real `.fsi` signatures for every package the profile consumes, and that a referenced package's signatures missing or drifted from `src/.../*.fsi` source FAILS the check (FR-004, SC-002)

### Implementation

- [X] T014 [US2] [skillist: fs-skia-template-update] Emit the `docs/api-surface/` tree at generation time in `build.fsx` (`runGenerateV3Products`/`generateV3Product`), copying the real public `.fsi` files verbatim, selected per profile from `capabilities.yml` `contracts:` for each capability the profile includes, so the bundled signatures stay in lockstep with source and are never hand-maintained (FR-004)
- [X] T015 [US2] [skillist: fs-skia-template-update] Register the new `docs/api-surface/` content in `.template.config/template.json` and assert it in `TemplateCheck`/`GeneratedGuidanceCheck`, failing loudly when a consumed package's signatures are absent or drift from source (FR-004, FR-005)
- [X] T016 [US2] [skillist: fs-skia-template-update] Generate a project, read a union case's exact field order (e.g. `SceneNode.Rectangle`) from the bundled `docs/api-surface/` with zero DLL reflection, and record it in `readiness/generated-api-reference.md` (SC-002)

**Checkpoint**: US2 is independently testable — a freshly generated project carries a local, reflection-free, authoritative API reference.

---

## Phase 5: User Story 3 (US3) — names do not collide on `open` [P2] [T1]

### Tests First

- [X] T017 [P] [US3] [skillist: fs-skia-skiaviewer] Add a consumer compile fixture under `readiness/fsi/` that `open`s the viewer namespace and defines its own `Normal` case plus `update`/`init` bindings; failing-first, it must FAIL to compile (collision) before the hardening (FR-008, SC-003)

### Implementation

- [X] T018 [US3] [T1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Add `[<RequireQualifiedAccess>]` to `ViewerWindowStartupState` in `src/SkiaViewer/SkiaViewer.fs`/`.fsi` and apply the consistent hardening (RQA or confirmed module-qualification) to the enumerated `update`/`init`-bearing viewer/Elmish/input surfaces from T007 in `src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput` `.fs`/`.fsi`, qualifying any repo usages so the surface compiles (FR-008)
- [X] T019 [US3] [T1] [skillist: fs-skia-template-update] Refresh `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`, `FS.Skia.UI.KeyboardInput.txt`, `FS.Skia.UI.Elmish.txt` (only if T007 concludes the Elmish `update`/`init` surface is hardened rather than already module-qualified), and the merged `FS.Skia.UI.txt` via `scripts/refresh-surface-baselines.fsx` and confirm `./fake.sh build -t PackageSurfaceCheck` passes with the qualified-access markers
- [X] T020 [US3] [skillist: fs-skia-skiaviewer, fs-skia-template-update] Record the migration note and version-bump intent in `readiness/name-collision-migration.md` (before/after `open` snippet; consumers referencing the affected names unqualified must now qualify them) and update all generated samples so a freshly generated project compiles with the clean, non-colliding surface (FR-008, SC-003)
- [X] T021 [US3] [skillist: fs-skia-skiaviewer] Build with `./fake.sh build -t Dev`, recompile the consumer fixture, and record the before-FAIL / after-PASS transcript in `readiness/fsi/` confirming the consumer's `Normal`/`update`/`init` resolve to the consumer's definitions (SC-003)

**Checkpoint**: US3 is independently testable — a consumer's `Normal`/`update`/`init` no longer collide after `open`; baselines and migration note are recorded.

---

## Phase 6: User Story 4 (US4) — consumer-facing, domain-agnostic generated guidance [P2]

### Tests First

- [X] T022 [P] [US4] [skillist: fs-skia-template-update, fs-skia-layout-readability] Add a failing-first generated-project scan asserting zero demo-specific identifiers (`tetris`, `score`, `level`, `next piece`, `board`, `piece`) in the starter app + tests, ≥1 consumer-runnable usage snippet in each generated skill, and zero generated references to framework-only paths/targets (`CapabilityCheck`, `PackLocal`, `src/.../X.fsi`) (FR-005, FR-006, FR-007, SC-004)

### Implementation

- [X] T023 [US4] [skillist: fs-skia-template-update, fs-skia-layout-readability] Neutralize the demo-specific identifiers in `template/base/src/Product/Model.fs`, `View.fs`, `EvidenceCommands.fs`, `LayoutEvidence.fs`, and `template/base/tests/Product.Tests/Tests.fs`, replacing them with domain-agnostic equivalents while preserving the generic game-starter shape (HUD region, gameplay region, primary-interaction counter) so `fs-skia-layout-readability` stays meaningful (FR-007, SC-004)
- [X] T024 [US4] [skillist: fs-skia-template-update, fs-skia-scene] Add at least one consumer-runnable usage snippet (scene construction, host wiring, or evidence production) to each generated skill under `template/fragments/*/skill/SKILL.md` (and matching `README.md`), and remove every reference to framework-only paths/build targets absent from a generated consumer project (FR-005, FR-006)
- [X] T025 [US4] [skillist: fs-skia-template-update] Generate a project, run `./fake.sh build -t GeneratedGuidanceCheck` then `./fake.sh build -t GeneratedProductCheck` (sequential), and record zero demo ids, zero framework-only paths, and ≥1 runnable snippet in `readiness/generated-guidance.md` (SC-004)

**Checkpoint**: US4 is independently testable — generated starter tests are domain-agnostic and generated skills are consumer-facing and runnable.

---

## Phase 7: User Story 5 (US5) — canonical effects page [P3]

### Tests First

- [X] T026 [P] [US5] [skillist: fs-skia-template-update] Add a failing-first generated-project expectation that a single `docs/effects-boundary.md` is present and self-contained (names both effect categories, the boundary, and the `update`→host wiring) before authoring it (FR-009, SC-005)

### Implementation

- [X] T027 [US5] [skillist: fs-skia-elmish, fs-skia-skiaviewer, fs-skia-template-update] Author `template/base/docs/effects-boundary.md` describing both effect categories (application commands at the MVU edge vs viewer effects at the host boundary), the boundary, and the canonical `update`→host wiring (`Viewer.runApp viewerOptions generatedHost`); bundle it via `.template.config/template.json`; and repoint `docs/reports/generated-apps.md` to this single canonical page (FR-009)
- [X] T028 [US5] [skillist: fs-skia-template-update] Generate a project, confirm `docs/effects-boundary.md` is reachable and the wiring matches how the generated project wires effects, and record it in `readiness/effects-boundary.md` (SC-005)

**Checkpoint**: US5 is independently testable — one canonical effects page is reachable from a generated project without scattered reports/source.

---

## Phase 8: User Story 6 (US6) — consistent scene constructors [P3] [T1]

### Tests First

- [X] T029 [P] [US6] [skillist: fs-skia-scene] Add an FSI fixture under `readiness/fsi/` that constructs `Rectangle`/`PaintedRectangle`/`Text` via the existing positional constructors and via the new self-describing forms; failing-first, the self-describing forms do not yet exist (FR-010, SC-006)

### Implementation

- [X] T030 [US6] [T1] [skillist: fs-skia-scene] Add additive, self-describing constructors/helpers for `Rectangle`/`PaintedRectangle`/`Text` (a `Rect`-based and/or named-argument form consistent with `rectangleWithPaint`/`PaintedRectangle`) in `src/Scene/Scene.fs`/`.fsi`, retaining the existing positional DU cases and `Scene.rectangle`/`Scene.text` helpers so existing generated code keeps compiling (FR-010, SC-006)
- [X] T031 [US6] [T1] [skillist: fs-skia-scene, fs-skia-template-update] Refresh `readiness/surface-baselines/FS.Skia.UI.Scene.txt` and the merged `FS.Skia.UI.txt` via `scripts/refresh-surface-baselines.fsx` and confirm `./fake.sh build -t PackageSurfaceCheck` passes with the additive surface
- [X] T032 [US6] [skillist: fs-skia-scene] Build with `./fake.sh build -t Dev`, compile the FSI fixture, and record in `readiness/fsi/` that both the existing positional and the new self-describing constructors compile (SC-006)

**Checkpoint**: US6 is independently testable — a consistent self-describing form exists for each scene node and existing positional constructors still compile.

---

## Phase 9: Framework-repo dev-process (FR-011) — feature-targeting regression guard [P3]

*Strictly after all consumer-facing work (US1–US6); must never block or delay it.*

### Tests First

- [X] T033 [P] [skillist: speckit-evidence-graph, speckit-evidence-audit] Add a fixture in which a `tasks.md` merely mentions a filename in prose and confirm (failing-first framing) that the evidence gates resolve the active feature from `.specify/feature.json` and do NOT fire required evidence from the bare filename mention (FR-011, SC-008)

### Implementation

- [X] T034 [skillist: speckit-evidence-graph, speckit-evidence-audit] Add a regression guard asserting the `EvidenceGraph`/`EvidenceAudit` gates continue to target the feature in `.specify/feature.json` and refuse placeholder fallback, echoing the resolved feature id and why a filename mention did/didn't trigger (behavior established by feature 037) (FR-011, SC-008)

### Evidence

- [X] T035 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceGraph` then `./fake.sh build -t EvidenceAudit`; record the resolved feature.json target and the non-triggering filename-mention result in `readiness/feature-targeting-regression.md` (SC-008)

**Checkpoint**: FR-011 is independently testable — the gates target `.specify/feature.json` and do not fire on an incidental filename mention.

---

## Phase 10: Integration & Polish

- [X] T036 [skillist: []] Run the full sequential FAKE validation order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`), plus `PackageSurfaceCheck`, confirm a freshly generated consumer project builds, runs its tests, and produces its evidence using only local references (SC-001 governing), and record the non-authoritative aggregate results in `readiness/logs/`
- [X] T037 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and that the resolved feature id and real task count are echoed
- [X] T038 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — all evidence is real per the plan's Synthetic-evidence decision)_ | | | | | | | | |
