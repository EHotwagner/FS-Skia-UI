# Contract: Normalized Viewer Input

## Public Surface

Add or expose a public viewer/input contract with stable values equivalent to:

```fsharp
type ViewerKey =
    | ArrowLeft
    | ArrowRight
    | ArrowUp
    | ArrowDown
    | Enter
    | Space
    | Escape
    | Backspace
    | Letter of char
    | Digit of int
    | Function of int
    | Unknown of raw: string

module ViewerKeyboard =
    val normalize : raw: string -> ViewerKey
    val tryNormalizeEvent : event: ViewerEvent -> (ViewerKey * isDown: bool) option
```

Exact module/package placement must follow the existing viewer and keyboard
package ownership, with `.fsi` signatures drafted before implementation.

## Required Behavior

- Raw names for common keys normalize to documented values.
- Common alternate names such as left-arrow variants map to the same value.
- Letters, digits, function keys, unknown keys, key-down, and key-up remain
  observable.
- Generated apps do not compare raw backend-specific strings for user flows.

## Evidence

- FSI/packed-library transcript for public normalization helpers.
- Expecto tests for arrows, enter, space, escape, backspace, letters, digits,
  function keys, common alternates, and unknown values.
- Generated template test that starts the app through `ViewerEvent.KeyDown`.
- Readiness: `readiness/normalized-viewer-input.md`.
