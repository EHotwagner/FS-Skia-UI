# Contract: `ControlRenderResult<'msg>` — `BoundIds` and canonical `ControlId`

**Package**: `FS.Skia.UI.Controls` · **Surface**: `src/Controls/Types.fsi` (+ `Types.fs` mirror)
**Tier**: 1 (contracted change) — moves the surface baseline; recapture api-surface + per-package `.fsi.txt`.

## Before

```fsharp
type ControlRenderResult<'msg> =
    { Scene: Scene
      Layout: LayoutNode
      Bounds: (ControlId * Rect) list
      Diagnostics: ControlDiagnostic list
      EventBindings: ControlEventBinding<'msg> list
      NodeCount: int }
```

## After

```fsharp
type ControlRenderResult<'msg> =
    { Scene: Scene
      Layout: LayoutNode
      Bounds: (ControlId * Rect) list
      Diagnostics: ControlDiagnostic list
      EventBindings: ControlEventBinding<'msg> list
      /// Canonical ids (the unified `Key ?? structural-path` scheme) of every node
      /// carrying at least one event binding. Same scheme as `EventBindings` and
      /// `Bounds`, so a recovered id is a direct membership/lookup key. Populated by
      /// `renderTree` and `render` (and the retained path); read by `nearestAuthored`.
      BoundIds: Set<ControlId>
      NodeCount: int }
```

## Guarantees

1. **Single canonical scheme (SC-003)**: for any laid-out node, the id in `Bounds`, the id
   in `EventBindings`, and the membership key in `BoundIds` are identical: `Key ?? path`.
2. **`BoundIds` population**: `id ∈ BoundIds` ⟺ that node's `eventBindings` is non-empty.
3. **`renderTree`**: `Bounds` and `BoundIds` both populated; canonical ids for unkeyed
   nodes are paths (`"0.1"`), not `Kind`.
4. **`render` (preview)**: `Bounds = []` (unchanged); `BoundIds` **populated** from bound
   nodes; `EventBindings` ids adopt the unified scheme.
5. **Retained path**: both `RetainedRender` frames emit `BoundIds` via the same
   `boundIdsOf`, byte-identical to the full rebuild.

## Compatibility note (migration guidance)

- **Keyed consumers**: no change — the reported `ControlId` is still the `Key`. The new
  `BoundIds` field is additive.
- **Unkeyed consumers reading `Bounds`/`ControlEvent.ControlId`**: the id for an unkeyed
  control changes from the `Kind` string (e.g. `"button"`) to the structural path
  (e.g. `"0.1"`). This is the documented canonicalization (FR-007): the old `Kind`
  fallback collided for same-kind siblings, so the path scheme is a net correctness gain
  that also *adds* dispatch for the previously-dead unkeyed case. Match on the path (or
  add a `Key` to pin a stable id).
