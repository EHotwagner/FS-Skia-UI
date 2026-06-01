# Feature Specification: Dedicated Compiled Build Front-End + MEL Engine Extraction

**Feature Branch**: `045-foundations-build-frontend`  
**Created**: 2026-06-01  
**Status**: Draft  
**Input**: User description: "implement the next part of the plan — Stage 5 of `docs/reports/2026-05-31-1049-foundations-implementation-plan.md` (dedicated build front-end + MEL engine extraction + typed targets), continuing the foundations programme after Stages 0/1/2/3/4 (features 039–044)."

## Context & Motivation *(informative)*

This is the **keystone-completing** stage of the foundations programme. The companion analysis
found that most of `build.fsx` is "not a build — it is a governance library wearing a build's
clothes," and the plan's whole-programme definition of done targets `build.fsx` at **deleted or a
≤200-line shim**. Stages 3 and 4 (features 041, 043) already emptied the validator and
evidence-engine logic out of the script into the compiled, unit-tested `FS.Skia.UI.Build` library.
What remains inside the **4,767-line** `build.fsx` is:

1. The **orchestration core** — the Model-Effect-interpreter (MEL; the build-side MVU) engine
   (`BuildMsg`/`BuildEffect`/`BuildModel` + `update` + `interpret`) — a sound design that today
   lives in a script where it cannot be unit-tested, cannot be referenced from anywhere else, and
   is recompiled as one 207 KB unit on every edit.
2. Three **remaining heavy validators** still inline in the script: generated-product validation
   (~800 lines), generated-guidance / skill-section scanners (~200 lines), and
   process-health / bootstrap preflight (~267 lines).
3. The thin target list, dependency wiring (`==>`), and `runTarget` dispatcher.

The plan's de-risking spike (feature 039) already **confirmed decision D2**: a dedicated, normally
compiled FAKE build project (`build/Build.fsproj`, an exe run with `dotnet run`) can register and
drive targets via `Fake.Core.Target` with **no FSX runner and no `FSharp.Compiler.*` dependency**.
That scaffold exists today (a single `SpikeHello` target in `build/Program.fs`). This feature grows
it into the real front-end, moves the last logic out of `build.fsx` into the library, and **deletes
`build.fsx`**.

The decisive property is **behaviour parity**: every one of the ~37 targets must produce
byte-identical reports and artifacts after the move. The relocation is a refactor of the *tooling
around* the framework — the runtime architecture (`Scene → SkiaViewer → Elmish`) and the product's
public `.fsi` surface are explicitly untouched.

The validation contract migration the plan filed under Stage 5.5 (`Routing.fs` as compiled-F#
source of truth, `validation.contract.yml` generated from it, the `Route` target running
in-process) was **already pulled forward into feature 042 (Stage 1)**. So this feature's only
remaining obligation there is to *consume* `Routing.fs` from the relocated front-end — no new
routing work.

## Clarifications

### Session 2026-06-01

- Q: SC-002/FR-012 require byte-identical output across all ~37 targets, but some are environment-dependent (`FsiTranscripts`/`TemplateCheck` documented pre-existing RED on this toolchain; test-running/log targets carry timestamps and ordering). What should the golden-parity gate compare? → A: Reports, normalized — diff each target's deterministic governance reports/artifacts with known nondeterminism (timestamps, absolute paths) normalized; compare test-shelling targets by verdict + report rather than raw stdout; exclude the documented pre-existing-RED gates using the same stash-control disclosure feature 039 used.
- Q: Is the warm-build-not-slower criterion (SC-007/FR-014) a hard merge-blocking gate or a recorded-and-explained measurement? → A: Recorded, not blocking — cold/warm wall-clock is captured and reported vs baseline; the feature is not blocked if warm builds are not strictly faster, provided behaviour parity (SC-002) holds and any regression is explained. Behaviour parity is the real gate; build-time is an observation, per the plan's "expect warm builds faster" framing.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - The build runs from a compiled front-end with full parity (Priority: P1)

