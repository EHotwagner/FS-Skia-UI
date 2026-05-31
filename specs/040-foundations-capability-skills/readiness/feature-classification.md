# Feature Classification & Evidence Obligations — 040 (T003)

## Change classification

- **Tier**: **Tier 1 (contracted change).** Introduces new build-tooling
  dependencies (seven adopt-set `PackageVersion` entries) and two new
  build-tooling `.fsi`-backed `build/Governance` modules plus two new FAKE
  targets. Obligations scoped to `build/**` and `tests/Governance.Tests`.

## Affected layer

- **Build-tooling only**: `build/Governance/SkillSync.fs(i)`,
  `build/Governance/SkillExamples.fs(i)`, the new
  `build/SkillExamples/SkillExamples.fsproj` tangle project, `build.fsx` (two
  targets), `Directory.Packages.props`, `docs/reports/dependencies.md`,
  `tests/Governance.Tests`, and the six `.claude`/`.agents` capability skills.
- **No runtime under `src/**`.** No `src/*` source is edited.

## Public-API impact

- **No tracked runtime surface diff** — the eight runtime packages and their
  surface baselines (`PackageSurfaceCheck`, `FsiTranscripts`) are untouched.
- **New build-tooling `.fsi`** (`SkillSync.fsi`, `SkillExamples.fsi`) — curated
  Principle II companions; not part of the tracked runtime baselines. No access
  modifiers in any `.fs`.

## Principle IV (MVU/effect boundary)

- **Plugs into the existing `build.fsx` `update`/effect boundary.** The two new
  targets dispatch new effects (`SkillSyncGate`, `SkillExamplesGate`) executed
  by the interpreter at the edge; the hashing/extraction/tangling logic is pure
  over its inputs. No new long-lived `Model`/`Msg` algebra is required.

## Evidence obligations

- Six refined capability skills, byte-identical across both trees.
- `SkillSyncCheck` PASS (byte-identity), with a flip-one-byte → FAIL → restore →
  PASS self-test.
- `SkillExamplesCheck` PASS (all ` ```fsharp ` blocks compile against the pinned
  adopt set), with a broken-block → FAIL-names-skill/block → fix → PASS
  self-test.
- `EvidenceGraph` / `EvidenceAudit` unchanged: no capability skill appears in
  any `tasks.deps.yml` `skillist` (SC-005).

## Synthetic evidence

- **None.** Real evidence throughout — rendered skill files, a real SHA-256
  comparison over the two real trees, and a real `dotnet build` of the real
  tangled examples against the real pinned packages. No `[S]`/`[SEH]` tasks. The
  deliberate-break self-tests (T020/T025) are gate self-tests, not shipped
  synthetic fixtures (see `gate-self-tests.md`).
