---
title: Implementation Completeness Review - Foundations Rewrite + V3 Monolith Retirement
---

# Implementation Completeness Review: Foundations Rewrite + V3 Monolith Retirement

- **Date:** 2026-06-02 22:30 CEST
- **Author:** Claude Code (review requested by maintainer)
- **Status:** Findings report. Read-only analysis; no code changed.
- **Scope:** Verifies whether the work described in three planning/analysis reports has
  actually shipped, flags problems/omissions found on disk, and proposes next steps.
- **Reports reviewed:**
  - [`2026-05-31-0908-foundations-rewrite-analysis.md`](./2026-05-31-0908-foundations-rewrite-analysis.md) — the four-question analysis (process tiers, build project, Python→F#, prose→code).
  - [`2026-05-31-1049-foundations-implementation-plan.md`](./2026-05-31-1049-foundations-implementation-plan.md) — the staged foundations programme (Stages 0–7).
  - [`2026-06-02-v3-modular-distribution-implementation-plan.md`](./2026-06-02-v3-modular-distribution-implementation-plan.md) — the staged V3 monolith-retirement programme (Stages 0–5).
- **Repo state at review:** branch `main`, HEAD `ca7a8b6`; working tree clean except one
  untracked readiness file (see §4.3).

---

## TL;DR

**Both programmes are fully implemented and merged to `main`.** Every stage of the
foundations rewrite (features 039–047) and every stage of the V3 monolith retirement
(features 048–053) landed, with the headline deletions and new gates verified on disk and
the compiled build front-end confirmed *running* (`./fake.sh build -t Route` executes the
dedicated `build/Build.fsproj` exe and prints a typed routing decision in 64 ms).

The residual items are **minor and almost entirely disclosed**: a real-but-low-impact
version-pin drift in the generated template's `build.fsx` (§4.1), lingering FS3261 nullness
**warnings** in the governance library (§4.2), an uncommitted readiness scratch file (§4.3),
and a set of **explicitly-deferred / non-goal** design items that the now-clean architecture
finally unblocks (§5). No stage is missing, mis-built, or silently dropped.

---

## 1. Method

I treated each plan's "definition of done" as a checklist and verified it against the live
tree, not against the plans' own self-reported status. Concretely:

- `git log`/`git ls-files` to confirm merges, deletions, and that removed files are not
  merely untracked.
- Directory/grep sweeps for the monolith (`src/Lib`, `FS.Skia.UI` package id), the deleted
  build script (`build.fsx`), and the ported-away scripts (`compute-task-graph.py`,
  `audit-status-scan.py`, `run-audit.sh`).
- Reading the two closeout after-baselines and the ADR set.
- **Empirically running** `./fake.sh build -t Route` to prove the keystone (compiled
  front-end) actually works, not just that its files exist.

---

## 2. Foundations rewrite (analysis 0908 → plan 1049) — verdict: **COMPLETE**

The analysis converged on **one keystone** (a tested F# governance library extracted from
`build.fsx`) and **one policy** (a two-tier process). Both shipped, plus all supporting
stages.

| Stage | Plan deliverable | Feature | On-disk verification | Status |
|---|---|---|---|---|
| 0 | Baselines + 5 ADRs + D2 spike | 039 | `docs/adr/0001–0005`; `_baselines/2026-05-31-foundations.md` + `-spike-d2-outcome.md` | ✔ |
| 1 | Two-tier `Route` process (compiled `Routing.fs`) | 042 | `Route` runs; prints `developer-class/tier/gates`; `validation.contract.yml` generated from `Routing.fs` | ✔ |
| 2 | Single-source generation (`.claude`←`.agents`, constitution, skillist) | 040 + 044 | `SkillSyncCheck`; 25↔25 skills; constitution fragment markers | ✔ |
| 3 | Governance library skeleton + first validators | 041 | `build/Governance/{Targets,Findings,TargetMetadata,Capabilities}.fs(i)` | ✔ |
| 4 | Python evidence engine → in-process F# | 043 | `build/Governance/Evidence/**` (TaskParser, DepsParser, Graph, Audit, StatusRegion, DiffScan, Render); **0 tracked `.py`/`run-audit.sh`** | ✔ |
| 5 | **`build.fsx` deleted in full** + dedicated build project | 045 | root `build.fsx` **absent**; `build/Build.fsproj` + `Program.fs` + `Engine/{Model,Update,Interpret}.fs`; `fake.sh` runs `dotnet run --project build/Build.fsproj` | ✔ |
| 6 | `ConstitutionCheck` gate + contract `schema_version` + prose trim + `.gitignore` | 046 | `GeneratedProductContract.fs` deprecation/removal window | ✔ |
| 7 | Closeout: grep-proofs, after-baseline, ADR 0006, doc pass | 047 | `_baselines/2026-06-02-foundations-after.md`; `docs/adr/0006` | ✔ |

**Key empirical proof.** The single highest-risk claim across both programmes was "the
4,688-line `build.fsx` is gone and replaced by a compiled exe." Verified live:

```
$ ./fake.sh build -t Route
developer-class=framework-author
tier=agent-ready
gates=Dev, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance
Finished (Success) 'Route' in 00:00:00.06
```

This proves (a) the dedicated FAKE exe builds and dispatches, (b) the typed `Routing.fs`
selector is the live decision engine, and (c) the DualDisplay→X11 graphics normalization
(features 049/053) is wired into the front-end. The tri-language evidence boundary is
genuinely gone: `git ls-files '*.py'` and `**/run-audit.sh` both return 0.

### 2.1 The four original questions — all answered in code

- **Q1 two-tier process** → `Routing.fs` with `DeveloperClass`/`Tier`/glob-predicate rules;
  `inner-loop = [Dev]` for framework-internal work, escalation for consumer/governance paths.
- **Q2 build project** → went beyond the "dedicated build project" framing to the analysis's
  recommended end-state: a tested governance *library* + thin compiled front-end.
- **Q3 Bash/Python → F#** → ported in full; the flagship gate computes in-process with typed
  results.
- **Q4 prose → code** → bucket (a) rules codified (`Routing`, `Guidance`,
  `GeneratedProductContract`, in-process `EvidenceGraph/Audit`); bucket (c) duplication
  generated, not synced.

---

## 3. V3 monolith retirement (plan 0602) — verdict: **COMPLETE**

| Stage | Plan deliverable | Feature | On-disk verification | Status |
|---|---|---|---|---|
| 0 | Baseline + ADRs 0007–0011 + `PerPackageSurfaceDiff` + parity oracle | 048 | `docs/adr/0007–0011`; `_baselines/2026-06-02-v3-before.md` | ✔ |
| 1 | **Keystone** host extraction + scene-vocabulary unification | 050 | `src/SkiaViewer/Host/**`; `SceneConversion.fs` gone; no `SkiaViewer→Lib` ref | ✔ |
| 2 | Relocate `AgentValidation` → `FS.Skia.UI.Build` | 051 | `build/Governance/AgentValidation.fs(i)`; `knownGates` now governance config | ✔ |
| 3–4 | `FS.Skia.UI.Input` package; decouple samples/tests; retire parity bridge | 052 | `src/Input/Input.fsproj` (9th package); `tests/Input.Tests` | ✔ |
| 5 | **Delete `src/Lib`**, unpublish `FS.Skia.UI`, Route-gate baselines, cleanliness gate, migration docs, ADR 0012, after-measurement | 053 | see below | ✔ |

**Stage 5 verified item-by-item:**

- `src/Lib` **deleted** — `src/` holds exactly nine packages (Controls, Controls.Elmish,
  Elmish, Input, KeyboardInput, Layout, Scene, SkiaViewer, Testing). No `Lib`.
- `FS.Skia.UI` **unpublished** — absent from `Directory.Packages.props`, no template pin, not
  in `packProjects`. The only tracked `FS.Skia.UI` (no suffix) hits are in **frozen historical
  spec logs** under `specs/015,032/...` and the after-baseline's own grep-command text — not
  live references. (The `src/**/obj/Release/*.nuspec` hits are gitignored build output.)
- `PerPackageSurfaceDiff` **Route-gated** — `Routing.fs:214` adds it to the `package-surface`
  rule; rendered into `validation.contract.yml:155`; on the `knownGates` allowlist.
- **Cleanliness gate** present — `GeneratedProduct.fs:958` asserts generated `app`/`governed`
  carry no `samples/`, framework `docs/reports`, historical `specs/`, or framework README.
- **Migration docs** — `docs/migration/v2-to-v3.md` with the full surface map.
- **Closeout** — `docs/adr/0012`; `_baselines/2026-06-02-v3-after.md` (LOC→0, dup-types→0,
  transitive-pull→none).
- The Stage-2-noted stale `Routing.fs` comment pointing at `src/Lib/AgentValidation.fs` was
  **resolved** — the comment at lines 201–210 now correctly describes the relocated allowlist.

The "modularity leak" (default `app` profile transitively pulling the whole monolith via
`SkiaViewer→Lib`) is closed: `SkiaViewer` references only `Scene` + `KeyboardInput`.

---

## 4. Problems & omissions found

All are minor. Listed worst-first.

### 4.1 Version-pin drift in the generated template's `build.fsx` (concrete, low-impact)

`template/base/build.fsx:1` pins the evidence engine at

```
#r "nuget: FS.Skia.UI.Build, 0.1.45-preview.1"
```

while `template/base/Directory.Packages.props` pins the same package at **`0.1.56-preview.1`**
— an 11-revision gap — even though the build.fsx comment states the `#r` "is kept in sync with
the FS.Skia.UI.Build pin in Directory.Packages.props."

- **Cause:** the recurring "Update fs-skia-ui template package pins" flow updates
  `Directory.Packages.props` but does **not** rewrite the `#r "nuget:"` literal in the
  generated `build.fsx`.
- **Why it slips the gate:** `template/base/tests/Product.Tests/Tests.fs:533` asserts only the
  substring `#r "nuget: FS.Skia.UI.Build` — it does not check the version, so the drift is
  invisible to `TemplateCheck`/`GeneratedProductCheck`.
- **Impact:** a freshly generated app would restore an older governance engine (0.1.45) for its
  own `EvidenceGraph/Audit` than the version its product packages reference. Functionally
  low-risk (the engine API is stable across these previews) but it contradicts the "pinned for
  every release" guarantee and is a latent footgun once the engine API changes.
- **Fix:** have the pin-bump step also rewrite the `build.fsx` `#r` literal, and tighten the
  Product.Tests assertion to match the *exact* `Directory.Packages.props` version.

### 4.2 FS3261 nullness warnings remain in the governance library

The `Route` build emits FS3261 nullness warnings in `build/Governance/Guidance.fs` (lines
~522, 543) and `build/Governance/Front/Governance.fs` (lines ~185–189, 328, 335). They are
**warnings, not errors** — FS3261 is evidently not promoted by the project's
`TreatWarningsAsErrors` / `FS0078`-as-error policy — so the build is green. Still, they sit
oddly against the framework's own warnings-as-errors discipline and are worth clearing
(pattern-match the nullable `Process`/`string` values) or explicitly suppressing with a note.

### 4.3 Uncommitted readiness scratch file

`specs/053-v3-monolith-retirement/readiness/package/local-packages.md` is untracked. Because
it lives under an evidence/governance path, it **escalates `Route`** to `agent-ready`
(matched-rule `evidence-governance`) — which is exactly why the live `Route` above did not show
the routine `inner-loop`. This is the known "readiness-dir churn" hazard: commit it (if it's
genuine 053 evidence) or `.gitignore` it (if it's pack-flow scratch) so the tree is clean and
`Route` reflects real changes.

### 4.4 Headline targets met "by mechanism," not by measurement (disclosed)

The closeout baselines are commendably honest that several *numeric* targets from the original
analysis were not literally hit:

- **Governance-Markdown "low hundreds of lines"** — **not met** (≈6,876 lines remain). The
  after-baseline rightly notes the analysis's "~23,000 lines / 21:1" figure was an
  over-estimate (true rule+guidance baseline ≈6,882), so the *goal itself* was anchored to a
  wrong number. The remaining prose now doubles as pinned author-guidance that the 041–044
  generation-currency term-checks depend on, so it cannot shrink freely without decoupling
  guidance prose from the term anchors (see §5.5).
- **Ceremony time / agent-context bytes / warm-build time** — recorded as **estimates only**;
  no timing or token-accounting harness was ever built. The *mechanisms* (light default tier,
  per-task skillist loading, compiled front-end with no per-run FSX compile) are real and
  tested, but the efficiency *wins the programme was sold on* are not instrumented.

This is a transparency strength, not a defect — but it means "did framework iteration actually
get faster/cheaper?" remains unproven by data.

### 4.5 What is *not* a problem (checked and cleared)

- `template/base/build.fsx` **existing is correct** — it is the generated consumer's own
  governance script that `#r`s the *packaged* `FS.Skia.UI.Build` engine in-process (ADR D1 /
  feature 043). The root framework `build.fsx` is the one that was deleted; these are different
  files. (Its only issue is the version pin in §4.1.)
- Template **profiles** `app` / `headless-scene` / `governed` / `sample-pack` all exist in
  `.template.config/template.json` — so the "sample-pack home" Stage 3 needed is first-class,
  and the design's profile expansion is further along than the V3 plan's Non-Goals implied.

---

## 5. Possible next steps

Ordered low-risk/high-value first. None is required for correctness — both programmes are done.

### 5.1 Housekeeping (hours)
- Fix the `build.fsx` pin drift (§4.1): teach the pin-bump flow to rewrite the `#r` literal;
  tighten the Product.Tests version assertion. **This is the only finding with user-visible
  risk.**
- Clear or suppress the FS3261 warnings (§4.2).
- Commit-or-ignore the untracked readiness file (§4.3) so `Route` reflects reality.

### 5.2 Charts/DataGrid package split — the largest deferred design item
`v3Design.md` envisioned a separate `FS.Skia.UI.Charts` package; both V3 and foundations plans
list it as an explicit Non-Goal. Charts/DataGrid still live inside `src/Controls`. With the
monolith gone and the per-package surface-baseline machinery in place, this is now the natural
next modularization — and the cleanest remaining gap between the shipped state and the original
design vision.

### 5.3 Close the visual-parity gap (disclosed deferral)
Reference screenshots for `effects-gallery` / `screenshot-gallery` were never captured (no
non-interactive entry point), and persistent-window first-frame capture is headless-GPU-
infeasible in CI. Scene-output parity is the authoritative oracle, so this is acceptable — but
adding a non-interactive screenshot entry point would let those two galleries get true visual
parity coverage and retire the last "infeasible-and-disclosed" caveat.

### 5.4 Instrument the efficiency claims (if they matter)
If the programme's velocity/cost goals are to be *demonstrated* rather than asserted (§4.4),
add a lightweight timing/token harness around inner-loop vs full-pipeline runs. Otherwise,
formally retire those metrics as "mechanism delivered, measurement out of scope."

### 5.5 Decouple author-guidance prose from generation-currency anchors
The reason governance Markdown can't shrink (§4.4) is that term-checks pin specific phrases.
Separating "genuine guidance an agent reads" from "term anchors the currency check requires"
would let the prose actually reach the "low hundreds" goal — or, more honestly, let the goal be
restated against the corrected ≈6,882 baseline.

### 5.6 Programme retrospective follow-through
Feature 047 created a tracked recurring-run schedule for the dogfood set (to stop the light tier
silently becoming the only tier and rotting the harness). Confirm that schedule is actually
firing now that both programmes are closed; a dormant dogfood schedule is the one way the
two-tier process quietly degrades.

---

## 6. Conclusion

The maintainer's two large programmes — the foundations tooling rewrite and the V3 monolith
retirement — are **both complete and merged**, and the central, highest-risk claims
(`build.fsx` deleted for a compiled front-end; `src/Lib` deleted with the leak closed) are
**verified on disk and at runtime**, not merely self-reported. The codebase is now a modular
distribution of nine focused packages plus a tested governance engine, with a typed two-tier
`Route` process and an in-process F# evidence gate.

The only finding with any user-visible risk is the template `build.fsx` engine-pin drift
(§4.1); everything else is cosmetic housekeeping or work the plans deliberately scoped out. The
most valuable forward move is the deferred **Charts package split** (§5.2), which would carry the
modular-distribution design across its last unfinished boundary.
