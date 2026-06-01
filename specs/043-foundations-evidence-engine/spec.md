# Feature Specification: Foundations Evidence Engine Port (Stage 4)

**Feature Branch**: `043-foundations-evidence-engine`
**Created**: 2026-06-01
**Status**: Draft
**Input**: User description: "@docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md @docs/reports/2026-05-31-1049-foundations-implementation-plan.md implement the next part of the plan."

## Context & Scope

The next un-started part of the foundations programme is **Stage 4 — Port the Python
graph/audit into the library** (see `docs/reports/2026-05-31-1049-foundations-implementation-plan.md`).
Stages 0, 1, 2.1, and 3 are implemented (features 039–042); the governance library
`FS.Skia.UI.Build` exists at `build/Governance/` and the build already calls it in-process for
target-metadata, capability, skill-sync, and routing checks.

Today the project's flagship governance gate — the evidence graph and merge-gate audit — is the
**only** remaining computation that crosses three languages and a process boundary:

```
build.fsx (StartTarget EvidenceGraph/EvidenceAudit)
  → shells out to .specify/extensions/evidence/scripts/bash/run-audit.sh   (1,284 lines Bash)
    → invokes .specify/extensions/evidence/scripts/python/compute-task-graph.py  (1,310 lines)
    → invokes .specify/extensions/evidence/scripts/python/audit-status-scan.py   (150 lines)
      → writes task-graph.json / task-graph.md → re-parsed back in F# as opaque blobs
```

The F# side has no typed view of its own most important gate. This feature replaces the Python
engine with typed, unit- and property-tested F# inside `FS.Skia.UI.Build`, computing the evidence
graph and audit **in-process**, and proves byte-for-byte output parity against the Stage-0 golden
fixtures (`tests/Governance.Tests/fixtures/evidence-golden/{036,037,038}`) before any Python is
removed.

This is a designated **dogfood** feature: it runs the full Spec Kit + evidence pipeline on itself,
keeping the consumer governance harness honest. It is also a consumer-contract change (it updates
`template/base/**` so generated projects consume the packaged engine), so it **escalates** to the
full serialized gate set under the Stage-1 `Route` policy.

The runtime architecture (`Scene → SkiaViewer → Elmish`) and all public `.fsi` surfaces are **out
of scope and untouched**.

**Scope note (resolved during clarification):** `run-audit.sh` is not a thin wrapper around the two
standalone `.py` files — it embeds **9 Python heredocs / 25 `python3` calls** across these audit
sections: (1) graph compute, (2) SEH-summary, (3) readiness-contract scan, (4) persistent-launch
evidence scan, (5) persistent-GUI runtime readiness scan, (5b) window-visibility readiness scan,
(5c) audit-status region scan, (6) diff-scan, and the final verdict. This feature ports **all** of
them (the full audit), retires `run-audit.sh` entirely, and deletes every Python file.

## Clarifications

### Session 2026-06-01

- Q: How far should the Stage-4 port go — what does "no `python3` in the evidence path" (SC-003)
  mean for this feature? → A: **Full audit port.** Port all 9 embedded scans (graph, SEH-summary,
  readiness-contract, persistent-launch, persistent-GUI, window-visibility, audit-status, diff-scan,
  verdict) into in-process F#, fully retire and **delete** `run-audit.sh`, and delete every Python
  file. SC-003 ("zero `python3` in the evidence path") holds literally.
- Q: The 5 additional scans (readiness-contract, persistent-launch, persistent-GUI,
  window-visibility, diff-scan) have no Stage-0 golden fixture — how is their parity proven before
  the Python is deleted? → A: **Extend the golden oracle.** Before deleting Python, capture each
  additional scan's JSON output as **new committed golden fixtures** across the three fixture
  features (036/037/038) and byte-diff the F# output against them, extending the proven Stage-0
  byte-parity pattern.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Maintainer runs the evidence gate in-process with parity (Priority: P1)

A maintainer runs `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` on a
feature. The gates compute the task DAG, synthetic propagation, and audit verdict entirely in
compiled F# — no Bash orchestrator, no Python subprocess — and produce `task-graph.json`,
`task-graph.md`, and the audit count block that are **byte-identical** to what the Python engine
produced for the same inputs.

