# Feature Specification: Fix Implementation-Completeness Review Findings

**Feature Branch**: `054-fix-review-findings`
**Created**: 2026-06-03
**Status**: Draft
**Input**: User description: "create specs to fix the problems/omissions"

## Context

The 2026-06-02 implementation-completeness review
(`docs/reports/2026-06-02-2230-foundations-and-v3-implementation-completeness-review.md`)
confirmed both large programmes (foundations 039–047, V3 retirement 048–053) are complete,
and surfaced a short list of **minor problems/omissions** in §4. This feature fixes the three
concrete, fixable ones:

- **§4.1** — the generated template's evidence-engine pin (`template/base/build.fsx`) drifts
  from the template's package pin (`template/base/Directory.Packages.props`), and the test that
  guards it only checks a substring, not the version, so the drift is invisible to the gates.
- **§4.2** — `FS3261` nullness warnings persist in the compiled governance library.
- **§4.3** — a pack-flow scratch artifact is left untracked under a readiness path, dirtying the
  tree and escalating `Route`.

The review's softer findings (§4.4 efficiency instrumentation) and larger forward design items
(§5, e.g. the Charts package split) are **explicitly out of scope** — they are not defects.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Generated app restores the correct evidence engine (Priority: P1)

A developer runs `dotnet new fs-skia-ui` to create an app, then runs the generated governance
build. The generated `build.fsx` must restore the **same** `FS.Skia.UI.Build` engine version
that the rest of the generated project is pinned to — not an older one. Today the generated
`build.fsx` carries a stale `#r "nuget: FS.Skia.UI.Build, <old>"` literal while
`Directory.Packages.props` pins a newer version, because the recurring pin-bump flow updates the
props file but not the `#r` literal.

**Independent test**: Generate an app (or inspect the template source); assert the
`#r "nuget: FS.Skia.UI.Build, <X>"` version in `build.fsx` is byte-equal to the
`FS.Skia.UI.Build` `PackageVersion` in the same template's `Directory.Packages.props`. After a
simulated pin bump, both move together. A deliberate mismatch fails the template gate.

**Why P1**: it is the only review finding with user-visible runtime risk — a generated app can
silently run an out-of-date governance engine, and the guarantee that pins move "for every
release" is currently violated.

### User Story 2 - Governance library builds warning-clean (Priority: P2)

A maintainer builds the governance front-end (`./fake.sh build -t Route`, or a direct
`dotnet build`). The build emits **no** `FS3261` nullness warnings. Today warnings appear in
`build/Governance/Guidance.fs` and `build/Governance/Front/Governance.fs` (nullable
`string` / `System.Diagnostics.Process` values). They do not fail the build (FS3261 is not
promoted to error), but they contradict the project's warnings-as-errors discipline.

**Independent test**: A clean build of `FS.Skia.UI.Build.fsproj` produces zero `FS3261`
diagnostics in its output. Behaviour of every affected function is unchanged (existing
Governance.Tests stay green).

**Why P2**: cosmetic/quality; no functional risk, but it is noise that obscures real warnings.

### User Story 3 - Pin-bump flow leaves a clean tree (Priority: P3)

After the recurring version-bump / `PackLocal` flow runs, `git status` is clean — no untracked
pack-flow scratch is left under a tracked readiness path. Today
`specs/053-v3-monolith-retirement/readiness/package/local-packages.md` is an untracked
local-package inventory left by that flow; because it sits under an evidence/governance path it
escalates `Route` and is the kind of "readiness churn" that masks real changes.

**Independent test**: Resolve the existing stray artifact (gitignore the pack-flow scratch
location and remove the stray file, or commit it if it is genuine evidence), then confirm
`git status --porcelain` is empty and a routine framework-internal diff routes to `inner-loop`
rather than being escalated by a stray governance-path file.

**Why P3**: pure hygiene; no functional or contract impact.

### Edge Cases

