# Research: Tetris Demo Integration Improvements

## Normalized Viewer Input Surface

Decision: Add a public normalized viewer key model and conversion helpers near
the viewer/input boundary, then reuse the same mapping from generated apps and
tests.

Rationale: The Tetris integration failed in the live path because generated
app code handled domain keys while the viewer delivered raw backend key
strings. A stable `ViewerKey`-style model lets app code and tests use one
documented input vocabulary for arrows, enter, space, escape, backspace,
letters, digits, function keys, and unknown keys.

Alternatives considered: Keep using raw Silk.NET key names in app code
(rejected because it repeats backend-specific string matching); require all
apps to compose the lower-level keyboard runtime manually (rejected because the
failure was caused by discoverability and composition gaps); move all input
normalization into templates only (rejected because framework users outside
templates need the same contract).

## Bounded Real Viewer Smoke

Decision: Add bounded viewer execution helpers that stop after first-frame or
frame-count evidence and return structured success/failure records.

Rationale: Shell `timeout` plus log scanning cannot distinguish success from a
hung process cleanly. A framework-owned bounded run can report frames rendered,
elapsed time, initial output size, renderer mode, last diagnostic, blocked
stage, and unsupported-environment classification.

Alternatives considered: Continue using external timeout wrappers (rejected
because success is outside the app); add only generated app `--smoke` flags
(rejected because each generated app would still need duplicated viewer
lifecycle handling); use headless evidence as the only smoke path (rejected
because real viewer startup remains a separate requirement).

## Diagnostic Categories And Capture

Decision: Extend viewer diagnostics from a broad verbose switch to level,
category, sampling, and sink options while preserving a compatibility shortcut
for existing verbose behavior.

Rationale: Startup evidence should not be drowned by repeated per-frame
swapchain and scene logs. Tests and hosts need an in-process capture sink so
diagnostics can be asserted without process-level stderr scraping.

Alternatives considered: Keep `Verbose: bool` only (rejected because it mixes
startup and frame-loop concerns); use a third-party structured logging
dependency now (rejected because repository governance has not selected one);
write all diagnostics only to files (rejected because tests and hosts need
direct capture).

## Headless Scene Evidence

Decision: Provide deterministic scene-level visual evidence that can render or
summarize a representative scene without opening a native window, while
keeping real bounded viewer startup as separate evidence.

Rationale: CI hosts may lack a desktop session, display server, GPU, Vulkan
presentation, or runtime directory. Deterministic scene evidence gives
generated apps a supported non-window proof path and can report
unsupported-environment diagnostics when required capabilities are absent.

Alternatives considered: Require every validation host to support native
windows (rejected as unrealistic); treat scene-level evidence as equivalent to
live viewer evidence (rejected by the clarification); defer all visual
evidence to manual screenshots (rejected because readiness must be automated).

## Generated Template Input Flows

Decision: Update graphical templates and generated tests so user-reachable
screens are driven through viewer key events for start, options, primary
interaction, pause/back where present, and restart/end flows.

Rationale: Pure domain-message tests can pass while the visible app is
unresponsive. Generated projects need defaults that exercise the same input
path users exercise.

Alternatives considered: Document recommended flows only (rejected because the
failure mode is easy to reintroduce); test only gameplay controls (rejected
because start/options/restart screens are user-reachable); make Tetris-specific
template rules (rejected because the feature targets reusable generated app
patterns).

## Local Consumer Package Guidance

Decision: Add one documented local integration workflow or command that prints
the local feed path, package identities, versions, consumer configuration
snippet, restore command, and stale/missing feed diagnostics.

Rationale: Consumer package drift is outside app source but appears as build or
runtime failure. Making package identity and feed state explicit lets users
correct setup before blaming generated app code.

Alternatives considered: Leave package setup in README prose only (rejected
because stale feed detection needs command output); publish remote packages for
all validation (out of scope); copy framework source into consumers (rejected
because generated products must consume package boundaries).

## Optional Generated App Host Convenience

Decision: Provide an optional app-host convenience path only after lower-level
viewer primitives remain directly available.

Rationale: Generated apps repeatedly combine model initialization, update,
view/scene production, key mapping, ticks, diagnostics, and smoke mode. A small
host builder can make the common path hard to miswire without hiding the lower
level APIs needed by advanced users.

Alternatives considered: Force all apps through a high-level host (rejected
because low-level viewer primitives are part of the framework value); skip the
host helper entirely (rejected because generated apps will otherwise duplicate
edge plumbing); make it Tetris-specific (rejected because generated app
patterns should be reusable).
