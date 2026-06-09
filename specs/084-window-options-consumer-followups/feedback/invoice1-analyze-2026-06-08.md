---
phase: analyze
date: 2026-06-08
severity: minor
---

## Process friction

The `/speckit-analyze` skill mandates the mechanical Pass G symbol cross-check
via `./fake.sh build -t SymbolCrossCheck` ("do **not** hand-derive the
set-difference"), but that target does not exist in this generated invoice
project's `build.fsx` — it ships only `Dev`, `Test`, `Verify`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit`.
The command failed with `Unknown generated invoice1 target: SymbolCrossCheck`,
forcing a manual, non-authoritative symbol set-difference instead. The manual
pass still surfaced the real finding (`MoveEditField` present in
data-model.md + tasks.md but missing from plan.md), so the gap was not blocking,
but the skill's instruction and the generated target surface are out of sync.
What would have helped: either the generated `build.fsx` exposing
`SymbolCrossCheck`, or the analyze skill degrading gracefully to a documented
manual fallback when the target is absent (mirroring how the EvidenceGraph
skill resolves the feature from `.specify/feature.json`).

## Generalizable code

none — no F# was written this phase (analyze is read-only).

## Skill gaps

The analyze skill assumes build targets (`SymbolCrossCheck`, and elsewhere
`DependencyReport` / `TemplateCheck`) that this generated consumer project does
not ship; a "target-availability probe" step — list available `build.fsx`
targets, then skip-with-notice any analyze step whose target is missing — would
have avoided the failed invocation and the ambiguity about whether Pass G ran
authoritatively.

## Research links

research blocked — offline environment; finding is local (target absent from
this repo's `build.fsx`), no external docs required.
