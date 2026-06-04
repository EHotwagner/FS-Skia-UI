# Tasks: Breakout-Demo Consumer Friction Follow-ups & Feedback-Prompt Expansion

**Feature branch**: `061-breakout-consumer-friction-followups`
**Spec**: `specs/061-breakout-consumer-friction-followups/spec.md`
**Plan**: `specs/061-breakout-consumer-friction-followups/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written, by the evidence audit. No `[S]`/`[SEH]`
work is anticipated — per the plan all evidence is real (a freshly generated
`dotnet new fs-skia-ui --feedback true` project plus real gate runs). Should a task
discover an unavoidable synthetic dependency, Principle V disclosure applies and the
row is added to the Synthetic-Evidence Inventory.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase). FAKE-backed targets
  (`./fake.sh …`) share `.fake` state and MUST run sequentially regardless of `[P]`.
- **[US1]**…**[US6]** — user-story scope (mirrors spec User Stories 1–6).
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal). This feature is
  **Tier 2** throughout (D8: arcade helpers documented, not shipped — no `.fsi`
  surface). A task that reverses D8 to *ship* a helper escalates to `[T1]` and pulls
  in `.fsi` + per-module surface baselines; flag at this phase, never relabel at
  implementation time.

Every task has a matching entry in `tasks.deps.yml`; every line mirrors its
structured `skillist` via `[skillist: …]`. FAKE-backed validation runs in the
deterministic order Dev → GeneratedGuidanceCheck → TemplateCheck →
GeneratedProductCheck → EvidenceGraph → EvidenceAudit; `./fake.sh build -t Route` is
the authoritative gate list — re-run after each change-set.

## Governance risk levels

- **small** — single skill/template prose edit (e.g. FR-010 pitfall note, FR-011
  convention docs): focused validation = the gate(s) Route prints for that path
  (`SkillSyncCheck`/`SkillQualityCheck`/`TemplateCheck`), no broad rerun.
- **medium** — coupled skill+contract+stale-ref change (FR-003) or authoring-template
  change (FR-006/008/009): focused validation = `GeneratedGuidanceCheck` +
  `TemplateCheck`/`TemplateDrift` + `GeneratedProductCheck`.
- **broad** — `build/Governance/**` output change (FR-004/005/007): broad validation
  (`EvidenceGraph` + `EvidenceAudit` + Governance unit tests) is required because the
  audit/graph terminal output is consumer-facing. Non-authoritative aggregate runs
  (Dev aggregate) are recorded as such and never substitute for the focused gate.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm `.specify/feature.json` resolves to `specs/061-breakout-consumer-friction-followups` and cross-link spec/plan/research/data-model/contracts for this feature
- [X] T002 [P] [skillist: []] Record the Tier-2 classification and evidence obligations: no public `.fsi` change (D8 documents helpers, not ships); Principle IV N/A (no stateful/IO runtime); Principle V none-planned (all real evidence); Principle I degenerate (no new API to sketch in `.fsi`)
- [X] T003 [P] [skillist: []] Scaffold readiness placeholders discoverable before implementation — `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md`, `governance-risk-levels.md`, `runtime-limitations.md`, `feedback-hook-autofire.md`, `readiness-recoverability.md`, `arcade-helper-triage.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T004 [skillist: []] Run `./fake.sh build -t Route` baseline and capture the printed tier + minimal gate list to `readiness/focused-gates.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Map every canonical edit site and the `.agents`→`.claude` regeneration discipline: phase-skill sources `.agents/skills/speckit-{specify,clarify,plan,tasks,analyze,checklist,implement}/SKILL.md`; template-only `template/feedback/skill/SKILL.md`; governance `build/Governance/{Evidence/Scans.fs,Evidence/Audit.fs,Front/Governance.fs,SkillQuality.fs}`; `.specify` presets; `template/base/**`; keyboard-input skill canonical source; `src/Elmish/skill` + `.agents/skills/fs-skia-layout-readability` — confirm which require `RefreshSurfaceBaselines`
- [X] T006 [P] [skillist: []] Record governance risk levels (small/medium/broad), the focused validation per level, when broad validation is required, and how non-authoritative aggregate results are recorded → `readiness/governance-risk-levels.md`
- [X] T007 [P] [skillist: []] Record runtime limitations / non-graphical scope (no rendering, no persistent launch; FAKE-backed targets serialized on shared `.fake` state) and the aggregate-hang diagnostics shape → `readiness/runtime-limitations.md` + `readiness/aggregate-hang-diagnostics.md`
- [X] T008 [skillist: []] Confirm no synthetic evidence is required (Principle V) and define the real-evidence harness: a fresh `dotnet new fs-skia-ui --feedback true` project plus real gate runs; record that `[S]`/`[SEH]` are not anticipated

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 — Per-phase feedback auto-fires + fourth prompt (P1) [US1]

*FR-001/002/003 → SC-001/002. Note: FR-001/002 are skill prose (interpreted
instructions, not buildable behavior — D1), proven by the generated-project harness;
only FR-003's prompt-count gets the executable D6 gate assertion.*

### Tests First

- [X] T009 [P] [US1] [skillist: fsharp-build-orchestration] Add a failing-first low-cost gate assertion (D6 / FB-5, `SkillQuality.fs`- or `TemplateCheck`-adjacent) that the generated feedback skill enumerates exactly four `1.`–`4.` prompts and the record schema contains a `## Skill gaps` section — fails before T012

### Implementation

- [X] T010 [US1] [skillist: []] FR-001: rewrite the "Check for extension hooks" block in every canonical phase-skill source to multi-file discovery — read `.specify/extensions.yml`, then enumerate `.specify/extensions/*/*.yml`, merge `hooks.<before|after>_<phase>`, dedup by `(extension, command)` (first wins), drop `enabled: false`, do not evaluate `condition` (HD-1/2/3/5)
- [X] T011 [US1] [skillist: []] FR-002: add the one-line `Note: optional hook {extension}:{command} is registered but was not run (skipped).` phase-end notice to the same discovery block for discovered-but-not-run optional hooks (HD-4)
- [X] T012 [US1] [skillist: []] FR-003: finish the 3→4 prompt expansion in `template/feedback/skill/SKILL.md` — the fourth prompt ("What additional or new skills would have been helpful during the *{phase}* phase? … or 'none'") and the matching `## Skill gaps` record-schema section, including the "none" parity path (FB-1/2/3)
- [X] T013 [US1] [skillist: []] FR-003: update the 058 sourcing contract `specs/058-skills-quality-feedback/contracts/feedback-capture.md` (attribution credits 061) and sweep every stale "three prompts" reference to four across `specs/058-skills-quality-feedback/{spec.md,research.md,plan.md,tasks.md,readiness/template-feedback-true.md,readiness/task-graph.*}` (FB-4 / SC-002)
- [X] T014 [US1] [skillist: fsharp-build-orchestration] Regenerate `.claude/**` from the edited `.agents` phase-skill sources via `RefreshSurfaceBaselines`; confirm `SkillSyncCheck` / `TargetMetadataDrift` green and `.claude` mirrors `.agents` byte-for-byte (HD-6)

