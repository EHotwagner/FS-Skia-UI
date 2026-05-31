# Foundations Baseline — 2026-05-31

SHA-pinned "before" snapshot for the foundations programme. Every count names
the exact command that produced it so a clean checkout at the pinned SHA can
reproduce it (FR-001, FR-009, SC-001). This is the measurement floor every later
stage proves "no regression" / "parity" against.

## Pinned context

| Field | Value |
|---|---|
| `git_commit` | `34faf1ed61ec0ec2a8a2a81168517cb5ccf499d1` |
| `git_commit` (short) | `34faf1e` |
| `branch` | `039-foundations-baseline-spike` |
| `captured_at` | `2026-05-31T13:16:44Z` |
| Toolchain | dotnet `10.0.300` · python `3.14.5` · FAKE `6.1.4` |

```bash
git rev-parse HEAD          # 34faf1ed61ec0ec2a8a2a81168517cb5ccf499d1
date -u +%Y-%m-%dT%H:%M:%SZ # 2026-05-31T13:16:44Z
```

**Working-tree note (spec Edge Cases — recorded, not failed):** the snapshot was
taken while feature 039 itself was in progress. Every corpus below is measured
from **committed** content via `git ls-files`, so the in-flight, not-yet-committed
039 artifacts (the new `build/**` projects, ADRs, fixtures, baseline/outcome
docs) are **excluded** — these numbers are the genuine "before" state. The two
new build-tooling projects did not exist as committed files at the pinned commit.

## Script size — orchestration vs validation (FR-001)

```bash
wc -l build.fsx                         # 4688
grep -cE 'StartTarget "' build.fsx      # 45   (orchestration: target dispatch cases)
grep -cE 'let .*[Vv]alidate' build.fsx  # 22   (validation: Validate* functions)
```

| Metric | Value | Class |
|---|---:|---|
| `build.fsx` total lines | **4688** | — |
| `StartTarget "<name>"` dispatch cases | **45** | orchestration (MEL/`interpret`/`StartTarget`) |
| `Validate*` functions | **22** | validation logic |

`build.fsx` is the monolith the programme decomposes: a thin orchestration spine
(the 45 target cases routed through the MEL `interpret` loop) wrapping a large
bulk of inline validation logic (the 22 `Validate*` functions plus helpers). The
orchestration/validation split is reported structurally (marker counts); exact
per-line attribution is deferred to the Stage 4 port, which moves the validation
bulk into the compiled governance library. The Python evidence engine (below) is
the first validation slice already extracted out of `build.fsx`.

## Governance Markdown volume (FR-001)

```bash
git ls-files '.claude/skills/*.md' | wc -l                                   # 19
git ls-files '.agents/skills/*.md' | wc -l                                   # 19
git ls-files '.claude/skills/*.md' '.agents/skills/*.md' | xargs cat | wc -l # 5854
wc -l .specify/memory/constitution.md                                        # 336
git ls-files '.specify/templates/*.md' '.specify/presets/*/templates/*.md' | xargs cat | wc -l  # 1508
git ls-files 'specs/**/*.md' | wc -l                                         # 773
git ls-files 'specs/**/*.md' | xargs cat | wc -l                            # 58880
```

| Corpus | Files | Lines |
|---|---:|---:|
| `.claude/skills` ↔ `.agents/skills` mirror (combined) | 38 (19 ↔ 19) | 5854 |
| `.specify/memory/constitution.md` (governing-principles doc) | 1 | 336 |
| Templates (`.specify/templates` + preset templates) | — | 1508 |
| `specs/**/*.md` | 773 | 58880 |

The `.claude`/`.agents` skill pair is a byte-identical mirror (synchronized
peers, 19 ↔ 19); the 5854-line combined count is the governance-skill prose the
single-source-generation stage (Stage 6) targets. The 58880-line `specs/**`
volume is dominated by per-feature ceremony artifacts across 40 features.

## F# / Bash / Python LOC mix (FR-001)

```bash
git ls-files '*.fs' '*.fsi' '*.fsx' | xargs cat | wc -l   # 44398  (191 files)
git ls-files '*.sh'                  | xargs cat | wc -l   #  3744  (17 files)
git ls-files '*.py'                  | xargs cat | wc -l   #  1460  (2 files)
```

