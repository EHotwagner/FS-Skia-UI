# Readiness contract (feature 108)

For each authoritative command, this feature records: the command, the artifact path, the failure
class, and the next action.

| Concern | Command | Artifact | Failure class | Next action |
|---|---|---|---|---|
| Route tier + gate list | `./fake.sh build -t Route [--enforce]` | [governance-risk-levels.md](./governance-risk-levels.md) | missing-evidence | author the named artifact |
| Build + tests | `./fake.sh build -t Dev` | [logs/build.txt](./logs/build.txt) | product-defect | fix code/tests, rerun |
| Aggregate + per-package surface | `./fake.sh build -t PackageSurfaceCheck` / `PerPackageSurfaceDiff` | readiness/surface-baselines, readiness/per-package-surface | surface-drift | `RefreshSurfaceBaselines`, recapture |
| FSI transcripts | `./fake.sh build -t FsiTranscripts` | [fsi/](./fsi) | product-defect | update prelude/transcript |
| Template | `./fake.sh build -t TemplateCheck` / `TemplateDrift` | readiness/template | template-drift | reconcile template manifest |
| Generated product | `./fake.sh build -t GeneratedProductCheck` | [generated-validation.md](./generated-validation.md) | environment / product-defect | classify per generated-validation |
| Generated guidance | `./fake.sh build -t GeneratedGuidanceCheck` | [generated-guidance-validation.md](./generated-guidance-validation.md) | guidance-drift | reconcile generated guidance |
| Controls doc coverage | `./fake.sh build -t ControlsDocCoverageCheck` | the edited `.fsi` `///` docs | doc-coverage | add `///` to new public members |
| Evidence graph | `./fake.sh build -t EvidenceGraph` | [task-graph.md](./task-graph.md) | graph-defect | fix tasks.deps.yml |
| Evidence audit | `./fake.sh build -t EvidenceAudit` | [evidence-audit.md](./evidence-audit.md) | synthetic / diff-scan | resolve [S]/diff-scan hit |