### Story verification

- [X] T015 [US1] [skillist: fs-skia-template-update] FR-001/003 evidence: pack/install the template, generate a fresh `--feedback true` project with the hook present only under `.specify/extensions/feedback/feedback.yml`, complete a phase **without an explicit nudge**, and capture the auto-fired record (with the new `## Skill gaps` section) + the no-surviving-"three prompts" grep → `readiness/feedback-hook-autofire.md` (SC-001/002)

**Checkpoint**: US1 independently verifiable — feedback auto-fires with four prompts.

---

## Phase 4: User Story 2 — Readiness contract discoverable without decompiling (P1) [US2]

*FR-004/005 → SC-003/004. Broad risk: `build/Governance/**` output change.*

### Tests First

- [X] T016 [P] [US2] [skillist: fsharp-build-orchestration] Add a failing-first Governance unit test pinning the per-file readiness-contract failure diagnostic — `fileName` + the full `required-tokens` (+ `required-fields` / `required-table-header` where applicable) + the `missing:` subset, derived from the same `requiredTokens` data that enforces the rule (RC-1/RC-2) — fails before T017
- [X] T017 [US2] [skillist: fsharp-code-generation] FR-004: carry the per-file required shape from `Scans.fs` (`MissingTerms` already holds it) through `Audit.fs` and print the complete expected schema per failing readiness file in `Front/Governance.fs` (replace the bare `readiness-contract-hits=%d` collapse) — single source = the enforced `terms` list, cannot drift
- [X] T018 [P] [US2] [skillist: []] FR-005: resolve the defect-class concept to the single literal `product-defect` across the readiness audit and any source governance scan; discover and remove/correct any residual project-prefixed `<project>-defect` rule, template, doc, or test (or document a genuinely-distinct use at both sites) (DC-1/DC-2 / SC-004)

