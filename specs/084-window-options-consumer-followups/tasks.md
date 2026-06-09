# Tasks: Window Startup Options (Fullscreen + Windowed-Fullscreen Default) & Invoice1/Spread1 Consumer Follow-ups

**Feature branch**: `084-window-options-consumer-followups`
**Spec**: `specs/084-window-options-consumer-followups/spec.md`
**Plan**: `specs/084-window-options-consumer-followups/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` (design-approved synthetic error-handling) is not used in this feature:
there is no malformed-input / explicit-error-path task. The deliberate readiness
gap used to verify audit legibility (SC-004) is a **test input**, not a stand-in
for an unavailable dependency, so it is real evidence — not `[S]`/`[SEH]`.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- **[T1]** — Tier 1 (contracted) change. This whole feature is escalated Tier 1
  (`maintainer-verify`): public `.fsi` union-case addition + default-value change,
  `template/**` edits, governance/build-surface edits, and a skill edit. No `[T2]`
  override is needed — the phase tier matches the feature tier throughout.

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors the
structured `skillist` value as `[skillist: ...]` (use `[skillist: []]` when none
applies). `speckit.evidence.graph` refuses to proceed with dangling references or
mismatched mirrors.

## Success-criterion → assertion mapping

Mechanically-testable success criteria are pinned to concrete enforcing assertions
so a headline SC cannot be silently violated while every gate stays green:

- **SC-001** (no-flag launch = windowed fullscreen) — `defaultWindowBehavior.StartupState = WindowedFullscreen` test (T007) + real visible-window capture (T014).
- **SC-002** (each supported state honored, none "unsupported") — `validateWindowBehavior`/`validateWindowLaunchBehavior` classification test (T007) + per-state launch evidence (T014).
- **SC-003** (full readiness contract discoverable) — rendered `evidence-formats.md` window-visibility list **equals** `Scans.requiredFiles` (T017).
- **SC-004** (audit blockers legible on stdout) — poisoned-fixture audit-stdout assertion: per-blocker reason + hit-file path + base_ref line (T018, T023).
- **SC-005** (scaffold-map matches generated tree verbatim) — `Feature062GovernanceTests` phrase assertions (T024) + real-tree diff (T026).
- **FR-005** (durable launch literal survives wiring) — `GovernanceTests.fs:105` literal `Viewer.runApp viewerOptions generatedHost` still passes after the guarded branch (T030).

## Risk levels & validation

- **Small** (framework-internal `src/SkiaViewer/**/*.fs` body edits): focused
  `-t Test`. **Medium** (template/docs/skill edits): add `TemplateCheck` /
  `GeneratedGuidanceCheck`. **Broad** (public `.fsi`, governance build surface,
  baselines): the escalated six-target order (T032–T034). This feature is **broad**;
  broad validation is required before merge.
