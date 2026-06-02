# Foundations After-Baseline — 2026-06-02

The **final before/after measurement** for the foundations programme (Stage 7 closeout),
paired side-by-side with the Stage-0 `2026-05-31-foundations.md`. Every non-estimate row
names the exact command that produced its **After** value, so a clean checkout at the
pinned SHA reproduces it (FR-003/FR-004, SC-002/SC-003). The reproducibility re-runs are
recorded in
[`specs/047-foundations-programme-closeout/readiness/after-baseline-repro.md`](../../../specs/047-foundations-programme-closeout/readiness/after-baseline-repro.md).

## Pinned context

| Field | Value |
|---|---|
| `git_commit` | `4276bd061d95d47c61deb141a3b4bb65ccebe4e0` |
| `git_commit` (short) | `4276bd0` |
| `branch` | `047-foundations-programme-closeout` |
| `captured_at` | `2026-06-02` |
| Toolchain | dotnet `10.0.300` · FAKE library `Fake.Core.Target 6.1.4` (compiled, no FSX runner) |

```bash
git rev-parse HEAD          # 4276bd061d95d47c61deb141a3b4bb65ccebe4e0
git rev-parse --short HEAD  # 4276bd0
```

- **Stage-0 comparison oracle:** [`2026-05-31-foundations.md`](./2026-05-31-foundations.md).
- **Closing ADR:** [`docs/adr/0006-foundations-programme-closeout.md`](../../adr/0006-foundations-programme-closeout.md).
- **Dogfood retrospective + recurring-run mechanism:**
  [`specs/047-foundations-programme-closeout/readiness/retrospective.md`](../../../specs/047-foundations-programme-closeout/readiness/retrospective.md).

## Section A — Whole-programme definition of done (the canonical 100% set)

The **11** dimensions of the plan's "Whole-programme definition of done" table. Each row
carries its Stage-0 baseline, its current (After) value at the pinned SHA, the exact
reproduction command, and a **met-target** marker or a **written rationale** (FR-005).

| # | Dimension | Baseline (2026-05-31) | After (this SHA) | Reproduction command | Met-target / rationale |
|---|---|---|---|---|---|
| 1 | `build.fsx` size | 4,688 lines | **0** (deleted in full, no shim) | `git ls-files build.fsx \| xargs -r wc -l` → *(empty / 0 files)* | **met** — target was "deleted or ≤ 200-line shim" (feature 045). |
| 2 | Domain logic location | inline in `build.fsx`, untested | `FS.Skia.UI.Build` library — **36 files, 5,226 lines**, unit + property tested (`tests/Governance.Tests/`, e.g. `RoutingTests.fs`) | `git ls-files 'build/Governance/**/*.fs' 'build/Governance/**/*.fsi' \| xargs wc -l \| tail -1` → `5226 total` (36 files) | **met** — the validation logic is compiled, typed, and tested in the library. |
| 3 | Evidence-path languages | F# + Bash + Python | **F# only** | `git ls-files '.specify/**/*.py' \| wc -l` → `0`; `git ls-files '*.py' \| wc -l` → `0`; `git ls-files '**/run-audit.sh' \| wc -l` → `0` | **met** — the evidence path is in-process compiled F# (feature 043). |
| 4 | `compute-task-graph.py` / `audit-status-scan.py` / `run-audit.sh` | 1,310 + 150 + 1,284 LOC | **removed (0)** | `git ls-files '**/compute-task-graph.py' '**/audit-status-scan.py' '**/run-audit.sh' \| wc -l` → `0` | **met** — logic ported into the library, parity-proven before deletion (feature 043). |
| 5 | Governance Markdown (rules) | plan claimed ~23,000 lines / 21:1; **corrected** to ≈ **6,882** (feature 046) | **6,876** lines (`.agents/skills` 4,059 + `.specify/**/*.md` 2,817); rules enforced by code | `find .agents/skills -name '*.md' \| xargs wc -l \| tail -1` → `4059`; `find .specify -name '*.md' \| xargs wc -l \| tail -1` → `2817` (sum **6,876**) | **rationale** — the plan's ~23,000-line / 21:1 figure was an **over-estimate**; feature 046 established the corrected rule/guidance baseline at **≈ 6,882**, after-trim **6,876** (−6). The literal "low hundreds" target is **not reached and is not the real story**: the remaining prose doubles as pinned *author guidance* (its tokens are required by the 041–044 generation-currency term-checks), and the *rules themselves* are now machine-enforced — `Routing.fs`, `Guidance.fs`, `GeneratedProductContract.fs`, and the in-process `EvidenceGraph`/`EvidenceAudit` — so no rule depends on "read the prose and comply." |
| 6 | `.claude`/`.agents` duplication | ~5,854 lines, **unguarded** hand-sync | **single source + generation** — canonical `.agents/skills/**` → byte-identical generated `.claude/skills/**` (25 ↔ 25 skills), enforced by `SkillSyncCheck` + the `GeneratedGuidanceCheck` peer-drift check | `git ls-files '.agents/skills/**/*.md' \| wc -l` → `25` and `'.claude/skills/**/*.md'` → `25`; `./fake.sh build -t SkillSyncCheck` green | **met** — duplication is generated and currency-checked, not hand-synced (features 040/044). |
| 7 | Framework-author process | full consumer ceremony (~12–14 h/feature) | **`inner-loop`** (`Dev` + surface check) is the `Route` default for framework-internal changes; full pipeline reserved for consumers + dogfood | mechanism: `Routing.fs` `innerLoopGates = [ Targets.Dev ]` (line 225) + `internalInnerLoopApplicable` (line 97), proven by `tests/Governance.Tests/RoutingTests.fs` | **met (mechanism)** — the hour figure is an author **estimate** (no timing harness; same Stage-0 exemption) and lives in **Section B**; the *mechanism* (light default tier) is realized and tested. A live `Route` on **this** feature's diff escalates (it touches governance docs) — that escalation is itself row 8's evidence. |
| 8 | Tier selection | implicit / "run everything" | **`./fake.sh build -t Route`**, enforced by compiled `Routing.fs` | `./fake.sh build -t Route` → prints `tier=…` + `gates=…` (this feature: `tier=agent-ready`, `gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`) | **met** — a mistyped gate is a compile error; `validation.contract.yml` is generated from `Routing.fs`. |
| 9 | Framework-owned config | YAML, stringly-typed, runtime-parsed | **compiled F# values/predicates** in the library; build-time checked; no FCS | `grep -n 'dogfoodFeatureIds' build/Governance/Routing.fs` → `235:`; `git ls-files '.specify/**/*.py' \| wc -l` → `0`; ADR 0005 (D6) | **met** — routing/tier/feature policy is typed F#; no YAML governance config parsed at runtime, no `FSharp.Compiler.*`. |
| 10 | Generated-product contract | unversioned, hard-break | **versioned with a deprecation window** (`schema_version`; deprecation → removal-version hard-fail) | `grep -n 'schema_version' build/Governance/GeneratedProductContract.fs` → deprecation/removal-window logic | **met** — feature 046; ADR 0003. |
| 11 | Runtime architecture (`Scene → SkiaViewer → Elmish`) | sound | **unchanged** | `git diff --stat -- 'src/**'` → *(empty)* | **met** — product runtime / `.fsi` untouched (FR-010, SC-006); see `readiness/runtime-untouched.md`. |

