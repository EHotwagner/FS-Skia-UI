# Compatibility Impact

Feature: `021-persistent-launch-evidence`

- no new runtime package dependency was introduced.
- public API surface changed in SkiaViewer and Testing; surface baselines were
  refreshed intentionally.
- generated app default behavior remains persistent and user-driven.
- evidence mode is opt-in and self-closes only for readiness evidence.
- unsupported hosts report blocked stages and missing facts instead of using
  deterministic layout/render evidence as a persistent-window substitute.

