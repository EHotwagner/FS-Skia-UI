# Quickstart: Retained-Frame Pointer Routing

## What this feature does

Pointer moves/clicks on the live host stop rebuilding a full control tree to route.
Routing now reads the **retained frame** (cached `LayoutResult`, `retainedHitTest`,
the frame's `EventBindings`/`BoundIds`, and a new retained-id → authored-id lookup),
performing **zero** `host.View` + `Control.renderTree` renders for routing. The old
full-render path is kept only as a parity oracle / counted fallback.

## Run the routing first

```sh
./fake.sh build -t Route          # prints the authoritative tier + minimal gate list
```

Expect escalation to **controls-public-surface** (because `ControlsElmish.fsi`'s
`FrameMetrics` gains `FullRenderFallbackCount`). Run only what `Route` prints; the
escalated set is, in deterministic FAKE order:

```sh
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

FAKE-backed commands share `.fake` state — run them **sequentially**, never
concurrently.

## Regenerate surface + per-package baselines (after the `.fsi` field lands)

```sh
./fake.sh build -t RefreshSurfaceBaselines
```

Updates `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` and the
per-package baselines with the new field.

## Run the parity + fallback tests

```sh
dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj \
  --filter "FullyQualifiedName~Feature110"
```

- `Feature110RetainedRoutingParityTests` — retained route vs. preserved
  `routeInteractivePointer` oracle: dispatched messages, matched identity, focus
  outcome equal across keyed / unkeyed-same-kind-sibling / composite / nested
  scenes (FR-006, SC-003/004).
- `Feature110FallbackTests` — a forced unroutable case increments
  `FullRenderFallbackCount` and still dispatches identically to the oracle
  (FR-007/009, SC-006).

## Regenerate the feature-109 corpus goldens (records the before/after delta)

```sh
PERF_CORPUS_REGEN=1 dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj \
  --filter "FullyQualifiedName~Feature109CorpusTests"
```

After this feature the corpus pointer scenarios show routing full-render counts
dropping to zero and `FullRenderFallbackCount = 0` per frame (FR-010, SC-005/007).
Record the before/after delta under
`specs/110-retained-pointer-routing/readiness/`.

## What must NOT change (FR-011 / SC-008)

At-rest rendered output, control geometry, focus/keyboard semantics, and every
dispatched-message result stay byte-identical. The only intended observable deltas:
fewer routing full-renders, and the new `FullRenderFallbackCount` field.

## Out of scope (Phase 3+)

Frame scheduler (`FrameCause`/`FrameInvalidation`), narrowed visual-state stamping,
view/control memoization, viewport virtualization, damage rects / picture caches,
text / layout caches, `SkiaViewer` backend review. The full-render path is **not**
removed.
