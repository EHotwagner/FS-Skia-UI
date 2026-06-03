# Runtime Limitations — 059-speckit-tasks-validation-feedback

This feature is **governance + template + authoring-guidance** scope only. It
introduces **no product runtime, layout, rendering, Vulkan, or Skia change**.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux,
renders through **Vulkan**, and depends on a **SkiaSharp preview** native build.
Platforms remain **unsupported macOS/mobile/browser**, and there is
**no software-renderer fallback**. This feature changes none of that; it touches
no product runtime code.

## This feature's scope

- Compiled evidence engine: `DepsParser` gains an `owns` field; `Audit` removes
  the title-trigger matcher and validates structured `owns:` ownership; the
  FR-007 wrapper directive is emitted standalone.
- Template `build.fsx` resolves the feature from `.specify/feature.json` (with a
  `SPECKIT_FEATURE_DIR` override) and fails loud — no sample synthesiser.
- Bundled skills/presets/templates updated; `fs-skia-layout-evidence` is split
  into `fs-skia-layout-readability` + `fs-skia-evidence-mode`.
- Principle IV (Elmish/MVU) is **N/A** — governance tooling, no product state.
  The only file read is at the `build.fsx` interpreter edge.

## Environment limitation — `Verify` aggregate

The `Verify` umbrella target cannot bootstrap in this sandbox (its preflight
restores the FAKE runner as a `dotnet-fake` global tool, which is not installed
here; gates are driven via `./fake.sh`). Each maintainer-verify gate is run
**individually and sequentially**; the merge gate `EvidenceAudit` is the
authority. This is an environment constraint, not a readiness defect.
