# Contract: Demonstrative Preview Samples & Currency

Scope: the per-control sample source, the real render path, the strengthened currency gate,
and the regenerated evidence record. Each clause is independently testable.

## P1 — Single declared sample source (FR-002)

There is exactly one declared list of `ControlSampleDefinition` entries, keyed by control
`Id`, in `catalogFacts` order.
- **P1.1 Totality**: the set of definition ids equals the set of `catalogFacts` ids — no
  missing id, no orphan id. (Test: set-equality over ids.)
- **P1.2 Explicitness**: each `Demonstrative` entry constructs its sample state from fixed
  literals through the typed front door; no reliance on bare `.defaults` to supply visible
  content. (Test/review: no demonstrative entry renders an empty-content widget.)

## P2 — Real render-only path only (FR-003)

Every committed preview is produced by `Widget.toControl` → `Control.render Theme.light` →
`SceneNode.Group` → `SkiaViewer.captureScreenshotEvidence` with
`CaptureMode = ViewerRenderTargetPng`, result `status = ScreenshotOk`.
- No preview is fabricated, hand-drawn, placeholder, metadata-only, or 1×1.
- (Test: render harness uses only that path; evidence record `RendererMode` is uniformly
  `render-only / ViewerRenderTargetPng`.)

## P3 — Decodable, non-1×1, non-trivial (FR-004) + strengthened gate (FR-005)

`ControlsCatalogDocsCheck` over the committed tree:
- **P3.1** PASSes on the regenerated demonstrative tree.
- **P3.2** FAILs with the matching finding on each negative case (one per class, SC-003):
  blanked/`Trivial` (bytes < pinned `T`), `Missing`, `Undecodable`, `Orphan`, stale/missing
  detail region, `DeadLink`.
- **P3.3** `Trivial` is treated as a failing preview exactly like missing — a demonstrative
  preview that regresses to empty/near-empty content fails.
- (Test: failing-first governance tests in `FS.Skia.UI.Build` test project.)

## P4 — Deterministic, idempotent regeneration (FR-008)

Re-running the render harness over the same sample source on the same render-capable host
produces **byte-identical** PNGs.
- (Test: harness idempotence over committed bytes / a hash manifest — 0 spurious diffs,
  SC-004.)

## P5 — Committed source assets; GPU-free consumption (FR-009)

Previews are committed under `docs/img/controls/`; `dotnet fsdocs build --strict --eval`
copies them with every image link resolving and requires **no** render-capable host.
- (Test: docs build evidence `docs-build.md`.)

## P6 — Honest unsupported + visible counts (FR-007, SC-005)

A control that cannot be honestly rendered has `Kind = Unsupported`, commits no image, and
carries a `preview-status: unsupported` detail-page marker. The regenerated
`controls-preview-evidence.md` records rendered-vs-unsupported counts, reconciled to the
supported catalog size with 0 silent omissions.
- (Test: evidence-record consistency check cross-referenced by the gate.)