A maintainer (or AI agent) invokes a build target. Today `fake.sh`/`fake.cmd` shell to the
`dotnet fake` CLI, which content-hash-compiles the 4,767-line `build.fsx` and runs it. After this
feature, the same invocation runs a **normally compiled** dedicated project
(`dotnet run --project build/Build.fsproj -- <target>`) that registers every target from the typed
`Targets` graph and delegates each target body to `FS.Skia.UI.Build`. Every target produces output
**byte-identical** to the pre-migration baseline.

**Why this priority**: It is the visible, load-bearing change — the entry point every framework
change already runs through. It delivers incremental compilation and IDE-grade tooling for build
logic, removes the per-edit whole-script recompile tax, and (with US2/US3) lets `build.fsx` be
deleted. Parity is what makes the move safe to ship.

**Independent Test**: Capture a golden baseline of every target's report/artifact from the current
`build.fsx` path; run the same targets through `dotnet run --project build/Build.fsproj`; diff the
outputs — they are byte-identical. `./fake.sh build -t <target>` and `fake.cmd` resolve to the new
front-end and behave identically to before.

**Acceptance Scenarios**:

1. **Given** the dedicated front-end is built, **When** `dotnet run --project build/Build.fsproj -- <target>`
   is run for each registered target, **Then** every target produces output byte-identical to the
   captured `build.fsx` baseline.
2. **Given** `fake.sh`/`fake.cmd` are updated, **When** a contributor runs `./fake.sh build -t Dev`
   (and the other targets), **Then** the launcher routes through the compiled front-end and the
   result is identical to the prior `dotnet fake` path.
3. **Given** the migration is complete, **When** the toolchain is inspected, **Then** `fake-cli` is
   removed from `.config/dotnet-tools.json` and no `dotnet fake` invocation remains in the launchers.
4. **Given** the front-end and library, **When** dependencies are listed, **Then** no
   `FSharp.Compiler.*` / FCS reference is present anywhere (the build is compiled, not script-loaded).

---

### User Story 2 - The 4,767-line script is gone; its logic lives in the tested library (Priority: P2)

A maintainer who needs to change build orchestration or a heavy validator currently edits a 207 KB
script with no incremental compilation, no IDE tooling, and no unit tests on its most consequential
logic. After this feature the MEL engine and the three remaining heavy validators are normal,
compiled, unit-tested library modules, and `build.fsx` is **deleted** (D2 confirmed; the ≤200-line
shim fallback is not needed).

**Why this priority**: It is the structural payoff of the keystone — the "governance library
wearing a build's clothes" finally stops being a script. It depends on US1's front-end existing to
host the relocated targets, so it is sequenced second.

**Independent Test**: After the move, `build.fsx` is absent from the tracked tree (or ≤200 lines if
a blocker forces the documented fallback); the MEL engine (`update`/`interpret`/`BuildModel`) and
the generated-product / guidance / preflight validators are modules of `FS.Skia.UI.Build` with
curated `.fsi` signatures; the full serialized FAKE gate set still passes.

**Acceptance Scenarios**:

1. **Given** the relocation is complete, **When** the tracked tree is inspected, **Then** `build.fsx`
   is deleted (or ≤200 lines under the documented fallback), recorded against the 4,767-line / 4,688
   baseline.
2. **Given** the MEL engine is extracted, **When** the library is built under
   `TreatWarningsAsErrors`, **Then** the engine modules compile clean with curated `.fsi` surfaces
   and the front-end delegates each target body to them (no inlined orchestration logic in the exe).
3. **Given** generated-product / generated-guidance / preflight validation is moved, **When** the
   corresponding targets run, **Then** their reports are byte-identical to baseline.

---

### User Story 3 - The build's most consequential logic gains direct unit tests (Priority: P3)

The rules that decide whether a target runs, what effects it emits, and whether a generated product
is valid are the most consequential logic in the repository, yet today they are untestable inside a
script. After this feature `update` is a **pure** function tested with typed effect-list assertions
(given a `Target`, assert the emitted `BuildEffect` list), and the relocated validators have direct
unit tests asserting typed findings.

