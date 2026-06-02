# V3 Implementation Plan: Retire the `FS.Skia.UI` Monolith, Finish Modular Distribution

- **Date:** 2026-06-02 CEST
- **Author:** Claude Code (planning, requested by maintainer)
- **Status:** In progress. **Stage 0 ✔ implemented and merged-ready (feature `048-v3-retirement-baseline`).** Stages 1–5 not yet started. Each stage becomes one Spec Kit feature via `/speckit-specify`.

## Programme progress

| Stage | State | Feature | Notes |
|---|---|---|---|
| 0 — Baseline, ADRs, per-package surface baselines, parity oracle | **✔ Done** | `048-v3-retirement-baseline` | All success criteria met; `EvidenceAudit` PASS (24 real tasks, 0 blockers, zero synthetic). See per-stage status below. |
| 1 — KEYSTONE host extraction + type unification | ☐ Not started | — | Gated on Stage 0 (now satisfied). |
| 2 — Relocate `AgentValidation` | ☐ Not started | — | Premise corrected below (already decoupled from the build front-end). |
| 3 — Repoint/retire legacy samples | ☐ Not started | — | |
| 4 — Repoint legacy tests; retire parity bridge | ☐ Not started | — | |
| 5 — Delete `src/Lib`, decommission `FS.Skia.UI`, enforce | ☐ Not started | — | Picks up the deferred per-package Route rule + enforcement (see Stage 0 finding). |
- **Companion design:** [`v3Design.md`](./v3Design.md) — the V3 modular-distribution design (the "analysis" for this plan; this plan is its executable, staged form).
- **Baseline pin:** SHA `031e56072779c736adf6dd8b0345e17b58a62e73` (branch `main`).

> **This is the first foundations-era programme that deliberately edits the runtime `src/**`.**
> Every prior programme (the foundations rewrite, features 039–047) held the runtime invariant —
> *no edits under `src/Scene`, `src/SkiaViewer`, …*. V3 cannot: its whole point is to move runtime
> code between packages and delete the legacy core. The regression discipline therefore shifts from
> "don't touch the runtime" to "**prove byte/visual output parity on every move**" using the parity
> net that already exists (`tests/Parity.Tests`, the screenshot galleries). That shift is the single
> most important thing this plan adds over the foundations plan.

---

## How to read this plan

The companion design (`v3Design.md`) concluded V3 turns FS.Skia.UI from a *repo you copy* into a
*modular distribution you reference*. A large amount of that design **already shipped** incrementally:
the per-capability packages exist with real implementations, the template references only the split
packages, and new samples/tests are on the split packages. What remains is **one coherent goal —
retire the legacy `FS.Skia.UI` monolith (`src/Lib`)** — which the maintainer has chosen to take on in
full.

This plan stages that retirement into ordered, independently-shippable features, each:

- delivering standalone value and revertible without unwinding later stages,
- gated by **output-parity** exit criteria (not "runtime untouched"),
- preserving the **generated-consumer contract** (the template stays green throughout),
- run under the two-tier `Route` process from feature 042 — but because every stage touches
  `src/**/*.fsi` and/or `template/**`, **every stage escalates** to the full serialized gate set and
  is a **dogfood** feature.

### The architecture finding that shapes the stages

`src/Lib` (4,858 LOC, published as the broad `FS.Skia.UI` package) survives because the **Vulkan/Skia
host was never moved into the `SkiaViewer` package**. Concretely:

- `Lib/Library.fs` still defines a **complete parallel copy of the scene vocabulary** under namespace
  `FS.Skia.UI` (`VertexMode`, `Vertex`, `TextRun`, `FontSpec`, `PerspectiveTransform`, plus the
  `Colors`/`Paint`/`Path`/`Scene` modules) — duplicating the `FS.Skia.UI.Scene` package, which
  defines the same types under `FS.Skia.UI.Scene`.
- `Lib/Library.fs` owns the **host**: the `Viewer` module (`create`/`run`/`withEventMapping`/
  `withEffectMapping`/`withSubscription`/`defaultConfiguration`), `Diagnostics` (`RenderDiagnostic`),
  and the internal `VulkanStartup`/`VulkanResources`.
