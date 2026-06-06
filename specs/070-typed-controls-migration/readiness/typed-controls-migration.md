# Typed controls migration (070) — design and evidence

## What this feature delivers

The typed front door started in `065` (six controls) is completed: the **41
remaining** catalog controls each gain an additive, compiler-checked authoring
surface under `FS.Skia.UI.Controls.Typed.*` — an immutable `Props` record,
`defaults`, and a `view` returning `Widget<'msg>` (plus typed `init`/`update` for
stateful controls). The legacy string-keyed API is **byte-frozen**; the public
surface delta is additive-only.

## Mechanic grouping (nine new `Widgets/*` files)

| Group | File | Controls |
| --- | --- | --- |
| Display (pure) | `Display.fs[i]` | rich-text, label, image, icon, separator, badge, progress-bar, spinner, validation-message |
| Input (one optional event) | `Input.fs[i]` | icon-button, numeric-input, radio-group, switch, slider |
| Stateful text (TextInput) | `TextAreaWidget.fs[i]` | text-area |
| Selection collections (Collections model) | `CollectionsWidgets.fs[i]` | list-view, list-box, multi-select-list, combo-box, tree-view |
| Layout containers (pure) | `Containers.fs[i]` | grid, dock, wrap, border, panel, scroll-viewer, split-view |
| Navigation / composite | `Navigation.fs[i]` | tabs, menu, context-menu, toolbar |
| Overlay / transient | `Overlay.fs[i]` | tooltip, dialog, toast, overlay |
| Charts / graph (pure) | `ChartsWidgets.fs[i]` | line-chart, bar-chart, pie-chart, scatter-plot, graph-view |
| Escape hatch (bridge) | `CustomControlWidget.fs[i]` | custom-control (via `Widget.ofControl`) |

## Per-control taxonomy field choices

Uniform with `065`: each catalog `requiredAttribute` (PascalCased) is a
**non-optional** `Props` field; every other value is optional and resolves through
`defaults`; every catalog event is an **optional callback** that lowers to **no
binding** when `None`. No field is `obj`, untyped, or a string-named event
(FR-003/SC-005), enforced by a grep guard over the nine new `.fsi` files.

Where the `data-model.md` sketch named a richer type that does not exist in the
Controls package (`RadioItem`, `TabItem`, `MenuItem`, `TreeNode`, `GridLength`,
`DockSide`, `ImageSource`, `ScrollState`, `GraphNode`/`GraphEdge`, `Orientation`),
the migration follows the data-model's explicit override — *"reuse the existing
legacy types where they already exist… the parity test pins the exact lowering and
is the source of truth"* — and uses the real legacy type the builder accepts
(`string list`, `string`, `float`, `ChartSeries list`, `StackOrientation`, …), so
every field has a real, parity-provable lowering and no fabricated schema ships.

## Lowering is real — zero `[S]`

Every typed `view` calls the exact legacy `*.create`/`Attr` builder (or
`Control.standard (Custom <id>)` for the collections / context-menu / scroll-viewer
/ split-view ids that have no dedicated builder) and lifts the result through
`Widget.ofControl`. The lowered `Control<'msg>` is therefore structurally equal to
the legacy authoring call **by construction**, proven per control by the
41-row parity matrix ([typed-lowering-parity.md](./typed-lowering-parity.md)).
**There is no synthetic evidence in this feature** — the Synthetic-Evidence
Inventory in `tasks.md` is empty and the intended `EvidenceAudit` verdict is PASS
with zero `[S]`/`[S*]`.

## Stateful delegation (no forks)

`text-area` and the five selection collections reuse the existing pure models and
return the existing `Model`/`Msg`/`Effect` types. The delegation-equality tests
assert each typed `update` result equals the reused model's `update` for the same
input, and `init` equals the reused model's `init`. No parallel model type is
introduced.
