# Validation contract — `Route` escalation and the gates run (FR-009)

`./fake.sh build -t Route` classifies this change and prints the authoritative gate list; only those
gates were run, sequentially (FAKE-backed targets share `.fake` state and are never concurrent).

## Route verdict

```
developer-class=framework-author
tier=agent-ready
gates=Dev, PackageSurfaceCheck, FsiTranscripts, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance, specify-catchall, docs-only, package-surface
```

The governance-path + monolith-`.fsi`-shrinking change **escalates** to `agent-ready` (broad
validation). `./fake.sh build -t Route --enforce` reports the escalated tier with every required
evidence artifact present.

## Gate results (each run sequentially)

| Gate | Result | Log |
|------|--------|-----|
| `Dev` | Ok — 347 tests, 347 passed | `readiness/logs/dev.log` |
| `PackageSurfaceCheck` | Ok — baseline matches reflection surface | `readiness/logs/package-surface-check.log` |
| `FsiTranscripts` | Ok | `readiness/logs/fsi-transcripts.log` |
| `GeneratedGuidanceCheck` | Ok | `readiness/logs/generated-guidance-check.log` |
| `TemplateDrift` | Ok | `readiness/logs/template-drift.log` |
| `EvidenceGraph` | Ok — `verdict=ok` | `readiness/logs/evidence-graph.log` |
| `EvidenceAudit` | PASS — 0 synthetic | `readiness/logs/evidence-audit.log` |

`validation.contract.yml` is **not** edited (SC-007); the template and every generated profile
build/restore/run exactly as before; the default `app` is byte-unchanged (SC-006). The only package
delta is `FS.Skia.UI` no longer carrying the module and `FS.Skia.UI.Build` now carrying it.
Aggregate FAKE results are non-authoritative; no race-like failure occurred.