### Story verification

- [X] T019 [US2] [skillist: fs-skia-template-update] FR-004 evidence: in a fresh project with no passing sibling, trigger the readiness-contract failures and reach a passing `EvidenceAudit` using **only** the audit output (and/or shipped templates) — no `FS.Skia.UI.Build.dll` decompilation, no sibling copy → `readiness/readiness-recoverability.md` (SC-003)

**Checkpoint**: US2 independently verifiable — required shapes recoverable from output.

---

## Phase 5: User Story 3 — Accurate build/test & graph-verdict guidance (P2) [US3]

*FR-006/007 → SC-005.*

### Tests First

- [X] T020 [P] [US3] [skillist: fsharp-build-orchestration] Add a failing-first Governance unit test pinning the `EvidenceGraph` terminal token `verdict=ok (no cycles, no dangling refs, no [S*])` on a clean graph and `verdict=error (<reason>)` on failure, consistent with `EvidenceAudit`'s `verdict=PASS|FAIL` style (GV-1/2/3) — fails before T021

### Implementation

- [X] T021 [US3] [skillist: fsharp-code-generation] FR-007: emit the explicit terminal `verdict=…` line in the `EvidenceGraph` in-process output (`Front/Governance.fs` `=== speckit.evidence.graph ===` block), reasons inline, additive to exit-code semantics
- [X] T022 [P] [US3] [skillist: []] FR-006: state that `Dev` is a completion-marker / log-writer target (`readiness/logs/Dev.txt`, no real compile feedback) and that `Test`/`Verify` (`dotnet test`) is the authoritative compile/test path, in `template/base/README.md`, `template/base/docs/product.md`, and the tasks-template build guidance

**Checkpoint**: US3 independently verifiable — quickstart accurate, clean graph self-evident.

---

## Phase 6: User Story 4 — Self-describing authoring templates (P2) [US4]

*FR-008/009 → SC-006. Preset copy authoritative; generic copy gets the pointer (D5).*

- [X] T023 [P] [US4] [skillist: []] FR-008: inline the `GeneratedGuidanceCheck` pass-criteria as a template comment in the *Repository Governance Decisions* block of the plan template (no empty/boilerplate/`NEEDS CLARIFICATION`/placeholder markers; `N/A`-with-rationale counts as filled) — `.specify/presets/fsharp-opinionated/templates/plan-template.md` (authoritative) and the generic `.specify/templates/plan-template.md`
- [X] T024 [P] [US4] [skillist: []] FR-009: name the exact preset-relative paths (`.specify/presets/fsharp-opinionated/templates/tasks-template.md` and `…/tasks-deps-template.yml`) in `.agents/skills/speckit-tasks/SKILL.md`, and add a one-line "authoritative copy: preset path — edit there" pointer to the generic `.specify/templates/tasks-template.md`
- [X] T025 [US4] [skillist: fsharp-build-orchestration] Regenerate any generation-owned blocks via `RefreshSurfaceBaselines` (constitution fragments live in the generic copies), and confirm `GeneratedGuidanceCheck` / `TemplateDrift` green after the template/skill edits

**Checkpoint**: US4 independently verifiable — author-facing criteria + preset pointer present.

---

## Phase 7: User Story 5 — Pitfalls note covers consumer-internal DU collisions (P2) [US5]

*FR-010 → SC-007. Extend the existing note, do not re-add (D7).*

