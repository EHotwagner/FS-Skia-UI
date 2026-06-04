# Contract: TemplateUpdateSkillPackageCheck (FR-009 / SC-006)

## Purpose
Guarantee the `fs-skia-template-update` skill's enumerated package set exactly equals
the current packable-project set, so it cannot drift (no phantom Lib, no missing
`SkillSupport`/`Input`).

## Inputs
- `PackableProject[]`: discovered from `*.fsproj` with `<IsPackable>true</IsPackable>`
  or `<PackageId>` under `src/**` and `build/Governance/**`.
- The skill's enumerated package IDs, parsed from
  `.agents/skills/fs-skia-template-update/SKILL.md` (the step-3 props-pin list and the
  step-5 feed-verification loop).

## Rules
1. **Feed-loop enumeration** (step 5) MUST equal the full packable set
   (currently 11: `Build, Scene, SkiaViewer, Elmish, KeyboardInput, Input, Layout,
   Controls, Controls.Elmish, Testing, SkillSupport`).
   - Failure (phantom): `template-update skill lists FS.Skia.UI (Lib) — not packable`.
   - Failure (missing): `template-update skill missing FS.Skia.UI.Input / .SkillSupport`.
2. **Props-pin enumeration** (step 3) MUST equal the template-pinned subset
   (`template/base/Directory.Packages.props` `FS.Skia.UI.*` entries). `Input` is
   packable but not pinned, so it appears in the feed loop, not the props list.
3. The "nine repo packages" count text MUST match the enumerated count.

## Routing
Routed by `.agents/skills/**` (and `build/Governance/**`) globs; appears in
`validation.contract.yml` after regeneration. Canonical edits in `.agents`; `.claude`
regenerated via `RefreshSurfaceBaselines` (`SkillSyncCheck` stays green).

## Test evidence (failing-first)
- Negative: current skill text (phantom Lib + missing Input/SkillSupport) fails the
  check before the edit; passes after.
- Diff assertion: skill set symmetric-difference packable set == ∅.

## Acceptance
SC-006 (diff skill list against packable `.fsproj` set → zero phantom, zero missing).
