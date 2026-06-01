# Phase 1 Data Model — Foundations Evidence Engine Port (Stage 4)

Typed F# domain for the in-process evidence engine. These are the entities the
parsers produce, the algorithms transform, the scans emit, and the renderer
serializes byte-for-byte against the golden fixtures. Field names below are the
*F# model*; the **rendered JSON** key names/order must match the Python output
exactly (see [contracts/](./contracts/)) and are the byte-parity surface.

## Parsed inputs

### `DeclaredStatus` (DU)
From the `tasks.md` status box. `Pending` (`[ ]`), `Done` (`[X]`), `Synthetic`
(`[S]`), `Failed` (`[F]`), `Skipped` (`[-]`), `Star` (`[*]`).
- *Validation*: an unrecognised box char is a parse error (no silent default).

### `EffectiveStatus` (DU)
Result of propagation: `Declared of DeclaredStatus` | `AutoSynthetic`.
- *Rule*: see `Graph.propagate` (FR-005). Only `Done` tasks with a
  synthetic/auto dependency (and not accepted-SEH) become `AutoSynthetic`.

### `SehMetadata` (record, optional per task)
`annotation: bool`, `approvalLabel: string option`, `designSource: string option`,
`syntheticInputClass: string option`, `expectedErrorBehavior: string option`,
`rationale: string option`, `acceptanceStatus: string option`,
`diagnostics: string list`.
- *Derived*: `accepted = declared=Synthetic ∧ annotation ∧ approvalLabel.IsSome ∧
  acceptanceStatus = Some "accepted-seh" ∧ diagnostics = []`.
- *Late-SEH*: `designSource`/`acceptanceStatus` containing implementation-time
  terms ("implementation", "readiness cleanup", "after audit") ⇒ late.

### `TaskRecord` (record)
`id: string`, `declared: DeclaredStatus`, `phase: int option`, `story: string option`,
`tier: string option`, `parallel: bool`, `title: string`,
`skillist: string list`, `skillistMirror: string list option`,
`explicitDeps: string list`, `phaseDeps: string list`, `seh: SehMetadata option`,
`skillMatchAssessments: SkillAssessment list`.
- *Source*: `TaskParser` from `tasks.md` line grammar + Synthetic-Evidence
  Inventory table; `explicitDeps`/`skillist` merged from `DepsParser`.
- *Validation*: `skillistMirror` (the `[skillist: …]` mirror on the `tasks.md`
  line) MUST equal `skillist` (the authoritative `tasks.deps.yml` value).

### `DepsModel` (record)
Parsed `tasks.deps.yml`: `map: Map<string, DepsEntry>` where `DepsEntry =
{ deps: string list; skillist: string list option }`. Supports both the legacy
bare-list form and the object `{deps, skillist}` form (FR-002).

### `SkillRegistry` (record)
`skills: Map<string, string>` (skill-id → resolved `SKILL.md` path), discovered
across `.agents/skills`, `src/*/skill`, `template/fragments/*/skill` (FR-003).
- *Validation*: each `skillist` id resolves to exactly one `SKILL.md`
  (ambiguous/missing = error).

## Graph

### `Graph` (record)
`nodes: Map<string, TaskRecord>`, `edges: Map<string, string list>` (task → deps
including auto-injected phase-checkpoint edges).

### `GraphResult` (record)
`verdict: GraphVerdict` (`Ok` | `Error`), `errors: string list`,
`warnings: string list`, `cycles: string list list`,
`tasks: ResolvedTask list` (id-sorted) where each `ResolvedTask` carries
`declared`, `effective`, `rootCause: string list` (direct synthetic/auto deps,
populated only when `effective = AutoSynthetic`), plus the rendered task fields.
- *States*: `Error` when cycles, dangling refs, self-deps, unresolved skills,
  mirror mismatch, or missing skill-loading evidence — preserving the Python
  `verdict: error` / non-zero-exit semantics (spec Edge Cases).
- *Transition*: `parse → validate-and-merge → detect-cycles → (if acyclic)
  topo-sort → propagate`.

## Audit scans (FR-006 / FR-006a / FR-010)

Each scan is a pure function over already-read inputs, returning a `ScanResult`.

### `ScanHit` (record)
`path: string`, `reason: string`, `blocking: bool`, `validationArea: string`,
plus scan-specific optional fields (`missingTerms`, `missing`, `status`,
`line`, `pattern`, `severity`, `match`). Rendered shape per scan must match the
corresponding golden `*-hits.json` byte-for-byte.

### `ScanResult` (record)
`area: string`, `hits: ScanHit list`, `blockingCount: int`. One per scan:
`readiness-contract` → `readiness-contract-hits.json`; `persistent-launch` →
`persistent-launch-hits.json`; `persistent-gui-runtime` →
`persistent-gui-runtime-hits.json`; `window-visibility` →
`window-visibility-hits.json`; `audit-status` → `audit-status-hits.json`;
`diff-scan` → `diff-scan-hits.json` (`{base_ref, blocking[], advisory[]}`).

### `StatusRegion` scan (FR-006)
First-region-wins extraction of a ```audit-status fenced region; key=value lines
(case-insensitive keys); **duplicate key within a region = parse error**; prose
never interpreted. Blocking conditions: `taskbar-only=true`; `taskbar-entry=true ∧
window-visible=false`; `exact-package-match ∉ {true,yes}`;
`package-resolution=nu1603`.

### `DiffScan` (FR-010)
Inputs: a supplied unified `git diff` (read at the edge) + `audit-patterns.yml`
(patterns + whitelist + severity_overrides, read via `YamlDotNet`). Per pattern
regex over added lines; whitelist (`file_glob` + `line_regex`) suppresses;
`block` severity hits → blocking, `advisory` → reported-not-blocking.

## Audit verdict (FR-006 / FR-008)

### `SehSummary` (record)
`acceptedSehTasks: int`, `unacceptedSyntheticTasks: int`,
`autoSyntheticTasks: int`, `lateSehTasks: int`, `diagnostics: Diagnostic list`.
Rendered to `seh-audit-summary.json`; the four counts are the **audit count
block** vocabulary (FR-008, Invariant 6) and `audit-counts.txt` oracle fields
(plus `real-tasks`).

### `AuditResult` (record)
`verdict: AuditVerdict` (`Pass` | `Fail` | `Blocked`), `sehSummary: SehSummary`,
`scans: ScanResult list`, `totalBlockers: int`.
- *Rule*: `totalBlockers = unacceptedSynthetic + invalidSeh + diffScanBlocking +
  readinessContract + persistentLaunch + persistentGuiRuntime + windowVisibility +
  auditStatus`. `Pass` ⇔ `totalBlockers = 0`; else `Fail` (exit 2). An
  `--accept-synthetic` override logs to `synthetic-evidence.json` but does **not**
  change the verdict/exit (human decision recorded, not a silenced gate — Principle V).

## Rendering (FR-007)

`Render` serializes `GraphResult` → `task-graph.json` (schema_version 1.0,
id-sorted), `task-graph.md` (verdict block, skill-assessment table, status
counts, SEH classification table, Mermaid graph, ASCII tree, propagation
report), Mermaid, ASCII tree; and `SehSummary` → the audit count block. Every
output is **byte-compatible** with the Python schema (the golden fixtures are
the assertion). Determinism (sorted keys, fixed indentation, exact separators,
trailing newline) is mandatory.

## Findings

All validators return the existing uniform `Findings.ValidationFinding`
(`finding`/`renderDetail`) so the engine surfaces structured, actionable
diagnostics (Principle VII) rather than ad-hoc strings.
