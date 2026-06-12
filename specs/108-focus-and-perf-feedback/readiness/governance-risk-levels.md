# Governance risk levels (feature 108)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls, FS.Skia.UI.Controls.Elmish, FS.Skia.UI.KeyboardInput, FS.Skia.UI.SkillSupport
public-api-impact=yes
mvu-applicability=yes (host-loop stories reuse the runInteractiveApp / RetainedRender seam; update stays pure)
route-tier=agent-ready (controls-public-surface, maintainer-verify)

## Risk classification

- small (framework-internal `.fs` only → inner-loop `Dev`): N/A — this feature moves public `.fsi`.
- medium (test/readiness/doc additions): the Feature108 test suites, the readiness set, and the two
  template doc additions.
- broad (public `.fsi` surface move + new `Theming`/`EvidenceTour` modules + new `InteractiveAppHost`
  fields): THIS feature. Broad validation is required → full controls-public-surface route.

## Authoritative gate list (Route, run sequentially)

Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck, GeneratedProductCheck,
ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck,
ControlsDocCoverageCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck,
SkillContractPathCheck, TemplateDrift, EvidenceGraph, EvidenceAudit.

## Evidence obligations

Structural-Scene focus-ring diff (US1), byte-stable `FrameMetrics` golden (US2/US3), pointer-coalescing
proof (US4), `Control.map`/tri-state/modifier proofs (US5), WCAG contrast reference (US6), host-seam +
interactive-readiness docs (US7), recaptured aggregate + per-package surface baselines, and
EvidenceGraph/EvidenceAudit PASS (0 synthetic). No synthetic evidence is planned (every obligation has
a real path).

## Required evidence per risk level

- required evidence (broad / public `.fsi`): recaptured aggregate + per-package surface baselines
  (`PackageSurfaceCheck` / `PerPackageSurfaceDiff` green over the regenerated baselines) and the new
  public members' `///` docs (`ControlsDocCoverageCheck`).
- required evidence (US1–US6 behaviour): the structural-Scene focus-ring diff, the byte-stable
  `FrameMetrics` golden, the coalescing proof, the `Control.map`/tri-state/modifier proofs, and the
  WCAG contrast reference — each enforced by a green Feature108 test suite named on the proof file.
- required evidence (US7 docs): the `scaffold-map.md` host-seam note + `interactive-readiness.md`,
  reconciled against `ControlsElmish.fsi` / the `fs-skia-controls-host` skill.
- required evidence (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
