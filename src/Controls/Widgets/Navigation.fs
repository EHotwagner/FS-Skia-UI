namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

type TabsProps<'msg> =
    { Id: ControlId option
      Items: string list
      SelectedKey: string option
      OnChanged: (string -> 'msg) option }

type MenuProps<'msg> =
    { Id: ControlId option
      Items: string list
      OnSelected: (string -> 'msg) option }

type ContextMenuProps<'msg> =
    { Id: ControlId option
      Items: string list
      OnSelected: (string -> 'msg) option }

type ToolbarProps<'msg> =
    { Id: ControlId option
      Children: Widget<'msg> list
      OnClick: 'msg option }

// File-private lowering helpers. `menu` and `context-menu` are distinct per-id
// modules over the same legacy menu mechanic; `context-menu` lowers to its own
// kind via `Control.standard (Custom "context-menu")`. Hidden by absence from
// Navigation.fsi.
module private NavigationLowering =
    let withKeyOpt id control =
        match id with
        | Some key -> FS.Skia.UI.Controls.Control.withKey key control
        | None -> control

    let onString (eventKind: string) (map: string -> 'msg) : Attr<'msg> =
        Attr.onWith eventKind (fun event -> event.Payload |> Option.defaultValue "" |> map)

module Tabs =
    let defaults: TabsProps<'msg> =
        { Id = None; Items = []; SelectedKey = None; OnChanged = None }

    let view (props: TabsProps<'msg>) : Widget<'msg> =
        let attrs =
            [ yield FS.Skia.UI.Controls.Tabs.items props.Items
              match props.SelectedKey with
              | Some key -> yield FS.Skia.UI.Controls.Tabs.selected key
              | None -> ()
              match props.OnChanged with
              | Some map -> yield FS.Skia.UI.Controls.Tabs.onChanged map
              | None -> () ]

        FS.Skia.UI.Controls.Tabs.create attrs
        |> NavigationLowering.withKeyOpt props.Id
        |> Widget.ofControl

module Menu =
    let defaults: MenuProps<'msg> = { Id = None; Items = []; OnSelected = None }

    let view (props: MenuProps<'msg>) : Widget<'msg> =
        let attrs =
            [ yield FS.Skia.UI.Controls.Menu.items props.Items
              match props.OnSelected with
              | Some map -> yield FS.Skia.UI.Controls.Menu.onSelected map
              | None -> () ]

        FS.Skia.UI.Controls.Menu.create attrs
        |> NavigationLowering.withKeyOpt props.Id
        |> Widget.ofControl

module ContextMenu =
    let defaults: ContextMenuProps<'msg> = { Id = None; Items = []; OnSelected = None }

    let view (props: ContextMenuProps<'msg>) : Widget<'msg> =
        let attrs =
            [ yield Attr.items props.Items
              match props.OnSelected with
              | Some map -> yield NavigationLowering.onString "onSelected" map
              | None -> () ]

        Control.standard (StandardControlKind.Custom "context-menu") attrs
        |> NavigationLowering.withKeyOpt props.Id
        |> Widget.ofControl

module Toolbar =
    let defaults: ToolbarProps<'msg> = { Id = None; Children = []; OnClick = None }

    let view (props: ToolbarProps<'msg>) : Widget<'msg> =
        let children = props.Children |> List.map Widget.toControl

        let attrs =
            [ yield FS.Skia.UI.Controls.Toolbar.children children
              match props.OnClick with
              | Some msg -> yield Attr.on "onClick" msg
              | None -> () ]

        FS.Skia.UI.Controls.Toolbar.create attrs
        |> NavigationLowering.withKeyOpt props.Id
        |> Widget.ofControl
