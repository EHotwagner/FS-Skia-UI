# Feature 112 tier / package / obligations (T003)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls (ControlRuntime internal seam) + FS.Skia.UI.Controls.Elmish (live renderRetained seam)
public-api-impact=internal RuntimeStampResult + applyRuntimeVisualStateTargeted + runtimeStampFor (ControlRuntime.fsi); no public signature change; RuntimeStateTouchedNodeCount internal
mvu-applicability=N/A (no Model/Msg/Effect/init/update/interpreter change — dispatch outcomes byte-identical, FR-008; only the per-frame visual-state stamp mechanism changes)
interactive-ui-gate=N/A (delivers an internal stamp optimization observable via the internal count + the preserved live render path, not a new interactive surface)
evidence-obligations=targeted-vs-oracle scene parity (Feature112TargetedStampParityTests), touched-node count (Feature112TouchedCountTests), precedence (Feature112PrecedenceTests), route selection (runtimeStampFor), per-package Controls surface baseline, XML-doc on the new internal seam, at-rest byte-identity via the Scene-parity suite under Dev
