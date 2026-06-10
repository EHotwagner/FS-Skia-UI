# Contract: Authored Event-Binding Dispatch in the Interactive Host (FR-001 / FR-002 / FR-003)

**Surface**: `src/Controls.Elmish/ControlsElmish.fs(i)` — `routeInteractivePointer`, `runInteractiveApp`.
**Kind**: behavioral change to a shipped public function + a corrected `.fsi` doc. Additive.

## Behavior

For each `PointerInteraction` emitted by `Pointer.update` in a `routeInteractivePointer` step:

1. The host resolves the interaction's hit `ControlId` to the **authored** id via the
   nearest-keyed-ancestor recovery ([recovery.md](./recovery.md)).
2. The host looks up `rendered.EventBindings` for a binding with `ControlId =` resolved id **and**
   `EventKind =` the interaction's event kind (`Click`→`"click"`, value-change→`"changed"`).
3. **Match** ⇒ the host dispatches **only** that binding's message
   (`binding.Dispatch <synthesized ControlEvent>`) for this interaction and **does not** also offer it
   to `host.MapPointer`.
4. **No match** (no binding for the resolved id/kind, or recovery returned `None`) ⇒ the host offers the
   **raw** interaction to `host.MapPointer`, exactly as today.

The produced `'msg list` (and therefore `host.Update` folding) preserves interaction order.

## Guarantees

- **G1 (dispatch).** A control authored with `onClick`/`onChanged`, hosted via `runInteractiveApp`,
  has its bound message dispatched on the matching live interaction — with **zero** `MapPointer` clauses
  authored for it (SC-001).
- **G2 (precedence, no double-dispatch).** When both an authored binding and a `MapPointer` clause could
  respond to one interaction, the **binding wins** and `MapPointer` is **not** consulted for that
  interaction; the model never advances twice for one interaction (FR-003).
- **G3 (additive / non-regressive).** An interaction with **no** consuming binding behaves bit-for-bit as
  before — `MapPointer`-only consumers are unbroken (SC-001 second clause).
- **G4 (honest contract).** The `ControlsElmish.fsi` doc accurately states whether/how authored
  `EventBindings` fire; the prior false "`Layout.hitTestComputed` × `EventBindings`" claim is made true
  by G1 (or corrected to match) — **no** doc promises dispatch the code does not perform (SC-002, FR-002).

## Verification

- Headless: build a tree with one `onClick` and one `onChanged` binding, run `routeInteractivePointer`
  (or `runInteractivePointerOnce`) with a synthesized press+release at the control's bounds; assert the
  **bound** message appears in the returned `'msg list` and folds into the model; assert a competing
  `MapPointer` clause for the same control does **not** also fire (G2).
- A control with no binding + a `MapPointer` clause still routes via `MapPointer` (G3).
- Surface test: recaptured `ControlsElmish` per-package `.fsi.txt` and api-surface reflect the corrected
  doc; no stale "by ControlId" claim remains (G4).
</content>
