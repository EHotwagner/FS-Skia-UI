# Runtime Limitations — 062-space-invaders-consumer-friction-followups

This feature is **governance + template + skills + a small public helper surface** scope.
It introduces **no product runtime, layout, rendering, Vulkan, or Skia change**. The only
new public API (FR-010 `FS.Skia.UI.SkillSupport.Random` / `.Hud`) is pure value-type
utilities — integer arithmetic and `float` band math — with no I/O and no graphics.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux, renders
through **Vulkan**, and depends on a **SkiaSharp preview** native build. Platforms remain
**unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**. This
feature changes none of that; it touches no product runtime code.

## This feature's scope

- No new framework `Model` / `Msg` / `Effect` / `Cmd` / `init` / `update` / interpreter on
  any product surface — Principle IV (Elmish/MVU) is **N/A** here (recorded in
  `target-metadata.md`). The FR-004/005/007 changes are render-string / diagnostic additions
  in `build/Governance/**`; no I/O shape changes.
- The FR-010 seeded RNG is a **pure value-type utility for a consumer's Elmish core** —
  threaded through their pure `update`, owning no state and performing no I/O. It
  deliberately avoids ambient `System.Random` so consumer `update` stays pure and
  replayable. `reserveHudBand` is plain `float` band math, no `Scene.Rect` dependency.
- No persistent windowed host is launched by any task; there is no on-screen visibility
  check.

## Environment limitation — aggregate `Verify`/`Ci`

The aggregate `Verify`/`Ci` umbrella cannot run in this sandbox: its `VerifyPreflight`
step bootstraps the FAKE runner as a `dotnet-fake` global tool, which is not installed here
(gates are driven via `./fake.sh`). This is an environment constraint, not a feature or
readiness defect. Mitigation (the supported mode here): every constituent gate Route prints
was run **individually and sequentially** — see the per-target verdict table in
`target-metadata.md`. The merge gate `EvidenceAudit` is the authoritative verdict. The
auto-generated `agent-ready-verdict.md` therefore stays `degraded` in-sandbox; this is a
**non-authoritative aggregate** limitation, not a gate failure.
