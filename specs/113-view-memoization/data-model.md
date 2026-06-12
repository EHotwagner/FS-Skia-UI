# Phase 1 Data Model: View Memoization and Stable Dependency Contracts

Internal/public types and the values threaded through the retained render path. Exact
field names are finalized during `.fsi` sketching (Constitution Principle I); the shapes
below are the contract.

## Memoization seam (internal, `FS.Skia.UI.Controls`)

### `MemoEntry` (internal)
One cached memoized result for a control identity. **Storage representation (pinned):**
- `Dependency: obj` — the deterministic dependency value the site supplied last frame (the
  reuse key), **boxed** so a single uniform `MemoCache` can hold entries from
  heterogeneous sites. Reuse is decided by `=` on the boxed value, which dispatches to F#
  **structural** equality for the underlying value — never object identity (FR-005). The
  box is the comparison subject only; it is never compared by reference.
- `Subtree: Scene list` — the previously-lowered subtree (the lowered `Scene` fragment).
  `Scene list` is a **reference type**, so a hit returns the **same list instance** stored
  last frame (FR-004 "reference-equal where the seam guarantees reuse", contract C1). The
  stored subtree type is **specialized to `Scene list` this rung** because the only
  memoized site (the DataGrid row/column projection) lowers to `Scene list`; widening the
  stored type to other lowered results travels with the deferred `Style.resolve` site
  (see below).

### `MemoCache` (internal)
- `Map<ControlId, MemoEntry>` — the per-frame memo store, keyed by the control's stable
  `ControlId` (R1). Carried frame-to-frame in the retained structure; an absent key is a
  cold miss.

### `MemoOutcome` (internal)
- `Hit` | `Miss` — what a single `memoize` call resolved to. Aggregated per frame into
  the two metric counts.

### The seam (`val internal memoize` — exact signature finalized in `.fsi`)
The dependency value is boxed to `obj` at the seam boundary (see `MemoEntry` above) and the
stored/returned subtree is `Scene list` this rung, so the seam is **specialized to
`Scene list` subtrees** for the DataGrid site (generalizing the stored subtree type travels
with the deferred `Style.resolve` site). Conceptually: given a `ControlId`, a dependency
value, a thunk that computes the subtree, and the prior `MemoCache`, return
`(subtree, nextCache, outcome)`:
- **Hit**: the prior entry exists for that `ControlId` **and** its `Dependency` compares
  **equal** to the supplied dependency → return the stored `Subtree` **without running the
  thunk**; outcome `Hit`.
- **Miss**: no prior entry, or the dependency compares unequal → **run the thunk**, store
  `{ Dependency = dep; Subtree = result }` under the `ControlId`; outcome `Miss`.
- **Never** reuses across an unequal/unknown dependency (FR-001/FR-005).

### Always-miss switch (FR-008)
An internal flag (or a disabled-mode variant) that forces every `memoize` call to the
**Miss** path (thunk always runs, nothing reused). The parity oracle for FR-006/FR-007.
Not a public consumer toggle.

## Representative memoized site (control-internal)

- **DataGrid row/column projection** (`Control.fs` `gridGeom` / the `cells → Scene`
  projection): wrapped in `memoize`, keyed by the DataGrid's `ControlId`, dependency =
  the deterministic value capturing the projection's real inputs (cell/column data +
  theme/geometry). On a steady-state frame (unchanged data + theme) → **Hit**, the prior
  projected subtree is reused. Rendered output stays byte-identical (FR-003/FR-014).
- `Style.resolve` — **explicitly deferred** this rung. The seam is general enough to wrap
  it later, but `Style.resolve` lowers to a `ResolvedStyle` (not a `Scene list`), so wiring
  it requires widening the stored `MemoEntry.Subtree` type beyond `Scene list`; that
  widening + the second site are deferred to a later rung. No task memoizes `Style.resolve`
  in feature 113 (the DataGrid projection is the sole, load-bearing representative site that
  satisfies FR-003).

## Frame-work-record threading

The retained step aggregates the frame's memo outcomes:
- `MemoHits` / `MemoMisses` (ints) — added to the step's result record (alongside
  `WorkReductionRecord` / `RemeasuredNodeCount`). Sum over all memoized sites evaluated
  while building that frame. Both `0` on a frame that evaluates no memoizable control.

## `FrameMetrics` additions (public, `FS.Skia.UI.Controls.Elmish`)

Two new public fields on the existing `FrameMetrics` record (breaking `.fsi` change):
- `MemoHitCount: int` — memo hits while building this frame (a memoized site whose
  dependency was unchanged and whose subtree was reused). `0` on an idle / no-memoizable
  frame.
- `MemoMissCount: int` — memo misses while building this frame (a memoized site whose
  dependency changed, or a cold first frame). `0` on an idle / no-memoizable frame.

Both are deterministic, reproduced by `Perf.runScript`, and **golden-asserted** in the
corpus. The `zero` record carries both `0`; every per-frame construction site
(pointer-move, tick, key, idle, model) sets them from the last retained-step record.

## Stability-diagnostic report (public, `FS.Skia.UI.Controls` `Diagnostics`)

A pure report `val` (exact name/signature finalized in `Diagnostics.fsi`):
- **Input**: two builds of the same logical control (sub)tree (`Control<'msg>` ×
  `Control<'msg>`).
- **Output**: `ControlDiagnostic list` — one finding per attribute/event that compared
  **unequal** across the two builds despite no semantic change (an always-new
  `UntypedValue`, a per-frame event closure, a rebuilt list, an unstable key), naming the
  control (`ControlId` + `ControlKind`) and the offending input. Empty list ⇒ the tree's
  inputs are stable across builds.
- Reuses the existing `ControlDiagnostic` vocabulary (a new `ControlDiagnosticCode` for
  the instability class if needed). **Report-only**, asserted in `Controls.Tests`; NOT an
  enforced gate (FR-012).

## State transitions / invariants

- **Hit ⇒ no recompute**: a hit returns the stored subtree without invoking the thunk
  (FR-004).
- **Miss ⇒ recompute + store**: a miss runs the thunk and overwrites the entry for that
  `ControlId` (FR-004).
- **memo-on ≡ memo-off**: for every frame, the scene built with memoization equals the
  scene built always-miss (FR-006) and equals the pre-feature baseline (SC-002).
- **No staleness**: a real input change changes the dependency value ⇒ a miss ⇒ a fresh
  subtree; a too-coarse dependency is caught by the memo-on/memo-off parity test, never
  shipped (FR-007).
- **Idle ⇒ 0/0**: a frame that evaluates no memoizable control reports both counts `0`
  (FR-009).
