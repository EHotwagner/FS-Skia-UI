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
    /// Feature 093 (E3): attach an ordered list of style classes (list order = attach order).
    /// Lowers to a single `Style`-category attribute carrying `StyleClassesValue`. Absent ≡
    /// `[]` ≡ the behaviour-preserving base case (FR-005). The last `styleClasses` attribute on
    /// a control wins (the codebase's last-writer attribute convention).
    val styleClasses: classes: StyleClass list -> Attr<'msg>
    /// Feature 093 (E3): set the control's current `VisualState` for the resolver. A host wires
    /// its `ControlRuntime` Hover/Press/Focus state into this each frame; it rides the control
    /// through the keyed reconciler so a state-driven look survives a sibling shift (FR-006,
    /// SC-005). Absent ≡ `Normal` ≡ the behaviour-preserving base case.
    val visualState: state: VisualState -> Attr<'msg>
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
