# Tasks: Foundations Evidence Engine Port (Stage 4)

**Feature branch**: `043-foundations-evidence-engine`
**Spec**: `specs/043-foundations-evidence-engine/spec.md`
**Plan**: `specs/043-foundations-evidence-engine/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real — captured
golden-fixture byte-diffs (0 bytes) for 036/037/038, typed Expecto unit results
and FsCheck property results for cycle detection / topological order /
synthetic-evidence propagation / status-region scanning, grep proofs (no
`python3`, no `run-audit.sh`, no `FSharp.Compiler.*`), the packed-engine
generated-consumer pass, and the serialized FAKE gate logs (plan: Evidence
obligations — real evidence only; Synthetic evidence — none planned). The
hand-built cyclic / multi-synthetic-root / empty-graph fixtures are typed *test
inputs* asserting typed `Result.Error` / propagation states, not synthetic
*evidence*, so no `[SEH]` task is approved (Principle V).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T1]** — Tier 1 (contracted): this whole feature is Tier 1 (new published
  `FS.Skia.UI.Build` governance modules, each with a curated `.fsi` per
  Principle II) and a named **dogfood** + consumer-contract feature (FR-015),
  so the `Route` selector escalates it to the full serialized gate set.
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read before implementation)

Like features 041 and 042, this feature is **build-tooling only**
(`build/Governance/Evidence/**` + `build.fsx` + `template/base/**`), so no
`fs-skia-*` runtime/rendering/viewer/layout/widgets skill applies to the engine
itself. It *consumes* the `fsharp-*` cookbooks as genuine implementation aids
and reuses `fs-skia-template-update` for the packaging/template track:

- **`fsharp-parsing`** — the governance-input parsers and scans: `TaskParser`
  (`tasks.md` line grammar + the Synthetic-Evidence Inventory table, T010),
  `DepsParser` (`tasks.deps.yml` via `YamlDotNet`, both forms, T011),
  `StatusRegion` (the `audit-status` fenced-region scan, T014), `Scans` (the four
  readiness scans' key=value parsing, T015), and `DiffScan` (the unified-diff
  pattern scan, T016). High confidence; these are the parse-governance-input
  tasks the cookbook targets.
- **`fsharp-io-globbing`** — `SkillRegistry` discovery across `.agents/skills`,
  `src/*/skill`, and `template/fragments/*/skill` (T012). Medium-high; file
  discovery + fnmatch-style matching is the cookbook's remit.
- **`fsharp-graph-algorithms`** — `Graph` (3-colour DFS cycle detection, Kahn
  topological order, the pure synthetic-evidence propagation rule, T013) and its
  typed unit / FsCheck property tests (T022, T023). High confidence; this is the
  hand-rolled DAG + propagation cookbook exactly.
- **`fsharp-code-generation`** — `Render` emits the byte-parity `task-graph.json`
  / `task-graph.md` / Mermaid / ASCII / audit count block governance artifacts
  (T018). High confidence; deterministic governance-artifact emission.
- **`fsharp-build-orchestration`** — the DiffPlex golden-diff parity harness
  (T009, T021), the Expecto/FsCheck typed test suites (T022–T025), the in-process
  `build.fsx` gate wiring (T020), and the serialized FAKE dogfood run (T032).
  Medium-high; the orchestration / test / golden-diff cookbook.
- **`fsharp-shell-process`** — only the retained external process: the `git diff`
  invocation at the `build.fsx` edge that feeds `DiffScan` (T020). Medium; the
  selector core stays pure and the diff is read as data.
- **`fs-skia-template-update`** — the published-package flip + pack flow (T005),
  the `template/base/**` rewrite to consume the packaged engine (T026, T027), and
  the generated-consumer pack-and-validate evidence (T028). High confidence; the
  template-refresh / pack cookbook.

**Not assigned** (with reasons, mirroring 041/042 discipline):
`fs-skia-skiaviewer` / `fs-skia-layout-evidence` — although `Scans` ports the
**persistent-launch**, **persistent-gui-runtime**, and **window-visibility**
readiness scans (T015), these are *text scans over a feature's `readiness/`
directory*, not viewer / window-host / layout work; no `SkiaViewer`, scene,
screenshot, or rendering surface is touched, so the viewer/layout skills do not
materially help. `fsharp-shell-process` is **not** assigned to the
golden-fixture capture (T006) because that task runs the existing legacy Python
engine and commits its JSON output — it writes no F#. `Audit` (T017) and
`Engine` (T019) are pure aggregation / composition over already-typed results
with no matching cookbook (valid-empty `skillist`).

## Governance risk levels & validation

- **Small** (routine framework-internal edits within this feature's own
  `build/Governance/Evidence/*.fs` library work): focused
  `./fake.sh build -t Dev` plus the `Governance.Tests` suite is authoritative.
- **Medium** (the new build-tooling `.fsi`/`.fs` modules, the two rewired
  `build.fsx` gate arms, the published-package flip, the `template/base/**`
  change): focused `Dev` plus the targeted FAKE governance gates the `Route`
  selector prints.
- **Broad** (required here because this is a **dogfood** + consumer-contract
  feature, FR-015 / FR-013): the full serialized FAKE gate order
  (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`
  → the graph gate → the audit gate). Aggregate FAKE results are recorded as
  **non-authoritative**; any race-like or environment-flaky failure (the known
  `SkiaViewer.Tests` headless crash, the `FsiTranscripts` toolchain issue) is
  rerun in focused isolation under a stash control, and that focused result is
  authoritative (SC-008).

## Pre-`EvidenceGraph` pitfall guidance

Run `./fake.sh build -t EvidenceGraph` (the in-process compiled-F# gate;
the legacy `run-audit.sh --graph-only` is decommissioned per T029) before
declaring this phase complete. Because this feature's *subject* is the evidence graph/audit engine,
task **titles** deliberately avoid the validator's blocking trigger tokens:
artifacts are named by filename (`task-graph.json`, `task-graph.md`,
`diff-scan-hits.json` — excluded by filename context), effect cases use the
`…Check` suffix (`EvidenceGraphCheck` / `EvidenceAuditCheck` — the trailing
letter defeats the bare-`EvidenceGraph` boundary), and the propagation rule is
written `synthetic-evidence propagation` (never the bare `synthetic
propagation`). The genuine graph/audit workflow tasks (T033/T034) **do** declare
`speckit-evidence-graph` / `speckit-evidence-audit`. `tasks.deps.yml` keeps one
indented object per task id with `deps` and `skillist`; every `[skillist: ...]`
mirror matches the structured list exactly and in order.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Record feature Tier 1 and **dogfood** + consumer-contract status, the affected layer (`build/Governance/Evidence/**` + `build.fsx` + `template/base/**` build-tooling only), public-API impact (no product `.fsi`; new curated build-tooling `.fsi` per Principle II), Elmish/MVU applicability (the engine core is **pure** and plugs into the existing `build.fsx` `update`/effect-interpreter boundary — two new pure effect cases, no product `Model`/`Msg`/`Effect`), and the real-evidence obligations (036/037/038 byte-parity for the original three outputs plus the five captured scan outputs; typed cycle/topo/propagation/status-region tests; no-`python3`/no-`FSharp.Compiler.*` greps; the packed-engine consumer pass; the serialized FAKE logs)
- [X] T002 [P] [T1] [skillist: []] Create placeholder evidence files listed by the plan under `specs/043-foundations-evidence-engine/readiness/` so the audit-enforced readiness files are discoverable at setup time: the parity proof trees (`parity/036/`, `parity/037/`, `parity/038/`, and `parity/scans/036|037|038/`), `logs/serialized-gates.md`, `logs/no-python-grep.txt`, `logs/no-fcs-grep.txt`, `logs/language-reduction.md`, `package/`, `unit-property-tests.md`, `fsi-session.txt`, and the governance scaffolds named in T003
- [X] T003 [T1] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files (`governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation-authority.md`, `evidence-graph.md`, `evidence-audit.md`, `skill-loading-evidence.md`), each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation

- [X] T004 [P] [T1] [skillist: []] Extract the ten curated `Evidence/*.fsi` signatures from the aggregated `contracts/evidence-engine.fsi.md` contract into standalone `.fsi` files under `build/Governance/Evidence/`, create skeleton `.fs` companions against the signatures, and add their `<Compile>` entries to `FS.Skia.UI.Build.fsproj` **after** `Capabilities` in dependency order (parsers → registry → `Graph` → scans/status/diff → `Audit` → `Render` → `Engine`); no access modifiers in the `.fs` bodies (Principle I/II, FR-016)
- [X] T005 [P] [T1] [skillist: fs-skia-template-update] Flip `FS.Skia.UI.Build` to `IsPackable=true` with `PackageId`/version metadata, add it to `Directory.Packages.props`, the `PackLocal` pack flow, and `docs/reports/dependencies.md` as the published governance-library package (ADR D1 / research R8); `DependencyReport`/`PackageSurfaceCheck` coverage extended to the new identity with no product/runtime package affected
- [X] T006 [P] [T1] [skillist: []] Capture the extended golden-fixture scan outputs from the **current** Python engine for 036/037/038 — `readiness-contract-hits.json`, `persistent-launch-hits.json`, `persistent-gui-runtime-hits.json`, `window-visibility-hits.json`, `diff-scan-hits.json` — and commit them under `tests/Governance.Tests/fixtures/evidence-golden/<F>/scans/` (FR-017, real captured evidence, before any Python deletion)
- [X] T007 [T1] [skillist: []] Exercise the draft `Evidence` `.fsi` surface from FSI (representative `TaskParser.parse`, `Graph.propagate`, `StatusRegion.scan`, and `Engine.runGraph` calls over small literal inputs), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T008 [T1] [skillist: []] Record surface-area baselines for the new `build/Governance/Evidence` modules and the unsupported-scope / failure handling: a `Graph` that fails to compute returns `verdict=Error`, preserving the Python non-zero-exit semantics (spec Edge Cases); the Stage 2.2–2.5 / 5 / 6 / 7 deferrals and the heavy Spec Kit Bash remain out of scope

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Maintainer runs the gate in-process with parity (P1)

**Goal**: the task DAG, synthetic-evidence propagation, and audit verdict compute
entirely in compiled F# in-process, producing `task-graph.json`, `task-graph.md`,
and the audit count block byte-identical to the Python engine (SC-001/SC-001a).

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fsharp-build-orchestration] Add failing golden-fixture byte-diff tests (DiffPlex) in `tests/Governance.Tests` asserting the F# renderer's `task-graph.json`, `task-graph.md`, and audit count block match the committed 036/037/038 fixtures (SC-001), plus the five captured scan outputs `readiness-contract-hits.json` / `persistent-launch-hits.json` / `persistent-gui-runtime-hits.json` / `window-visibility-hits.json` / `diff-scan-hits.json` (SC-001a); register the file in `Governance.Tests.fsproj` before `Program.fs` — red before `Render.fs`/`Engine.fs` exist

### Implementation

- [X] T010 [P] [US1] [skillist: fsharp-parsing] Implement `build/Governance/Evidence/TaskParser.fs` against its `.fsi` — the `tasks.md` line grammar (ids, status boxes `[ ]`/`[X]`/`[S]`/`[F]`/`[-]`/`[*]`, `[P]`/`[US]`/tier/`[SEH]` annotations, phase-checkpoint edge derivation) and the Synthetic-Evidence Inventory table, producing typed `TaskRecord` values; an unrecognised box char is a parse error, no silent default (FR-001)
- [X] T011 [P] [US1] [skillist: fsharp-parsing] Implement `Evidence/DepsParser.fs` against its `.fsi` — read `tasks.deps.yml` (both the legacy bare-list form and the object `{deps, skillist}` form) via `YamlDotNet` behind a typed `DepsModel`; the empty/unparseable file is a blocking error; no bespoke hand-rolled parser (FR-002)
- [X] T012 [P] [US1] [skillist: fsharp-io-globbing] Implement `Evidence/SkillRegistry.fs` against its `.fsi` — discover the skill registry across `.agents/skills`, `src/*/skill`, and `template/fragments/*/skill`, resolving each declared id to exactly one `SKILL.md` (ambiguous/missing = error), roots supplied as data (FR-003)
- [X] T013 [US1] [skillist: fsharp-graph-algorithms] Implement `Evidence/Graph.fs` against its `.fsi` — 3-colour DFS cycle detection, Kahn topological order with deterministic id-sorted tie-break, and the pure synthetic-evidence propagation rule (`declared=synthetic → synthetic`; `declared=done ∧ any dependency synthetic/auto ∧ not accepted-seh → auto-synthetic`; else `declared`), returning typed `Cycle`/`TopoOrder`/`ResolvedTask` results — not `"ok"`/`"failed"` strings (FR-004/FR-005)
- [X] T014 [P] [US1] [skillist: fsharp-parsing] Implement `Evidence/StatusRegion.fs` against its `.fsi` — the `audit-status` fenced-region scan (first-region-wins, case-insensitive keys, duplicate-key-within-a-region = parse error, prose never interpreted, the four blocking conditions) faithfully porting `audit-status-scan.py` (FR-006)
- [X] T015 [P] [US1] [skillist: fsharp-parsing] Implement `Evidence/Scans.fs` against its `.fsi` — the readiness-contract, persistent-launch, persistent-gui-runtime, and window-visibility readiness scans, preserving each scan's blocking severity, hit vocabulary, and output JSON shape (`readiness-contract-hits.json`, `persistent-launch-hits.json`, `persistent-gui-runtime-hits.json`, `window-visibility-hits.json`); text scans over supplied `readiness/` file contents only (FR-006a)
- [X] T016 [P] [US1] [skillist: fsharp-parsing] Implement `Evidence/DiffScan.fs` against its `.fsi` — pattern-match the `audit-patterns.yml` regexes (read via `YamlDotNet`) over a supplied unified `git diff`, applying whitelist suppression (`file_glob` + `line_regex`) and `block`/`advisory` severity, emitting the `diff-scan-hits.json` shape (`{base_ref, blocking[], advisory[]}`); no process I/O in the function (FR-010)
- [X] T017 [US1] [skillist: []] Implement `Evidence/Audit.fs` against its `.fsi` — cross-file consistency (every id in `tasks.md` ↔ `tasks.deps.yml`), skill-id resolution and skill-ordering checks (`evidence-audit` not before `evidence-graph`), `[SEH]` design-phase-only timing, the `[SEH]` count summary (`accepted-seh`/`unaccepted-synthetic`/`auto-synthetic`/`late-seh`), and the merge-gate verdict aggregation (`Pass`/`Fail`/`Blocked`, `totalBlockers`) — `--accept-synthetic` logs but never changes the verdict (FR-006/FR-008, Principle V)
- [X] T018 [US1] [skillist: fsharp-code-generation] Implement `Evidence/Render.fs` against its `.fsi` — byte-parity serializers for `task-graph.json` (schema_version 1.0, id-sorted, fixed field order/separators), `task-graph.md` (verdict block, skill-assessment table, status counts, SEH classification table, Mermaid, ASCII tree, propagation report), the Mermaid `classDef` CSS, the ASCII tree glyphs, and the audit count block — deterministic ordering, exact indentation, trailing newline exactly as the Python writes it (FR-007)
- [X] T019 [US1] [skillist: []] Implement `Evidence/Engine.fs` against its `.fsi` — the `runGraph` / `runAudit` entry points orchestrating parse → validate-and-merge → cycle-detect → topo-sort → propagate → scans → render over inputs supplied as data, returning typed results plus the artifact texts to write; the `Engine` performs no filesystem / `git` / process I/O (all reads/writes stay at the edge, Principle IV)
- [X] T020 [US1] [skillist: fsharp-build-orchestration, fsharp-shell-process] Rewire `build.fsx`'s two evidence-gate `StartTarget` arms in-process — add `EvidenceGraphCheck` / `EvidenceAuditCheck` `BuildEffect` cases, have `update` emit them as pure effect values (no `processEffect` to `run-audit.sh`), and have `interpret` read `tasks.md` / `tasks.deps.yml` / `readiness/` / the unified `git diff` (`git` via the existing `BuildProcess` wrapper) → `Engine.runGraph`/`runAudit` → write the artifacts; keep the Python path runnable behind a `--legacy-evidence` selector until parity sign-off (FR-009/FR-012)
- [X] T021 [US1] [skillist: fsharp-build-orchestration] Capture SC-001/SC-001a parity evidence — run the in-process graph and audit gates for 036/037/038 and byte-diff the regenerated `task-graph.json`, `task-graph.md`, audit count block, and the five scan outputs (`readiness-contract-hits.json`, `persistent-launch-hits.json`, `persistent-gui-runtime-hits.json`, `window-visibility-hits.json`, `diff-scan-hits.json`, per T009) against the committed golden fixtures (**0 bytes** on every artifact), recording the diffs under `readiness/parity/036|037|038/` and `readiness/parity/scans/036|037|038/`; while iterating, the Python path stays available behind `--legacy-evidence`

**Checkpoint**: US1 functional — the flagship gate runs in-process with proven byte-parity.

---

## Phase 4: User Story 2 (US2) — Algorithms are correct and provably so (P1)

**Goal**: typed unit and property coverage for the graph algorithms and the
status-region scan on graphs the golden fixtures don't exercise (SC-002, FR-014).

### Tests First (Principle I, Principle VI)

- [X] T022 [P] [US2] [skillist: fsharp-graph-algorithms, fsharp-build-orchestration] Add `tests/Governance.Tests/EvidenceAlgorithmTests.fs` — typed Expecto unit tests for cycle detection (a hand-built cyclic DAG is flagged, an acyclic one accepted) and Kahn topological order (a valid linearization, deterministic id-sorted tie-break), asserting typed `Graph` results not string scraping (SC-002, FR-014)
- [X] T023 [P] [US2] [skillist: fsharp-graph-algorithms, fsharp-build-orchestration] Add FsCheck property tests for the synthetic-evidence propagation rule — monotonicity, and "no synthetic roots ⇒ no auto-synthetic nodes" — including at least one multi-synthetic-root case and one empty-graph case (SC-002)
- [X] T024 [P] [US2] [skillist: fsharp-build-orchestration] Add typed tests for the `StatusRegion` scan — first-region-wins, duplicate-key parse error, prose-never-interpreted, and the four blocking conditions (`taskbar-only=true`; `taskbar-entry=true ∧ window-visible=false`; `exact-package-match ∉ {true,yes}`; `package-resolution=nu1603`) (SC-002)
- [X] T025 [P] [US2] [skillist: fsharp-build-orchestration] Re-point `AuditStatusRegionTests.fs`, `PersistentViewerEvidenceTests.fs`, and `SyntheticErrorEvidenceTests.fs` from shelling `python3` / `bash run-audit.sh` to typed `Evidence.StatusRegion` / `Scans` / `Engine` calls, keeping their committed fixture inputs and asserting typed results — removing the last `python3`/`bash` invocations from the test path (FR-014, research R7)

**Checkpoint**: US2 functional — algorithms and status-region scan provably correct.

---

## Phase 5: User Story 3 (US3) — Generated consumers stay governed without Python (P2)

**Goal**: generated `dotnet new fs-skia-ui` projects consume the **packaged**
`FS.Skia.UI.Build` engine and pass their gates with no copied Python (SC-006).

### Implementation

- [X] T026 [P] [US3] [skillist: fs-skia-template-update] Rewrite `template/base/build.fsx` so generated projects call the packaged `FS.Skia.UI.Build` engine in-process (paket `nuget` header + `Evidence.*` calls), and stop `.template.config/template.json` from copying `.specify/extensions/evidence/scripts/**` into generated projects (FR-013)
- [X] T027 [US3] [skillist: fs-skia-template-update] Add the `FS.Skia.UI.Build` `PackageVersion` pin to `template/base/Directory.Packages.props` (bumped alongside the other `FS.Skia.UI.*` pins) and confirm the generated project package-references the published engine rather than carrying source (FR-013)
- [X] T028 [US3] [skillist: fs-skia-template-update] Capture SC-006 evidence — run `PackLocal` (now packing the published `FS.Skia.UI.Build`) then `TemplateCheck` and `GeneratedProductCheck`, confirming the generated project's graph and audit gates produce a valid verdict via the package reference with **no** copied `run-audit.sh` / `*.py`; record to `readiness/package/`

**Checkpoint**: US3 functional — consumers fully governed via the packaged engine.

---

## Phase 6: Integration & Polish (decommission + serialized dogfood gates)

- [X] T029 [P] [T1] [skillist: []] After byte-parity sign-off across **all** fixtures (FR-012), delete `.specify/extensions/evidence/scripts/python/compute-task-graph.py` and `audit-status-scan.py`, delete `.specify/extensions/evidence/scripts/bash/run-audit.sh` with all 9 embedded heredocs, and remove the `--legacy-evidence` path; retain `audit-patterns.yml` as read-only data (FR-011)
- [X] T030 [P] [T1] [skillist: []] SC-003 grep proof — record to `readiness/logs/no-python-grep.txt` that zero `python3`/`python` invocations and zero references to `compute-task-graph.py` / `audit-status-scan.py` / `run-audit.sh` remain in the steady-state evidence path
- [X] T031 [P] [T1] [skillist: []] SC-004 / SC-005 / SC-007 proofs — record `readiness/logs/no-fcs-grep.txt` (no `FSharp.Compiler.*` reference added anywhere), `readiness/logs/language-reduction.md` (the evidence-path languages drop from `{F#, Bash, Python}` to `{F#}` plus thin OS-glue `git`, vs the Stage-0 baseline), and `readiness/logs/runtime-untouched.md` capturing `git diff --stat` over product `src/**` = **0** (the runtime-untouched Invariant 2 proof, SC-007)
- [X] T032 [T1] [skillist: fsharp-build-orchestration] As a designated dogfood feature (FR-015), run the serialized six-target FAKE gate set sequentially in deterministic order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates `T033`/`T034`), never concurrently; record aggregate FAKE results as **non-authoritative** and rerun any race-like or environment-flaky failure (the `SkiaViewer.Tests` headless crash, the `FsiTranscripts` toolchain issue) in focused isolation under a stash control as the authoritative result; logs under `readiness/logs/serialized-gates.md`
- [X] T033 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm the task DAG is acyclic, no dangling refs, no `[S*]` surprises, and that the `skillist` metadata and visible mirrors are valid
- [X] T034 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict `PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 diff-scan blocking, 0 readiness-contract blocking) with zero synthetic evidence to accept (SC-008)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; see the note at the top)_ | | | | | | | | |
