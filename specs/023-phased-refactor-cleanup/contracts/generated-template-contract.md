# Contract: Generated Template Refactor

## Generated Source Responsibilities

Generated product source may be split into responsibility-specific files:

- model and update state,
- rendering/view description,
- layout evidence,
- evidence commands and report writer,
- window options,
- entrypoint and command dispatch.

`Program.fs` remains the generated entrypoint and command dispatcher.

## Compile Order

`template/base/src/Product/Product.fsproj` must list generated files in F#
dependency order. Profile-specific files must be included only when the selected
generated profile needs them.

## Stable Generated Behavior

The split must preserve:

- supported generated profiles,
- command names and arguments,
- evidence report fields and status vocabulary,
- output paths and exit-code meanings,
- generated test expectations except for intentional source-shape assertions
  that now reflect the new file ownership.

## Validation

Run `TemplateCheck`, `GeneratedGuidanceCheck`, and `TemplateDrift` for the split
phase. Fresh generated products must instantiate, build, and pass their
generated tests for every profile supported before the cleanup.
