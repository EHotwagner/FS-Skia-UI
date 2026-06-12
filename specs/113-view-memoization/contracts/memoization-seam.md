# Contract: Control-Internal Memoization Seam (internal)

**Surface**: `val internal memoize` (+ `type internal MemoEntry` / `MemoCache` /
`MemoOutcome`) in the owning `FS.Skia.UI.Controls` `.fsi`, reached by tests via
`InternalsVisibleTo "Controls.Tests"`. **Not** a public consumer `Control.memo` /
`Widget.memo` primitive (deferred, clarified 2026-06-12).

**Storage representation (pinned):** `MemoEntry.Dependency` is `obj` (a **boxed**
deterministic value; reuse decided by F# structural `=`, never object identity, C3/FR-005)
and `MemoEntry.Subtree` is `Scene list` (a reference type, so a `Hit` returns the **same
instance** — C1). The stored subtree is **specialized to `Scene list`** this rung because
the DataGrid row/column projection is the sole memoized site; the `Style.resolve` site
(which lowers to `ResolvedStyle`, requiring a wider stored type) is **deferred**.

## Behaviour

Given a stable `ControlId`, a deterministic dependency value `dep`, a thunk that computes
the subtree, and the prior `MemoCache`:

| Prior state | `dep` vs stored | Result | Outcome |
|---|---|---|---|
| entry exists for `ControlId` | **equal** | return stored subtree, **thunk NOT run** | `Hit` |
| entry exists for `ControlId` | **unequal** | run thunk, store `{dep; result}` | `Miss` |
| no entry (cold) | — | run thunk, store `{dep; result}` | `Miss` |

- **C1 (reuse)**: a `Hit` returns the **same subtree instance** stored last frame
  (reference-equal where the seam guarantees reuse) and does **not** run the thunk
  (FR-004).
- **C2 (store)**: a `Miss` runs the thunk and stores the result keyed by `ControlId` +
  `dep` for the next frame (FR-004).
- **C3 (equality is sole reuse condition)**: reuse happens **iff** the dependency value
  compares equal; the seam **never** reuses across an unequal or unknown dependency
  (FR-001/FR-005). When in doubt, it misses.
- **C4 (always-miss)**: in always-miss mode every call takes the `Miss` path with nothing
  reused — the FR-008 parity oracle.

## Parity & no-staleness obligations

- **C5 (memo-on ≡ memo-off, FR-006)**: for every corpus frame, the rendered scene built
  with the seam active is **byte-identical** to the scene built always-miss — and to the
  pre-feature baseline (SC-002).
- **C6 (no staleness, FR-007)**: when a memoized control's real inputs change, the
  dependency value changes ⇒ a `Miss` ⇒ a fresh subtree. A too-coarse dependency value is
  caught by the C5 parity test, never shipped as a stale frame.

## Count semantics (feeds `FrameMetrics`)

- **C7**: the retained step aggregates the frame's outcomes into `MemoHits` / `MemoMisses`
  (sum over all memoized sites evaluated that frame), surfaced as the public
  `FrameMetrics.MemoHitCount` / `MemoMissCount`.
- **C8 (idle, FR-009)**: a frame that evaluates no memoizable control reports both counts
  `0` (no spurious memo accounting).

## Test obligations (`tests/Controls.Tests/Feature113*`)

1. Steady-state stable dependency → `Hit`, subtree reference-reused, thunk not run
   (instrument the thunk to assert non-invocation).
2. Changed dependency → `Miss`, fresh subtree.
3. Cold first frame → `Miss`.
4. memo-on vs memo-off scene byte-identity over representative frame sequences (C5).
5. Real-input-change reflects the change (C6, no staleness).
