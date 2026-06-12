# Feature 111 tier / package / obligations (T003)

feature-tier=tier-1-contracted
affected-package=FS.Skia.UI.Controls.Elmish (+ internal FS.Skia.UI.Controls retained surface, consumed only)
public-api-impact=new public `FrameCause` DU + `FrameMetrics` `FrameCause`/`DiffRan`/`LayoutRan`/`PaintRan`; `ViewCalled`/`FullRenderCount` narrow on model-unchanged frames
mvu-applicability=N/A (no Model/Msg/Effect/init/update/interpreter change — dispatch outcomes byte-identical, FR-008; only per-frame phase scheduling + cause/phase observability change)
evidence-obligations=cause classification (Feature111FrameCauseTests), phase record (Feature111PhaseRecordTests), view-skip byte-identity + frame-rate work (Feature111ViewSkipTests), updated honesty test (Feature109MetricsHonestyTests), regenerated corpus goldens (view-free-delta.md), surface + per-package baselines, XML-doc on the new surface
