// CONTRACT SKETCH (Phase 1) — layout containers + navigation + overlay. Children/
// content are `Widget<'msg>`, lowered via `Widget.toControl` with order preserved
// (the 065 Stack pattern). No `Widget.toControl`/`ofControl` appears in author code
// for composition (US1 acceptance #2).
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

// ---- Layout containers ----
type GridProps<'msg> = { Id: ControlId option; Rows: GridLength list; Columns: GridLength list; Children: Widget<'msg> list }
type DockProps<'msg> = { Id: ControlId option; Children: (DockSide * Widget<'msg>) list }
type WrapProps<'msg> = { Id: ControlId option; Orientation: Orientation; Spacing: float; Children: Widget<'msg> list }
type BorderProps<'msg> = { Id: ControlId option; Thickness: float; Padding: float; Child: Widget<'msg> }
type PanelProps<'msg> = { Id: ControlId option; Children: Widget<'msg> list }
type SplitViewProps<'msg> = { Id: ControlId option; Orientation: Orientation; Children: Widget<'msg> list; OnChanged: (float -> 'msg) option }
type ScrollViewerProps<'msg> = { Id: ControlId; Child: Widget<'msg>; OnChanged: (ScrollState -> 'msg) option }

module Grid =
    val defaults: GridProps<'msg>
    /// Lowers children via `Widget.toControl` into `Grid.create`, order preserved.
    val view: props: GridProps<'msg> -> Widget<'msg>

module Dock =
    val defaults: DockProps<'msg>
    val view: props: DockProps<'msg> -> Widget<'msg>

module Wrap =
    val defaults: WrapProps<'msg>
    val view: props: WrapProps<'msg> -> Widget<'msg>

module Border =
    val defaults: BorderProps<'msg>
    val view: props: BorderProps<'msg> -> Widget<'msg>

module Panel =
    val defaults: PanelProps<'msg>
    val view: props: PanelProps<'msg> -> Widget<'msg>

module SplitView =
    val defaults: SplitViewProps<'msg>
    val view: props: SplitViewProps<'msg> -> Widget<'msg>

module ScrollViewer =
    // If scroll runtime is owned by the Collections model the parity/interaction test
    // pins `init`/`update` delegation; otherwise this is pure Props -> Widget.
    val defaults: controlId: ControlId -> ScrollViewerProps<'msg>
    val view: props: ScrollViewerProps<'msg> -> Widget<'msg>

// ---- Navigation / composite ----
type TabsProps<'msg> = { Id: ControlId option; Items: TabItem<'msg> list; SelectedKey: string option; OnChanged: (string -> 'msg) option }
type MenuProps<'msg> = { Id: ControlId option; Items: MenuItem<'msg> list; OnSelected: (string -> 'msg) option }
type ContextMenuProps<'msg> = { Id: ControlId option; Items: MenuItem<'msg> list; OnSelected: (string -> 'msg) option }
type ToolbarProps<'msg> = { Id: ControlId option; Children: Widget<'msg> list; OnClick: 'msg option }

module Tabs =
    val defaults: TabsProps<'msg>
    val view: props: TabsProps<'msg> -> Widget<'msg>

module Menu =
    val defaults: MenuProps<'msg>
    val view: props: MenuProps<'msg> -> Widget<'msg>

module ContextMenu =
    val defaults: ContextMenuProps<'msg>
    val view: props: ContextMenuProps<'msg> -> Widget<'msg>

module Toolbar =
    val defaults: ToolbarProps<'msg>
    val view: props: ToolbarProps<'msg> -> Widget<'msg>

// ---- Overlay / transient ----
type TooltipProps<'msg> = { Id: ControlId option; Text: string; Target: Widget<'msg> option }
type DialogProps<'msg> = { Id: ControlId option; Title: string option; IsOpen: bool; Children: Widget<'msg> list; OnSelected: (string -> 'msg) option }
type ToastProps<'msg> = { Id: ControlId option; Text: string; Severity: ValidationState }
type OverlayProps<'msg> = { Id: ControlId option; IsOpen: bool; Child: Widget<'msg> }

module Tooltip =
    val defaults: TooltipProps<'msg>
    val view: props: TooltipProps<'msg> -> Widget<'msg>

module Dialog =
    val defaults: DialogProps<'msg>
    val view: props: DialogProps<'msg> -> Widget<'msg>

module Toast =
    val defaults: ToastProps<'msg>
    val view: props: ToastProps<'msg> -> Widget<'msg>

module Overlay =
    val defaults: OverlayProps<'msg>
    val view: props: OverlayProps<'msg> -> Widget<'msg>
