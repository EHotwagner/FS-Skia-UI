# Validation contract currency

command: `./fake.sh build -t Route` + `git diff -- build/Governance/validation.contract.yml`
artifact path: this file.
failure class: ValidationContractDrift.
next action: none — the contract is unchanged.

- `Route` reports tier `agent-ready`, gates =
  `Dev, PackageSurfaceCheck, FsiTranscripts, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`
  (matched rules: `evidence-governance, specify-catchall, docs-only, package-surface`). The explicit
  `PerPackageSurfaceDiff` was run additionally.
- `build/Governance/validation.contract.yml` is **unchanged** this feature — no `Routing.fs` rule was
  added or modified. Adding the per-package Route-gating rule (the Stage-0 deferral) remains **Stage 5**;
  it touches only governance config now that `knownGates` lives in `FS.Skia.UI.Build` (feature 051).
- Governance edits this feature are confined to `build/Governance/PerPackageSurface.fs` (the new
  in-scope package) and `build/Governance/Front/Helpers.fs` (the `PackLocal` `packProjects` entry) —
  neither alters `Routing.fs` or the rendered contract, so currency vs `Routing.fs` is preserved.
