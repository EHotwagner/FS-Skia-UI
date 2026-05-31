# Feature Tier & Evidence Obligations (T002)

**Feature:** 038-authoring-guidance-consistency
**Tier:** **Tier 1 (contracted change).**

## Why Tier 1

The feature modifies the public `.fsi` surface:

- **FR-008 (US3):** adds `[<RequireQualifiedAccess>]` to
  `ViewerWindowStartupState` (`src/SkiaViewer/SkiaViewer.fsi`) — a recorded,
  breaking-but-accepted change (migration note + version bump).
- **FR-010 (US6):** adds additive, self-describing `Scene` constructors
  (`src/Scene/Scene.fsi`) — additive only, no removals.
- **FR-004 (US2):** bundles real `.fsi` signatures verbatim into generated
  output (no source-surface change, but ships the contract locally).

## Affected layers

| Layer | Files |
|---|---|
| Public `.fsi` contracts | `src/SkiaViewer/SkiaViewer.fsi`/`.fs`, `src/Scene/Scene.fsi`/`.fs`; `src/Elmish`, `src/KeyboardInput` evaluated (no change — already module-qualified, see `collision-name-enumeration.md`) |
| Surface baselines | `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`, `FS.Skia.UI.Scene.txt`, merged `FS.Skia.UI.txt` |
| Governance tooling | `build.fsx` (`GeneratedGuidanceCheck`, `TemplateCheck`, V3 product generation) |
| Template / generated output | `template/base/src/Product/*`, `template/base/tests/Product.Tests/Tests.fs`, `template/fragments/*/skill/SKILL.md`, `template/base/docs/*`, `.template.config/template.json` |

## Elmish / MVU applicability

**Not applicable.** The feature introduces no stateful workflow, command,
effect, subscription, or interpreter *behavior*. FR-009 (US5) documents the
existing effects boundary; FR-008 is a naming/visibility change; FR-010 adds
pure constructors. No `Model`/`Msg`/`Effect` contract is added or changed.

## Public-contract impact

Yes — Tier 1. `.fsi` + surface-baseline updates, migration note (FR-008), and a
merge-time version bump are required. FR-010 is additive (baseline grows, no
removals). FR-004 ships the already-pinned signatures verbatim (adds no package).

## Evidence obligations (from plan's Evidence Plan)

| Obligation | Evidence file |
|---|---|
| US1 ids resolve; guard fails on dangling/drift | `skill-resolution.md` + `skill-resolution-fixtures/` |
| US1 `.agents`↔`.claude` peers agree | peer-comparison in `skill-resolution.md` |
| US2 local API reference present + reflection-free | `generated-api-reference.md` |
| US3 no collision after `open` | `fsi/` (FAIL before / PASS after) |
| US3 surface delta + migration | refreshed baselines + `name-collision-migration.md` |
| US4 domain-agnostic + consumer-facing | `generated-guidance.md` |
| US5 single reachable effects page | `effects-boundary.md` |
| US6 both constructor forms compile | `fsi/` |
| FR-011 targeting regression | `feature-targeting-regression.md` |
| SC-001 governing | generated project builds/tests/evidence using only local refs — `logs/` |
