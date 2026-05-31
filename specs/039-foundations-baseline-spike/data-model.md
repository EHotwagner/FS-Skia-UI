# Phase 1 Data Model: Foundations Baseline & Build-Library Spike

This feature produces **documents and fixtures**, not runtime domain types. The "entities" below are the artifacts the feature commits; each lists its required fields (what a reviewer must find), validation rules (from the spec's acceptance scenarios), and the requirement it satisfies.

---

## Entity: BaselineRecord

The point-in-time "before" snapshot. One document: `docs/reports/_baselines/2026-05-31-foundations.md`.

| Field | Description | Rule |
|---|---|---|
| `git_commit` | Exact SHA the baseline describes | MUST be present and pinned (FR-001, FR-004 edge case, SC-001) |
| `build_fsx_lines` | `build.fsx` total line count | MUST equal `wc -l build.fsx` at the SHA (4,688 at authoring) |
| `build_fsx_breakdown` | Orchestration-vs-validation split | MUST distinguish orchestration (MEL/`interpret`/`StartTarget`) from validation (`Validate*`) lines |
| `governance_md_lines` | Governance Markdown counts | MUST include `.claude/skills` ↔ `.agents/skills` mirror, `.specify/memory/constitution.md`, templates, `specs/**` |
| `language_mix` | F# / Bash / Python LOC | MUST be three explicit counts with the `git ls-files`+`wc` command recorded |
| `ceremony_time_estimate` | Per-feature ceremony hours | MUST record the current estimate (~12–14h) and label it an estimate |
| `golden_fixture_manifest` | Which features captured + where | MUST list the three features and fixture paths; MUST record any substitution |
| `meta_process_link` | Pointer to the meta-process record | MUST link to `plan.md` §Programme Meta-Process |
| `measurement_commands` | Literal commands per metric | MUST be reproducible by a reviewer (SC-001) |

**Validation**: a reviewer reading the doc finds every required metric *and* the SHA, and can re-run each recorded command to reproduce the numbers (SC-001).

---

## Entity: GoldenFixtureSet

The frozen evidence outputs that become the Stage 4 parity oracle. One subtree per feature under `tests/Governance.Tests/fixtures/evidence-golden/<feature>/`.

| Field | Description | Rule |
|---|---|---|
| `feature` | The feature the fixture was captured from | MUST be one of the three selected frozen features (038, 037, 017) or a recorded substitute |
| `task_graph_json` | `task-graph.json` output | MUST be the unmodified output of the existing `EvidenceGraph` path |
| `task_graph_md` | `task-graph.md` output | MUST be the unmodified output of the existing `EvidenceGraph` path |
| `audit_counts` | Audit status/count block | MUST capture `accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, `late-seh-tasks` |
| `source_commit` | SHA the fixture was captured at | MUST match the baseline `git_commit` |

**Validation rules**:
- **Reproducible (FR-003, SC-002)**: re-running the existing evidence commands on the same feature at the same SHA regenerates output **byte-for-byte identical** to the committed fixture.
- **Deterministic (Edge Case)**: if a re-run differs, the non-determinism MUST be removed (deterministic re-capture) or the feature substituted and recorded; an unstable fixture is never committed.
- **Captured via existing engine unchanged (FR-011)**: no edit to the Python/Bash evidence path.
- **Designation**: the set is explicitly labelled the Stage 4 parity oracle.

---

## Entity: ArchitectureDecisionRecord (ADR)

Five records under `docs/adr/`, one per shaping decision.

| Field | Description | Rule |
|---|---|---|
| `id` / `date` | Dated, sequential ADR identifier | MUST be discrete and dated (SC-005) |
| `decision` | The choice made | MUST state the resolved decision |
| `alternatives` | Options considered | MUST list alternatives (FR-004, SC-005) |
| `rationale` | Why this choice | MUST state rationale |
| `stages_shaped` | Which later stages depend on it | MUST name the affected stages |

**Required set (FR-004)**: D1 (governance-library placement & distribution), D2 (build front-end form), generated-product contract versioning policy, D4 (Spec Kit fork stance), D6 (configuration representation). Exactly one ADR per decision (SC-005).

---

## Entity: SpikeOutcomeRecord

One document: `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`.

| Field | Description | Rule |
|---|---|---|
| `verdict` | The outcome | MUST be exactly one of `"D2 confirmed"` or `"fallback triggered"` (FR-007, SC-004) |
| `run_command` | The `dotnet run` invocation | MUST be recorded for reproduction |
| `run_output` | The target's success/failure output | MUST show the target ran *from the library* (FR-006) |
| `fcs_check` | FCS-absence verification | MUST record the `dotnet list package --include-transitive` result (FR-012) |
| `blocker` | If fallback: the named blocker | MUST be reproducible (error text, package, command) when verdict = fallback |
| `fallback_path` | If fallback: the Stage 5 path | MUST document the thin-`build.fsx` shim as the path forward when verdict = fallback |

**Validation**: no ambiguous outcome is permitted — neither-confirmed-nor-blocked is a failure (spec Edge Cases, SC-004).

---

## Entity: MetaProcessRecord

Recorded in `plan.md` §Programme Meta-Process (single discoverable place), linked from the baseline.

| Field | Description | Rule |
|---|---|---|
| `default_tier` | Tier foundations features run under | MUST state lightweight framework-author loop, with governance/consumer-contract features escalating (FR-008) |
| `dogfood_features` | The named full-pipeline set | MUST name Stage 1 and Stage 4 (FR-008, SC-007) |

---

## Entity: NewProjectPair (build-tooling)

The two compiled projects the spike stands up. Not a document — a code artifact with contract obligations.

| Field | Description | Rule |
|---|---|---|
| `governance_library` | `build/Governance/FS.Skia.UI.Build.fsproj` | `net10.0`, inherits `Directory.Build.props`; one public module with curated `.fsi` (Principle II); compiles zero-warning (SC-003) |
| `build_front_end` | `build/Build.fsproj` (Exe) | References the library; registers ≥1 target via `Fake.Core.Target`; runs via `dotnet run`; compiles zero-warning |
| `spike_target` | The one trivial target | Body lives in the library, no inline duplication in the front-end (FR-006) |
| `central_packages` | New `Fake.Core.*` versions | MUST be in `Directory.Packages.props` only (FR-012); **no** FSharp.Compiler.Service |

---

## Standing Invariants (preserved by every part of this feature)

These are not produced entities but the gate the whole feature must not regress (FR-009, FR-010, SC-006):

1. **Runtime untouched** — no edits under `src/Scene`, `src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput`, `src/Layout`, `src/Controls`, `src/Controls.Elmish`, `src/Lib`.
2. **Surface unchanged** — `PackageSurfaceCheck` + `FsiTranscripts` show no baseline diff.
3. **Existing targets unchanged** — `Dev`, `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit` keep identical behaviour/output.
4. **net10 conventions** — new projects inherit `Directory.Build.props`; no `PackageVersion` outside `Directory.Packages.props`; no FCS.
5. **FAKE sequencing** — FAKE-backed validation runs in the canonical serialized order, never concurrently.
6. **Additive solution** — adding the two projects to `FS-Skia-UI.sln` changes no existing target's output.
