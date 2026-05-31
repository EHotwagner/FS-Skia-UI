# New `build/Governance` Public Surface & Fail-Fast Diagnostics — 040 (T009)

| Field | Value |
|---|---|
| Authoritative command | `dotnet build build/Governance/FS.Skia.UI.Build.fsproj` (compiles the `.fsi` against the `.fs`) |
| Artifact path | `build/Governance/SkillSync.fsi`, `build/Governance/SkillExamples.fsi` |
| Failure class | build-tooling-surface |
| Next action on failure | Update the `.fsi` and `.fs` in the same task (Principle II); no access modifiers in `.fs` |

## Surface baseline — `FS.Skia.UI.Build.SkillSync` (`SkillSync.fsi`)

- `type SkillPairResult = { Slug; ClaudePath; AgentsPath; ClaudeHash: string option; AgentsHash: string option }`
- `val expectedSlugs: string list`
- `val claudeRelPath: slug:string -> string`
- `val agentsRelPath: slug:string -> string`
- `val sha256Hex: bytes:byte[] -> string`
- `val inSync: result:SkillPairResult -> bool`
- `val drifted: results:SkillPairResult list -> SkillPairResult list`
- `val checkPair: root:string -> slug:string -> SkillPairResult`
- `val checkAll: root:string -> SkillPairResult list`
- `val renderReport: results:SkillPairResult list -> string`
- `val renderFailureMessage: results:SkillPairResult list -> string`

## Surface baseline — `FS.Skia.UI.Build.SkillExamples` (`SkillExamples.fsi`)

- `type CodeBlock = { SkillSlug; BlockIndex: int; StartLine: int; Source: string }`
- `val underscoreSlug: slug:string -> string`
- `val moduleName: block:CodeBlock -> string`
- `val extractBlocks: skillSlug:string -> markdown:string -> CodeBlock list`
- `val renderSkillFile: skillRelPath:string -> blocks:CodeBlock list -> string`
- `val extractAll: skills:(string * string * string) list -> (string * CodeBlock list) list`

## Fail-fast / no-silent-skip diagnostics (Principle VII)

- **Missing skill file** — `SkillSync.checkPair` yields `None` for the absent
  side; `inSync` returns `false`, so a missing file on either side is a FAIL
  naming the slug, never a skip. (Covered by a `tests/Governance.Tests` case
  over a synthetic root.)
- **Byte drift** — `SkillSyncCheck` emits `renderFailureMessage` naming every
  drifted slug and both digests; non-zero exit via `FailWith`.
- **Empty extraction** — `SkillExamplesCheck` hard-fails if zero ` ```fsharp `
  blocks are found across the six skills (no silent skip).
- **Block compile failure** — the F# diagnostic is mapped back to the owning
  skill + block via the generated `// source:` comment and named in both the
  failure message and the log; missing report/log is caught by `RequireFiles`.

Exercised live in `readiness/fsi-session.txt` and the gate self-tests
(`gate-self-tests.md`).