**Independent test:** Run both gates on each of the three Stage-0 golden-fixture features
(036, 037, 038) and diff the regenerated `task-graph.json`, `task-graph.md`, and audit count block
against the committed golden fixtures. A pass is **zero bytes of difference** on all three. This is
fully testable on its own and delivers the core value (typed, in-process flagship gate) even before
the Python files are deleted.

### User Story 2 - Algorithms are correct and provably so (Priority: P1)

A maintainer (or a future contributor) needs confidence that cycle detection, topological ordering,
and synthetic-evidence propagation behave correctly on graphs the golden fixtures don't exercise
(deep chains, multiple synthetic roots, cycles, empty graphs).

**Independent test:** Unit tests assert cycle detection flags a hand-built cyclic DAG and accepts an
acyclic one; topo sort returns a valid linearization; and property tests assert propagation
invariants (a graph with no synthetic roots yields no `auto-synthetic` nodes; propagation is
monotone). These run without any feature directory or git state.

### User Story 3 - Generated consumer projects stay fully governed without Python (Priority: P2)

An AI agent building an app from `dotnet new fs-skia-ui` runs the consumer's `EvidenceGraph` /
`EvidenceAudit` gates. They pass using the **packaged** `FS.Skia.UI.Build` engine rather than a
copied-in Python + `run-audit.sh`.

**Independent test:** `./fake.sh build -t GeneratedProductCheck` (and `TemplateCheck`) are green
with the template no longer carrying the Python evidence scripts, and the generated project's
evidence gates produce a valid audit verdict.

### Edge Cases

- A `tasks.md` / `tasks.deps.yml` pair containing a dependency **cycle** → the gate fails with the
  same error vocabulary the Python engine emitted (no silent pass, no hang).
- A `done` task whose dependency is `synthetic` → propagates to `auto-synthetic` exactly as the
  Python rule did; an unaccepted `[S]` blocks the audit.
- Both legacy bare-list and object (`{deps, skillist}`) forms of `tasks.deps.yml` parse identically.
- `audit-status` structured regions: first-region-wins, duplicate-key is an error, prose is never
  interpreted — preserved faithfully from `audit-status-scan.py`.
