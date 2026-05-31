# Capability Ownership — C1–C21 → exactly one skill (T026)

Each capability from the report (`metadata.source`) §2 is owned by exactly one
skill; union = {C1..C21}, intersection = ∅ (SC-001). Every skill's frontmatter
cites the capability report as `metadata.source` (SC-003, verified below).

| Capabilities | Family | Owning skill |
|---|---|---|
| C1, C2, C3, C4, C5, C16, C21 | parsing + JSON I/O + regex | `fsharp-parsing` (C5 JSON-write folded with C4 per report §3.4) |
| C6, C7, C8, C9 | graph algorithms | `fsharp-graph-algorithms` |
| C10, C11, C12 | document/artifact gen + F# source gen + quotations caveat | `fsharp-code-generation` |
| C13, C14 | file discovery + glob (fnmatch) | `fsharp-io-globbing` |
| C15, C17 | git + process wrapping | `fsharp-shell-process` |
| C18, C19, C20 | orchestration + diffing + testing | `fsharp-build-orchestration` |

## Coverage check

- **Union** = C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,C11,C12,C13,C14,C15,C16,C17,C18,C19,C20,C21
  = **{C1..C21}** — 100% coverage (SC-001).
- **Intersection** = ∅ — every capability owned by exactly one skill.
- The report's **eight families** (§2) collapse onto the six owning skills:
  *parsing* + *JSON I/O* → `fsharp-parsing`; *graph* → `fsharp-graph-algorithms`;
  *document gen* + *F# source gen* → `fsharp-code-generation`; *discovery &
  globbing* → `fsharp-io-globbing`; *shell/process/git* → `fsharp-shell-process`;
  *orchestration* + *diffing* + *testing* → `fsharp-build-orchestration`.

## `metadata.source` citation (SC-003)

All six skills carry
`source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md`
in frontmatter (confirmed by grep across both trees).

## Scope guard (SC-005 / FR-007) — T027

- Cross-tree byte-identity re-verified after refinement: `SkillSyncCheck` PASS
  over the refined six (`skill-sync-check.md`).
- None of the six `fsharp-*` capability skills appears in any `tasks.deps.yml`
  `skillist` or `tasks.md` mirror — they are capability/reference skills,
  discovered by description, never wired into the evidence graph. Confirmed by
  grep over `tasks.deps.yml` (only `speckit-evidence-graph` / `-audit` appear in
  skillists, on T029/T030).
