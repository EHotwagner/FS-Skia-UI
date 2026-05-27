# Contract: Window Observation Diagnostics

## Purpose

Prevent visible-window success from being mislabeled as headless-only when
external observation or capture tools fail.

## Diagnostic Sources

Diagnostics must identify their source:

- `real-launch`: facts from an actual persistent evidence launch.
- `generic-host-probe`: desktop/session capability checks that did not open the
  generated app.
- `synthetic-fixture`: parser or error-handling fixture.

## Required Distinctions

Readiness diagnostics must distinguish:

- Missing desktop prerequisites.
- Process launch failure.
- Window creation failure.
- First-frame or render failure.
- Window observation failure.
- Capture failure.
- Input verification failure.
- Controlled-exit failure.
- Artifact write failure.

## Classification Rule

If desktop prerequisites are present and the app process remains alive, failure
to find a matching title, handle, screenshot, or capture must be classified as
observation/capture blocked unless viewer-native facts prove a stronger failure.

## Required Output

Observation diagnostics must include:

- Command.
- Host/session facts.
- Viewer-native window facts when available.
- External observation attempts and results.
- Missing facts.
- Blocked stage.
- Classification.
- Message.

## Forbidden Output

- `headless-only` or equivalent classification based solely on external window
  search failure.
- Claims that deterministic layout evidence is screenshot or visible-window
  proof.
