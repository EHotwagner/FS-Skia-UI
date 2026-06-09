# Implementation Plan: Governance Gate Hardening

**Branch**: `087-governance-gate-hardening` | **Date**: 2026-06-09 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/087-governance-gate-hardening/spec.md`

## Summary

Harden six concrete weaknesses observed end-to-end during feature-086 so the
governance gates stay **trustworthy signals**. All six are changes to the
compiled governance engine `FS.Skia.UI.Build` (`build/Governance/**`) — no
`src/**/*.fsi` public surface changes, no product/runtime behavior changes. The
core moves: (1) give every generated product a resolvable feature context so
`GeneratedProductCheck` can reach a real green, and classify each step's failure
as product-defect vs environment independently; (2) add a static pinned-vs-local
package-skew check that compares generated-source API references against the
pinned package's already-captured surface baselines; (3) fold per-package surface
baselines into `RefreshSurfaceBaselines` with byte-idempotent writes; (4) add a
three-state audit verdict (`Pass` / `PassWithAcceptedDeferrals` / `Fail`) with
accepted deferrals recorded as durable structured data; (5) restrict `[S*]`
propagation to real data dependencies (`ExplicitDeps`) so phase-checkpoint edges
no longer contaminate; (6) add a provenance field to skill-loading-evidence rows
and surface a missing load at implementation time. FR-011 is the invariant
spanning all of it: no genuine block is relaxed to obtain a green.

## Technical Context

**Language/Version**: F# / .NET 10 (`net10.0`)
**Primary Dependencies**: None new. Existing `FS.Skia.UI.Build` engine (Expecto + FsCheck for `Governance.Tests`, FAKE targets, DiffPlex for golden diffs).
**Testing**: Expecto + FsCheck property tests in `tests/Governance.Tests/`; FAKE-target evidence runs into `readiness/`; seeded-input fixtures for verdict/skew/propagation.
**Target Platform**: Linux + Windows (governance engine is host-OS-agnostic pure F#).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Template-affecting. `template/base/docs/evidence-formats.md`
  changes (new skill-loading-evidence `provenance` column + accepted-deferral
  record shape). `GeneratedProductCheck`'s FR-001 fix adds a generated
  `.specify/feature.json` (carrying a usable `feature_directory`) to the
  generated tree (per research.md R1; step-split is the documented fallback
  only) — a template/scaffold concern. No `.template.config/template.json`
  identity change; no new generated capability. Command-surface text of the
  `/speckit-*` skills is **not** changed beyond FR-010's earlier-gap surfacing.
- **Dependency impact**: N/A — no `Directory.Packages.props` consumer-pin change,
  no new NuGet dependency, no `docs/dependencies.md` or `DependencyReport`
  coverage change. (FR-003's skew check compares against *existing* captured
  surface baselines; it adds no package dependency.)
- **Command-surface impact**: `build.fsx`/`build/Governance/**` change. Targets
  affected: `GeneratedProductCheck` (FR-001/002/004), `TemplateCheck` (FR-004),
  `RefreshSurfaceBaselines` (FR-005/006), `EvidenceAudit` (FR-007/008/009),
  `EvidenceGraph` (FR-009); a new static skew check (target or sub-check of
  `TemplateCheck`/`GeneratedProductCheck`, FR-003). `Route` may re-route as
  governance paths change; `validation.contract.yml` regenerates from
  `Routing.fs` and is currency-checked by `TargetMetadataDrift`. FAKE-backed
  targets are run **sequentially** in the deterministic six-target order; only
  non-FAKE reads/tests run in parallel. Escalated order for this change:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: The generated tree gains a resolvable feature
  context for FR-001 — a usable `.specify/feature.json` `feature_directory` (per
  research.md R1; step-split fallback only); forbidden-artifact and api-surface
  scans for generated products are unchanged. No change to default/minimal
  generated contents, selected Controls guidance, or generated `Dev` behavior.
- **Evidence paths**: All under `specs/087-governance-gate-hardening/readiness/`:
  - `generated-product-check-green.txt` — clean-tree `GeneratedProductCheck` green run log (FR-001/002, SC-001).
  - `generated-product-defect-classification.txt` — seeded product defect + concurrent env obstacle → product-defect verdict (FR-002, SC-002).
  - `package-skew-seeded.txt` / `package-skew-clean.txt` — skew check fails on seeded unpinned-API ref (names symbol+file+version gap), passes on real tree, no network restore (FR-003/004, SC-003/004).
  - `refresh-surface-baselines-idempotent.txt` — `RefreshSurfaceBaselines` run twice, `git status` clean (FR-005/006, SC-005/006).
  - `audit-three-verdicts.txt` + `seh-audit-summary.json` samples — clean-PASS / PASS-with-accepted-deferrals / FAIL on three seeded inputs (FR-007/008, SC-007).
  - `synthetic-propagation-no-phase-edge.txt` — leaf `[S]` propagates `[S*]` to zero phase-edge-only tasks (FR-009, SC-008).
  - `skill-loading-evidence-provenance.md` + at-implementation gap report (FR-010, SC-009).
  - `true-positive-gates-still-block.txt` — diff-scan / additive-surface / window-visibility / persistent-launch / synthetic-honesty still block on seeded real violations (FR-011, SC-010).
- **`.fsi` / contract impact**: No `src/**/*.fsi` public-surface change. The
  changed *contracts* are governance-internal and generated from single sources:
  `validation.contract.yml` (from `Routing.fs`), target metadata, the audit's
  machine-readable verdict schema (`seh-audit-summary.json` gains a verdict-state
  enum + accepted-deferral records), and `docs/evidence-formats.md` (the
  skill-loading provenance field + accepted-deferral record shape). Currency
  enforced by `TargetMetadataDrift` / `SkillSyncCheck`.
- **MVU/effect boundary**: The engine keeps its existing pure-`update` /
  edge-interpreter split (`Engine/{Model,Update,Interpret}.fs`,
  `Front/Governance.fs`). All six fixes stay on the **pure** side where possible:
  verdict computation (`Audit.verdict`), `[S*]` propagation (`Graph.propagate`),
  and skew comparison are pure functions over `tasks.md` / `tasks.deps.yml` /
  `readiness/` / surface baselines. Only the FR-001 feature-context provisioning
  and the green-run I/O live at the interpreter edge. No new effect algebra, no
  new host I/O classes beyond reading already-present baseline files.
- **Synthetic evidence**: This feature *fixes* the synthetic machinery; its own
  evidence is real (seeded fixtures are real inputs to pure functions, exercised
  through `Governance.Tests` + FAKE runs). Seeded "defect"/"skew"/"deferral"
  fixtures are real test inputs, not synthetic substitutes for missing product
  capability — no `[S]` expected. If any seeded error-path input is infeasible to
  produce really, it is disclosed per Principle V at task time, not relabeled.
- **Test evidence**: Failing-first `Governance.Tests` (Expecto + FsCheck) for:
  three-state verdict (property: accepted-deferral never masks an unaccepted
  synthetic or blocking hit); `[S*]` propagation over `ExplicitDeps` only
  (property: a phase-edge-only downstream of an `[S]` leaf is never `[S*]`);
  skew detection (symbol present-in-source/absent-in-pinned-baseline →
  finding); skill-loading provenance parse/validate; per-package baseline
  idempotence (byte-equal rerun). Target-level evidence per the Evidence paths
  above. Each test fails before the change and passes after.
- **Observability**: Each fix emits actionable structured diagnostics. FR-002:
  per-step classification names the failing step + product-defect|environment.
  FR-003: skew finding names symbol + file + pinned-vs-local version gap. FR-004:
  every generated-product report states its package set (local-packed vs pinned).
  FR-008: audit surfaces unaccepted-vs-accepted synthetic counts separately and
  records accepted deferrals as structured data. FR-010: gap report names the
  task + missing skill at implementation time. No silent failure; missing
  artifact classes still fail loudly.
- **Deferred scope**: Out of scope (per spec "Unsupported scope"): the
  keyboard-injection harness (the 086 deferral artifact itself), any
  product/runtime/platform/distribution change, retrofitting historical
  features' readiness, and any `/speckit-*` workflow-command text change beyond
  FR-010's earlier-gap surfacing. FR-001's chosen path (research.md R1) is a
  resolvable generated feature context (a usable `.specify/feature.json`);
  **splitting the authoritative build/test step from the env-dependent `Verify`
  step remains the documented fallback** only if feature resolution proves
  infeasible at implementation.

**Initial Constitution Check: PASS.** No principle violated. Tier 1
(contracted change) at the governance-contract level — it changes inter-tool
contracts (verdict schema, evidence-formats, target metadata) generated from
single sources, with no `src/**/*.fsi` change. No complexity-justification items
(no SRTP/reflection/type-providers/custom-CE introduced).

## Project Structure

```
build/Governance/                         # FS.Skia.UI.Build — single home of all rules
  Engine/Model.fs                         # FR-001: activeFeatureId resolution + feature-context provisioning
  Engine/Update.fs                        # FR-001/004: GeneratedProductCheck/TemplateCheck effect shaping, report package-set field
  Evidence/Graph.fs                       # FR-009: propagate over ExplicitDeps only (line ~128 allDeps → explicit)
  Evidence/TaskParser.fs                  # FR-009: keep PhaseDeps for ordering/cycle, exclude from taint
  Evidence/Audit.fs                       # FR-007/008: three-state verdict, accepted-deferral records; FR-010 skill-loading provenance
  Evidence/EvidenceFormatSchema.fs        # FR-008/010: verdict-state enum, accepted-deferral + provenance schema (single source)
  Evidence/Render.fs                      # FR-008: seh-audit-summary.json verdict-state + accepted/unaccepted counts
  Front/Governance.fs                     # FR-001/002/003/004: interpreter edge — green run, per-step classification, skew check, package-set reporting
  PerPackageSurface.fs                    # FR-005/006: captureCurrent invoked by RefreshSurfaceBaselines, byte-idempotent writes
  Routing.fs                              # routing for new/changed governance paths; source of validation.contract.yml
  Targets.fs                              # target metadata (new skew sub-check if a discrete target)

