// Graph.fsi — DAG cycle detection, Kahn topological order, and the pure
// synthetic-evidence propagation rule (feature 043, FR-004/FR-005; Principle II).
//
// Build-tooling only. Pure — faithfully ports detect_cycles / topo_sort /
// propagate from compute-task-graph.py. The cycle list and effective statuses
// are typed (FR-014), not "ok"/"failed" strings.
namespace FS.Skia.UI.Build.Evidence

/// Propagation result for a task. `AutoSynthetic` is the computed `[S*]`.
type EffectiveStatus =
    | DeclaredEff of DeclaredStatus
    | AutoSynthetic

/// A task plus its propagated effective status and (for `[S*]`) the sorted
/// upstream synthetic/auto ids that caused it.
type ResolvedTask =
    { Task: TaskRecord
      Effective: EffectiveStatus
      RootCause: string list }

/// `Ok` iff no errors and no cycles; else `Error` (Python verdict semantics).
type GraphVerdict =
    | Ok
    | Error

/// The dependency graph: original tasks.md order plus id-keyed records (deps are
/// each record's `ExplicitDeps @ PhaseDeps`).
type Graph =
    { Order: string list
      Nodes: Map<string, TaskRecord> }

/// The fully-computed graph result the renderer serializes. Assembled by the
/// Engine from parse/validate/propagate. `Tasks` is id-sorted.
type GraphResult =
    { FeatureName: string
      Verdict: GraphVerdict
      Errors: string list
      Warnings: string list
      Cycles: string list list
      Tasks: ResolvedTask list
      RootCause: Map<string, string list> }

module Graph =

    /// Dedupe `ExplicitDeps @ PhaseDeps` preserving first-seen order.
    val allDeps: TaskRecord -> string list

    /// Build a Graph from records, preserving tasks.md order.
    val ofTasks: TaskRecord list -> Graph

    /// The canonical effective-status string ("synthetic"/"auto-synthetic"/…).
    val effectiveString: EffectiveStatus -> string

    /// 3-colour DFS cycle detection; each cycle is a list of ids (closing id
    /// repeated, as the Python engine emits).
    val detectCycles: Graph -> string list list

    /// Kahn topological order with ascending-id tie-break. Assumes acyclic.
    val topoSort: Graph -> string list

    /// Compute effective status + root-cause map over a topological order.
    /// Returns id-sorted resolved tasks and the root-cause map (FR-005).
    val propagate: Graph -> topo: string list -> ResolvedTask list * Map<string, string list>
