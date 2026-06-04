# Phase 0 Research: Asteroids Consumer Friction Follow-ups

This research resolves the open approach decisions in the spec. Each finding is
grounded in the current working tree, not the report's `0.1.62-preview.1`
snapshot.

## D1. Is 059's `resolveFeatureDir` already in the tree? (FR-001/FR-002)

- **Decision**: 059 is **merged** (commit `ce9ba61 Merge 059-speckit-tasks-validation-feedback (squash)`).
  `template/base/build.fsx` already defines `resolveFeatureDir` (SPECKIT_FEATURE_DIR
  override → `.specify/feature.json` → loud fail, no bundled-sample fallback) and the
  `EvidenceGraph`/`EvidenceAudit` runners already `printfn "feature-directory=%s"`
  and `printfn "tasks=%d"`. `template/base/Directory.Packages.props` already pins
  every `FS.Skia.UI.*` at `0.1.63-preview.1`.
- **Rationale**: The report observed `0.1.62`, which predates the merge. The fix is
  *implemented*; the remaining obligations are (a) **ship**: confirm the template
  package is packed at the current version, installed, and that a freshly generated
  project exhibits the behavior, and (b) **prove it** with a verification log.
- **Consequence**: FR-001 is reframed from "implement" to "verify end-to-end in a
  generated project + capture the echoed `feature-directory=`/`tasks=` and the loud-
  failure paths." FR-002 is "bump (if needed) + `TemplatePack` + install + regen log."
- **Alternatives considered**: Re-implementing the resolver — rejected; it exists and
  is already gate-covered by `GeneratedProjectValidationTests`.

## D2. How to provide the authoritative API surface in generated projects (FR-003)

- **Decision**: **Emit** `docs/api-surface/<Pkg>/<file>.fsi` into generated projects
  by generating the tree into `template/base/docs/api-surface/` from the existing
  `template/capabilities.yml` `contracts:` lists (the canonical Spec→`.fsi` mapping
  already names every consumed `.fsi`). Generation is a code-generated step in
  `FS.Skia.UI.Build` with a **currency check** (regenerate via
  `RefreshSurfaceBaselines`, enforce no-drift like the skill tree / contract.yml).
- **Rationale**: `capabilities.yml` already lists each capability's `contracts:`
  (`src/Scene/Scene.fsi`, `src/KeyboardInput/KeyboardInput.fsi`, …). Copying those
  verbatim into the template docs tree is single-source (no hand-sync) and keeps the
  emitted surface byte-identical to the real signatures. Every product-skill names a
  single-file package whose `.fsi` last-segment matches `<Pkg>.fsi`
  (`Scene/Scene.fsi`, `KeyboardInput/KeyboardInput.fsi`, `Elmish/Elmish.fsi`,
  `SkiaViewer/SkiaViewer.fsi`, `Testing/Testing.fsi`), so the skill-claimed path is
  exactly the emitted path.
- **Alternatives considered**:
  - *Repoint skills at a packaged ref `.fsi`* — rejected: NuGet ref assemblies are not
    a readable `.fsi`, and consumers cannot "read the union case field order locally"
    from a DLL, which is the whole point the skills promise.
  - *Hand-author the docs tree* — rejected: drifts from real signatures the moment a
    `.fsi` changes; violates the single-source rule.
- **Scope note**: Multi-`.fsi` packages (Layout, Controls) emit all their
  `contracts:` files under `docs/api-surface/<Pkg>/`; the product-skill only needs to
  name a path that exists in the emitted set. The default `app` profile emits the
  surface for the packages it pins.

## D3. Anti-drift check for skill-claimed contract paths (FR-004)

- **Decision**: Add a governance rule (`SkillContractPathCheck`, or fold into
  `GeneratedProductCheck`/`TemplateCheck`) that fails when any capability/product
  skill references a `docs/api-surface/...fsi` path **not** present in the emitted
  `template/base/docs/api-surface/` tree, and conversely flags an emitted file no
  skill claims (orphan). Also fails if any skill claims "no DLL reflection needed"
  against an absent path.
- **Rationale**: Closes the F2/F9 root cause: skill claims and generated output drift
  apart silently. A path-equality check between the skills' named sources and the
  emitted tree makes drift a red build. Mirrors the existing `SkillSyncCheck` /
  `TargetMetadataDrift` currency pattern.

## D4. Splitting generated tests (FR-005)

- **Decision**: Split `template/base/tests/Product.Tests/Tests.fs` (570 lines, mixes
  `productSource`-reading governance/source-structure scans with scaffold-behavior
  tests) into two compilation units in the same fsproj: **`GovernanceTests.fs`**
  (durable, model-agnostic source/structure/visual-evidence-guidance scans) and
  **`BehaviorTests.fs`** (replaceable scaffold `view`/`update`/scene-text behavior).
- **Rationale**: A consumer who swaps the scaffold model rewrites only
  `BehaviorTests.fs`; the governance scans keep compiling/running. Both files keep
  their `//#if (profile == ...)` conditionals. `Product.Tests.fsproj` compile order
  lists `GovernanceTests.fs` before `BehaviorTests.fs`. `TemplateCheck` /
  `GeneratedProductCheck` source-structure assertions update to the two filenames.
