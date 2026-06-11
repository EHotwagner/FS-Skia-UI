# De-duplication grep transcript (T008/T011) — SC-001 / SC-002

Each consolidated helper now has exactly one body, and every former call site references
the single shared home.

## SC-001 — one body per helper

```
$ grep -rn "let withKeyOpt" src/Controls/Widgets/
src/Controls/Widgets/WidgetLowering.fs:14:    let withKeyOpt id control =

$ grep -rn "let onString\b" src/Controls/Widgets/
src/Controls/Widgets/WidgetLowering.fs:20:    let onString (eventKind: string) (map: string -> 'msg) : Attr<'msg> =

$ grep -rn "let onStringList" src/Controls/Widgets/
src/Controls/Widgets/WidgetLowering.fs:24:    let onStringList (eventKind: string) (map: string list -> 'msg) : Attr<'msg> =

$ grep -rn "let a11y" src/Controls/Widgets/
src/Controls/Widgets/WidgetLowering.fs:33:    let a11y (role: AccessibilityRole) (nameSource: string) (navigationKeys: string list) : Attr<'msg> =
```

- `withKeyOpt`: 9 verbatim copies removed → 1 in `WidgetLowering`.
- `onString`: 4 copies removed → 1 in `WidgetLowering`.
- `onStringList`: 1 copy removed → 1 in `WidgetLowering`.
- `a11y` (FR-004): 2 near-identical copies removed → 1 in `WidgetLowering`.

5 per-family `*Lowering` modules became empty after the moves and were deleted
(`ButtonsLowering`, `NavigationLowering`, `PickersLowering`, `DisplayLowering`,
`OverlayLowering`); their `view`s now reference `WidgetLowering` directly. The 5 that kept
other members (`ChartLowering`, `CollectionLowering`, `ContainerLowering`, `InputLowering`,
`LegacyControls`) lost only the moved helpers.

## SC-002 — no inline float-parse lambda; one named `tryParseFloat`

```
$ grep -nE 'let onChanged map = Attr.onWith' src/Controls/Control.fs
(no matches — all 8 inline parsers collapsed)

$ grep -n "let onChanged " src/Controls/Control.fs
1626:    let onChanged map = ChangeAdapters.onChangedBool map
1631:    let onChanged map = ChangeAdapters.onChangedBool map
1636:    let onChanged map = ChangeAdapters.onChangedFloat map
1641:    let onChanged map = ChangeAdapters.onChangedFloat map
1648:    let onChanged map = ChangeAdapters.onChangedString map
1653:    let onChanged map = ChangeAdapters.onChangedString map
1659:    let onChanged map = ChangeAdapters.onChangedString map
1703:    let onChanged map = ChangeAdapters.onChangedString map

$ grep -n "Double.TryParse" src/Controls/Control.fs
1608:        match Double.TryParse value with        # the single use, inside tryParseFloat

$ grep -n "let tryParseFloat" src/Controls/Control.fs
1607:    let tryParseFloat (value: string) : float option =
```

The 8 inline `onChanged` parsers in `Control.fs` are replaced by references to
`ChangeAdapters.onChangedBool` / `onChangedFloat` / `onChangedString`, and the float shape
delegates to the single named `tryParseFloat` — the twice-duplicated 217-char nested
number-parse lambda is gone. `ChangeAdapters` is hidden from consumers by absence from
`Control.fsi` (no visibility qualifier, per Principle II).

## FR-004 partial — `intentStyle` consolidation deferred (surface-safety)

The `intentStyle` (`ButtonIntent -> string`) duplication in `Input.fs` + `Primitives.fs`
is **not** folded into `WidgetLowering`: `ButtonIntent` is a **public** type defined in
`Primitives.fsi`, which compiles **after** `WidgetLowering.fs`, so a shared `intentToString`
in `WidgetLowering` cannot see `ButtonIntent`, and relocating the type would move a
per-package surface baseline. FR-004 is a SHOULD bounded by the zero-surface banner
(FR-011/FR-012), so the two small copies remain. The `a11y` half of FR-004 (no such
ordering dependency) was consolidated. SC-001's enumerated set
(`withKeyOpt`/`onString`/`onStringList`/`onChanged`) is fully satisfied.