**Why this priority**: It is the durable quality win the analysis named, but it follows naturally
once US2 has extracted the logic into the library, so it is sequenced last.

**Independent Test**: `Governance.Tests` contains new suites that call the library's `update` and
the relocated validators directly and assert typed results (effect lists, findings) — not strings —
for representative targets and validation cases.

**Acceptance Scenarios**:

1. **Given** the extracted MEL engine, **When** `update` is invoked for representative targets in a
   unit test, **Then** it returns the expected typed `BuildEffect` list as a pure function (no I/O).
2. **Given** the relocated validators, **When** their unit tests run against fixtures, **Then** they
   assert typed findings and match the golden reports.

---

### Edge Cases

- **`.fake` state / `dotnet tool restore` interaction**: The launchers currently `dotnet tool
  restore` then `dotnet fake`. Switching to `dotnet run` must not leave a stale `fake-cli` restore
  step or rely on `.fake` cache state; the FAKE-sequencing invariant (never concurrent) still holds.
- **Target registration completeness**: Every target present in the typed `Targets` graph must be
  registered in the front-end. A missing registration must be a **compile error or a startup
  failure**, never a silently absent target (the exhaustive `Target` match is the safeguard).
- **Default-target / argument passing**: `dotnet run -- <target>` must forward target names and
  flags (e.g. `Route --enforce`) exactly as `dotnet fake` did, so existing invocations and CI
  scripts are unaffected.
- **Report determinism**: Any target whose output embeds a path, ordering, or timestamp must remain
  deterministic so the golden parity diff is meaningful (sort/normalise as the script did).
- **Spike residue**: The `SpikeHello` target and any leftover spike scaffolding (`build/SkillExamples`
  remnants, `build/spike-verify.sh`) are removed or superseded without affecting real targets.
- **Generated consumers**: Generated projects already consume the packaged `FS.Skia.UI.Build`
  evidence engine (feature 043). Relocating framework-repo build logic must not change the consumer
  package contract or the generated-product structural expectations (those checks must stay
  byte-identical; their *versioning* is Stage 6, out of scope here).

## Requirements *(mandatory)*

### Functional Requirements

**Dedicated compiled front-end (US1)**

- **FR-001**: The build MUST run from a dedicated, normally-compiled project
  (`build/Build.fsproj`, exe, `dotnet run`) that references `FS.Skia.UI.Build` and registers
  **every** target from the typed `Targets` graph, delegating each target body to the library with
  no inlined orchestration or validation logic in the exe.
- **FR-002**: `fake.sh` and `fake.cmd` MUST invoke the dedicated front-end
  (`dotnet run --project build/Build.fsproj -- <args>`) instead of the `dotnet fake` CLI, forwarding
  target names and flags (e.g. `-t Dev`, `Route --enforce`) with identical semantics.
- **FR-003**: `fake-cli` MUST be removed from `.config/dotnet-tools.json`, and no `dotnet fake`
  invocation MUST remain in the launchers or scripts (grep-provable).
- **FR-004**: No `FSharp.Compiler.*` / FSharp Compiler Services dependency MUST be introduced
  anywhere; the build is compiled, never script-loaded at runtime (grep-provable, per ADR D6).
- **FR-005**: The front-end MUST consume the existing `Routing.fs` (feature 042) in-process for the
  `Route` target; this feature introduces **no new routing logic** — Stage 5.5 was already delivered
  in feature 042.

**MEL engine + heavy-validator extraction; retire `build.fsx` (US2)**

- **FR-006**: The MEL engine (`BuildMsg`, `BuildEffect`, `BuildModel`, `update`, `interpret`) MUST
  be relocated into `Engine/` modules of `FS.Skia.UI.Build`, wired to the typed `Targets` union
  (string `StartTarget "…"` dispatch replaced by typed dispatch), each with a curated `.fsi`
  (Principle II).
