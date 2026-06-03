# Tasks: Trustworthy `/speckit.tasks` Validation Experience

**Feature branch**: `059-speckit-tasks-validation-feedback`
**Spec**: `specs/059-speckit-tasks-validation-feedback/spec.md`
**Plan**: `specs/059-speckit-tasks-validation-feedback/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view. Never
hand-write `[S*]`.

`[SEH]` + `synthetic-error-handling-approved` is assigned only during design /
planning / task generation. **None planned here** — the malformed-input parser
tests run real code against real malformed strings, which is real evidence, not
synthetic.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US5]** — user-story scope
- Tier: the whole feature is **Tier 1 (contracted)**; per-task tier markers are
  omitted because every phase matches the spec tier.

Every task has a matching `tasks.deps.yml` key, and every line mirrors the
structured `skillist` as `[skillist: ...]`. This feature also dogfoods the new
per-task `owns:` field on its own `tasks.deps.yml`; titles are free-form and are
never the source of ownership.

## Author pitfall guidance (read before running `EvidenceGraph`)

Until this feature lands, the **title-trigger matcher is still live**. Task
titles that literally contain a capability trigger phrase (e.g. `task graph`,
`EvidenceGraph`, `evidence audit`, `EvidenceAudit`, `constitution`,
`validator diagnostics`, `synthetic propagation`) must declare the matching
skill in their `skillist`, or the graph gate blocks. Filenames (`Audit.fs`,
`tasks-deps-template.yml`) are exempt (filename context). `tasks.deps.yml` MUST
keep the indented one-key-per-id object shape; inline maps like
`T001: { deps: [], skillist: [] }`, duplicate keys, dangling dep ids, and
mismatched visible `[skillist: ...]` mirrors are all invalid.

## Canonical verification targets

`./fake.sh build -t Route` prints the authoritative tier + gate list for the
real diff. FAKE-backed targets share `.fake` state and are **not**
concurrency-safe — run them sequentially in the escalated order: `Dev`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
`EvidenceGraph`, `EvidenceAudit`. Regenerate `.claude` peers +
`validation.contract.yml` with `RefreshSurfaceBaselines` (currency enforced by
`SkillSyncCheck` / `TargetMetadataDrift`). The validation entry point for the
task graph is the in-process compiled `EvidenceGraph` target — there is **no**
`run-audit.sh` runner.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold the feature workspace: link spec, plan, research, data-model, and contracts, and create the `readiness/` directory
- [X] T002 [P] [skillist: []] Complete readiness notes for the audit-enforced placeholder files (`readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, `readiness/generated-validation-authority.md`, `readiness/skill-loading-evidence-workflow.md`, plus `target-metadata.md` and `agent-ready-verdict.md` for the escalated path), each naming the authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Record the feature Tier (Tier 1 contracted), affected layers (compiled governance engine plus consumer template and bundled skills), public-contract impact, Principle IV / MVU applicability (not applicable — governance tooling, no runtime state), and the required evidence obligations
- [X] T004 [skillist: []] Record the governance-risk level (small, medium, broad), the focused validation selected for this change, when broad validation is required, and how non-authoritative aggregate results are captured

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fsharp-parsing] Extend the `DepsEntry` record in `DepsParser.fsi` and `DepsParser.fs` with an optional `owns` field and parse the per-task `owns` key (default none)
- [X] T006 [P] [skillist: []] Revise `Audit.fsi` to drop the `expectedCapabilityMatches` signature and declare the `owns`-driven assessment surface
- [X] T007 [P] [skillist: []] Finalize the versioned deps-file schema contract under `contracts/tasks-deps-schema.md` as the shipped consumer contract, with the `owns` vocabulary and the directive error table
- [X] T008 [P] [skillist: []] Record the public-surface and skill-registry baseline obligations and the `RefreshSurfaceBaselines` regeneration step for the contract change
- [X] T009 [skillist: []] Record unsupported-scope boundaries (no new validator capabilities, no synthetic-propagation redesign) and the loud-failure diagnostics expected at the resolver and parser edges

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 — Validation never silently passes the wrong feature (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fsharp-build-orchestration] Add failing tests asserting the template `build.fsx` resolves the feature from `.specify/feature.json`, honors the override variable, and fails loud with no sample fallback
- [X] T011 [P] [US1] [skillist: []] Add a failing `GeneratedProduct.fs` expectation asserting a generated project ships no sample feature and inherits the loud-fail resolver behaviour

