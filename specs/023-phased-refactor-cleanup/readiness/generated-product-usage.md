# Generated Product Usage

status=ok

Generated product user-facing usage is preserved while ownership is split into
responsibility-specific source files.

Stable generated contracts:

- Command names remain unchanged.
- Report fields, status vocabulary, output paths, stdout echo behavior, parent
  directory creation, and exit-code meanings remain unchanged.
- Generated profile names and package IDs remain unchanged.
- `Program.fs` is reduced to launch and command dispatch only after generated
  model, view, layout evidence, evidence commands, and window options move to
  their generated files.

Validation path:

- `TemplateCheck`, `GeneratedGuidanceCheck`, and `TemplateDrift` remain the
  generated product gates.
- Phase evidence is recorded in `generated-evidence-cleanup.md` and
  `template-split-validation.md`.