| Language | Files | LOC |
|---|---:|---:|
| F# (`.fs`/`.fsi`/`.fsx`) | 191 | 44398 |
| Bash (`.sh`) | 17 | 3744 |
| Python (`.py`) | 2 | 1460 |

The two Python files are the evidence engine
(`.specify/extensions/evidence/scripts/python/compute-task-graph.py` +
`audit-status-scan.py`) — the exact code Stage 4 ports to compiled F# and proves
parity against the golden fixtures below. The F#/Bash/Python balance is the
migration dial the programme moves (Python+Bash governance tooling → compiled
F#).

## Per-feature ceremony-time estimate (FR-001 — explicit estimate, exempt from the measurement-command rule)

There are **40** feature directories under `specs/` at the pinned SHA:

```bash
find specs -maxdepth 1 -mindepth 1 -type d | wc -l   # 40
```

**Estimate: ~12–14 h of ceremony per feature** under the current single-tier
process. This figure is carried forward from the foundations implementation plan
(`docs/reports/2026-05-31-1049-foundations-implementation-plan.md`) and is an
**author estimate, not an instrumented timing** (no timing harness exists yet, so
per FR-001 it is exempt from the measurement-command rule and labelled an
estimate). Derivation inputs: authoring spec + plan + tasks + tasks.deps.yml,
running the serialized FAKE gate sequence, capturing readiness evidence, and
resolving evidence-audit findings — repeated per feature. The two-tier `Route`
(Stage 1) is expected to cut this for framework-author-loop features; this
baseline is the "before" the cut is measured against.

## Golden-fixture manifest (FR-002 / SC-001) — Stage 4 parity oracle

Fixtures committed under
`tests/Governance.Tests/fixtures/evidence-golden/<feature>/` — each with
`task-graph.json`, `task-graph.md`, and `audit-counts.txt`. Every fixture's
`source_commit` equals this baseline's `git_commit`
(`34faf1ed61ec0ec2a8a2a81168517cb5ccf499d1`). Reproducibility is verified
byte-for-byte (SHA-1) by re-running the existing engine (FR-003, SC-002); the
capture/verify procedure is in the fixtures
[`README.md`](../../../tests/Governance.Tests/fixtures/evidence-golden/README.md).

| Role | Feature | `accepted-seh` | `unaccepted-synthetic` | `auto-synthetic` | `late-seh` | real-tasks |
|---|---|---:|---:|---:|---:|---:|
| current / most-recent completed | `038-authoring-guidance-consistency` | 0 | 0 | 0 | 0 | 38 |
| historical | `037-authoring-audit-robustness` | 0 | 0 | 0 | 0 | 30 |
| historical (substitute for 017) | `036-archive-readiness-api-docs` | **1** | 0 | 0 | 0 | 32 |

**Substitution (recorded per spec Edge Cases / FR-003):** the plan named
`017-synthetic-error-evidence` as the third source, but `017` does **not**
produce a stable evidence output at the pinned SHA — its graph compute fails
(`exit 3`, `verdict: error`) because its skilled tasks have no committed
`readiness/skill-loading-evidence.md`, so the audit halts before a count block.
Rather than commit an unstable fixture, `017` is replaced by
`036-archive-readiness-api-docs`, the merged feature that passes graph compute
deterministically **and** carries an accepted `[SEH]` task (`accepted-seh=1`,
T005), preserving the synthetic-propagation coverage `017` was chosen for. See
the fixtures README for the coverage-honesty note (no stable source exercises
`[S*]` auto-synthetic / unaccepted counts; that is a documented follow-up).

This fixture set is designated the **Stage 4 parity oracle**: the ported F#
engine must reproduce these files byte-for-byte before the Python is deleted.

## Programme meta-process link (FR-008 / SC-007)

The authoritative programme meta-process — the default process tier for
foundations features and the named dogfood feature set — is recorded in
[`specs/039-foundations-baseline-spike/plan.md` §Programme Meta-Process](../../../specs/039-foundations-baseline-spike/plan.md#programme-meta-process-fr-008--recorded-here-the-single-discoverable-place).
That section is the single discoverable record for SC-007; this baseline links to
it rather than duplicating it.

## Spike outcome link

The D2 build-library spike outcome — **D2 confirmed** (clean compiled front-end
drives a target whose body lives in the governance library; no
FSharp.Compiler.Service) — is recorded in
[`2026-05-31-spike-d2-outcome.md`](./2026-05-31-spike-d2-outcome.md).
