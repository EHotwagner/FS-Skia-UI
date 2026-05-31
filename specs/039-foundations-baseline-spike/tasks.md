# Tasks: Foundations Baseline & Build-Library Spike

**Feature branch**: `039-foundations-baseline-spike`
**Spec**: `specs/039-foundations-baseline-spike/spec.md`
**Plan**: `specs/039-foundations-baseline-spike/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed by the evidence audit from the DAG; never write
it by hand. `[SEH]` is an annotation paired with
`synthetic-error-handling-approved`, assigned only during design/planning/task
generation — not at implementation time.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when its outcome is reachable from a
user-facing entry point and that path was actually exercised. For this feature:
US1's reachable path is the real `dotnet run … -- SpikeHello` invocation; US2's
is the real re-run-and-diff of the existing evidence commands; US3/US4's is the
committed, reviewer-readable document. No stateful or I/O-bearing runtime
workflow is added, so **Principle IV (MVU/effect boundary) is not applicable**
to any task here (recorded in T003).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- Whole feature is **Tier 1**; per-task `[T1]`/`[T2]` annotations are omitted
  because every phase matches the feature's overall tier.

Every task has a matching key in `tasks.deps.yml` (with `deps` and `skillist`),
and every task line mirrors its structured `skillist` as `[skillist: …]`
(`[skillist: []]` when empty). `speckit.evidence.graph` refuses to proceed on
dangling refs, mirror mismatches, or unresolved skill ids.

## Canonical Verification Targets & FAKE serialization

FAKE-backed commands (`./fake.sh`, `fake.cmd`, `dotnet fake`) share `.fake`
state and are **not** safe to run concurrently. Run the no-regression sequence
sequentially in the canonical order: `Dev` → `GeneratedGuidanceCheck` →
`TemplateCheck` → `GeneratedProductCheck` → `DependencyReport` → `TemplateDrift`
→ `EvidenceGraph` → `EvidenceAudit`, plus `PackageSurfaceCheck` /
`FsiTranscripts` for the surface invariant. The
spike's own `dotnet run --project build/Build.fsproj` is **not** FAKE-backed and
does not touch `.fake` state, but is still run separately from any FAKE target.

## Task-graph pitfall guidance (read before `EvidenceGraph`)

Avoid title trigger phrases that imply unrelated capabilities (e.g. "persistent
GUI runtime" or "window visibility validation fixture") — this feature owns no
viewer/window evidence. The no-regression aggregate task (T024) deliberately
omits the literal target names `EvidenceGraph`/`EvidenceAudit` from its title so
it is not mis-classified into the graph/audit trigger groups; the actual graph
and audit gates are the discrete tasks T025/T026 carrying the matching skills.
Setup readiness aggregation uses the `Complete readiness notes` prefix to
suppress capability-expectation checks.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory scaffold and links between `spec.md`, `plan.md`, `data-model.md`, `research.md`, `quickstart.md`, and `contracts/` are present and consistent
- [X] T002 [P] [skillist: []] Complete readiness notes scaffolding — create placeholder readiness files discoverable before implementation (`readiness/logs/`, `readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, `readiness/evidence-graph.md`, `readiness/evidence-audit.md`), each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier 1, affected layer (build-tooling projects only — **no** runtime under `src/**`), public-API impact (no tracked runtime surface diff; one new build-tooling `.fsi`), and evidence obligations; state explicitly that **Principle IV (MVU/effect boundary) is not applicable** (no stateful/I-O runtime workflow) and that **no synthetic evidence** is anticipated

---

## Phase 2: Foundation

- [X] T004 [P] [skillist: []] Add central `Fake.Core.Target` (+ minimal `Fake.Core.*` companion) `PackageVersion` entries to `Directory.Packages.props` and the matching build-tooling rows to `docs/dependencies.md` (need / version-pinning / owner); declare **no** `FSharp.Compiler.Service` and no `PackageVersion` outside central package management (FR-012)
- [X] T005 [skillist: []] Draft the governance library public surface as `build/Governance/Spike.fsi` — the single curated signature `val run : unit -> string` (Principle II) — and record that this is a new build-tooling surface, **not** a tracked runtime surface baseline, so `PackageSurfaceCheck`/`FsiTranscripts` must show no diff

**Checkpoint**: Foundation ready — dependency disclosure and the library `.fsi` contract exist; story work may begin.

---

## Phase 3: User Story 1 — De-risk D2 via the spike (US1, P1)

### Tests First

