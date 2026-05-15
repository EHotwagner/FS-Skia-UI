# Evidence Policy

V1 evidence is produced by the canonical FAKE targets. Current package surface
baselines live at a stable root path, while feature-specific evidence remains
under the active feature readiness directory.

| Artifact Class | Stable Path |
|----------------|-------------|
| Package surface baselines | `readiness/surface-baselines/*.txt` |
| Build/test/package logs | `specs/007-v2-template-packaging/readiness/logs/*.txt` |
| Public contract FSI transcripts | `specs/007-v2-template-packaging/readiness/fsi/*.txt` |
| Sample smoke output | `specs/007-v2-template-packaging/readiness/sample-smoke/*.txt` |
| Template validation output | `specs/007-v2-template-packaging/readiness/template/**` |
| Dependency governance output | `specs/007-v2-template-packaging/readiness/dependencies.md` |
| Generated guidance output | `specs/007-v2-template-packaging/readiness/generated-guidance.md` |
| Template drift output | `specs/007-v2-template-packaging/readiness/template-drift.md` |
| Task graph output | `specs/007-v2-template-packaging/readiness/task-graph.json` and `.md` |
| Evidence audit output | `specs/007-v2-template-packaging/readiness/logs/evidence-audit.txt` and `diff-scan-hits.json` |
| Local packages | `~/.local/share/nuget-local/*.nupkg` |

Historical feature readiness folders remain repository evidence. They are not
the source of truth for current package baselines and should not be patched to
make current package checks pass.

## Required Targets

`Dev`, `Verify`, `Ci`, `PackLocal`, `RefreshSurfaceBaselines`,
`PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`,
`TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
`EvidenceGraph`, and `EvidenceAudit` are the evidence-producing targets.
`Verify` fails when any required V1 or V2 artifact class is missing.

## Synthetic Evidence

Tasks marked `[S]` must disclose the synthetic reason in code, tests, and the
Synthetic-Evidence Inventory in `tasks.md`. This v1 slice is expected to use
real process and filesystem evidence. Any future synthetic fixture must name
the real-evidence path before it can be accepted.

## Roadmap Boundary

Full visual evidence, release validation, an external template repository split,
and distribution automation are roadmap extensions. Future phases may add
targets for those classes, but V2 validation remains non-visual.
