# Contract: `SkillSyncCheck` and `SkillExamplesCheck` Build Targets

Both targets are FAKE-backed (`./fake.sh build -t <Target>`), added to `build.fsx`
(`requiredTargets`, `targetDependencyRows`, and the `Dev` dependency list so `Verify`/`Ci` inherit
them). Both MUST obey the repository's serialized FAKE-run order — never run concurrently with other
FAKE-backed targets (`CLAUDE.md`/`AGENTS.md`).

## `SkillSyncCheck` (FR-002, FR-011, SC-002)

- **Purpose**: prove all six capability skills are byte-identical across `.claude/skills/<slug>/`
  and `.agents/skills/<slug>/`.
- **Mechanism**: in-process SHA-256 (`System.Security.Cryptography.SHA256`) over `File.ReadAllBytes`
  of each pair (no `diff`/`cmp`/`sha256sum`; R3). No newline normalization — true byte-identity.
- **Dependencies**: `[]`.
- **Inputs**: the six `SkillPair`s.
- **PASS contract**:
  - Exit code 0.
  - Writes `readiness/skill-sync-check.md` listing the six slugs and their shared hash.
  - Writes log `readiness/logs/skill-sync-check.txt`.
- **FAIL contract**:
  - Non-zero exit (`FailWith`).
  - Message names every drifted slug and prints both hashes (`claudeHash` / `agentsHash`).
  - A missing file on either side is a FAIL (named), never a skip.
- **Self-test (SC-002)**: in-sync → PASS; flip one byte in one pair → FAIL naming that slug; restore
  → PASS.

## `SkillExamplesCheck` (FR-014, SC-007) — tangle-and-compile

- **Purpose**: prove every ` ```fsharp ` block across the six skills compiles against the pinned
  adopt-set packages.
- **Mechanism**:
  1. In-process extract (`build/Governance` `SkillExamples`): read the six `SKILL.md`, pull every
     ` ```fsharp ` block, wrap each in `module Skill.<slug_underscored>.Block<NN>` with a
     `// source: <skillPath>:<startLine>` comment, write to `build/SkillExamples/Generated/<slug>.fs`
     (deterministic; regenerated each run; never hand-edited).
  2. `dotnet build build/SkillExamples/SkillExamples.fsproj` (references the adopt set; capture to
     `readiness/logs/skill-examples-check.txt`).
- **Dependencies**: `[]` (or `[ "SkillSyncCheck" ]` — sync before compile; final choice recorded at
  implementation).
- **PASS contract**:
  - Exit code 0; the examples project compiled.
  - Writes `readiness/skill-examples-check.md` listing the per-skill block count.
- **FAIL contract**:
  - Non-zero exit.
  - The F# compiler diagnostic is mapped back (via the `// source:` comment / block registry, R4) to
    the owning skill + block index, and named in both the message and the report.
  - A missing examples-project artifact or empty extraction is a FAIL (no silent skip; Principle VII).
- **Self-test (SC-007)**: refined skills → PASS; introduce a deliberately broken block (wrong API
  call) → FAIL naming the skill/block; fix → PASS.

## Wiring & non-impact

- `Dev` gains both as dependencies; `Verify`/`Ci` inherit transitively. No other target
  (`PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, `EvidenceAudit`, `GeneratedProductCheck`) changes meaning.
- Neither target references the six skills from any `tasks.deps.yml` `skillist`; the evidence graph is
  unchanged (FR-007/SC-005).
- All new packages are build-tooling scope, pinned in `Directory.Packages.props`, FCS-free (FR-008).
