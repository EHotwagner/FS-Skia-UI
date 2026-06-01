# Quickstart: Foundations Two-Tier Development Process (Stage 1)

Recipe for implementing and verifying the `Route` entry point and the compiled routing policy.
Order follows Principle I (FSI sketch → tests → impl) and the 040/041 extraction pattern.

## 1. Author the contracts (FSI sketch, Principle I)
- Copy `contracts/Routing.fsi` and `contracts/ContractView.fsi` to
  `build/Governance/Routing.fsi` and `build/Governance/ContractView.fsi`.
- Add the four files to `build/Governance/FS.Skia.UI.Build.fsproj` `<Compile>` **after**
  `TargetMetadata.fs` (Routing depends on `Targets`) and before `Capabilities`:
  `Routing.fsi`, `Routing.fs`, `ContractView.fsi`, `ContractView.fs`.

## 2. Add the `Route` target case (typed single source)
- Add `Route` to the `Target` DU in `Targets.fsi` **and** `Targets.fs` (same position).
- Extend `name`, `directPrerequisites` (`Route -> []`), and `allTargets` (so metadata derives).
  `timeoutClass`/`cost`/`failureOwner` fall through to the `focused`/`low`/`governance` defaults.

## 3. Write failing-first Governance.Tests (≥6, SC-004 / FR-010)
- `tests/Governance.Tests/RoutingTests.fs` — call `Routing.select` / `selectForFeature` directly
  on literal `Diff` values; assert the **typed** `Selection.Tier` + `Selection.Gates`:
  1. `src/Scene/Foo.fs` only → `FrameworkAuthor`/`InnerLoop`/`[Dev]` (no surface check).
  2. `src/Lib/Foo.fsi` → escalates, gates include `PackageSurfaceCheck`.
  3. `template/base/x` → escalates, gates include `TemplateCheck`, `GeneratedProductCheck`.
  4. `.specify/templates/x` → escalates (generated-guidance).
  5. Mixed `src/Scene/Foo.fs` + `template/base/x` → highest tier wins (escalates, not inner-loop).
  6. dogfood `selectForFeature FrameworkAuthor "042" {scene-only diff}` → `fullPipelineGates`.
  7. Unknown path `weird/path.txt` → default-deny to `Verify` (never empty).
- `tests/Governance.Tests/ContractViewTests.fs` — `currencyDrift` returns `None` for the rendered
  text and `Some _` for a hand-mutated string (SC-007).
- Register both in `Governance.Tests.fsproj` `<Compile>` before `Program.fs`. Run and watch them
  fail to compile/assert (modules not yet implemented).

## 4. Implement `Routing.fs` + `ContractView.fs`
- `Routing.fs`: the rule table (data-model R5), `tierRank`, `innerLoopGates`, `select`
  (default-deny + `maxBy tierRank` escalation + registry-order gate de-dup), `selectForFeature`
  (dogfood override), `unmetArtifacts`, `enforceDiagnostic`, `renderSelection`. **No** access
  modifiers in `.fs` (Principle II). **No** `dotnet fsi` / FCS / `select-tier.fsx` (SC-006).
- `ContractView.fs`: `render` (deterministic YAML), `currencyDrift`.
- Get Governance.Tests green: `dotnet test tests/Governance.Tests` (non-FAKE, safe).

## 5. Wire the `Route` edge in `build.fsx`
- `#load "build/Governance/Routing.fs"` and `"build/Governance/ContractView.fs"` beside the 041 loads.
- Add `StartTarget Targets.Route ->` to `update`: compute the git union-`Diff` (R2) via `BuildProcess`,
  parse `--developer-class` and `--enforce` from the FAKE args, resolve `activeFeatureId`, call
  `selectForFeature`, then emit print or (`--enforce`) `File.Exists`→`unmetArtifacts`→exit-code effects.
- Fold `ContractView.currencyDrift` detection into the `TargetMetadataDrift` body; fold
  `ContractView.render` regeneration into `RefreshSurfaceBaselines` (R1).

## 6. Generate the contract & docs/guidance
- `./fake.sh build -t RefreshSurfaceBaselines` to (re)emit `validation.contract.yml` from `Routing.fs`.
- Update `CLAUDE.md` + `AGENTS.md`: "run `Route` first; run only the gates it prints"; reframe the
  serialized six-target order as the escalated/maintainer-verify path (FR-008). Update
  `SequentialFakeGuidanceTests.fs` to assert the new guidance.
- Document tiers/`Route`/`--enforce` in `docs/reports/build.md` and `docs/reports/speckit.md` (FR-009).

## 7. Capture evidence (dogfood — full serialized order, FR-015)
Capture `Route` transcripts into `specs/042-foundations-two-tier-process/readiness/`:
- SC-001: `Route` on a `src/Scene/*.fs`-only tree → `framework-author`/`inner-loop`/`Dev`.
- SC-002: `Route` on `template/base/**` (escalated) and on a `.fsi` (adds PackageSurfaceCheck); unknown path → fallback.
- SC-003: `Route --enforce` non-zero naming `readiness/package-surface-expectations.md`, then zero once present.
- SC-005: dogfood `Route` → full gate set on a would-be inner-loop diff.
- SC-007: hand-edit `validation.contract.yml`; show `TargetMetadataDrift` rejects it; regenerate; passes.

Then run the serialized FAKE order (never concurrent), logs into `readiness/logs/`:
`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`.

## 8. Confirm no product surface moved (SC-009)
`git diff --stat src/**` empty; `PackageSurfaceCheck`/`FsiTranscripts` no baseline diff; no new
`PackageVersion` outside `Directory.Packages.props`; `grep -r "select-tier.fsx\|dotnet fsi\|FSharp.Compiler"` over build/library is empty (SC-006).
