# Contract: Governance Diagnostics

## Generated Guidance Check

`GeneratedGuidanceCheck` validates generated spec and plan templates semantically.

Required checks:

- required section headings exist in active and preset templates
- required prompts appear within the correct section
- prompts are not satisfied solely by deferred roadmap text
- active and preset templates have matching required prompt sets for each generated artifact class
- failures name the template path, section, prompt, and mismatch class

Required generated spec prompt classes:

- package impact
- public contract impact
- state workflow impact
- layout/rendering impact
- evidence obligations
- unsupported scope
- build-target impact

Required generated plan decision classes:

- template ownership
- dependency impact
- command-surface impact
- generated project impact
- evidence paths
- `.fsi` / contract impact
- MVU/effect boundary
- synthetic evidence
- test evidence
- observability
- deferred scope

## Template Drift

`TemplateDrift` validates template-owned path changes against required alignment evidence.

Required checks:

- classify changed template-owned paths by path class
- map each path class to required alignment classes
- verify same-diff alignment files are present
- verify active feature spec, plan, or readiness evidence names the changed path or affected feature area
- accept a deferral only when `readiness/template-deferrals.yml` contains id, paths, rationale, owner, and target phase
- failures name the changed path, path class, missing alignment class, and acceptable remediation

## Build Organization

`build.fsx` organization must preserve:

- one documented entry command for `Dev`, `Verify`, and `Ci`
- existing `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, and edge `interpret`
- target graph semantics for existing governance targets
- cross-platform load behavior for Windows and Linux

Physical split is accepted only when the documented targets load cross-platform. Otherwise, named sections in one canonical `build.fsx` are the required fallback.

## Evidence

- `specs/008-targeted-refactor-governance/readiness/generated-guidance.md`
- `specs/008-targeted-refactor-governance/readiness/template-drift.md`
- `specs/008-targeted-refactor-governance/readiness/build-organization.md`
