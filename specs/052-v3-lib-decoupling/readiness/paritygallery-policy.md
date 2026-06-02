# ParityGallery policy (ADR 0010)

command: inspection of `samples/ParityGallery/ParityGallery.fsproj`.
artifact path: this file.
failure class: SamplePolicyResidue.
next action: full relocation/retirement of the scene-output sample is deferred to Stage 5.

## Decision: keep on split packages (already monolith-free)

`samples/ParityGallery` was repointed onto `FS.Skia.UI.Scene` + `FS.Skia.UI.SkiaViewer` in Stage 1 and
references **no** monolith package or project. It builds clean on the split packages and still
demonstrates a supported viewer capability.

Per ADR 0010, the decision is to **keep** it as-is for this feature:

- It is already monolith-free, so it does not block `src/Lib` decoupling.
- Fully retiring/relocating it (and the Scene-only scene-output oracle in `tests/Parity.Tests`) touches
  the governance scanning lists that reference `tests/Parity.Tests` (`GeneratedProduct.fs`,
  `Front/Helpers.fs`, `Governance.Tests`) — that cleanup belongs with the Stage 5 monolith decommission
  cluster, not here.

No sample references the `FS.Skia.UI` monolith after this feature (FR-007).
