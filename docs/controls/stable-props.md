# Stable props — keeping memoized reuse working

Feature 113 (Phase 5) added a control-internal **memoization seam**: when an expensive pure
projection's *declared inputs* are unchanged frame-to-frame, the previously-lowered subtree is
**reused** instead of recomputed (the same idea as React `memo`/`useMemo`, Compose `remember`, and
SwiftUI dependency-local bodies). The representative memoized site this rung is the **DataGrid
row/column projection**.

Reuse is decided by **structural equality (`=`)** on a deterministic *dependency value* (the theme,
the evaluated box, and the projection's cells). When the dependency compares equal to the prior
frame's, the seam returns a **hit** (no recompute); when it differs — or on a cold first frame — it is
a **miss** (recompute + store). The two public `FrameMetrics` counters `MemoHitCount` /
`MemoMissCount` make this observable and golden-asserted.

This page is the author-facing companion to the **stability diagnostic**
(`Diagnostics.stabilityReport`, report-only): it names the concrete **reuse-breaking patterns** — the
*always-new* inputs that compare unequal across two builds of the same model despite no semantic
change — and how to make each input stable.

## Why an "always-new" input defeats reuse

The seam can only reuse a subtree when this frame's dependency is **equal** to last frame's. An input
that is reconstructed every frame into a value that does **not** compare equal forces a miss every
frame, so the projection is recomputed every frame and the memo buys nothing (and `MemoMissCount`
stays high while `MemoHitCount` stays `0` — your visible regression signal).

> The seam **never** reuses across an unequal or unknown dependency. A too-coarse dependency is caught
> by the memo-on/memo-off parity test, never shipped as a stale frame — so an always-new input is a
> *performance* problem, never a *correctness* one.

## Reuse-breaking patterns and their fixes

### 1. A rebuilt `UntypedValue` (an always-new boxed value)

A value attribute reconstructed into a fresh object each frame (e.g. `UntypedValue (makeRecord ())`
where `makeRecord` allocates a value with **reference** equality) compares unequal every frame.

- **Fix:** carry a value with **structural** equality (an F# record, tuple, list, DU, or primitive),
  or hoist the construction so the same instance is reused across frames. Structurally-equal values
  compare equal even when reconstructed, so a rebuilt-but-equal record is fine.

### 2. A per-frame event closure (a fresh lambda each build)

A handler written inline as `fun e -> ...` is a **new function value every frame**. Functions have no
structural equality, and F# re-wraps a function in a fresh closure adapter at each use site, so an
event value is effectively **always reference-fresh**. The stability diagnostic reports every per-frame
event closure as an instability — which is why the diagnostic is **advisory, not a build gate**:
consumers legitimately use event closures.

- **Fix (when it matters for a memoized subtree):** keep the handler **out of the memoized
  dependency**. The DataGrid projection's dependency is its *cells/theme/box*, not its event handlers,
  so ordinary `onClick`/`onChange` closures do **not** defeat its reuse. Only include in a dependency
  the inputs that actually change the *rendered output*.

### 3. A rebuilt list that is reference-unequal

A list reconstructed each frame (`[ for x in xs -> ... ]`) is a new instance. Under **structural**
equality this is still fine — a structurally-equal rebuilt list compares **equal** and hits. It only
breaks reuse under a *reference*-equality scheme.

- **Fix:** the seam uses structural `=`, so a structurally-equal rebuilt list is already stable. Keep
  the list's *contents* deterministic (same order, same elements) and it will hit. Avoid folding
  nondeterministic data (timestamps, GUIDs, hash-set iteration order) into the list.

### 4. An unstable key

A node whose stable `Key` changes across builds of the same model re-keys the retained identity and
forces a rebuild (and, under the diagnostic, is flagged as an instability).

- **Fix:** derive `Key` from **stable domain identity** (a row id, an item id), never from a loop
  index that shifts, a freshly-generated id, or a value that changes every frame.

## Diagnosing instability

`Diagnostics.stabilityReport first second` takes **two builds of the same logical (sub)tree** (the
same model run through `View` twice) and returns one `UnstableReuseInput` finding per attribute/event
that compared unequal despite no semantic change, naming the control (`ControlId` + `ControlKind`) and
the offending input. An **empty list** means the tree's inputs are stable across builds — exactly the
case memoization can exploit. It is a **report tool asserted in tests**, not an enforced gate.

```fsharp
let a = view model
let b = view model            // same model, built twice
match Diagnostics.stabilityReport a b with
| [] -> ()                    // stable — reuse will work
| findings -> findings |> List.iter (fun f -> printfn "%s" f.Message)
```

## Scope (feature 113)

The memoized site this rung is the **DataGrid row/column projection only**; `Style.resolve` and the
remaining expensive transforms are deferred (the seam is kept general enough to wrap them later). There
is **no** public `Control.memo` / `Widget.memo` primitive this rung. The stability diagnostic is
**report-only**. See `specs/113-view-memoization/` for the full contract.
