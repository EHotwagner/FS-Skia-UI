# Runtime limitations + permanent non-goals — feature 106 (controls-api-discoverability)

## Supported runtime

Feature 106 touches the framework wherever it runs: a **.NET 10 desktop** host rendering
through **Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop
(`net10.0`). **unsupported macOS/mobile/browser** — there is **no software-renderer fallback**;
those are out of scope for the framework and therefore for this feature.

Feature 106 is documentation + governance + a behaviour-preserving starter migration: it adds
**no** runtime code path, window, GPU, or wall-clock dependency. The typed front door lowers
structurally to the same lowered control IR (proven by `TypedLoweringTests`), so the generated
starter renders the same controls it did before; the new gate is a pure `.fsi`-text analysis.
The change is platform-independent and introduces no new runtime failure mode.

## Public-surface scope (no `.fsi` shape delta)

`///` doc comments are added/replaced across `src/Controls/**/*.fsi`; no `val`/`type`/`member`
signature is added, removed, or retyped. Per-package surface baselines are byte-stable because
`PerPackageSurface.normalize` strips `//`-prefixed lines before diffing; the consumer-visible
api-surface bundle (`template/base/docs/api-surface/Controls/*.fsi`) copies comments verbatim
and is regenerated current by `RefreshSurfaceBaselines`. The new `ControlsDocCoverageCheck`
gate is internal build tooling (`build/Governance/**`), not a product runtime surface.

## Out of scope / permanent non-goals

- **Non-Controls boilerplate** — the remaining `"Public contract … exposed"` occurrences
  outside `src/Controls/**` are deferred (this feature scopes the gate + rewrite to the
  Controls public surface that triggered the reflection incident).
- **Migrating all 52 controls to the typed front door** — only the starter's demonstrated set
  uses it; the rest stay reachable and documented (the legacy builder surface is fully
  documented, not removed).
- **A from-scratch fsdocs HTML site**, and **shipping `.fsi` source inside the NuGet package**
  (F# does not do this) — both out of scope.
- **No controls-model redesign** — no XAML / data-binding / dependency-properties (permanent
  non-goals).

## Failure diagnostics

No new runtime failure path is introduced. The gate fails the build (with an actionable
per-member report) only when a placeholder / empty / duplicate-only summary regresses onto the
Controls public surface; that is a documentation-coverage failure class
(`controls-doc-placeholder`), not a product runtime failure.