- The readiness-contract, persistent-launch, persistent-GUI, and window-visibility scans preserve
  their exact blocking/advisory severity, hit vocabulary, and output JSON shape when a feature's
  `readiness/` directory is missing required files or acceptance keywords (so a feature that blocked
  under the Python audit still blocks under the F# audit, and vice-versa).
- A feature whose evidence graph fails to compute (e.g. missing skill-loading evidence, as
  `017-synthetic-error-evidence` does at the Stage-0 SHA) → same non-zero exit / `error` verdict
  semantics as today.
- The known `SkiaViewer.Tests` headless flake and `FsiTranscripts` toolchain issue are
  pre-existing, runtime-side, and out of scope (a stash-control proves independence if they recur).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001 (parser port)**: The system MUST parse `tasks.md` in compiled F# — task ids, status
  boxes (`[ ]`, `[X]`, `[S]`, `[F]`, `[-]`, `[*]`), `[P]`/`[US]`/tier/`[SEH]` annotations,
  phase-checkpoint edges, and Synthetic-Evidence Inventory tables — reproducing the structure
  `compute-task-graph.py` extracts.
- **FR-002 (deps port)**: The system MUST parse `tasks.deps.yml` in compiled F# supporting **both**
  the legacy bare-list form and the object `{deps, skillist}` form, via a managed YAML reader
  (`YamlDotNet`) behind a typed model — no bespoke hand-rolled YAML parser.
- **FR-003 (skill registry port)**: The system MUST discover the skill registry across
  `.agents/skills`, `src/*/skill`, and `template/fragments/*/skill` as the Python engine does, for
  skill-id resolution.
- **FR-004 (graph algorithms)**: The system MUST compute cycle detection (3-colour DFS) and
  topological order (Kahn) in compiled F#, returning typed results (not "ok"/"failed" strings).
- **FR-005 (synthetic propagation)**: The system MUST implement synthetic propagation as a pure
  function: `declared=synthetic → synthetic`; `declared=done ∧ any dependency synthetic/auto →
  auto-synthetic`; otherwise `declared`. Behaviour MUST match the Python rule exactly.
- **FR-006 (audit + status regions)**: The system MUST compute cross-file consistency (every task in
  `tasks.md` ↔ `tasks.deps.yml`), skill-id resolution, skill-ordering (`evidence-audit` not before
  `evidence-graph`), `[SEH]` design-phase-only timing, the audit verdict (`PASS`/`FAIL`/`BLOCKED`),
  and the `audit-status` structured-region scan (first-region-wins, duplicate-key = error, no prose
  interpretation) — faithfully porting `audit-status-scan.py`.
- **FR-006a (full embedded-scan port)**: The system MUST also port the remaining audit scans
  currently embedded as Python heredocs inside `run-audit.sh` — the **readiness-contract scan**, the
  **persistent-launch evidence scan**, the **persistent-GUI runtime readiness scan**, and the
  **window-visibility readiness scan** — into in-process F#, preserving each scan's blocking
  semantics, hit vocabulary, and output JSON shape (`readiness-contract-hits.json`,
  `persistent-launch-hits.json`, `persistent-gui-runtime-hits.json`, `window-visibility-hits.json`). After
  this port, the `EvidenceAudit` verdict is computed entirely in F# from these scans plus FR-005/
  FR-010, with no Python contribution.
- **FR-007 (rendering parity)**: The system MUST render `task-graph.json`, `task-graph.md`, the
  Mermaid diagram, the ASCII tree, and the audit count block **byte-compatible** with the Python
  output schema, so the Stage-0 golden fixtures match exactly.
- **FR-008 (status vocabulary parity)**: The audit MUST emit the same status vocabulary and counts as
  the Python it replaces — `accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`,
  `late-seh-tasks` — with identical values for identical inputs (Invariant 6).
- **FR-009 (in-process gate wiring)**: `EvidenceGraph` and `EvidenceAudit` MUST compute results by
  calling `FS.Skia.UI.Build` in-process; they MUST NOT shell out to `run-audit.sh` or invoke any
  Python interpreter in the steady state.
- **FR-010 (diff-scan port)**: The diff-scan portion of `run-audit.sh` (git pattern matching against
  the audit patterns) MUST be ported to compiled F#, preserving its blocking semantics; only thin OS
  glue (e.g. invoking `git`) may remain external.
- **FR-011 (decommission)**: Once parity is proven, the system MUST delete
  `compute-task-graph.py`, `audit-status-scan.py`, **and `run-audit.sh` entirely** (no thin shim is
  retained, per the full-audit-port clarification), along with all embedded Python heredocs they
  contained. No `python3` invocation may remain anywhere in the evidence path (grep MUST prove this).
- **FR-012 (parity-gated removal)**: The Python engine (the two `.py` files and `run-audit.sh` with
  its embedded scans) MUST remain runnable in parallel (behind a `--legacy-evidence` selector or
  equivalent) until byte-identical parity is signed off across **all** golden fixtures — the original
  three (`task-graph.json`, `task-graph.md`, audit counts) plus the extended scan-output fixtures
  (FR-018); only then are the Python files, `run-audit.sh`, and the legacy path removed.
- **FR-013 (consumer distribution)**: The template (`template/base/**`) MUST be updated so generated
  projects consume the **packaged** `FS.Skia.UI.Build` evidence engine instead of carrying the Python
  scripts + `run-audit.sh`. Generated projects MUST still pass their `EvidenceGraph`/`EvidenceAudit`
  gates.
- **FR-014 (typed tests)**: New unit and property tests MUST assert **typed** results (graph nodes,
  propagation states, audit verdicts, finding records), not string-matching, and MUST live in
  `tests/Governance.Tests` referencing the library directly.
- **FR-015 (dogfood)**: This feature MUST run the full Spec Kit + evidence pipeline on itself (it is a
  designated dogfood feature) and clear the serialized six-target gate set.
- **FR-016 (no FCS / runtime script loading)**: No FSharp Compiler Services / runtime-script-loading
  dependency may be introduced; the engine is plain compiled F# (grep MUST prove no
  `FSharp.Compiler.*` reference is added).
