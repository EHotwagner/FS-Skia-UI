# Controls rendering parity — typed views (065)

Proves T017 / `ControlsRenderingCheck`: typed views produce **no visual or
accessibility diff** versus the legacy IR, because they lower to the same
`Control<'msg>` and reuse the existing render path byte-for-byte. Evidence:
`tests/Controls.Tests/RenderingTests.fs` (test "typed views render byte-for-byte
identical to legacy IR at multiple viewports").

## Render-readback hashes (typed ≡ legacy, ≥2 viewports)

A `Stack[ TextBlock "Catalog"; Button "Save" ]` authored through the typed front
door versus the equivalent legacy builder tree, rendered with `Theme.light` and
hashed via `Scene.renderReadbackEvidence`:

| Viewport | typed nodes | legacy nodes | deterministic hash (both) | match | typed diagnostics |
| --- | --- | --- | --- | --- | --- |
| 320×240 | 3 | 3 | `30209a93a4d96175866a082bda2e3a4ef1985ea8b787937ff466465033077404` | ✅ | 0 |
| 1024×768 | 3 | 3 | `92540c6bac89af150cc2bfa93483fd7cd13fe1c115ef3a7fa764301799d3bae3` | ✅ | 0 |

The typed and legacy deterministic hashes are **identical** at each viewport, and
the typed render emits **zero** diagnostics — the same render path is reused, so
there is no visual diff.

## Accessibility parity

The lowered typed root's `Accessibility` metadata equals the legacy-built root's
(asserted in the same test). Because typed views lower to the identical IR,
`Control.diagnostics` / accessibility validation behave identically — no new a11y
diff and no new diagnostics path is introduced.

## Scope note

Render proof for this feature is **headless** (`renderReadbackEvidence`
deterministic hashes), matching the `DeterministicRenderOnly` evidence level —
this is a library-surface feature, not a generated game-layout readability claim.
The gallery typed-authoring panel (T019) demonstrates authoring from the sample's
default executable path and is **not** claimed as interactive graphical readiness.
