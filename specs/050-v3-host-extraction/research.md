# Phase 0 Research — V3 Stage 1 Host Extraction & Scene-Vocabulary Unification

All NEEDS CLARIFICATION are resolved (the spec carried none; these decisions resolve the *how* the
plan depends on). Decisions are recorded as Decision / Rationale / Alternatives.

## D1 — Host-split boundary inside `Library.fs`

**Decision.** Carve the host out of the single `src/Lib/Library.fs` (namespace `FS.Skia.UI`,
2,408 lines) along these module boundaries into `src/SkiaViewer/Host/*.fs(i)`:
- `Viewer` (2364–2409) → `src/SkiaViewer/Host/Viewer.fs(i)` (public — preserves `create`/`run`/
  `withEventMapping`/`withEffectMapping`/`withSubscription`/`defaultConfiguration`).
- `VulkanHost` (838–2363, internal host body) + the already-separate-file `VulkanStartup.fs(i)` and
  `VulkanResources.fs(i)` → `src/SkiaViewer/Host/Vulkan.fs(i)` (internal). The two separate files
  travel whole; the in-`Library.fs` `VulkanHost` block is relocated alongside them.
- `Diagnostics`/`RenderDiagnostic` (364–400) → `src/SkiaViewer/Host/Diagnostics.fs(i)` **unless** the
  residual `Parity` helper still needs `RenderDiagnostic`, in which case the type's canonical home is
  decided in implementation (prefer `Scene` if it is genuinely a scene-output type, else keep a
  minimal copy with `Parity` in `Lib`). Resolve by following the actual reference at edit time.

Residue staying in a shrunken `Library.fs(i)`: `Parity` (730–837) and any minimal type it requires.
Deleted from `Lib`: the duplicate scene vocabulary `Colors` (353–363), `Paint` (401–450), `Path`
(451–528), `Scene` (529–729), plus `Diagnostics`/`Viewer`/`VulkanHost` once moved.

**Rationale.** `Library.fs` interleaves four concerns in one namespace; the move is file surgery, not
a clean file relocation. Splitting on the existing module boundaries keeps each moved unit's `.fsi`
small and reviewable and matches the compile order already in `Lib.fsproj`
(`VulkanResources`→`VulkanStartup`→…→`Library`).

**Alternatives considered.** *Move the whole `Library.fs` verbatim into SkiaViewer* — rejected: it
would drag `Parity` and the duplicate scene types into `SkiaViewer`, defeating the deduplication.
*Leave `VulkanHost` in `Lib` and reference it* — rejected: that keeps the `SkiaViewer → Lib` edge and
the leak open.

## D2 — Retyping substitution map (`Lib`'s `FS.Skia.UI` types → `FS.Skia.UI.Scene`)

**Decision.** Replace every host-internal use of `Lib`'s scene types with the `Scene` package
equivalent: `Vertex`→`FS.Skia.UI.Scene.Vertex` (fsi 187), `VertexMode`→`…VertexMode` (192),
`TextRun`→`…TextRun` (174), `FontSpec`→`…FontSpec` (168), `PerspectiveTransform`→
`…PerspectiveTransform` (144), and the `Scene`/`Paint`/`Path`/`Colors` modules → the matching
`FS.Skia.UI.Scene` modules (fsi `Colors` 365, `Paint` 378, `Path` 393, `Scene` 404). The host's
rendering reads scene values; with one vocabulary the read is direct and `SceneConversion.fs` is
deleted (FR-003).

**Rationale.** `Scene` already owns the canonical, byte-stable vocabulary the public API speaks. The
bridge existed only to translate between the two copies; deleting one copy deletes the need to bridge.

**Alternatives considered.** *Keep the conversion but point both sides at `Scene`* — rejected:
self-identity conversion is dead code. *Promote `Lib`'s copy as canonical* — rejected by ADR 0008
(scene vocabulary single source is `Scene`).

## D3 — Parity sequencing and the deletion precondition (the merge gate)

**Decision.** Order strictly: (1) build the new host in `SkiaViewer`, retype it, delete
`SceneConversion.fs`, sever the `Lib` reference; (2) repoint `Parity.Tests` onto the moved host and
prove **0-byte** scene-output diff vs the committed Stage-0 golden for all three seeds
(`basic-viewer`/`effects-gallery`/`screenshot-gallery`); (3) only after (2) is clean, delete `Lib`'s
host + duplicate-scene modules (FR-005). The legacy host source is **not** removed until parity is
clean (ADR 0011 / FR-008 / SC-002).

**Rationale.** The byte-identical gate is the only proof the relocation is behaviour-preserving rather
than asserted. Deleting the old source first would discard the side-by-side oracle.

**Alternatives considered.** *Dual-build flag retaining the old host for live A/B* — optional only;
the Stage-0 golden is already committed, so a live old-vs-new build is unnecessary. If a contributor
uses one locally, it is removed at sign-off.

## D4 — Parity oracle: scene-output authoritative, screenshots corroborate

