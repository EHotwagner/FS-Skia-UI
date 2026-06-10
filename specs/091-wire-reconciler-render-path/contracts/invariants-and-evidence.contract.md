# Contract: Invariants on the wired path + evidence obligations

This contract is what the **promoted 067 property suite** and the new evidence captures assert. It binds the
wired render path to the same guarantees feature 067 proved for `Reconcile` in isolation, **now on the live
path**, plus the three new capturable proofs.

## Promoted invariants (wired-path property tests, ≥1,000 generated `(prev, next)` pairs)

Source generators reused: `tests/Controls.Tests/ReconcileTests.fs` `Gen067.pair`. The assertions move from
`diff`/`apply` in isolation to `RetainedRender.step`/`init` output.

| Invariant | Wired-path assertion | Spec ref |
|-----------|----------------------|----------|
| **Round-trip** | `(RetainedRender.step theme size (RetainedRender.init theme size prev) next).Render` is byte-identical to `Control.renderTree theme size next`. | FR-006 / SC-005 |
| **Determinism** | Two independent runs of the same frame sequence produce identical `Render` + identical minted `RetainedId`s. | FR-006 / SC-005 |
| **Totality** | `step` never throws for any `(prev, next)`, including duplicate-key and empty-tree cases. | FR-006 / SC-005 |
| **Identity-at-rest** | `next` structurally equal to `prev` → `Keep` no-op: zero re-measure/re-paint, zero `RetainedId` churn, zero spurious diagnostics. | FR-006 / SC-005 |

## New capturable evidence (real, headless/offscreen)

| Proof | Mechanism | Artifact | Spec ref |
|-------|-----------|----------|----------|
| **Identity survival** | Two renders differing only outside keyed control K; assert K matches `ChildKeep`/`Update` (not `Replace`) and keeps its `RetainedId`; a `Kind`-changed control is `Replace`d. | test log | SC-001 |
| **Focus/animation survives** | Reuse the 090 before/after render-diff primitive: render → set focus / start a per-control clock → dispatch an **unrelated** model update → re-render → assert focus unchanged + clock advanced (not reset); a rebuild-every-frame baseline **fails**. | `readiness/survives-proof/{before,after}.png` + `survives-proof.txt` | SC-002 |
| **Golden-diff parity** | Wired output vs full rebuild of `next`, every test scene, zero diff. | `readiness/retained-parity/{wired,rebuild}.png` + `retained-parity.txt` | SC-004 |
| **Measured work reduction** | Localized single-leaf change: record `RecomputedNodeCount` (wired) vs `BaselineNodeCount` (== N); assert `Recomputed ≤ ChangedSubtreeBound < Baseline`. | `readiness/partial-update/work-reduction.txt` | SC-003 |
| **Diagnostics surfaced** | Duplicate-keyed sibling list on the live path → `KeyCollision` reaches the `ControlDiagnostic` channel; path stays total. | test log | SC-006 |
| **Disposition flip currency** | `.agents`↔`.claude` `fs-skia-reconciliation` byte-identity after the "wired on the render path" update. | `readiness/skill-sync-check.md` | SC-007 |
| **Route escalation green** | `./fake.sh build -t Route` prints the expected escalation; the serialized six-target order is green. | six-target logs | SC-008 |

## Honesty constraints
- Render-only / deterministic capture; no live Vulkan window required for any evidence
  ([[fs-skia-evidence-mode]]).
- No synthetic evidence planned. A deliberately-malformed duplicate-key fixture, if used to drive the
  diagnostics-surfacing test on literal input, carries full Principle-V `[S]`/`[SEH]` disclosure.
- Correctness wins: any parity/round-trip failure is a hard gate failure, not a weakened assertion (the
  wired path falls back to full-rebuild-equivalent output rather than diverging).
