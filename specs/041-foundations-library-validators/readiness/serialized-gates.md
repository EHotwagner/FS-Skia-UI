# Serialized FAKE gate results (T021 — small→medium governance risk)

FAKE-backed targets share `.fake` state and were run **strictly sequentially**, never
concurrent (CLAUDE.md / AGENTS.md). Aggregate results are recorded as **non-authoritative**;
the focused per-gate rerun is authoritative.

| Order | Gate | Result | Notes |
|-------|------|--------|-------|
| 1 | `Dev` | **Ok** | runs Governance.Tests incl. the parity + typed-finding suites; focused executable reports 295/295 |
| 2 | `GeneratedGuidanceCheck` | **Ok** | unchanged generated-guidance scan |
| 3 | `TemplateCheck` | **Ok** (exit 0) | `readiness/template/verdict.md` = PASS (source/package app, headless-scene, governed, sample-pack) |
| 4 | `GeneratedProductCheck` | **Ok (exit 0)** | `generated-product-validation.md` Category=Completed; V3 source/package products generated, scanned, and consumer-validated |
| 5 | `EvidenceGraph` | **Ok** | graph acyclic, no dangling refs, no `[S*]`; skillist + mirrors valid |
| 6 | `EvidenceAudit` | **PASS** | verdict=PASS; 0 unaccepted synthetic, 0 auto-synthetic, 0 diff-scan hits, 0 readiness-contract hits |

Documented environment flakes (039 baseline) remain non-authoritative if they recur in an
aggregate run; rerun the affected gate in focused isolation (authoritative) before product
debugging: `SkiaViewer.Tests` headless `libdecor-gtk` crash, `FsiTranscripts` env flake.

Authoritative command per gate: `./fake.sh build -t <Gate>` run in isolation. Failure class:
`governance / serialized-gate`. Next action on a race-like failure: rerun that single gate in
focused isolation and treat that result as authoritative.
