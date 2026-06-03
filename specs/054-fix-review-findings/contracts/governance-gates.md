# Contracts: Governance Gates Affected

This feature exposes **no `.fsi` / public-API surface change** (Tier 2, internal). The contracts
it touches are the **governance gate behaviours** — the checks that other parts of the system and
generated consumers rely on. Each contract below states the assertion before and after.

## C1 — Template engine-pin parity (`TemplateCheck` / `GeneratedProductCheck`)

**Surface**: `tests/Governance.Tests/GeneratedProjectValidationTests.fs`
(test "generated evidence graph and audit run the packaged engine in-process", ~line 289–303).

**Before** (weak):
```fsharp
[ "#r \"nuget: FS.Skia.UI.Build"   // prefix-only — ignores the version
  "open FS.Skia.UI.Build.Evidence"
  ... ]
|> List.iter (fun required -> Expect.stringContains build required ...)
```

**After** (exact parity — FR-003):
```fsharp
// Extract the #r literal version from template/base/build.fsx and the
// FS.Skia.UI.Build PackageVersion from template/base/Directory.Packages.props;
// assert byte-equal. The #r-prefix presence check may remain, but version
// equality is the binding assertion.
let scriptVer = ... // regex: #r "nuget: FS.Skia.UI.Build, (X)"
let propsVer  = ... // regex: PackageVersion Include="FS.Skia.UI.Build" Version="(X)"
Expect.equal scriptVer propsVer "template build.fsx #r pins the same FS.Skia.UI.Build as Directory.Packages.props"
```

**Contract**: equal versions → PASS; any divergence → FAIL, naming both values. Comparison is
exact-string (tolerates `-preview.N`; never numeric).

## C2 — Pin-bump flow updates both pins (`fs-skia-template-update` skill)

**Surface**: `.agents/skills/fs-skia-template-update/SKILL.md` step 3 (canonical) →
regenerated `.claude` peer via `RefreshSurfaceBaselines` ([[skill-edit-validation-path]]).

**Before**: step 3 bumps only `template/base/Directory.Packages.props`
(`sed -i 's/Version="<old>"/Version="<new>"/g' ...`). The `#r` literal in `build.fsx` is never
touched → drifts.

**After** (FR-002): step 3 additionally rewrites the `#r` literal, e.g.
```bash
sed -i 's#\(#r "nuget: FS.Skia.UI.Build, \)[^"]*"#\1<new-version>"#' template/base/build.fsx
```
listed alongside the props edit and the affected-files inventory. Result: one flow leaves both
pins equal — C1 passes without a manual second edit (SC-003).

**Contract**: running the documented bump leaves `ScriptVersion == PropsVersion`.

## C3 — Zero FS3261 in the governance library (`Dev` / compiler)

**Surface**: `build/Governance/FS.Skia.UI.Build.fsproj` `<WarningsNotAsErrors>` element.

**Before**: `<WarningsNotAsErrors>$(WarningsNotAsErrors);FS3261</WarningsNotAsErrors>` — FS3261
is a non-fatal warning; 34 sites emit on a clean build, `Dev` stays green anyway.

**After** (FR-005/FR-009): all 34 sites resolved; the FS3261 entry removed from this fsproj's
`WarningsNotAsErrors`. FS3261 is now an **error** for this project only.

**Contract**: a clean build emits 0 FS3261; any re-introduced FS3261 fails the build (compiler-
enforced). `Directory.Build.props` global policy unchanged. No `.fsi`/baseline change (the
`Engine/Model.fs:72` fix aligns impl to the existing `.fsi`, not the reverse).

## C4 — Clean tree / Route classification (`EvidenceAudit` diff-scan, `Route`)

**Surface**: `.gitignore`; repository working tree.

**Before**: `specs/053-v3-monolith-retirement/readiness/package/local-packages.md` untracked →
escalates `Route` to `agent-ready` via `evidence-governance`.

**After** (FR-007/008): scratch removed; `specs/*/readiness/package/` ignored. `git status
--porcelain` empty.

**Contract**: after the standard pin-bump/pack flow the tree is clean; a routine framework-
internal diff routes to `inner-loop` (SC-007). Authored evidence `.md` files outside
`readiness/package/` remain tracked (no broad sweep).

## Non-contracts (explicitly unchanged)

- No `.fsi` signature, public docs, surface baseline, or sample contract changes.
- `EvidenceGraph` / `EvidenceAudit` **semantics** unchanged (only the diff-scan input — a clean
  tree — differs).
- No new dependency; no package identity/content/version change beyond the routine post-merge
  pin alignment.
