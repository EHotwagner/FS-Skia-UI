# Normalized Viewer Input Readiness

## Scope

Readiness evidence for stable viewer key values, key-down/key-up conversion,
alternate raw-name handling, unknown-key preservation, generated template input
flows, and packed-library or FSI public-surface validation.

## Setup Notes

- Tier: Tier 1 contracted public/API and generated-template change.
- Affected areas: `src/KeyboardInput/`, `src/SkiaViewer/`, generated
  graphical templates, generated tests, and public surface baselines.
- Public contract impact: `.fsi` signatures must be drafted before `.fs`
  implementation for normalized viewer input.
- Generated template impact: generated app code must consume normalized viewer
  input instead of backend-specific raw key strings.
- Package identity constraint: package identities remain stable.

## Evidence

- Focused package tests:
  `readiness/logs/t016-normalized-input-tests.txt` passed arrows, enter,
  space, escape, backspace, letters, digits, function keys, alternate raw
  names, unknown raw keys, key-down events, and key-up events.
- Public FSI transcript:
  `readiness/logs/t023-normalized-input-fsi.txt` exercised
  `ViewerKeyboard.normalize` and `ViewerKeyboard.normalizeEvent` through the
  compiled public package assembly.
- Generated template validation:
  `readiness/logs/t020-generated-template-tests-after-pack.txt` passed
  viewer-key start/options/interaction/pause/restart paths against local
  packages after `FS.Skia.UI.KeyboardInput` was repacked.

## Requirement Mapping

- FR-001 through FR-003: covered by `ViewerKey`, alternate raw-name
  normalization, unknown preservation, and down/up event conversion tests.
- FR-018: optional app-host/public viewer input contracts are present in
  `src/SkiaViewer/SkiaViewer.fsi`.
- FR-019: generated input diagnostics name input value, raw key, direction,
  screen, expected transition, and flow.
- SC-001 and SC-002: generated template and package tests prove initial-screen
  start through viewer key events and normalized input coverage.