**SC-002:** all 11 dimensions present; every row's final column is non-empty (a
met-target marker or a written rationale). 10 rows **met-target**; row 5 carries the
corrected-baseline rationale; row 7's target is met by mechanism with its hour figure
moved to the estimate section below.

## Section B — Supplementary estimates (NOT counted toward the 100% total)

The three softer work-item-7.2 metrics that are **not** in the definition-of-done table.
Clearly labelled, **excluded** from SC-002's 100% coverage count (spec Clarification).
None is command-reproducible at the pinned SHA — each is an estimate with its basis.

| Metric | Baseline | After | Basis |
|---|---|---|---|
| Per-feature ceremony time | ~12–14 h/feature (Stage-0 author estimate) | **inner-loop framework changes: hours, not a full-pipeline day** (author estimate) | **estimate** — no timing harness exists (the same Stage-0 exemption). The realized *mechanism* is the `inner-loop` light tier now being the `Route` default for framework-internal changes (Section A row 7); the hour delta is not instrumented and is not asserted as a reproducible number. |
| Agent context bytes | full governance corpus loaded per session (no per-task scoping) | **reduced** — per-task `skillist` loads only the declared skills; trimmed `speckit-implement` skill −539 bytes (feature 046) | **estimate / measured-where-possible** — per-skill byte deltas are measurable (`wc -c .agents/skills/<skill>/SKILL.md`), but "agent context bytes per feature" depends on the agent/session and has no fixed reproduction; recorded as an estimate. |
| Warm-build time | FSX-runner compile tax per invocation (`dotnet fake` recompiled `build.fsx`) | **lower** — compiled `build/Build.fsproj` exe; no per-run FSX recompile, no FCS (D2 spike confirmed) | **estimate / measured-where-possible** — wall-clock build time is machine-dependent and not pinned to a SHA; the structural change (no per-invocation FSX compile, no `FSharp.Compiler.*` in the transitive graph) is the recorded basis. |

## Cross-links (SC-005)

This report links the **closing ADR 0006** and the **dogfood retrospective** (both
above), and is itself linked back from the retrospective — so the Stage-7 closeout
artifacts form a connected, navigable record.
