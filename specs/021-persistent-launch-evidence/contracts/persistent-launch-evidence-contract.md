# Contract: Persistent Launch Evidence

## Purpose

Define the public viewer and generated readiness contract for proving a real
persistent graphical launch.

## Proposed Public Surface

The implementation must review and extend `src/SkiaViewer/SkiaViewer.fsi`
before `.fs` implementation. Exact names may change during `.fsi` design, but
the public surface must represent:

- A persistent evidence request with command, timeout, evidence path, input
  probe, and controlled-close policy.
- A persistent evidence artifact with the required audit fields.
- A viewer launch outcome that records first-frame, viewer-native window facts,
  input-dispatch status, close reason, blocked stage, classification, category,
  and message.
- An effect or interpreter operation for writing the artifact.

## Required Artifact Fields

Every produced artifact must include:

```text
status
mode
command
window-opened
input-dispatch
exit-path
blocked-stage
classification
category
message
```

Passing supported-host artifacts must also include `first-frame-presented=true`
or an equivalent viewer-owned render fact.

## Accepted Supported-Host Pass

```text
status=ok
mode=interactive-window
window-opened=true
first-frame-presented=true
input-dispatch=verified|not-verified|not-required
exit-path=true
blocked-stage=none
classification=ok
```

`input-dispatch=not-verified` is acceptable only when recorded explicitly.

## Rejected Pass Claims

- A timeout-killed process without controlled close.
- Layout metadata or deterministic hashes presented as visible-window proof.
- External title search success without viewer first-frame/window facts.
- Synthetic fixture output.
- Missing required fields.

## State Workflow Contract

The viewer workflow must preserve a pure update boundary:

- `init` returns model plus startup effects.
- `update` records messages and emits effects.
- Window opening, rendering, input dispatch, close request, and artifact writes
  are effects interpreted at the edge.

Generated gameplay reducers must remain pure and must not perform viewer or
filesystem work.