- **FR-007**: The pure decision logic (`update`) MUST be separated from all filesystem / `git` /
  process / write I/O, which MUST remain at the interpreter edge (`interpret`), so `update` is
  unit-testable without touching the repo tree (Principle IV).
- **FR-008**: Generated-product validation (~800 lines) MUST move into a `GeneratedProduct.fs`
  library module; its structural checks MUST remain **behaviour-identical** to the current
  `build.fsx` logic (schema versioning / deprecation window is Stage 6 and is out of scope here).
- **FR-009**: Generated-guidance / skill-section scanners (~200 lines) MUST move into a `Guidance.fs`
  library module, behaviour-preserving.
- **FR-010**: Process-health / bootstrap preflight (~267 lines) MUST move into a `Preflight.fs`
  library module, behaviour-preserving.
- **FR-011**: `build.fsx` MUST be **deleted** from the tracked tree (D2 confirmed). The ≤200-line
  `#r`-the-DLL shim is retained **only** as a documented fallback if migration surfaces a concrete
  blocker; if used, the residual line count MUST be recorded against the 4,767 / 4,688 baseline.

**Parity, tests, and measurement (US1/US2/US3)**

- **FR-012**: Every registered target MUST produce reports and artifacts **byte-identical** to a
  baseline captured from the pre-migration `build.fsx` path, proven by a golden diff before
  `build.fsx` is removed. The parity oracle compares each target's **deterministic governance
  reports/artifacts** with known nondeterminism (timestamps, absolute paths) **normalized**;
  **test-shelling** targets are compared by **verdict + report**, not raw stdout; and the targets
  documented as pre-existing RED on this toolchain (`FsiTranscripts`, `TemplateCheck`'s
  `SkiaViewer.Tests` headless flake) are **excluded** from the byte-diff via the same stash-control
  disclosure feature 039 used (proven feature-independent: they fail identically with this feature's
  edits stashed). Exclusions MUST be enumerated and justified, never silent.
- **FR-013**: `update` MUST have direct unit tests asserting the typed `BuildEffect` list for
  representative targets; the relocated validators MUST have unit tests asserting typed findings —
  all in `Governance.Tests`, which already project-references the library.
- **FR-014**: Cold-build and warm-build wall-clock MUST be recorded against the baseline (the
  expectation per the plan is warm builds at least as fast, since a compiled library replaces the
  207 KB script recompile). This is a **recorded-and-explained measurement, not a merge-blocking
  gate**: a non-improvement does not block the feature provided behaviour parity (FR-012) holds and
  any regression is explained.
- **FR-015**: All new/relocated library modules MUST live in `FS.Skia.UI.Build`, compile clean under
  `net10.0` / `TreatWarningsAsErrors`, inherit `Directory.Build.props`, and add no `PackageVersion`
  outside `Directory.Packages.props`.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package **identity** change. `FS.Skia.UI.Build` (already packable and
  consumed by generated projects since feature 043) gains internal `Engine/`, `GeneratedProduct`,
  `Guidance`, and `Preflight` modules; its **consumer-facing** contract (the packaged evidence
  engine) is unaffected. `build/Build.fsproj` is a non-packable build-tooling exe. A version bump in
  the normal merge/pack flow is expected but introduces no new `PackageVersion` outside
  `Directory.Packages.props`. No controls/chart/graph/DataGrid authoring change.
- **Public contract impact**: No product `.fsi` signatures, public APIs, sample contracts, or
  product surface baselines change. The only `.fsi` edits are **curated** signatures for the new
  governance-library modules (build-tooling surface, not product surface), per Principle II.
- **State workflow impact**: No change to the **runtime** stateful workflow, I/O, commands, effects,
  subscriptions, or interpreter. The MEL engine being relocated is the **build-side** model/effect
  interpreter, not the product Elmish runtime; its behaviour is preserved and proven by golden parity.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshots, Vulkan,
  Skia, visual output, or unsupported-environment diagnostics are touched.
