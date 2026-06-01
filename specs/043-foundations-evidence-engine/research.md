# Phase 0 Research — Foundations Evidence Engine Port (Stage 4)

All Technical-Context unknowns are resolved below. Each item: **Decision / Rationale / Alternatives considered.** No `NEEDS CLARIFICATION` remains.

## R1 — Consumer distribution mechanism (FR-013)

- **Decision**: **Full pack-and-publish.** Flip `FS.Skia.UI.Build` to `IsPackable=true`, give it `PackageId`/version metadata, add it to the `PackLocal` flow and `Directory.Packages.props`, and rewrite `template/base/build.fsx` so generated projects reference the **packaged** engine (`#r "nuget: FS.Skia.UI.Build, <pin>"` via the paket header, then call `Evidence.*` in-process). `.template.config/template.json` stops copying `.specify/extensions/evidence/scripts/**`.
- **Rationale**: Matches FR-013 / SC-006 literally and the package-impact governance prompt ("Generated consumers shift from copied Python scripts to a package reference on the published engine"). Confirmed with the maintainer (clarifying question, 2026-06-01). One port serves repo + consumers; consumers stay lean (no source copies). ADR D1 already designated `FS.Skia.UI.Build` as the published governance-library package.
- **Alternatives considered**: (a) *Source-ship interim* — ship the Evidence modules as source into the template, package later. The implementation plan's Stage-4 risk-mitigation sanctions this fallback; rejected as the primary path because it leaves FR-013/SC-006 partially deferred. **Retained as the documented fallback if packaging slips** (see R9). (b) *Repo-only, defer all consumer distribution* — rejected: would not satisfy SC-006 as written.

## R2 — Module decomposition under `build/Governance/Evidence/`

- **Decision**: Ten modules mirroring the Stage-4 work-item split (`TaskParser`, `DepsParser`, `SkillRegistry`, `Graph`, `StatusRegion`, `Scans`, `DiffScan`, `Audit`, `Render`, `Engine`), each with a curated `.fsi` (Principle II). Compile order appended **after** `Capabilities.fs` in `FS.Skia.UI.Build.fsproj`; dependency order is parsers → registry → graph → scans/status/diff → audit → render → engine.
- **Rationale**: Mirrors the existing one-module-per-validator pattern (`Capabilities.fs`, `TargetMetadata.fs`, `Routing.fs`) the repo already proved in Stages 3–4. Keeps each module ≤ a few hundred lines and independently unit-testable. `Engine` is the only module that orchestrates; everything below it is pure over passed-in inputs.
- **Alternatives considered**: One monolithic `Evidence.fs` (rejected — too large, harder to test in isolation, fights Principle III legibility); merging `Scans` into per-scan modules (deferred — the four readiness scans share key=value parsing helpers, so one `Scans` module with internal sub-functions is plainer; can split later if it grows).

## R3 — Byte-parity strategy and the extended oracle

- **Decision**: Port behind a parity gate. Keep the Python engine runnable in parallel behind a `--legacy-evidence` selector (FR-012). The parity oracle is the three committed Stage-0 fixtures (`task-graph.json`, `task-graph.md`, `audit-counts.txt` for 036/037/038), **extended** by capturing each newly-ported scan's JSON output from the **current Python engine** across the same three features and committing them as new golden fixtures under `fixtures/evidence-golden/<F>/scans/` (FR-017) **before** any Python is deleted. F# output is byte-diffed (DiffPlex) against every fixture; sign-off = 0 bytes on all. Only then are the Python files, `run-audit.sh`, and the legacy path removed.
- **Rationale**: Extends the proven Stage-0 pattern; the five additional scans (readiness-contract, persistent-launch, persistent-gui-runtime, window-visibility, diff-scan) have no Stage-0 fixture, so capture-then-diff is the only way to prove parity before deletion. Resolved in spec clarification Session 2026-06-01.
- **Byte-parity hazards to honour** (from the Python source map): JSON key ordering (`compute-task-graph.py` sorts tasks by id — emit a deterministic, id-sorted map; match key order field-by-field); JSON indentation/separators (match Python `json.dumps` spacing exactly); Markdown 4-space nested-list indentation; Mermaid `classDef` CSS exactly (`fill`/`stroke`/`stroke-width`/`stroke-dasharray`); diff parsing assumes `git diff` unified output with the same context behaviour; trailing newline presence. The golden-diff test is the gate that catches any divergence.

## R4 — `tasks.deps.yml` parsing (FR-002)

- **Decision**: Read both YAML forms (legacy bare-list `T001: [T000]` and object `T001: {deps: [...], skillist: [...]}`) with **`YamlDotNet`** behind a typed `DepsModel`, mirroring how Stage 3's `Capabilities.fs` reads its catalog. No bespoke hand-rolled YAML parser (the Python had one; it is **not** ported).
- **Rationale**: `YamlDotNet` is already a central managed dependency (A3). A typed model + `YamlDotNet` is the constitution-preferred path (managed reader, no clever parsing). A DepsParser fixture suite covers both forms (spec Edge Cases) and the empty/unparseable-file blocking-error case.
- **Alternatives considered**: Porting the Python minimal-YAML parser verbatim (rejected — re-introduces an untyped bespoke parser the programme is removing); JSON intermediate (rejected — unnecessary, changes the on-disk format consumers author).

