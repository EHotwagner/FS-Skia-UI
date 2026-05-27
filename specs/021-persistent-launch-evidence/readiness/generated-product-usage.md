# Generated Product Usage

Feature: `021-persistent-launch-evidence`

Generated products preserve normal persistent interactive launch as the default
path and expose readiness evidence through explicit commands.

- default launch: `Viewer.runAppWithWindowBehavior viewerOptions windowBehavior generatedHost`
- persistent launch evidence: `--launch-evidence <path>`
- image/screenshot/readback evidence: explicit visual evidence commands only
- app-owned names in guidance: `Product.Program.view`,
  `Product.Program.generatedHost`, `Product.Program.update`

Validation:

- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t TemplateCheck`

