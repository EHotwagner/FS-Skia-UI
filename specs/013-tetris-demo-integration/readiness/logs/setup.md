# Setup Readiness Notes

## Artifact Consistency

Reviewed `spec.md`, `plan.md`, `data-model.md`, `quickstart.md`, and the
contracts under `contracts/`. The shared scope is Tetris demo integration
support through viewer input, bounded viewer smoke, diagnostics, deterministic
scene-level evidence, generated template validation, local package guidance,
and generated consumer validation.

No reviewed artifact requires Tetris-specific game-rule changes. The explicit
unsupported scope excludes changing game-specific Tetris rules.

## Tier And Impact

- Classification: Tier 1 contracted public/API, generated-template, build,
  testing, and readiness change.
- Affected packages/modules: `src/KeyboardInput/`, `src/SkiaViewer/`,
  `src/Scene/`, `src/Testing/`, generated templates, tests, build scripts,
  guidance docs, and readiness artifacts.
- Public `.fsi` impact: normalized viewer input, diagnostics, bounded smoke,
  scene evidence, and generated/app-host-facing contracts must be governed by
  signatures and surface baselines.
- Generated template impact: generated graphical apps must expose
  viewer-key-driven user flows for start, options, interaction, pause/back, and
  restart/exit where those screens exist.
- Command-surface impact: `Verify`, `Ci`, `PackLocal`,
  `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` may change.
  `DependencyReport` changes only if package inventory reporting needs it.
- Package identity stability: package identities remain stable unless a
  separate planned package change says otherwise.

## Synthetic Evidence Policy

Synthetic fixtures may cover forced pre-frame failures, unsupported host
classification, stale package feeds, scanner inputs, and deterministic
non-window scenes. Final readiness still needs real public-surface or
generated-product evidence where the host supports it, and unsupported-host
outcomes must be explicit diagnostics.

No missing prerequisite artifacts were found for foundation work.
