# Tasks: Decommission, Measure, Document the New Normal (Stage 7 Closeout)

**Feature branch**: `047-foundations-programme-closeout`
**Spec**: `specs/047-foundations-programme-closeout/spec.md`
**Plan**: `specs/047-foundations-programme-closeout/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real and
reproducible: committed grep-proof command output, after-baseline metrics
re-run from their recorded commands at the pinned feature SHA, a real
full-pipeline dogfood retrospective, a tracked runnable recurring-run
mechanism, and the serialized escalated FAKE gate logs. No `[S]`/`[SEH]` task
is approved (Principle V); `EvidenceAudit` MUST return `verdict=PASS` with zero
synthetic (SC-007).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]** confirm no interim scaffolding remains, **[US2]** before/after
  measurement report, **[US3]** new-normal docs + closing ADR, **[US4]**
  dogfood retrospective + recurring-run mechanism
- **[T1]** / **[T2]** — this feature is **Tier 2** throughout (no product `.fsi`
  / surface-baseline / `PackageVersion` change — SC-006; documentation,
  measurement, and verification-record only). No story is Tier 1. Because it
  touches `CLAUDE.md`/`AGENTS.md`/governance docs and a recurring-run schedule
  file, `Route` **escalates** it; as the programme-closing feature it is run as a
  **dogfood** candidate through the full serialized six-target set.
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read first)

This is a **documentation / measurement / verification-record** feature: it
**authors no F# source** (no `build/**`, no `src/**`), introduces no scene,
window, Elmish runtime, input, layout, or widget surface, and adds no behaviour.
The deliverables are Markdown artifacts (grep proofs, the after-baseline report,
the closing ADR, the retrospective, doc edits) and a tracked schedule-definition
file; the measurement "tests" are existing `git`/`wc`/`grep`/`./fake.sh`
commands run by hand and recorded.

Consequently **no `fs-skia-*` runtime/rendering/viewer/layout/widgets skill
applies**, and **no `fsharp-*` cookbook applies** either — there is no F# to
write (`fsharp-parsing`/`-code-generation`/`-graph-algorithms`/`-io-globbing`),
no new `git`/process wrapper authored from F# (`fsharp-shell-process` — the
proofs run `git` directly), and no FAKE-target authoring or golden-diff work
(`fsharp-build-orchestration` — the serialized gate task only *runs* the
existing, unchanged targets, so its `skillist` is a deliberate `valid-empty`).
`fs-skia-template-update` is not assigned — there is **no** `dotnet new
fs-skia-ui` product / package-pin / `template.json` change (the by-design
generated `template/base/build.fsx` is explicitly excluded from the scaffolding
proof and untouched). `speckit-constitution` is not assigned — no
`.specify/memory/constitution.md` edit; the closing ADR records *realized
decisions D1–D6* as prose, and its task title says "closing ADR" (never the
`constitution` trigger word).

The only genuine capability skills are the two real workflow tasks: **T019**
declares `speckit-evidence-graph` and **T020** declares `speckit-evidence-audit`.
Every other task takes a justified `valid-empty` `skillist` (Markdown / `git` /
gate-run evidence capture carries no cookbook).

## Governance risk levels & validation

- **Small** (routine Markdown edits inside this feature's own `readiness/` and
  doc surfaces): focused review plus a `git diff` over the edited files is the
  **required evidence** and is authoritative for the level.
- **Medium** (the after-baseline measurement rows): each non-estimate row's
  recorded reproduction command is re-run at the pinned SHA and must yield the
  reported After value (SC-003); that re-run is the required evidence.
- **Broad** (required here, because this is a governance-doc + `CLAUDE.md` /
  `AGENTS.md` + recurring-run-schedule change that `Route` escalates): the full
  serialized FAKE gate order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck`
  → `GeneratedProductCheck` → the final graph and audit gates). **Broad
  validation is required** whenever a governance/contributor doc that the
  `Route`-first model depends on is changed. Aggregate FAKE results are recorded
  as **non-authoritative**; any race-like or environment-flaky failure (the known
  `SkiaViewer.Tests` headless crash) is rerun in focused isolation and that
  focused result is authoritative.

## Pre-graph-gate pitfall guidance

Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`)
before declaring this phase complete. Task **titles** deliberately avoid the
validator's blocking trigger tokens: the closing-ADR task says "closing ADR"
(never `constitution`/`constitutional`); no non-graph/non-audit title uses
`evidence graph` / `task graph` / `evidence audit` / `diff-scan` /
`synthetic propagation` / `validator diagnostics`; the genuine graph/audit
workflow tasks (T019/T020) **do** declare `speckit-evidence-graph` /
`speckit-evidence-audit` and name `EvidenceGraph` / `EvidenceAudit` directly; the
readiness-scaffold task (T002) uses the safe `Create placeholder evidence files
listed by the plan` wording and the readiness-aggregation task (T003) uses the
`Complete readiness notes` prefix, so their hyphenated filename citations
(`evidence-graph.log`, `evidence-audit.log`) do not fire the capability checks.
There is **no** viewer, persistent-launch, or window-visibility work, so no such
trigger phrase appears. The after-baseline (T008/T010) and the closing ADR
(T012) cross-reference each other by **fixed path** only — no backward task edge
is written, so the DAG stays acyclic. `tasks.deps.yml` keeps one indented object
per task id with `deps` and `skillist`; every `[skillist: …]` mirror matches the
structured list exactly and in order.

---

## Phase 1: Setup

- [X] T001 [T2] [skillist: []] Record feature Tier 2 (documentation / measurement / verification-record, escalated by `Route` to the full serialized set because it touches `CLAUDE.md` / `AGENTS.md` / governance docs and the recurring-run schedule file), the affected surfaces (`docs/reports/_baselines/2026-06-02-foundations-after.md`, `docs/adr/0006-foundations-programme-closeout.md`, `README.md`, `docs/reports/build.md`, `docs/reports/speckit.md`, `CLAUDE.md`, `AGENTS.md`, the tracked recurring-run schedule file, and `specs/047-foundations-programme-closeout/readiness/**`), the public-API impact (none — no product `.fsi`, surface-baseline, or `PackageVersion` change, SC-006), the Elmish/MVU applicability (N/A — no stateful or I/O-bearing workflow; the measurement artifacts only *read* `git`/build outputs and add no `Model`/`Msg`/`Effect`), and the real-evidence obligations (committed grep proofs, the after-baseline with per-row reproduction commands, the closing ADR, the dogfood retrospective + recurring-run mechanism, the runtime-untouched proof, and the serialized escalated FAKE gate logs; zero synthetic)
- [X] T002 [P] [T2] [skillist: []] Create placeholder evidence files listed by the plan under `specs/047-foundations-programme-closeout/readiness/` so the audit-enforced readiness files are discoverable at setup: `scaffolding-proof.md`, `after-baseline-repro.md`, `docs-coverage.md`, `retrospective.md`, `runtime-untouched.md`, the three always-required contract files `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, and `logs/` (`dev.log`, `generated-guidance-check.log`, `template-check.log`, `generated-product-check.log`, `evidence-graph.log`, `evidence-audit.log`)
- [X] T003 [T2] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files — `governance-risk-levels.md` (the small / medium / broad levels, their required evidence, and when broad validation is required), `aggregate-hang-diagnostics.md` (verdict / stage / elapsed duration / last observed command / focused rerun / non-authoritative aggregate), and `runtime-limitations.md` (the .NET 10 desktop / Vulkan / SkiaSharp preview / unsupported macOS/mobile/browser / no software-renderer fallback statements) — each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation (document shapes fixed first)

- [X] T004 [T2] [skillist: []] Scaffold the after-baseline report `docs/reports/_baselines/2026-06-02-foundations-after.md` per `contracts/after-baseline.md` — the pinned-context header block (`git_commit` full+short, `branch`, `captured_at`, toolchain), the empty Section A 11-row definition-of-done table (`Dimension | Baseline (2026-05-31) | After (this SHA) | Reproduction command | Met-target / rationale`), the empty Section B supplementary-estimate table (clearly labelled, excluded from the 100% total), and the fixed-path cross-link placeholders to the Stage-0 baseline `2026-05-31-foundations.md` and the closing ADR `docs/adr/0006-foundations-programme-closeout.md` (values filled in US2)

**Checkpoint**: Foundation ready — readiness discoverable and the after-baseline document shape fixed; story work may begin.

---

## Phase 3: User Story 1 (US1) — confirm no interim scaffolding remains (P1)

**Goal**: committed grep-proof artifacts show the tracked tree contains no
interim scaffolding, each reproducible from its recorded command; any residual
the sweep surfaces is removed or corrected (FR-001/002, SC-001).

- [X] T005 [P] [US1] [skillist: []] Run the file-existence proofs for the dead artifacts — `git ls-files build.fsx`, `git ls-files '**/select-tier.fsx'`, `git ls-files '**/run-audit.sh'`, `git ls-files '.specify/**/*.py'` — and record each exact command with its empty output in `readiness/scaffolding-proof.md` (excluding gitignored build output and the by-design generated `template/base/build.fsx`)
- [X] T006 [P] [US1] [skillist: []] Run the scoped token proofs for the flag/runner patterns (`--legacy-evidence`; `fake-cli` / `dotnet fake` / `FSharp.Compiler.*`) per `contracts/scaffolding-proof.md` — record the full unscoped matches and the allowlist-scoped zero result, naming each retained match's non-scaffolding class (frozen `specs/**` + impl-plan history, the `build/Governance/Guidance.fs` enforcement scan-strings, the `Directory.Packages.props` assert-the-absence comments, and the live-FAKE entry-point diagnostics in `build/Program.fs` / `build/Governance/Preflight.fs`) in `readiness/scaffolding-proof.md`
- [X] T007 [US1] [skillist: []] Remove or correct any match outside the named allowlist as a genuine residual (e.g. a live dead-script reference or unguarded flag the scoped sweep surfaces), re-run the affected proof until the scoped result is zero, and record each correction with `verdict = residual-removed`; where the sweep is already clean, record `verdict = clean` for that entry. The `branch-vs-master` stale doc reference in `docs/reports/build.md` is corrected by the US3 doc pass (T011), not here. (FR-002)

