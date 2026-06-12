# Touched-node before/after delta (feature 112, T016 / FR-007)

The runtime visual-state stamp previously rebuilt the WHOLE tree every live frame
(`ControlRuntime.applyRuntimeVisualState` reconstructs every node). The targeted stamp rebuilds only the
controls whose final state changed (the affected identities + ancestor paths).

`RuntimeStateTouchedNodeCount` is an **INTERNAL** count (clarified 2026-06-12) — returned by the
targeted-stamp result and asserted in `Controls.Tests`; it is **NOT** a public `FrameMetrics` field (the
runtime-state stamp runs only on the live host, so a golden-asserted field would be a permanently-`0`
corpus column). The live host surfaces it best-effort at the interpreter edge (`lastRuntimeStateTouched`).

## Delta (from Feature112TouchedCountTests)

| Scenario (tree size) | BEFORE (whole-tree stamp) | AFTER (targeted stamp) |
|----------------------|---------------------------|------------------------|
| hover move A→B (21 nodes) | 21 (every node rebuilt) | **3** (two leaves + shared root) |
| focus move A→B (21 nodes) | 21 | **3** |
| persistent hover / at-rest (21 nodes) | 21 | **0** (whole tree reused) |
| hover sweep over a 51-node tree | 51 per step | **≤ 3** per step (proportional to affected, not N — SC-006) |
| full-tree oracle route (`runtimeStampFor None`) | — | node count (the regression guard — a whole-tree stamp is visible) |

## Authority

The standing Scene-parity / golden suite under `Dev` remains the authority for at-rest rendered output +
geometry byte-identity ([byte-identity-authority.md](./byte-identity-authority.md)); the targeted stamp's
scene equals the full-tree oracle's (`Feature112TargetedStampParityTests`), so no rendered output changed
— only the number of nodes rebuilt to stamp.
