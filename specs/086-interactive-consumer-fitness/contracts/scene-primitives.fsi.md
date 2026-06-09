# Contract: `FS.Skia.UI.Scene` additions (FR-013, FR-014)

Additive-only `.fsi` deltas to `src/Scene/Scene.fsi`. Existing cases unchanged
(back-compat). Per-package + cross-package Scene baselines recapture after.

## `SceneNode` — two new cases

```fsharp
type SceneNode =
    | ...                                  // all existing cases unchanged
    | Translate of (float * float) * Scene // (dx, dy) offset wrapping a sub-scene  (FR-013)
    | SizedText of (float * float) * string * float * Color  // pos, text, size, color (FR-014)
```

## `SceneElement` — two new descriptors

```fsharp
type SceneElement =
    | ...
    | TranslateElement
    | SizedTextElement
```

## Smart constructors (module `Scene`)

```fsharp
/// Offset an entire sub-scene by (dx, dy). Offsets ALL node kinds uniformly —
/// including Path/Points/Vertices/Chart — by pushing a canvas translation, so it
/// replaces a hand-written coordinate-walking shift. Nesting composes additively.
val translate : dx: float -> dy: float -> scene: Scene -> Scene

/// A Text node with an explicit font size, for chrome sized to its container.
/// Bare `Scene.text` (no size) keeps its current default-font rendering.
val sizedText : position: (float * float) -> text: string -> size: float -> color: Color -> Scene
```

## Behavioral laws (semantic tests)

1. `translate dx dy s` shifts the effective coordinates of **every** node in `s`
   (assert on a scene containing `Path`/`Points`/`Chart`) by exactly `(dx, dy)`.
2. `translate a 0 (translate b 0 s)` ≡ `translate (a+b) 0 s` (composition).
3. `sizedText p t sz c` renders glyphs at `sz`; a narrow-column label at small `sz`
   fits without clipping (SC-006).
4. Bare `Text` rendering is **byte-identical** to pre-feature (back-compat golden).
