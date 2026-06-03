# Runtime Limitations — 058-skills-quality-feedback

This feature is **governance + template + authoring** scope only. It introduces
**no product runtime, layout, rendering, Vulkan, or Skia change**.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux, renders
through **Vulkan**, and depends on a **SkiaSharp preview** native build. Platforms remain
**unsupported macOS/mobile/browser**, and there is **no software-renderer fallback** — a
host without the Vulkan/Skia native stack cannot render. This feature changes none of
that; it touches no product runtime code.

## This feature's scope

- No new `Model` / `Msg` / `Effect` / `Cmd` / `init` / `update` / interpreter on any
  product surface — Principle IV (Elmish/MVU) is **N/A** here (recorded in
  `target-metadata.md` / T004).
- The only new "flow" is the authoring-time per-phase feedback prompt, delivered through
  the existing Spec Kit `after_*` hook surface in a generated project's
  `.specify/extensions/feedback/`. It owns no product-runtime state.
- The new `FS.Skia.UI.SkillSupport` package is build/authoring-scoped (the same shipping
  pattern as `FS.Skia.UI.Build`): it ships to the template but is not a rendering/runtime
  package.
- No persistent windowed host is launched by any task in this feature; there is no
  on-screen visibility check involved.

## Environment limitation — `Verify` aggregate (2026-06-03)

The `Verify` umbrella target cannot run in this sandbox: its `VerifyPreflight` step bootstraps
the FAKE runner as a `dotnet-fake` global tool and fails with an **environment-failure**
(`FAKE runner did not start after tool restore: ... dotnet-fake does not exist`). Gates in this
environment are driven via `./fake.sh`, not the global tool. This is an environment constraint,
not a feature or readiness defect. Mitigation (the supported mode here): every constituent
maintainer-verify gate was run **individually and sequentially** and passed — see the per-target
verdict table in `target-metadata.md`. The merge gate `EvidenceAudit` is **PASS**. The
auto-generated `agent-ready-verdict.md` stays `degraded` because the aggregate that would mark it
ready cannot bootstrap in-sandbox.
