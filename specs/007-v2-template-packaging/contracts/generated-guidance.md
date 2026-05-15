# Contract: Generated Spec and Plan Guidance

This contract defines required governance prompts in generated Spec Kit artifacts.

## Spec Template Requirements

Generated specifications must ask contributors to identify:

- package impact,
- public contract impact,
- state workflow impact,
- layout and rendering impact,
- evidence obligations,
- unsupported or deferred scope,
- build-target impact.

The prompt set must appear in the active project template and the preset-owned template inherited by generated products.

Required paths:

```text
.specify/templates/spec-template.md
.specify/presets/fsharp-opinionated/templates/spec-template.md
```

## Plan Template Requirements

Generated implementation plans must require decisions for:

- template ownership,
- dependency impact,
- command-surface impact,
- generated project impact,
- evidence paths.

Plans must also carry constitution checks for `.fsi`/public contract impact, MVU/effect boundary applicability, synthetic evidence disclosure, test evidence, observability, and deferred scope.

Required paths:

```text
.specify/templates/plan-template.md
.specify/presets/fsharp-opinionated/templates/plan-template.md
```

## Required Target

`./fake.sh build -t GeneratedGuidanceCheck`

The target must:

- inspect active project templates and preset-owned templates,
- fail when required prompt text is missing,
- verify generated docs distinguish V2 obligations from deferred visual, release, and external distribution work,
- write `specs/007-v2-template-packaging/readiness/generated-guidance.md`.

## Pass Criteria

- Every required spec prompt is present in generated spec guidance.
- Every required plan prompt is present in generated plan guidance.
- The generated artifacts do not require manual copying from historical feature directories.
- Deferred roadmap boundaries are visible in generated guidance.
