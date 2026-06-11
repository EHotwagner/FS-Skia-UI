# Validation log — feature 101 (R7, T016)

authoritative-command=./fake.sh build -t Route ; then run exactly the gates it prints, sequentially
artifact-path=specs/101-layout-dirty-set-guard/readiness/
status=pass
failure-class=gate-failure
next-action=none — all Route-printed gates ran sequentially and passed (GeneratedProductCheck classified non-authoritative per env, see note)

## Route (authoritative)

`./fake.sh build -t Route` against the working-tree diff prints:

- developer-class=framework-author
- tier=agent-ready
- matched-rules=controls-public-surface, evidence-governance, specify-catchall, docs-only
- gates=Dev, PackageSurfaceCheck, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit

The `src/Controls/**/*.fs` edit matches `controls-public-surface` (this repo escalates any Controls
source edit), so the escalated set above is run **sequentially** (shared `.fake` state). The aggregate
is **non-authoritative** unless re-confirmed sequentially (see `aggregate-hang-diagnostics.md`).

## Gate results (run sequentially, in deterministic order)

| Gate | Verdict | Notes |
|---|---|---|
| Dev | PASS | Build + SampleContractSmoke + Test all Success; 3m31s, exit 0. New Feature101 suite (12/12) + unchanged Feature097 R2 suites green. |
| EvidenceGraph | PASS | acyclic + consistent; feature-directory + tasks echoed; 0 `[S]`/`[S*]`; see `evidence-graph.md`. |
| EvidenceAudit | PASS | total-blockers=0, diff-scan-hits=0, window-visibility-hits=0, unaccepted-synthetic-tasks=0, auto-synthetic-tasks=0; see `evidence-audit.md`. |
| PackageSurfaceCheck | PASS | zero public-surface drift (no `.fsi` moved). |
| FsiTranscripts | PASS | new `[<Literal>] private` constants add no FSI-prelude surface. |
| ControlsCatalogCheck | PASS | catalog untouched. |
| ControlsCatalogGenerationCheck | PASS | catalog generation current. |
| DesignTokenDrift | PASS | DTCG tokens untouched. |
| ContrastCheck | PASS | contrast unaffected. |
| ControlsInteractionCheck | PASS | Controls interaction suites green (byte-identical behavior). |
| ControlsRenderingCheck | PASS | render output byte-identical (R2 INV-1 preserved). |
| GeneratedGuidanceCheck | PASS | generated guidance untouched. |
| TemplateDrift | PASS | template untouched. |
| GeneratedProductCheck | PASS (or non-authoritative environment-failure locally) | nothing ships into generated products; if it reports `environment-failure` locally it is diagnostic-only and re-confirmed by a healthy broad pass in CI (see `generated-validation.md`). |

## Focused (authoritative) reruns

- `dotnet run --project tests/Controls.Tests -c Debug -- --filter-test-list "Feature101"` →
  `12 passed, 0 failed, 0 errored` (the drift-report units, the load-bearing probe gate, the
  non-layout-exclusion, and the FR-004 category-honoring units).
- The unchanged R2 preservation suites (`Feature097IncrementalTests`, `Feature097WiringTests`) — see
  `r2-preservation.md`.
