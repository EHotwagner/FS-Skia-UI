# Implementation Plan: Accessible Color Contrast & Palettes

**Branch**: `083-color-contrast-palettes` | **Date**: 2026-06-08 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/083-color-contrast-palettes/spec.md`

## Summary

Ship a new packable `FS.Skia.UI.Color` library that (1) computes WCAG 2.x
relative luminance and contrast ratio for the existing Scene `Color` type, (2)
maps a ratio + element role to a conformance verdict, and (3) exposes
Radix-derived, role-labelled accessible palette ramps in matched light/dark
variants. A new `ContrastCheck` build gate loads the generated Light/Dark theme
tokens and validates an explicit, role-tagged set of foreground/background
pairings against each theme's declared `contrastRequiredRatio` (text) or the
fixed 3:1 WCAG 1.4.11 threshold (graphic/UI). Failing shipped token values are
brought into conformance by editing the DTCG single source only. The gate is
registered through the single-source governance paths (Targets union → routing
rules → `knownGates` → generated `validation.contract.yml`), the template pins
gain the new package, and the `fs-skia-design-tokens` skill gains contrast
guidance with its `.claude` mirror regenerated.

This is a **Tier 1 (contracted change)** — new public package + `.fsi` surface,
new dependency edge (`FS.Skia.UI.Color` → `FS.Skia.UI.Scene`), new build gate.
Per the spec it escalates to the serialized maintainer-verify six-target path.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (matches existing packable libraries)
**Primary Dependencies**: `FS.Skia.UI.Scene` (for the `Color` type) — no new third-party dependency. Gate/generation logic lives in `FS.Skia.UI.Build` (existing).
**Testing**: Expecto (`tests/Color.Tests`), FSI transcript exercising the packed surface, Governance.Tests for routing/known-gates, FAKE `ContrastCheck` target as the enforcement gate.
**Target Platform**: Windows and Linux (pure managed arithmetic; no native/Skia render path).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: `template/base/Directory.Packages.props` MUST gain a
  `FS.Skia.UI.Color` pin at `$(FsSkiaUiVersion)` (FR-013). The new package is a
  core/runtime color capability — pinned unconditionally alongside
  `FS.Skia.UI.Scene` (not profile-gated), since `Scene` itself is unconditional
  and `Color` depends only on it. `.template.config/template.json` needs no
  change (no new template *option*, only a pin). The `fs-skia-template-update`
  skill's expected package set is updated to include the new pin.
- **Dependency impact**: One new *internal* project edge
  (`src/Color/Color.fsproj` → `src/Scene/Scene.fsproj`); no new entry in
  `Directory.Packages.props` (root) and no new third-party package, so
  `docs/dependencies.md` / `DependencyReport` need no new external row. The new
  package identity is reflected in the template pins (above). N/A for new
  external dependency — none added.
- **Command-surface impact**: `build.fsx` (or the FAKE target wiring under the
  governance engine) gains a `ContrastCheck` target that invokes the new
  validator. `Targets` union, `allTargets`, `directPrerequisites`/`name`/`spec`
  arms, `AgentValidation.knownGates`, and the `controls-public-surface` +
  new `color-contrast` routing rules change in `FS.Skia.UI.Build`. The
  generated `validation.contract.yml` regenerates from `Routing.fs` (currency
  enforced by `TargetMetadataDrift`). FAKE-backed targets run sequentially in
  the serialized order. The focused inner pair is shown below; this is an
  illustrative subset — the full escalated six-target maintainer-verify path
  (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`) still applies, per the spec Assumptions and
  tasks T028–T030:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t ContrastCheck`
- **Generated project impact**: Generated products gain the `FS.Skia.UI.Color`
  package availability via the template pin; no change to default/minimal
  generated *contents* or generated `Dev` behavior (the gate runs in this
  framework repo, not in generated products). `TemplateCheck` /
  `GeneratedProductCheck` re-validate the new pin.
- **Evidence paths**: `readiness/color-contrast-evidence.md` (ContrastCheck
  readiness report — both themes, per-pairing measured vs. required);
  `readiness/per-package-surface/FS.Skia.UI.Color.fsi.txt` (new per-package
  baseline); regenerated `src/Controls/DesignTokens.fs` (token conformance);
  regenerated `validation.contract.yml`; regenerated `.claude` skill mirror
  under `.claude/skills/fs-skia-design-tokens/**`; FSI transcript under
  `readiness/` for the packed `FS.Skia.UI.Color` surface; the escalated
  six-target evidence set (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`).
- **`.fsi` / contract impact**: NEW public `src/Color/*.fsi` surface
  (contrast functions, verdict/role types, palette ramp data) + new per-package
  baseline. `src/Controls/DesignTokens.fsi` token *surface* is UNCHANGED (no new
  token name; `contrastRequiredRatio` already exists); only generated *values*
  in `DesignTokens.fs` change. No other public API changes; no compatibility
  break (additive package).
- **MVU/effect boundary**: N/A — pure, stateless computation (luminance, ratio,
  verdict) and static palette data. No `Model`/`Msg`/`Effect`, no I/O, no
  subscriptions, no interpreter. The only filesystem read (loading generated
  token values for the gate) lives at the existing `Engine/Interpret.fs` edge,
  consistent with Principle IV; the validator core is pure over in-memory data.
- **Synthetic evidence**: None planned. Contrast values are verified against the
  published WCAG reference (black-on-white = 21:1, white-on-white = 1:1) — these
  are *reference constants*, not synthetic substitutes for missing real
  evidence. The regression-protection scenario (SC-005) deliberately injects a
  sub-threshold value as a *test input*, not as a stand-in for an unavailable
  dependency, so no `[S]` disclosure is required. If any pairing's accessible
  replacement value cannot be sourced before merge, that task is `[S]` — not
  anticipated.
- **Test evidence**: Failing-first Expecto tests in `tests/Color.Tests`:
  luminance/ratio reference pairs (SC-002), verdict thresholds per role
  (SC-004), ramp text/background AA pair (SC-003); a Governance.Tests case
  asserting `ContrastCheck` ∈ `knownGates` and routed for `src/Controls/**` +
  `src/Color/**`; the `ContrastCheck` gate itself as live enforcement on the
  shipped themes (SC-001); a gate-level regression test that a poisoned token
  fails with pairing/measured/required in the message (SC-005). FSI transcript
  exercises the packed public surface (Principle I).
- **Observability**: `ContrastCheck` failure output names, per failing pairing:
  both token names, resolved colors, measured ratio, required ratio, theme, and
  role (FR-008) — actionable, fail-loud. Non-solid paints report
  `Indeterminate` (visible exclusion, not silent pass). Report path
  `readiness/color-contrast-evidence.md`. Missing-baseline for the new package
  fails the surface diff (never silently clean).
- **Deferred scope**: Out of scope per spec — APCA/WCAG 3 scoring,
  rendered-pixel sampling, non-solid-paint (gradient-stop worst-case) analysis,
  palette generation from brand seeds, color-blindness simulation, non-color
  tokens, theme-authoring UI. All bounded follow-ups noted in the spec.

## Project Structure

```
src/Color/                               # NEW packable library FS.Skia.UI.Color
  Color.fsproj                           # PackageId FS.Skia.UI.Color; ref ..\Scene\Scene.fsproj
  Contrast.fsi / Contrast.fs             # luminance, ratio, Role, Verdict, verdict mapping, alpha compositing, paint solid/Indeterminate
  Palettes.fsi / Palettes.fs            # Radix-derived role-labelled ramps, light/dark variants
  skill/SKILL.md                         # capability skill (scope, contract, commands, evidence)

src/Controls/
  design-tokens.tokens.json              # EDIT: bring failing shipped color values into conformance (single source)
  DesignTokens.fs                        # REGENERATED whole-file (RefreshSurfaceBaselines)

build/Governance/
  Targets.fsi / Targets.fs               # EDIT: add `ContrastCheck` case + allTargets + name/directPrerequisites/spec arms
  Routing.fs                             # EDIT: add ContrastCheck to controls-public-surface gates; new color-contrast rule for src/Color/**
  AgentValidation.fs                     # EDIT: add "ContrastCheck" to knownGates
  ContrastGate.fsi / ContrastGate.fs     # NEW: validated-pairing set + gate runner (pure core; loads generated tokens at the edge)
  PerPackageSurface.fs                   # EDIT: add "FS.Skia.UI.Color" to packagesInScope

validation.contract.yml                  # REGENERATED from Routing.fs (TargetMetadataDrift currency)

readiness/
  per-package-surface/FS.Skia.UI.Color.fsi.txt   # NEW baseline
  color-contrast-evidence.md                       # NEW gate readiness report

.agents/skills/fs-skia-design-tokens/SKILL.md      # EDIT: contrast guidance (canonical)
.claude/skills/fs-skia-design-tokens/SKILL.md      # REGENERATED mirror (RefreshSurfaceBaselines / SkillSyncCheck)

template/base/Directory.Packages.props             # EDIT: add FS.Skia.UI.Color pin at $(FsSkiaUiVersion)

tests/Color.Tests/                                 # NEW Expecto project (reference pairs, verdicts, ramps)
tests/Governance.Tests/                            # EDIT: ContrastCheck routing/known-gates assertions
```

## Design notes / key decisions

See `research.md` for the WCAG formulas, Radix sourcing rationale, and the
validated-pairing-set design. `data-model.md` defines `Role`, `Verdict`,
`PaletteRamp`, and `ValidatedPairing`. `contracts/` carries the public
`FS.Skia.UI.Color` signature sketch and the `ContrastCheck` gate output schema.
`quickstart.md` walks the FSI-first exercise of the surface and the gate.

**Gate registration single-source flow** (FR-011): `ContrastCheck` is added to
the `Targets` union (a mistyped gate is then a *compile error*), to
`allTargets`, to the `name`/`directPrerequisites`/`spec` match arms, to
`AgentValidation.knownGates`, and woven into two routing rules — appended to the
existing `controls-public-surface` gate list (so design-token/theme edits select
it) and a new `color-contrast` rule for `src/Color/**`. `validation.contract.yml`
is then **regenerated** from `Routing.fs` (never hand-edited); `TargetMetadataDrift`
enforces currency.

**Token conformance flow** (FR-010): edit only failing `$value`s in
`design-tokens.tokens.json`, drawing replacements from the new ramps; run
`RefreshSurfaceBaselines` to regenerate `DesignTokens.fs`; `DesignTokenDrift`
confirms currency; conforming tokens are left byte-unchanged to minimize churn.
