# Contract: Evidence and Governance

## Persistent Graphical Launch Artifact

Required artifact format:

```text
status=ok|unsupported|failed
mode=persistent-window
command=<default executable command>
window-opened=true|false
input-dispatch=true|false|not-applicable
exit-path=true|false
blocked-stage=<stage-or-empty>
classification=UnsupportedEnvironment|ProductDefect|empty
category=<diagnostic-category-or-empty>
message=<reviewer-facing message>
```

Completion requires at least one supported-host artifact with:

```text
status=ok
mode=persistent-window
window-opened=true
exit-path=true
```

Keyboard-capable profiles also require:

```text
input-dispatch=true
```

## Bounded Evidence Separation

The following are valid evidence helpers but do not satisfy persistent graphical readiness:

- `Viewer.runBounded`
- `Viewer.runUntilFirstFrame`
- `Viewer.runForFrames`
- `--bounded-smoke`
- `--bounded-smoke-frame-diagnostics`
- `--scene-evidence`
- deterministic scene metadata
- unsupported-host diagnostics without supported-host launch evidence

## Governance Gates

`GeneratedGuidanceCheck` must fail when a viewer-backed graphical app:

- defaults to printing metadata or counting controls
- exits without persistent launch attempt
- exposes only bounded smoke or scene evidence
- lacks `Viewer.runApp` or equivalent persistent host invocation
- lacks keyboard dispatch for keyboard-capable profiles

`EvidenceAudit` must fail interactive graphical readiness when:

- persistent launch evidence is missing
- bounded-only evidence is substituted
- unsupported-host diagnostics are the only launch evidence
- required evidence fields are absent or ambiguous

`GeneratedProductCheck` must run and record explicit generated consumer evidence for semantic tests, bounded evidence, scene evidence, persistent launch source/wiring, and supported-host launch artifact collection when a supported host is available.
