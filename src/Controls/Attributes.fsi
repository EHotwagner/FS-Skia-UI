namespace FS.Skia.UI.Controls

module Attr =
    val create: name: string -> category: AttrCategory -> value: AttrValue<'msg> -> Attr<'msg>
    val standardAttribute: name: StandardAttributeName -> value: StandardAttributeValue<'msg> -> Attr<'msg>
    val customAttribute: name: string -> value: obj -> Attr<'msg>
    val standardEvent: eventKind: StandardEventKind -> msg: 'msg -> Attr<'msg>
    val customEvent: eventKind: string -> msg: 'msg -> Attr<'msg>
    val text: value: string -> Attr<'msg>
    val value: value: string -> Attr<'msg>
    val items: values: string list -> Attr<'msg>
    val child: control: Control<'msg> -> Attr<'msg>
    val children: controls: Control<'msg> list -> Attr<'msg>
    val enabled: value: bool -> Attr<'msg>
    val visible: value: bool -> Attr<'msg>
    val readOnly: value: bool -> Attr<'msg>
    val loading: value: bool -> Attr<'msg>
    val selected: value: bool -> Attr<'msg>
    val width: value: float -> Attr<'msg>
    val height: value: float -> Attr<'msg>
    val padding: value: float -> Attr<'msg>
    val margin: value: float -> Attr<'msg>
    val style: name: string -> Attr<'msg>
    val theme: theme: Theme -> Attr<'msg>
    val validation: state: ValidationState -> Attr<'msg>
    val accessibility: metadata: AccessibilityMetadata -> Attr<'msg>
    val on: eventKind: string -> msg: 'msg -> Attr<'msg>
    val onWith: eventKind: string -> map: (ControlEvent -> 'msg) -> Attr<'msg>