**Checkpoint**: User Story 1 complete — scaffolding proven gone, every entry `clean` or `residual-removed`.

---

## Phase 4: User Story 2 (US2) — the programme's promises measured before vs after (P1)

**Goal**: the after-baseline report pairs each of the 11 definition-of-done
dimensions with its Stage-0 value, current value, reproduction command, pinned
SHA, and a met-target marker or written rationale; the three softer metrics sit
in a clearly-labelled estimate section; every non-estimate row reproduces
(FR-003/004/005, SC-002/003).

- [X] T008 [US2] [skillist: []] Fill Section A's 11 definition-of-done rows in `docs/reports/_baselines/2026-06-02-foundations-after.md` — each with its Stage-0 baseline value (from `2026-05-31-foundations.md`), its current value, the exact reproduction command, the pinned feature SHA, and a met-target marker or a written rationale; include the corrected ≈6,882-line governance-Markdown baseline rationale (feature 046, **not** the overstated ~23,000) and the framework-author-process estimate rationale (no timing harness; the mechanism is the `inner-loop` light tier now being the `Route` default), reusing the US1 proof commands for the `build.fsx → 0` / removed-runner rows
- [X] T009 [US2] [skillist: []] Fill Section B's three supplementary estimate metrics (per-feature ceremony time, agent context bytes, warm-build time) in the clearly-labelled estimate section — each with its baseline value, after value, and an `estimate` basis stating why it is not command-reproducible — explicitly excluded from the 100% definition-of-done total (spec Clarification, SC-002)
- [X] T010 [US2] [skillist: []] Reproduce every non-estimate Section A metric by re-running its recorded command at the pinned SHA and confirm the output matches the reported After value, capturing the re-runs (command + output) in `readiness/after-baseline-repro.md` (SC-003)

