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
| Generated consumer validation details | Active feature `readiness/generated-consumer-validation/**` |
| Task graph output | Active feature `readiness/task-graph.json` and `.md` |
| Evidence audit output | Active feature `readiness/logs/evidence-audit.txt` and `diff-scan-hits.json` |
| Local packages | `~/.local/share/nuget-local/*.nupkg` |

Feature `013-tetris-demo-integration` additionally requires these active
feature readiness files before final review:

- `readiness/normalized-viewer-input.md`
- `readiness/bounded-viewer-smoke.md`
- `readiness/diagnostics.md`
- `readiness/headless-scene-evidence.md`
- `readiness/generated-template-input-flows.md`
- `readiness/local-consumer-packages.md`
- `readiness/generated-consumer-validation.md`
- `readiness/evidence-graph.md`
- `readiness/evidence-audit.md`

Persistent viewer readiness must come from a distinct graphical launch artifact
from the default executable path. The artifact records `status`,
`mode=persistent-window`, `command`, `window-opened`, `input-dispatch`,
`exit-path`, `blocked-stage`, `classification`, `category`, and `message`.

Bounded viewer runs, first-frame evidence, frame-count evidence, deterministic
scene metadata, and unsupported-host diagnostics are CI and diagnostic helpers.
They do not replace supported-host persistent graphical launch evidence.
They must be cited as helper evidence only; reviewers should treat a package
that contains only bounded or unsupported-host artifacts as incomplete for
interactive graphical readiness.

Generated graphical consumer validation starts from `PackLocal` output, restores
the generated product from the local feed, runs generated semantic tests, then
runs persistent source/wiring checks, bounded smoke, and deterministic scene
evidence. Unsupported-host diagnostics must remain separate from supported-host
persistent launch evidence.

Screenshot evidence is a distinct proof kind. A report with
`evidence-kind=screenshot` is valid screenshot proof only when it records
live viewer-window capture after first-frame presentation, `proves-screenshot=true`,
a readiness-local PNG artifact, positive decoded dimensions, and non-blank
pixel validation. The record must also include command, app/sample identity,
host facts, capture mode, blocked stage, classification, category, message, and
timestamp so reviewers can trace the artifact without a local rerun.

Unsupported hosts may record fallback diagnostics such as
`deterministic-scene-evidence`, but deterministic scene evidence, layout facts,
launch logs, metadata hashes, pixel-readback diagnostics, manual descriptions,
synthetic placeholders, blank images, and unreadable files must not claim
screenshot proof.

Fallback and diagnostic-only evidence must not claim screenshot proof.
semantic scene facts such as lander, terrain, landing pad, and HUD metrics may
be reported as deterministic-scene-evidence, but deterministic scene metadata
does not prove semantic object presence in a live screenshot. pixel-readback fallback records must include `fallback-reason` and `proves-screenshot=false`
unless live viewer-window screenshot capture succeeded.

Viewer diagnostics evidence should record both startup-focused and
frame-focused paths. Startup-focused runs keep frame-loop diagnostics disabled
or sampled to zero; frame-focused runs must enable the frame category
explicitly and cap repeated frame messages. In-process diagnostic sink evidence
is preferred over process stderr scraping.

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

Synthetic error-handling tasks are the narrow exception. A malformed-input or
explicit error-path task may be annotated `[SEH]` and labeled
`synthetic-error-handling-approved` only during design, planning,
clarification, or task generation. It still remains `[S]`; reviewers should
look for the inventory row fields `Design source`, `Synthetic input class`,
`Expected error behavior`, and `Acceptance status=accepted-seh`. EvidenceAudit
reports `accepted-seh-tasks`, `unaccepted-synthetic-tasks`,
`auto-synthetic-tasks`, and `late-seh-tasks` so accepted synthetic evidence is
visible without being treated as real evidence.

Eligible `[SEH]` cases include malformed parser input, corrupt file content,
invalid command arguments, protocol violations, missing required data, hostile
payloads, and forced error-result fixtures. Convenience mocks, incomplete
integrations, unavailable product capability, missing host support, placeholder
outputs, speed-only fixtures, and ordinary in-memory substitutes remain
ordinary synthetic evidence. Implementation-time relabeling is rejected; send
newly discovered cases back to task/design review.

## Roadmap Boundary

Full visual evidence, release validation, an external template repository split,
and distribution automation are roadmap extensions. Future phases may add
targets for those classes, but current validation remains non-visual.
