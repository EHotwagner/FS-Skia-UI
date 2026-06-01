# Implementation Plan: Foundations Evidence Engine Port (Stage 4)

**Branch**: `043-foundations-evidence-engine` | **Date**: 2026-06-01 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/043-foundations-evidence-engine/spec.md`

## Summary

Replace the tri-language evidence gate (`build.fsx → run-audit.sh (1,284 lines Bash) → compute-task-graph.py (1,310) + audit-status-scan.py (150) → JSON re-parsed in F#`) with typed, unit- and property-tested **compiled F#** inside `FS.Skia.UI.Build`, computing the evidence graph and the full merge-gate audit **in-process**. All nine embedded scans are ported (graph compute, SEH-summary, readiness-contract, persistent-launch, persistent-GUI, window-visibility, audit-status region, diff-scan, verdict). Parity is proven **byte-for-byte** against the Stage-0 golden fixtures (036/037/038) — extended with newly-captured golden fixtures for the five scan outputs that have no Stage-0 oracle — **before** any Python or Bash is deleted. Per the resolved consumer-distribution decision, `FS.Skia.UI.Build` becomes a **packed, published** package; generated `dotnet new fs-skia-ui` projects reference it and stop carrying the Python + `run-audit.sh`. The runtime architecture (`Scene → SkiaViewer → Elmish`) and every public product `.fsi` are untouched.

**Technical approach**: port-behind-a-parity-gate. The Python engine stays runnable in parallel behind a `--legacy-evidence` selector (FR-012) until byte-identical parity is signed off across **all** fixtures (original three outputs + five new scan outputs), only then are the Python files, `run-audit.sh`, and the legacy path deleted. The engine is plain compiled F# — no FSharp Compiler Services, no runtime-loaded `.fsx`, no bespoke YAML parser (`YamlDotNet` behind a typed model for `tasks.deps.yml`). Pure algorithms (`parse`, `cycle-detect`, `topo-sort`, `propagate`, `render`, each scan as a pure predicate over read inputs) are unit/property-tested; all filesystem reads, `git` invocation, and artifact writes stay at the `build.fsx` interpreter edge per Principle IV.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (inherits `Directory.Build.props`: `TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management).
**Primary Dependencies**: `YamlDotNet 17.1.0` (already central, reused for `tasks.deps.yml`); `Fake.Core.Target 6.1.4` (build-tooling only). **No new runtime dependency.** Explicitly **no** `FSharp.Compiler.*` (FR-016 / SC-004) and **no** new bespoke parser (FR-002).
**Testing**: Expecto (unit) + FsCheck (property: propagation monotonicity, no-synthetic-root invariant) in `tests/Governance.Tests`, asserting **typed** results (FR-014); golden-fixture byte-diff via DiffPlex (the established Stage-0 parity oracle); FAKE-target evidence from the serialized six-target dogfood run.
**Target Platform**: Windows and Linux (governance text artifacts only; no platform-specific runtime).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design — both pass.*

This change is a **Tier 1 (contracted change)** for the governance library surface only: it adds new public modules to `FS.Skia.UI.Build` (each requiring a curated `.fsi` per Principle II) and changes that package's *contents and packability*. It introduces **no** product `.fsi` change, **no** product surface-baseline diff, and **no** new dependency. The runtime is untouched (Invariant 2). It is a designated **dogfood** + consumer-contract change → escalates to the full serialized gate set under the `Route` policy.

### Repository Governance Decisions

