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
| Process-health preflight | Active feature `readiness/process-health.md` |
| Bootstrap runner validation | Active feature `readiness/bootstrap-runner.md` |
| Verification verdicts | Active feature `readiness/verification-verdicts.md` |
| Focused gate summaries | Active feature `readiness/focused-gates.md` |
| Governance scanner summaries | Active feature `readiness/governance-scanners.md` |
| Stale boundary scan | Active feature `readiness/stale-boundary-scan.md` |
| Generated product validation summary | Active feature `readiness/generated-product-validation.md` |
| Task graph output | Active feature `readiness/task-graph.json` and `.md` |
| Evidence audit output | Active feature `readiness/logs/evidence-audit.txt` and `diff-scan-hits.json` |
| Local packages | `~/.local/share/nuget-local/*.nupkg` |

Historical feature readiness folders remain repository evidence. They are not
the source of truth for current package baselines and should not be patched to
make current package checks pass.

## Required Targets

`Dev`, `VerifyPreflight`, `CiPreflight`, `Verify`, `Ci`, `PackLocal`,
`RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `FsiTranscripts`,
`SampleContractSmoke`, `TemplateCheck`, `CapabilityCheck`, `SkillCheck`,
`GeneratedProductCheck`, `DependencyReport`, `GeneratedGuidanceCheck`,
`TemplateDrift`, `StaleBoundaryScan`, `EvidenceGraph`, `EvidenceAudit`, and
`FinalReadiness` are the evidence-producing targets. `Verify` fails when any
required package, template, generated-product, guidance, drift, dependency,
graph, or audit artifact class is missing. Broad aggregate preflight and
bootstrap failures are recorded as `environment-failure`, not product evidence.

## Broad Verdicts

Broad verification writes a concise verdict with the category `success`,
`product-failure`, `environment-failure`, or `degraded`. Environment failures
name the failing stage, health or bootstrap diagnostics, affected log/report
paths, and the recommended rerun environment. After an aggregate
`environment-failure`, final readiness remains blocked until a later healthy
`Verify` or `Ci` pass is recorded.

## Focused Gates And Scanners

Focused gates are diagnostic evidence and remain directly invocable even when a
broad local runner is degraded. They report direct prerequisites, log paths,
readiness paths, timestamps, and stale build/restore remediation. Governance
scanner evidence must name rule ids, file paths, generated profiles, package or
project references, capability ids, source/test markers, stale terms, and
remediation hints.

Focused passing evidence can help diagnose a broad failure, but it does not
replace required broad `Verify`/`Ci` evidence for final readiness after an
environment failure.

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
