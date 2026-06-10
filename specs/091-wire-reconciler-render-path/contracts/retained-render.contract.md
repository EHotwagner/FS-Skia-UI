# Contract: `module internal RetainedRender` (the wired render path)

**Status:** internal contract — `module internal`, **no public-surface entry** (mirrors `module internal
Reconcile` / `module internal SceneRenderer`; zero baseline delta, 067 SC-005). Reached by tests via
`[<assembly: InternalsVisibleTo("Controls.Tests")>]`. This is a *contract between framework internals and
the property tests*, not a consumer API.

## Entry points (internal)

```fsharp
namespace FS.Skia.UI.Controls

module internal RetainedRender =

    /// Build the initial retained structure from the first frame's lowered tree.
    /// Equivalent in output to `Control.renderTree theme size control`, plus the retained
    /// fragments + minted stable identities. Total; never throws.
    val init:
        theme: Theme -> size: FS.Skia.UI.Scene.Size -> control: Control<'msg> ->
            RetainedRender<'msg>

    /// Produce the next frame from the retained `prev` and the next lowered tree, by
    /// `Reconcile.diff`-ing and applying the patch to the retained structure. Returns the
    /// next retained structure, the render result, and any diagnostics surfaced from the diff.
    ///
    /// Guarantees (asserted by the promoted 067 suite on the WIRED path):
    ///   - totality:        never throws for any (prev, next); duplicate keys -> KeyCollision diagnostic
    ///   - determinism:     identical (prev, next) -> identical Result + identical minted RetainedIds
    ///   - identity-at-rest: next structurally equal to prev.Root.Control -> Keep no-op, no re-measure
    ///   - round-trip:      Result.Render is byte-identical to `Control.renderTree theme size next`
    val step:
        theme: Theme -> size: FS.Skia.UI.Scene.Size ->
        prev: RetainedRender<'msg> -> next: Control<'msg> ->
            RetainedRenderStep<'msg>

and RetainedRenderStep<'msg> =
    { Retained: RetainedRender<'msg>
      Render: ControlRenderResult<'msg>            // identical to a full rebuild of `next`
      Diagnostics: ControlDiagnostic list          // == (Reconcile.diff prev.Root.Control next).Diagnostics
      WorkReduction: WorkReductionRecord }          // measured re-measure/re-paint node counts (SC-003)

and WorkReductionRecord =
    { BaselineNodeCount: int                        // nodes a full rebuild would re-measure/re-paint (== N)
      RecomputedNodeCount: int                      // nodes the wired path actually recomputed
      ChangedSubtreeBound: int }                    // upper bound = size of the changed subtree
```

## Behavioral contract (binds `step`)

| # | Obligation | Spec ref |
|---|------------|----------|
| C1 | `step` MUST compute its patch via `Reconcile.diff prev.Root.Control next` and apply **that** patch (key-first-then-positional; `Kind` mismatch → `Replace`). No alternative matching rule. | FR-001 |
| C2 | `step.Render` MUST be **byte-for-byte identical** to `Control.renderTree theme size next` for every input. | FR-005 / SC-004 |
| C3 | A `ChildKeep`/`Keep`/`Update` match MUST carry the prev node's `RetainedId`; a `Replace`/`ChildRemove` MUST drop it (no false identity). | FR-003 / SC-001 |
| C4 | `step.Diagnostics` MUST equal the diff's `Diagnostics` and be surfaced through the existing `ControlDiagnostic` channel — never dropped. `step` stays total in their presence. | FR-007 / SC-006 |
| C5 | For a localized change, `RecomputedNodeCount ≤ ChangedSubtreeBound < BaselineNodeCount` (work bounded by the changed subtree, not N). | FR-004 / SC-003 |
| C6 | Minted `RetainedId`s come from `prev.NextId` (monotonic), so identical frame sequences are reproducible across runs/processes (no clock/randomness). | FR-006 / SC-005 |
| C7 | **Correctness-wins fallback:** if assembling the partial result would violate C2, `step` MUST fall back to `Control.renderTree theme size next` and rebuild the retained structure from it. | FR-005 resolution |

## Negative contract (what `step` MUST NOT do)
- MUST NOT expose any mutable view-model, data binding, or dependency/attached property (roadmap non-goals).
- MUST NOT throw on duplicate keys / empty trees (→ diagnostics).
- MUST NOT retain identity across a `Replace` (SC-001 negative case).
- MUST NOT promote `Reconcile` or `RetainedRender` to public surface (stays `module internal`).
