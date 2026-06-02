# Quickstart — Reproduce the Closeout Proofs

A reviewer follows this to verify Stage 7 **without trusting prose** — every claim is reproducible
from a committed command. Run from the repo root at the feature SHA.

## 1. Scaffolding is gone (US1 / SC-001)

```bash
# Dead artifacts — each prints nothing:
git ls-files build.fsx
git ls-files '**/select-tier.fsx'
git ls-files '**/run-audit.sh'
git ls-files '.specify/**/*.py'

# Flag/runner — full token grep then the scoped (allowlisted) grep; scoped prints nothing:
git grep -n -- '--legacy-evidence'                                   # only specs/043 + impl-plan prose
git grep -nE 'fake-cli|dotnet fake|FSharp\.Compiler\.' -- . ':!template/base/build.fsx'
```

Cross-check each retained match against the allowlist in
[`readiness/scaffolding-proof.md`](./readiness/scaffolding-proof.md): frozen history, the
`Guidance.fs` enforcement regex, the `Directory.Packages.props` absence-comments, or live-FAKE
diagnostics. Any match outside those classes should already be removed/corrected.

## 2. The programme's promises, measured (US2 / SC-002 / SC-003)

Open [`docs/reports/_baselines/2026-06-02-foundations-after.md`](../../docs/reports/_baselines/2026-06-02-foundations-after.md).
For any definition-of-done row, copy its **Reproduction command** and run it — the output matches
the **After** value. Spot-checks:

```bash
git ls-files build.fsx | xargs -r wc -l                              # 0 — build.fsx deleted
git ls-files '**/compute-task-graph.py' '**/audit-status-scan.py' '**/run-audit.sh' | wc -l  # 0
git rev-parse HEAD                                                   # matches the row's pinned SHA
```

Confirm there are 11 definition-of-done rows (each with a met-target marker or a rationale) and that
the three softer metrics (ceremony time, context bytes, warm-build time) appear only in the
**supplementary estimate** section.

## 3. A new contributor can work from the docs (US3 / SC-004)

Read `README.md`, `docs/reports/build.md`, `docs/reports/speckit.md`, `CLAUDE.md`, `AGENTS.md`.
Confirm each describes the two-tier `Route` process, `Route` as the entry point, `FS.Skia.UI.Build`
as the single home of all rules, and the generate-don't-sync principle — and that none presents the
serialized six-target order as the unconditional default. Then dry-run the contributor path:

```bash
./fake.sh build -t Route          # prints the tier + minimal gate list for the current diff
```

Read the closing ADR [`docs/adr/0006-foundations-programme-closeout.md`](../../docs/adr/0006-foundations-programme-closeout.md)
(outcome + realized D1–D6 + steady-state model), cross-linked from the after-baseline.

## 4. The harness cannot silently rot (US4 / SC-005)

Open [`readiness/retrospective.md`](./readiness/retrospective.md): it confirms features 042 and 043
ran the full serialized pipeline green and identifies the recurring-run mechanism. Find the tracked
schedule-definition file (per [`contracts/recurring-run.md`](./contracts/recurring-run.md)) and the
documented manual fallback — runnable with no live external CI service.

## 5. Runtime + gates (SC-006 / SC-007)

```bash
git diff --stat -- 'src/**'        # empty — product runtime untouched
# Escalated serialized set (run sequentially — FAKE is not concurrency-safe):
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph        # verdict=ok
./fake.sh build -t EvidenceAudit        # verdict=PASS, zero synthetic
```

(Modulo the documented pre-existing `FsiTranscripts` / `SkiaViewer.Tests` headless-flake Class-C
exclusions disclosed by prior foundations features.)
