# Tier & scope (T001)

- **Tier**: **Tier 2 (internal change)** throughout — build-tooling process-launch
  behavior only. No public API, `.fsi`, surface-baseline, or `PackageVersion`
  change. No story is Tier 1. (Constitution Tier and Routing tier are independent
  axes; both are satisfied — see Routing below.)
- **Routing**: `Route` **escalates** this feature to the `maintainer-verify` tier.
  The changed paths under `build/**` and `tests/**` fall outside the inner-loop
  `src/**` allowance, so escalation is via routing **default-deny** (the `build/**`
  edits do not match the `build-target-contract` rule's `build.fsx` /
  `scripts/build/**` / `validation.contract.yml` globs, so they carry no
  `Route --enforce`-required artifacts; the readiness set below is recorded
  voluntarily to keep the escalated-path evidence complete). Captured live at setup
  (working tree then dominated by `specs/**` + `AGENTS.md`):
  `developer-class=framework-author`, `tier=agent-ready`,
  `gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`,
  `dogfood-forced=false`, `matched-rules=evidence-governance, specify-catchall,
  docs-only`. The escalated `maintainer-verify` serialized order this feature
  repairs and is validated by is `Dev → GeneratedGuidanceCheck → TemplateCheck →
  GeneratedProductCheck → EvidenceGraph → EvidenceAudit` (see T013).
- **Affected surfaces**:
  - `build/Governance/Front/BuildEnvironment.fs` (NEW — pure `GraphicsDisplayState`
    classification + `normalizeGraphicsEnv`)
  - `build/Governance/Front/BuildProcess.fs` (EDIT — normalize child
    `startInfo.Environment` at the spawn edge; enrich the kill-on-timeout diagnostic)
  - `build/Governance/Front/BuildProcessHealth.fs` (EDIT — same normalization for
    `runShortCommand`)
  - `build/Program.fs` (EDIT — normalize the ambient process environment once at
    startup so every descendant inherits the deterministic selection)
  - `tests/Governance.Tests/GraphicsEnvironmentTests.fs` (NEW — pure unit + FsCheck
    property tests, a real process-spawn contract test, the diagnostic-builder test)
  - `specs/049-fix-escalated-flake/readiness/**`
- **Public-API impact**: **none** — no product `.fsi`, surface-baseline, or
  `PackageVersion` change. `git diff --stat -- 'src/**'` MUST be empty (verified at
  T014).
- **Elmish/MVU applicability**: **N/A** — the build front-end already owns an
  Engine `Model`/`Update`/`Interpret`. The new `normalizeGraphicsEnv` is a **pure
  function** (`Map<string,string> -> Map<string,string>`) consumed only at the
  existing interpreter edge (process spawn) and at `Program` startup; it adds no new
  `Model`/`Msg`/`Effect` and `update` stays pure.
- **Real-evidence obligations**: failing-first unit + FsCheck property tests for the
  pure normalization function; a real process-spawn contract test that inspects a
  real child's inherited environment and real exit code; the kill-on-timeout
  diagnostic-builder unit test; a **single-run** escalated execution on the headless
  host with captured logs; and the named readiness set
  (`aggregate-hang-diagnostics.md`, `runtime-limitations.md`,
  `graphics-env-contract.md`, `governance-risk-levels.md`, `target-metadata.md`,
  `agent-ready-verdict.md`). **Zero synthetic.**
