# Package-surface expectations — typed controls front door (065)

Feature-specific surface delta for the `package-surface` /
`controls-public-surface` rules. The change is **additive-only** to the single
shipped `FS.Skia.UI.Controls` package; no other package baseline changes.

## Regeneration

- Command: `./fake.sh build -t RefreshSurfaceBaselines` (T020).
- Reviewed gate: `PackageSurfaceCheck` (aggregate,
  `readiness/surface-baselines/FS.Skia.UI.Controls.txt`) and
  `PerPackageSurfaceDiff`.

## Intentional additive delta (18 added, 0 removed)

New public types/modules in `readiness/surface-baselines/FS.Skia.UI.Controls.txt`:

- `FS.Skia.UI.Controls.Widget` (module) and `FS.Skia.UI.Controls.Widget`1` (the
  sealed opaque type) — the lowering seam (`ofControl`/`toControl`/`render`).
- `FS.Skia.UI.Controls.Typed.TextBlock` + `TextBlockProps`1`
- `FS.Skia.UI.Controls.Typed.Button` + `ButtonProps`1` + `ButtonIntent` (+`Tags`)
- `FS.Skia.UI.Controls.Typed.CheckBox` + `CheckBoxProps`1`
- `FS.Skia.UI.Controls.Typed.Stack` + `StackProps`1` + `StackOrientation` (+`Tags`)
- `FS.Skia.UI.Controls.Typed.TextBox` + `TextBoxProps`1`
- `FS.Skia.UI.Controls.Typed.DataGrid` + `DataGridProps`1`

## Why the diff is safe

- **Zero removed lines** — every existing legacy module/type/member is byte-stable
  (the legacy string-keyed API is frozen, FR-007). The seven other shipped
  split-package baselines are unchanged.
- The internal `{ Lowered: Control<'msg> }` representation of `Widget<'msg>` stays
  in `Widget.fs`; it is **not** on the `.fsi`, so the baseline exposes only the
  opaque sealed type plus the `Widget` module functions (Principle II).
- No `Props` field is `obj` or a string-named event (FR-005); the typed events are
  `'msg option` / `(T -> 'msg) option`. Asserted by the `.fsi`-grep guard in
  `TypedControlContractTests.fs`.
- No new dependency (FR-011): `Controls.fsproj` adds no `PackageReference` and no
  `Fable.Elmish` reference, so no package's dependency surface changes.
