---
phase: tasks
date: 2026-06-08
severity: minor
---

## Process friction
Task generation itself was smooth — the preset templates
(`tasks-template.md` / `tasks-deps-template.yml`) gave a clear, directive
structure and the `EvidenceGraph` target validated the 33-task DAG on the first
run (clean: no cycles, no dangling refs, `[S*]`=0, both `owns:` declarations
accepted). One real friction point: the `skillist` id must be the skill file's
`name:` value, not its directory name, and there is exactly one mismatch in the
registry — `.agents/skills/fs-skia-ui-widgets/SKILL.md` declares
`name: spread1-widgets`. A generator that assumed directory==name would have
emitted an unresolvable id and only learned at validation time. What would have
helped: a one-line note in the tasks skill calling out that specific
directory/name divergence (the skill already warns about the general case, but
not the concrete offender).

## Generalizable code
none — this phase authored only Markdown/YAML artifacts (`tasks.md`,
`tasks.deps.yml`), no F# source.

## Skill gaps
none — `fs-skia-scene`, `fs-skia-keyboard-input`, `fs-skia-elmish`,
`fs-skia-evidence-mode`, `fs-skia-layout-readability`, `fs-skia-skiaviewer`,
`fsharp-parsing`, and `fsharp-graph-algorithms` covered every task cleanly, and
the speckit evidence skills covered the gate-ownership tasks. No missing
capability skill was encountered while mapping the spreadsheet work.

## Research links
research blocked — offline; no hard problem encountered this phase that
required external docs.
