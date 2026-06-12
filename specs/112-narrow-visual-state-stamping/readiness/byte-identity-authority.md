# At-Rest Byte-Identity Authority (FR-008 / SC-005)

**Purpose**: Records, as a visible decision, which evidence proves the at-rest **rendered output** and
**control geometry** byte-identity clause of FR-008 / SC-005.

## Decomposition

| Clause | Authority | Task |
|--------|-----------|------|
| The targeted stamp's `Stamped` renders byte-identically to the full-tree oracle | `Feature112TargetedStampParityTests` (`Scene` equality of `Control.renderTree`) | T012/T013 |
| Consumer-set / Disabled precedence unchanged | `Feature112PrecedenceTests` | T014 |
| At-rest rendered output + control geometry | Standing Scene-parity / golden suite (091/092/096–103) run under `Dev` | T019 (gate) |
| The whole-tree work dropped to affected-paths | `RuntimeStateTouchedNodeCount` (`Feature112TouchedCountTests`) | T008/T015 |

## Decision

This feature is a per-frame **stamp mechanism** change; it does not change the retained step, layout
evaluation, or any paint path — it narrows which nodes are rebuilt to stamp runtime visual state, and the
targeted stamp produces the **byte-identical** stamped tree the full oracle would (a reused node already
carries `finalState cur`; a rebuilt node is the fresh node with `finalState cur`; a derived `Normal`
emits no attribute). At-rest rendered scene + per-control geometry are therefore unchanged **by
construction**, and the **existing Scene-parity / golden suite** (run under `./fake.sh build -t Dev`,
T019) is the standing authority for that clause; the focused mechanism parity is `Feature112` (T012).

Any unexpected scene/geometry golden movement during `Dev` is a **blocking regression**, not an accepted
change.
