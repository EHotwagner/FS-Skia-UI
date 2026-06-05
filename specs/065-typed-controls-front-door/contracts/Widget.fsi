// CONTRACT SKETCH (Phase 1) — proposed src/Controls/Widget.fsi
// The sealed lowering seam. Lives in the base FS.Skia.UI.Controls namespace
// because render + the Elmish adapter consume it. Additive-only; nothing in the
// existing public surface changes.
namespace FS.Skia.UI.Controls

/// Opaque public return type of every typed `view`. Wraps the lowered
/// `Control<'msg>` IR plus room for later reconciliation metadata. The internal
/// representation (`{ Lowered: Control<'msg> }`) stays in the `.fs` — Principle II.
[<Sealed>]
type Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Widget =
    /// Migration bridge: lift a legacy `Control<'msg>` into the typed tree
    /// (e.g. to drop it into a typed `Stack.Children`). FR-002.
    val ofControl: control: Control<'msg> -> Widget<'msg>
    /// Lowering accessor — the single, explicit seam to the existing IR. Used by
    /// render and the Elmish adapter. FR-002. Invariant: toControl (ofControl c) = c.
    val toControl: widget: Widget<'msg> -> Control<'msg>
    /// Convenience = `Control.render theme (toControl widget)`. FR-009.
    val render: theme: Theme -> widget: Widget<'msg> -> ControlRenderResult<'msg>
