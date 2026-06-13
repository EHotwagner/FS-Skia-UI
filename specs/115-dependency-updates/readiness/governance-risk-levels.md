# Governance risk levels (feature 115)

feature-tier=tier-2-internal (safe product bumps) + governance-escalation (spec-kit `.specify/**` asset)
affected-packages=FSharp.Core (10.1.300→10.1.301) + Microsoft.Extensions.FileSystemGlobbing (10.0.8→10.0.9) in Directory.Packages.props; speckit_version recorded-version edit in .specify/init-options.json; held bumps adopted only if drop-in
public-api-impact=none (zero `.fsi`, zero surface-baseline, zero golden, zero sample-contract delta — FR-003)
mvu-applicability=no change for the safe bumps (Update/effects/subscriptions/interpreter untouched); a Fable.Elmish adoption is held behind a drop-in check and fully reverted if it would touch the boundary (FR-005)
route-tier=agent-ready (governance/consumer-contract escalation via `.specify/**`)

## Risk classification

- **small** — a single safe patch/minor pin edit (FSharp.Core, Microsoft.Extensions.FileSystemGlobbing).
  Focused validation is `Dev`; no broad rerun. There is no `.fsi` touch.
- **medium** — the spec-kit `.specify/**` recorded-version asset bump. Focused validation adds
  `GeneratedGuidanceCheck` / `TemplateDrift` because it touches a consumer-contract path; broad
  rerun only on a gate-reported drift.
- **broad** — any held major bump experiment (US2: YamlDotNet, Fable.Elmish, the Expecto +
  Microsoft.NET.Test.Sdk + YoloDev.Expecto.TestSdk cluster). Each **required evidence** is the full
  routed gate set before an adopt decision; **broad validation** is mandatory here because the blast
  radius is unknown until proven. A held bump that is not cleanly drop-in is fully reverted (FR-005).

THIS feature's safe-bump rung is **small/medium** (no `.fsi`, governance-asset escalation only); each
held-bump experiment is independently **broad**.

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

`./fake.sh build -t Route` was run against the real diff and printed (see `focused-gates.md`):
`Dev`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`. Only the gates Route
prints were run, FAKE-backed targets **sequentially**. Non-authoritative aggregate results are advisory
only in [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is the
focused per-target rerun.

## Required evidence per risk level

- **required evidence** (small / safe product pins): `Dev` green with zero surface-baseline / golden /
  generated-product diff after the bump (SC-001, SC-002).
- **required evidence** (medium / spec-kit asset): `GeneratedGuidanceCheck` + `TemplateDrift` green;
  `speckit_version` equals the version in use (SC-005).
- **required evidence** (broad / each held bump): the full routed gate set green **with no source change**
  → `adopted`; otherwise revert + record `deferred(<failing gate + symptom>)` (SC-003, FR-004, FR-005).
- **required evidence** (US3 template): `TemplateCheck` + `GeneratedProductCheck` confirm a fresh
  `dotnet new fs-skia-ui` project restores + builds (SC-004).
- **broad validation** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` with 0 synthetic.
