# Implementation Plan: Fix Implementation-Completeness Review Findings

**Branch**: `054-fix-review-findings` | **Date**: 2026-06-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/054-fix-review-findings/spec.md`

## Summary

Fix the three concrete defects the 2026-06-02 implementation-completeness review surfaced in §4:

1. **§4.1 template engine-pin drift** — `template/base/build.fsx`'s
   `#r "nuget: FS.Skia.UI.Build, 0.1.45-preview.1"` literal lags `Directory.Packages.props`'
   `0.1.56-preview.1`. Align the pins now (FR-004), extend the `fs-skia-template-update` pin-bump
   flow to rewrite **both** (FR-002), and strengthen the `GeneratedProjectValidationTests`
   substring check into an exact version-parity assertion (FR-003) so the gate can never miss it
   again.
2. **§4.2 FS3261 nullness warnings** — resolve **all 34 sites across 8 files** (the review saw
   only 2 on an incremental build; user-confirmed full scope) by safe null handling, then remove
   the project-local `WarningsNotAsErrors;FS3261` escape hatch so regressions fail the build
   (FR-005/FR-009). Behaviour-preserving; `.fsi`/baselines untouched.
3. **§4.3 stray pack-flow scratch** — remove
   `specs/053-v3-monolith-retirement/readiness/package/local-packages.md` and `.gitignore` the
   `specs/*/readiness/package/` scratch location so the tree stays clean and `Route` reflects
   real changes (FR-007/FR-008).

Tier 2 (internal change): no public-API/`.fsi`/surface-baseline change, no new dependency, no
observable behaviour change. The `Engine/Model.fs:72` fix aligns the impl to the **existing**
`.fsi`, not the reverse.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: none added. Acts on `FS.Skia.UI.Build` (governance lib),
`template/base/**`, `Governance.Tests`, `.gitignore`, and the `fs-skia-template-update` skill.
**Testing**: Expecto (`Governance.Tests`); FAKE targets (`Route`, `Dev`, `TemplateCheck`,
`GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`); reproducible grep/build commands in
[quickstart.md](./quickstart.md).
**Target Platform**: Windows and Linux (governance tooling is cross-platform; no runtime/graphics
surface touched).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial evaluation: PASS.** No principle is violated; the change is behaviour-preserving
internal cleanup plus a gate-strengthening. Notes per principle:

- **I (Spec→FSI→Tests→Impl)**: no new public surface, so no new `.fsi` to sketch. The one
  test-first artifact is the strengthened parity assertion (FR-003) and the zero-FS3261 gate
  (FR-009) — both written/observed failing against current tree before the fix lands.
- **II (Visibility in `.fsi`)**: no `.fs` access modifiers added; `Engine/Model.fs` fix conforms
  the impl to the existing `.fsi` (`featureId: string`), so the curated surface is unchanged.
- **III (Idiomatic simplicity)**: null handling uses plain pattern matching / `nonNull` /
  `Option.ofObj`; no SRTP, reflection, custom operators, or active patterns introduced.
- **IV (MVU boundary)**: N/A — no new stateful or I/O-bearing workflow. The `Process.Start` fix in
  `Front/Governance.fs` keeps the existing edge interpreter shape; it only null-guards the result.
- **V (Synthetic disclosure)**: no synthetic evidence. All evidence is real (live build logs,
  real grep/diff, real `git status`). No `[S]` tasks anticipated.
- **VI (Test evidence)**: the parity gate and zero-FS3261 state are demonstrated failing-before /
  passing-after; existing `Governance.Tests` must stay green (FR-006).
- **VII (Observability/safe failure)**: the `Process.Start` null-guard **fails fast** (returns
  `Error`) rather than dereferencing a null — strengthens, not weakens, safe-failure.

**Post-design re-check: PASS** (no new violations introduced by the Phase 1 design below).

### Repository Governance Decisions

- **Template ownership**: **Yes, template changes are central.** `template/base/build.fsx` (the
  `#r` literal) and `template/base/Directory.Packages.props` are edited; the
  `fs-skia-template-update` skill (`.agents/skills/fs-skia-template-update/SKILL.md`, regenerated
  to `.claude`) gains the dual-pin bump step. `.template.config/template.json` does **not** change
  (no file added/removed/renamed in the template; only literal versions and an existing script
  body change). No template-content deferral needed.
- **Dependency impact**: **No new dependency.** `Directory.Packages.props` (template) has its
  `FS.Skia.UI.Build` `PackageVersion` already at `0.1.56-preview.1`; the feature only makes the
  derived `#r` literal match it. `docs/dependencies.md` and `DependencyReport` need **no** change
  (no package identity/version policy change). N/A for new-dependency justification.
- **Command-surface impact**: **Yes.** `TemplateCheck` / `GeneratedProductCheck` gain the stricter
  exact-version parity assertion (FR-003, via `Governance.Tests`). `Dev` must show the governance
  library building warning-clean and the removed escape hatch makes FS3261 a build error for
  `FS.Skia.UI.Build.fsproj`. `EvidenceGraph` / `EvidenceAudit` semantics are **unchanged** (only
  a clean tree differs as diff-scan input). FAKE-backed targets run **sequentially** in the
  deterministic order (Route → Dev → GeneratedGuidanceCheck → TemplateCheck →
  GeneratedProductCheck → EvidenceGraph → EvidenceAudit); never concurrent
  ([[fake-build-constraints]]).
- **Generated project impact**: **Yes (positive).** A generated app's `build.fsx` will restore the
  **correct** `FS.Skia.UI.Build` engine version (matching its own props pin) instead of a stale
  one. No change to default/minimal contents, selected-Controls guidance, local skills,
  placeholder/excluded-history scans, or generated `Dev` behaviour beyond the corrected pin.
