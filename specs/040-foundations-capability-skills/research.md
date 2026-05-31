# Phase 0 Research: Foundations F# Capability Skills

All decisions confine to **build-tooling scope** (`build/**`, `tests/Governance.Tests`), never ship
in a generated product, and never re-introduce `FSharp.Compiler.*`/FCS (spec FR-008). Library
verdicts are taken as authoritative from the capability report
(`docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md`) and are NOT
re-opened (spec Assumptions).

## R1 — Making each ` ```fsharp ` block compile in isolation (tangle strategy)

- **Decision**: The extractor wraps each block in its own generated module —
  `module Skill.<skillSlug>.Block<NN>` (slug with `-`→`_`) — written to
  `build/SkillExamples/Generated/<skillSlug>.fs` (blocks for one skill concatenated, each in its own
  nested/top-level module). Authoring convention (documented in the frontmatter contract): **every
  ` ```fsharp ` block MUST be valid F# module contents** — `let`/`type`/`open`/`module` declarations
  or a `let _ = <expr>` to anchor a bare expression. No block may rely on bindings declared in another
  block. `open` statements appear inside the block so the wrapper stays a dumb envelope.
- **Rationale**: One module per block gives stable identity for diagnostics (R4), prevents
  cross-block name collisions, and keeps `SKILL.md` the single source — the generated `.fs` is
  derived, never hand-edited. Module-content validity is the minimal constraint that lets a snippet
  compile without inventing a `main`.
- **Alternatives considered**: (a) concatenating all blocks into one module — rejected: name
  collisions and lost per-block identity; (b) compiling each block as a standalone script via
  `dotnet fsi` — rejected: reintroduces the FSX/script-runner path the foundations programme is
  removing, and is slower; (c) requiring each block to be a full program with `[<EntryPoint>]` —
  rejected: far too heavy for cookbook snippets.

## R2 — Warning policy for the generated examples project

- **Decision**: The `SkillExamples.fsproj` sets `<TreatWarningsAsErrors>false</TreatWarningsAsErrors>`
  and does NOT inherit the repo's `WarningsAsErrors` promotions (FS0025/26/52/64/78). It still fails
  the build on **errors**. Rationale: FR-014's bar is *compile-verified API-correctness* ("provably
  builds"), not warning-cleanliness; cookbook snippets legitimately have unused bindings, shadowing,
  or non-exhaustive matches for brevity. Visibility-modifier rule (FS0078) is irrelevant — generated
  modules carry no `.fsi`.
- **Implementation note**: Because `Directory.Build.props` sets these centrally, the examples project
  overrides them locally in its own `PropertyGroup` (later-wins). It also sets `IsPackable=false`.
- **Alternatives considered**: keeping `TreatWarningsAsErrors=true` and forcing every snippet to be
  warning-clean — rejected: it punishes readable teaching examples and tempts authors to add noise
  (`_` discards, `| _ ->` arms) that obscures the API point. The gate's job is "does this API call
  compile against the real package," not lint.

## R3 — Byte-identity mechanism for `SkillSyncCheck` (no `diff`/`cmp` in env)

- **Decision**: Compute SHA-256 in-process with `System.Security.Cryptography.SHA256` over the raw
  bytes of each `.claude/skills/<name>/SKILL.md` and `.agents/skills/<name>/SKILL.md`, compare the
  hex digests per pair. Read with `File.ReadAllBytes` (byte-exact; no newline normalization) so the
  check is true byte-identity including trailing newline and line endings.
- **Rationale**: The repository environment has no `diff`/`cmp` (spec FR-011 note). In-process hashing
  is deterministic, dependency-free (BCL), and matches the `fsharp-shell-process` in-process-first
  verdict — no shelling to `sha256sum` either. Comparing digests (not full contents) keeps the FAIL
  message compact while still naming the offender and showing both hashes.
- **Alternatives considered**: (a) shelling to `sha256sum` — works but adds an external-process
  dependency the BCL makes unnecessary; the FR-011 note allows `sha256sum` only because it predates
  the in-process decision. (b) byte-by-byte `Seq.forall2` compare — equivalent correctness but no
  compact digest to print and O(n) memory of both files; hashing is cleaner for the report.

## R4 — Block→skill identity for diagnostics

- **Decision**: The extractor records, per block, `{ skillSlug; blockIndex (1-based, per skill);
  startLine (line of the opening fence in SKILL.md) }`. The generated module name encodes
  `skillSlug` + `blockIndex`; a generated `// source: .claude/skills/<slug>/SKILL.md:<startLine>`
  comment precedes each module. On compile failure, `SkillExamplesCheck` maps the failing generated
  `.fs` file+line back to the source skill+block via this comment/registry and names it in the FAIL
  message and `readiness/skill-examples-check.md`.
- **Rationale**: FR-014/SC-007 require the gate to "point at the offending skill/block." Carrying the
  source coordinate through tangling is the only way to translate a compiler error on generated code
  back to the authored source.
- **Alternatives considered**: emitting `#line` directives into the generated `.fs` to make the
  compiler report the original SKILL.md path/line directly — attractive and may be adopted as an
  enhancement, but the comment+registry mapping is simpler to test and does not depend on compiler
  `#line` behavior for `.md` virtual files; recorded as a possible refinement.

## R5 — Examples-project package set (adopt-set, FCS-free)

