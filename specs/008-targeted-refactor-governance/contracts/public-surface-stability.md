# Contract: Public Surface Stability

## Scope

This contract covers the public F# surface exposed by `src/Lib/Library.fsi`, public package surface baselines, documented samples, and public records reviewed for invariant recommendations.

## Required Behavior

- `src/Lib/Library.fsi` remains source-compatible for existing consumers.
- Public modules `Colors`, `Diagnostics`, `Paint`, `Path`, `Scene`, `Parity`, and `Viewer` remain callable with the same names and signatures.
- Public records, discriminated unions, and opaque `Scene` behavior remain construction-compatible unless a separate follow-up spec authorizes a public API change.
- Package surface baselines under `readiness/surface-baselines/` remain unchanged for this feature.
- Any implementation split must not add public exports to `FS.Skia.UI`, `FS.Skia.UI.Layout`, or `FS.Skia.UI.Charts`.

## Failure Conditions

- `src/Lib/Library.fsi` changes without a separate approved public API specification.
- Surface baseline output changes for anything other than explicitly approved non-public build metadata.
- A new implementation helper file exposes unintended public modules, functions, or types.
- A Yoga fallback diagnostic or record invariant recommendation requires a new public type, field, union case, constructor, or function in this feature.

## Evidence

- `specs/008-targeted-refactor-governance/readiness/public-surface.txt`
- `specs/008-targeted-refactor-governance/readiness/semantic-tests.txt`
- Package surface test output from `tests/Package.Tests`
- Follow-up proposals in `specs/008-targeted-refactor-governance/readiness/follow-ups.md` when public API gaps are found
