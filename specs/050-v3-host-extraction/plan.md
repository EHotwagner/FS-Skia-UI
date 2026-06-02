# Implementation Plan: V3 Stage 1 — KEYSTONE: Host Extraction & Scene-Vocabulary Unification

**Branch**: `050-v3-host-extraction` | **Date**: 2026-06-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/050-v3-host-extraction/spec.md`
**Programme source**: `docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md` §Stage 1

## Summary

Stage 1 is the **keystone** of the V3 monolith-retirement programme and the first stage that edits
runtime `src/**`. It moves the Vulkan/Skia **host** (`Viewer`, `Diagnostics`/`RenderDiagnostic`, the
internal `VulkanStartup`/`VulkanResources`, and the host body currently inside `Library.fs`'s
`VulkanHost` module) out of the `FS.Skia.UI` monolith (`src/Lib`) into the
`FS.Skia.UI.SkiaViewer` package, **retyped onto the canonical `FS.Skia.UI.Scene` vocabulary**. That
single move makes three things vanish together (FR-001…FR-005):

1. `src/SkiaViewer/SceneConversion.fs` — the bridge that exists *only* because the host spoke `Lib`'s
   `FS.Skia.UI` types while the public API spoke `FS.Skia.UI.Scene`. One vocabulary ⇒ no conversion.
2. the `SkiaViewer → Lib` `ProjectReference` — after the move `SkiaViewer` depends only on `Scene` +
   `KeyboardInput` + its native packages, so the packed `FS.Skia.UI.SkiaViewer` **no longer
   package-depends on `FS.Skia.UI`** (the modularity **leak is closed** — FR-004/SC-001/SC-003).
3. `Lib`'s duplicate scene vocabulary + host modules (`Colors`/`Paint`/`Path`/`Scene`/`Diagnostics`/
   `Viewer` + `VulkanHost`/`VulkanStartup`/`VulkanResources`) — deleted (FR-005/SC-004).

Because the legacy samples and the affected tests consume `Lib` via **`ProjectReference`** (source,
not the published package), deleting those `Lib` modules would break their compilation. Per the
maintainer-confirmed re-scoping, this stage also **pulls the mechanical sample/test repointing
forward** (work items 1.7–1.8, the parts of programme Stages 3–4 needed for greenness) so the full
serialized gate set stays green at Stage-1 exit (FR-006/SC-005). `Lib` after this stage retains only
`AgentValidation` (Stage 2), the duplicate `KeyboardInput` (dead once nothing references `Lib`), and
the `Parity` helper (retires Stage 4).

The **non-negotiable gate** is output parity: the moved-and-retyped host MUST re-derive the Stage-0
deterministic scene-output golden **byte-identically** for all three seeds before the legacy `Lib`
host source is deleted (FR-008/SC-002, ADR 0011). `Parity.Tests` is **repointed and retained** as the
Stage-1 parity harness — its *retirement* is Stage 4 (FR-007).

### Planning finding that shapes the approach (read before tasks)

The on-disk reality is more specific than "move three modules", and the difference drives the task
ordering:

- **`Library.fs` is a single 2,408-line file** whose `FS.Skia.UI` namespace interleaves the scene
  vocabulary (`Colors` 353–363, `Diagnostics` 364–400, `Paint` 401–450, `Path` 451–528, `Scene`
  529–729) with the parity helper (`Parity` 730–837) and the host (`VulkanHost` 838–2363,
  `Viewer` 2364–2409). The host body (`VulkanHost`) is **internal** and is the largest single block.
  The move is therefore a **file-surgery split of `Library.fs`/`Library.fsi`**, not a tidy
  module-file relocation — the host carves out to `src/SkiaViewer/Host/*.fs(i)` and the residue
  (`Parity` + the `RenderDiagnostic` type if still needed by `Parity`) stays in a shrunken
  `Library.fs(i)`. The compile order in `Lib.fsproj` (`VulkanResources` → `VulkanStartup` →
  `AgentValidation` → `Library` → `KeyboardInput`) means `VulkanResources.fs(i)` and
  `VulkanStartup.fs(i)` (separate files already) travel **whole** into `SkiaViewer`.

- **`SkiaViewer.fs` (122 KB) already re-exposes the host API** by wrapping `Lib.Viewer.*` and
  converting types through `SceneConversion.fs`. So the **net public surface of `SkiaViewer` should
  be stable** (FR-011/SC-007): the wrapper's outward `.fsi` largely survives; what changes is that
  its *implementation* now owns the host directly (no `Lib` call, no conversion). The realistic risk
  is a **small** surface delta where a previously-converted type is now the `Scene` type directly —
  that delta is recorded in the per-package baseline and verified by `PerPackageSurfaceDiff`.

- **`Scene` already owns the full canonical vocabulary** (`src/Scene/Scene.fsi`: `Vertex` 187,
  `VertexMode` 192, `TextRun` 174, `FontSpec` 168, `PerspectiveTransform` 144, `Scene`/`Colors`/
  `Paint`/`Path` modules) and is **FSharp.Core-only** (no `ProjectReference`/`PackageReference` in
  `Scene.fsproj`). Retyping is therefore a **substitution onto an existing target**, and FR-010/SC-006
  reduce to: do not add a `Scene → SkiaViewer` back-edge and do not add a heavy dependency to `Scene`.

- **`PerPackageSurfaceDiff` exists from Stage 0** (`build/Governance/PerPackageSurface.fs(i)`,
  `Target.PerPackageSurfaceDiff`) but is **runnable-only / not Route-gated** (its known-gate
  allowlist coupling to runtime `AgentValidation` is deferred to Stage 2/5). This stage **runs it
  explicitly** to record/verify the `SkiaViewer` baseline delta; it does **not** add a Routing rule
  (that stays deferred), so `validation.contract.yml` is unchanged.

### Parity sequencing (the merge gate governs deletion order)

Per ADR 0011 and FR-008, the legacy `Lib` host source MUST NOT be deleted until parity is clean. The
plan therefore sequences: **(a)** build the new host in `SkiaViewer` and retype it; **(b)** repoint
`Parity.Tests` onto the new host and prove **0-byte** scene-output diff vs the committed Stage-0
golden for `basic-viewer`/`effects-gallery`/`screenshot-gallery`; **only then (c)** delete `Lib`'s
host + duplicate scene modules and sever the reference. The optional dual-build flag from the
programme plan (old host retained behind a flag for side-by-side) is **not required** because the
oracle is already committed; if a contributor uses one for local A/B, it is removed at sign-off.

## Technical Context

**Language/Version**: F# / .NET `net10.0`. New/moved projects inherit `Directory.Build.props`
(`TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management); **no** new `PackageVersion`
outside `Directory.Packages.props` — the native packages the host needs (`Silk.NET.*`, `SkiaSharp*`)
are already referenced by both `Lib.fsproj` and `SkiaViewer.fsproj`, so the move adds **no** package
references to `SkiaViewer`, it only removes the `Lib` `ProjectReference`.
**Primary Dependencies**: no new runtime dependency. The host's native stack (`Silk.NET.Input`,
`Silk.NET.Vulkan`(+`.Extensions.KHR`), `Silk.NET.Windowing`(+`.Extensions`), `SkiaSharp`(+Linux/Win32
native assets), `Fable.Elmish`) already exists on `SkiaViewer.fsproj`. `Scene` stays FSharp.Core-only.
**Testing**: Expecto semantic tests through the public `.fsi`; the **byte-identical scene-output
parity test** (`Parity.Tests`, deterministic, authoritative); the **native startup/cleanup tests**
travelling with the host into `SkiaViewer`; reference-frame corroboration for `basic-viewer` where
headless capture is feasible; the full serialized escalated FAKE gate set.
**Target Platform**: Windows and Linux. Headless Linux capture is subject to the known
`SkiaViewer.Tests` libdecor-gtk flake — **scene-output is the authoritative parity oracle**;
screenshots corroborate; a focused rerun is authoritative over the aggregate run (recorded flake
guidance).
**Stage-0 oracle pin**: scene-output golden at
`tests/Parity.Tests/fixtures/v3-host-golden/scene-output/{basic-viewer,effects-gallery,screenshot-gallery}.txt`;
reference frame `.../screenshots/basic-viewer.png`; environment `.../capture-environment.md`.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design (below).*

**Change classification**: **Tier 1 (contracted change)** for the **runtime**: it moves public
runtime surface between packages (`SkiaViewer.fsi` internals change; `Library.fsi` shrinks), changes
the packed `FS.Skia.UI.SkiaViewer` dependency set (drops `FS.Skia.UI`), and deletes runtime modules.
It therefore requires the full chain: `.fsi` updates, the `SkiaViewer` surface-baseline update, parity
+ native test evidence, and migration-note-equivalent ADR alignment (0007/0008/0011 already lock the
shape). **Behaviour is preserved** (parity is the proof). `Route` escalates this `src/**/*.fsi`-touching,
consumer-contract change to the **dogfood** full serialized gate set.

### Repository Governance Decisions

- **Template ownership**: **No `template.json` change; the default `app` profile is validated, not
  edited.** No `template/**` fragment, capability, or package-policy authoring change. The host move
  is internal to the `SkiaViewer` package the `app` profile already consumes by **package**, so the
  generated `app`'s project files do not change — but its **transitive graph does** (the monolith
  drops out). `TemplateCheck` MUST stay green and MUST show the default `app` still
  restoring/building/running (FR-009/SC-008); the leak-proof dump MUST show `FS.Skia.UI` absent from
  the generated `app`'s resolved graph (FR-009/SC-003). The template's package **pins** are unchanged
  in this stage (no version bump; versions bump at merge per the merge skill, not here).
- **Dependency impact**: **Net dependency *reduction*, no additions.** `Directory.Packages.props` is
  unchanged (no new `PackageVersion`). `SkiaViewer.fsproj` **loses** `<ProjectReference Include="..\Lib\Lib.fsproj" />`
  and gains nothing (its native packages already present). The packed `FS.Skia.UI.SkiaViewer` package
  **loses** its `FS.Skia.UI` package dependency (SC-001). `docs/reports/dependencies.md` /
  `DependencyReport` coverage: the dependency graph *shrinks*; if `DependencyReport` snapshots the
  per-package dependency set it is regenerated to drop the `SkiaViewer → FS.Skia.UI` edge — verify and
  update if the report tracks it. The leak-proof dump (Stage-0 reproduction command) is the evidence.
- **Command-surface impact**: **No new build target; no `build.fsx`/front-end behaviour change.** The
  host move runs under the **existing** escalated serialized gate set. `PerPackageSurfaceDiff` (Stage-0
  target) is **invoked explicitly** to record/verify the `SkiaViewer` baseline delta; **no Routing
  rule is added** (the Stage-0 deferral stands), so `validation.contract.yml` is unchanged and
  `TargetMetadataDrift`/`ContractView` currency holds. No change to the behaviour of `Dev`, `Verify`,
  `Ci`, `PackLocal`, `TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`,
  `DependencyReport`, `EvidenceGraph`, `EvidenceAudit`, or the aggregate `PackageSurfaceCheck`.
  FAKE-backed commands share `.fake` state — run **sequentially** in the deterministic order; safe
  non-FAKE reads may still parallelize. Serialized order:
  1. `./fake.sh build -t Route` (confirm escalation + required artifacts; `--enforce` to fail on a missing one)
  2. `./fake.sh build -t Dev`
  3. `./fake.sh build -t PerPackageSurfaceDiff` (record/verify the `SkiaViewer` baseline delta — SC-007)
  4. `./fake.sh build -t GeneratedGuidanceCheck`
  5. `./fake.sh build -t TemplateCheck`
  6. `./fake.sh build -t GeneratedProductCheck`
  7. `./fake.sh build -t EvidenceGraph`
  8. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: **Behaviour preserved, graph lighter.** Default/minimal generated
  contents, selected Controls guidance, local skills, validation logs, and placeholder/excluded-history
  scans are unchanged. The only observable change to a generated `app` is that its resolved transitive
  graph **no longer contains** `FS.Skia.UI` (the leak closes) — proven by the leak-proof dump and a
  green `TemplateCheck`/`GeneratedProductCheck`. The `fs-skia-skiaviewer` capability skill
  (`src/SkiaViewer/skill/SKILL.md`) is reviewed for any host-ownership wording that the move makes
  stale and updated if needed (it speaks to viewer host contracts).
- **Evidence paths**: exact readiness/output paths —
  - Parity (the merge gate): re-derived scene-output diff vs `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/*.txt`,
    recorded at `specs/050-v3-host-extraction/readiness/parity-scene-output-diff.md` (0-byte for all
    three seeds); reference-frame corroboration / recorded infeasibility at
    `specs/050-v3-host-extraction/readiness/parity-reference-frame.md`.
  - Leak proof (FR-004/009): `specs/050-v3-host-extraction/readiness/leak-proof.md` — the Stage-0
    reproduction command showing (a) `FS.Skia.UI.SkiaViewer` packed graph has no `FS.Skia.UI`, and
    (b) the generated default `app` resolves without `FS.Skia.UI`.
  - Surface baseline (FR-011/SC-007): updated `readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt`;
    the `PerPackageSurfaceDiff` run recorded at `specs/050-v3-host-extraction/readiness/per-package-surface-diff.md`
    (clean against the updated baseline; any net delta explicitly justified there).
  - Native startup/cleanup (FR-012): the startup-cleanup test results recorded with the `Dev`/host
    smoke run at `specs/050-v3-host-extraction/readiness/native-startup-cleanup.md`.
  - Persistent viewer launch (FR-009/SC-005): repointed `BasicViewer` visible-window / first-frame
    evidence at `specs/050-v3-host-extraction/readiness/window-visibility.md` (or the recorded
    GPU-passthrough infeasibility note per Principle V).
  - Template validation (FR-009/SC-008): the `TemplateCheck` result (default `app`
    restores/builds/runs, monolith absent from its transitive graph) at
    `specs/050-v3-host-extraction/readiness/template-check-validation.md`.
  - FSI transcript: `specs/050-v3-host-extraction/readiness/fsi/skiaviewer-host.txt` (exercise the
    moved host's public surface — `create`/`run`/`defaultConfiguration` — through the packed/loaded
    `SkiaViewer` surface).
  - Standard evidence-gate output: `specs/050-v3-host-extraction/readiness/{evidence-graph.md,evidence-audit.md,validation-contract.md,aggregate-hang-diagnostics.md}`.
- **`.fsi` / contract impact**: **Two runtime `.fsi` files change; net public surface should be
  stable.** New `src/SkiaViewer/Host/*.fsi` curate the moved host's surface (Principle II — every
  moved public module gets its `.fsi`). `src/SkiaViewer/SkiaViewer.fsi` is re-pointed internally onto
  the `Scene` vocabulary and the in-package host; its **outward** signatures should be unchanged
  because the wrapper already re-exposed the host API. `src/Lib/Library.fsi` **shrinks** (host +
  duplicate-scene declarations removed; `Parity`/residue retained). `SceneConversion.fs` (an internal
  bridge, not in any `.fsi`) is deleted. The `SkiaViewer` per-package surface baseline is updated to
  record the move; any net delta is recorded and justified. No documented public API of `Scene`
  changes. Compatibility note: consumers using `SkiaViewer` by **package** are unaffected (surface
  stable, dependency lighter); consumers using `Lib`'s host/scene by **`ProjectReference`** migrate to
  `Scene` + `SkiaViewer` — that migration is performed in-tree by FR-006.
- **MVU/effect boundary**: **The host *is* the Elmish edge; its boundary is preserved, not
  redesigned.** `Viewer` exposes `create`/`run`/`withEventMapping`/`withEffectMapping`/
  `withSubscription`/`defaultConfiguration` — the Elmish `Program`/`Cmd`/subscription integration at
  the runtime edge (Principle IV). The move **relocates** this edge between packages with **identical
  function shapes** (FR-001); `update` purity and the effect-at-the-edge contract are unchanged. No
  new `Model`/`Msg`/`Cmd` is introduced. Parity (byte-identical scene-output + native startup/cleanup
  tests) is the real-interpreter evidence that the boundary behaves identically post-move.
- **Synthetic evidence**: **None planned — all evidence is real.** Parity re-derives from the **real
  moved host** against the **real committed Stage-0 golden** (byte equality, not a mock). The leak
  proof reads the **real** packed graph and the **real** generated `app`. Native startup/cleanup tests
  exercise the **real** Vulkan/Skia host. No `[S]`/`[S*]`/`[SEH]` task is anticipated; `EvidenceAudit`
  MUST return PASS on zero-synthetic evidence (SC-008). If headless **reference-frame** capture is
  infeasible in CI, that is disclosed per Principle V at `readiness/parity-reference-frame.md`
  (scene-output remains authoritative) — it is **not** faked.
- **Test evidence**: failing-first / behaviour-preserving semantics —
  (a) **parity** (FR-008/SC-002): with the new host wired, `Parity.Tests` scene-output diffs **0
  bytes** vs the Stage-0 golden for all three seeds; this test must be **red** if the retyped host
  diverges (the edge-case guard) and green only on exact match — it gates deletion of the `Lib` host.
  (b) **native startup/cleanup** (FR-012): the existing startup-cleanup tests, moved with the host,
  pass in `SkiaViewer` with unchanged lifetime behaviour.
  (c) **repointed-consumer build** (FR-006/SC-005): every repointed sample + test restores/builds/runs
  green against the split packages; `Lib.Tests`/`Smoke.Tests`/`Package.Tests`/`Parity.Tests` compile
  against `Scene`+`SkiaViewer` (+`Elmish` where used); `Governance.Tests`/`ParityGallery` keep a
  reduced `Lib` reference (`AgentValidation`/`Parity` only).
  (d) **surface** (FR-011/SC-007): `PerPackageSurfaceDiff` is clean against the updated `SkiaViewer`
  baseline; the aggregate `PackageSurfaceCheck` stays green.
  (e) **leak** (FR-004/FR-009): the leak-proof dump shows `FS.Skia.UI` absent from
  `FS.Skia.UI.SkiaViewer` and from the generated `app`.
- **Observability**: the host's existing structured startup/subsystem/asset-failure diagnostics
  (`RenderDiagnostic`, the `Diagnostics` module) travel with it **unchanged** — operationally
  significant events still emit actionable context and still fail fast (Principle VII). The parity
  test fails **loud** with the seed name and the first divergent scene-output line + golden path on
  any diff. The leak-proof and surface-diff runs name the exact reproduction command and the artifact
  to update on failure. No silent-pass path is introduced by the move.
- **Deferred scope**: explicitly **out of this feature** (later programme stages) — `AgentValidation`
  relocation into the governance library (Stage 2); **retirement** of the `Parity.Tests`/`Parity`
  bridge (Stage 4, after parity sign-off — here it is *repointed and retained*); `src/Lib` deletion +
  `FS.Skia.UI` unpublish + per-package Route-gating/hard enforcement + the generated-project
  cleanliness gate (Stage 5); the separate `FS.Skia.UI.Charts` split; any new rendering architecture;
  any template-profile expansion. Adding a Routing rule for `PerPackageSurfaceDiff` remains deferred
  (Stage-0/Stage-5 finding). No FCS / dynamic compilation / runtime script-loading is introduced
  (carried invariant 7 / FR-013).

## Project Structure

```
specs/050-v3-host-extraction/
  spec.md                     # (exists)
  plan.md                     # this file
  research.md                 # Phase 0 — host-split boundary, retyping map, parity sequencing, repoint matrix
  data-model.md               # Phase 1 — entities: Host modules, retype map, parity oracle, repoint matrix, surface delta
  quickstart.md               # Phase 1 — how to run parity, the leak proof, the surface diff, the gate set
  contracts/
    host-extraction.md            # contract: moved host surface, retype substitution, deletion preconditions
    repoint-matrix.md             # contract: each consumer's before/after references + acceptance
  checklists/
    requirements.md           # (exists)
  readiness/                  # evidence (parity/leak/surface/native/fsi/graph/audit) — created during implement

# Runtime source moved / changed (the keystone edits)
src/SkiaViewer/Host/Vulkan.fs(i)            # VulkanResources + VulkanStartup + VulkanHost body, retyped onto Scene
src/SkiaViewer/Host/Diagnostics.fs(i)       # Diagnostics / RenderDiagnostic (if not retained by Lib.Parity)
src/SkiaViewer/Host/Viewer.fs(i)            # Viewer: create/run/withEventMapping/withEffectMapping/withSubscription/defaultConfiguration
src/SkiaViewer/SkiaViewer.fsi               # re-pointed onto Scene + in-package host (net surface stable)
src/SkiaViewer/SkiaViewer.fs                # wrapper now owns the host directly (no Lib call, no conversion)
src/SkiaViewer/SceneConversion.fs           # DELETED (bridge no longer needed)
src/SkiaViewer/SkiaViewer.fsproj            # Lib ProjectReference REMOVED; Host/*.fs(i) added; SceneConversion removed
src/Lib/Library.fs(i)                       # SHRUNK: Colors/Paint/Path/Scene/Diagnostics/Viewer/VulkanHost removed; Parity (+residue) retained
src/Lib/VulkanResources.fs(i)               # MOVED into src/SkiaViewer/Host
src/Lib/VulkanStartup.fs(i)                 # MOVED into src/SkiaViewer/Host
src/Lib/Lib.fsproj                          # VulkanResources/VulkanStartup compile items removed

# Repointed consumers (pulled forward from programme Stages 3–4 — re-scoping)
samples/BasicViewer/BasicViewer.fsproj            # Lib → Scene + SkiaViewer
samples/EffectsGallery/EffectsGallery.fsproj      # Lib → Scene + SkiaViewer
samples/ScreenshotGallery/ScreenshotGallery.fsproj# Lib → Scene + SkiaViewer
samples/InteractiveViewer/InteractiveViewer.fsproj# Lib → Scene + SkiaViewer (+ Elmish where used)
samples/DemoReel/DemoReel.fsproj                  # drop Lib (already refs SkiaViewer/Layout/Controls/Elmish)
tests/Lib.Tests/Lib.Tests.fsproj                  # Lib → Scene + SkiaViewer (host/scene assertions)
tests/Smoke.Tests/Smoke.Tests.fsproj              # Lib → Scene + SkiaViewer
tests/Package.Tests/Package.Tests.fsproj          # Lib (host/scene) → Scene + SkiaViewer
tests/Parity.Tests/Parity.Tests.fsproj            # repointed to drive the moved host; RETAINED as parity harness (not retired)
# Reduced-reference (kept until their later stage):
tests/Governance.Tests/Governance.Tests.fsproj    # keeps Lib → AgentValidation only (Stage 2)
samples/ParityGallery/ParityGallery.fsproj        # keeps Lib → Parity helper only (Stage 4)

# Surface baseline updated (descriptive of the moved surface)
readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt   # records the host move; net delta empty-or-justified
```

**Reference graph after Stage 1 (intent):** `SkiaViewer → {Scene, KeyboardInput}` + native packages;
`Scene → ∅` (FSharp.Core-only, unchanged); **no** `SkiaViewer → Lib` and **no** `Scene → SkiaViewer`
back-edge (FR-010/SC-006). `Lib` still depends on its native stack (for the residual `KeyboardInput`
duplicate + `Parity`) but **nothing in `src/**` references `Lib` for host/scene** after repointing;
`Governance.Tests`/`ParityGallery` reference only `Lib`'s `AgentValidation`/`Parity` residue.

## Validation (escalated serialized gate set)

`Route` escalates this change (consumer-contract + `src/**/*.fsi`; dogfood). Run FAKE-backed targets
**sequentially** in the order under *Command-surface impact* above, with the **parity merge gate**
ordering enforced: build + retype the new host and prove the **0-byte** `Parity.Tests` scene-output
diff **before** deleting the legacy `Lib` host source (ADR 0011 / FR-008). The aggregate
`PackageSurfaceCheck` must remain green and unweakened; `PerPackageSurfaceDiff` must be clean against
the updated `SkiaViewer` baseline. Re-run any race-like FAKE failure sequentially before product
debugging; for the known headless flake, a focused `Parity.Tests`/host rerun is authoritative over the
aggregate run, with scene-output as the primary oracle.

## Post-Design Constitution Re-Check

After Phase 1 (data-model + contracts), no new violations: the host's Elmish edge moves with
**identical function shapes** and a pure-`update`/effect-at-edge contract (Principle IV preserved, not
redesigned); every moved public module carries a curated `.fsi` and the `SkiaViewer` surface baseline
is updated (Principle II / Tier 1); the change is **behaviour-preserving** with real, zero-synthetic
parity + native + leak evidence and a failing-on-divergence parity gate (Principles V/VI); structured
host diagnostics travel unchanged and the parity/leak/surface runs fail loud with actionable context
(Principle VII); no FCS/dynamic compilation is introduced (FR-013). The single Tier-1 obligation —
runtime `.fsi` + baseline updates — is planned and gated. **PASS.**
