# Visual Evidence Honesty — Feature 089

**N/A for runtime.** This feature touches no Skia/Vulkan/host render path and
produces **no visual artifact** (no screenshot, no render-target PNG, no preview
image). There is therefore no visual-evidence honesty claim to make: nothing is
rendered, so nothing could be a metadata-only or wrong-render-path substitution.

The window-visibility evidence class was triggered only because the task text
names `real-image-evidence.md`; that record (and the sibling
`interactive-visible-window.md` / `window-state-diagnostics.md` / etc.) honestly
declare `not-applicable` / `mode=render-only` with no window and no image claim.

The one *honesty discipline this feature adds* is the inverse: the new
`speckit-implement` interactive-UI run-and-use gate (VERIFY-IMPL-1) requires
future interactive-UI features to confirm their captured evidence exercised the
**production render path** rather than a bespoke parallel scene — so a truthful
screenshot of the *wrong* path can no longer count as proof.
