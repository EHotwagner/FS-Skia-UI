# Phase 1 Contracts: `.fsi` Signature Deltas

This feature is Tier 1. Three surfaces change. Internal-module changes (`RetainedRender`) carry
zero public-surface-baseline delta but still update the per-package internal baseline; the
`SkiaViewer` and `ControlsElmish` changes update public per-package + cross-package baselines.

---

## 1. `src/Controls/RetainedRender.fsi` — internal module (zero public-surface delta)

```fsharp
/// Per-frame work reduction (SC-003). 092: split into changed vs shifted work.
///   RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount
///   RecomputedNodeCount < BaselineNodeCount   (for any localized change)
type internal WorkReductionRecord =
    { BaselineNodeCount: int
      RecomputedNodeCount: int
      ChangedSubtreeBound: int
      ShiftedNodeCount: int }          // NEW — shifted-but-unchanged work, counted distinctly

type internal RetainedRender<'msg> =
    { Root: RetainedNode<'msg>
      NextId: uint64
      StateByIdentity: Map<RetainedId, RetainedUiState>
      Theme: Theme }                   // NEW — the theme this structure was painted under (FR-008 reuse key)

/// 092: init now also reports first-frame diagnostics (e.g. duplicate-key KeyCollision) and the
/// adapter reuses the painted scene (single first-frame paint, FR-009).
type internal RetainedInit<'msg> =     // NEW result type (or init returns a tuple)
    { Retained: RetainedRender<'msg>
      Render: ControlRenderResult<'msg>
      Diagnostics: ControlDiagnostic list }

module internal RetainedRender =
    val init: theme: Theme -> size: Size -> control: Control<'msg> -> RetainedInit<'msg>   // CHANGED return

    val step: theme: Theme -> size: Size -> prev: RetainedRender<'msg> -> next: Control<'msg>
                -> RetainedRenderStep<'msg>                                                // unchanged sig

    /// NEW (FR-004): deepest retained node whose Fragment.Box contains the point — a stable,
    /// per-node identity that disambiguates unkeyed same-kind siblings. None for a true gap.
    val retainedHitTest: x: float -> y: float -> retained: RetainedRender<'msg> -> RetainedId option
```

Guarantees unchanged from 091 (totality / determinism / identity-at-rest / round-trip byte-identity),
plus: a theme change between `step` calls invalidates all fragment reuse (FR-008); `init` surfaces a
first-frame collision (FR-009).

---

## 2. `src/SkiaViewer/SkiaViewer.fsi` — public seam widening (FR-006)

```fsharp
type InteractiveViewerHost<'model,'msg> =
    { Init: ...
      Update: ...
      View: ...
      MapKey: ViewerKey -> bool -> 'msg list      // CHANGED from `'msg option`
      MapPointer: ...
      Tick: ...
      Diagnostics: ... }
```

**Semantics**: `[]` = key not handled by the host seam (was `None`); a non-empty list dispatches
**every** message in order through `update` (FR-006). T005 MUST enumerate every
`ViewerKey -> bool -> 'msg option` field across the viewer host records and either widen each to
`'msg list` (same semantics) or, if no sibling field exists, record that absence in the task
note — the scope of the widening is resolved at contract time, not left open.

### Compatibility / migration note (ships in the public migration guidance)

> `InteractiveViewerHost.MapKey` returns `'msg list` instead of `'msg option`. Migration is
> mechanical: `Some m` → `[ m ]`, `None` → `[]`. Hosts that returned a single message are unchanged
> in behavior; hosts that need to emit several messages from one key now can. This is a breaking
> signature change to the interactive host record and is noted in the package release notes.

---

## 3. `src/Controls.Elmish/ControlsElmish.fsi` — package surface (focus routing on identity)

The focus/text routing seam (currently keyed by `ControlId`) re-keys onto the retained structure.
Per Principle I (FSI-first), these seam signatures are **pinned here before any `.fs` body is
written** — T006 drafts exactly this `.fsi` and T015/T016 implement against it; implementation may
not silently alter the shape (a shape change returns to design/contract review and re-captures the
baseline):

```fsharp
/// 092: focus resolution returns the stable RetainedId of the clicked control via the retained
/// tree (FR-004), replacing the ControlId hitTest|>nearestAuthored path.
val resolveFocus: retained: RetainedRender<'msg> -> x: float -> y: float -> RetainedId option

/// 092: deliver a keystroke to the focused control's RetainedId-keyed TextInput state held in the
/// retained structure, seeding from the control's current value + line mode on first focus (FR-005),
/// and return the next retained structure plus ALL matched onChanged product messages (FR-006).
val routeFocusedText:
    retained: RetainedRender<'msg> -> focused: RetainedId option -> msg: TextInputMsg
        -> RetainedRender<'msg> * 'msg list
```

`mapKey` in the closure now returns `'msg list` (matching the widened seam) and dispatches all
matched messages. The 090 `ControlId`-keyed `routeFocusedText` signature is **replaced** (breaking
within this package surface — covered by the recaptured baseline + migration note). Any consumer
calling the old seam migrates to the retained-structure form.

---

## Surface-baseline obligations

- Recapture per-package surface for `FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Controls.Elmish`
  (public deltas) and `FS.Skia.UI.Controls` (internal `.fsi` per-package baseline; no public delta).
- Recapture the cross-package surface baseline (MapKey + ControlsElmish seam).
- Add the compatibility/migration note to the public docs/release notes for the MapKey widening.
