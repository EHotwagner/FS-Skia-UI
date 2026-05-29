# Template Ownership Inspection

Task: T005
Captured: 2026-05-29T11:46:09+02:00

## Loaded Skill

- `fs-skia-template-update`: `/home/developer/projects/FS-Skia-UI/.agents/skills/fs-skia-template-update/SKILL.md`

## Ownership Points

- `template/base/build.fsx`: generated build targets and command workflow.
- `template/base/src/Product/EvidenceCommands.fs`: generated evidence commands and report writing.
- `template/fragments/*/README.md`: generated guidance fragments.
- `template/base/.template.config/template.json`: generated file inclusion and profile behavior.
- `template/base/Directory.Packages.props`: package pins if framework package versions change.
- `.template.package/FS.Skia.UI.Template.fsproj`: template package version if the template package is repacked.

## Validation Decision

Combined template validation is required if generated command behavior, generated app source, generated guidance, or template files change. Package-only validation is acceptable only if implementation touches framework packages without changing generated output; that exception must be documented in `readiness/template-drift.md` or the final TemplateDrift evidence.

Expected generated validation targets:

- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t TemplateDrift`
