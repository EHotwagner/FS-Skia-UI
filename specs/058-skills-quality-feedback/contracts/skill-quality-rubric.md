# Contract: Skill-Quality Rubric & `SkillQualityCheck`

The machine-checkable contract the new gate enforces over every in-scope skill.

## In-scope set (FR-001/FR-004)

- `.agents/skills/fsharp-*/SKILL.md`
- `.agents/skills/fs-skia-*/SKILL.md` (incl. the new `fs-skia-feedback-capture`)
- `template/product-skills/fs-skia-*/SKILL.md`
- template fragment skills (`template/fragments/*/skill/SKILL.md`) and
  `template/base/.agents/skills/*/SKILL.md`

**Excluded**: `.agents/skills/speckit-*` (vendored). The gate MUST NOT flag or
modify them.

## Required sections (per in-scope skill)

A skill PASSES only when all rows are satisfied:

1. **Scope / when-to-use** — a non-empty section stating when the skill applies.
2. **Driven-library API** — for a library-driven skill, names the driven
   library's public API entry points (the `.fsi` surface). A non-library skill
   instead carries an explicit `no backing library` declaration.
3. **Runnable example** — ≥1 fenced code block exercising the driven API (waived
   by the `no backing library` declaration).
4. **External research links** — ≥2 external URLs, including official
   documentation (F#/.NET docs and/or the driven library's docs).
5. **Persistent-problem mandate** — the FR-017 statement (official docs first,
   then community) + where findings/links are recorded.
6. **Related** — ≥1 cross-link to another skill (`[[slug]]`), where applicable.
7. **Sources** — a sources line.

## Failure contract (FR-003, Principle VII)

On any unmet row the gate FAILS and the report names the **skill slug** and the
**specific missing section**. The gate never silently passes a skill it could not
parse.

## Pure-function shape (design intent — authoritative `.fsi` in build/Governance)

```fsharp
// build/Governance/SkillQuality.fsi (sketch)
type RequiredSection =
    | Scope | DrivenLibraryApi | RunnableExample
    | ResearchLinks | PersistentProblemMandate | Related | Sources

type SkillCheckResult =
    { Slug: string
      InScope: bool
      Missing: RequiredSection list }   // empty = pass

/// Pure: given a parsed skill, report missing required sections.
val checkSkill: parsed: ParsedSkill -> SkillCheckResult

/// Pure: aggregate over the enumerated in-scope corpus → Findings.
val checkCorpus: skills: ParsedSkill list -> Findings.Finding list
```

## Output artifact

`specs/058-skills-quality-feedback/readiness/skill-quality-check.md` — PASS list
(every in-scope skill, every section satisfied) plus a demonstrated FAIL row
naming skill+section (proves the gate bites).
