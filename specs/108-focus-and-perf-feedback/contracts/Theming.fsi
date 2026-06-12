// CONTRACT SKETCH — feature 108 US6 (FR-017/018). New src/Controls/Theming.fsi.
// Placement decision D8: lives in Controls (where `Theme` is), NOT SkillSupport.
namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene

/// The role colours `toTheme` projects onto the framework `Theme`. Small record;
/// final field set finalised at implementation against `Theme`'s fields.
type RolePalette =
    { Background: Color
      Foreground: Color
      Accent: Color
      Danger: Color
      Muted: Color
      FocusRing: Color }

module Theming =
    /// theme mode + accent -> a resolved role palette (the live-theming primitive
    /// the ControlsShowcase3 author re-derived by hand).
    val resolve: mode: FS.Skia.UI.Color.Palettes.RampVariant -> accent: Color -> RolePalette

    /// Project a role palette onto the framework `Theme` — the "paint theme" passed
    /// to the render path (Control.renderTree) so the captured palette is EXACT, while
    /// the consumer keeps a static `host.Theme` for the fragment-reuse key (FR-018).
    val toTheme: palette: RolePalette -> Theme

// WCAG contrast is REUSED, not re-implemented:
//   FS.Skia.UI.Color.Contrast.ratio : Color -> Color -> float   (relative-luminance)
//   FS.Skia.UI.Color.Contrast.check : Role -> bg -> fg -> ContrastResult (AA/AAA verdict)
// SC-010: `Contrast.ratio` matches the WCAG reference for known pairs; AA thresholds
// (>=4.5:1 normal, >=3:1 large) are checkable via `Contrast.verdict`/`check`.
