# Redundant-qualifier cleanup (T012/T013) — SC-003

US2 removes the in-source `private` keywords the `.fsi` (or enclosing `module internal`)
already enforces, keeping the explanatory comments and the load-bearing keep-list.

## Removed (redundant — the `.fsi` is the boundary)

- **`module private *Lowering` → `module`** across `src/Controls/Widgets/`. The audit cited
  10; US1 deleted 5 of them (they became empty after the helper moves), so the remaining 5 are
  de-privatized here: `ChartLowering`, `CollectionLowering`, `ContainerLowering`,
  `InputLowering`, `LegacyControls`. End state: `grep -rn "module private" src/Controls/Widgets/`
  → **0**. Each module's "Hidden by absence from `<X>.fsi`" comment is retained.
- **`Reconcile` `let private` → `let`** (3): `attrValueEqual`, `diffAttrs`, `isKeepOp`. The
  uncited `applyAttrChanges` is left `let private`.
- **`RetainedRender` `let private` → `let`** (4): `childPath`, `clockDuration`, `fadeAnimation`,
  `currentOpacity`. The uncited `fadeOutAnimation` and `firstFrameCollisions` are left
  `let private`.

Total redundant qualifiers retired: 5 module + 3 + 4 = 12 in-source `private` keywords (plus
the 5 `private` removed incidentally by deleting the emptied modules in US1), so all 10 cited
`module private *Lowering` sites and the 7 cited `let private` sites no longer carry a
redundant qualifier — the audit's ~17-site set.

## Keep-list (load-bearing — unchanged, FR-006)

```
$ grep -cn "module internal SceneRenderer" src/SkiaViewer/SceneRenderer.fs   # 1 (kept)
$ grep -n "let private" src/Controls/Reconcile.fs        # only applyAttrChanges (uncited)
$ grep -n "let private" src/Controls/RetainedRender.fs   # only fadeOutAnimation, firstFrameCollisions (uncited)
```

- `module internal SceneRenderer` (exhaustiveness guard) — untouched.
- The `InternalsVisibleTo` test seams (`Reconcile`, `RetainedRender`, `ControlInternals`,
  `ControlRuntime`, `ControlsElmish`) — untouched (the `internal` accessibility stays).
- The `let private` helpers inside the **exposed** `module internal ControlInternals` — untouched
  (55 `let private` remain in `Control.fs`, all inside the exposed `ControlInternals` /
  `slotRegions`).

## Verification

`dotnet build src/Controls/Controls.fsproj` succeeds (0 warnings, 0 errors); the de-privatized
helpers stay hidden via their `.fsi` (absent) — no encapsulation weakened, no behaviour change.
