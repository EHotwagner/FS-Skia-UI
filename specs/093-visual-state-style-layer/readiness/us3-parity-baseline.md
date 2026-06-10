# US3 — one declarative resolver replaces procedural per-kind styling (SC-003)

The migrated kinds (`Button` box+label, `CheckBox` rich-geometry) route their
paint through `Style.resolve` instead of inline `theme.*` color branching. For
the default (no-class) case the resolver output is **structurally-`Scene`-equal**
to the prior procedural output.

## Evidence (real, in-repo)

- **Captured baselines (T020)** — `readiness/parity/*.scene.txt`: the frozen
  pre-refactor procedural geometry for `button`/`check-box`/`check-box-checked`
  in `light` and `dark` (a frozen-literal oracle, the same technique
  `DesignTokenParityTests` uses).
- **Parity test (T021)** — `tests/Controls.Tests/Feature093ParityTests.fs`:
  `ControlInternals.faithfulContent` for the migrated kinds with no class is
  **byte-for-byte equal** to the frozen procedural geometry, both themes,
  checked and unchecked; deterministic across calls.
- **Base fidelity** — `Style.resolve theme base [] Normal = base` exactly
  (resolver test G4), which is why the migrated no-class render is byte-identical.
- **Inspection (SC-003 clause)** — `buttonGeom`/`checkboxGeom` in
  `src/Controls/Control.fs` now compute a `ResolvedStyle` base and read back
  `style.Fill`/`style.Foreground`/`style.Stroke`; **no per-kind inline
  visual-state color branch remains** for them (the `if primary`/`if on`
  branches now only select geometry, not colors).

## Result

PASS — migrated kinds' no-class output is byte-identical to the procedural
baseline; styling flows through the single resolver.
