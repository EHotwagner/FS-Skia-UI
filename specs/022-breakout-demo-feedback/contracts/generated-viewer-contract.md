# Contract: Generated Viewer Guidance

Generated graphical apps must use one packaged persistent viewer launch
contract consistently.

## Required Contract

- Generated source calls the selected public viewer entry point.
- Generated tests assert the same entry point name and compile against the
  packed package.
- Generated quickstart and product docs use the same name.
- `GeneratedGuidanceCheck` fails when guidance references a public name that is
  not present in the packed package consumed by a fresh generated app.
- The readiness report records the package version, selected entry point, and
  files scanned.

## Acceptance

- Fresh generated app launch guidance works without editing placeholder strings
  or comments.
- Drift between `Viewer.runApp`, `Viewer.runAppWithWindowBehavior`, or any
  future replacement fails before release.
- Deterministic render proof, persistent launch proof, and screenshot proof are
  named as separate evidence kinds.
