# Validation contract — routing + gate record

`./fake.sh build -t Route` for this change:

```
developer-class=framework-author
tier=agent-ready
gates=Dev, PackageSurfaceCheck, FsiTranscripts, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance, specify-catchall, docs-only, package-surface
```

The change touches public runtime `.fsi` surface (new `src/SkiaViewer/Host/*.fsi`, enriched
`src/Scene/Scene.fsi`, shrunken `src/Lib/Library.fsi`), consumer `.fsproj`s, and
`readiness/surface-baselines/**`; `Route` routes it to the broad **agent-ready** gate set above.
Per the project contract, only the gates `Route` prints are run (plus the explicit, non-Route-gated
`PerPackageSurfaceDiff` for the per-package delta record — Stage-0 deferral).

Gate results (run sequentially; FAKE shares `.fake` state):

| Gate | Result |
|------|--------|
| `Route` | Ok (agent-ready) |
| `Dev` | Ok (build + all semantic/parity suites) |
| `PackageSurfaceCheck` | Ok |
| `PerPackageSurfaceDiff` | Ok (zero drift) |
| `GeneratedGuidanceCheck` | Ok |
| `TemplateDrift` | Ok |
| `FsiTranscripts` | Ok |
| `EvidenceGraph` | see `evidence-graph.md` |
| `EvidenceAudit` | see `evidence-audit.md` |

Aggregate results are non-authoritative for environment flakes; the known `SkiaViewer.Tests`
headless libdecor-gtk crash is mitigated by the front-end X11 self-force (graphics-env normalization
observed in the Route run) and is re-run focused if it surfaces, with deterministic scene-output as
the authoritative parity oracle.
