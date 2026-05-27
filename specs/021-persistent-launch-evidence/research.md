# Research: Persistent Launch Evidence

## Decision: Viewer-Owned Persistent Evidence Is Authoritative

Persistent-launch readiness will prefer facts produced by SkiaViewer itself:
window creation, first-frame presentation, input dispatch status, close reason,
and artifact write status. External tools such as `wmctrl` and `xdotool` can be
recorded as supplemental observation attempts, but they cannot be the sole
authority for headless-only classification when viewer or user-visible facts
show a real desktop launch.

**Rationale**: The mailbox report showed a visible app window while external
title search failed. EvidenceAudit needs structured fields, but title search is
not reliable enough across Wayland, XWayland, compositor, backend, and window
title/class behavior.

**Alternatives considered**: Keep relying on shell window tools; accept manual
observation without structured fields; treat any observation failure as
unsupported host. These alternatives either miss visible windows or weaken audit
repeatability.

## Decision: Evidence Mode Is Separate From Default Launch

Generated apps will keep their default interactive launch persistent and
user-driven. The readiness workflow will invoke an explicit evidence mode that
opens the real viewer, waits for first frame, optionally dispatches input,
requests a controlled evidence close, and writes the artifact.

**Rationale**: A real game should not self-close during normal use. A readiness
operation needs bounded behavior and structured output.

**Alternatives considered**: Make normal launch return quickly; use `timeout`
to kill the process; require users to close the app manually during audit. These
break normal behavior or fail to prove `exit-path=true`.

## Decision: Blocked Stages Are Fine-Grained

Persistent-launch failures will classify the blocked stage as one of:
desktop prerequisites, process launch, window creation, first frame/render,
window observation or capture, input verification, controlled exit, artifact
write, or unknown. Observation/capture failure is distinct from unsupported
desktop prerequisites.

**Rationale**: The reported failure was an evidence-capture gap, not gameplay or
desktop incapability. Fine-grained stages prevent misleading diagnostics.

**Alternatives considered**: Reuse broad unsupported-host categories or existing
bounded-run stages only. Those categories are too coarse for persistent-window
readiness.

## Decision: Input Dispatch May Be Explicitly Not Verified

A supported-host passing artifact may record input dispatch as verified or not
verified, but the field must be present and honest. Lack of input verification
is a limitation, not a hidden pass.

**Rationale**: FR-003 allows explicit non-verification. Some hosts may expose
window/first-frame facts before reliable input injection exists.

**Alternatives considered**: Require input verification for every pass; omit the
field when unavailable. The first over-constrains launch evidence; the second
violates the audit contract.

## Decision: Benign Host Warnings Are Non-Fatal Context

Known messages such as missing optional GTK modules are recorded as benign
environment warnings only when launch, first-frame/render, and exit facts pass.
Warnings paired with concrete launch, rendering, layout, or package failure
remain fatal under their real class.

**Rationale**: The app can work despite those warnings. Suppressing them would
hide useful host context, while treating them as fatal would block working
desktop launches.

**Alternatives considered**: Ignore all host warnings; fail on any warning;
classify warnings before launch facts are known. Each option loses either
signal or correctness.

## Decision: Generated Guidance Uses Qualified App-Owned Names

Generated docs and tests use `Product.Program.view` for the scene,
`Product.Program.generatedHost` for the viewer host, and
`Product.Program.update` for reducer examples when framework capability
namespaces are in scope.

**Rationale**: The mailbox report showed unqualified `update` resolving to a
framework capability reducer. Qualified names avoid collisions in generated
samples.

**Alternatives considered**: Rename framework reducers or rely on open-order.
Those are broader or fragile changes.

## Decision: Required Readiness Files Are Task-Visible

Task generation must include the required readiness files before the final
EvidenceAudit task: persistent launch evidence, observation diagnostics, host
warning classification, generated guidance, and audit summary.

**Rationale**: Prior audit failures occurred because required readiness contract
files were discovered only at the final gate.

**Alternatives considered**: Let EvidenceAudit be the first discovery point.
That delays feedback and makes implementation status harder to interpret.
