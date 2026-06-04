# Runtime Limitations — 061-breakout-consumer-friction-followups

This feature is **governance + template + skills** scope only. It introduces **no product
runtime, layout, rendering, Vulkan, or Skia change**.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux, renders
through **Vulkan**, and depends on a **SkiaSharp preview** native build. Platforms remain
**unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**. This
feature changes none of that; it touches no product runtime code.

## This feature's scope

- No new `Model` / `Msg` / `Effect` / `Cmd` / `init` / `update` / interpreter on any product
  surface — Principle IV (Elmish/MVU) is **N/A** here (recorded in `target-metadata.md` / T002).
  The FR-004/FR-007 changes are pure render-string additions in `build/Governance/**`
  (`Render.graphVerdictLine`, `Render.readinessContractDiagnostics`) wired at the existing
  evidence front-end edge; no I/O shape changes.
- The arcade helpers triaged by FR-011 (fixed-step accumulator, collision/reflection,
  paddle rebound, `reserveHudBand`) are **documented as pure `update`-side conventions**
  (see `arcade-helper-triage.md`), not shipped runtime — so no `Model`/`Msg`/`Effect`/
  interpreter is introduced.
- No persistent windowed host is launched by any task; there is no on-screen visibility check.

## Environment limitation — aggregate `Verify`/`Ci`

The aggregate `Verify`/`Ci` umbrella cannot run in this sandbox: its `VerifyPreflight`
step bootstraps the FAKE runner as a `dotnet-fake` global tool, which is not installed here
(gates are driven via `./fake.sh`). This is an environment constraint, not a feature or
readiness defect. Mitigation (the supported mode here): every constituent gate Route prints
was run **individually and sequentially** — see the per-target verdict table in
`target-metadata.md`. The merge gate `EvidenceAudit` is the authoritative verdict. The
auto-generated `agent-ready-verdict.md` therefore stays `degraded` in-sandbox; this is a
**non-authoritative aggregate** limitation, not a gate failure.
