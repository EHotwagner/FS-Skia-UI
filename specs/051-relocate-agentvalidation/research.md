# Phase 0 Research — V3 Stage 2: Relocate `AgentValidation`

All open questions in the spec's Assumptions block are engineering placements with
obvious, low-stakes defaults. They are resolved here against the live tree (branch
`051-relocate-agentvalidation`, baseline pin per the programme plan). No
`NEEDS CLARIFICATION` remains.

## D1 — Destination namespace

- **Decision**: The relocated module becomes `namespace FS.Skia.UI.Build.AgentValidation`
  (today it is `namespace FS.Skia.UI.AgentValidation`).
- **Rationale**: Matches the existing governance-library siblings
  `FS.Skia.UI.Build.Routing`, `FS.Skia.UI.Build.TargetMetadata`,
  `FS.Skia.UI.Build.PerPackageSurface`. It is a distinct fully-qualified namespace from
  `FS.Skia.UI.Build.Front.Support` (the module that carries the front-end's minimal shadow
  types `ValidationSelectionModel`/`ValidationSelectionMsg`/`ValidationSelectionEffect`/
  `AgentVerdict`), so the same-named richer types **cannot collide or silently shadow** —
  satisfying the spec's "namespace collision" edge case (FR-011).
- **Alternatives considered**:
  - Keep `FS.Skia.UI.AgentValidation` (rejected — it advertises the runtime monolith
    namespace from a build-tooling library and is misleading; FR-007 requires the monolith
    namespace to vanish from all consumers).
  - Merge into `FS.Skia.UI.Build.Front.Support` (rejected — the spec explicitly scopes out
    reconciling/unifying the shadow types; that is a redesign, not a relocation).

## D2 — Compile-order slot in `FS.Skia.UI.Build.fsproj`

- **Decision**: Insert `AgentValidation.fsi` then `AgentValidation.fs` **immediately after
  the `Spike.fsi`/`Spike.fs` scaffold pair**, i.e. at the head of the capability list,
  before `SkillTreeGen` and (importantly) before `Routing`.
- **Rationale**: `src/Lib/AgentValidation.fs` opens only `System`, `System.Diagnostics`,
  `System.IO`, `System.Text`, `System.Text.Json` — **BCL-only, no governance-module
  dependency** (verified by grep: no `open FS.Skia.*`, no `InternalsVisibleTo` coupling).
  It therefore compiles correctly at any slot; placing it at the head keeps it ahead of
  every governance capability so no later module can shadow it and it is trivially
  discoverable. Placing it **before `Routing.fs`** is forward-compatible: the Stage-5
  per-package Route rule will want `Routing` to consume `knownGates`, which the F# compile
  order requires to come first (this stage only makes that *possible* — FR-008).
- **Alternatives considered**: Co-locate beside `TargetMetadata`/`Routing`/`ContractView`
  (the gate/contract cluster). Rejected as marginally more "thematic" but it would place
  `AgentValidation` *after* `Routing`, defeating the Stage-5 forward-compat above and
  buying nothing — there is no compile dependency in either direction today.

## D3 — Curated `.fsi` and doc-comment adaptation

- **Decision**: The relocated module keeps an explicit curated `.fsi` (its current 261-LOC
  surface, unchanged in shape). The only edits are (a) the `namespace` line and (b) the
  recurring doc-comment phrase `"…exposed by this FS.Skia.UI package."` adapted to
  `"…exposed by the FS.Skia.UI.Build governance library."` so the surface no longer claims
  to be runtime-package contract.
- **Rationale**: Principle II requires the curated `.fsi`; FR-003 requires the same logical
  surface "adapted only for the new namespace". The doc-comment phrase is a behaviour-free
  text adaptation that keeps the surface honest. No `val`/`type` is added, removed, or
  retyped — so behaviour parity (FR-004) is structural, not just tested.
- **Alternatives considered**: Drop the `.fsi` and rely on Build's compile order (rejected —
  violates Principle II and the spec's curated-boundary edge case).

## D4 — Surface-baseline handling

- **Decision**: Remove the 48 `FS.Skia.UI.AgentValidation.*` lines (the first 48 of 130) from
  `readiness/surface-baselines/FS.Skia.UI.txt` — the **monolith** aggregate surface
  baseline that `PackageSurfaceCheck` validates. Add **no** new baseline for the gained
  module.
- **Rationale**: `readiness/per-package-surface-expectations.md` states the build-tooling
  library `FS.Skia.UI.Build` is **excluded** from both the per-package surface baselines and
  the monolith surface sweep (it lives under `build/`, deliberately out of the surface
  tooling's reach — see the `FS.Skia.UI.Build.fsproj` header comment). So the move produces
  exactly one surface delta: the monolith shedding the module (FR-010/SC-006). The eight
  per-package runtime baselines are untouched (`PerPackageSurfaceDiff`/`PackageSurfaceCheck`
  stay green).
- **Alternatives considered**: Capture a `FS.Skia.UI.Build.txt` baseline for the gained
  surface (rejected — the surface tooling does not sweep `build/`; introducing a baseline
  there is out of scope and would itself be a governance-tooling change this stage avoids).

## D5 — Behaviour-parity demonstration

- **Decision**: Parity is demonstrated two ways: (1) **structural** — the `.fs`/`.fsi`
  bodies move byte-for-byte except the namespace line + doc-comment phrase (a `git
  diff -M` rename shows ~100% similarity); (2) **behavioural** — the relocated
  `AgentValidationFrameworkTests` suite (same assertion count, same fixtures) passes against
  the new home, exercising contract parse accept/reject diagnostics, the `knownGates` set,
  the `ValidationSelection` MVU transitions, and `AgentVerdict` (de)serialization. This is
  **real, non-synthetic** evidence (real `validation.contract.yml`-shaped fixtures + the
  real interpreter's file/git edges).
- **Rationale**: Satisfies SC-002/SC-003 ("same number of assertions… identical accept/
  reject diagnostics… identical `knownGates` set") without inventing a separate parity
  harness — the existing test IS the oracle, simply repointed.
- **Alternatives considered**: A bespoke before/after diff oracle over the parser (rejected —
  redundant with the structural rename diff + the repointed suite; adds synthetic surface
  for no gain).

## D6 — Sole-consumer re-verification

- **Decision**: Treat `tests/Governance.Tests/AgentValidationFrameworkTests.fs` as the only
  consumer; repoint its `open` and drop `Governance.Tests → src/Lib/Lib.fsproj`.
- **Rationale**: Re-verified on this branch — `grep -rn "FS.Skia.UI.AgentValidation"`
  over `*.fs(i)`/`*.fsproj`/`*.fsx` returns only that test's `open` plus the two
  `Compile Include` lines in `src/Lib/Lib.fsproj` (the files being moved). The build
  front-end already does **not** reference it (Stage-0 finding; `Front/Support.fs` carries
  its own shadow types). FR-007's grep gate covers the case where any other consumer is
  later discovered.
- **Alternatives considered**: none — this is a fact about the tree, not a choice.
