# Real image evidence (084)

evidence-kind=deferred
status=deferred
artifact-decodable=deferred
proves-scene-rendering=deferred
proves-desktop-visibility=deferred

## Scope

The authoritative **decodable windowed-fullscreen launch screenshot** (SC-001/SC-002)
is captured from the **generated default executable** on a **display-capable host** —
the project's documented, locally **non-authoritative** `GeneratedProductCheck` path
(see `aggregate-hang-diagnostics.md`). The framework repo ships libraries + a template
and opens no desktop window in its own validation, so no real desktop screenshot is
produced here.

No metadata-only screenshot is claimed as visual proof, no 1×1 fallback is asserted,
and no pixel-readback is offered as desktop-visibility proof. The honest state on this
host is `deferred`: the image evidence is produced where a window genuinely exists. The
framework-level behavior (the new state, default, validation, and the
`applyWindowBehaviorToOptions` mapping) is proven against the built library in
`readiness/fsi-session.txt` and `tests/SkiaViewer.Tests`.
