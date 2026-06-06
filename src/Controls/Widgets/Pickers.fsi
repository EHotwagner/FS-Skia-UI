namespace FS.Skia.UI.Controls.Typed

open System
open FS.Skia.UI.Controls
open FS.Skia.UI.Scene

/// One palette entry for a `ColorPicker`. `Color` is the reused
/// `FS.Skia.UI.Scene.Color` (no new dependency, no hex string); `Name` is the
/// accessible label and selection identity.
type ColorSwatch = { Name: string; Color: Color }

/// Immutable, compiler-checked authoring surface for a date entry with a popup
/// calendar. `Value` is a BCL `DateOnly` (never a string); `None` = no selection
/// (empty field). `IsOpen` is product-owned calendar visibility. `OnChange = None`
/// lowers to no binding.
type DatePickerProps<'msg> =
    { Id: ControlId option
      Value: DateOnly option
      Enabled: bool
      IsOpen: bool
      OnChange: (DateOnly -> 'msg) option }

/// Immutable, compiler-checked authoring surface for a time entry. `Value` is a BCL
/// `TimeOnly` (out-of-range time is unrepresentable); `None` = no selection.
/// `OnChange = None` lowers to no binding.
type TimePickerProps<'msg> =
    { Id: ControlId option
      Value: TimeOnly option
      Enabled: bool
      OnChange: (TimeOnly -> 'msg) option }

/// Immutable, compiler-checked authoring surface for a palette/swatch color picker.
/// `Swatches` is the required palette (empty ⇒ empty grid); `Selected` is the
/// highlighted swatch; `OnSelected = None` lowers to no binding.
type ColorPickerProps<'msg> =
    { Id: ControlId option
      Swatches: ColorSwatch list
      Selected: ColorSwatch option
      OnSelected: (ColorSwatch -> 'msg) option }

/// Public contract module exposed by this FS.Skia.UI package.
module DatePicker =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: DatePickerProps<'msg>
    /// Lowers to a legacy `Stack` of [ read-only field showing the formatted `Value`
    /// or a placeholder; trigger `Button`; an `Overlay` `Grid` of day `Button`s shown
    /// when `IsOpen` ]; `Value = None` ⇒ placeholder + empty calendar; `OnChange =
    /// None` ⇒ no binding.
    val view: props: DatePickerProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module TimePicker =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: TimePickerProps<'msg>
    /// Lowers to a legacy `Stack` of hour/minute segment `Button`s showing `Value`
    /// or a placeholder; `OnChange = None`/`Value = None` ⇒ no binding.
    val view: props: TimePickerProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module ColorPicker =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: ColorPickerProps<'msg>
    /// Lowers to a legacy `Wrap` of colored swatch `Button` cells (one per swatch),
    /// the `Selected` cell highlighted; empty `Swatches` ⇒ empty grid; `OnSelected =
    /// None` ⇒ no binding.
    val view: props: ColorPickerProps<'msg> -> Widget<'msg>
