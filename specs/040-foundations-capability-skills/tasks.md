# Tasks: Foundations F# Capability Skills

**Feature branch**: `040-foundations-capability-skills`
**Spec**: `specs/040-foundations-capability-skills/spec.md`
**Plan**: `specs/040-foundations-capability-skills/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real — the
rendered skill files, a real SHA-256 comparison over the two real trees, and a
real `dotnet build` of the real tangled examples against the real pinned
packages. No `[S]` / `[SEH]` task is anticipated (plan: Synthetic evidence —
None). The deliberate-break self-tests in T020/T025 are gate self-tests, not
shipped synthetic fixtures.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]**, **[US4]** — user-story scope
- **[T1]** — Tier 1 (contracted) change; this whole feature is Tier 1
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read before implementation)

The six `fsharp-*` capability skills authored by this feature are **capability /
reference** skills (FR-007 / SC-005): they MUST NOT appear in any task
`skillist` and MUST NOT alter the evidence graph. That is why the parsing,
graph, code-generation, globbing, shell, and build-orchestration tasks below all
carry `[skillist: []]` even though they are *about* those capabilities — the
subject skill is deliberately not self-referenced. The only non-empty skillists
are `speckit-evidence-graph` (T029) and `speckit-evidence-audit` (T030). No
`fs-skia-*` skill applies (no rendering, viewer, layout, widgets, or visual
output is touched).

## Pitfall guidance (read before `EvidenceGraph`)

- `tasks.deps.yml` uses **one object-shaped key per task id** with indented
  `deps` and `skillist` fields — never inline maps like
  `T001: { deps: [], skillist: [] }`.
- Every `Tnnn` in this file appears exactly once as a key in `tasks.deps.yml`;
  every dependency uses an exact `Tnnn` id; every `[skillist: ...]` mirror
  matches the structured list exactly and in order.
- Task titles below deliberately avoid Spec Kit title-trigger phrases
  (`tasks.deps.yml`, `evidence graph`, `synthetic propagation`, `diff-scan`,
  etc.) on the skill-authoring tasks, so the graph algorithms / parsing skills'
  refinement work is not misread as a graph/audit workflow task. T029/T030 use
  those phrases intentionally and carry the matching evidence skills.

## Canonical Verification Targets (serialized — FAKE shares `.fake` state)

Run FAKE-backed targets **sequentially**, never concurrently (`CLAUDE.md` /
`AGENTS.md`). This feature's serialized order:

1. `./fake.sh build -t Dev` (now includes `SkillSyncCheck` + `SkillExamplesCheck`)
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Governance risk level for this feature is **small→medium** (build-tooling only;
two new FAKE targets, seven build-tooling packages, new `build/Governance`
`.fsi`). Focused validation for the selected level = the two new gates plus the
serialized order above; broad validation (full `Verify`) is required only if a
gate failure looks race-like or the concurrent FAKE context is unknown.
Aggregate FAKE results are recorded as **non-authoritative**; the focused
per-gate rerun is authoritative.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Add the seven adopt-set build-tooling `PackageVersion` entries (`FSharp.SystemTextJson`, `XParsec` 1.0.0, `Microsoft.Extensions.FileSystemGlobbing`, `Fake.IO.FileSystem`, `Fake.Tools.Git`, `DiffPlex`, `FsCheck`) to `Directory.Packages.props` in a build-tooling `ItemGroup`, each pinned per Central Package Management; resolve exact net10-compatible versions against NuGet
- [X] T002 [P] [T1] [skillist: []] Add rows for the seven build-tooling packages to `docs/reports/dependencies.md` (need, version, maintenance owner = build-tooling/governance) so `DependencyReport` reads them as build-tooling scope, never product/runtime
- [X] T003 [skillist: []] Record feature Tier 1, affected layer (`build/Governance` build-tooling only), public-API impact (no product `.fsi`; new build-tooling `.fsi` required), Elmish/MVU applicability (plugs into the existing `build.fsx` `update`/effect boundary — no new `Model`/`Msg` algebra), and the evidence obligations (refined skills + `SkillSyncCheck` PASS + `SkillExamplesCheck` PASS)
- [X] T004 [skillist: []] Complete readiness notes for the feature's required readiness placeholder files under `specs/040-foundations-capability-skills/readiness/` (governance-risk-levels, aggregate-hang-diagnostics, runtime-limitations, generated-validation-authority, evidence-graph, evidence-audit), each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation

- [X] T005 [T1] [skillist: []] Draft the curated `.fsi` signatures for the two new `build/Governance` public modules — `SkillSync.fsi` (`SkillPair` model, skill-pair discovery, SHA-256 byte-identity comparison; pure core + IO edge) and `SkillExamples.fsi` (`CodeBlock` model, ` ```fsharp ` extraction, generated-module/tangle rendering) — Principle II curated companions, no access modifiers in `.fs`
- [X] T006 [P] [T1] [skillist: []] Scaffold `build/SkillExamples/SkillExamples.fsproj` referencing exactly the adopt set (FCS-free), with `IsPackable=false`, a local `TreatWarningsAsErrors=false` override (errors still fail), and an empty regenerated `Generated/` directory
- [X] T007 [T1] [skillist: []] Add `SkillSyncCheck` + `SkillExamplesCheck` target stubs to `build.fsx` (new `StartTarget` dispatch arms / effect DU cases) and register them in `requiredTargets`, `targetDependencyRows`, and the `Dev` dependency list so `Verify`/`Ci` inherit them — no existing target changes meaning
- [X] T008 [skillist: []] Exercise the draft `.fsi` from FSI (representative pair-hash and block-extraction calls), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T009 [skillist: []] Record surface-area baselines for the new `build/Governance` modules and the unsupported-scope handling + fail-fast failure diagnostics (missing file / empty extraction → explicit FAIL, no silent skip)