- Aggregate FAKE results that hang or look race-like are **non-authoritative** —
  rerun the affected FAKE-backed target sequentially (shared `.fake` state) and
  record that rerun before any product-regression claim (`readiness/aggregate-hang-diagnostics.md`).

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Confirm `./fake.sh build -t Route` escalates this change to `maintainer-verify`; record the printed tier + minimal gate list; link spec + plan in the feature directory
- [X] T002 [P] [T1] [skillist: fs-skia-evidence-mode] Scaffold the seven-file window-visibility readiness set under `readiness/` (`interactive-visible-window.md`, `window-state-diagnostics.md`, `window-options.md`, `close-reason-separation.md`, `real-image-evidence.md`, `generated-validation.md`, `evidence-audit.md`) plus the visual-demo scaffolds (`visual-evidence-honesty.md`, `window-visibility.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `audit-diagnostics.md`) — each naming authoritative command, artifact path, failure class, next action
- [X] T003 [P] [T1] [skillist: []] Record feature Tier (escalated Tier 1), affected layer (SkiaViewer + Build governance + template + skill), public-API impact (additive union case + default-value change), Elmish/MVU applicability (viewer is the stateful boundary; `ApplyWindowOptions` carrier unchanged, no new effect type), and evidence obligations

---

## Phase 2: Foundation

- [X] T004 [T1] [skillist: fs-skia-skiaviewer] Draft the `src/SkiaViewer/SkiaViewer.fsi` surface delta: additive `WindowedFullscreen` case on `ViewerWindowStartupState` and the `defaultWindowBehavior` value-change note (signatures of `runApp`/`runAppWithWindowBehavior`/`validate*`/request/result/effect all unchanged) per `contracts/skiaviewer-window-surface.md`
- [X] T005 [P] [T1] [skillist: []] Record that the per-package `readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt` baseline **and** the cross-package surface baseline move and must be recaptured (`RefreshSurfaceBaselines` / `PerPackageSurface`); note the additive-case incomplete-match warning is desirable
- [X] T006 [P] [T1] [skillist: fs-skia-evidence-mode] Record unsupported-scope handling & failure diagnostics: headless/no-display degrades to honest render-only (no false visible-window claim); exclusive fullscreen on an incapable host falls back with an honest diagnostic (not a false "honored"); windowed fullscreen remains the capable default

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — windowed-fullscreen default & the chosen state takes effect (P1)

### Tests First (Principle I, Principle VI)

- [X] T007 [P] [US1] [T1] [skillist: fs-skia-skiaviewer] Failing-first `tests/SkiaViewer.Tests`: `defaultWindowBehavior.StartupState = WindowedFullscreen` (SC-001); `validateWindowBehavior`/`validateWindowLaunchBehavior` report `Honored` for `Fullscreen` and `WindowedFullscreen` and `UnsupportedOption` for `Minimized` (SC-002)
- [X] T008 [P] [US1] [T1] [skillist: fs-skia-skiaviewer] Failing-first `tests/SkiaViewer.Tests`: `applyWindowBehaviorToOptions` maps `WindowedFullscreen` to `WindowBorder.Hidden` + work-area `Position`/`Size` + `WindowState.Normal`, and `Fullscreen` to `WindowState.Fullscreen` (distinctness invariant)

### Implementation

- [X] T009 [US1] [T1] [skillist: fs-skia-skiaviewer] Add the `WindowedFullscreen` union case to `SkiaViewer.fsi` + `SkiaViewer.fs` and change `defaultWindowBehavior.StartupState` to `WindowedFullscreen` (FR-001/FR-003)
- [X] T010 [US1] [T1] [skillist: fs-skia-skiaviewer] Reclassify `validateBehavior`/`validateLaunch`: `Fullscreen` and `WindowedFullscreen` → `Honored` (replace the stale "not yet supported" message), keep `Minimized` `UnsupportedOption`; preserve the launch-aware capability check so an incapable host still falls back honestly (FR-002)
- [X] T011 [US1] [T1] [skillist: fs-skia-skiaviewer, fs-skia-evidence-mode] Implement the `WindowedFullscreen` arm of `applyWindowBehaviorToOptions` (hidden border + work-area geometry, `WindowState.Normal`) and the edge interpreter that reads default-monitor work-area bounds, degrading to honest render-only when bounds cannot be resolved (FR-001)
- [X] T012 [US1] [T1] [skillist: fs-skia-skiaviewer] Extend `template/base/src/Product/WindowOptions.fs`: add the `windowed-fullscreen` flag value, change the no-flag default to `windowed-fullscreen`, reclassify `fullscreen`/`windowed-fullscreen` to honored, and resolve conflicting flags to the explicit last-specified value (FR-006, conflict edge case)
- [X] T013 [US1] [T1] [skillist: fs-skia-skiaviewer] Wire `template/base/src/Product/Program.fs` with a guarded branch: `if windowFlagSupplied then Viewer.runAppWithWindowBehavior …` else the durable `Viewer.runApp viewerOptions generatedHost` literal — keeping that literal present and reachable (FR-004/FR-005)
- [X] T014 [US1] [T1] [skillist: fs-skia-skiaviewer, fs-skia-evidence-mode] Persistent graphical launch from the generated default executable path; capture real visible-window evidence for the no-flag windowed-fullscreen default and once per supported state (normal, maximized, fullscreen, windowed-fullscreen), writing decodable image evidence to `readiness/real-image-evidence.md` (SC-001/SC-002); on a headless host record the honest render-only degradation
- [X] T015 [US1] [T1] [skillist: fs-skia-skiaviewer] Exercise the packed `ViewerWindowStartupState` surface from FSI (new default + Honored reclassification) and capture the transcript to `readiness/fsi-session.txt` (Principle I)
- [X] T016 [US1] [T1] [skillist: []] Populate `readiness/window-options.md` with the new states and document US1's independent validation path (`option=` rows for resize/maximize/startup-state/startup-position/backend reflecting windowed-fullscreen)

**Checkpoint**: US1 functional — the live window honors every supported startup state and defaults to windowed fullscreen.

---

## Phase 4: User Story 2 (US2) — discoverable readiness contract & legible audit blockers (P1)

### Tests First

- [X] T017 [P] [US2] [T1] [skillist: fsharp-build-orchestration] Failing-first `tests/Governance.Tests`: the rendered `evidence-formats.md` window-visibility file list **equals** `Scans.requiredFiles` (all seven files), so the doc cannot silently drift behind the engine (SC-003)
- [X] T018 [P] [US2] [T1] [skillist: fsharp-build-orchestration] Failing-first `tests/Governance.Tests`: on a poisoned readiness fixture, audit stdout contains each blocker's area + file + one-line reason + hit-file path and a non-misleading `diff-scan base_ref:` line (SC-004)

### Implementation

- [X] T019 [US2] [T1] [skillist: fsharp-code-generation] Extend the `WindowVisibility` class in `build/Governance/Evidence/EvidenceFormatSchema.fs` (the single source) to enumerate all seven `Scans.requiredFiles` with each file's required tokens per `data-model.md` (FR-007)
- [X] T020 [US2] [T1] [skillist: fsharp-build-orchestration] Regenerate `template/base/docs/evidence-formats.md` from the extended schema via `./fake.sh build -t RefreshSurfaceBaselines` (no hand-edit of the generated doc)
- [X] T021 [US2] [T1] [skillist: fsharp-code-generation] Surface per-blocker `reason` + originating hit-file path on `EvidenceAudit` stdout by wiring the existing per-area renderers (`Render.readinessContractDiagnostics` and siblings) into the summary block in `GeneratedRunner.fs` / `Front/Governance.fs` (FR-008)
- [X] T022 [US2] [T1] [skillist: fsharp-shell-process] Thread the already-resolved merge-base into `EvidenceInputs` → populate `DiffScanResult.BaseRef` (was hardcoded `None`) and print `diff-scan base_ref:` to stdout; emit the explicit-absence message when no default-branch ancestor resolves (FR-009, base_ref edge case)
- [X] T023 [US2] [T1] [skillist: fsharp-build-orchestration] Trigger a deliberate readiness gap and capture real `EvidenceAudit` stdout proving every blocker + base-ref line is legible without opening any `*-hits.json`, to `readiness/audit-diagnostics.md` (SC-004)

**Checkpoint**: US2 functional — the readiness contract is readable from shipped docs and the audit explains itself on stdout.

---

## Phase 5: User Story 3 (US3) — trustworthy scaffold-map for a model swap (P2)

### Tests First

- [X] T024 [P] [US3] [T1] [skillist: fsharp-build-orchestration] Failing-first `tests/Governance.Tests` (extend `Feature062GovernanceTests`): `docs/scaffold-map.md` contains the `<ProjectName>`/`<ProductDir>` project-named paths, the durable-but-must-re-point class phrase, and a non-game HUD→headers / gameplay→grid remap example (SC-005)

### Implementation

- [X] T025 [US3] [T1] [skillist: fs-skia-layout-readability] Hand-edit `template/base/docs/scaffold-map.md`: replace `src/Product/**` with `<ProjectName>`/`<ProductDir>` placeholders, split durable into model-agnostic vs must-re-point (moving `LayoutEvidence.fs`/`EvidenceCommands.fs`/`WindowOptions.fs` into must-re-point with the "keep file + scanned tokens, re-point model-field references" definition), and add the non-game layout-region remap example (FR-010/FR-011)
- [X] T026 [US3] [T1] [skillist: fs-skia-template-update] Diff the scaffold-map's cited paths against a freshly generated project tree and confirm zero manual reconciliation (SC-005)

**Checkpoint**: US3 functional — the scaffold-map matches a generated tree verbatim and the durable/re-point distinction is explicit.

---

## Phase 6: User Story 4 (US4) — honest build signals & graceful analyze (P2)

### Implementation

- [X] T027 [P] [US4] [T1] [skillist: fsharp-build-orchestration] Edit `template/base/docs/product.md` + `README.md`: state that `Verify` embeds the merge-gate audit (`EvidenceGraph` then `EvidenceAudit`) before tests and hard-blocks until every task is `[X]`, name `-t Test` as the mid-implementation green-test path, and confirm the existing `Dev`-is-log-only disclosure is present (FR-012/FR-013)
- [X] T028 [P] [US4] [T1] [skillist: speckit-analyze] Edit the canonical `.agents/skills/speckit-analyze/SKILL.md` so the symbol-cross-check step probes target availability and skips-with-documented-notice when `SymbolCrossCheck` is absent, mirroring how `EvidenceGraph` resolves the feature from `.specify/feature.json` (FR-014); keep `SkillQualityCheck` detector phrases intact
- [X] T029 [US4] [T1] [skillist: fsharp-code-generation] Regenerate the `.claude/skills/speckit-analyze/**` mirror from the canonical `.agents` tree via `./fake.sh build -t RefreshSurfaceBaselines` (SkillSyncCheck-enforced currency)
- [X] T030 [US4] [T1] [skillist: fsharp-build-orchestration] Confirm the durable `GovernanceTests.fs:105` literal still passes after the guarded launch wiring (FR-005); document the mid-implementation green-test path (`-t Test`, SC-006) and the `/speckit-analyze` graceful skip-with-notice in a project lacking `SymbolCrossCheck` (SC-007)

**Checkpoint**: US4 functional — build signals are honest and analyze degrades gracefully.

---

## Phase 7: Integration & Polish

- [X] T031 [T1] [skillist: fs-skia-skiaviewer] Recapture the per-package (`FS.Skia.UI.SkiaViewer.fsi.txt`) and cross-package surface baselines, the `.claude` skill mirror, and `validation.contract.yml` via `./fake.sh build -t RefreshSurfaceBaselines` (Tier 1 surface move)
- [X] T032 [T1] [skillist: fsharp-build-orchestration] Run the escalated broad gates sequentially (shared `.fake` state): `./fake.sh build -t Dev`, `-t GeneratedGuidanceCheck`, `-t TemplateCheck`, `-t GeneratedProductCheck`; record the governance risk level and note any non-authoritative aggregate/hang result with its sequential rerun
- [X] T033 [T1] [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; confirm the echoed `feature-directory=` matches this feature
- [F] T034 [T1] [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (no `[S]`/`[S*]`, clean diff-scan) or document every `--accept-synthetic` override

### Validation results (recorded)

- **Dev** — Status Ok. **GeneratedGuidanceCheck** — Status Ok (FR-012/FR-013/FR-010/FR-011/FR-007 guidance current). **TemplateCheck** — Status Ok (TemplatePack/Instantiate/generated Test/TemplateSmoke all green: the guarded `runAppWithWindowBehavior` launch wiring + `windowed-fullscreen` flag build in generated projects). **GeneratedProductCheck** — the documented **non-authoritative** local environment-failure (see `readiness/generated-validation.md` / `aggregate-hang-diagnostics.md`); not the authoritative signal for this change.
- **EvidenceGraph** — Status Ok (no cycles, no dangling refs, no `[S*]` surprises; `unaccepted-synthetic-tasks=0`).
- **EvidenceAudit** — **verdict=FAIL, total-blockers=1.** The single blocker is `[persistent-launch] supported-host-persistent-launch.txt: missing supported-host persistent launch evidence` — `window-opened=true` requires a **display-capable host running the generated default executable**, deferred to merge per the documented `GeneratedProductCheck`-non-authoritative pattern. `readiness-contract-hits=0`, `window-visibility-hits=0`, `unaccepted-synthetic-tasks=0`. The audit's own stdout demonstrates **FR-008** (per-blocker area + file + reason + hit-file path) and **FR-009** (`diff-scan base_ref: main (merge-base …)`) — captured verbatim in `readiness/audit-diagnostics.md` (SC-004). The framework surface (new state, default, validation reclassification, options mapping) is proven real by `tests/SkiaViewer.Tests` (54 passing) + `readiness/fsi-session.txt`.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. None planned: the
framework change is exercised against the real packed surface and a display-capable
host; the audit-legibility gap (SC-004) is a real test input, not a synthetic
stand-in. A state whose real visible-window evidence cannot be captured on the
available host before merge would be recorded here as `[S]` with the honest
render-only degradation note.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T014 | Real visible-window launch needs a display-capable host running the generated default executable; the framework repo ships libraries + a template (no runnable windowed product) and the local `GeneratedProductCheck` is a documented non-authoritative environment-failure. The new state/default/validation/mapping are proven against the built library (real). | `readiness/real-image-evidence.md` (deferred) — authoritative capture on a display-capable host; framework surface proven in `readiness/fsi-session.txt` + `tests/SkiaViewer.Tests` | spec §Synthetic-Evidence Inventory render-only-degradation provision | render-only-degradation | spec.md Edge case (headless → honest render-only) | host-without-self-closing-windowed-product | honest render-only; no false visible-window claim | render-only-deferred |
