// CONTRACT SKETCH — feature 108 US5 (FR-014). src/Controls/Control.fsi + Widget.fsi edits.
namespace FS.Skia.UI.Controls

module Control =
    /// Change only the message type of a control. `Kind`, `Key`, `Content`,
    /// `Accessibility`, and the `Children` shape are preserved exactly; every
    /// `Attr<'a>` whose `AttrValue<'a>` carries a `'a`-bearing handler has that
    /// handler mapped through `f` to produce `'b`. The result lowers structurally
    /// equal to a control authored directly in `'b` (SC-007), so a page authored as a
    /// self-contained `Control<PageMsg>` module folds into a shell `Control<Msg>` via
    /// one `Control.map PageMsg`. Keys / focus identity survive (only the msg changes).
    val map: f: ('a -> 'b) -> control: Control<'a> -> Control<'b>

module Widget =
    /// `Widget.map f = ofControl ∘ Control.map f ∘ toControl`.
    val map: f: ('a -> 'b) -> widget: Widget<'a> -> Widget<'b>
