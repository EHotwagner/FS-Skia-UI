# Contract: Focus-Aware Text-Routing Seam (FR-008 / FR-008a)

**Surface**: `src/Controls.Elmish/ControlsElmish.fs(i)` — an **additive** focus-aware text-delivery seam.
The existing `MapKey: ViewerKey -> bool -> 'msg option` field is **unchanged** (research D4).

## Behavior

When a key/text event arrives in `runInteractiveApp` and `ControlRuntime.FocusedControl` names a
focusable text control (`TextBox`/`TextArea`/`NumericInput`) present in the rendered tree, the host
delivers the keystroke (and committed/composed text) to **that** control via the existing
`TextInput.update` pipeline and folds the resulting product message. Otherwise it falls through to
`MapKey` exactly as today. A pointer **click** on a text control sets focus (via the existing
`FocusMovedByPointer` / `FocusControl` path) so a subsequent keystroke reaches it.

## Guarantees

- **T1 (typeable).** A focused text control receives typed characters at **that** control; an unfocused
  text control does **not** (SC-005).
- **T2 (reuses existing machinery).** Delivery routes through `ControlRuntime.FocusedControl` +
  `TextInput` `Msg`/`update`/`Effect` — **no** parallel text model is introduced (FR-008).
- **T3 (focus-on-click).** A pointer click on a text control can set focus so the next keystroke lands
  there (FR-008).
- **T4 (documented, no silent inertness).** The published `.fsi`/contract documents the seam; text
  controls are no longer silently non-interactive (FR-008).
- **T5 (scope guard).** Caret/selection gestures, IME UX beyond existing `Composition` hooks, undo/redo,
  and general focus/tab-traversal across all control kinds are **out of scope** — trajectory item E4
  (FR-008a).

## Verification

- Render a tree with a focusable text control; set focus via a pointer click on it (T3); deliver a
  keystroke through the seam; assert the character reaches the **focused** control's `TextInputModel` and
  not an unfocused one (T1).
- Surface test: the seam appears in the recaptured `ControlsElmish` `.fsi`/api-surface with documentation
  (T4); `MapKey`'s existing signature is unchanged (additive guard).
</content>
