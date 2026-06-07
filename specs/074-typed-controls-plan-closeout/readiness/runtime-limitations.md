# Runtime Limitations & Feature Classification — Typed-Controls Plan Closeout (074)

## Feature classification (T001)

- **Tier**: Tier 2 — internal / documentation-governance change. No public API surface,
  no new dependency, no inter-project contract change, no observable runtime behavior change.
- **Affected paths**:
  - `.agents/skills/fsharp-code-generation/SKILL.md` (edited — US1 C13 catalog-generation
    worked example) and its regenerated `.claude` peer.
  - `.agents/skills/fs-skia-reconciliation/SKILL.md` (new — US3) and its regenerated `.claude`
    peer; the regenerated skill index (`template/base/docs/skillist-reference.md`).
  - `docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md` (US2
    status/§13/§16 refresh; §1-onward provenance body untouched).
- **Public-API impact**: **none** (SC-005). No `.fsi` signature, package identity, or surface
  baseline changes. `Reconcile` stays `module internal` and is **not** wired into the render
  path by this feature (FR-010).
- **MVU applicability**: **N/A**. No stateful workflow, I/O, command, effect, subscription, or
  interpreter behavior is introduced or changed.
- **Evidence obligations**: skill-currency (canonical `.agents` ↔ generated `.claude` in sync)
  for both `fsharp-code-generation` and the new `fs-skia-reconciliation`; the refreshed plan
  report; plus the routed tier's readiness artifacts.

## Unsupported scope (deferred)

- **Wiring keyed reconciliation into the live render/diff path** — explicitly out of scope; a
  separate future feature. The `fs-skia-reconciliation` skill documents the parked module, it
  does not wire it.
- **A standalone `fs-skia-catalog-generation` skill** — out of scope; catalog generation is
  folded into `fsharp-code-generation` (C13).
- No product control, typed-surface, design-token, animation, or catalog-*content* change.

## Platform runtime boundary

This feature ships no runtime code; the framework's supported runtime boundary is unchanged
and recorded here for completeness:

- **.NET 10 desktop** host (Windows and Linux desktop).
- **Vulkan**-backed GPU path for the windowed host.
- **SkiaSharp preview** is the pinned rendering dependency (unchanged — no new dependency is
  added by this feature).
- **unsupported macOS/mobile/browser**: these targets are out of runtime scope.
- **no software-renderer fallback**: there is no software rasterizer substitute; unsupported
  hosts are reported as unsupported, not silently downgraded.

## Non-authoritative aggregate

`GeneratedProductCheck` fails locally for an environment reason (the generated product's
`.specify/feature.json` has no usable `feature_directory` entry, so its evidence-graph step
cannot resolve a feature), independent of this change — the generated product's own `Dev`,
`GeneratedGuidanceCheck`, and `TemplateDrift` all complete. It is a **non-authoritative
environment failure**, not a product regression; the authoritative focused gates all pass.