- **Alternatives considered**: A second test *project* — rejected as heavier than the
  friction warrants; one fsproj with two files is the minimal separation.

## D5. Keyboard skill correction (FR-006)

- **Decision**: Rewrite `template/product-skills/fs-skia-keyboard-input/SKILL.md`
  (canonical edit in `.agents`/template source) so the Usage example shows **only**
  the `mapKey : ViewerKey -> bool -> Msg option` boundary the `app` profile's
  `generatedHost` actually threads. Remove the `Keyboard.init bindings` /
  `KeyboardEffect` reducer flow as the consumer path; the "Public Contract" line
  points at the (now-emitted) `docs/api-surface/KeyboardInput/KeyboardInput.fsi` and
  describes only what the host uses.
- **Rationale**: F9 — following the skill verbatim must compile against the real host
  contract without an unused reducer abstraction (SC-004).

## D6. Pitfall notes (FR-007) and HUD/gameplay pattern (FR-008)

- **Decision (FR-007)**: Add a "Common pitfalls" section to `fs-skia-scene` (and the
  keyboard skill where relevant): (a) consumer geometry records (`Vec2`) colliding
  with framework `Point`/`Rect`, with the conversion note; (b) duplicate DU case
  names across co-opened modules (`ViewerKey.Unknown` vs
  `ViewerRunBlockedStage.Unknown`) with the fully-qualified resolution.
- **Decision (FR-008)**: Document the HUD/gameplay-region pattern in
  `fs-skia-layout-readability` (the 059 split target): reserve a HUD band; confine or
  clamp gameplay bounds to the gameplay region; overdraw the HUD.
- **Rationale**: Consumers reach the intended pattern by design, not gate-driven trial
  and error.

## D7. Template-update skill currency (FR-009 / SC-006)

- **Decision**: (a) Correct the skill text — remove the phantom bare-Lib
  `FS.Skia.UI.$v.nupkg` check (053 deleted `src/Lib`/unpublished `FS.Skia.UI`); add
  `FS.Skia.UI.SkillSupport` and `FS.Skia.UI.Input` to the step-5 feed-verification
  loop; correct the "nine repo packages" count. (b) Add a governance check
  (`TemplateUpdateSkillPackageCheck`) that diffs the skill's enumerated package IDs
  against the **packable `.fsproj` set** (`src/*` + `build/Governance/FS.Skia.UI.Build`)
  so the list cannot drift again.
- **Rationale**: SC-006 demands exact equality with the packable set. The packable set
  is currently: `FS.Skia.UI.Build` (build/Governance) + Scene, SkiaViewer, Elmish,
  KeyboardInput, **Input**, Layout, Controls, Controls.Elmish, Testing, **SkillSupport**
  (src). `Input` is packable but *not* template-pinned nor a registered capability — so
  the step-5 *feed* loop (verifies what was packed) must include it, while the
  *props-pin* enumeration (step 3) tracks only the pinned set. The check distinguishes
  the two enumerations.
- **Alternatives considered**: Text-only fix — rejected; SC-006/FR-009 explicitly
  require it be derived/checked so it "cannot drift."

## D8. Process/authoring follow-ups F6/F7 → FR-010/FR-011

- **Decision**: Treat as authoring-template/guidance improvements, not new hard merge
  gates (spec defers unless a low-cost check is found). FR-010 (SC→assertion mapping):
  add guidance + the split governance test demonstrating an enforcing assertion for a
  mechanically-testable SC; FR-011 (interacting-requirement notes): add a note to the
  spec-authoring guidance. No new blocking gate is introduced for these.
- **Rationale**: The spec scopes F6/F7/F10/F11 as process guidance; FR-004 and the
  split tests already supply the executable backbone.

## D9. Skill canonical-source workflow (FR-012)

- **Decision**: All skill edits land in canonical sources — `.agents/skills/**` for
  repo-local governance skills (`fs-skia-template-update`, `fs-skia-layout-readability`,
  `fs-skia-evidence-mode`) and `template/product-skills/**` / `src/*/skill/SKILL.md`
  for capability skills — then regenerate `.claude` via
  `./fake.sh build -t RefreshSurfaceBaselines`. `SkillSyncCheck` /
  `TargetMetadataDrift` / `SkillQualityCheck` must stay green.
- **Rationale**: Established 053/058 precedent; enforced, not optional.

## D10. Routing / gate set

- **Decision**: This change touches `template/**`, `.agents/skills/**`,
  `template/product-skills/**`, `build/Governance/**`, and (regenerated) `.claude/**`,
  so `Route` **escalates** to the `maintainer-verify` tier. Authoritative gate list
  comes from `./fake.sh build -t Route`; expected to include `Dev`,
  `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, the new
  api-surface/skill-path/template-update-package checks, `SkillSyncCheck`,
  `TargetMetadataDrift`, `SkillQualityCheck`, `EvidenceGraph`, `EvidenceAudit`.
- **Rationale**: Governance + consumer-contract paths escalate automatically per
  `Routing.fs`.
