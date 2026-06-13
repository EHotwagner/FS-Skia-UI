# Generated-guidance validation (feature 118)

`GeneratedGuidanceCheck` covers the generated product's selected-skill guidance and command
references. Feature 118 changes no selected Controls guidance, no generated local skills, no
validation logs, and no placeholder/excluded-history scans — it adds an additive, default-bearing
`ViewerOptions.PresentMode` field consumed by the SkiaViewer host. The generated product's
viewer guidance (`fs-skia-skiaviewer` / `fs-skia-project`) is unchanged; the only generated
source touched is the `ViewerOptions` construction sites in
`template/base/src/Product/EvidenceCommands.fs`, which gain `PresentMode =
ViewerPresentMode.OffscreenReadback` (default, byte-identical). The skill source
`src/SkiaViewer/skill/SKILL.md` is unchanged.

Run `./fake.sh build -t GeneratedGuidanceCheck` (in the routed order) to confirm generated
guidance currency; expected verdict: pass (no guidance drift introduced by this feature).
