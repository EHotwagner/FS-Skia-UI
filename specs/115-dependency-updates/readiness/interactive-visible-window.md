# interactive-visible-window — applicability (feature 115, T003)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 115 ships no new persistent/graphical entry point — it is a dependency-version + governance-asset
maintenance change (pin edits in `Directory.Packages.props`, a `speckit_version` recorded-version edit, and
held-bump adopt/defer experiments). It is proven through deterministic, headless evidence (the standing
Expecto/FsCheck suites + FAKE gates green on the bumped pins, zero surface/golden/generated-product diff).
A live Vulkan window is NOT required; the existing `runInteractiveApp` window-launch contract is unchanged
(its signature is untouched and no source is edited).
