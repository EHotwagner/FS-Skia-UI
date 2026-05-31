# Contract: Generated Local API Reference (US2)

**Satisfies:** FR-004 · SC-002

## Shape

- A freshly generated project contains `docs/api-surface/` holding the **real
  public `.fsi` files**, copied verbatim from `src/.../*.fsi`.
- The set is selected per generated **profile**: for each capability the profile
  includes, bundle that capability's `contracts:` `.fsi` paths from
  `template/capabilities.yml`.
  - `app` → Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Controls signatures.
  - `headless-scene` → Scene (+ optional Layout/Controls/Testing).
  - `governed` → Scene, Testing (+ optional capabilities selected).
  - `sample-pack` → Scene, SkiaViewer, Elmish (+ samples).

## Rules

1. **Verbatim, mechanical derivation.** Generated at template-emit time from the
   source `.fsi`; never hand-maintained. A derived `.md` summary is permitted
   only if generated verbatim and kept in lockstep.
2. **Completeness.** Every package the generated project references has its
   signatures present locally.
3. **Reflection-free outcome.** An author determines any union case's exact field
   order (e.g. `SceneNode.Rectangle of (float*float*float*float)*Color`) from the
   bundled signatures alone.

## Failure conditions (checked by GeneratedGuidanceCheck/TemplateCheck)

- A referenced package's signatures missing from the generated tree.
- Bundled signatures drift from the source `.fsi`.
- Generated guidance points at `src/.../*.fsi` paths absent from the project
  (FR-005 overlap).

## Evidence

`readiness/generated-api-reference.md` — bundled signatures present + a union
case shape read locally with zero reflection.