- **Decision**: `SkillExamples.fsproj` references exactly the report's minimal adopt set so every
  snippet's API is real: `YamlDotNet` (present), `FSharp.SystemTextJson`, `XParsec`,
  `Microsoft.Extensions.FileSystemGlobbing`, `Fake.IO.FileSystem`, `Fake.Tools.Git`, `DiffPlex`,
  `FsCheck`, plus `Fake.Core.Target`/`Fake.Core.Process` (present/transitive). `System.Text.Json` and
  `System.Security.Cryptography` are BCL. All versions pinned in `Directory.Packages.props` in a
  build-tooling `ItemGroup`. **No `FSharp.Compiler.*`.** "Consider/Reject" packages
  (Fabulous.AST/Myriad, Argu/Spectre, CliWrap/Fli, QuikGraph, Legivel, Thoth/Newtonsoft, Markdig/
  FSharp.Formatting) are NOT referenced — `fsharp-code-generation` discusses Fabulous.AST/Myriad as
  *deferred/consider* prose and its compile-verified blocks use only adopt-set + BCL APIs (e.g. doc
  rendering via `StringBuilder`/`Utf8JsonWriter`), so the project stays minimal.
- **Rationale**: The examples project must reference precisely what the skills tell agents to adopt,
  so a passing compile proves the cookbook's API calls are correct against the pinned versions
  (spec Assumptions). Pulling consider/reject packages would contradict FR-008's scope discipline.
- **Version pinning**: latest stable net10-compatible at plan time; XParsec pinned to v1.0.0 per the
  report. Exact versions resolved during implementation against NuGet and recorded in
  `Directory.Packages.props` + `docs/reports/dependencies.md`.
- **Alternatives considered**: referencing only a subset and marking some skills "illustrative-only"
  — rejected: FR-014 requires *every* block to compile, so every API a block touches must be
  referenced.

## R6 — Wiring into `build.fsx` (effects vs process)

- **Decision**: Add two `StartTarget` arms in the existing MEL `update`. `SkillSyncCheck` uses a new
  in-process effect that calls `build/Governance` `SkillSync` (read bytes, hash, compare) and writes
  `readiness/skill-sync-check.md` + log; on drift it emits `FailWith` naming the skill(s).
  `SkillExamplesCheck` (a) runs an in-process effect calling `SkillExamples` to (re)generate
  `build/SkillExamples/Generated/*.fs` from the six skills, then (b) `processEffect`
  `dotnet build build/SkillExamples/SkillExamples.fsproj` capturing to
  `readiness/logs/skill-examples-check.txt`, then `RequireFiles` on the report and `WriteStructuredReport`.
  Both targets are added to `requiredTargets`, `targetDependencyRows` (`SkillSyncCheck`→[],
  `SkillExamplesCheck`→[] or [`SkillSyncCheck`]), and appended to the `Dev` dependency list (so
  `Verify`/`Ci` inherit them).
- **Rationale**: Mirrors the existing focused-gate pattern (`GeneratedGuidanceCheck`,
  `EvidenceGraph`) — `processEffect` + `RequireFiles` + `WriteStructuredReport`/`focusedGateSummary`
  — so the new gates are consistent and testable. In-process hashing/extraction honors
  in-process-first; only the real compile shells out, which is irreducible.
- **Serialized FAKE order**: both targets are FAKE-backed; running them is part of `Dev` and obeys
  the `CLAUDE.md`/`AGENTS.md` serialized order. They must not be run concurrently with other
  FAKE-backed targets.
- **Alternatives considered**: a standalone `dotnet run` front-end target (build/Build.fsproj) for
  the gates — deferred: the authoritative targets still live in `build.fsx`; adding there keeps one
  dispatch surface until the Stage-5 front-end migration.

## R7 — Capability ownership coverage (C1–C21 → exactly one skill)

- **Decision**: Fix the ownership map exactly as the report §11 / spec FR-001 states:
  `fsharp-parsing` = C1–C4, C16, C21; `fsharp-graph-algorithms` = C6–C9; `fsharp-code-generation` =
  C10–C12; `fsharp-io-globbing` = C13–C14; `fsharp-shell-process` = C15, C17;
  `fsharp-build-orchestration` = C18–C20. **C5 (JSON writing)** is covered within `fsharp-parsing`'s
  JSON section (C4/C5 paired in the report §3.4) — every capability owned by exactly one skill, none
  orphaned (SC-001). Each skill cites the report as `metadata.source` (SC-003).
- **Rationale**: SC-001 demands 100% coverage with single ownership. C5 has no standalone skill, so
  it is folded into the parsing skill's JSON read/write subsection where the report co-locates it.
- **Note**: C19 (diffing/DiffPlex) and C20 (Expecto/FsCheck) sit in `fsharp-build-orchestration`
  per FR-001 (C18–C20). The graph skill's *property-testing* prose cross-links to it via `[[ ]]`.

## R8 — Scope guardrails (capability/reference only)

- **Decision**: None of the six skills is added to any `tasks.deps.yml` `skillist` or `tasks.md`
  mirror; no Spec Kit command file is added or changed. Verified by `EvidenceGraph`/`EvidenceAudit`
  outputs being unchanged (SC-005). Skill guidance stays within the adopt set and never recommends
  FCS/runtime-script compilation (FR-008); the `fsharp-code-generation` "quotations" subsection
  explicitly states the reject verdict.
- **Rationale**: FR-007 / SC-005 require zero evidence-graph impact; capability skills are discovered
  by description, not wired into tasks.

## Open items resolved

All NEEDS CLARIFICATION from the spec's Clarifications session are answered there (refine all six;
mechanize sync via `SkillSyncCheck`; cookbook bar = multiple runnable examples + API walkthrough per
capability; compile-verified via tangle-and-compile; wired into `Dev`/`Verify`). No unresolved
unknowns remain for Phase 1.
