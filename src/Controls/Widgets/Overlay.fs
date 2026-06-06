namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

type TooltipProps<'msg> =
    { Id: ControlId option
      Text: string }

type DialogProps<'msg> =
    { Id: ControlId option
      Title: string option
      IsOpen: bool
      Children: Widget<'msg> list
      OnSelected: (string -> 'msg) option }

type ToastProps<'msg> =
    { Id: ControlId option
      Text: string
      Severity: ValidationState }

type OverlayProps<'msg> =
    { Id: ControlId option
      IsOpen: bool
      Child: Widget<'msg> }

// File-private lowering helpers — hidden by absence from Overlay.fsi.
module private OverlayLowering =
    let withKeyOpt id control =
        match id with
        | Some key -> FS.Skia.UI.Controls.Control.withKey key control
        | None -> control

    let onString (eventKind: string) (map: string -> 'msg) : Attr<'msg> =
        Attr.onWith eventKind (fun event -> event.Payload |> Option.defaultValue "" |> map)

module Tooltip =
    let defaults: TooltipProps<'msg> = { Id = None; Text = "" }

    let view (props: TooltipProps<'msg>) : Widget<'msg> =
        FS.Skia.UI.Controls.Tooltip.create [ FS.Skia.UI.Controls.Tooltip.text props.Text ]
        |> OverlayLowering.withKeyOpt props.Id
        |> Widget.ofControl

module Dialog =
    let defaults: DialogProps<'msg> =
        { Id = None
          Title = None
          IsOpen = false
          Children = []
          OnSelected = None }

    let view (props: DialogProps<'msg>) : Widget<'msg> =
        let children = props.Children |> List.map Widget.toControl

        let attrs =
            [ yield FS.Skia.UI.Controls.Dialog.children children
              match props.Title with
              | Some title -> yield Attr.create "title" Content (TextValue title)
              | None -> ()
              yield Attr.selected props.IsOpen
              match props.OnSelected with
              | Some map -> yield OverlayLowering.onString "onSelected" map
              | None -> () ]

        FS.Skia.UI.Controls.Dialog.create attrs
        |> OverlayLowering.withKeyOpt props.Id
        |> Widget.ofControl

module Toast =
    let defaults: ToastProps<'msg> =
        { Id = None; Text = ""; Severity = Valid }

    let view (props: ToastProps<'msg>) : Widget<'msg> =
        FS.Skia.UI.Controls.Toast.create
            [ FS.Skia.UI.Controls.Toast.text props.Text
              Attr.validation props.Severity ]
        |> OverlayLowering.withKeyOpt props.Id
        |> Widget.ofControl

module Overlay =
    let defaults (child: Widget<'msg>) : OverlayProps<'msg> =
        { Id = None; IsOpen = false; Child = child }

    let view (props: OverlayProps<'msg>) : Widget<'msg> =
        let attrs =
            [ FS.Skia.UI.Controls.Overlay.child (Widget.toControl props.Child)
              Attr.selected props.IsOpen ]

        FS.Skia.UI.Controls.Overlay.create attrs
        |> OverlayLowering.withKeyOpt props.Id
        |> Widget.ofControl
