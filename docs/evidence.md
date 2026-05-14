# Evidence Policy

V1 evidence is produced by the canonical FAKE targets. Current package surface
baselines live at a stable root path, while feature-specific evidence remains
under the active feature readiness directory.

| Artifact Class | Stable Path |
|----------------|-------------|
| Package surface baselines | `readiness/surface-baselines/*.txt` |
| Build/test/package logs | `specs/006-template-framework-governance/readiness/logs/*.txt` |
| Public contract FSI transcripts | `specs/006-template-framework-governance/readiness/fsi/*.txt` |
| Sample smoke output | `specs/006-template-framework-governance/readiness/sample-smoke/*.txt` |
| Task graph output | `specs/006-template-framework-governance/readiness/task-graph.json` and `.md` |
| Evidence audit output | `specs/006-template-framework-governance/readiness/logs/evidence-audit.txt` and `diff-scan-hits.json` |
| Local packages | `~/.local/share/nuget-local/*.nupkg` |

Historical feature readiness folders remain repository evidence. They are not
the source of truth for current package baselines and should not be patched to
make current package checks pass.

## Required Targets

`Dev`, `Verify`, `Ci`, `PackLocal`, `RefreshSurfaceBaselines`,
`PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`,
`EvidenceGraph`, and `EvidenceAudit` are the v1 evidence-producing targets.
`Verify` fails when any required v1 artifact class is missing.

## Synthetic Evidence

Tasks marked `[S]` must disclose the synthetic reason in code, tests, and the
Synthetic-Evidence Inventory in `tasks.md`. This v1 slice is expected to use
real process and filesystem evidence. Any future synthetic fixture must name
the real-evidence path before it can be accepted.

## Roadmap Boundary

Template packaging, dependency governance, generated spec/plan hardening,
layout evidence, visual evidence, package consumer smoke, and release
validation are roadmap extensions. Future phases may add targets for those
classes, but v1 `Dev`, `Verify`, and `Ci` exclude them.