- **FR-017 (extended golden oracle)**: Before any Python is deleted, the system MUST capture the
  output JSON of each newly-ported scan (`readiness-contract-hits.json`, `persistent-launch-hits.json`,
  `persistent-gui-runtime-hits.json`, `window-visibility-hits.json`, and the diff-scan hit list) from the
  current Python engine across the three fixture features (036/037/038) and commit them as **new
  golden fixtures** under `tests/Governance.Tests/fixtures/evidence-golden/`. The F# output MUST diff
  byte-identical against these fixtures (extending the Stage-0 parity oracle to the full audit).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: `FS.Skia.UI.Build` (the published governance-library package per ADR D1) gains
  the evidence-engine modules, so its package **contents** change and it requires a version bump on
  the next pack. Generated consumers shift from copied Python scripts to a package reference on the
  published engine. No other package identity changes; no runtime/product package is affected.
- **Public contract impact**: No product `.fsi` signatures, documented public product APIs, sample
  contracts, or product surface baselines change. New library surface is exposed only through curated
  `.fsi` files on the new governance modules (internal build tooling, not the product's public
  contract). `PackageSurfaceCheck`/`FsiTranscripts` show no product baseline diff.
- **State workflow impact**: No product stateful-workflow / command / effect / subscription /
  interpreter behaviour changes. The only "interpreter" touched is `build.fsx`'s `BuildEffect`
  interpreter edge, where two `StartTarget` cases stop emitting a `processEffect` to `run-audit.sh`
  and instead call the library in-process.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshots, Vulkan,
  Skia, or visual-output change. "Rendering" here means rendering governance **text** artifacts
  (`task-graph.md`/JSON/Mermaid), not visual output.
- **Evidence obligations**: Real evidence required — golden-fixture byte-parity diffs for
  036/037/038 covering both the original outputs (`task-graph.json`, `task-graph.md`, audit count
  block) **and** the newly-captured scan-output fixtures (`readiness-contract-hits.json`,
  `persistent-launch-hits.json`, `persistent-gui-runtime-hits.json`, `window-visibility-hits.json`, diff-scan
  hits) per FR-017; unit/property test results for cycle detection, topo sort, propagation, and
  status-region scanning; a grep proof that no `python3` / `run-audit.sh` / `FSharp.Compiler.*`
  remains in the evidence path; the serialized-gate run log; and the generated-consumer evidence-gate
  pass. No synthetic evidence.
- **Unsupported scope**: The runtime architecture, public product surface, V3 modular package split,
  and the remaining Stage 2.2–2.5 / Stage 5 / Stage 6 work are out of scope. No visual, release,
  platform, or distribution-channel changes. The heavy Spec Kit Bash (`common.sh`, git scripts) is
  **not** ported here (deferred).
- **Build-target impact**: `EvidenceGraph` and `EvidenceAudit` change (now in-process). `TemplateCheck`,
  `GeneratedProductCheck`, and `GeneratedGuidanceCheck` are exercised because `template/base/**`
  changes (consumer distribution). `Dev` and the `Route`-selected escalated gate set run as the
  dogfood pipeline. `PackLocal` is affected only in that the `FS.Skia.UI.Build` package contents grow
  (version bump at merge). No change to `DependencyReport`/`TemplateDrift` semantics beyond the
  package-content delta.

## Success Criteria *(mandatory)*

- **SC-001 (parity)**: For each of the three golden-fixture features (036, 037, 038), the
  F#-regenerated `task-graph.json`, `task-graph.md`, and audit count block differ from the committed
  golden fixtures by **0 bytes**.
- **SC-001a (extended-scan parity)**: For each of the three fixture features, the F#-produced output
  of every newly-ported scan (`readiness-contract-hits.json`, `persistent-launch-hits.json`,
  `persistent-gui-runtime-hits.json`, `window-visibility-hits.json`, and the diff-scan hit list) differs from
  its newly-committed golden fixture (FR-017) by **0 bytes**.
- **SC-002 (algorithm coverage)**: Cycle detection, topological sort, synthetic propagation, and
  status-region scanning each have passing unit and/or property tests asserting typed results,
  including at least one cyclic-graph, one multi-synthetic-root, and one empty-graph case.
