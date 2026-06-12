# Phase 0 Research: Narrow Runtime Visual-State Updates

All unknowns from the Technical Context are resolved below. Each decision names the
concrete in-repo anchor it relies on.

## R1 — The changed-identity formulation (final-state parallel walk, not a `{prev∪cur}` id set)

**Decision**: A node is re-stamped iff its **final visual state** changed between the
previous and current runtime model, where
`finalState M node = if ControlInternals.visualStateOf node.Attributes <> Normal then
that consumer-set state else ControlRuntime.deriveVisualState M (node.Key ?? node.Kind)`.
The targeted stamp is a **parallel walk** of the previous-frame stamped tree and the
current fresh (un-stamped) tree (same structure); a node whose `finalState cur =
finalState prev` and whose descendants are all unchanged is **reused** untouched, else
it is rebuilt from the fresh node with `finalState cur` stamped.

**Rationale**: The spec/report describe the affected set as "previous and current
hover/focus + pressed", but the precise, parity-correct set is "nodes whose **final**
state changed". This matters in two ways the raw id-set misses:
- A **consumer-set** control (e.g. `Disabled`) that gains/loses hover does **not**
  change its final state (the consumer state always wins, `deriveVisualState` is
  ignored) — so it must NOT be re-stamped. Computing the changed set from the final
  state (with consumer precedence) excludes it automatically (FR-003).
- A control whose hover **persists** across frames (no change) has `finalState cur =
  finalState prev` → reused → `0` touched, satisfying the no-change=`0` rule (SC-003) —
  which a "stamp every current-active id over a fresh tree" approach cannot give
  (it would re-stamp the persistent hover every frame).

**Alternatives considered**:
- *Stamp all current-active ids over a fresh tree* — rejected: byte-identical to the
  oracle (the oracle also only modifies current-active ids over the fresh tree), but
  it re-stamps persistent hover every frame, so `RuntimeStateTouchedNodeCount` never
  reaches `0` on a no-change frame (fails SC-003) and the per-frame stamp work does not
  drop for a held hover.
- *Mutate the prev-stamped tree without the fresh tree* — rejected: the prev-stamped
  node's `visualState` attribute is ambiguous (consumer-set vs derived-set), so the
  changed set and the consumer-precedence cannot be computed correctly from it alone.

## R2 — Operate on the previous **stamped** tree + the current **fresh** tree

**Decision**: The targeted stamp's two inputs are the previous frame's **stamped** tree
(`retained.Value.Root.Control` — the `next` fed to `RetainedRender.step` last frame)
and the current frame's **un-stamped** view tree (feature 111's `viewFor` output). On
the model-unchanged hot path they have **identical structure** (both are
`host.View size model` of the same `(model, size)`, a pure view), so they zip
node-for-node. A reused node returns the **previous-frame instance** (already carrying
`finalState prev = finalState cur`, so the output equals the oracle's); a rebuilt node
is the **fresh** node with `finalState cur` stamped (a clean stamp, no stale derived
attribute).

**Rationale**: parity requires the output be the fresh view tree with `finalState cur`
on every node (= the full oracle). A reused prev-stamped node already IS that (its
non-`visualState` attributes equal the fresh node's because the model is unchanged, and
its stamp equals `finalState cur` because the state didn't change). Rebuilding from the
fresh node guarantees the stamp is computed cleanly (no need to "un-stamp" a stale
derived attribute). `setVisualState` is reused for the stamp; a `finalState cur =
Normal` node is left with **no** `visualState` attribute (the fresh node has none),
matching the oracle's "emit nothing at Normal" (byte-identity at rest).

**Open confirmation for implementation**: confirm the prev-stamped node's
non-`visualState` attributes equal the fresh node's on the model-unchanged path (they
must, since both come from `host.View` of the same model). The parity test (R4) is the
proof obligation if they diverge.

## R3 — Fallback boundary (model change / first frame / misalignment)

**Decision**: The live host uses the targeted stamp **only** on the model-unchanged
path — `retained` exists AND `viewFor` returned the cached tree
(`obj.ReferenceEquals(model, cachedModel) && size = cachedSize`, feature 111). On a
**model-changing** frame or the **first** frame, the whole view tree is (re)built
anyway, so the host uses the full `applyRuntimeVisualState` oracle (a full stamp is
appropriate there — there is no prior stamped tree to narrow against, or its structure
differs). If the targeted parallel walk detects a **structural misalignment**
(child-count mismatch — not expected when the model is unchanged) it returns a "fall
back" signal and the host runs the full oracle (FR-006). The previous runtime model is
stored in a host-loop `ref` (`lastRuntimeModel`) so the next frame can compute the
changed set.

**Rationale**: keeps the targeted path on the case it is designed for (host-owned
hover/focus/press with an unchanged model), and degrades to the correct full stamp
everywhere else — the full stamp is always available and always correct, so the
fallback can never mis-render. Normal hover/focus/press frames never hit the fallback.

## R4 — Byte-identity argument (FR-008 / SC-002)

**Decision**: The targeted stamp is byte-identical to the full oracle on the
model-unchanged path because, for every node:
- **reused** (final state unchanged): the prev-frame node = `fresh-attrs + finalState
  prev` = `fresh-attrs + finalState cur` = exactly what the oracle stamps;
- **rebuilt** (final state changed): the fresh node + `setVisualState (finalState cur)`
  (or no attribute when `Normal`) = exactly what the oracle stamps.
So the produced stamped tree is structurally equal to `applyRuntimeVisualState curModel
fresh`, node-for-node.

**Proof obligation**: FR-005 parity test compares, for keyed / nested / consumer-set /
unkeyed-sibling trees and a representative hover-move / focus-move / press-toggle set,
the **rendered scene** (via `Control.renderTree`) and the **resolved per-control visual
state** produced by the targeted stamp vs. the full oracle, asserting equality (`Scene`
has structural equality; `Control` has none → compare visual states + the rendered
scene, the technique features 092/096/103 used). The no-change frame and a
consumer-set-`Disabled` precedence case are explicit cases.

**Rationale**: parity-by-construction (same `finalState cur` on every node) plus a
direct two-path oracle comparison is the same proof shape features 091/092/110 used; it
is the strongest available evidence and is independent of a live window.

## R5 — Internal, deterministically-testable count (FR-007)

**Decision**: `RuntimeStateTouchedNodeCount` is the number of nodes the targeted walk
**rebuilt** (the changed-state paths), returned in the internal `RuntimeStampResult`.
The runtime-state bridge runs only on the live host (the deterministic `Perf.runScript`
corpus stamps visual state inline via the model, not via the bridge), so the
authoritative deterministic evidence is a direct `Controls.Tests` assertion on the
targeted-stamp result — `0` for a no-change frame, a small count for a localized hover/
focus/press change, far below the node count. The live host surfaces it best-effort
(diagnostic). It is **not** a public `FrameMetrics` field (clarified 2026-06-12), so
there is no permanently-`0` golden column and no corpus-golden churn.

**Rationale**: the count is a pure function of `(prevModel, curModel, prevStamped,
fresh)`, so it is byte-stable and testable without a live window — consistent with how
features 109–111 keep the authoritative determinism surface separate from the live
best-effort sink.