- **Template ownership** — **Changes required.** `.template.config/template.json` stops copying the Python/bash evidence scripts into generated projects; `template/base/build.fsx` is rewritten to call the packaged engine in-process; `template/base/Directory.Packages.props` + the generated project gain a `FS.Skia.UI.Build` package pin. This is a `template/**` + `.specify/**` change → escalates gates (`TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`).
- **Dependency impact** — **No new package.** `YamlDotNet`/`Fake.Core.Target` already in `Directory.Packages.props`. `FS.Skia.UI.Build` itself becomes a **published package** (IsPackable `false → true`, added to `PackLocal` + the pack/version flow + `docs/reports/dependencies.md`); generated consumers add a `PackageVersion` pin for it. `DependencyReport` coverage extended to the new package identity. No `FSharp.Compiler.*` (grep-proven, SC-004).
- **Command-surface impact** — `EvidenceGraph` and `EvidenceAudit` change (now in-process effects, no `processEffect` to `run-audit.sh`). `PackLocal` gains the new package. `TemplateCheck`/`GeneratedProductCheck`/`GeneratedGuidanceCheck` exercised via the template change. FAKE-backed commands run **sequentially** in the canonical serialized order (Invariant 5); safe non-FAKE reads/tests may parallelize.
- **Generated project impact** — generated projects shift from copied Python + `run-audit.sh` to a `FS.Skia.UI.Build` package reference; their `EvidenceGraph`/`EvidenceAudit` produce a valid verdict via the packaged engine (SC-006). Default/minimal generated contents otherwise unchanged.
- **Evidence paths** (all under `specs/043-foundations-evidence-engine/readiness/`):
  - `logs/serialized-gates.md` — the six-target dogfood run log.
  - `parity/036|037|038/*.diff` — byte-diff proof (0 bytes) for `task-graph.json`, `task-graph.md`, audit count block.
  - `parity/scans/036|037|038/*.diff` — byte-diff proof for the five new scan outputs (FR-017).
  - `logs/no-python-grep.txt` — grep proof: zero `python3`/`python`/`run-audit.sh`/`compute-task-graph.py`/`audit-status-scan.py` in the steady-state evidence path (SC-003).
  - `logs/no-fcs-grep.txt` — grep proof: no `FSharp.Compiler.*` added (SC-004).
  - `logs/language-reduction.md` — {F#,Bash,Python} → {F#} recorded vs the Stage-0 baseline (SC-005).
  - `logs/runtime-untouched.md` — `git diff --stat` over product `src/**` = 0, the runtime-untouched Invariant 2 proof (SC-007).
  - `package/` — `FS.Skia.UI.Build` pack output + generated-consumer evidence-gate pass (SC-006).
  - `unit-property-tests.md` — typed test results for cycle/topo/propagation/status-region (SC-002).
- **`.fsi` / contract impact** — new curated `.fsi` per Evidence module (governance/internal-tooling surface, **not** product public contract). `PackageSurfaceCheck`/`FsiTranscripts` show **no product baseline diff** (Invariant 1). No product public-API or sample-contract change.
- **MVU/effect boundary** — the only interpreter touched is `build.fsx`'s `BuildEffect` interpreter: the `StartTarget EvidenceGraph`/`EvidenceAudit` arms stop emitting a `RunProcess`/`processEffect` to `run-audit.sh` and emit new in-process effect cases (`EvidenceGraphCheck`/`EvidenceAuditCheck`) interpreted by calling `FS.Skia.UI.Build.Evidence.*`. `update` stays pure (effect *data* emitted, not executed); all I/O (file reads, `git`, writes) lives in `interpret`. Pure-transition tests assert the emitted effect list; interpreter tests run against the real fixture filesystem.
- **Synthetic evidence** — **none planned.** This feature produces only real evidence (golden byte-diffs, typed test runs, grep proofs). Its own audit must return `verdict=PASS` with 0 `[S]`/`[S*]`/late-seh/diff-scan (SC-008). The hand-built cyclic/multi-synthetic-root/empty-graph **test fixtures** (SC-002) are test inputs, not synthetic *evidence*, and live in test files named with the `Synthetic` token where they model synthetic states.
- **Test evidence** — failing-first: golden byte-diff tests fail until the renderer matches; property tests for propagation; typed unit tests for cycle/topo/status-region. Re-point `AuditStatusRegionTests`, `PersistentViewerEvidenceTests`, `SyntheticErrorEvidenceTests` from shelling `python3`/`bash run-audit.sh` to the typed library (FR-014).
- **Observability** — every scan emits structured findings (`ValidationFinding`) with actionable diagnostics matching the Python vocabulary; missing-artifact classes fail loudly (no silent pass). Graph-compute failure preserves the Python `error`-verdict / non-zero-exit semantics (spec Edge Cases).
- **Deferred scope** — Stages 2.2–2.5, 5 (MEL-engine relocation / `build.fsx` retirement beyond the two evidence arms), 6, 7; the heavy Spec Kit Bash (`common.sh`, git scripts); the V3 modular package split. The `--legacy-evidence` selector and Python files are removed **in this feature** at parity sign-off (not deferred) per FR-011/FR-012.

**Gate result: PASS** (pre-design and post-design). No principle violation requires justification; the one Tier-1 surface (new governance `.fsi` modules) is curated per Principle II.

## Project Structure

New/changed paths (repo-relative):

```
build/Governance/
  FS.Skia.UI.Build.fsproj          # IsPackable true; +PackageId/version metadata; +Evidence compile items
  Evidence/
    TaskParser.fsi  / .fs          # tasks.md grammar → typed TaskRecord list (FR-001)
    DepsParser.fsi  / .fs          # tasks.deps.yml (bare-list + {deps,skillist}) via YamlDotNet (FR-002)
    SkillRegistry.fsi / .fs        # .agents/skills, src/*/skill, template/fragments/*/skill (FR-003)
    Graph.fsi       / .fs          # 3-colour DFS cycle detect + Kahn topo + pure propagate (FR-004/005)
    StatusRegion.fsi / .fs         # audit-status region scan (first-region-wins, dup-key error) (FR-006)
    Scans.fsi       / .fs          # readiness-contract, persistent-launch, persistent-gui, window-visibility (FR-006a)
    DiffScan.fsi    / .fs          # audit-patterns.yml pattern match over a supplied git diff (FR-010)
    Audit.fsi       / .fs          # cross-file consistency, SEH summary, verdict aggregation (FR-006/008)
    Render.fsi      / .fs          # task-graph.json/.md/mermaid/ascii + audit count block, byte-parity (FR-007)
    Engine.fsi      / .fs          # runGraph / runAudit entry points; returns typed results; reads passed in
build/Build.fsproj                 # (front-end; unchanged this feature — build.fsx remains active per Stage 5 deferral)
build.fsx                          # EvidenceGraph/EvidenceAudit arms rewired in-process; #load Evidence/*; +effect cases
.specify/extensions/evidence/scripts/  # DELETED at sign-off: python/*.py, bash/run-audit.sh (FR-011)
.specify/extensions/evidence/audit-patterns.yml  # retained (data, read by DiffScan)
.template.config/template.json     # stop copying evidence scripts into generated projects
template/base/build.fsx            # generated evidence path → packaged engine in-process
template/base/Directory.Packages.props  # +FS.Skia.UI.Build pin
Directory.Packages.props           # +FS.Skia.UI.Build version (published package)
tests/Governance.Tests/
  Governance.Tests.fsproj          # +new Evidence test files
  Evidence*Tests.fs                # typed unit + FsCheck property tests (FR-014, SC-002)
  AuditStatusRegionTests.fs        # re-pointed: typed StatusRegion instead of python3
  PersistentViewerEvidenceTests.fs # re-pointed: typed Scans/Engine instead of bash run-audit.sh
  SyntheticErrorEvidenceTests.fs   # re-pointed: typed Engine instead of python3/bash
  fixtures/evidence-golden/
    {036,037,038}/                 # existing: task-graph.json/.md, audit-counts.txt
    {036,037,038}/scans/           # NEW (FR-017): readiness-contract-hits.json, persistent-launch-hits.json,
                                   #   persistent-gui-runtime-hits.json, window-visibility-hits.json, diff-scan-hits.json
```

**Design artifacts**: [research.md](./research.md) · [data-model.md](./data-model.md) · [contracts/](./contracts/) · [quickstart.md](./quickstart.md).

## Phase 2 note

Per the planning workflow this command stops after Phase 0/1 artifact generation. Task breakdown (`tasks.md` + `tasks.deps.yml`) is produced by `/speckit.tasks`. The serialized six-target dogfood gate set is the exit gate (CLAUDE.md / AGENTS.md).
