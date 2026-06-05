namespace FS.Skia.UI.Controls

type Widget<'msg> = { Lowered: Control<'msg> }

module Widget =
    let ofControl (control: Control<'msg>) : Widget<'msg> = { Lowered = control }

    let toControl (widget: Widget<'msg>) : Control<'msg> = widget.Lowered

    let render (theme: Theme) (widget: Widget<'msg>) : ControlRenderResult<'msg> =
        Control.render theme widget.Lowered
