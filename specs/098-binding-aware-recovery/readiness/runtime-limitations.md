# Runtime limitations & unsupported scope (feature 098, R3)

## Platform envelope (unchanged by R3)

target=.NET 10 desktop only.
graphics=Vulkan presentation; SkiaSharp preview rendering packages with explicit version pinning.
unsupported macOS/mobile/browser=not supported targets; R3 adds no new platform surface.
no software-renderer fallback=there is no software-renderer fallback; a missing GPU/window system is an
environment limitation, not a product defect. R3 changes id derivation and recovery only — no Skia/Vulkan
surface change, so the platform envelope is untouched.

## Feature-specific limitations & non-goals (FR-008 / FR-009)

- no routed / bubbling / tunneling event system, no command system, no new public event type — R3 is
  additive, flat per-`ControlId` bindings only.
- no framework-level focus-traversal change. The 092 retained focus path
  (`resolveFocus` / `RetainedRender.retainedHitTest` / the `RetainedId` domain) is **out of scope and must
  not regress** (FR-008); its domain is separate from the `Layout.evaluate` + `nearestAuthored` +
  `EventBindings` dispatch seam R3 corrects.
- no catalog-wide retrofit of all 52 typed views' binding surfaces — that is a separate fitness pass.
- no data-binding, observable, dependency/attached-property, lookless-template, or CSS-selector surface
  (permanent roadmap non-goals).

## Totality / failure behaviour (constitution VII)

- recovery is **total**: `nearestAuthored` returns `None` → host falls back to `MapPointer` when nothing on
  the hit path is keyed or bound — never a throw, never an invented `Kind`/root id.
- `boundIdsOf` / `eventBindingsOf` / `collectBoundsWith` are total walks: an unbound/unkeyed node simply
  contributes no id; they never throw.
- the `disabledOrReadOnly` guard is preserved — a disabled bound node does not dispatch.
- the click-equivalent kinds stay the existing closed set (`click` / `changed` / `selected`).
- no new diagnostic class; existing diagnostic surfacing is preserved verbatim.

## Scope note — canonical id change (FR-007)

The canonical `ControlId` value for **unkeyed** controls changes `Kind → structural-path` in the public
`Bounds` list and the `ControlEvent.ControlId` payload (a documented canonicalization; the old `Kind`
fallback collided for same-kind siblings). Keyed authoring is unchanged. This is a net correctness gain
that *adds* dispatch for the previously-dead unkeyed case; consumers reading an unkeyed id should match on
the path or add a `Key` to pin a stable label.