**Checkpoint**: Foundation ready — skill refinement and gate stories may proceed.

---

## Phase 3: User Story 1 (US1) — refined skills answer the agent's question

*Independent test*: from the matching skill alone, name the correct library and
controlling parity caution (parsing YAML two-shapes, globbing fnmatch drift,
graph propagation rule) without reading the source report.

- [X] T010 [P] [US1] [skillist: []] Refine the `fsharp-parsing` skill cookbook (C1–C5, C16, C21): verdicts (YamlDotNet / FSharp.SystemTextJson / XParsec / regex), the exact `tasks.md` task-line + box/annotation grammar and `audit-status` region semantics, the two-shape YAML caution, the Stage-0 golden-fixture byte-parity obligation, an API walkthrough + multiple runnable ` ```fsharp ` examples per owned capability, cautions, consuming stages, and Sources/links — written byte-identically to both `.claude/skills/fsharp-parsing/SKILL.md` and `.agents/skills/fsharp-parsing/SKILL.md`
- [X] T011 [P] [US1] [skillist: []] Refine the `fsharp-graph-algorithms` skill cookbook (C6–C9): verdicts (hand-roll + FsCheck), the exact cycle-detection / Kahn topo-sort / propagation parity rules, the Stage-0 golden-fixture byte-parity obligation, API walkthrough + multiple runnable examples per capability, cautions, consuming stages, Sources/links — byte-identical in both trees
- [X] T012 [P] [US1] [skillist: []] Refine the `fsharp-code-generation` skill cookbook (C10–C12): verdicts (StringBuilder / `Utf8JsonWriter` schema-1.0 emit; **reject** code-quotations/FCS; Fabulous.AST/Myriad deferred-consider prose), API walkthrough + multiple runnable examples per capability using only adopt-set + BCL APIs, cautions (no FCS, build-tooling-only), consuming stages, Sources/links — byte-identical in both trees
- [X] T013 [P] [US1] [skillist: []] Refine the `fsharp-io-globbing` skill cookbook (C13–C14): verdicts (`Microsoft.Extensions.FileSystemGlobbing`), the .NET-glob vs Python-`fnmatch` semantic-drift caution with the golden-test-before-cutover mitigation, API walkthrough + multiple runnable examples per capability, consuming stages, Sources/links — byte-identical in both trees
- [X] T014 [P] [US1] [skillist: []] Refine the `fsharp-shell-process` skill cookbook (C15, C17): verdicts (in-process-first; `Fake.Tools.Git` / `Fake.Core.Process` for residual shelling), API walkthrough + multiple runnable examples per capability, cautions, consuming stages, Sources/links — byte-identical in both trees
- [X] T015 [P] [US1] [skillist: []] Refine the `fsharp-build-orchestration` skill cookbook (C18–C20): verdicts (Fake target orchestration; DiffPlex golden-diff; Expecto + FsCheck testing), API walkthrough + multiple runnable examples per capability, cautions, consuming stages, Sources/links — byte-identical in both trees

**Checkpoint**: All six skills refined to the cookbook bar in both trees.

---

## Phase 4: User Story 2 (US2) — `SkillSyncCheck` byte-identity gate

*Independent test*: in sync → PASS; flip one byte in one pair → FAIL naming the
drifted slug; restore → PASS.

### Tests First (Principle I, Principle VI)

- [X] T016 [P] [US2] [skillist: []] Add a failing `tests/Governance.Tests` test for the SHA-256 pair comparator: equal bytes → in-sync; differing bytes → out-of-sync with the offending slug and both hex digests named (fails before `SkillSync.fs` exists)
- [X] T017 [P] [US2] [skillist: []] Add a failing `tests/Governance.Tests` test for skill-pair discovery: finds exactly the six expected pairs across both trees; a missing file on either side is a failure, never a skip (fails before discovery exists)

### Implementation

- [X] T018 [US2] [skillist: []] Implement `build/Governance/SkillSync.fs` against its `.fsi`: discover the six pairs, `File.ReadAllBytes` + `System.Security.Cryptography.SHA256` per file (no newline normalization), compare digests, name drift — pure comparison core with IO only at the edge
- [X] T019 [US2] [skillist: []] Wire the `SkillSyncCheck` effect/target in `build.fsx`: in-process hash, write `readiness/skill-sync-check.md` (PASS: six slugs + shared hash) and `readiness/logs/skill-sync-check.txt`, emit `FailWith` naming every drifted slug + both hashes on drift
- [X] T020 [US2] [skillist: []] Run `SkillSyncCheck` over the refined six (PASS lists six matching hashes); self-test: flip one byte in one `SKILL.md` → FAIL names that slug; restore → PASS; capture the PASS/FAIL/PASS evidence

**Checkpoint**: Byte-identity is mechanically enforced across both trees.

---

## Phase 5: User Story 4 (US4) — `SkillExamplesCheck` tangle-and-compile gate

*Independent test*: every ` ```fsharp ` block compiles → PASS; break one block's
API call → FAIL naming the skill/block; fix → PASS.