- [X] T006 [P] [US1] [skillist: []] Add the spike-target verification scaffold (failing-first): assert that invoking `SpikeHello` via `dotnet run --project build/Build.fsproj` must print the exact value returned by `Spike.run` (proving the body ran from the library, not inlined) and that `dotnet list build/Build.fsproj package --include-transitive` shows no `FSharp.Compiler.*` (contract `spike-target.contract.md`)

### Implementation

- [X] T007 [US1] [skillist: []] Create the governance library project `build/Governance/FS.Skia.UI.Build.fsproj` (`net10.0`, inherits `Directory.Build.props`) with `Spike.fsi` + `Spike.fs` implementing `run` as a trivial, identifiable success-message body (FR-005)
- [X] T008 [US1] [skillist: []] Create the dedicated build front-end `build/Build.fsproj` (Exe) + `Program.fs` that references the library and registers one `SpikeHello` target whose body is **only** a call into `Spike.run` (no inlined logic), dispatched via `Fake.Core.Target.runOrDefault` (FR-006)
- [X] T009 [US1] [skillist: []] Add `build/Build.fsproj` and `build/Governance/FS.Skia.UI.Build.fsproj` to `FS-Skia-UI.sln` additively — confirming the additions change no existing target's output (FR-010, invariant 6)
- [X] T010 [US1] [skillist: []] Build both projects with zero warnings (`dotnet build … -warnaserror`) under `net10.0` / `TreatWarningsAsErrors`; confirm every package version lives in `Directory.Packages.props` and `dotnet list … package --include-transitive` shows **no** `FSharp.Compiler.*` (FR-012, SC-003)
- [X] T011 [US1] [skillist: []] Run `dotnet run --project build/Build.fsproj -- SpikeHello` separately from any FAKE target (not FAKE-backed, no `.fake` state) and capture the output proving the success line is the value returned from `Spike.run` (SC-004)
- [X] T012 [US1] [skillist: []] Record the spike outcome in `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md` as exactly `"D2 confirmed"` or `"fallback triggered"` — including the `dotnet run` command, its output, the FCS-absence result, and (if fallback) the named reproducible blocker plus the thin-`build.fsx` `#r`-the-DLL shim documented as the Stage 5 path (FR-007, SC-004)

**Checkpoint**: US1 is independently testable — the front-end drives a target whose logic lives in the library, and the D2 confirm/fallback outcome is unambiguous.

---

## Phase 4: User Story 2 — Capture baseline + golden fixtures (US2, P1)

### Capture

