# Contract: Capability Skill `SKILL.md` Schema

Applies to each of the six capability skills in BOTH trees
(`.claude/skills/<slug>/SKILL.md` and `.agents/skills/<slug>/SKILL.md`). The two files MUST be
byte-identical (enforced by `SkillSyncCheck`). Authoring this schema satisfies FR-003…FR-013; the
` ```fsharp ` blocks additionally satisfy FR-014 via `SkillExamplesCheck`.

## Frontmatter (YAML, required)

```yaml
---
name: <slug>                      # == directory name, kebab-case (FR-003)
description: <one line>           # what the skill is for (FR-003)
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.   # (FR-003)
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md   # (FR-003, SC-003)
---
```

- `name` MUST equal `<slug>` and the two directory names.
- `metadata.source` MUST be the capability report path (every skill cites it — SC-003).

## Required body sections (in order)

1. **Title + one-paragraph intro** — what the skill owns, which capabilities (C-numbers), pointer to
   `metadata.source` for rationale.
2. **When to use** — concrete triggers (the inputs/operations this skill governs).
3. **Library verdicts** — for EACH owned capability: the adopted package(s) (+ pinned version where
   relevant) and the rejected/deferred alternatives, each with a one-line reason (FR-004).
4. **Exact grammars / rules** — REQUIRED for parity-critical skills (`fsharp-parsing`,
   `fsharp-graph-algorithms`): reproduce the `tasks.md` task-line regex, box/annotation tokens, the
   `audit-status` region semantics, the two `tasks.deps.yml` shapes, the synthetic-propagation rule,
   etc., AND state the Stage-0 golden-fixture byte-parity obligation before the Bash/Python is
   deleted (FR-005). Other skills include this section only where a grammar applies.
5. **API walkthrough + runnable examples** — for EACH owned capability: a walkthrough of the adopted
   library's relevant API surface AND ≥1 runnable ` ```fsharp ` example. Multiple examples per
   capability where the API has distinct modes (FR-012). This is the cookbook bar — prose-only does
   NOT satisfy it.
6. **Cautions** — the report's named cautions where they apply: the two `tasks.deps.yml` shapes,
   .NET-glob vs Python-`fnmatch` drift, determinism/parity, each with the "golden-test before
   cutover" mitigation (FR-006). Scope cautions (no FCS, build-tooling-only) where relevant (FR-008).
7. **Consuming stages** — the plan stage(s) that use this skill (FR-009).
8. **Sources / links** — working links to the adopted library docs/API reference for the owned
   capabilities, plus the capability report (FR-013).
9. **Related** — `[[other-skill-slug]]` cross-links.

## ` ```fsharp ` code-block authoring rules (FR-014 / R1)

- Each block MUST be valid F# **module contents**: `let` / `type` / `open` / nested `module`
  declarations, or `let _ = <expr>` to anchor a bare expression.
- A block MUST NOT depend on bindings declared in another block (each is wrapped in its own generated
  module).
- A block MUST use only adopt-set + BCL APIs (the examples project references exactly the adopt set;
  R5). Do NOT write snippets against consider/reject packages or `FSharp.Compiler.*` (FR-008).
- The block text in `SKILL.md` is the single source of the example — it is never hand-duplicated into
  the generated examples project (the tangler extracts it).

## Acceptance

- `SkillSyncCheck` PASS (both trees byte-identical for all six — SC-002).
- `SkillExamplesCheck` PASS (every ` ```fsharp ` block across the six compiles — SC-007).
- Spot-check (SC-004): from the skill alone an agent can name the correct library + controlling parity
  caution for parsing (YAML two-shapes), globbing (fnmatch drift), and graph (synthetic propagation).
