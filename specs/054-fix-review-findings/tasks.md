# Tasks: Fix Implementation-Completeness Review Findings

**Feature branch**: `054-fix-review-findings`
**Spec**: `specs/054-fix-review-findings/spec.md`
**Plan**: `specs/054-fix-review-findings/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

This feature ships **zero** synthetic evidence (Constitution-check Principle V:
all proofs are real builds, real grep/diff, real `git status`). No `[S]` or
`[SEH]` rows are anticipated; the Synthetic-Evidence Inventory below stays empty.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Tier annotation omitted: every task matches the spec's overall **Tier 2**
  (internal change — no public-API/`.fsi`/surface-baseline change).

Every task has a matching entry in `tasks.deps.yml` with both `deps` and
`skillist` fields. Each line mirrors the structured `skillist` via
`[skillist: ...]` (`[skillist: []]` when no capability skill applies).

## Pitfall guidance (read before running `EvidenceGraph`)

- `tasks.deps.yml` uses **one object-shaped key per task id** with indented
  `deps` and `skillist` fields — never inline maps like
  `T001: { deps: [], skillist: [] }`.
- Every `Tnnn` in this file appears exactly once as a key in `tasks.deps.yml`;
  dependency lists use exact `Tnnn` ids; the visible `[skillist: ...]` mirror
  matches the structured list exactly and in order.
- Phase-checkpoint edges are auto-injected (every Phase N+1 task implicitly
  depends on the last task of Phase N) — only non-phase cross-edges are written
  in the yml.
- Setup/readiness tasks that merely cite required filenames use the
  `Complete readiness notes` prefix so they do not trip capability-trigger
  groups; the graph/audit tasks (T020/T021) legitimately own the
  `EvidenceGraph`/`EvidenceAudit` work and carry the matching skill ids.

## Governance risk level

**Medium.** This change touches `template/**`, governance `build/Governance/**`,
a governance test, and `.gitignore`, so `Route` **escalates** to the
maintainer-verify path. Focused validation = the strengthened pin-parity
assertion (`TemplateCheck`/`GeneratedProductCheck`) + a clean-build FS3261 count
+ a `git status --porcelain` check. Broad validation (the full serialized
six-target order) is required at integration (Phase 6). Aggregate FAKE results
are recorded as **non-authoritative**; any race-like failure is rerun in focused
isolation as the authoritative result (FAKE shares `.fake` state — never run
concurrently).

---

## Phase 1: Setup

- [X] T001 [skillist: []] Record feature scope and evidence obligations in the plan — Tier 2 internal; affected layers are the governance library (`build/Governance/**`), the generated template (`template/base/**`), `tests/Governance.Tests`, and `.gitignore`; no public-API/`.fsi`/surface-baseline impact; Principle IV (Elmish/MVU) is **not applicable** (no new stateful or I/O-bearing workflow); required real evidence = pin-parity grep/diff, before/after clean-build FS3261 logs, a simulated-bump proof, a deliberate-mismatch gate proof, and a `git status --porcelain` empty proof
- [X] T002 [P] [skillist: []] Complete readiness notes for the audit-required readiness files — create `specs/054-fix-review-findings/readiness/` and author `governance-risk-levels.md` (the small / medium / broad risk levels, the focused validation required for the selected level, when broad validation is required, and how non-authoritative aggregate FAKE results are recorded), `aggregate-hang-diagnostics.md` (verdict / stage / elapsed duration / last observed command / focused rerun / non-authoritative aggregate), and `runtime-limitations.md` (.NET 10 desktop / Vulkan / SkiaSharp preview / unsupported macOS/mobile/browser / no software-renderer fallback) so the unconditional readiness-contract scan passes
- [X] T003 [P] [skillist: []] Complete readiness notes for this feature's authored-evidence placeholders — create placeholder `readiness/pin-parity-proof.md`, `readiness/fs3261-before-after.md`, `readiness/simulated-bump-proof.md`, `readiness/deliberate-mismatch-gate.md`, and `readiness/clean-tree-proof.md`, each naming its authoritative command, artifact path, failure class, and next action (regenerable logs land under `readiness/logs/**`, already gitignored)

---

## Phase 2: Foundation

- [X] T004 [skillist: []] Capture the pre-change baselines as failing-first evidence — record the live pin drift (`template/base/build.fsx` `0.1.45-preview.1` ≠ `template/base/Directory.Packages.props` `0.1.56-preview.1`), the clean `--no-incremental` build FS3261 count (**34** across 8 files), and `git status --porcelain` showing the stray `specs/053-v3-monolith-retirement/readiness/package/` scratch, into the readiness baseline files (the before-state for SC-001 / SC-004 / SC-006)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — generated app restores the correct evidence engine (P1)

### Tests First (Principle I, Principle VI)

- [X] T005 [P] [US1] [skillist: fsharp-parsing] Strengthen `tests/Governance.Tests/GeneratedProjectValidationTests.fs` to extract the `#r "nuget: FS.Skia.UI.Build, <ver>"` literal from `template/base/build.fsx` and the `FS.Skia.UI.Build` `PackageVersion` from `template/base/Directory.Packages.props`, then assert **exact string equality** (replacing the prefix-only `Expect.stringContains "#r \"nuget: FS.Skia.UI.Build"`), with a failure message naming both versions; confirm it **fails-first** against the current drift (FR-003, contract C1)

### Implementation

- [X] T006 [US1] [skillist: []] Align the `template/base/build.fsx` `#r` literal to `0.1.56-preview.1` so it equals the props `PackageVersion` (FR-001 / FR-004); confirm T005's parity assertion now **passes** (SC-001)
- [X] T007 [US1] [skillist: fs-skia-template-update] Extend the canonical `fs-skia-template-update` skill (`.agents/skills/fs-skia-template-update/SKILL.md`, step 3) so the pin-bump flow rewrites **both** the props `Version="<new>"` and the `build.fsx` `#r "nuget: FS.Skia.UI.Build, <new>"` literal in one flow (FR-002, contract C2), then regenerate the `.claude` peer via `./fake.sh build -t RefreshSurfaceBaselines`
- [X] T008 [US1] [skillist: fs-skia-template-update] Demonstrate the gate live (SC-002 / SC-003) — break the `#r` version, run `./fake.sh build -t TemplateCheck` (expect FAIL naming both versions), then `git checkout` and rerun (expect PASS); run a simulated pin bump through the extended skill flow and confirm both pins move together with no manual second edit; record the outcomes to `readiness/deliberate-mismatch-gate.md`, `readiness/simulated-bump-proof.md`, and `readiness/pin-parity-proof.md`

**Checkpoint**: User Story 1 — the `#r` pin matches the props pin, the flow keeps both current, and the gate catches a deliberate mismatch.

---

## Phase 4: User Story 2 (US2) — governance library builds warning-clean (P2)

### Tests First (Principle VI)

- [X] T009 [P] [US2] [skillist: []] Establish the failing-first zero-FS3261 gate — confirm the clean `dotnet build build/Governance/FS.Skia.UI.Build.fsproj --no-incremental` currently emits **34** FS3261 across 8 files and record the before-log excerpt to `readiness/fs3261-before-after.md` (FR-005 baseline, SC-004)

### Implementation — resolve every FS3261 site by safe null handling (behaviour-preserving)

> The per-file `~N` counts below are approximate upper bounds. The authoritative total is the
> clean `--no-incremental` count recorded at T004/T009; resolve to **0**, whatever the real start.

- [X] T010 [P] [US2] [skillist: []] Resolve the FS3261 sites in `build/Governance/GeneratedProduct.fs` (~22 sites, NullableBclString class) by pattern-matching `null` / `nonNull` / `Option.ofObj` with an explicit default — never force-unwrap; behaviour unchanged
- [X] T011 [P] [US2] [skillist: []] Resolve the FS3261 sites in `build/Governance/Front/Governance.fs` (~20 sites) including the `Process.Start` result — `match Process.Start startInfo with null -> Error … | proc -> …` so it **fails fast** (returns `Error`) on a null process instead of dereferencing it (observability preserved per Principle VII)
- [X] T012 [P] [US2] [skillist: []] Resolve the FS3261 sites in `build/Governance/Engine/Model.fs` (~14 sites) including line 72 — make the inferred value provably non-null so the impl matches the **existing** `.fsi` `val featureId: string` (no `.fsi` change, SignatureNullness class)
- [X] T013 [P] [US2] [skillist: []] Resolve the FS3261 sites in `build/Governance/Guidance.fs` (~8) and `build/Governance/Front/BuildProcess.fs` (~8) by safe null handling (NullableBclString class), behaviour unchanged
- [X] T014 [P] [US2] [skillist: []] Resolve the FS3261 sites in `build/Governance/Preflight.fs` (~6), `build/Governance/PerPackageSurface.fs` (~6), and `build/Governance/Front/BuildProcessHealth.fs` (~6) by safe null handling, behaviour unchanged
- [X] T015 [US2] [skillist: []] Remove the project-local `<WarningsNotAsErrors>$(WarningsNotAsErrors);FS3261</WarningsNotAsErrors>` from `build/Governance/FS.Skia.UI.Build.fsproj` (FR-009, contract C3) so FS3261 is now a build **error** for this project only — leave the repo-wide `Directory.Build.props` policy unchanged

### Verification

- [X] T016 [US2] [skillist: fsharp-build-orchestration] Verify SC-004 / SC-005 — a clean `--no-incremental` build emits **0** FS3261 (down from 34); `./fake.sh build -t Dev` is green including every `Governance.Tests` (behaviour preserved, FR-006); a deliberately re-introduced FS3261 now fails the build (escape hatch gone); record the before(34)/after(0) excerpt to `readiness/fs3261-before-after.md`

**Checkpoint**: User Story 2 — the governance library compiles warning-clean and the compiler enforces it.

---

## Phase 5: User Story 3 (US3) — pin-bump flow leaves a clean tree (P3)

- [X] T017 [US3] [skillist: []] Add `specs/*/readiness/package/` to `.gitignore` under the existing Feature-046 evidence-hygiene block (mirroring the `specs/*/readiness/logs/**` precedent), then `git rm`/delete the stray `specs/053-v3-monolith-retirement/readiness/package/local-packages.md` scratch (FR-007, contract C4) — the rule is scoped to the `package/` scratch subdir so authored `.md` evidence elsewhere stays tracked
- [X] T018 [US3] [skillist: []] Verify SC-006 / SC-007 — `git status --porcelain` is empty, the stray file is no longer tracked/present, and a routine framework-internal diff routes to `inner-loop` via `./fake.sh build -t Route` (the governance-path escalation is gone); record to `readiness/clean-tree-proof.md`

**Checkpoint**: User Story 3 — the tree is clean and `Route` reflects real changes.

---

## Phase 6: Integration & gates (escalated maintainer-verify, serialized)

- [X] T019 [skillist: fsharp-build-orchestration] Confirm `./fake.sh build -t Route` (and `Route --enforce`) reports the escalated maintainer-verify tier with every required evidence artifact present, then run the escalated FAKE gate set **sequentially, never concurrently** — `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` — recording aggregate results as **non-authoritative** and rerunning any race-like failure in focused isolation as the authoritative result; logs under `readiness/logs/`
- [X] T020 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the DAG is acyclic, no dangling refs, no `[S*]` surprises, and the structured task metadata plus visible `skillist` mirrors are valid (`verdict=ok`)
- [X] T021 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm `verdict=PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 blocking diff-scan, 0 blocking readiness-contract) with zero synthetic evidence to accept; this feature ships no `[S]` task

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; every proof is a real build log, real grep/diff, a real reverted-mismatch gate run, and a real `git status --porcelain`)_ | | | | | | | | |
