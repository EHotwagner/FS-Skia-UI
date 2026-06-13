# visual-evidence-honesty — applicability (feature 117, T002)

status=not-applicable

Feature 117 renders nothing NEW to a window. It is an internal bounded text-measure cache + a
layout-invalidated metric + three additive `FrameMetrics` fields, with no new scene, window, screenshot,
or pixel surface. There is no visual proof to make honest or dishonest — the proof is the deterministic
internal-seam tests + the `Perf.runScript` metrics + the standing Scene-parity suite under `Dev` staying
green with byte-identical at-rest output (FR-004; cache-on ≡ cache-off). No deterministic render-only
capture, no live-window screenshot, and no benign/blocking host-warning classification is produced or
claimed by this feature.
