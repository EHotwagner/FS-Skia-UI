# Feature Specification: V3 Stage 1 — KEYSTONE: Host Extraction & Scene-Vocabulary Unification

**Feature Branch**: `050-v3-host-extraction`
**Created**: 2026-06-02
**Status**: Draft
**Input**: User description: "implement the next part" of the V3 implementation plan
(`docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md` §Stage 1)

## Context

The V3 modular-distribution programme retires the legacy `FS.Skia.UI` monolith (`src/Lib`). Stage 0
(feature `048-v3-retirement-baseline`) was **record-and-oracle only**: it captured the SHA-pinned
baseline, the deterministic scene-output **parity golden fixtures** (re-derived byte-identically from
the *current* monolith host), the **eight per-package surface baselines**, and ADRs 0007–0011. No
runtime code moved.

This feature is **Stage 1 — the keystone**. It is the first programme that deliberately edits the
runtime `src/**`, moving the Vulkan/Skia **host** out of `src/Lib` into the `FS.Skia.UI.SkiaViewer`
package, **retyped onto the canonical `FS.Skia.UI.Scene` vocabulary**. That single move deletes the
`SceneConversion.fs` bridge, the `SkiaViewer → Lib` project reference, and `Lib`'s **duplicate** scene
vocabulary together — closing the modularity leak whereby any product using the viewer (the default
`app` profile) transitively pulled the entire old core back in.

**The architecture finding that motivates this (from Stage 0):** `Lib/Library.fs` owns the host
(`Viewer`, `Diagnostics`/`RenderDiagnostic`, internal `VulkanStartup`/`VulkanResources`) **and** a
complete parallel copy of the scene vocabulary under namespace `FS.Skia.UI` (`VertexMode`, `Vertex`,
`TextRun`, `FontSpec`, `PerspectiveTransform`, `Colors`/`Paint`/`Path`/`Scene`). `SkiaViewer` is a
wrapper that project-references `Lib`, calls `Viewer.*` for the real host, and carries
`SceneConversion.fs` (a bridge that converts between the `FS.Skia.UI.Scene` package types and `Lib`'s
`FS.Skia.UI` types) — existing only because the host speaks the old vocabulary while the public API
speaks the new one. Unify the vocabulary and the bridge, the reference, and the duplicates all vanish.

**Scope decision (maintainer-confirmed during specification):** because the legacy samples and tests
consume `Lib` via **`ProjectReference`** (source, not the published package) and use its host/scene
modules, deleting `Lib`'s host + duplicate scene vocabulary would break their compilation. The
maintainer chose to **delete those `Lib` modules in this stage and pull the mechanical sample/test
repointing forward** so the full serialized gate set stays green at Stage-1 exit. This stage therefore
absorbs the *mechanical repointing* portions of plan Stages 3–4; what remains for later stages is the
governance-parser relocation (Stage 2 — `AgentValidation`), the *retirement* of the `Parity.Tests`
bridge (Stage 4), and the final `src/Lib` deletion + unpublish + enforcement (Stage 5). `Lib` after this
stage retains only `AgentValidation`, the duplicate `KeyboardInput`, and the `Parity` helper.

The non-negotiable gate is **output parity**: the host, once moved and retyped, MUST re-derive the
Stage-0 scene-output golden **byte-identically** before the legacy source is removed.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Move the host without changing rendered output (Priority: P1)

As the maintainer relocating the Vulkan/Skia host between packages, I need the moved-and-retyped host to
produce **byte-identical scene output** and **visually-identical rendered frames** versus the Stage-0
baseline, so that I can prove the relocation is behaviour-preserving rather than trusting that it is.

**Independent test**: After the host moves into `FS.Skia.UI.SkiaViewer` and is retyped onto
`FS.Skia.UI.Scene`, the parity harness re-derives the deterministic scene-output for the seed set
(`basic-viewer`/`effects-gallery`/`screenshot-gallery`) and diffs it against the committed Stage-0
golden fixtures with **zero byte difference**. The reference `basic-viewer` frame matches the Stage-0
capture. A reviewer can confirm the host source no longer exists in `Lib` only after the parity diff is
clean.

### User Story 2 - Close the modularity leak (Priority: P1)

As a consumer of the default `app` template profile, I need the viewer package to stop transitively
pulling in the entire legacy monolith, so that the dependency-light promise of V3 finally holds for the
most common path — not just for `Scene`/`Layout`/`Charts`/`KeyboardInput`.

**Independent test**: `FS.Skia.UI.SkiaViewer.fsproj` has **no** `ProjectReference` to `Lib`; the packed
`FS.Skia.UI.SkiaViewer` package has **no** package dependency on `FS.Skia.UI`; and a freshly generated
default `app` resolves **without** the monolith in its transitive graph — verifiable by re-running the
Stage-0 leak-proof dependency-dump command and observing the monolith is gone.