- The `SkiaViewer` package is a **3,000-LOC wrapper** that project-references `Lib`, calls `Viewer.*`
  for the real host, and carries `SceneConversion.fs` (207 LOC) — a **bridge that converts between the
  `FS.Skia.UI.Scene` package types and `Lib`'s `FS.Skia.UI` types**, existing only because the host
  speaks the old vocabulary while the public API speaks the new one.

**Consequence (the modularity leak):** because `FS.Skia.UI.SkiaViewer` package-depends on the
`FS.Skia.UI` monolith, any product using the viewer — i.e. the **default `app` template profile** —
transitively pulls the entire old core back in. V3's dependency-light promise currently holds for
`Scene`/`Layout`/`Charts`/`KeyboardInput` but **not for the most common path**.

So the keystone is **host extraction + type unification as one move**: retype `Viewer`/Vulkan onto the
`FS.Skia.UI.Scene` vocabulary inside the `SkiaViewer` package, which deletes `SceneConversion.fs`, the
`Lib` reference, and the duplicate types together. Everything else (governance relocation, sample/test
repointing, deletion) is mechanical once the host moves.

### What already exists that we build on (do not reinvent)

| Asset | State today | This plan's use |
|---|---|---|
| Split packages `Scene`/`SkiaViewer`/`Elmish`/`KeyboardInput`/`Layout`/`Controls`/`Controls.Elmish`/`Testing` | Real implementations, acyclic refs onto `Scene`; `Scene` is FSharp.Core-only | The targets the monolith's code moves *into* |
| `template/base` | References **only** split packages; pure V3 already | The consumer contract that must stay green every stage — proves the leak is closed |
| `tests/Parity.Tests` | References `Lib`; the old-vs-new parity bridge | The **parity oracle** for the host move; retired once `Lib` is gone |
| Screenshot/visual galleries (`ScreenshotGallery`, `EffectsGallery`, `BasicViewer`, `InteractiveViewer`) | On the monolith | Visual-parity oracle for the host move; then repointed/retired |
| `FS.Skia.UI.Build` governance library + `Route`/`Routing.fs` two-tier process | From foundations 039–047 | Home for the relocated `AgentValidation`; the process every stage runs under |
| `Charts` capability | Lives in `src/Controls` (Charts.fs / DataGrid.fs), not a separate `Charts` package | Noted; the design's separate `Charts` package is **out of scope** here (see Non-Goals) |

---

## Invariants every stage must preserve

Unlike the foundations plan, **invariant 2 is replaced**. The runtime *is* edited; what must not
regress is *observable output* and the *consumer contract*.

1. **Generated-consumer contract green throughout.** `TemplateCheck` + `GeneratedProductCheck` +
   `GeneratedGuidanceCheck` pass at the end of every stage. The template already references split
   packages; no stage may make a generated `app` fail to restore/build/run.
2. **Output parity on every code move (replaces "runtime untouched").** Any code relocated between
   packages must produce **byte-identical scene output and visually-identical rendered frames** vs the
   baseline, proven by `Parity.Tests` and the screenshot galleries **before** the source copy is
   deleted. A move that cannot be parity-proven does not ship.
