# Quickstart: Foundations F# Capability Skills

How to refine a skill, run the two gates, and verify the feature. All FAKE-backed commands run
**sequentially** (never concurrent — `CLAUDE.md`/`AGENTS.md`).

## Refine a capability skill (the cookbook bar)

1. Edit `.claude/skills/<slug>/SKILL.md` to the schema in
   `contracts/skill-frontmatter.contract.md`: verdicts, parity grammars (parsing/graph), API
   walkthrough + runnable ` ```fsharp ` examples per owned capability, cautions, consuming stages,
   Sources/links.
2. Each ` ```fsharp ` block must be valid F# **module contents** using only adopt-set + BCL APIs
   (see `research.md` R1/R5).
3. Copy the file byte-for-byte to `.agents/skills/<slug>/SKILL.md` (the synchronized peer).

## Run the gates

```sh
# Byte-identity across both trees (in-process SHA-256):
./fake.sh build -t SkillSyncCheck      # PASS: readiness/skill-sync-check.md lists six matching hashes

# Tangle every ```fsharp block into build/SkillExamples and compile it:
./fake.sh build -t SkillExamplesCheck  # PASS: readiness/skill-examples-check.md lists per-skill block counts

# Both are dependencies of Dev (so Verify/Ci include them):
./fake.sh build -t Dev
```

## Verify the feature (success criteria)

- **SC-001 / SC-006**: all six skills exist, each cites the report, together cover C1–C21 with single
  ownership; every skill is refined to the cookbook bar.
- **SC-002**: `SkillSyncCheck` PASSes in sync; flip one byte in one `SKILL.md` → it FAILs naming that
  slug; restore → PASS.
- **SC-007**: `SkillExamplesCheck` PASSes; break one block's API call → it FAILs naming the
  skill/block; fix → PASS.
- **SC-004 (spot checks)**: from the skill alone, name the controlling caution —
  - `fsharp-parsing`: YamlDotNet, the two `tasks.deps.yml` shapes.
  - `fsharp-io-globbing`: `Microsoft.Extensions.FileSystemGlobbing`, .NET-glob vs `fnmatch` drift.
  - `fsharp-graph-algorithms`: hand-rolled + FsCheck, the synthetic-propagation rule.
- **SC-005**: `./fake.sh build -t EvidenceGraph` then `-t EvidenceAudit` outputs are unchanged — no
  skill appears in any `tasks.deps.yml` `skillist`.

## Governance unit/property tests

```sh
dotnet test tests/Governance.Tests/Governance.Tests.fsproj   # extractor + SHA-256 comparator behavior
```

## What this feature does NOT do

No Bash/Python port, no Stage-2…6 governance algorithm beyond in-skill snippets, no package shipped
in a generated product, no `FSharp.Compiler.*`. Those are later features (report §13).