### User Story 3 - Keep the consumer contract and the full build green (Priority: P1)

As an agent running the escalated serialized gate set, I need every generated consumer and every legacy
sample/test to still restore, build, and run after the host move and the `Lib`-module deletion, so that
Stage 1 ships as an independently-revertible, green increment rather than a half-migrated tree.

**Independent test**: With `Lib`'s host + duplicate scene vocabulary deleted, the samples and tests that
used them are repointed onto `FS.Skia.UI.Scene` + `FS.Skia.UI.SkiaViewer` (+ `Elmish` where used) and
the full serialized gate set (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
`EvidenceGraph`, `EvidenceAudit`) is green. `TemplateCheck` confirms the default `app` still
restores/builds/runs.

### Edge Cases

- **Render divergence on retyping**: coordinate/paint/transform edge cases may differ subtly when the
  host is retyped from `Lib`'s `FS.Skia.UI` types onto the `FS.Skia.UI.Scene` types. The byte-identical
  scene-output gate MUST block the legacy-source deletion until the diff is zero; the move is not
  "done" until parity is clean.
- **Headless-render flake**: the known `SkiaViewer.Tests` libdecor-gtk headless crash can mask or mimic
  a real regression. The deterministic scene-output golden remains the **authoritative** parity signal;
  screenshots corroborate; a focused rerun is authoritative over the aggregate run per the recorded
  flake guidance.
- **Native startup/cleanup lifetime shift**: `VulkanStartup`/`VulkanResources` lifetime and cleanup
  behaviour must not change when the modules move packages. The native startup-cleanup tests travel
  with the host into `SkiaViewer` and run in the gate.
- **Parity oracle must not retire here**: `Parity.Tests` is repointed/retained as the Stage-1 parity
  harness; it is **not** removed in this stage (its retirement is Stage 4, after sign-off).
- **Acyclic graph**: retyping must not introduce a back-edge (`Scene → SkiaViewer`) or add a heavy
  dependency to a base package; `Scene` stays FSharp.Core-only.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The host modules — `Viewer` (`create`/`run`/`withEventMapping`/`withEffectMapping`/
  `withSubscription`/`defaultConfiguration`), `Diagnostics` (`RenderDiagnostic`), and the internal
  `VulkanStartup`/`VulkanResources` — MUST move from `src/Lib/Library.fs` into the
  `FS.Skia.UI.SkiaViewer` package (under a `Host/` module group), **preserving the public function
  shapes** already re-exposed by the `SkiaViewer` wrapper.
- **FR-002**: The moved host MUST be **retyped onto the `FS.Skia.UI.Scene` vocabulary**: every internal
  use of `Lib`'s `FS.Skia.UI` scene types (`Vertex`/`VertexMode`/`TextRun`/`FontSpec`/
  `PerspectiveTransform`/`Scene`/`Paint`/`Path`/`Colors`/…) is replaced by the `FS.Skia.UI.Scene`
  package equivalent.
- **FR-003**: `src/SkiaViewer/SceneConversion.fs` MUST be deleted — with a single vocabulary there is no
  conversion to perform.
- **FR-004**: The `SkiaViewer → Lib` `ProjectReference` MUST be removed. After this stage
  `FS.Skia.UI.SkiaViewer` depends only on `FS.Skia.UI.Scene` + `FS.Skia.UI.KeyboardInput` + its native
  packages (Silk.NET/SkiaSharp), and the packed `FS.Skia.UI.SkiaViewer` package has **no** package
  dependency on `FS.Skia.UI`.
- **FR-005**: `Lib`'s now-redundant host and duplicate scene-vocabulary modules
  (`Colors`/`Paint`/`Path`/`Scene`/`Diagnostics`/`Viewer`) MUST be deleted from `src/Lib`. After this
  stage `Lib` retains only `AgentValidation` (Stage 2), the duplicate `KeyboardInput` (dead once nothing
  references `Lib`), and the `Parity` helper (retires in Stage 4).
- **FR-006**: Every consumer that used the deleted `Lib` host/scene modules via `ProjectReference` MUST
  be repointed so it builds green — the legacy samples (`BasicViewer`, `EffectsGallery`,
  `ScreenshotGallery`, `InteractiveViewer`, `DemoReel`) onto `FS.Skia.UI.Scene` +
  `FS.Skia.UI.SkiaViewer` (+ `Elmish` where used), and the affected tests (`Lib.Tests`, `Smoke.Tests`,
  `Package.Tests`, `Parity.Tests`) onto the split packages. A consumer that referenced `Lib` *only* for
  surface staying in `Lib` (`Governance.Tests` → `AgentValidation`; `ParityGallery`/`Parity.Tests` →
  the `Parity` helper) MAY keep a reduced `Lib` reference until its later stage.