- **Evidence paths**: `specs/054-fix-review-findings/readiness/` for this feature's artifacts —
  before/after FS3261 build-log excerpts, the pin-parity grep/diff proof, the simulated-bump
  proof, the deliberate-mismatch gate proof, and a `git status --porcelain` empty proof.
  Authored evidence is `.md`; regenerable logs land under `readiness/logs/**` (gitignored). The
  resolved stray lives at `specs/053-v3-monolith-retirement/readiness/package/` (removed +
  ignored). Heed [[readiness-dir-hazard]]: scratch goes in `/tmp`, not the repo root.
- **`.fsi` / contract impact**: **None.** No signature, public-doc, surface-baseline, or sample-
  contract change. The `Engine/Model.fs:72` nullness fix conforms the impl to the existing
  `.fsi` (`val featureId: string`). Tier 2 confirmed.
- **MVU/effect boundary**: **N/A.** No new `Model`/`Msg`/`Effect`/`init`/`update`/interpreter.
  The only I/O-adjacent edit null-guards an existing `Process.Start` call (`Front/Governance.fs`),
  preserving its current behaviour and failing fast on null.
- **Synthetic evidence**: **None.** No mocks/fakes/placeholders/canned responses. All proofs are
  real builds, real grep/diff, real `git status`. No `[S]`/`[SEH]` disclosure required; the
  `EvidenceAudit` diff-scan must end clean.
- **Test evidence**: failing-first parity assertion (FR-003) and zero-FS3261 gate (FR-009)
  demonstrated against the current tree; behaviour-preservation proven by the **existing**
  `Governance.Tests` staying green (FR-006). No new host/packed-library smoke needed (no runtime
  surface touched).
- **Observability**: the `Process.Start` null-guard emits an explicit `Error` (no silent null
  deref); the parity gate failure message names **both** divergent versions; the build error from
  the removed escape hatch names the FS3261 site. No new log path/report field required.
- **Deferred scope**: Charts/DataGrid package split (§5.2), efficiency/ceremony-time/agent-byte/
  warm-build instrumentation (§4.4), deferred visual reference frames (§5.3), global FS3261
  promotion in `Directory.Build.props`, and guidance-prose/term-anchor decoupling (§5.5) are
  **out of scope** — bounded follow-ups, not obligations of this feature.

## Project Structure

Files this feature touches (real paths):

```
template/base/build.fsx                                   # FR-001/004: align #r literal -> 0.1.56-preview.1
template/base/Directory.Packages.props                   # source of truth (already 0.1.56-preview.1; unchanged unless re-bumped)
.agents/skills/fs-skia-template-update/SKILL.md          # FR-002: step 3 also rewrites the #r literal (canonical)
.claude/skills/fs-skia-template-update/SKILL.md          # regenerated peer (RefreshSurfaceBaselines)
tests/Governance.Tests/GeneratedProjectValidationTests.fs # FR-003: prefix check -> exact version parity assertion
build/Governance/FS.Skia.UI.Build.fsproj                 # FR-009: remove WarningsNotAsErrors;FS3261 (project-local)
build/Governance/GeneratedProduct.fs                     # FR-005: ~22 FS3261 sites
build/Governance/Front/Governance.fs                     # FR-005: ~20 sites (incl. Process.Start null-guard)
build/Governance/Engine/Model.fs                         # FR-005: ~14 sites (incl. :72 signature nullness)
build/Governance/Guidance.fs                             # FR-005: ~8 sites
build/Governance/Front/BuildProcess.fs                   # FR-005: ~8 sites
build/Governance/Preflight.fs                            # FR-005: ~6 sites
build/Governance/PerPackageSurface.fs                    # FR-005: ~6 sites
build/Governance/Front/BuildProcessHealth.fs             # FR-005: ~6 sites
.gitignore                                               # FR-007: ignore specs/*/readiness/package/
specs/053-v3-monolith-retirement/readiness/package/local-packages.md  # FR-007: remove stray scratch
```

Generated artifacts for this plan: [research.md](./research.md), [data-model.md](./data-model.md),
[contracts/governance-gates.md](./contracts/governance-gates.md), [quickstart.md](./quickstart.md).

## Implementation Phases (preview for `/speckit-tasks`)

Ordered by spec priority; each story is independently testable.

1. **US1 (P1) — pin parity**: align `build.fsx` `#r` to `0.1.56-preview.1`; strengthen
   `GeneratedProjectValidationTests` to exact-version parity (write the assertion failing-first
   against the current drift, then fix the literal so it passes); extend the
   `fs-skia-template-update` skill step 3 to bump both pins; regenerate the `.claude` peer.
   Verify SC-001/002/003.
2. **US2 (P2) — warning-clean lib**: resolve every FS3261 site by resolution class
   (the clean `--no-incremental` count is captured at T004/T009 as the authoritative total; the
   per-file figures below are approximate upper bounds, not a fixed budget)
   (`data-model.md` NullnessSite); remove the `WarningsNotAsErrors;FS3261` escape hatch; confirm
   clean build = 0 FS3261 and `Governance.Tests` green. Verify SC-004/005.
3. **US3 (P3) — clean tree**: add `.gitignore` rule; remove the stray `local-packages.md`;
   confirm `git status --porcelain` empty and a routine diff routes `inner-loop`. Verify
   SC-006/007.

Each phase ends by running the **escalated maintainer-verify gate order** sequentially (see
quickstart.md) since the change set touches template/governance/contract paths and `Route` will
escalate.