- [X] T026 [US5] [skillist: fs-skia-keyboard-input] FR-010: extend the duplicate-DU-case "Common pitfalls" note in `template/product-skills/fs-skia-keyboard-input/SKILL.md` (the standalone, shipped canonical copy where 060's note lives — `src/KeyboardInput/skill/SKILL.md` carries no such note and is not its source) with the consumer-internal cross-module example `GameMode.Launch` vs `Msg.Launch` — bare `Launch` binds to the last-declared type, yielding misleading "expected GameMode but has type Msg" errors — and the fully-qualified resolution
- [X] T027 [US5] [skillist: fsharp-build-orchestration] `template/product-skills/**` is a standalone shipped skill root (not generated from `.agents`/`src`, so `SkillSyncCheck` does not govern it and no `RefreshSurfaceBaselines` regen is needed). Confirm `SkillQualityCheck` + `TemplateCheck` / `GeneratedProductCheck` / `TemplateDrift` green after the edit (FR-012)

**Checkpoint**: US5 independently verifiable — consumer-internal DU case documented.

---

## Phase 8: User Story 6 — Reusable arcade helpers triaged (P3) [US6]

*FR-011 → SC-008. All four documented as canonical conventions, not shipped (D8).*

- [X] T028 [P] [US6] [skillist: fs-skia-elmish] FR-011: document the fixed-step accumulator (`1/120 s`, capped steps/tick) deterministic `step` driver, the AABB / circle-vs-rect collision + single-reflection-per-step (axis by normalized penetration), and the paddle-rebound angle with a `|Dy|` floor as canonical MVU update/game-loop conventions (with reference snippets) in `src/Elmish/skill/SKILL.md`
- [X] T029 [P] [US6] [skillist: fs-skia-layout-readability] FR-011: document the `reserveHudBand` HUD-band reservation convention (gameplay region = surface − reserved band, clamp gameplay, overdraw HUD last) in `.agents/skills/fs-skia-layout-readability/SKILL.md`, extending 060 FR-008's HUD/gameplay pattern doc
- [X] T030 [US6] [skillist: []] FR-011: record the per-helper ship-vs-document decision (all four = `document`, with home skill and canonical-convention reference per helper) → `readiness/arcade-helper-triage.md` (SC-008); if any task elects to *ship* a helper instead, escalate that helper to Tier 1 and add its `.fsi` + surface baseline (D8 reversibility gate)
- [X] T031 [US6] [skillist: fsharp-build-orchestration] Regenerate `.claude/**` via `RefreshSurfaceBaselines`; confirm `SkillSyncCheck` / `TargetMetadataDrift` / `SkillQualityCheck` green after the two skill edits (FR-012)

**Checkpoint**: US6 independently verifiable — each helper triaged and recorded.

---

## Phase 9: Integration & Polish

- [X] T032 [skillist: []] Re-run `./fake.sh build -t Route` (and `Route --enforce`) after all change-sets; capture the final authoritative tier + gate list to `readiness/focused-gates.md`
- [X] T033 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Dev` (FAKE-backed, sequential) → `readiness/logs/Dev.txt`; obtain real compile/test feedback via `Test`/`Verify` (`dotnet test`); record the aggregate result as non-authoritative
- [X] T034 [skillist: fsharp-build-orchestration] Run the Route-listed content gates sequentially — `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `TemplateDrift`, `SkillContractPathCheck` — capturing each log under `readiness/logs/`
- [X] T035 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` for `specs/061-breakout-consumer-friction-followups`; confirm no cycles, no dangling refs, no `[S*]`, and the new `verdict=ok` terminal line prints (graph before/after recorded)
- [X] T036 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit`; confirm `verdict=PASS` for `specs/061-breakout-consumer-friction-followups` with no synthetic-propagation or diff-scan blocks (SC-009)
- [X] T037 [skillist: []] Finalize the escalated-tier readiness artifacts — `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md` (ISO-8601 timestamped rows, skill-loading notes), `aggregate-hang-diagnostics.md` — for `Route --enforce`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. None anticipated — all
evidence is real (a generated project + real gate runs).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
