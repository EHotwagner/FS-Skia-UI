# Generated Template Input Flows Readiness

## Scope

Readiness evidence for initial, options, main interaction, pause/back,
restart/exit, and app-host convenience flows driven by normalized viewer key
events.

## Setup Notes

- Tier: Tier 1 contracted generated-template change.
- Affected areas: `template/`, generated tests, generated quickstarts,
  template drift evidence, and optional app-host convenience contracts.
- Generated template impact: keyboard flows must be user-reachable through
  viewer events, not only domain-level messages.
- Tetris rule constraint: this feature does not change Tetris-specific game
  rules; it improves generated integration, input, validation, and evidence.

## Evidence

- Test-first generated template evidence:
  `readiness/logs/t017-t018-generated-template-tests.txt` captured the initial
  generated template failure before the updated local package API was available.
- User-reachable generated template validation:
  `readiness/logs/t020-generated-template-tests-after-pack.txt` passed the
  generated product tests after local package repack. The tests drive initial
  start, options navigation, main interaction, pause/back, end-screen restart,
  and pure MVU update flows through `ViewerKeyEvent` or normalized
  `ViewerKey` messages.
- Local package repack evidence:
  `readiness/logs/us1-keyboardinput-pack.txt` repacked the public
  `FS.Skia.UI.KeyboardInput` package used by the generated template.

## Requirement Mapping

- FR-004 through FR-006: generated template exposes and tests initial,
  options, main interaction, pause/back, and restart flows through viewer key
  events.
- FR-018: generated app input flow remains model/update based and can be
  hosted through lower-level viewer primitives.
- FR-019: template diagnostics name the generated app flow, input value, raw
  key, direction, screen, and expected transition.
- SC-001, SC-002, and SC-008: generated tests prove viewer-key driven start
  and navigation without raw backend string comparisons in transition logic.