- The template pin appears in more than one literal form (e.g. the `#r` line and a comment in
  `Directory.Packages.props`); the parity check must target the executable `#r` literal, not
  prose mentions, and must tolerate the `-preview.N` suffix format.
- A future engine version with a different number of digits/segments must still compare equal by
  exact string, not by numeric coercion.
- Fixing FS3261 must not change null-handling semantics (a value that was effectively non-null
  must stay non-null; a genuinely nullable value must be safely pattern-matched, not force-
  unwrapped).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The generated template's evidence-engine reference
  (`template/base/build.fsx`, the `#r "nuget: FS.Skia.UI.Build, <version>"` literal) MUST always
  carry the **same version** as the `FS.Skia.UI.Build` `PackageVersion` in
  `template/base/Directory.Packages.props`.
- **FR-002**: The recurring pin-bump flow (the same flow that emits the "Update fs-skia-ui
  template package pins" commits) MUST update **both** the `#r` literal in `build.fsx` and the
  `PackageVersion` in `Directory.Packages.props` so they cannot diverge.
- **FR-003**: A governance gate MUST fail when the `#r` literal version and the
  `Directory.Packages.props` version disagree. The existing `Governance.Tests`
  (`GeneratedProjectValidationTests`) substring assertion MUST be strengthened to assert the
  **exact** version, not just the `#r "nuget: FS.Skia.UI.Build` prefix.
- **FR-004**: The immediate existing drift MUST be corrected so the two pins match at the head of
  this feature.
- **FR-005**: The compiled governance library (`FS.Skia.UI.Build.fsproj`) MUST build with **zero
  `FS3261` nullness warnings**. A clean (`--no-incremental`) build emits **34 sites across 8
  files** — `GeneratedProduct.fs`, `Front/Governance.fs`, `Engine/Model.fs`, `Guidance.fs`,
  `Front/BuildProcess.fs`, `Preflight.fs`, `PerPackageSurface.fs`, `Front/BuildProcessHealth.fs`
  — not only the two (`Guidance.fs`, `Front/Governance.fs`) the review §4.2 spotted on an
  incremental build. **All** sites MUST be resolved by safe null handling (pattern match /
  nullable annotation / `nonNull`), not by blanket warning suppression unless a specific site is
  genuinely a false positive (which MUST be documented inline).
- **FR-006**: Resolving the warnings MUST NOT change the observable behaviour of any affected
  function; all existing Governance.Tests MUST remain green.
- **FR-007**: The stray untracked pack-flow artifact
  (`specs/053-v3-monolith-retirement/readiness/package/local-packages.md`) MUST be resolved, and
  the pack-flow scratch location MUST be `.gitignore`d (or otherwise prevented from re-appearing
  as untracked) so the flow leaves a clean tree.
- **FR-008**: After this feature, `git status --porcelain` MUST be empty on a fresh checkout that
  has run the standard pin-bump / pack flow.
- **FR-009**: Once all FS3261 sites are resolved (FR-005), the **project-local** escape hatch
  `<WarningsNotAsErrors>$(WarningsNotAsErrors);FS3261</WarningsNotAsErrors>` in
  `FS.Skia.UI.Build.fsproj` MUST be removed so that any future FS3261 in this project fails the
  build. This is a **project-local** promotion only; the repo-wide `Directory.Build.props`
  warnings policy is unchanged (global promotion of FS3261 remains out of scope).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package **identity**, **content**, or **version** changes are introduced
  by this feature itself. It corrects a stale version **reference** inside the generated
  template's build script to match the template's already-pinned engine version, improving
  **generated package consumers** (a generated app restores the correct `FS.Skia.UI.Build`). No
  Charts/legacy package migration is involved.
- **Public contract impact**: No `.fsi` signature, documented public API, sample contract, or
  surface-baseline change. (Internal null-handling edits inside `FS.Skia.UI.Build` do not alter
  its curated `.fsi`.)
- **State workflow impact**: None. No commands, effects, subscriptions, or interpreter behaviour
  change; the null-handling fixes are behaviour-preserving.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshot, Vulkan,
  Skia, or visual-output change.
- **Evidence obligations**: real grep/diff proof of `#r`↔`Directory.Packages.props` version
  parity; a build-log excerpt showing zero `FS3261` in the governance library; a simulated
  pin-bump showing both pins move together and the gate catching a deliberate mismatch; a
  `git status --porcelain` empty proof.
- **Unsupported scope**: the Charts/DataGrid package split, efficiency/ceremony-time
  instrumentation, agent-context-byte or warm-build measurement, and capture of deferred visual
  reference frames (effects/screenshot galleries) are **out of scope** (review §4.4, §5).
- **Build-target impact**: `TemplateCheck` / `GeneratedProductCheck` gain a stricter version-
  parity assertion (FR-003); the pin-bump flow (`PackLocal` / template-update path) is extended
  (FR-002); `Dev` must show the governance library building warning-clean (FR-005). No change to
  `EvidenceGraph` / `EvidenceAudit` semantics.

## Success Criteria *(mandatory)*

- **SC-001**: At the head of this feature, the `FS.Skia.UI.Build` version in
  `template/base/build.fsx` exactly equals the version in `template/base/Directory.Packages.props`
  (verified by a single reproducible command).
- **SC-002**: A deliberately mismatched `#r` version fails a governance gate; reverting the
  mismatch makes the gate pass — demonstrated live.
- **SC-003**: A simulated pin bump updates **both** the `#r` literal and the `PackageVersion` in
  one flow, leaving them equal (no manual second edit needed).
- **SC-004**: `dotnet build build/Governance/FS.Skia.UI.Build.fsproj --no-incremental` (and
  `./fake.sh build -t Route`) emit **0** `FS3261` warnings; the pre-change clean build emitted
  ~**34** across 8 files (the exact count is recorded as the T004/T009 baseline; before/after
  captured). With the escape hatch removed (FR-009), a re-introduced FS3261 fails the build.
- **SC-005**: All existing Governance.Tests remain green; no `.fsi` or surface baseline changes.
- **SC-006**: `git status --porcelain` is empty after running the standard pin-bump / pack flow;
  the stray `local-packages.md` is resolved and its scratch location is ignored.
- **SC-007**: A routine framework-internal diff (no governance-path files) routes to `inner-loop`
  via `Route`, confirming the readiness-path churn is gone.

## Assumptions

- **Pin-parity direction**: `Directory.Packages.props` (`PackageVersion`) is treated as the
  source of truth, and `build.fsx`'s `#r` literal is the derived value the flow keeps current.
- **Scratch file disposition (§4.3)**: `local-packages.md` is **pack-flow scratch** (a local
  NuGet inventory the version-bump/pack flow emits), not durable 053 evidence, so it is removed
  and its location is gitignored rather than committed. If review judges it genuine evidence, it
  is committed instead — either way the tree ends clean (FR-008).
- **FS3261 resolution**: the warnings are resolved by safe null handling, preserving current
  behaviour; the build does not currently promote FS3261 to error, and this feature does not
  change that promotion policy (it just removes the warnings).
- **No version churn introduced**: this feature does not itself bump any package version beyond
  the routine post-merge pin flow; the corrective pin alignment may coincide with that flow.

## Out of Scope

- Charts/DataGrid package split (`FS.Skia.UI.Charts`) — review §5.2.
- Instrumenting ceremony-time / agent-context-byte / warm-build efficiency claims — review §4.4.
- Capturing deferred visual reference frames for the effects/screenshot galleries — review §5.3.
- Promoting FS3261 to a build error **globally** in `Directory.Build.props` (a repo-wide policy
  change). FR-009 removes only the **project-local** `FS.Skia.UI.Build.fsproj` escape hatch.
- Decoupling author-guidance prose from generation-currency term anchors — review §5.5.
