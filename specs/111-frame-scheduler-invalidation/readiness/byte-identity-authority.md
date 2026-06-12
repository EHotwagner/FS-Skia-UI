# At-Rest Byte-Identity Authority (FR-008 / SC-007)

**Purpose**: Records, as a visible decision, which evidence proves the at-rest **rendered output** and
**control geometry** byte-identity clause of FR-008 / SC-007 — the part of byte-identity that is not a
metric value.

## Decomposition

| Clause | Authority | Task |
|--------|-----------|------|
| The view-skip reuse is byte-identical (`prev.Root.Control` == fresh `host.View` of the unchanged model; the view-skipped overlay == the view-re-run overlay) | `Feature111ViewSkipTests` (structural `%A` of the Control + `Scene` equality of the step output) | T014 |
| At-rest rendered output + control geometry | Standing Scene-parity / golden suite (features 091/092/096–103) run under `Dev` | T021 (gate), confirmed in T018 |
| No rendered-scene/geometry delta in the regenerated goldens | Corpus goldens carry only the new cause/phase fields + the view-free tick metric flip; no scene/box golden moved | T018 |

## Decision

This feature is a per-frame **scheduling/observability** change; it does not change the retained step,
layout evaluation, or any paint path — it removes a redundant `host.View` *call* on a model-unchanged
frame (FR-003) and reuses the tree `host.View` would have produced (pure view in `(model, size)`,
FR-009 keeps the full-tree stamp). The at-rest rendered scene and per-control geometry are therefore
unchanged **by construction**, and the **existing Scene-parity / golden test suite** (run as part of
`./fake.sh build -t Dev`, T021) is the standing authority for that clause — no new render-equality
assertion is introduced beyond the focused view-skip mechanism test (T014).

T018 explicitly confirms there is **zero** rendered-scene/geometry golden delta against the pre-feature
state; any unexpected scene/geometry golden movement during `Dev` is a **blocking regression**, not an
accepted change.
