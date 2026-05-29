# Contract: Default Text Glyph Capture

## Scope

Default text nodes used for HUD labels, status messages, and instructions must be readable in generated screenshot evidence on supported Linux desktop hosts with common Latin fonts.

## Required Behavior

- Render `Scene.text` and `Scene.textRun` screenshot evidence with glyph-shaped coverage, not solid rectangles.
- Prefer native Skia/default typeface rendering when glyphs are available.
- Use deterministic vector fallback or an equivalent readable fallback when native glyph lookup fails.
- Preserve explicit font support for authors who need brand or typography guarantees.

## Required Capability Check

The validation check must write or reference:

- command
- host platform/session facts
- screenshot path
- expected text bounds or probe region
- glyph coverage metric
- solid-block or placeholder detection result
- fallback used, if any
- normalized status: `ok`, `unsupported`, or `failed`
- diagnostic message and next action

## Pass Conditions

- The screenshot is decodable.
- The expected text region contains visible glyph-shaped variation.
- The result is not only a filled rectangle, metadata hash, placeholder box, or deterministic-scene-only report.
- Unsupported hosts are classified without claiming screenshot glyph proof.