- **Evidence obligations**: Real evidence required — a golden baseline of every target's
  output captured before migration and a byte-identical diff after; `update` typed-effect-list unit
  tests and relocated-validator unit tests; proof `build.fsx` is deleted (or ≤200-line shim,
  recorded); grep proofs of no `dotnet fake` / no `fake-cli` / no `FSharp.Compiler.*`; cold/warm
  build wall-clock recorded vs baseline; the standard serialized FAKE gate sequence green (`Dev`,
  `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`,
  `EvidenceAudit`). No synthetic evidence.
- **Unsupported scope**: Not in scope — generated-product contract **versioning** / `schema_version`
  / deprecation window (Stage 6.4), prose/skill/constitution **content** trimming (Stage 6),
  codifying remaining bucket-(a) prose rules as new production gates (Stage 6.1), evidence-artifact /
  `.gitignore` hygiene (Stage 6.5), any further Python/Bash porting, and any runtime, rendering,
  packaging-identity, or public-`.fsi` product-surface change.
- **Build-target impact**: This is a build-**system** change: every target's invocation path moves
  from the `dotnet fake` script to the compiled front-end, so **all** of `Dev`, `Verify`, `Ci`,
  `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`, `Route`, `RefreshSurfaceBaselines`, and
  the rest must remain registered and behaviour-identical. `fake.sh`/`fake.cmd` and
  `.config/dotnet-tools.json` change. `build.fsx` is removed. As a change to `build.fsx` /
  `scripts/build` / governance paths, `Route` escalates this to the full gate set.

## Success Criteria *(mandatory)*

- **SC-001**: `build.fsx` is **deleted** from the tracked tree (or ≤200 lines under the documented
  fallback), recorded against the 4,767-line working / 4,688-line Stage-0 baseline.
- **SC-002**: Every registered target's **deterministic governance reports/artifacts** are
  **byte-identical** to the pre-migration golden baseline (timestamps/absolute paths normalized;
  test-shelling targets matched by verdict + report), proven by a zero-byte diff across the parity
  oracle set — i.e. all targets **except** the enumerated, stash-control-justified pre-existing-RED
  exclusions (`FsiTranscripts`, `TemplateCheck` headless flake), which are disclosed, not hidden.
- **SC-003**: `dotnet run --project build/Build.fsproj -- <target>` runs each target; `./fake.sh` and
  `fake.cmd` route through the compiled front-end; `fake-cli` is absent from
  `.config/dotnet-tools.json` and no `dotnet fake` invocation remains (grep-proven).
- **SC-004**: No `FSharp.Compiler.*` / FCS reference exists anywhere (grep-proven); the build is
  compiled, not script-loaded.
- **SC-005**: `update` has direct unit tests asserting the typed `BuildEffect` list for
  representative targets, and the relocated generated-product / guidance / preflight validators have
  unit tests asserting typed findings; all green.
- **SC-006**: The MEL engine and the three heavy validators are modules of `FS.Skia.UI.Build` with
  curated `.fsi` surfaces, compiling clean under `TreatWarningsAsErrors`; the front-end contains no
  inlined orchestration/validation logic.
- **SC-007**: Cold-build and warm-build wall-clock are **recorded and reported** against the
  baseline. This is an observation, not a merge gate: warm builds are *expected* to be at least as
  fast as the prior `dotnet fake` script-recompile path, but a non-improvement does not block the
  feature provided SC-002 behaviour parity holds and any regression is explained.
