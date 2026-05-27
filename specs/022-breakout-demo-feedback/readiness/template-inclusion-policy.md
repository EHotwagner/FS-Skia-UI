# Template Inclusion Policy

Task: T007

Skill: `fs-skia-template-update` loaded from `.agents/skills/fs-skia-template-update/SKILL.md`.

Result: PASS.

Inspected files:

- `.template.config/template.json`
- `template/base/src/Product/Program.fs`
- `template/base/tests/Product.Tests/Tests.fs`
- `template/base/docs/product.md`
- `template/fragments/*/README.md`

Findings:

- `.template.config/template.json` copies `template/base/` to the generated project and excludes `bin`/`obj` outputs.
- `.specify/`, `.agents/skills/`, `.template.config/generated/`, and profile-specific capability skills are included through existing source mappings.
- The feature's planned generated changes are edits to existing generated source, tests, docs, and fragment README files. No new generated file or renamed generated file is required at this point.
- If later implementation adds a new generated evidence command file, readiness artifact, or generated helper source file outside `template/base/`, this inspection must be revisited before T044.

