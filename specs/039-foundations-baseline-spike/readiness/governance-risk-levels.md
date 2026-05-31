# Governance Risk Levels — 039 Foundations Baseline & Build-Library Spike

| Field | Value |
|---|---|
| Authoritative command | `./fake.sh build -t EvidenceAudit` (full merge-gate) + `./fake.sh build -t Dev` for compile |
| Artifact path | `specs/039-foundations-baseline-spike/readiness/logs/` |
| Failure class | governance-risk-classification |
| Next action on failure | Re-run the named command sequentially; if a build-tooling project regresses, fix the project, never weaken a gate |

## Risk level for this feature

This feature's governance risk level is **small** (additive build-tooling only;
no runtime, no consumer contract, no governance prose change). The risk-level
scale used across the programme is:

- **small** risk — additive, isolated change; **required evidence** is a clean
  focused build/test of the touched projects plus the runtime-untouched check.
  This feature sits here.
- **medium** risk — changes an inter-project contract or a shared build path;
  **required evidence** adds the relevant surface/check gates for the affected
  area.
- **broad** risk — touches governance prose, consumer contracts, or the
  template/generated-product surface; **required evidence** is the full
  serialized gate set (**broad validation**: `Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
  `DependencyReport` → `TemplateDrift` → `EvidenceGraph` → `EvidenceAudit`),
  plus `PackageSurfaceCheck`/`FsiTranscripts`.

## Risk classification for this feature

- **Tier**: Tier 1 (two new build-tooling projects + a new inter-project
  contract + a new library identity). Obligations scoped to the new
  `build/**` projects only.
- **Runtime risk**: **none / small.** No file under `src/**` is edited; the
  eight runtime packages and their surface baselines are untouched (SC-006), so
  no **broad validation** of the runtime surface is required.
- **Governance-prose risk**: **none / small.** No constitution / Spec Kit
  command / consumer-contract text changes. ADRs and baseline docs are *new*
  records, not edits to governed prose.
- **Build-tooling risk**: **small (contained).** New central `Fake.Core.*`
  package entries are build-only and excluded from every generated product
  (DependencyReport scoping). The single real unknown — FAKE-as-library without
  FSharp.Compiler.Service — was de-risked by the spike (D2 confirmed).

This feature does not introduce a persistent GUI runtime, window-visibility, or
persistent-launch surface; it owns no viewer/window evidence.