### Tests First (Principle I, Principle VI)

- [X] T021 [P] [US4] [skillist: []] Add a failing `tests/Governance.Tests` test for the ` ```fsharp ` block extractor: returns blocks with stable `{skillSlug; blockIndex (1-based, per skill); startLine}` identity in document order (fails before `SkillExamples.fs` exists)
- [X] T022 [P] [US4] [skillist: []] Add a failing `tests/Governance.Tests` test for the tangler: wraps each block as `module Skill.<slug_underscored>.Block<NN>` preceded by a `// source: <skillPath>:<startLine>` comment, deterministic across runs (fails before the tangler exists)
- [X] T023 [US4] [skillist: []] Implement `build/Governance/SkillExamples.fs` against its `.fsi`: extract every ` ```fsharp ` block from the six `SKILL.md`, render `build/SkillExamples/Generated/<slug>.fs` deterministically (regenerated each run, never hand-edited)
- [X] T024 [US4] [skillist: []] Wire the `SkillExamplesCheck` target in `build.fsx`: regenerate `Generated/*.fs`, `dotnet build build/SkillExamples/SkillExamples.fsproj` capturing to `readiness/logs/skill-examples-check.txt`, map any compiler diagnostic back to the owning skill + block via the `// source:` comment, write `readiness/skill-examples-check.md` (per-skill block count); missing artifact or empty extraction → explicit FAIL
- [X] T025 [US4] [skillist: []] Run `SkillExamplesCheck` over the refined six (PASS lists per-skill block counts); self-test: break one block's API call → FAIL names the skill/block; fix → PASS; capture the PASS/FAIL/PASS evidence

**Checkpoint**: Every cookbook example is compile-verified against the pinned packages.

---

## Phase 6: User Story 3 (US3) — full capability coverage, single ownership

*Independent test*: for each capability family in the report, locate the owning
skill; confirm the six together cover C1–C21 and each cites the report.

- [X] T026 [US3] [skillist: []] Verify the C1–C21 ownership map: each capability owned by exactly one skill (parsing C1–C5/C16/C21; graph C6–C9; code-gen C10–C12; globbing C13–C14; shell C15/C17; orchestration C18–C20), union = {C1..C21}, intersection = ∅, and every skill's frontmatter cites the capability report as `metadata.source`; record the ownership table
- [X] T027 [US3] [skillist: []] Re-verify cross-tree byte-identity after refinement (`SkillSyncCheck` PASS over the refined six) and confirm none of the six capability skills appears in any task `skillist`

---

## Phase 7: Integration & Polish

- [X] T028 [skillist: []] Run the serialized FAKE validation order (`Dev` — now including `SkillSyncCheck` + `SkillExamplesCheck` — then `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`), recording aggregate FAKE results as non-authoritative and rerunning any race-like gate failure in focused isolation
- [X] T029 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and that none of the six capability skills appears in any `skillist` (SC-005: the evidence graph is unchanged by this feature)
- [X] T030 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS with no synthetic evidence to accept (this feature ships none)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This feature ships
**none** (plan: Synthetic evidence — None). The deliberate-break self-tests in
T020/T025 are gate self-tests, not shipped synthetic fixtures.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
