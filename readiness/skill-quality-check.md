# Skill-quality bar — the `SkillQualityCheck` rubric policy

Every FS-authored skill is held to one section rubric, enforced by the
`SkillQualityCheck` gate (feature 058, FR-001/FR-003). A change under any skill home
escalates via `Routing.fs` (the `skill-quality` rule) and must re-pass this gate.

## In-scope set (FR-004)

- `.agents/skills/**` (the `fsharp-*` and `fs-skia-*` families)
- `src/**/skill/SKILL.md` (the product capability skills)
- `template/product-skills/**`, `template/fragments/**/skill/SKILL.md`,
  `template/base/.agents/skills/**`

The vendored `.agents/skills/speckit-*` tree is **excluded** — the gate never flags or
rewrites it.

## Required rubric rows (every in-scope skill)

A skill PASSES only when all are present: **Scope / when-to-use**, **Driven-library
API** (names the driven `.fsi` surface, or an explicit "no backing library"
declaration), **Runnable example** (≥1 fenced code block exercising that API),
**External research links** (≥2 external URLs, official docs first), **Persistent-problem
mandate** (the FR-017 statement: official online docs first, then community, and where
findings are recorded), **Related** (`[[slug]]` cross-links), and **Sources**.

## Failure behaviour (Principle VII)

On any unmet row the gate FAILS loud, naming the **skill slug** and the **specific
missing section**; it never silently passes a skill it could not parse. The per-skill
PASS/FAIL report for a given change lands under `specs/<feature>/readiness/skill-quality-check.md`.

## Driven library

The five library-backed `fsharp-*` skills cite the shipped `FS.Skia.UI.SkillSupport`
`.fsi` (the same code `FS.Skia.UI.Build` runs via ProjectReference);
`fsharp-build-orchestration` cites the `FS.Skia.UI.Build` front-end; the `fs-skia-*`
skills cite their product-package surfaces.
