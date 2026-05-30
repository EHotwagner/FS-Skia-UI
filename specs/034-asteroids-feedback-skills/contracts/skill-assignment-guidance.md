# Contract: Skill Assignment Guidance

## Scope

Generated FS.Skia.UI implementation tasks expose relevant local skills before work starts.

## Required Behavior

- Every generated task includes structured `skillist` metadata in `tasks.deps.yml` and a visible `[skillist: ...]` mirror in `tasks.md`.
- Tasks that span implementation and evidence work may list multiple skills.
- Skill ids resolve to exactly one readable `SKILL.md` path using the current local skill inventory.
- Visual demo tasks include advisory patterns for scene rendering, screenshot capture, layout readability, persistent viewer launch, deterministic evidence mode, generated-package validation, evidence graph validation, evidence audit validation, and debug-until-green loops.
- Public API discovery and documentation tasks point to the applicable `.fsi` documentation guidance and validation path.
- Tasks with no applicable specialized skill declare `skillist: []` and explain the no-skill rationale when the task category might otherwise look skill-covered.

## Acceptance Cues

- The generated guidance names implementation skills, evidence graph/audit skills, `fs-skia-layout-evidence`, `fs-skia-template-update`, and debug-loop guidance where applicable.
- Multiple skills are ordered by dependency of use: implementation/domain guidance before evidence validation, evidence graph before evidence audit, focused debug guidance before broad validation reruns.
- Advisory FS.Skia.UI suggestions do not become hard failures for otherwise valid task lists.
