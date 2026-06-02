# Feature Specification: Decommission, Measure, Document the New Normal

**Feature Branch**: `047-foundations-programme-closeout`
**Created**: 2026-06-02
**Status**: Draft
**Input**: User description: "Check what was implemented last feature and whether the implementation plan is up to date; then implement the next part of the plan — Stage 7 of `docs/reports/2026-05-31-1049-foundations-implementation-plan.md` (decommission interim scaffolding, produce the final before/after measurement report, document the new development model), continuing the foundations programme after Stages 0–6 (features 039–046)."

## Context & Motivation *(informative)*

This is **Stage 7** — the closeout — of the foundations programme. Stages 0–6 (features
039–046) extracted the entire build/governance engine out of the 4,688-line `build.fsx` into the
compiled, tested `FS.Skia.UI.Build` library; replaced the tri-language (F#/Bash/Python) evidence
gate with in-process F#; made the two-tier `Route` process authoritative via compiled `Routing.fs`;
single-sourced the `.claude`/`.agents`/constitution/skillist duplications; and codified the last
prose rules into self-enforcing gates with a versioned generated-product contract.

The keystone is built. Stage 7 does not add capability — it **confirms the programme's promises
against the Stage-0 baseline, removes any interim scaffolding, and documents the new development
model so it sticks** for the next contributor (human or agent).

**What verification at authoring time already shows (so this feature records, not redoes).** A
working-tree sweep confirms the interim scaffolding the plan's 7.1 names is **already gone** from
the tracked tree: no root `build.fsx` (deleted in feature 045 — only the generated consumer's
`template/base/build.fsx` thin front-end remains, by design), no `scripts/build/select-tier.fsx`,
no `run-audit.sh`, no `--legacy-evidence` flag, no `*.py` under `.specify/` (the evidence path is
{F#}-only since feature 043), and no `fake-cli`/`dotnet fake`/`FSharp.Compiler.*` (feature 045).
So **7.1 is primarily a verification-and-record task** producing grep-proof artifacts, not a
deletion task; the spec captures any residual that the sweep surfaces during implementation and
removes it.

The genuinely-remaining Stage-7 work is therefore: the **final before/after measurement report**
(7.2) comparing every promised dimension against the Stage-0 baseline; the **documentation of the
new normal** (7.3) across the contributor-facing surfaces plus a closing ADR; and the **dogfooding
retrospective + recurring full-pipeline schedule** (7.4) so the consumer-governance path cannot rot.

This is framework-tooling + governance-documentation work that **escalates** via `Route` (it
touches `CLAUDE.md`/`AGENTS.md`/governance docs and may touch `.gitignore`/scheduling config) to
the appropriate gate set. The runtime architecture (`Scene → SkiaViewer → Elmish`) and the
product's public `.fsi` surface are explicitly **untouched**.

## Clarifications

### Session 2026-06-02

- Q: Where does the "after" baseline live and what does it compare against? → A: A new
  `docs/reports/_baselines/2026-06-02-foundations-after.md` paired side-by-side with the Stage-0
  `docs/reports/_baselines/2026-05-31-foundations.md`; each row carries its reproduction command
  (same measurement-command discipline Stage 0 used) and a SHA pin, so the deltas are independently
  reproducible.
- Q: How is the dogfooding "recurring full-pipeline run" realized given the local-only toolchain?
  → A: As a committed, discoverable schedule definition (the repository's existing scheduling
  surface) plus a documented manual fallback command; the feature does not require a live CI
  service to exist, only that the recurring-run mechanism is defined, discoverable, and runnable.
- Q: What counts as a measurement "miss" that must be explained rather than silently accepted? → A:
  Any whole-programme definition-of-done dimension whose after-value does not reach the plan's
  target value is recorded with a written rationale in the after-baseline (mirroring the honest
  variance disclosures features 041/045/046 used), never padded or hidden.
- Q: Which dimension set defines the after-baseline's "100%" coverage, given the plan's 11-row
  definition-of-done table differs from work-item 7.2's metric list? → A: The 11-row
  "Whole-programme definition of done" table is the canonical 100%-coverage set (each with a
  met-target marker or written rationale); the three softer 7.2 metrics not in that table
  (per-feature ceremony time, agent context bytes, warm-build time) are recorded in a clearly
  labelled supplementary "estimate" section, not counted toward the 100% definition-of-done total.
- Q: What concrete artifact realizes the recurring full-pipeline run, given the repo has no
  committed schedule file today (only a Claude Code `.claude/scheduled_tasks.lock`)? → A: A tracked
  schedule-definition file committed under a discoverable repo path (a documented routine/cron
  spec), paired with the documented manual full-pipeline fallback command; discoverable in the
  tree and runnable with no dependency on a live external CI service.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Confirm no interim scaffolding remains (Priority: P1)

A maintainer (or auditing agent) wants assurance that the migration left nothing half-finished —
no dead script, no compatibility flag, no second code path that could silently diverge from the
compiled library.

**Independent test:** Run the recorded grep-proof commands; each returns zero matches in the
tracked tree (excluding gitignored `artifacts/`/`.fake/` build output and the by-design generated
`template/base/build.fsx`). A reviewer can reproduce every proof from the artifact without trusting
prose.

**Acceptance scenarios:**

1. **Given** the tracked tree at the feature SHA, **When** the scaffolding-proof commands run,
   **Then** there is no root `build.fsx`, no `select-tier.fsx`, no `run-audit.sh`, no
   `--legacy-evidence` flag, no `.specify/**/*.py`, and no `fake-cli`/`dotnet fake`/
   `FSharp.Compiler.*` reference — each proven by a committed grep artifact.
2. **Given** any residual scaffolding the sweep surfaces, **When** the feature completes, **Then**
   it has been removed (or, if intentionally retained, its retention is documented with a reason).

### User Story 2 - See the programme's promises measured before vs after (Priority: P1)

A maintainer wants a single page that answers "did we get what the programme promised?" — every
whole-programme definition-of-done dimension, baseline value vs current value, with reproducible
commands and an honest note on any target not reached.

**Independent test:** Open `docs/reports/_baselines/2026-06-02-foundations-after.md`; every
definition-of-done row from the plan has a before value (matching the Stage-0 baseline), an after
value, and either a met-target marker or a written rationale. Each after value is reproducible from
its recorded command.

**Acceptance scenarios:**

1. **Given** the Stage-0 baseline, **When** the after-baseline is produced, **Then** it reports the
   after value for each dimension: `build.fsx` lines (4,688 → 0), evidence-path languages
   ({F#,Bash,Python} → {F#}), `compute-task-graph.py`/`audit-status-scan.py`/`run-audit.sh` LOC
   (removed), governance rule Markdown lines, `.claude`/`.agents` duplication (single-sourced),
   framework-author process tier, tier-selection mechanism, framework-owned config representation,
   and generated-product contract versioning.
2. **Given** a dimension whose after value does not reach the plan's target, **When** it is
   recorded, **Then** the row carries a written rationale (e.g. the prose-line baseline correction
   already disclosed in feature 046), not a padded or omitted number.

### User Story 3 - A new contributor can work without reading 23,000 lines of prose (Priority: P1)

A new contributor (or fresh agent session) needs to make a routine framework change and know
exactly what to run, without absorbing the whole governance corpus.

**Independent test:** Following only the updated `README.md` / `docs/reports/build.md` /
`docs/reports/speckit.md` / `CLAUDE.md` / `AGENTS.md`, a contributor runs `./fake.sh build -t
Route` on a sample change, gets the minimal gate list, runs only those gates, and proceeds — having
read documentation that describes the two-tier process, the `Route` entry point, the governance
library as the home of all rules, and the generate-don't-sync principle.

**Acceptance scenarios:**

1. **Given** the updated docs, **When** a contributor reads them, **Then** they describe the
   two-tier process, the `Route` entry point, the `FS.Skia.UI.Build` governance library as the
   single home of all rules, and the generate-don't-sync principle, with no instruction to "read
   the prose and comply" for any rule now enforced by code.
2. **Given** the programme is complete, **When** the closing ADR is written, **Then** it records
   the programme's outcome, the decisions realized (D1–D6), and the new steady-state development
   model, cross-linked from the after-baseline.

### User Story 4 - The consumer-governance harness cannot silently rot (Priority: P2)

A maintainer wants confidence that the full Spec Kit + evidence pipeline keeps running on the
dogfood features so the consumer-facing governance does not decay now that framework-author work
uses the light tier by default.

**Independent test:** Open the committed retrospective; it confirms the named dogfood features
(Stage 1 / feature 042 and Stage 4 / feature 043) exercised the full pipeline, and a discoverable
recurring-run mechanism (schedule definition + documented manual fallback command) is in place to
re-run the full pipeline on the dogfood set.

**Acceptance scenarios:**

1. **Given** the programme's dogfood features, **When** the retrospective is written, **Then** it
   confirms each kept the harness honest (full serialized pipeline green) and identifies the
   standing recurring-run mechanism.
2. **Given** the recurring-run mechanism, **When** a maintainer inspects it, **Then** it is
   discoverable in the repository and runnable, with a documented manual fallback that does not
   depend on a live external CI service existing.

### Edge Cases

- A grep proof finds an unexpected residual (e.g. a stale doc reference to `run-audit.sh` or
  `build.fsx`): the residual is removed or the reference corrected, and the proof re-run clean.
- A documentation surface still instructs readers to run the "serialized six-target order"
  unconditionally: it is corrected to the `Route`-first model (the serialized order is the
  escalated `maintainer-verify` path only).
- An after-baseline measurement command is not reproducible on the toolchain: the command is fixed
  or the metric is labelled an estimate (the same exemption Stage 0 applied to the ceremony-hours
  figure), never left unreproducible-but-asserted.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The feature MUST produce committed grep-proof artifacts demonstrating the tracked
  tree contains no interim scaffolding: no root `build.fsx`, no `scripts/build/select-tier.fsx`, no
  `run-audit.sh`, no `--legacy-evidence` flag, no `.specify/**/*.py`, and no
  `fake-cli`/`dotnet fake`/`FSharp.Compiler.*` reference (excluding gitignored build output and the
  by-design generated `template/base/build.fsx`).
- **FR-002**: Any residual interim scaffolding the verification sweep surfaces MUST be removed, or,
  if intentionally retained, documented with the reason for retention.
- **FR-003**: The feature MUST produce a final measurement report at
  `docs/reports/_baselines/2026-06-02-foundations-after.md` that pairs each of the plan's 11
  "Whole-programme definition of done" dimensions (the canonical coverage set) with its Stage-0
  baseline value and its current value. The three softer work-item-7.2 metrics not in that table
  (per-feature ceremony time, agent context bytes, warm-build time) MUST be recorded in a clearly
  labelled supplementary "estimate" section and are not counted toward the definition-of-done total.
- **FR-004**: Each after-baseline metric MUST carry the reproduction command that produced it and a
  recorded feature SHA, so deltas are independently reproducible (the Stage-0 measurement-command
  discipline), except metrics explicitly labelled estimates.
- **FR-005**: Any definition-of-done dimension whose after value does not reach the plan's target
  MUST be recorded with a written rationale, not padded or omitted.
- **FR-006**: The contributor-facing documentation surfaces (`README.md`, `docs/reports/build.md`,
  `docs/reports/speckit.md`, `CLAUDE.md`, `AGENTS.md`) MUST describe the new development model: the
  two-tier process, the `Route` entry point, the governance library as the single home of all
  rules, and the generate-don't-sync principle.
- **FR-007**: No documentation surface may instruct readers to run the full serialized six-target
  order as the unconditional default; it MUST be presented as the escalated `maintainer-verify`
  path, with `Route` as the entry point for selecting the minimal gate set.
- **FR-008**: A closing ADR MUST record the programme's outcome, the realized decisions (D1–D6),
  and the new steady-state development model, cross-linked from the after-baseline.
- **FR-009**: A dogfooding retrospective MUST confirm the named dogfood features (042, 043)
  exercised the full pipeline and MUST identify a discoverable, runnable recurring-run mechanism for
  re-running the full pipeline on the dogfood set: a tracked schedule-definition file committed under
  a discoverable repo path (a documented routine/cron spec) plus a documented manual full-pipeline
  fallback command. The mechanism MUST NOT depend on a live external CI service existing.
- **FR-010**: The feature MUST NOT change the product runtime, any product `.fsi` surface, any
  surface baseline, or any `PackageVersion` (it is documentation, measurement, and cleanup only).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, contents, or version changes. No controls/chart/graph/
  DataGrid authoring change; no Charts migration guidance involved.
- **Public contract impact**: No product `.fsi` signature, documented public API, sample contract,
  or surface-baseline change. Only governance/contributor documentation and measurement artifacts
  change.
- **State workflow impact**: No stateful workflow, I/O, command, effect, subscription, or
  interpreter behavior change. (The measurement report may *read* build outputs but adds no new
  build behavior.)
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshot, Vulkan,
  Skia, visual-output, or unsupported-environment-diagnostic change.
- **Evidence obligations**: Real evidence paths — committed grep-proof artifacts (FR-001), the
  after-baseline report with reproduction commands (FR-003/FR-004), the closing ADR (FR-008), the
  dogfooding retrospective + recurring-run mechanism (FR-009), and the standard
  `EvidenceGraph`/`EvidenceAudit` PASS for this feature with zero synthetic evidence.
- **Unsupported scope**: No history rewrite, no committed-evidence tree cleanup (D3: future
  regenerable logs/zips are already gitignored; existing committed evidence stays as-is). No live
  external CI service is stood up. No V3 modular-package work. No runtime or visual change.
- **Build-target impact**: No target behavior changes. The feature runs the existing gates via
  `Route` (escalated set, since it touches governance docs); it does not add, rename, or alter any
  FAKE target, and the typed `Targets` registry is unchanged.

## Success Criteria *(mandatory)*

- **SC-001**: Grep-proof artifacts show **zero** tracked-tree matches for every interim-scaffolding
  pattern in FR-001 (excluding gitignored build output and the by-design generated
  `template/base/build.fsx`), each reproducible from the committed command.
- **SC-002**: The after-baseline report covers **100%** of the plan's 11 "Whole-programme
  definition of done" dimensions, each with a before value, an after value, and either a met-target
  marker or a written rationale; the three softer 7.2 metrics (ceremony time, context bytes,
  warm-build time) appear in a clearly labelled supplementary estimate section.
- **SC-003**: Every non-estimate after-baseline metric is reproducible: re-running its recorded
  command at the pinned SHA yields the reported value.
- **SC-004**: All five named documentation surfaces (`README.md`, `build.md`, `speckit.md`,
  `CLAUDE.md`, `AGENTS.md`) describe the two-tier `Route` process, the governance library, and the
  generate-don't-sync principle, and none presents the serialized six-target order as the
  unconditional default.
- **SC-005**: The closing ADR and the dogfooding retrospective (with the recurring-run mechanism)
  are committed and cross-linked from the after-baseline.
- **SC-006**: Runtime untouched — `git diff` over product `src/**` shows zero changes, and
  `PackageSurfaceCheck`/`FsiTranscripts` show no product surface-baseline diff.
- **SC-007**: This feature's `EvidenceGraph` returns `verdict=ok` and `EvidenceAudit` returns
  `verdict=PASS` with zero synthetic evidence, and the escalated gate set selected by `Route` is
  green (modulo the documented pre-existing `FsiTranscripts` / `SkiaViewer.Tests` headless-flake
  Class-C exclusions disclosed by prior foundations features).

## Assumptions

- The interim scaffolding named by the plan's 7.1 is already removed by features 043/045 (verified
  at authoring time); Stage 7 verifies-and-records this and removes only any residual the sweep
  surfaces. If the sweep is clean, 7.1 produces proof artifacts and no deletions.
- The prose-reduction "after" figure uses the corrected ≈ 6,882-line rule/guidance baseline
  established in feature 046, not the plan's overstated ~23,000, and the after-baseline states this
  correction explicitly.
- The recurring full-pipeline run is satisfied by a committed, discoverable schedule definition plus
  a documented manual fallback command; no live external CI service is required to exist for the
  feature to be complete.
- This feature runs as framework-tooling/governance documentation; because it touches `CLAUDE.md`/
  `AGENTS.md`/governance docs it escalates via `Route`, and (as the programme-closing feature) it
  is a reasonable **dogfood** candidate that may run the full serialized pipeline for itself.

## Dependencies

- Stages 1–6 (features 042, 040+044, 041, 043, 045, 046) complete and merged to `main`.
- The Stage-0 baseline `docs/reports/_baselines/2026-05-31-foundations.md` (the comparison oracle).
- The plan's "Whole-programme definition of done" table (the dimensions to measure).