### Implementation

- [X] T012 [US1] [skillist: fsharp-build-orchestration] Replace `ensureGeneratedEvidencePackage` in `template/base/build.fsx` with a `feature.json` resolver (`SPECKIT_FEATURE_DIR` override, then `.specify/feature.json`, then loud fail) and delete the sample synthesiser, the `specs/generated-evidence-workflow` fallback, and the sample-era `GENERATED_EVIDENCE_FEATURE_DIR` selector
- [X] T013 [US1] [skillist: fsharp-build-orchestration] Echo the resolved feature directory and task count (and the `SPECKIT_FEATURE_DIR` override when set) from the validation target so authors can confirm what was validated
- [X] T014 [US1] [skillist: []] Update `GeneratedProduct.fs` expectations to assert sample absence and the loud-fail message naming `.specify/feature.json` and the override variable
- [X] T015 [US1] [skillist: fs-skia-template-update] Generate a consumer project, run the validation target, and confirm the echoed directory and count match the feature with a loud failure when none resolves; capture the transcript under `readiness/`

**Checkpoint**: US1 demonstrably eliminates the false-green path.

---

## Phase 4: User Story 2 — The documented validation command works as written (US2, P2)

### Tests First

- [X] T016 [P] [US2] [skillist: speckit-evidence-graph] Cross-read the `speckit-tasks` Validation section and the `speckit-evidence-graph` skill to confirm they name the same in-process `EvidenceGraph` entry point

### Implementation

- [X] T017 [US2] [skillist: speckit-evidence-graph] Rewrite the `speckit-tasks` skill Validation section to remove the non-existent `run-audit.sh` runner and defer to the canonical `EvidenceGraph` target, documenting the `SPECKIT_FEATURE_DIR` override variable
- [X] T018 [US2] [skillist: speckit-evidence-graph] Reconcile the `speckit-evidence-graph` skill so both skills present one non-contradictory validation entry point

**Checkpoint**: US2 — copy-verbatim command runs; the two skills agree.

---

## Phase 5: User Story 3 — `tasks.deps.yml` validates on the first correct attempt (US3, P2)

### Tests First

- [X] T019 [P] [US3] [skillist: fsharp-parsing] Add failing parser tests: bare top-level `Tnnn` keys missing the wrapper emit the standalone directive error and are not buried under downstream no-key errors

### Implementation

- [X] T020 [US3] [skillist: fsharp-parsing] Short-circuit `DepsParser.fs` to detect bare top-level task-id keys missing the `tasks` wrapper and emit the FR-007 directive message first and standalone
- [X] T021 [P] [US3] [skillist: []] Update `tasks-deps-template.yml` to exemplify `schema_version`, the `tasks` wrapper, and the per-task `owns` field with a complete minimal example
- [X] T022 [US3] [skillist: speckit-tasks] Document the required wrapper, version key, and `owns` field in the `speckit-tasks` skill with an embedded copyable deps example (the removed sample is no longer the only reference)
- [X] T023 [US3] [skillist: speckit-evidence-graph] Validate that a deps file authored strictly from the template and skill text passes the schema gate on the first attempt (SC-003)

**Checkpoint**: US3 — first-attempt authoring passes; the wrapper error is directive.

---

## Phase 6: User Story 5 — Honest, low-friction assessment and the skill split (US5, P3)

### Tests First