## R5 — Algorithms: cycle detection, topo sort, propagation (FR-004/005)

- **Decision**: Hand-roll 3-colour DFS cycle detection and Kahn topological sort over a typed adjacency model in `Graph.fs`, returning typed results (`Cycle list`, `TopoOrder`) — not `"ok"/"failed"` strings. Synthetic propagation is a pure function: `declared=synthetic → synthetic`; `declared=done ∧ any dependency synthetic/auto-synthetic ∧ not accepted-seh → auto-synthetic`; else `declared`. Kahn queue ordering is **deterministic** (sorted by task id) to match Python tie-breaking.
- **Rationale**: Standard, fully unit/property-testable; the `fsharp-graph-algorithms` capability skill covers exactly this. Property tests assert monotonicity and "no synthetic roots ⇒ no auto-synthetic nodes" (SC-002). The `accepted-seh` exclusion is part of the Python rule and must be preserved (036 fixture has `accepted-seh-tasks=1`).
- **Alternatives considered**: A library graph package (rejected — over-kill, new dependency, hides the rule the audit must prove); recursion-only propagation (use topo-order iteration with a `mutable` accumulator where plainer per Principle III).

## R6 — MVU/effect boundary for the gate rewire (Principle IV)

- **Decision**: Add `EvidenceGraphCheck`/`EvidenceAuditCheck` cases to `build.fsx`'s `BuildEffect` (alongside `CapabilityCatalogCheck`/`RouteSelect`). `update`'s `StartTarget EvidenceGraph`/`EvidenceAudit` arms emit these effect *values* (pure); `interpret` executes them by reading the feature's `tasks.md`/`tasks.deps.yml`/readiness files + a `git diff`, calling `Evidence.Engine.runGraph`/`runAudit`, and writing the artifacts. The library functions take inputs as data and return typed results — no I/O inside the pure core.
- **Rationale**: Exactly the in-process pattern Stages 3–4 established (`runCapabilityCatalogCheck`, `runRouteSelection`). Keeps `update` pure (testable: assert emitted effect list) and confines filesystem/`git`/write side-effects to the edge. `git` invocation is the only retained external process (thin OS glue, FR-010/SC-005) via the `fsharp-shell-process` skill.
- **Alternatives considered**: Calling the engine directly inside `update` (rejected — violates Principle IV, breaks pure-transition tests); keeping a thin `run-audit.sh` shim (rejected — FR-011 forbids any shim; full deletion required).

## R7 — Re-pointing existing evidence tests (FR-014)

- **Decision**: Re-point `AuditStatusRegionTests.fs` (today shells `python3 audit-status-scan.py`) to call `Evidence.StatusRegion`; `PersistentViewerEvidenceTests.fs` and `SyntheticErrorEvidenceTests.fs` (today shell `bash run-audit.sh` / `python3 compute-task-graph.py`) to call `Evidence.Engine`/`Scans`. Keep their committed fixture inputs; change only the invocation to typed library calls asserting typed results.
- **Rationale**: These suites already encode the exact blocking/advisory semantics and hit vocabulary the F# scans must preserve — re-pointing them turns the parity contract into typed assertions (FR-008/FR-014) and removes the last python3/bash invocations from the test path (SC-003). `GovernanceEvidenceTests.fs` mostly asserts file contents already; minimal change.
- **Alternatives considered**: Leaving them shelling out (rejected — would leave `python3`/`run-audit.sh` in the evidence path, failing SC-003); rewriting fixtures (rejected — fixtures are the parity inputs, must stay stable).

## R8 — Packaging `FS.Skia.UI.Build` (consequence of R1)

- **Decision**: `IsPackable=true` with `PackageId=FS.Skia.UI.Build`, version managed in the repo's bump flow (Framework Governance Prompt: version bump on next pack). Add to `PackLocal` (pack output → `~/.local/share/nuget-local/`) and `Directory.Packages.props`; generated `template/base/Directory.Packages.props` gets a matching pin (bumped alongside the other `FS.Skia.UI.*` pins). `DependencyReport`/`PackageSurfaceCheck` coverage extended to the new identity.
- **Rationale**: ADR D1 designated this package as published; R1 requires it now. The package ships build-tooling governance modules only — **not** referenced by any `src/**` product project, so no product/runtime package is affected (governance prompt). `YamlDotNet` becomes a transitive dependency of the published package; record in `docs/reports/dependencies.md`.
- **Alternatives considered**: A separate slim "evidence-only" package (rejected — fragments the governance library against ADR D1's single-package decision; defer any split to V3).

## R9 — Risks and fallbacks

- **Parser divergence** (Markdown table / indentation edge cases): the golden-diff gate blocks merge until 0 bytes; Python stays runnable behind `--legacy-evidence` until sign-off.
- **Packaging slips** (publish/version-flow friction): documented fallback is R1(a) source-ship into the template as an interim, package in a follow-up — recorded as a bounded deferral, not a silent gap.
- **`YamlDotNet` parses minimal YAML differently than the Python bespoke parser**: DepsParser fixture suite covers both forms; golden parity gate catches any difference in `task-graph.json`.
- **Pre-existing environment flakes** (`SkiaViewer.Tests` headless crash, `FsiTranscripts` toolchain issue): out of scope, runtime-side; isolate with a stash control if they recur (spec Edge Cases, SC-008).
