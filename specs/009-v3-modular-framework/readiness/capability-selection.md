# Capability Selection

## Resolver

Capability closure is resolved from `template/capabilities.yml` by including
the selected capability and every declared dependency. The current validation
matrix covers:

| Row | Requested capabilities | Resolved prerequisites |
|-----|------------------------|------------------------|
| `app-source` | Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Charts | Scene prerequisite included for all dependent capabilities; SkiaViewer included for Elmish. |
| `app-package` | Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Charts | Same as `app-source`, generated from the archived template payload. |
| `headless-scene-source` | Scene | No viewer, Elmish, keyboard input, layout, charts, or samples. |
| `governed-source` | Scene, Testing | Scene and Testing with full product governance. |
| `sample-pack-source` | Samples | Scene, SkiaViewer, and Elmish prerequisites plus sample content. |

## Output Messages

Generated file-list reports include selected package references and selected
skills for every matrix row under
`specs/009-v3-modular-framework/readiness/generated-file-lists/`.

Samples are excluded from `app`, `headless-scene`, and `governed` rows and are
included only for `sample-pack`.
