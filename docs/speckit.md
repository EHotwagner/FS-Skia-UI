# Spec Kit Governance

Generated specs and implementation plans must carry the same governance
questions that this repository expects from contributors.

## Specification Prompts

Spec templates ask for package impact, public contract impact, state workflow
impact, layout/rendering impact, evidence obligations, unsupported scope, and
build-target impact before implementation planning begins.

## Planning Prompts

Plan templates require decisions for template ownership, dependency impact,
command-surface impact, generated project impact, evidence paths, `.fsi` or
contract impact, MVU/effect boundary applicability, synthetic evidence, test
evidence, observability, and deferred scope.

## Preset Inheritance

The active `.specify/templates/` files and the
`.specify/presets/fsharp-opinionated/templates/` overrides must stay aligned so
new generated products inherit the same governance prompts without manual
copying from historical feature directories.

## Deferred Roadmap

Generated guidance validation is section-aware: prompts must appear in the
expected governance section, cannot be satisfied solely by deferred roadmap
text, and active templates must remain aligned with the F# preset templates.

Generated artifacts distinguish current V2 obligations from deferred visual
evidence, release validation, an external repository split, and distribution
automation.

Deferred visual evidence remains outside V2 pass/fail validation.
Deferred distribution automation remains outside V2 pass/fail validation.
