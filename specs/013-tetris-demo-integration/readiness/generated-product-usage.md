# Generated Product Usage Readiness

## Scope

Generated graphical consumers must expose user-reachable input, smoke, and
scene-evidence paths through public packages.

## Evidence

- `./fake.sh build -t TemplateCheck` passed.
- `./fake.sh build -t GeneratedGuidanceCheck` passed.
- `./fake.sh build -t TemplateDrift` passed.
- `./fake.sh build -t GeneratedProductCheck` passed.
- Generated flow and validation details are recorded in:
  - `readiness/generated-template-input-flows.md`
  - `readiness/generated-consumer-validation.md`
  - `readiness/generated-product-validation.md`
  - `readiness/generated-product-verify/**`

## Result

Generated products cover normalized viewer-key start/options/interaction,
pause/back where generated, restart/exit, bounded smoke, deterministic scene
evidence, diagnostics, and local package restore setup.