**Checkpoint**: User Story 2 complete — 11 rows present and reproducible, estimates clearly separated.

---

## Phase 5: User Story 3 (US3) — a new contributor can work without reading the whole corpus (P1)

**Goal**: the five contributor-facing surfaces describe the new development
model and none presents the serialized six-target order as the unconditional
default; a closing ADR records the programme outcome and the steady-state model
(FR-006/007/008, SC-004).

- [X] T011 [P] [US3] [skillist: []] Update the five contributor-facing surfaces (`README.md`, `docs/reports/build.md`, `docs/reports/speckit.md`, `CLAUDE.md`, `AGENTS.md`) to describe the new development model — the two-tier process, the `Route` entry point, the `FS.Skia.UI.Build` governance library as the single home of all rules, and the generate-don't-sync principle — ensuring none presents the serialized six-target order as the unconditional default (it is the escalated `maintainer-verify` path), and repoint `AGENTS.md`'s `<!-- SPECKIT START -->`…`<!-- SPECKIT END -->` plan reference to this feature's plan (FR-006/007)
- [X] T012 [P] [US3] [skillist: []] Write the closing ADR `docs/adr/0006-foundations-programme-closeout.md` in the 0001–0005 format (`Status`, `Date`, `Decision source`, `## Context`, `## Decision`, `## Alternatives considered`, `## Consequences / rationale`) recording the programme outcome, the realized decisions D1–D6, and the new steady-state development model, linking the Stage-0 baseline and the after-baseline (FR-008, SC-005)
- [X] T013 [US3] [skillist: []] Record per-surface doc-coverage evidence in `readiness/docs-coverage.md` — for each of the five surfaces, the presence of the four required concepts (two-tier process, `Route` entry point, governance library as the single home of all rules, generate-don't-sync) and the absence of any instruction presenting the serialized six-target order as the unconditional default (FR-006/007, SC-004)

**Checkpoint**: User Story 3 complete — docs describe the new normal, closing ADR committed and cross-linked.

---

## Phase 6: User Story 4 (US4) — the consumer-governance harness cannot silently rot (P2)

**Goal**: a committed retrospective confirms the dogfood features ran the full
pipeline green and identifies a discoverable, runnable recurring-run mechanism
with no live-CI dependency (FR-009, SC-005).

- [X] T014 [P] [US4] [skillist: []] Write the dogfood retrospective `readiness/retrospective.md` confirming features 042 and 043 each exercised the full serialized pipeline green (with pointers to their readiness), concluding the harness was kept honest, and identifying the recurring-run mechanism; add the cross-link back from the after-baseline so the closeout artifacts form a connected record (SC-005)
- [X] T015 [P] [US4] [skillist: []] Commit the tracked, discoverable schedule-definition file (path + format fixed per `contracts/recurring-run.md`) naming the dogfood set (042, 043), the full serialized six-target pipeline as the body to re-run, and a cadence, and document the manual full-pipeline fallback command sequence (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates, run sequentially), with no dependency on a live external CI service (FR-009)
- [X] T016 [US4] [skillist: []] Verify the recurring-run mechanism is discoverable and runnable — the schedule file is tracked (`git ls-files`), the manual fallback is documented and runnable by hand, and neither requires a live external CI service to exist — recording the confirmation in `readiness/retrospective.md` (SC-005)

**Checkpoint**: User Story 4 complete — retrospective committed, recurring-run mechanism tracked and runnable.

---

## Phase 7: Integration & Polish (runtime-untouched proof, serialized escalated gates)

- [X] T017 [P] [T2] [skillist: []] Capture the runtime-untouched standing-invariants proof in `readiness/runtime-untouched.md` — `git diff --stat -- 'src/**'` is empty (product runtime / `.fsi` untouched), `PackageSurfaceCheck` / `FsiTranscripts` show no product surface-baseline diff, and no new `PackageVersion` lives outside `Directory.Packages.props` (FR-010, SC-006)
- [X] T018 [T2] [skillist: []] Run the escalated serialized six-target FAKE gate set sequentially (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates T019/T020), never concurrently; record aggregate FAKE results as **non-authoritative** and rerun any race-like or environment-flaky failure (the known `SkiaViewer.Tests` headless crash) in focused isolation as the authoritative result; logs under `readiness/logs/`
- [X] T019 [skillist: speckit-evidence-graph] Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`) — confirm the DAG is acyclic, no dangling refs, no `[S*]` surprises, and the structured `skillist` metadata and visible mirrors are valid (`verdict=ok`)
- [X] T020 [skillist: speckit-evidence-audit] Run the merge-gate audit (`./fake.sh build -t EvidenceAudit`) — confirm `verdict=PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 blocking diff-scan, 0 blocking readiness-contract) with zero synthetic evidence to accept (SC-007)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; every proof is a real recorded command, every metric reproduces at the pinned SHA, and the retrospective records a real full-pipeline run)_ | | | | | | | | |
