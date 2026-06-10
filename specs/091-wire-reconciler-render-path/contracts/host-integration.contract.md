# Contract: Host-loop integration of the retained render path

**Status:** behavioral change to existing host loops; **signatures unchanged** (honest `.fsi` doc updates
only, by default). If research D5 adds a public seam it is additive and recaptured — but the default path
here is zero public-surface delta.

## Seam 1 — `runInteractiveApp` (`src/Controls.Elmish/ControlsElmish.fs:331`)

**Today:** holds `pointerState`/`focusedText`/`textModels`/`latest` refs and calls
`Control.renderTree host.Theme size (host.View size model)` fresh on every interaction (`:349/376/391`).

**Wired:** add a `retained : RetainedRender<'msg> option ref` alongside the existing refs.

```
on each frame (size, model):
    let next = host.View size model
    match retained.Value with
    | None        -> retained.Value <- Some (RetainedRender.init host.Theme size next)
    | Some prev   -> let s = RetainedRender.step host.Theme size prev next
                     surfaceDiagnostics s.Diagnostics          // FR-007
                     retained.Value <- Some s.Retained
    // re-key focus/text state to the stable RetainedId (FR-003):
    focusedText / textModels lookups go through retained StateByIdentity, not the path-derived ControlId
```

| Obligation | Spec ref |
|------------|----------|
| `focusedText` / `textModels` re-key to the **stable** `RetainedId`, so focus + text-input survive a positional shift. | FR-003 / SC-002 |
| The consumer host contract (`Init`/`Update`/`View`/`MapKey`/`MapPointer`/`Tick`/`Theme`/`Diagnostics`) is **unchanged** — no consumer rewrite to benefit. | FR-008 / SC-007 |
| `host.Update` folding is unchanged; no new effects/subscriptions/interpreter behavior. | FR-008 |
| `.fsi` doc updated to state honestly: "the host produces each frame by diffing the next tree against a retained previous tree and applying the patch." | FR-008 (honesty) |

## Seam 2 — `SkiaViewer.dispatchHostMsg` (`src/SkiaViewer/SkiaViewer.fs:2364`, size variant `:2437`)

**Today:** `currentScene <- host.View currentModel` — an unconditional full re-render after each message.

**Wired:** thread a retained structure through `currentScene`'s neighbours so the repaint diffs the next
tree against the retained previous and applies the patch (O(changed-subtree)); output identical to the full
re-render (golden parity). Signatures unchanged; `.fsi` doc notes the behavioral change.

| Obligation | Spec ref |
|------------|----------|
| The repaint becomes O(changed-subtree) for a localized update; rendered output is identical to the prior full re-render. | FR-004 / FR-005 |
| `currentModel`/`host.Update` semantics unchanged; the seam stays the repaint integration point named by the spec. | FR-004 |

## Seam 3 — Per-control animation clock (FR-003 survives-proof only)

Attach an `AnimationState`/`Elapsed` (Scene `Animation`, feature 073) to the retained identity, advanced by
the existing `host.Tick` delta and sampled via `Animation.applyAt`. Scope is **only** proving a kept
control's clock continues across an unrelated re-render — **not** broad animation retargeting (sequenced
after E2).

## Seam 4 — Diagnostics surfacing (FR-007)

`RetainedRender.step`'s `Diagnostics` (== `Reconcile.diff` diagnostics, e.g. `KeyCollision` from duplicate
sibling keys) flow through the **existing** `ControlDiagnostic` channel that `ControlRenderResult.
Diagnostics` already carries — never silently dropped; the path stays total in their presence.