- **SC-003 (no Python in evidence path)**: A repository grep finds **zero** `python3`/`python`
  invocations and **zero** references to `compute-task-graph.py` / `audit-status-scan.py` /
  `run-audit.sh` in the steady-state evidence path; both Python files **and** `run-audit.sh` (with all
  9 embedded heredocs) are **deleted** — no shim is retained.
- **SC-004 (no FCS)**: A grep proves no `FSharp.Compiler.*` dependency was added anywhere.
- **SC-005 (language reduction)**: The languages in the evidence path drop from {F#, Bash, Python} to
  {F#} (plus thin OS-glue git invocation), recorded against the Stage-0 baseline.
- **SC-006 (consumer governance intact)**: `TemplateCheck`, `GeneratedProductCheck`, and
  `GeneratedGuidanceCheck` are green with the template consuming the packaged engine; a generated
  project's `EvidenceGraph`/`EvidenceAudit` produce a valid verdict.
- **SC-007 (invariants hold)**: Invariants 1–6 from the implementation plan hold — public product
  surface unchanged, runtime untouched (`git diff` over `src/**` for product runtime = 0), generated
  consumers still fully governed, net10 conventions honoured (no new `PackageVersion` outside
  `Directory.Packages.props`), FAKE sequencing respected, and evidence output parity proven.
- **SC-008 (dogfood pipeline green)**: This feature's own evidence audit returns `verdict=PASS`
  (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 diff-scan, 0 readiness-contract blocking)
  with zero synthetic evidence, and the serialized six-target gate set is green (pre-existing
  environment-side flakes, if any, isolated by a stash control).

## Assumptions

- **A1**: "The next part of the plan" resolves to **Stage 4** (Python evidence-engine port). Rationale:
  Stages 0/1/2.1/3 are implemented; the implementation plan's suggested sequencing places Stage 4 as
  the next library-track step after Stage 3 ("highest-value port, the flagship gate"), and it depends
  only on Stage 3 (done) plus the Stage-0 golden fixtures (present). Stage 2.2–2.5 is independent and
  could be done in parallel, but the plan's spine is 3→4→5→6, so Stage 4 is the canonical next step.
- **A2**: The three committed golden fixtures (036/037/038) are the authoritative parity oracle, per
  Stage 0, **extended** in this feature with newly-captured golden fixtures for the additional audit
  scans (FR-017). The remaining `[S*]` auto-synthetic / unaccepted-count coverage gap (no stable
  feature exercises it) is still handled with hand-built unit/property fixtures (SC-002).
- **A3**: `YamlDotNet` is already a managed dependency (used by the Stage-3 capability catalog) and is
  the YAML reader for `tasks.deps.yml`; no new bespoke parser is introduced.
- **A4**: The library is distributed as the published `FS.Skia.UI.Build` package (ADR D1); the repo
  build front-end project-references it in-solution and generated consumers package-reference it.
- **A5**: The engine is plain compiled F# (ADR D6) — no FCS, no runtime-loaded `.fsx`.

## Dependencies

- Stage 3 (`FS.Skia.UI.Build` library + `tests/Governance.Tests` reference) — **done** (feature 041).
- Stage 0 golden evidence fixtures under `tests/Governance.Tests/fixtures/evidence-golden/` — **present**.
- Stage 1 `Route` policy (this change escalates to the full gate set as a consumer-contract + dogfood
  feature) — **done** (feature 042).
- `YamlDotNet` managed dependency — present.

## Out of Scope

- The framework runtime (`Scene`, `SkiaViewer`, `Elmish`, `Layout`, `Controls`, `Lib`) and all public
  product `.fsi` surfaces.
- Stage 2.2–2.5 (single-source generation of skills/constitution/skillist).
- Stage 5 (dedicated build front-end, MEL-engine extraction, `build.fsx` retirement) beyond the two
  `StartTarget` evidence cases this feature rewires.
- Stage 6 (codifying remaining prose rules, contract versioning, prose trim) and Stage 7 (decommission
  + final measurement).
- Porting the heavy Spec Kit Bash (`common.sh`, git scripts).
- The V3 modular package split.