- **SC-008**: All standing invariants hold — product public surface unchanged (`PackageSurfaceCheck`,
  `FsiTranscripts` no product baseline diff), runtime untouched (`git diff` over `src/**` = 0),
  generated consumers still fully governed (`TemplateCheck`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck` green), net10 conventions honoured, FAKE sequencing respected, evidence
  output vocabulary/counts unchanged.

## Key Entities *(include if feature involves data)*

- **Dedicated build front-end** — `build/Build.fsproj` (exe) + `build/Program.fs`; references the
  governance library, registers every target from the typed `Targets` graph via `Fake.Core.Target`,
  delegates target bodies to the library, dispatches via `Target.runOrDefaultWithArguments`.
- **MEL engine** — `BuildMsg` / `BuildEffect` / `BuildModel` + `update` (pure decision function) +
  `interpret` (I/O edge); relocated from `build.fsx` into `FS.Skia.UI.Build` `Engine/` modules.
- **Relocated validators** — `GeneratedProduct.fs` (generated-project structural validation),
  `Guidance.fs` (generated-guidance / skill-section scanners), `Preflight.fs` (process-health /
  bootstrap), behaviour-preserving moves of inline `build.fsx` logic.
- **Golden target-output baseline** — a captured set of every target's reports/artifacts from the
  current `build.fsx` path; the parity oracle the post-migration outputs are diffed against.
- **Launchers / toolchain** — `fake.sh`, `fake.cmd`, `.config/dotnet-tools.json`; rewired to
  `dotnet run` over the front-end with `fake-cli` removed.

## Assumptions

- **D2 confirmed → delete, don't shim.** Feature 039's spike proved `Fake.Core.Target` drives a
  target from a compiled exe with no FSX runner and no `FSharp.Compiler.*` dependency, so the
  default outcome is **deleting** `build.fsx`. The ≤200-line `#r`-the-DLL shim is retained only as a
  documented fallback if a concrete blocker appears; planning will confirm before relying on it.
- **Generated-product contract versioning is Stage 6.** This feature **moves** the ~800-line
  generated-product validation into the library behaviour-identically; adding `schema_version` and a
  deprecation window is explicitly deferred to Stage 6.4 to keep this feature a behaviour-preserving
  refactor with a clean parity proof.
- **Parity is captured before the move.** A golden baseline of all target outputs is taken from the
  current `build.fsx` path first; the move is gated on a byte-identical diff against it (the same
  capture-then-diff discipline that gated the Stage-4 Python port).
- **`Routing.fs` is already the routing source of truth** (feature 042); the front-end merely
  consumes it. No interim `select-tier.fsx` exists or is created.
- **FAKE remains the orchestration library**, just driven from a compiled exe rather than the
  `fake` CLI; the `==>` dependency wiring and target semantics are preserved, only relocated.
- **Spike scaffolding is superseded.** `SpikeHello`, `build/spike-verify.sh`, and any
  `build/SkillExamples` remnants are removed or replaced by the real front-end without affecting
  registered targets.

## Dependencies

- **Feature 039** (`build/Build.fsproj` + `build/Governance/FS.Skia.UI.Build.fsproj` skeleton;
  D2 spike) — the confirmed dedicated-project scaffold this feature grows into the real front-end.
- **Feature 041** (typed `Targets` DU, `TargetMetadata`, in-process dispatch) — the typed-target
  model the front-end registers against; a renamed/missing target is a compile error.
- **Feature 042** (`Routing.fs`, `ContractView`, `Route`) — the compiled routing source the
  front-end consumes; this feature performs no further routing migration (Stage 5.5 already done).
- **Feature 043** (`FS.Skia.UI.Build` made packable + consumed by generated projects; evidence
  engine in-process) — the packaged library the relocated modules join; the consumer contract must
  stay unaffected.
- **Feature 044** (single-source generation folded into `RefreshSurfaceBaselines`) — the
  regeneration/currency targets that must keep working when their bodies are driven from the new
  front-end.
- **Stage-0 baseline** (`docs/reports/_baselines/2026-05-31-foundations.md`) — the 4,688-line
  `build.fsx` figure SC-001's line-delta is measured against.

## Out of Scope

- Generated-product contract versioning / `schema_version` / deprecation window (Stage 6.4).
- Codifying remaining bucket-(a) prose rules as new production gates (Stage 6.1).
- Trimming or rewriting skill / constitution / template **content** (Stage 6.2–6.3).
- Evidence-artifact / `.gitignore` hygiene (Stage 6.5).
- Any further Python / Bash porting beyond what features 039–043 already delivered.
- Any runtime, rendering, packaging-identity, or public-`.fsi` product-surface change.
- New routing logic or any `select-tier.fsx` (Stage 5.5 was delivered in feature 042).
