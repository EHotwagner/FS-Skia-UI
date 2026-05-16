# Evidence Policy

Repository evidence is produced by the canonical FAKE targets. Current package
surface baselines live at a stable root path, while feature-specific evidence
remains under the active feature readiness directory.

| Artifact Class | Stable Path |
|----------------|-------------|
| Package surface baselines | `readiness/surface-baselines/*.txt` |
| Build/test/package logs | Active feature `readiness/logs/*.txt` |
| Public contract FSI transcripts | Active feature `readiness/fsi/*.txt` |
| Sample smoke output | Active feature `readiness/sample-smoke/*.txt` |
| Template validation output | Active feature `readiness/template/**` |
| Capability catalog output | Active feature `readiness/capability-catalog.md` |
| Selected skill output | Active feature `readiness/selected-skills.md` |
| Generated product matrix output | Active feature `readiness/generated-file-lists/**` and `readiness/generated-product-verify/**` |
| Dependency governance output | Active feature `readiness/dependencies.md` |
| Generated guidance output | Active feature `readiness/generated-guidance.md` |
| Template drift output | Active feature `readiness/template-drift.md` |
| Task graph output | Active feature `readiness/task-graph.json` and `.md` |
| Evidence audit output | Active feature `readiness/logs/evidence-audit.txt` and `diff-scan-hits.json` |
| Local packages | `~/.local/share/nuget-local/*.nupkg` |

Historical feature readiness folders remain repository evidence. They are not
the source of truth for current package baselines and should not be patched to
make current package checks pass.

## Required Targets

`Dev`, `Verify`, `Ci`, `PackLocal`, `RefreshSurfaceBaselines`,
`PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`,
`TemplateCheck`, `CapabilityCheck`, `SkillCheck`, `GeneratedProductCheck`,
`DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`,
and `EvidenceAudit` are the evidence-producing targets. `Verify` fails when
any required package, template, generated-product, guidance, drift,
dependency, graph, or audit artifact class is missing.

## Synthetic Evidence

Tasks marked `[S]` must disclose the synthetic reason in code, tests, and the
Synthetic-Evidence Inventory in `tasks.md`. Synthetic native acquisition
fixtures must name the real-evidence path before they can be accepted, and the
task inventory must identify any task whose pass depends directly on symbolic
handles or canned failures.

## Roadmap Boundary

Full visual evidence, release validation, an external template repository split,
and distribution automation are roadmap extensions. Future phases may add
targets for those classes, but current validation remains non-visual.
