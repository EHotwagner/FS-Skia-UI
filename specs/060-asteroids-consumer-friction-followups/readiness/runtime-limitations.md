# Runtime Limitations — 060-asteroids-consumer-friction-followups

This feature is **governance + template + skills** scope only. It introduces **no product
runtime, layout, rendering, Vulkan, or Skia change**.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux, renders
through **Vulkan**, and depends on a **SkiaSharp preview** native build. Platforms remain
**unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**. This
feature changes none of that; it touches no product runtime code.

## This feature's scope

- No new `Model` / `Msg` / `Effect` / `Cmd` / `init` / `update` / interpreter on any product
  surface — Principle IV (Elmish/MVU) is **N/A** here (recorded in `target-metadata.md` / T004).
  The new build-side effects (`RegenerateApiSurface`, `SkillContractPathScan`,
  `TemplateUpdatePackageScan`) follow the existing compiled-front-end MVU edge: a pure
  decision in `Engine/Update.fs`, all I/O at the `Engine/Interpret.fs` edge.
- The generated `build.fsx` evidence-runner feature resolution (`resolveFeatureDir`) was
  shipped by 059 and is verified end-to-end here, not redesigned — see
  `generated-project/feature-resolution.log`.
- No persistent windowed host is launched by any task; there is no on-screen visibility check.

## Environment limitation — aggregate `Verify`/`Ci` (2026-06-04)

The aggregate `Verify` umbrella cannot run in this sandbox: its `VerifyPreflight` step
bootstraps the FAKE runner as a `dotnet-fake` global tool, which is not installed here
(gates are driven via `./fake.sh`). This is an environment constraint, not a feature or
readiness defect. Mitigation (the supported mode here): every constituent maintainer-verify
gate was run **individually and sequentially** and passed — see the per-target verdict table
in `target-metadata.md`. The merge gate `EvidenceAudit` is **PASS**. The auto-generated
`agent-ready-verdict.md` therefore stays `degraded` in-sandbox.