**Decision.** The authoritative signal is the deterministic **scene-output** golden
(`tests/Parity.Tests/fixtures/v3-host-golden/scene-output/*.txt`), diffed byte-for-byte. The
`basic-viewer` **reference frame** (`.../screenshots/basic-viewer.png`) corroborates where headless
capture is feasible; `effects-gallery`/`screenshot-gallery` are scene-output-only (their reference
frames were deferred at the Stage-0 pin). Headless capture infeasibility is **disclosed**, not faked
(Principle V), at `readiness/parity-reference-frame.md`.

**Rationale.** The known `SkiaViewer.Tests` libdecor-gtk headless flake can mask/mimic a regression;
deterministic scene-output is immune to it. A focused rerun is authoritative over the aggregate.

**Alternatives considered.** *Screenshot-primary parity* — rejected: flake-sensitive and not
byte-deterministic across environments.

## D5 — Repoint matrix (consumers off the deleted `Lib` modules)

**Decision.** Repoint by current dependency reason:

| Consumer | Today refs | After Stage 1 |
|---|---|---|
| `samples/BasicViewer` | `Lib` | `Scene` + `SkiaViewer` |
| `samples/EffectsGallery` | `Lib` | `Scene` + `SkiaViewer` |
| `samples/ScreenshotGallery` | `Lib` | `Scene` + `SkiaViewer` |
| `samples/InteractiveViewer` | `Lib` | `Scene` + `SkiaViewer` (+ `Elmish` if used) |
| `samples/DemoReel` | `Lib`,`SkiaViewer`,`Layout`,`Controls`,`Elmish` | drop `Lib`; keep the rest |
| `tests/Lib.Tests` | `Lib` | `Scene` + `SkiaViewer` |
| `tests/Smoke.Tests` | `Lib` | `Scene` + `SkiaViewer` |
| `tests/Package.Tests` | `Lib`(+Layout/Controls cond.) | `Scene` + `SkiaViewer` for host/scene; keep others |
| `tests/Parity.Tests` | `Lib` | repointed to drive moved host + `Parity` residue; **retained** |
| `tests/Governance.Tests` | `Lib`,`FS.Skia.UI.Build` | keep `Lib` → **`AgentValidation` only** (Stage 2) |
| `samples/ParityGallery` | `Lib` | keep `Lib` → **`Parity` helper only** (Stage 4) |

**Rationale.** Consumers split into "used the deleted host/scene" (must move to `Scene`+`SkiaViewer`)
vs "used only surface that stays in `Lib`" (`AgentValidation`/`Parity` — keep a reduced reference
until that surface's own stage). This is the minimal repointing that keeps the tree green (FR-006).

**Alternatives considered.** *Repoint everything off `Lib` now* — rejected: `Governance.Tests` and
`ParityGallery` legitimately consume `Lib`'s `AgentValidation`/`Parity`, which do not move in Stage 1.

## D6 — `SkiaViewer` surface stability and the baseline update

**Decision.** Treat the `SkiaViewer` net public surface as **expected-stable** (the wrapper already
re-exposed the host API) and update `readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt` to
match the post-move `.fsi`. Run `PerPackageSurfaceDiff` explicitly; any **net** delta (e.g. a formerly
converted type now surfaced as the `Scene` type directly) is recorded and justified in
`readiness/per-package-surface-diff.md` and accepted only if it reflects the intended unification.

**Rationale.** FR-011/SC-007 require the move recorded in the per-package baseline with a clean diff.
Stage 0 built `PerPackageSurfaceDiff` precisely for signature-sensitive per-package drift; this is its
first real consumer.

**Alternatives considered.** *Rely on the aggregate `PackageSurfaceCheck`* — rejected: it is
type-name-only and monolith-inclusive (Stage-0 D1/D2), so it cannot see a moved signature.

## D7 — Dependency/native-package handling

**Decision.** Add **no** package references to `SkiaViewer`; only **remove** the `Lib`
`ProjectReference`. The host's native stack (`Silk.NET.*`, `SkiaSharp*`, `Fable.Elmish`) is already on
`SkiaViewer.fsproj`. Remove the `VulkanResources`/`VulkanStartup` compile items from `Lib.fsproj` and
add the moved `Host/*` items to `SkiaViewer.fsproj` in dependency-correct compile order
(`Vulkan` → `Diagnostics` → `Viewer` → `SkiaViewer`). `Directory.Packages.props` is untouched.

**Rationale.** Central Package Management already pins these versions and both projects already
reference them; the move is a reference *reduction*, satisfying FR-010/SC-006 (no heavy dependency
added, `Scene` stays FSharp.Core-only).

**Alternatives considered.** *Bump versions during the move* — rejected: version bumps happen at merge
(merge skill), not mid-stage.

## D8 — `PerPackageSurfaceDiff` invocation, not Route-gating

**Decision.** Invoke `PerPackageSurfaceDiff` **explicitly** in the gate sequence; do **not** add a
`Routing.fs` rule for it. `validation.contract.yml` stays unchanged.

**Rationale.** The Stage-0 implementation finding (the known-gate allowlist lives in runtime
`AgentValidation`, relocated in Stage 2) makes Route-gating a runtime change deferred to Stage 2/5.
This stage uses the target as a runnable check, exactly as Stage 0 left it.

**Alternatives considered.** *Wire the rule now* — rejected: it would require editing the runtime
`knownGates` allowlist, which is Stage 2 work and out of scope here.
