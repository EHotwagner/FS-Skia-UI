# Contract: Stability-Diagnostic Report (public)

**Surface**: a new public `val` in `FS.Skia.UI.Controls` `Diagnostics.fsi` returning
`ControlDiagnostic list`. A **report/diagnostic tool**, NOT an enforced CI gate this rung
(clarified 2026-06-12) — consumers legitimately use event closures, so failing the build
on them would be too aggressive. Precedent: 101's `layoutDriftReport` (a pure report
function asserted in tests, not wired as a gate).

## Behaviour

Given **two builds of the same logical control (sub)tree** (same model → same `View`,
built twice), the report walks the two trees in parallel and returns one finding per
attribute/event that compared **unequal** despite no semantic change — the always-new
inputs that defeat reuse:

- a rebuilt `UntypedValue` (a value attribute reconstructed each frame),
- a per-frame event closure (a fresh lambda each build),
- a rebuilt list (a structurally-equal list reconstructed each frame, reference-unequal),
- an unstable key.

Each finding names the **control** (`ControlId` + `ControlKind`) and the **offending
input** (the attribute/event name).

| Two builds | Result |
|---|---|
| attribute/event-equal (stable inputs) | **empty list** — no instability findings (FR-012) |
| an injected always-new attribute/event | **one finding** naming the control + input (FR-011) |

## Why two builds of the same model

The caller supplies two builds of the *same logical tree*, so any unequal attribute/event
is by construction an always-new input, not a semantic change. (A genuinely changed model
is a different tree and outside this report's contract — the report proves *stability
across an unchanged model*, which is exactly the case memoization wants to exploit.)

## Test obligations (`tests/Controls.Tests/Feature113*`)

1. A fixture tree built twice with stable attributes/events → report returns **no**
   findings.
2. The same tree with an injected always-new attribute (or per-frame closure) → report
   **flags** that attribute/event as a reuse-breaking instability, naming control + input.
3. (Doc) the stable-props guidance page (`docs/controls/stable-props.md`) names the
   concrete reuse-breaking patterns (rebuilt `UntypedValue`, per-frame closures, rebuilt
   lists, unstable keys) and how to make them stable (FR-013/SC-005).