- [X] T013 [P] [US2] [skillist: []] Capture the baseline document `docs/reports/_baselines/2026-05-31-foundations.md`, SHA-pinned to the recorded commit: `build.fsx` line count with orchestration-vs-validation breakdown, governance Markdown counts (`.claude`↔`.agents` skill mirror, the governing-principles document under `.specify/memory/`, `templates/`, and `specs/**`), the F#/Bash/Python LOC mix, and the per-feature ceremony-time estimate — record the literal measurement command for every line-count/LOC metric so a reviewer can reproduce it; the per-feature ceremony-time figure is an explicit estimate (record its derivation inputs) and is exempt from the measurement-command rule (FR-001, SC-001)
- [X] T014 [P] [US2] [skillist: speckit-evidence-graph] Capture the golden task-graph fixtures (`task-graph.json` + `task-graph.md`) for features `038-authoring-guidance-consistency`, `037-authoring-audit-robustness`, and `017-synthetic-error-evidence` via the **existing** `EvidenceGraph` path (unchanged), archived under `tests/Governance.Tests/fixtures/evidence-golden/<feature>/` (FR-002)
- [X] T015 [P] [US2] [skillist: speckit-evidence-audit] Capture the golden audit count block (`audit-counts.txt`: `accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, `late-seh-tasks`) for the same three features via the **existing** `EvidenceAudit` path (unchanged), archived alongside their graph fixtures (FR-002)

### Verification

- [X] T016 [US2] [skillist: speckit-evidence-graph, speckit-evidence-audit] Prove the fixtures are byte-for-byte reproducible: re-run the existing evidence commands per feature and `diff` against the committed fixtures (empty diffs for all three files across all three features). If any re-run differs, remove the non-determinism (deterministic re-capture) or substitute a merged feature and record the substitution — never commit an unstable fixture (FR-003, SC-002)
- [X] T017 [US2] [skillist: []] Finalize the baseline's golden-fixture manifest (the three captured features, their fixture paths, and any recorded substitution), set each fixture's `source_commit` equal to the baseline SHA, link to `plan.md` §Programme Meta-Process, and designate the fixture set the **Stage 4 parity oracle** (FR-002, SC-001)

**Checkpoint**: US2 is independently testable — a reviewer reproduces every baseline number and regenerates all golden fixtures byte-for-byte from the pinned commit.

---

## Phase 5: User Story 3 — Record shaping ADRs (US3, P2)

- [X] T018 [P] [US3] [skillist: []] Write `docs/adr/0001-governance-library-placement-and-distribution.md` (D1) stating decision, alternatives, rationale, and the stages it shapes (FR-004, SC-005)
- [X] T019 [P] [US3] [skillist: []] Write `docs/adr/0002-build-front-end-form.md` (D2) stating decision, alternatives, rationale, and shaped stages, citing the recorded spike outcome (FR-004, SC-005)
- [X] T020 [P] [US3] [skillist: []] Write `docs/adr/0003-generated-product-contract-versioning.md` (contract-versioning policy) stating decision, alternatives, rationale, and shaped stages (FR-004, SC-005)
- [X] T021 [P] [US3] [skillist: []] Write `docs/adr/0004-spec-kit-fork-stance.md` (D4) stating decision, alternatives, rationale, and shaped stages (FR-004, SC-005)
- [X] T022 [P] [US3] [skillist: []] Write `docs/adr/0005-configuration-representation.md` (D6) stating decision, alternatives, rationale, and shaped stages (FR-004, SC-005)

**Checkpoint**: US3 is independently testable — one discrete, dated ADR exists per shaping decision, each with decision/alternatives/rationale/stages.

---

## Phase 6: User Story 4 — Establish the programme meta-process (US4, P3)

- [X] T023 [US4] [skillist: []] Record the programme meta-process in `plan.md` §Programme Meta-Process as the single discoverable place — default lightweight framework-author loop (governance/consumer-contract-touching features escalate) and the named dogfood feature set (Stage 1, Stage 4) — and cross-link it from the finalized baseline document (FR-008, SC-007)

**Checkpoint**: US4 is independently testable — the default tier and named dogfood set are recorded and discoverable in one place.

---

## Phase 7: Integration & Polish

- [X] T024 [skillist: []] Run the canonical serialized FAKE no-regression sequence (`Dev` -> `GeneratedGuidanceCheck` -> `TemplateCheck` -> `GeneratedProductCheck` -> `DependencyReport` -> `TemplateDrift`) plus `PackageSurfaceCheck` / `FsiTranscripts`, and the runtime-untouched `git diff --name-only` check over `src/**`; confirm the sequence is green with **no** surface baseline diff and that the new build-tooling `PackageVersion` entries are reflected without error in `DependencyReport`, then record the non-authoritative aggregate results in `readiness/logs/` (FR-009, FR-010, FR-012, SC-006). Results in `readiness/logs/no-regression.md`: `Dev`/`GeneratedGuidanceCheck`/`GeneratedProductCheck`/`DependencyReport`/`TemplateDrift`/`PackageSurfaceCheck` PASS; the two readiness gates (T025/T026) PASS; `src/**` untouched (0 changes); no surface diff. Two gates RED for pre-existing, feature-independent reasons (proven via a stash control): `FsiTranscripts` (`controls-prelude.fsx` exits 1 on this toolchain) and `TemplateCheck` (its `Test` target hits the known `SkiaViewer.Tests` headless flake); out of scope per FR-009/FR-011.
- [X] T025 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and that the resolved feature id and real task count are echoed
- [X] T026 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS (no synthetic propagation, no diff-scan hits) or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. For `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — all evidence is real per the plan's Synthetic-evidence decision: real `wc`/`git` baseline counts, real outputs of the existing evidence engine, and a real compiled spike target)_ | | | | | | | | |

## Implementation Notes (US1 — D2 confirmed)

US1's spike resolved to **D2 confirmed**: both build-tooling projects compile
clean under `net10.0`/`TreatWarningsAsErrors` (`0 warnings, 0 errors`), the
`SpikeHello` target runs via `dotnet run` and prints the value returned by
`FS.Skia.UI.Build.Spike.run` (proving the body ran from the library, not
inlined), and `dotnet list build/Build.fsproj package --include-transitive`
shows **no** `FSharp.Compiler.*` (FR-012). Evidence and exact commands are in
`docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`; the committed
verification scaffold `build/spike-verify.sh` reproduces the result
(`SPIKE-VERIFY PASS: D2 confirmed`). No synthetic evidence is involved.
