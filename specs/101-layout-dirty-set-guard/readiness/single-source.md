# Single-source — US2 evidence (feature 101, R7, T012)

authoritative-command=git diff src/Controls/Control.fs ; dotnet run --project tests/Controls.Tests -c Debug -- --filter-test-list "Feature101"
artifact-path=src/Controls/Control.fs
status=pass
failure-class=duplicated-layout-name-literal
next-action=if a new layout-driving name literal is added, route it through the [<Literal>] token and let the probe gate pin membership

## One authoritative token per layout-driving name (SC-002)

After R7, each layout-driving attribute name has exactly **one** authoritative definition inside the
internal `ControlInternals` module — a `[<Literal>] private` constant — and every read resolves to it:

```fsharp
let [<Literal>] private AttrWidth = "width"
let [<Literal>] private AttrHeight = "height"
let [<Literal>] private AttrOrientation = "orientation"
```

| Consumer | Before | After |
|---|---|---|
| `nodeWidth` (`hasAttr` / `floatValue`) | `"width"` ×2 | `AttrWidth` |
| `nodeHeight` (`hasAttr` / `floatValue`) | `"height"` ×2 | `AttrHeight` |
| `layoutNode` preview reads | `"width"` / `"height"` | `AttrWidth` / `AttrHeight` |
| `orientationOf` (`tryLast`) | `"orientation"` | `AttrOrientation` |
| `toLayout` size derivation (`hasAttr`) | `"width"` / `"height"` | `AttrWidth` / `AttrHeight` |
| `layoutAffectingAttrNames` | `Set.ofList [ "width"; "height"; "orientation" ]` | `Set.ofList [ AttrWidth; AttrHeight; AttrOrientation ]` |

`git grep '"width"\|"height"\|"orientation"' src/Controls/Control.fs` after R7 returns **zero**
layout-driving-name string literals inside `ControlInternals` (the only remaining `"orientation"`
literal is the public `Stack.orientation` **builder** in a separate module — the emit side, not a
classifier read; it is orthogonal to the read/classify drift the gate guards).

## Why this is "the comment's claim made true"

- The name tokens remove **typo** drift cheaply — one authoritative token per name (SC-002's "exactly
  one authoritative definition"); they are `private`, so **no `Control.fsi` change** and **no behavior
  change** (byte-identically the same three strings).
- The runtime `layoutAffectingAttrNames` `Set` is kept (the hot `layoutDirtySet` classifier needs a
  cheap `Set.contains`) but is no longer a free-to-drift second list: the behavioral-probe **membership**
  equality gate (T009, see `drift-guard.md`) enforces it equals what `toLayout` reads, so adding a name
  to the lowering without the classifier (or vice-versa) is now **impossible to ship** — it fails the
  build naming the attribute. There is **zero** independent hand-maintained second list left free to
  drift.

The false single-sourcing comment at the old `Control.fs:1207` (and the mirroring note near
`layoutDirtySet` in `RetainedRender.fs`) was corrected to stop claiming the literal and `toLayout` are
auto-single-sourced and to point at the gate instead.
