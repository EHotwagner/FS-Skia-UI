# Compatibility Impact

Status: compatible with documented package and generated-template changes.

The generated app path remains source-compatible while gaining real persistent window presentation through `FS.Skia.UI.SkiaViewer`. The package boundary now intentionally includes repo-owned presenter functionality from `FS.Skia.UI`.

Compatibility notes:

- Existing generated default launch remains the default interactive command.
- Evidence modes remain explicit commands or flags.
- Existing governed package versions remain exact for the generated consumer.
- The template retains disclosed compatibility fallback code for pre-change package validation; packed generated validation exercises the real package path.

Evidence:

- TemplateCheck: `readiness/logs/t047-retry-template-check-presenter-tests.txt`
- GeneratedProductCheck: `readiness/logs/t047-retry-generated-product-check-presenter-tests.txt`
- Dependency governance: `readiness/dependency-governance.md`
