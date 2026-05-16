# Contract: Local Agent Skills

## Purpose

Capability skills guide AI-assisted work in generated products and framework
packages. Skills are selected by capability, not copied wholesale.

## Capability Skill Required Sections

Each capability skill must include:

- scope and owned files or product areas
- public contract and surface baseline guidance
- build commands
- test commands
- evidence and readiness expectations
- package boundary guidance
- generated product considerations, when applicable

## Generated Product Skill Rules

The default app profile must copy:

- project-level generated product skill
- Scene skill
- SkiaViewer skill
- Elmish skill
- KeyboardInput skill
- Layout skill
- Charts skill

It must not copy:

- sample-pack skill unless sample profile is selected
- framework-maintenance-only skills
- skills for unselected optional capabilities

## Validation Contract

`SkillCheck` must fail when:

- a capability has no skill path
- the skill file is missing
- required sections are absent
- generated product output misses a selected skill
- generated product output contains unrelated skills
- a skill names verification commands that do not exist in the relevant product
  or framework command surface

Failures must name the capability, skill path, missing section, or unexpected
generated destination.
