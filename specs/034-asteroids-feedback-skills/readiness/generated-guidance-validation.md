# Generated Guidance Validation

command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter Asteroids`
scanned files: `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`, `.agents/skills/speckit-tasks/SKILL.md`, `template/base/README.md`, `template/base/docs/product.md`
observed: skill assignment terms, advisory-only status, readiness scaffolds, visual proof honesty, warning classification, and owner categories are present.
missing: none.
failure class: GeneratedGuidanceValidation.
next action: run `./fake.sh build -t GeneratedGuidanceCheck`, `./fake.sh build -t TemplateCheck`, and `./fake.sh build -t GeneratedProductCheck` sequentially after guidance edits.

| Term group | Observed terms | Advisory-only status |
|------------|----------------|----------------------|
| skills | scene rendering, screenshot capture, layout readability, generated-package validation, graph validation, audit validation | advisory hints unless a blocking trigger applies |
| readiness | authoritative command, artifact path, failure class, next action | required scaffold fields |
| warnings | benign warning, blocking warning, deferred warning | classified, not silently ignored |