template/base/docs/evidence-formats.md    # FR-008/010: provenance field + accepted-deferral record shape (generated/contract)
template/base/.specify/feature.json       # FR-001 (R1 decision): generated resolvable feature context
validation.contract.yml                   # regenerated from Routing.fs

tests/Governance.Tests/                   # Expecto + FsCheck: verdict, propagation, skew, provenance, idempotence

specs/087-governance-gate-hardening/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/                              # governance-internal contracts (this feature)
  readiness/                              # evidence artifacts (see Evidence paths)
```

## Phase 0 — Outline & Research

See [research.md](./research.md). Resolves the open design choices:
- **R1 (FR-001)** — resolvable generated feature context vs. step-split fallback.
- **R2 (FR-003)** — static skew comparison source (pinned surface baseline) + no-network-restore strategy.
- **R3 (FR-006)** — root cause of trailing-newline churn and the byte-idempotent write rule.
- **R4 (FR-007/008)** — verdict-state model and accepted-deferral durable record placement.
- **R5 (FR-009)** — `ExplicitDeps`-only taint, retaining `PhaseDeps` for ordering/cycle detection.
- **R6 (FR-010)** — provenance signal source (captured vs asserted) and the at-implementation-time gap surface.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — the engine value types that change: the
  three-state `AuditVerdict`, the `AcceptedDeferral` record, the skill-loading
  `LoadProvenance` field, the `PackageSet` report tag, the `PackageSkewFinding`,
  and the propagation-input split (`ExplicitDeps` for taint vs `allDeps` for
  ordering).
- [contracts/](./contracts/) — governance-internal contract shapes: the
  `seh-audit-summary.json` verdict schema, the accepted-deferral record, the
  skill-loading-evidence row schema (with `provenance`), the package-skew finding
  shape, and the per-step product-defect/environment classification signal.
- [quickstart.md](./quickstart.md) — how a maintainer exercises each fix and
  reads its verdict.

**Agent context update**: `AGENTS.md` SPECKIT plan reference points at this plan.

**Post-Design Constitution Re-check: PASS.** Design stays pure where the spec
requires (verdict/propagation/skew are pure functions), keeps the
`update`-pure/interpreter-edge boundary, introduces no new dependency or public
`.fsi` surface, and preserves every true-positive block (FR-011) — the new
verdict state is an *additional* PASS category that can never pass an unaccepted
synthetic or a blocking hit.

## Phase 2 — Stop

Planning ends here. `/speckit-tasks` generates `tasks.md` + `tasks.deps.yml`
next. The escalated six-target order above is the verification path for this
governance-contract change.
