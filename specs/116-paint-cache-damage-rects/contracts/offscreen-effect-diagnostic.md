# Contract: Advisory Offscreen-Effect Diagnostic

Surface: a new advisory `ControlDiagnosticCode` case (`Types.fsi`), surfaced as a
`ControlDiagnostic` through the existing `Diagnostics` channel — exactly like `KeyCollision`.
(FR-011; US4.)

## When it fires

The framework surfaces the diagnostic when a control's paint **requires offscreen
composition**:

- a **non-opaque opacity group** over a multi-node subtree (`withOpacity` alpha < 1 over > 1
  node, `SceneRenderer.fs:28-30`),
- a **clip** (`ClipNode`, `SceneRenderer.fs:356-367`), or
- a **drop-shadow / image-filter** effect (`CreateDropShadow`, `SceneRenderer.fs:125`).

## Guarantees

1. **Fires when offscreen-forcing.** A control whose paint requires offscreen composition is
   flagged with an advisory `ControlDiagnostic` naming the control + the offscreen-forcing
   effect.
2. **Silent otherwise.** A control with no offscreen-forcing effect produces no offscreen-
   effect diagnostic.
3. **Advisory only.** The diagnostic never fails a build and never alters rendered output:
   in both the fires and does-not-fire cases, rendered output is **byte-identical** to the
   pre-feature state.

## Why advisory

Offscreen composition is a real cost (a separate layer + composite) but a legitimate design
choice. The diagnostic makes the cost (and its caching consequence) **visible** to the
author rather than discovered as jank — it informs, it does not block (matching
`KeyCollision`'s non-blocking advisory model).

## Evidence

- `tests/Controls.Tests/Feature116OffscreenDiagTests.fs` — fires for an opacity-group / clip
  / drop-shadow control; does not fire for a plain control; rendered output unchanged in both.
