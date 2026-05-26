# Contract: Readiness Evidence

## Required Files

The feature is not implementation-ready until task generation includes these readiness files as explicit obligations:

- `readiness/interactive-lifecycle.md`
- `readiness/evidence-launch-mode.md`
- `readiness/container-session-diagnostics.md`
- `readiness/package-resolution.md`
- `readiness/generated-verify.md`
- `readiness/game-visual-evidence.md`
- `readiness/task-workflow-guidance.md`
- `readiness/evidence-audit.md`

## Interactive Lifecycle Evidence

Must include:

```text
mode=interactive-window
window-opened=true
first-frame-presented=true
self-closed-for-evidence=false
user-close-observed=true|false
input-dispatch=verified|not-verified|not-required
```

Acceptance requires proof that first-frame presentation alone did not close the session. In automated tests this may be a disclosed fake window-loop regression; supported-host runs must provide real launch evidence when available.

## Evidence Launch Mode

Must include:

```text
mode=persistent-evidence
first-frame-presented=true|false
self-closed-for-evidence=true|false
input-dispatch=verified|not-verified|not-required
```

Evidence mode may close itself but must never claim to be an ongoing interactive session.

## Container Session Diagnostics

Must include:

```text
diagnostic-class=environment-session
xdg-runtime-dir=<path-or-empty>
runtime-dir-exists=true|false
runtime-dir-owner-suitable=true|false
runtime-dir-permissions-suitable=true|false
display=<value-or-empty>
display-socket-exists=true|false
session-bus=<value-or-empty>
fallback-runtime-dir=<path-or-empty>
fallback-full-desktop-session=false
```

## Package Resolution Evidence

Must include exact requested/resolved package pairs and all configured sources. Any `NU1603` or mismatch is a failure.

## Generated Verify Evidence

Must include command evidence for generated restore, generated test execution when present, bounded evidence run, visual evidence run or unsupported-host diagnostic, and final verification verdict.

## Visual Game Evidence

Preferred:

```text
evidence-kind=screenshot
path=<image-path>
board-readable=true
input-or-progress-observed=true
```

Fallback:

```text
evidence-kind=pixel-readback
path=<artifact-path>
fallback-reason=screenshot-unavailable
board-readable=true
input-or-progress-observed=true
```

Unsupported host:

```text
evidence-kind=unsupported-host
unsupported-reason=<specific reason>
```

Text-only scene metadata is not visual game evidence.

## Task Workflow Guidance

Must define an implementation batch record with task ids, shared evidence, graph before/after paths, and skill-loading notes. Must define a red-green log with failing assertion, command, change reference, and final passing command for related test clusters.

## Audit Expectations

`EvidenceAudit` must fail when required readiness files are missing, when bounded evidence is substituted for interactive evidence, when visual evidence is only text metadata on a supported host, when package mismatch is unresolved, or when generated tests exist but did not run.
