# Contract — `FS.Skia.UI.Build.Evidence` public surface

The engine's consumer contract is its curated `.fsi` surface (Principle II) plus
the **byte-parity output schema** (separate file). This is **governance / build-tooling**
surface — it is **not** part of the product's public `.fsi` contract and does not
change any product surface baseline (Invariant 1). Consumers: the repo `build.fsx`
interpreter edge and generated `template/base/build.fsx` (via the packaged engine, R1/R8).

## Module surfaces (sketch — refined in FSI before `.fs` bodies, Principle I)

```fsharp
namespace FS.Skia.UI.Build.Evidence

/// Parsed tasks.md (FR-001). Pure over the file's text.
module TaskParser =
    val parse : tasksMd: string -> Result<TaskRecord list, string list>

/// Parsed tasks.deps.yml, both forms (FR-002). YamlDotNet behind the typed model.
module DepsParser =
    val parse : depsYml: string -> Result<DepsModel, string list>

/// Skill registry discovery (FR-003). Roots passed in (I/O at edge).
module SkillRegistry =
    val build : roots: string list -> SkillRegistry

/// Graph algorithms (FR-004/005). Pure.
module Graph =
    val detectCycles : Graph -> string list list
    val topoSort     : Graph -> Result<string list, string list list>   // Error carries cycles
    val propagate    : Graph -> ResolvedTask list                        // deterministic, id-sorted

/// audit-status region scan (FR-006). Pure over file text.
module StatusRegion =
    val scan : files: (string * string) list -> ScanResult               // (path, contents)

/// readiness-contract / persistent-launch / persistent-gui / window-visibility (FR-006a). Pure.
module Scans =
    val readinessContract  : readinessFiles: (string * string) list -> ScanResult
    val persistentLaunch   : ScanInput -> ScanResult
    val persistentGui      : ScanInput -> ScanResult
    val windowVisibility   : ScanInput -> ScanResult

/// diff-scan (FR-010). Pure over a supplied diff + patterns; git invoked at edge.
module DiffScan =
    val scan : patternsYml: string -> unifiedDiff: string -> ScanResult

/// Cross-file consistency, SEH summary, verdict (FR-006/008). Pure.
module Audit =
    val sehSummary : ResolvedTask list -> SehSummary
    val verdict    : SehSummary -> ScanResult list -> AuditResult

/// Byte-parity renderers (FR-007). Pure string producers.
module Render =
    val taskGraphJson : GraphResult -> string
    val taskGraphMd   : GraphResult -> string
    val auditCounts   : SehSummary  -> string

/// Orchestration entry points (Engine reads are supplied as data; I/O at the edge).
module Engine =
    /// Inputs already read by the interpreter; returns typed result + the artifact texts to write.
    val runGraph : EvidenceInputs -> GraphResult * GraphArtifacts
    val runAudit : EvidenceInputs -> AuditResult * AuditArtifacts
```

## Behavioural contract (parity, FR-007/008/012)

- `Render.*` output MUST be **byte-identical** to the Python engine for identical
  inputs — proven by the golden-fixture diff before any Python is deleted.
- `Graph.topoSort` tie-breaks by ascending task id (matches Python Kahn ordering).
- `Graph.propagate` applies the FR-005 rule including the `accepted-seh` exclusion.
- `Audit.verdict` ⇒ `Pass` iff `totalBlockers = 0`; `--accept-synthetic` never
  changes the verdict (Principle V).
- A graph that fails to compute returns `verdict = Error` (non-zero-exit
  semantics preserved at the interpreter edge).
- No function performs filesystem, `git`, or process I/O; all reads/writes are at
  the `build.fsx` `interpret` boundary (Principle IV). No `FSharp.Compiler.*`
  (FR-016/SC-004), no bespoke YAML parser (FR-002).

## Effect-boundary contract (`build.fsx`)

```fsharp
// New BuildEffect cases (alongside CapabilityCatalogCheck / RouteSelect)
type BuildEffect =
    | EvidenceGraphCheck of EvidenceInputs * outputPaths
    | EvidenceAuditCheck of EvidenceInputs * outputPaths
    // …existing cases…

// update (pure): StartTarget EvidenceGraph/EvidenceAudit emit the above (no RunProcess to run-audit.sh)
// interpret (edge): read tasks.md/deps/readiness/git-diff → Engine.runGraph/runAudit → write artifacts
```