- [X] T024 [P] [US5] [skillist: fsharp-graph-algorithms] Add failing audit tests: free-form titles no longer block, ownership derives from the `owns` field, and unknown `owns` values and missing implied skills are reported

### Implementation

- [X] T025 [US5] [skillist: fsharp-graph-algorithms] Remove `capabilityTriggerGroups`, `triggerMatchesTitle`, and `expectedCapabilityMatches` from `Audit.fs` and derive the `SkillAssessment` from the `owns` field
- [X] T026 [US5] [skillist: fsharp-graph-algorithms] Validate the `owns` closed vocabulary and the implied-skill-present rule in `Audit.fs` with directive error messages
- [X] T027 [P] [US5] [skillist: []] Author the two split skills `fs-skia-evidence-mode` and `fs-skia-layout-readability` under `.agents/skills` and retire `fs-skia-layout-evidence`
- [X] T028 [US5] [skillist: fs-skia-template-update] Wire `.template.config/template.json` sources and `template/capabilities.yml` to register both new skills, then regenerate the `.claude` peers via `RefreshSurfaceBaselines`
- [X] T029 [US5] [skillist: speckit-tasks] Describe the skill-assessment behaviour honestly in the `speckit-tasks` skill (trusted-as-declared, and what the high-confidence cases key off) and add `owns` migration guidance
- [X] T030 [US5] [skillist: []] Confirm free-form natural-language titles never flip evidence ownership now that the title matcher is gone (SC-006)
- [X] T038 [US5] [skillist: fsharp-code-generation] Re-point every compiled and authored reference to the retired `fs-skia-layout-evidence`: the two `Verbatim` splice sources and the hardcoded capability-skill list in `build/Governance/GovernedBlocks.fs` (L158, L170, L303-315) to the two split skills; the canonical capability-skill list authored in `.specify/templates/constitution-template.md` and its preset twin; and the prose refs in `template/base/README.md`, `.specify/templates/tasks-template.md`, and `.specify/presets/fsharp-opinionated/{templates/tasks-template.md,commands/speckit.tasks.md}` — so `GeneratedGuidanceCheck` / `SkillSyncCheck` stay green (FR-012/FR-013)

**Checkpoint**: US5 — ownership is structured; titles are free-form; the catch-all is split.

---

## Phase 7: User Story 4 — Skill hints resolve to real registered skills (US4, P3)

### Tests First

- [X] T031 [P] [US4] [skillist: fsharp-io-globbing] Add a failing `Governance.Tests` check enumerating every skill id in the bundled hint tables and asserting each resolves to exactly one consumer-registerable skill

### Implementation

- [X] T032 [US4] [skillist: []] Correct the bundled hint tables in the `speckit-tasks` skill and the deps template, replacing the unresolvable `fs-skia-layout` hint and the layout example with the registered readability skill id
- [X] T033 [US4] [skillist: fsharp-io-globbing] Confirm following the corrected hints produces zero unresolved-skill validation failures (SC-004)

**Checkpoint**: US4 — every hint id resolves.

---

## Phase 8: Integration & Polish

- [X] T034 [skillist: fsharp-code-generation] Regenerate the `.claude` peers and `validation.contract.yml` via `RefreshSurfaceBaselines` and confirm `SkillSyncCheck` and `TargetMetadataDrift` currency (SC-007)
- [X] T035 [skillist: []] Run the escalated FAKE gate order sequentially — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck` — and record the non-authoritative aggregate results
- [X] T036 [skillist: speckit-evidence-graph] Run the in-process `EvidenceGraph` target and confirm no cycles, no dangling references, the correct feature directory and task count echo, and no `[S*]` surprises
- [X] T037 [skillist: speckit-evidence-audit] Run the `EvidenceAudit` merge gate and confirm the verdict is PASS or document every accepted synthetic override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. None are planned —
real evidence is the corrected, self-consistent guidance plus a real generated
consumer validating its own feature.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
