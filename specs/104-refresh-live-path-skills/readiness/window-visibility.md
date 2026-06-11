# Window visibility — applicability decision (feature 104, T002/T022)

status=not-applicable
mode=render-only
window-obligation=none

## Visible decision

The persistent-launch / viewer-launch task-generation rule does **not** apply to feature 104. The
change adds and edits **no** default-executable, persistent-launch, or graphical entry point and
changes **no** observable rendering output — every edit is skill-documentation Markdown (the
`.agents/skills/fs-skia-reconciliation` refresh, the `src/Controls/skill` E3/E4 edit, the NEW
`.agents/skills/fs-skia-controls-host`) plus the governance-generated `.claude/skills/**` mirror and
`skillist-reference.md`. There is no window, no screenshot, and no desktop-visibility claim.
Rendering output is byte-identical to pre-104.

The full window-visibility evidence set records this not-applicable decision with honest values:

- [interactive-visible-window.md](./interactive-visible-window.md) — status=not-applicable, mode=render-only
- [close-reason-separation.md](./close-reason-separation.md) — no window close to classify
- [window-state-diagnostics.md](./window-state-diagnostics.md) — every diagnostic-class not-applicable
- [window-options.md](./window-options.md) — every option not-applicable
- [real-image-evidence.md](./real-image-evidence.md) — no image produced; rendering output byte-identical to pre-104
- [generated-validation.md](./generated-validation.md) — nothing ships into the template/generated products

No live desktop window is involved at any point — feature 104 is a documentation-currency
(skill-honesty) pass.
