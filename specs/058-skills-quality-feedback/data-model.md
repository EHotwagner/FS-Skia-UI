# Phase 1 Data Model: Skills Quality Uplift & Per-Phase Feedback Loop

Conceptual entities and their fields/rules. F# shapes here are *design intent*;
the authoritative signatures live in `contracts/`.

## Skill

A capability brief in the `.agents`/`.claude` tree or template `product-skills/`.

| Field | Meaning |
| --- | --- |
| `slug` | kebab-case id (frontmatter `name`) |
| `family` | `fsharp-*`, `fs-skia-*`, or `product-skill` |
| `drivenLibrary` | the package whose API the skill cites: `SkillSupport` (fsharp-*), a product package (fs-skia-*), or `none` (pure-process) |
| `sections` | the parsed section set used by the quality bar |
| `inScope` | true for FS-authored skills; false for vendored `speckit-*` |

**Rules**: vendored `speckit-*` skills are never `inScope` (FR-004 — excluded,
never flagged or rewritten). Canonical edits land in `.agents`; `.claude` is
generated (FR-005 — no hand-sync, no `SkillSyncCheck` drift).

## Skill-Quality Bar (rubric)

The required-section rubric `SkillQualityCheck` enforces (FR-001/FR-002). Every
in-scope skill MUST contain:

| Section | Requirement |
| --- | --- |
| Scope / when-to-use | present, non-empty |
| Driven-library API | for library-driven skills: names the driven library's public API entry points; for non-library skills: an explicit **"no backing library"** declaration |
| Runnable example | ≥1 runnable code example exercising the driven API (or covered by the "no backing library" declaration) |
| External research links | ≥2 external links (SC-002) incl. official docs |
| Persistent-problem mandate | the FR-017 research-mandate statement + where findings/links are recorded |
| Related links | cross-links (`[[slug]]`) to related skills |
| Sources | a sources line |

**Rules**: a missing section fails the gate naming **skill + missing section**
(FR-003). The "no backing library" edge case satisfies the API/example rows
without fabricating content (spec Edge Cases). A family with no helper yet may
reference `SkillSupport`'s **intended home** for that family without the bar
demanding non-existent API.

## Support library (FS.Skia.UI.SkillSupport)

| Field | Meaning |
| --- | --- |
| `packageId` | `FS.Skia.UI.SkillSupport` |
| `modules` | per-family: `Graph`, `Parsing`, `Globbing`, `CodeGen`, `ShellProcess` (+ future homes) |
| `surface` | curated `.fsi` per module; governed by `PerPackageSurfaceDiff` |
| `shipsWith` | the template, unconditionally (FR-008) — present for every profile |
| `consumedBy` | `FS.Skia.UI.Build` (ProjectReference) + generated-project agents |

**Rules**: visibility lives in `.fsi` only (Principle II); per-package surface
baseline required (FR-010); no existing public `.fsi` altered (additive).

## Feedback record

A dated, phase-identified entry under `specs/<feature>/feedback/`.

| Field | Meaning |
| --- | --- |
| `phase` | one of specify / clarify / plan / tasks / analyze / implement |
| `date` | creation date (ISO-8601) |
| `processFriction` | answer to prompt (a): what went wrong + what would have helped |
| `generalizableCode` | answer to prompt (b): skill family/topic + candidate helper (+ external docs/research links) or "none" |
| `severity` | answer to prompt (c): `none` / `minor` / `major` / `blocker` |
| `researchLinks` | when created after a hard problem: the external research links involved (official-docs-first, then community) |

**Rules**: one record per phase (FR-014); written only on phase completion
(FR-016); a record naming generalizable code MUST capture enough to triage it into
`SkillSupport` — the skill topic + candidate helper (FR-015); offline, the
research field records "research blocked + why" (FR-018).

**State**: a phase has feedback states `not-captured` → `captured` (terminal on
completion). An aborted phase stays `not-captured` (no partial record).

## Feedback parameter

| Field | Meaning |
| --- | --- |
| `name` | `feedback` (template `symbols`) |
| `datatype` | `bool` |
| `default` | `false` |
| `effect(true)` | emit `after_*` feedback hooks + `fs-skia-feedback-capture` skill + `feedback/` destination |
| `effect(false)` | no change — output byte-identical to today (FR-012/SC-006) |

## SkillQualityCheck gate

| Field | Meaning |
| --- | --- |
| `target` | `SkillQualityCheck` (in `Targets.Target`) |
| `routeRule` | matches `.agents/skills/**`, `template/product-skills/**` |
| `tier` | escalation contributor (consumer-contract / authoring) |
| `output` | `readiness/skill-quality-check.md` (PASS list; FAIL names skill+section) |
| `knownGate` | registered in `AgentValidation.knownGates` |

## Relationships

- A **Skill** (fsharp-*) cites the **Support library**'s documented API; its
  runnable example exercises that surface (FR-009).
- A **Skill** (fs-skia-*) cites a **product package** surface as its driven API
  (D4); not `SkillSupport`.
- A **Feedback record** that names generalizable code feeds the **Support
  library** backlog (FR-015 → US2 destination).
- **SkillQualityCheck** validates every in-scope **Skill** against the **rubric**.
- The **Feedback parameter** controls whether **Feedback records** are produced.