- **FR-007**: `Parity.Tests` MUST be repointed and **retained** as the Stage-1 parity harness, NOT
  retired in this stage. It MUST re-derive the moved host's deterministic scene-output for the Stage-0
  seed set and diff it against the committed Stage-0 golden fixtures.
- **FR-008**: The moved-and-retyped host's deterministic scene-output MUST be **byte-identical** to the
  Stage-0 golden fixtures for every seed (`basic-viewer`/`effects-gallery`/`screenshot-gallery`);
  reference frames MUST be visually identical where capture is feasible. The legacy `Lib` host source
  (FR-005) MUST NOT be deleted until this parity diff is clean (ADR 0011).
- **FR-009**: The default `app` template profile MUST still restore/build/run (`TemplateCheck` green)
  and MUST **no longer** pull the `FS.Skia.UI` monolith transitively (leak-proof dump shows the monolith
  absent from the generated `app`'s resolved graph).
- **FR-010**: The package graph MUST remain acyclic and `FS.Skia.UI.Scene` MUST remain FSharp.Core-only;
  the host move may not introduce a back-edge or add a heavy dependency to a base package.
- **FR-011**: The `SkiaViewer` per-package public-surface baseline MUST be updated to record the host
  move; the **net public surface should be stable** (the host API was already re-exposed by the
  wrapper). Any net surface delta MUST be explicitly recorded in the per-package baseline and pass
  `PerPackageSurfaceDiff`.
- **FR-012**: The native startup/cleanup tests MUST travel with the host into `SkiaViewer` and run in
  the gate; native startup/cleanup behaviour MUST NOT change.
- **FR-013**: No FCS / dynamic compilation / runtime script-loading may be introduced by the host move
  (carried invariant 7).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: **Package *contents* change; identities and the published set do not.** Host code
  moves from the `FS.Skia.UI` monolith into the `FS.Skia.UI.SkiaViewer` package; the packed
  `FS.Skia.UI.SkiaViewer` **loses** its transitive dependency on `FS.Skia.UI` (leak closed). No package
  is renamed or re-versioned in this stage, and `FS.Skia.UI` is **not** unpublished here (Stage 5). No
  Charts/Controls authoring change.
- **Public contract impact**: **`SkiaViewer`'s `.fsi` changes internally but its net public surface
  should be stable** (the host API was already re-exposed by the wrapper). The `SkiaViewer` per-package
  surface baseline is updated to record the move; any net delta is recorded. `SceneConversion` (an
  internal bridge) is removed. No documented public API of `Scene` changes.
- **State workflow impact**: **Host runtime moves but its behaviour is preserved.** `Viewer`
  (`create`/`run`/event/effect/subscription mappings, `defaultConfiguration`) keeps its function shapes;
  the Elmish interpreter contract is unchanged. Parity proves no observable behaviour change.
- **Layout/rendering impact**: **Rendering is relocated and retyped but MUST be byte/visually
  identical.** The Vulkan/Skia host moves packages and is retyped onto `FS.Skia.UI.Scene`; the
  scene-output golden + reference frames are the merge gate. Headless-flake risk is mitigated by
  treating scene-output as authoritative and using a focused rerun.
- **Evidence obligations**: byte-identical scene-output parity diff vs the Stage-0 golden (the merge
  gate); leak-proof dependency dump showing `FS.Skia.UI.SkiaViewer` and the generated `app` no longer
  resolving `FS.Skia.UI`; updated `SkiaViewer` per-package surface baseline with a clean
  `PerPackageSurfaceDiff`; green full serialized gate set with `EvidenceAudit` PASS on real
  (zero-synthetic) evidence. Reference-frame corroboration where capture is feasible; recorded
  infeasibility per Principle V otherwise.
- **Unsupported scope**: No `AgentValidation` relocation (Stage 2). No `Parity.Tests` *retirement*
  (Stage 4) — it is repointed/retained here. No `src/Lib` deletion, `FS.Skia.UI` unpublish, per-package
  Route-gating/enforcement, or generated-project cleanliness gate (Stage 5). No separate
  `FS.Skia.UI.Charts` package split. No new rendering architecture. No template-profile expansion.
- **Build-target impact**: No new build target is added; the host move runs under the existing escalated
  serialized gate set (`Route` escalates this consumer-contract + `src/**/*.fsi`-touching change to
  dogfood). `PerPackageSurfaceDiff` (from Stage 0) is run to record/verify the `SkiaViewer` baseline
  delta. `TemplateCheck`/`GeneratedProductCheck`/`GeneratedGuidanceCheck` confirm the consumer contract;
  `EvidenceGraph`/`EvidenceAudit` are the evidence gate. No change to the *behaviour* of `Dev`,
  `Verify`, `Ci`, `PackLocal`, or `DependencyReport`.

## Success Criteria *(mandatory)*

- **SC-001**: `FS.Skia.UI.SkiaViewer.fsproj` has **zero** `ProjectReference` to `Lib`, and the packed
  `FS.Skia.UI.SkiaViewer` package has **zero** package dependency on `FS.Skia.UI` (verified against the
  Stage-0 leak-proof dump).
- **SC-002**: The moved-and-retyped host re-derives the Stage-0 scene-output golden **byte-identically**
  (0-byte diff) for all three seeds; the legacy `Lib` host source is deleted only after that diff is
  clean.
- **SC-003**: A freshly generated default `app` resolves **without** `FS.Skia.UI` in its transitive
  dependency graph (leak closed), verifiable by the recorded dependency-dump command.
- **SC-004**: `src/Lib` no longer contains the `Viewer`/`Diagnostics`/`Colors`/`Paint`/`Path`/`Scene`
  modules nor `VulkanStartup`/`VulkanResources`; `SceneConversion.fs` is deleted. `Lib` retains only
  `AgentValidation`, the duplicate `KeyboardInput`, and the `Parity` helper.
- **SC-005**: No project references the deleted `Lib` modules; the legacy samples and the affected tests
  build green against the split packages. `Parity.Tests` is retained (not removed) and runs as the
  parity harness.
- **SC-006**: `FS.Skia.UI.Scene` remains FSharp.Core-only and the package graph remains acyclic (no new
  back-edge or heavy base-package dependency).
- **SC-007**: The `SkiaViewer` per-package surface baseline is updated; the recorded net public-surface
  delta is **empty or explicitly justified**, and `PerPackageSurfaceDiff` is clean against the updated
  baseline.
- **SC-008**: The full serialized gate set (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`) is green, with `EvidenceAudit` returning
  PASS on real (zero-synthetic) evidence.

## Key Entities

- **Host modules**: `Viewer`, `Diagnostics` (`RenderDiagnostic`), internal `VulkanStartup`/
  `VulkanResources` — the Vulkan/Skia host being moved into `FS.Skia.UI.SkiaViewer` and retyped onto
  `FS.Skia.UI.Scene`.
- **`SceneConversion.fs`**: the legacy bridge between `FS.Skia.UI.Scene` and `Lib`'s `FS.Skia.UI` types
  — deleted by this stage.
- **Parity oracle**: the Stage-0 committed deterministic scene-output golden fixtures
  (`tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt`) + reference frame, re-derived
  by the moved host as the merge gate.
- **Repointed consumers**: the legacy samples and affected tests moved off the deleted `Lib` modules
  onto the split packages.
- **`SkiaViewer` per-package surface baseline**:
  `readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt`, updated to record the host move.

## Assumptions

- **Scope (maintainer-confirmed):** Stage 1 deletes `Lib`'s host + duplicate scene vocabulary **and**
  pulls the *mechanical* sample/test repointing forward (the parts of plan Stages 3–4 needed to keep the
  build green). It does **not** relocate `AgentValidation` (Stage 2), **retire** `Parity.Tests`
  (Stage 4), or delete/unpublish `src/Lib`/`FS.Skia.UI` (Stage 5). The plan is updated to reflect this
  re-scoping.
- The parity oracle is the **committed Stage-0 golden** re-derived by the new host (deterministic
  scene-output authoritative; screenshots corroborate). Because the golden is already committed, a live
  old-vs-new dual-build is optional; if used, the dual-build flag is removed once parity is signed off.
- `SkiaViewer`'s net public surface is expected to be **stable** because the wrapper already re-exposed
  the host API; the per-package baseline update should be empty or a small recorded delta.
- The seed set for parity is the Stage-0 closed set (`basic-viewer`/`effects-gallery`/
  `screenshot-gallery`); `basic-viewer` has a real reference frame, the other two are scene-output-only
  (their reference frames were deferred at the Stage-0 pin).
- New/moved projects inherit `Directory.Build.props` (`net10.0`, `TreatWarningsAsErrors`,
  `FS0078`-as-error, Central Package Management); no new `PackageVersion` outside
  `Directory.Packages.props`.
- This is a **dogfood** feature; `Route` escalates it to the full serialized gate set, run in the
  deterministic FAKE-sequenced order (never concurrently).
