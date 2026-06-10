# Contract: Host Key-Routing Seam (`FS.Skia.UI.Controls.Elmish`)

The interactive host's key-delivery seam — the E4 analogue of the 092 `routeFocusedText`. It binds
the pure `Focus` model (focus-model.md) to the live retained tree (E2 identity) and the authored
`EventBindings`, and is wired into `runInteractiveApp` ahead of the existing `host.MapKey` fallback.
`internal` because it takes the internal `RetainedRender` structure; the adapter tests reach it via
`InternalsVisibleTo` (it IS the production key-routing path — SC-002/SC-004 exercise it with no
hand-seeded identity map).

## Surface (`src/Controls.Elmish/ControlsElmish.fsi`, additive)

```fsharp
/// E4 (FR-003/FR-006/FR-007): route a delivered key to the current FocusedControl over the
/// RETAINED tree, generalizing the 092 routeFocusedText text seam to all interactive kinds.
/// Resolves the focused control via its stable RetainedId (E2 identity), reads its
/// KeyboardOperation, and applies Focus.route:
///   - Activate  → the focused control's authored activation EventBindings (the same message a
///                 pointer activation dispatches), matched by (ControlId, EventKind), fired ONCE
///                 (no double-dispatch);
///   - Navigate  → the focused control's authored value-change/selection bindings;
///   - Traverse  → Focus.traverse over `order`, emitting ControlRuntimeMsg.FocusControl next;
///   - Fallthrough → no message (the host then consults host.MapKey).
/// A focused TEXT control's text-relevant keys are handled by the unchanged E1 routeFocusedText
/// path BEFORE this is consulted (so text delivery is not regressed, SC-003). Returns the
/// (possibly unchanged) retained structure, the focus-update ControlRuntime messages, and the
/// focused control's authored product messages. Total; never throws (an unmatched key → no msgs).
val internal routeFocusedKey:
    retained: RetainedRender<'msg> ->
    focused: RetainedId option ->
    order: TabOrder ->
    key: ViewerKey ->
    shift: bool ->
        RetainedRender<'msg> * ControlRuntimeMsg list * 'msg list
```

`runInteractiveApp`'s `.fsi` doc is updated to honestly describe the key path (echoing the E1
lesson that the contract doc must match the code): each native key is normalized, offered to the
E1 `routeFocusedText` seam (text controls), then to `routeFocusedKey` (general activation /
navigation / traversal), and finally falls through to `host.MapKey`.

## Key normalization

`ViewerKey` (`FS.Skia.UI.KeyboardInput`) is normalized to the string names used in
`KeyboardOperation.ActivationKeys`/`NavigationKeys` and a traversal flag:

| `ViewerKey` (+ shift) | normalized `key` | `isTab` |
|-----------------------|------------------|---------|
| `Enter` | `"Enter"` | false |
| `Space` | `"Space"` | false |
| `ArrowLeft`/`Right`/`Up`/`Down` | `"ArrowLeft"`/… | false |
| `Unknown "Tab"` (host Tab) | `"Tab"` | **true** |
| other | best-effort name | false |

The normalization lives at the host edge (the interpreter), keeping `Focus.route` host-independent
(it takes plain strings + flags). Tab + `shift` → `Traverse Previous`; Tab alone → `Traverse Next`.

## Precedence (R3, mirrors E1 binding-wins / host-fallback)

1. **Text seam (E1, unchanged)** — focused text control + text-relevant key → `routeFocusedText`.
2. **`routeFocusedKey` / `Focus.route`** — `Activate` / `Navigate` → authored bindings.
3. **Traversal** — `Traverse move` → `FocusControl (Focus.traverse order focused move)`.
4. **`host.MapKey`** — `Fallthrough`.

No double-dispatch: an authored binding that matches consumes the key; `host.MapKey` is consulted
only for keys no binding and no traversal matched (FR-007).

## Laws (asserted by adapter tests, via `InternalsVisibleTo`)

- **Activation once** (SC-002): a focused `Button` + an `ActivationKey` produces exactly the
  pointer-equivalent product message, exactly once.
- **Navigation** (SC-002): a focused `Slider` + a `NavigationKey` (ArrowLeft/Right) produces its
  value-change message deterministically. (A composite such as `RadioGroup` is one tab stop; its
  arrows fire its authored value-change/selection binding — E4 owns no sub-focus cursor.)
- **Text unchanged** (SC-003): a focused text control + a printable key routes through
  `routeFocusedText` identically to E1 (the E1 evidence still passes).
- **Stability over the live retained path** (SC-004): after a sibling-shifting `RetainedRender.step`,
  `routeFocusedKey` still resolves the focused control to the same `RetainedId` — no hand-seeded map.
- **Fall-through is total** (SC-006): a key matching nothing yields `([], [])` and the host consults
  `host.MapKey`; never throws.
