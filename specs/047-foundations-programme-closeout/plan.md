# Implementation Plan: Decommission, Measure, Document the New Normal

**Branch**: `047-foundations-programme-closeout` | **Date**: 2026-06-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/047-foundations-programme-closeout/spec.md`

## Summary

**Stage 7 — the closeout** of the foundations programme. This feature adds **no capability**.
It (1) **verifies-and-records** that no interim scaffolding remains via committed grep-proof
artifacts; (2) produces the **final before/after measurement report**
`docs/reports/_baselines/2026-06-02-foundations-after.md`, pairing each of the plan's 11
"Whole-programme definition of done" dimensions with its Stage-0 baseline value and current value
(plus a clearly-labelled supplementary *estimate* section for the three softer 7.2 metrics);
(3) **documents the new normal** across `README.md`, `docs/reports/build.md`,
`docs/reports/speckit.md`, `CLAUDE.md`, `AGENTS.md` and a **closing ADR 0006**; and (4) writes the
**dogfooding retrospective** + commits a **discoverable, runnable recurring-run mechanism** (a
tracked schedule-definition file + a documented manual full-pipeline fallback).

**Technical approach** (grounded in reconnaissance, see [research.md](./research.md)):

1. **Scaffolding proof (US1)** — the authoring-time sweep confirms the *artifacts* are already
   gone (no root `build.fsx`, no `scripts/build/select-tier.fsx`, no `run-audit.sh`, no
   `.specify/**/*.py`). The naive token patterns `--legacy-evidence` / `fake-cli` / `dotnet fake`
   / `FSharp.Compiler.*` **do** match the tracked tree, but every match is **non-scaffolding**:
   frozen feature-history prose in `specs/**`, the governance library's own *enforcement
   scan-strings* (`build/Governance/Guidance.fs` detects `dotnet fake` in docs), dependency/manifest
   comments that *assert the absence* (`Directory.Packages.props`), and legitimate FAKE entry-point
   diagnostics. The central Phase-0 decision is therefore the **proof scope**: file-existence proofs
   (`git ls-files`) for the dead artifacts, and *scoped* proofs (active code paths / dependency
   manifests / live command surface) for the flags/runner — each with a documented allowlist so the
   proof is zero-by-construction without rewriting frozen history. Any genuine residual the sweep
   surfaces is removed; this is otherwise a **verification-and-record** task, not a deletion task.
2. **After-baseline (US2)** — a new `docs/reports/_baselines/2026-06-02-foundations-after.md`
   paired side-by-side with the Stage-0 `2026-05-31-foundations.md`. Each of the 11
   definition-of-done rows carries before-value, after-value, a reproduction command, the pinned
   feature SHA, and either a met-target marker or a written rationale (notably the **corrected
   ≈6,882-line prose baseline** from feature 046, not the plan's overstated ~23,000). The three
   softer 7.2 metrics (per-feature ceremony time, agent context bytes, warm-build time) go in a
   clearly-labelled supplementary *estimate* section, **not** counted toward the 100% total.
3. **New-normal docs + ADR (US3)** — update the five named surfaces to describe the two-tier
   `Route` process, `Route` as the entry point, `FS.Skia.UI.Build` as the single home of all rules,
   and the generate-don't-sync principle; ensure none presents the serialized six-target order as
   the unconditional default (it is the escalated `maintainer-verify` path). Correct residual stale
   references the sweep surfaces (e.g. `branch-vs-master` in `build.md`). Add **ADR 0006** closing
   the programme (outcome, realized D1–D6, steady-state model), cross-linked from the after-baseline.
4. **Dogfood retrospective + recurring run (US4)** — a committed retrospective confirming features
   042 and 043 exercised the full serialized pipeline green, plus a tracked schedule-definition file
   under a discoverable path and a documented manual fallback command, with no dependency on a live
   external CI service.

This change **escalates** via `Route` (it touches `CLAUDE.md`/`AGENTS.md`/governance docs and the
recurring-run schedule definition), and as the programme-closing feature is run as a **dogfood**
candidate on the full serialized six-target set. The product runtime, every product `.fsi` surface,
all surface baselines, and every `PackageVersion` are **untouched** (FR-010, SC-006).

## Technical Context

**Language/Version**: F# / .NET `net10.0` (no product or build-library source change; this feature
is documentation, measurement, and verification-record artifacts).
**Primary Dependencies**: None new. Measurement reproduction commands use existing tooling already
present at the pinned SHA (`git`, `wc`, `grep`, `./fake.sh build -t …`); no new dependency, package,
or `PackageVersion` change.
**Testing**: No new automated tests (no behavior change — Constitution VI applies to
behavior-changing code). Evidence is **real reproducible commands**: committed grep-proof artifacts,
after-baseline metric commands re-run at the pinned SHA, and the standard `EvidenceGraph` /
`EvidenceAudit` PASS for this feature. The escalated gate set selected by `Route` is run serially.
**Target Platform**: Windows and Linux (documentation/measurement only; no runtime/visual change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.* — **PASS** (initial and
post-design). This is a **Tier 2** change (no public API surface, no dependency, no inter-project
contract change; documentation + measurement + verification-record only).

### Repository Governance Decisions

- **Template ownership**: **No `.template.config/template.json` change.** No source, sample, test,
  Spec Kit asset, package-policy, or command-surface change touches the template. The new
  recurring-run schedule-definition file is a *repository governance* artifact (it re-runs the
  dogfood pipeline), not a generated-product input, so it requires no template update. No deferral
  needed.
- **Dependency impact**: **None.** No `Directory.Packages.props` change, no
  `docs/dependencies.md` change, no new dependency, no generated-template inclusion change, and no
  `DependencyReport` coverage change. The `Directory.Packages.props` `FSharp.Compiler.Service`
  mention is a pre-existing *assert-the-absence* comment and is left as-is.
- **Command-surface impact**: **No FAKE target added, renamed, or altered; the typed `Targets`
  registry is unchanged.** The feature *runs* the existing escalated gate set via `Route`; it does
  not change `build.fsx`, wrappers, `Dev`, `Verify`, `Ci`, `TemplateCheck`, `DependencyReport`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, or `EvidenceAudit` behavior. Because
  it touches `CLAUDE.md`/`AGENTS.md`/governance docs it **escalates**; FAKE-backed commands run
  **sequentially** in the deterministic escalated order (never concurrent — shared `.fake` state):
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: **None.** No change to default/minimal generated contents, selected
  Controls guidance, local skills, validation logs, placeholder/excluded-history scans, or generated
  `Dev` behavior. The by-design generated `template/base/build.fsx` thin front-end is unchanged and
  is explicitly excluded from the scaffolding proof.
- **Evidence paths** (all **real**, no synthetic): readiness lives under
  `specs/047-foundations-programme-closeout/readiness/` (regenerable logs/zips are gitignored per
  feature-046's `.gitignore` rule; authored proof Markdown is committed):
  - Scaffolding grep-proofs (FR-001/SC-001): `readiness/scaffolding-proof.md` — one recorded command
    + output block per pattern, with the documented allowlist for non-scaffolding matches.
  - After-baseline report (FR-003/004/005, SC-002/003): committed at
    `docs/reports/_baselines/2026-06-02-foundations-after.md` (a deliverable doc, not transient
    readiness), with per-row reproduction commands + pinned SHA.
  - Reproducibility proof (SC-003): `readiness/after-baseline-repro.md` — each non-estimate command
    re-run at the pinned SHA, output matching the reported value.
  - Doc-surface proof (FR-006/007, SC-004): `readiness/docs-coverage.md` — per-surface evidence that
    the four required concepts are present and the serialized order is not the unconditional default.
  - Closing ADR (FR-008): `docs/adr/0006-foundations-programme-closeout.md`.
  - Dogfood retrospective + recurring-run mechanism (FR-009, SC-005): `readiness/retrospective.md`
    plus the tracked schedule-definition file (path fixed in Phase 0 / contracts) and the documented
    manual fallback command.
  - Runtime-untouched proof (SC-006): `readiness/runtime-untouched.md` — `git diff` over `src/**`
    empty; `PackageSurfaceCheck`/`FsiTranscripts` no surface-baseline diff.
  - Gate logs (SC-007): serialized six-target logs in `readiness/` (`dev.log`,
    `generated-guidance-check.log`, `template-check.log`, `generated-product-check.log`,
    `evidence-graph.log`, `evidence-audit.log`); `EvidenceGraph` `verdict=ok`, `EvidenceAudit`
    `verdict=PASS`, zero synthetic.
- **`.fsi` / contract impact**: **No product `.fsi` signature, documented public API, sample
  contract, or surface-baseline change** (SC-006). No build-tooling `.fsi` change either (no
  `build/**` source change). The only "contract" artifacts are *documentation* of the
  already-shipped generated-product contract versioning (feature 046) inside the after-baseline.
- **MVU/effect boundary**: **N/A** — no stateful or I/O-bearing workflow is introduced. The
  measurement report *reads* build/git outputs to record values but adds no `Model`/`Msg`/`Effect`,
  no command, no subscription, and no interpreter behavior (Constitution IV scope not triggered).
- **Synthetic evidence**: **None planned.** All evidence is real: committed grep-proof command
  output, after-baseline metrics reproduced from recorded commands at the pinned SHA, and a real
  full-pipeline retrospective. No `[S]`/`[S*]`/`[SEH]` tasks are expected; `EvidenceAudit` MUST
  return `verdict=PASS` with zero synthetic (SC-007).
- **Test evidence**: **No failing-first automated tests** — there is no behavior change to assert
  (Constitution VI applies to behavior-changing code; this feature changes documentation and adds
  verification-record artifacts). The "tests" are the **reproduction commands**: every non-estimate
  after-baseline metric is re-run at the pinned SHA and must yield the reported value (SC-003), and
  every scaffolding proof command must return its recorded zero/allowlisted result (SC-001). The
  existing governance gates (the escalated six-target set) must stay green (SC-007).
- **Observability**: The artifacts are themselves the diagnostics. Every proof and metric names the
  **exact command** that produced it (Stage-0 measurement-command discipline); any
  definition-of-done miss carries a **written rationale** at its row (FR-005), never a padded or
  omitted number; the scaffolding proof's allowlist names *why* each non-scaffolding match is
  retained. No silent acceptance.
- **Deferred scope**: Per spec **Unsupported scope** — no history rewrite, no committed-evidence
  tree cleanup (D3: future regenerable logs/zips already gitignored; existing committed evidence
  stays as-is), no live external CI service stood up, no V3 modular-package work, no runtime or
  visual change. The recurring-run obligation is satisfied by a *committed, discoverable schedule
  definition + documented manual fallback*, not by provisioning live CI.

## Project Structure

```
specs/047-foundations-programme-closeout/
├── spec.md
├── plan.md                       # this file
├── research.md                   # Phase 0 — proof-scope decision, metric-command resolution
├── data-model.md                 # Phase 1 — artifact "entities" (proof entry, baseline row, ADR, schedule)
├── quickstart.md                 # Phase 1 — reproduce-every-proof walkthrough
├── contracts/
│   ├── scaffolding-proof.md      # the proof-scope contract + allowlist (FR-001/002)
│   ├── after-baseline.md         # the 11-row schema + estimate-section rule (FR-003/004/005)
│   └── recurring-run.md          # schedule-definition file shape + manual fallback (FR-009)
└── readiness/                    # evidence (regenerable logs/zips gitignored; authored proofs committed)

docs/reports/_baselines/
├── 2026-05-31-foundations.md             # Stage-0 baseline (comparison oracle, unchanged)
└── 2026-06-02-foundations-after.md       # NEW — final before/after report (FR-003)

docs/adr/
└── 0006-foundations-programme-closeout.md  # NEW — closing ADR (FR-008)

README.md                         # new-normal doc pass (FR-006/007)
docs/reports/build.md             # new-normal doc pass + stale-ref correction (FR-006/007)
docs/reports/speckit.md           # new-normal doc pass (FR-006/007)
CLAUDE.md                         # new-normal doc pass (FR-006/007)
AGENTS.md                         # new-normal doc pass + plan-reference repoint (FR-006/007)
.specify/schedules/foundations-dogfood-pipeline.yml   # NEW — tracked recurring-run schedule (FR-009; shape in contracts/recurring-run.md)
```

## Phase 0 — Research

See [research.md](./research.md). Resolves the closeout's open questions:

- **R1 — Scaffolding proof scope.** How to make FR-001's proof *zero-by-construction* given the
  naive tokens match frozen history, enforcement scan-strings, and assert-the-absence comments.
  Decision: file-existence proofs for dead artifacts + scoped proofs for flags/runner, each with a
  documented allowlist (history prose / enforcement strings / absence-comments / generated front-end).
- **R2 — Canonical coverage set.** The 11-row "Whole-programme definition of done" table is the
  100% set; the three softer 7.2 metrics are supplementary estimates (spec Clarifications).
- **R3 — Per-metric reproduction commands.** For each of the 11 rows, the exact command that
  produces its after-value (mirroring the Stage-0 baseline commands), and which rows carry a
  rationale (the corrected prose baseline, the framework-author ceremony estimate).
- **R4 — Recurring-run realization.** Which discoverable repo path + file format the schedule
  definition takes, and the manual fallback command, with no live-CI dependency (spec Clarifications).
- **R5 — ADR numbering/format.** Next sequential ADR is `0006`, following the `docs/adr/000N-*.md`
  format (Status/Date/Decision-source/Context/Decision/Consequences) established by 0001–0005.

No `NEEDS CLARIFICATION` remain — the five spec Clarifications (session 2026-06-02) resolved the
after-baseline location, recurring-run realization, miss-definition, canonical coverage set, and the
concrete schedule artifact.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — the artifact "entities": `ScaffoldingProofEntry`
  (pattern, command, result, allowlist note), `DefinitionOfDoneRow` (dimension, baseline, after,
  command, SHA, met-target | rationale), `EstimateMetric`, `ClosingAdr`, `RetrospectiveEntry`,
  `RecurringRunMechanism` (schedule file + manual fallback). These are document shapes, not F# types.
- [contracts/scaffolding-proof.md](./contracts/scaffolding-proof.md) — the proof-scope contract:
  the patterns, the proof command per pattern, the zero/allowlist acceptance rule, and the exact
  allowlist of non-scaffolding match classes.
- [contracts/after-baseline.md](./contracts/after-baseline.md) — the after-baseline schema: the 11
  canonical rows, the required per-row fields, the rationale rule for a missed target, and the
  separate supplementary estimate section.
- [contracts/recurring-run.md](./contracts/recurring-run.md) — the recurring-run mechanism: the
  tracked schedule-definition file path + shape, the documented manual full-pipeline fallback
  command, and the no-live-CI constraint.
- [quickstart.md](./quickstart.md) — the "reproduce every proof from the artifact" walkthrough a
  reviewer follows without trusting prose.
- **Agent context update**: `AGENTS.md` plan reference repointed from feature 046 to this plan
  (between the `<!-- SPECKIT START -->`/`<!-- SPECKIT END -->` markers).

**Constitution re-check (post-design): PASS.** No design element introduces a public-surface,
dependency, MVU, or synthetic-evidence obligation; all evidence paths are real and reproducible.

## Phase 2 — (planned by `/speckit-tasks`, not generated here)

Tasks will be story-grouped (US1 scaffolding proof, US2 after-baseline, US3 new-normal docs + ADR,
US4 retrospective + recurring run) with `tasks.deps.yml` + `skillist` metadata. Expected ordering:
the scaffolding proof and metric reproduction precede the after-baseline (which cites them); the
after-baseline and ADR cross-link (ADR drafted before the baseline's cross-link row is finalized);
the doc pass and retrospective are independent. No task is expected to need `[S]`.