3. **Per-package public surface controlled.** Each package's public `.fsi` is the authoritative
   contract. Surface changes are recorded in a **per-package surface baseline** (a new artifact this
   plan introduces — the design's acceptance criterion). Net public API of a package may change only
   where a stage explicitly records it.
4. **Acyclic package graph maintained.** `Scene` depends on FSharp.Core only; no stage may introduce a
   back-edge (e.g. `Scene → SkiaViewer`) or a new heavy dependency into a base package.
5. **net10 conventions honoured.** New/moved projects inherit `Directory.Build.props`
   (`net10.0`, `TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management); no new
   `PackageVersion` outside `Directory.Packages.props`.
6. **FAKE sequencing respected.** FAKE-backed validation runs in the deterministic serialized order;
   never concurrently. Every stage escalates via `Route` and is dogfood (full pipeline).
7. **No FCS / runtime-script-loading reintroduced.** (Carried from foundations; the host move must not
   smuggle in dynamic compilation.)

### The standard per-stage validation command sequence

Every stage escalates (touches `src/**/*.fsi` and/or `template/**`), so the exit-gate sequence is the
canonical serialized order, plus the **parity gate**:

```
./fake.sh build -t Route            # confirms escalation + dogfood
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
# plus, until Stage 4: the Parity.Tests + screenshot-gallery parity oracle
```

---

## Stage dependency overview

```
Stage 0  Baseline, ADRs, per-package surface baselines, parity oracle   [✔ DONE — feature 048]
   │
   └──> Stage 1  KEYSTONE — host extraction + type unification          [the hard, runtime move]
                 (Viewer + Vulkan + Diagnostics move Lib → SkiaViewer,
                  retyped onto FS.Skia.UI.Scene; SceneConversion.fs +
                  SkiaViewer→Lib reference + duplicate Lib scene types deleted)
                    │
                    ├──> Stage 2  Relocate AgentValidation  (Lib → FS.Skia.UI.Build governance lib)
                    │
                    ├──> Stage 3  Repoint / retire the 6 legacy samples (monolith → split or sample-pack)
                    │
                    └──> Stage 4  Repoint legacy tests; retire the Parity.Tests bridge
                                     │
                                     └──> Stage 5  Delete src/Lib; stop publishing FS.Skia.UI;
                                                   per-package surface baselines enforced; generated-
                                                   project cleanliness gate; V2→V3 migration docs; measure
```

**Sequencing:** Stage 0 first (de-risk + oracle). Stage 1 is the keystone and gates everything after
it. Stages 2–4 are independent of each other and may ship in any order / in parallel once Stage 1
lands. Stage 5 is the closeout and requires 1–4 complete (nothing may reference `Lib`).

---

## Stage 0 — Baseline, decisions, per-package surface baselines, parity oracle

> **✔ Implemented in feature `048-v3-retirement-baseline`** (branch `048-v3-retirement-baseline`,
> commits `8da3d55` + `ba36d55`). Record-and-oracle only; `src/**` byte-unchanged,
> `validation.contract.yml` + `Directory.Packages.props` unchanged (SC-007/FR-010/FR-011). Escalated
> serialized gates green (`Dev`, `PerPackageSurfaceDiff` zero-drift, `GeneratedGuidanceCheck`,
> `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`); `EvidenceAudit` **PASS** (24 real tasks,
> 0 blockers, zero synthetic). The first aggregate `GeneratedProductCheck` hit the known
> `SkiaViewer.Tests` headless libdecor-gtk test-host crash; the focused rerun (48/48) and the target
> retry are authoritative. **Per-work-item status is annotated inline below.**

**Goal:** Make the retirement measurable and safe before moving any runtime code. Capture the before-
state, lock the decisions, and — critically — **stand up the parity oracle and the per-package surface
baselines that don't yet exist**, because without them a runtime move is unverifiable.

**Why first:** Every later stage claims "output unchanged" and "surface controlled." Both are
unprovable without a captured oracle and per-package baselines.

**Dependencies:** none.

### Work items

0.1 **Capture quantitative baseline** → `docs/reports/_baselines/2026-06-02-v3-before.md`: **✔ done.**
   - `src/Lib` LOC by file (measured: Library.fs 2,408; KeyboardInput.fs 1,398; AgentValidation.fs 835;
     VulkanStartup.fs 119; VulkanResources.fs 92; + `.fsi` — 6,214 total incl. `.fsi`).
   - The **transitive-dependency proof of the leak**: `dotnet list … reference` dumps showing
     `FS.Skia.UI.SkiaViewer → Lib` and `Elmish → SkiaViewer → Lib`, with the `SceneConversion.fs`
     bridge — each metric named with its reproduction command.
   - The duplicate-type inventory: **34** types defined in *both* `Scene.fsi` and `Lib/Library.fsi`.
   - Inventory of `Lib` consumers (6 samples, 5 test projects, `SkiaViewer`). **Finding:**
     `build/Governance/Front/Support.fs` **no longer consumes** the monolith's `AgentValidation`
     (the governance library already owns `CapabilityRow`/`ValidationFinding`); the surface's only
     remaining consumer is `tests/Governance.Tests/AgentValidationFrameworkTests.fs` — this corrects
     Stage 2's premise (see Stage 2 below).

0.2 **Stand up the parity oracle.** **✔ done.** Implemented a deterministic, environment-independent
   scene-output encoder over the **current host's** `Scene` values for the closed seed set
   `basic-viewer`/`effects-gallery`/`screenshot-gallery`, committed under
   `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt`. **The monolith host's `Scene`
   is opaque**, so the encoding fingerprints it via the host's deterministic introspection
   (`Scene.describe` element kinds + `Scene.diagnostics` + `Scene.renderReadbackEvidence`'s
   environment-independent `DeterministicHash`) under a versioned `format: scene-output/v1` — it
   re-derives **byte-identically** (SC-003) and is the **authoritative** Stage-1 oracle. Reference
   screenshots are corroboration only: a **real Vulkan-captured** `basic-viewer.png` (640×480) was
   committed; `effects-gallery`/`screenshot-gallery` reference frames are **deferred at the pin** (no
   non-interactive screenshot entry point in those galleries — capturing them needs an interactive
   keypress or a sample-code change, out of scope). `capture-environment.md` records OS/GPU/Vulkan/
   toolchain/commands.

0.3 **Introduce per-package surface baselines.** **✔ done — as a distinct, additive capability.** The
   plan's loose name "a `PackageSurfaceCheck` extension" is **superseded**: a `PackageSurfaceCheck`
   target already exists and is coarse (reflection over compiled assemblies → exported-type-**name**
   sets, monolith-inclusive) and cannot see a changed **signature**. So Stage 0 added a
   **distinctly-named, additive `PerPackageSurfaceDiff`** target over a new artifact tree
   `readiness/per-package-surface/<PackageId>.fsi.txt` (normalized full `.fsi` text, signature-
   sensitive, DiffPlex line diff; `build/Governance/PerPackageSurface.fs(i)`), leaving the existing
   aggregate `PackageSurfaceCheck` green and unchanged. The **eight** baselines were captured at the
   pin and diff at **zero drift** (SC-004); a real reverted one-package edit drifts exactly that
   package (SC-005).

   **Finding — Route-gating deferred (runtime-coupling):** a `Routing.fs` rule for the new target
   would render `PerPackageSurfaceDiff` into `validation.contract.yml`'s `routing_rules.required_gates`,
   and the contract validator's **known-gate allowlist lives in the runtime monolith**
   (`src/Lib/AgentValidation.fs` `knownGates`). Teaching it the gate is a runtime change, which Stage 0
   forbids (record-and-oracle only). So **no Routing rule was added** and `validation.contract.yml` is
   unchanged; the target ships additive + runnable directly (`./fake.sh build -t PerPackageSurfaceDiff`).
   **Route-gating + enforcement move to Stage 5**, and become clean once **Stage 2** relocates
   `AgentValidation` into `FS.Skia.UI.Build` (then `knownGates` is governance config, not runtime).

0.4 **Record ADRs** under `docs/adr/` (continuing the `000N` series; foundations ended at 0006).
   **Written and accepted (feature 048):**
   [ADR 0007](../adr/0007-host-ownership.md),
   [ADR 0008](../adr/0008-scene-vocabulary-single-source.md),
   [ADR 0009](../adr/0009-agentvalidation-placement.md),
   [ADR 0010](../adr/0010-legacy-sample-policy.md),
   [ADR 0011](../adr/0011-parity-oracle-method.md).
   - **ADR 0007 — Host ownership:** the Vulkan/Skia host (`Viewer`, `VulkanStartup`,
     `VulkanResources`, `RenderDiagnostic`) is owned by `FS.Skia.UI.SkiaViewer`; `Lib` is retired.
   - **ADR 0008 — Scene-vocabulary single source:** `FS.Skia.UI.Scene` types are canonical; the `Lib`
     `FS.Skia.UI` duplicates are deleted, not aliased. The host is retyped onto the Scene package;
     `SceneConversion.fs` is removed (no permanent compatibility shim — it kept an unwanted dependency
     alive).
   - **ADR 0009 — `AgentValidation` placement:** the governance contract parser moves into the
     governance library `FS.Skia.UI.Build`, removing the build→runtime-package coupling. (It was never
     runtime.)
   - **ADR 0010 — Legacy-sample policy:** the 6 monolith samples are repointed onto split packages
     where they demonstrate a supported capability, or moved to an opt-in `sample-pack` and excluded
     from the default template per the design; `ParityGallery` retires with the parity bridge.
   - **ADR 0011 — Parity-oracle method:** byte-identical scene output (`Parity.Tests`) + visual
     screenshot parity is the merge gate for the host move; the `--legacy`-style dual-build is retained
     only until parity is signed off, then deleted.

### Exit criteria — ✔ all met (feature 048)

- ✔ Baseline file + leak proof + duplicate-type inventory committed (SC-001/002).
- ✔ Parity golden fixtures captured from the **current** host and re-derive byte-identically (SC-003);
  reference screenshot captured (BasicViewer real frame; other two deferred — corroboration only).
- ✔ Eight per-package surface baselines committed; the **additive `PerPackageSurfaceDiff`** target is
  green at zero drift (SC-004). The existing aggregate `PackageSurfaceCheck` stays green and unchanged.
- ✔ ADRs 0007–0011 written, accepted, and linked from this plan (SC-006).
- ✔ No runtime code changed (`git diff --stat src/` empty, SC-007); full serialized gate sequence
  green; `EvidenceAudit` PASS, zero synthetic (SC-008).
- ⚠ **Deviation (recorded):** the per-package Route rule + enforcement were deferred to Stage 5 (the
  runtime-coupling finding in §0.3); the capability ships additive and runnable, not yet Route-gated.

### Risks & mitigations

- *Risk:* screenshots are environment-sensitive (headless flake — see the known `SkiaViewer.Tests`
  libdecor-gtk crash). *Mitigation:* capture on a pinned toolchain; prefer the deterministic
  scene-output `Parity.Tests` as the primary oracle, screenshots as corroboration; record the
  capture environment.

**Effort:** ~2–3 days. **Revert:** delete docs/fixtures; nothing else touched.

---

## Stage 1 — KEYSTONE: host extraction + scene-vocabulary unification

**Goal:** Move the Vulkan/Skia host out of `Lib` into the `SkiaViewer` package, **retyped onto the
`FS.Skia.UI.Scene` vocabulary**, deleting the duplicate scene types, the `SceneConversion.fs` bridge,
and the `SkiaViewer → Lib` reference in one coherent move. This closes the modularity leak.

**Why now / why the keystone:** It is the only hard, high-risk step and the precondition for retiring
`Lib`. Until the host moves, `Lib` cannot die and the default `app` profile keeps pulling the monolith.

**Dependencies:** Stage 0 (oracle + per-package baselines). **Designated dogfood.**

### Work items

1.1 **Move the host modules** `Viewer`, `Diagnostics` (`RenderDiagnostic`), and internal
   `VulkanStartup` / `VulkanResources` from `Lib/Library.fs` into the `SkiaViewer` package (new
   `Host/` modules), preserving public function shapes (`create`/`run`/`withEventMapping`/
   `withEffectMapping`/`withSubscription`/`defaultConfiguration`).

1.2 **Retype the host onto `FS.Skia.UI.Scene`.** Replace every internal use of `Lib`'s `FS.Skia.UI`
   scene types (`Vertex`/`VertexMode`/`TextRun`/`FontSpec`/`PerspectiveTransform`/`Scene`/`Paint`/…)
   with the `FS.Skia.UI.Scene` package equivalents.

1.3 **Delete `SceneConversion.fs`** (the 207-LOC bridge) — with one vocabulary, no conversion remains.

1.4 **Sever `SkiaViewer → Lib`.** Remove the `ProjectReference`; `SkiaViewer` now depends only on
   `Scene` + `KeyboardInput` + its native packages (Silk.NET/SkiaSharp). `FS.Skia.UI.SkiaViewer` no
   longer package-depends on `FS.Skia.UI` — **leak closed**.

1.5 **Delete the duplicate scene vocabulary from `Lib`.** Remove `Lib`'s `Colors`/`Paint`/`Path`/
   `Scene`/`Diagnostics`/`Viewer` modules (now homed in `Scene`/`SkiaViewer`). What remains in `Lib`
   after this stage: `AgentValidation` (Stage 2), the duplicate `KeyboardInput.fs` (dead once nothing
   references `Lib`), and the `Parity` helper (retires in Stage 4).

1.6 **Prove parity (the gate).** `Parity.Tests` scene-output is byte-identical to the Stage-0 golden;
   screenshots from the galleries are visually identical. Keep `Lib`'s old host runnable behind a
   temporary build flag until parity is signed off (ADR 0011), then it is removed in 1.5.

### New / changed artifacts

- `src/SkiaViewer/Host/*.fs(i)` (Viewer + Vulkan + Diagnostics), retyped onto `Scene`.
- `src/SkiaViewer/SceneConversion.fs` **deleted**; `SkiaViewer.fsproj` `Lib` reference **removed**.
- `src/Lib/Library.fs(i)` reduced to non-scene/non-host residue.
- `SkiaViewer` per-package surface baseline updated (recorded delta — public surface should be
  *stable*; the host API was already re-exposed by the wrapper).

### Exit criteria

- `SkiaViewer.fsproj` has **no** `ProjectReference` to `Lib`; packed `FS.Skia.UI.SkiaViewer` has **no**
  package dependency on `FS.Skia.UI` (leak-proof dump vs Stage 0).
- `Parity.Tests` scene output byte-identical to Stage-0 golden; gallery screenshots visually identical.
- Default `app` template still restores/builds/runs (`TemplateCheck` green) and **no longer** pulls the
  monolith transitively.
- `Scene` remains FSharp.Core-only; package graph still acyclic.
- Invariants 1–7 hold; full serialized gate sequence green.

### Risks & mitigations

- *Risk:* subtle render divergence when retyping (coordinate/paint edge cases). *Mitigation:* the
  byte-identical `Parity.Tests` gate blocks merge until zero-diff; dual-build flag keeps the old host
  for side-by-side until sign-off.
- *Risk:* the `SkiaViewer.Tests` headless flake masks a real regression. *Mitigation:* focused rerun is
  authoritative (per the known flake); parity uses deterministic scene-output as primary oracle.
- *Risk:* native-startup lifetime/cleanup behaviour shifts when modules move. *Mitigation:* the
  native startup-cleanup tests travel with the host into `SkiaViewer` and run in the gate.

**Effort:** ~7–12 days (the single hardest stage). **Revert:** restore the `Lib` reference + the dual-
build flag; the old host is retained until 1.5, so revert is a flag flip until sign-off.

---

## Stage 2 — Relocate `AgentValidation` out of the runtime monolith

**Goal:** Move `FS.Skia.UI.AgentValidation` (the governance contract parser — `ValidationContract`,
`ValidationSelection`, `ValidationSelectionInterpreter`, `AgentVerdict`) from `src/Lib` into the
governance library `FS.Skia.UI.Build`, removing the build→runtime-package coupling.

**Why now:** It is governance, not runtime. **Stage-0 finding (revises this premise):**
`build/Governance/Front/Support.fs` **already does not consume** the monolith's `AgentValidation` (the
governance library owns `CapabilityRow`/`ValidationFinding`); at the pin the surface's only remaining
consumer is `tests/Governance.Tests/AgentValidationFrameworkTests.fs` (not any runtime package — the
`Testing` package's similarly-named `GeneratedValidationContract*` types are its own). Moving it lets
`Lib` shed 835 LOC. **It also unblocks the deferred per-package Route rule:** once `knownGates` lives in
`FS.Skia.UI.Build` rather than `src/Lib`, adding `PerPackageSurfaceDiff` to `validation.contract.yml`
no longer touches runtime code (see Stage 0 §0.3 finding).

**Dependencies:** Stage 1 (so `Lib` is already shrinking). Independent of Stages 3–4.

### Work items

2.1 Move `AgentValidation.fs(i)` into `build/Governance/**` as a curated-`.fsi` module of
   `FS.Skia.UI.Build` (Principle II).
2.2 Repoint `build/Governance/Front/Support.fs` and the `Governance.Tests` suites
   (`AgentValidationFrameworkTests.fs`, `AsteroidsFeedbackSkillGuidanceTests.fs`) at the new location.
2.3 Confirm no runtime package referenced it (it did not); record the verification.

### Exit criteria

- `FS.Skia.UI.AgentValidation` no longer exists under `src/Lib`; the build front-end consumes it from
  the governance library; `Governance.Tests` green.
- Generated consumers unaffected (`GeneratedProductCheck` green) — `AgentValidation` was never shipped
  to products.
- Invariants 1–7 hold.

**Effort:** ~2–3 days. **Revert:** move the module back; reference is symmetric.

---

## Stage 3 — Repoint or retire the legacy samples

**Goal:** Remove the last *sample-side* consumers of the monolith. The 6 legacy samples
(`BasicViewer`, `EffectsGallery`, `ParityGallery`, `ScreenshotGallery`, `InteractiveViewer` —
`PackageReference FS.Skia.UI`; `DemoReel` — `ProjectReference` to both) move to split packages or to an
opt-in sample-pack.

**Why now:** Independent cleanup; unblocks deletion. Per the design, samples default to excluded from
generated products and live in the framework repo (or a `fs-skia-ui-samples` pack).

**Dependencies:** Stage 1 (host available in `SkiaViewer`). Independent of Stages 2, 4.

### Work items

3.1 Repoint each viewer sample onto `FS.Skia.UI.Scene` + `FS.Skia.UI.SkiaViewer` (+ `Elmish` where
   used), dropping the `FS.Skia.UI` reference.
3.2 `ParityGallery` is retired together with the parity bridge (its job ends when `Lib` is gone) — or
   repointed if it still demonstrates a supported capability.
3.3 Confirm each repointed sample restores/builds/runs and (for visual samples) still produces the
   Stage-0 reference screenshots.

### Exit criteria

- No sample references `FS.Skia.UI` (the monolith) by package or project.
- Visual samples still match Stage-0 reference screenshots (parity carried from Stage 1).
- Invariants 1–7 hold.

**Effort:** ~2–4 days. **Revert:** restore the sample references.

---

## Stage 4 — Repoint legacy tests; retire the parity bridge

**Goal:** Remove the last *test-side* consumers of the monolith and retire `Parity.Tests` once its
old-vs-new parity job is complete.

**Dependencies:** Stages 1–3 (everything else off the monolith). Stage 1's parity must be signed off
before `Parity.Tests` retires.

### Work items

4.1 Repoint `Lib.Tests`, `Smoke.Tests`, `Package.Tests`, `Governance.Tests` onto the split packages
   (their `Lib` references were to the monolith's runtime; the equivalent surface now lives in
   `Scene`/`SkiaViewer`).
4.2 **Retire `Parity.Tests`** — it exists to prove old-host == new-host; once `Lib`'s host is gone and
   Stage 1 parity is signed off, there is no "old" to compare. Fold any still-valuable assertions into
   `SkiaViewer.Tests`/`Scene.Tests` first.
4.3 Confirm the full test suite is green with no project referencing `Lib`.

### Exit criteria

- No test project references `Lib`; `Parity.Tests` removed (assertions migrated where valuable).
- Full serialized gate sequence green; `EvidenceGraph`/`EvidenceAudit` PASS.
- Invariants 1–7 hold.

**Effort:** ~3–5 days. **Revert:** restore the test references; un-retire `Parity.Tests`.

---

## Stage 5 — Delete `src/Lib`, decommission `FS.Skia.UI`, enforce cleanliness, measure

**Goal:** Remove the monolith entirely, stop publishing the `FS.Skia.UI` package, lock in the
per-package surface baselines, add the generated-project cleanliness gate, write the V2→V3 migration
docs, and produce the after-measurement.

**Dependencies:** Stages 1–4 (nothing references `Lib`).

### Work items

5.1 **Delete `src/Lib`** (`git rm`): `Library.fs(i)`, the duplicate `KeyboardInput.fs(i)`,
   `VulkanStartup`/`VulkanResources` residue, `InternalsVisibleTo.fs`. Remove `Lib` from the solution.
5.2 **Stop publishing `FS.Skia.UI`.** Remove `Lib`'s `IsPackable`/`PackageId`; drop it from `PackLocal`
   / the pack-version flow / `docs/reports/dependencies.md`. Verify no `Directory.Packages.props` or
   template pin still names `FS.Skia.UI`.
5.3 **Enforce per-package surface baselines** (from Stage 0) as a merge gate — the additive
   `PerPackageSurfaceDiff` target fails on an unrecorded per-package `.fsi` change. (Design acceptance
   criterion.) **Picks up the Stage-0 deferral:** add the `Routing.fs` rule that Route-selects
   `PerPackageSurfaceDiff` and render it into `validation.contract.yml` — clean here because Stage 2
   has relocated `AgentValidation`'s `knownGates` into the governance library, so the gate name is
   governance config rather than a runtime-monolith edit.
5.4 **Add the generated-project cleanliness gate** (`GeneratedProjectCheck` / extend
   `GeneratedProductCheck`): assert a generated default `app` contains no `samples/`, no framework docs
   set, no historical `specs/`, no framework README copy, and references packages rather than copying
   framework projects. (Design acceptance criteria.)
5.5 **V2→V3 migration docs:** a table mapping the old `FS.Skia.UI` surface to the split packages
   (`FS.Skia.UI.Scene` / `.SkiaViewer` / `.Elmish` / `.KeyboardInput` / `.Layout` / `.Controls`), how
   to move an app's package references, and the removed-`SceneConversion` note.
5.6 **After-measurement** → `docs/reports/_baselines/2026-06-02-v3-after.md`: `Lib` LOC (→ 0),
   monolith-transitive-pull (→ none), duplicate-type count (→ 0), package count, per-package surface
   baselines present, generated-`app` cleanliness asserted. Closing **ADR 0012** (programme closeout).

### Exit criteria

- `src/Lib` gone (grep proves no `Lib`/`FS.Skia.UI`-monolith reference anywhere outside history).
- `FS.Skia.UI` no longer packed/published; nothing references it.
- Per-package surface baselines enforced; generated-project cleanliness gate green.
- Migration docs published; after-baseline shows the targeted reductions.
- Full serialized gate sequence green; generated consumers fully governed.

**Effort:** ~3–4 days. **Revert:** the deletion is git-revertible until the package is unpublished;
keep `Lib` recoverable behind the solution until 5.6 signs off.

---

## Whole-programme definition of done

| Dimension | Baseline (2026-06-02) | Target |
|---|---|---|
| `src/Lib` (`FS.Skia.UI` monolith) | 4,858 LOC, published | deleted; package unpublished |
| Host ownership | `Viewer`/Vulkan in `Lib`; `SkiaViewer` wraps it | host in `FS.Skia.UI.SkiaViewer`; no `Lib` |
| Scene vocabulary | duplicated in `Scene` pkg **and** `Lib` | single source: `FS.Skia.UI.Scene` |
| `SceneConversion.fs` bridge | 207 LOC | deleted |
| Default `app` profile transitive deps | pulls the whole monolith | split packages only (leak closed) |
| `AgentValidation` (governance) | in runtime `src/Lib` | in `FS.Skia.UI.Build` governance library |
| Legacy samples on monolith | 6 | 0 (repointed or sample-pack) |
| Test projects on monolith | 5 (+`Parity.Tests` bridge) | 0; `Parity.Tests` retired |
| Per-package surface baselines | aggregate only | per public package, enforced |
| Generated-project cleanliness gate | absent | present + green |
| Runtime architecture (Scene→SkiaViewer→Elmish) | sound | **unchanged in behaviour** (parity-proven) |

---

## Decisions (proposed — confirm during Stage 0 ADRs)

- **V1 — Full retirement, in full.** The maintainer chose to retire `src/Lib` entirely (not just
  extract the host), staged across features 1–5 above.
- **V2 — Host owned by `SkiaViewer`, retyped onto `Scene`** (ADR 0007/0008); no permanent
  `SceneConversion` shim.
- **V3 — `AgentValidation` → governance library** (ADR 0009).
- **V4 — Parity oracle = byte-identical `Parity.Tests` + visual screenshots** (ADR 0011); deterministic
  scene-output is primary, screenshots corroborate (headless-flake aware).
- **V5 — Sequencing:** Stage 0 → Stage 1 keystone → Stages 2/3/4 in parallel → Stage 5 closeout.

## Non-Goals (carried from `v3Design.md`)

- **No new rendering architecture.** Scene→SkiaViewer→Elmish stays; only package boundaries move.
- **Charts/DataGrid package split is out of scope.** They live in `src/Controls` today and work; a
  separate `FS.Skia.UI.Charts` package is a *future* design item, not part of monolith retirement.
- **Template profile expansion** (`headless-scene`, `full-governed`, `sample-pack` as first-class
  switches) is out of scope except where Stage 3 needs an opt-in sample-pack home.
- **No plugin runtime / dynamic loader.** No FCS, no runtime script loading (invariant 7).

---

## Suggested entry point

**Stage 0 is done** (feature 048): the **parity golden fixtures** (captured from the *current* monolith
host, byte-identical re-derivation) and the **eight per-package surface baselines** now exist, so a
Stage-1 runtime move is verifiable. **Stage 1 is the whole game** — the host extraction + type
unification — and is the right next `/speckit-specify` feature. Stages 2–4 are mechanical cleanups that
parallelize once Stage 1 lands (Stage 2 also unblocks the deferred per-package Route rule); Stage 5 is
the closeout (and picks up Route-gating + enforcing the per-package baselines).

Each stage below becomes one Spec Kit feature: run `/speckit-specify` with the stage's goal + work
items, then `/speckit-plan`, `/speckit-tasks`, `/speckit-implement`.
