namespace FS.Skia.UI.Controls

/// Public contract module exposed by this FS.Skia.UI package.
module Attr =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: name: string -> category: AttrCategory -> value: AttrValue<'msg> -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val standardAttribute: name: StandardAttributeName -> value: StandardAttributeValue<'msg> -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val customAttribute: name: string -> value: obj -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val standardEvent: eventKind: StandardEventKind -> msg: 'msg -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val customEvent: eventKind: string -> msg: 'msg -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: value: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val value: value: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val items: values: string list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val child: control: Control<'msg> -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: controls: Control<'msg> list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val enabled: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val visible: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val readOnly: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val loading: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val selected: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val width: value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val height: value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val padding: value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val margin: value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val style: name: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val theme: theme: Theme -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val validation: state: ValidationState -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val accessibility: metadata: AccessibilityMetadata -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val on: eventKind: string -> msg: 'msg -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onWith: eventKind: string -> map: (ControlEvent -> 'msg) -> Attr<'msg>
