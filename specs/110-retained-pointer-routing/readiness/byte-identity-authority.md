# At-Rest Byte-Identity Authority (FR-011 / SC-008)

**Purpose**: Records, as a visible decision, *which* evidence proves the at-rest
**rendered output** and **control geometry** byte-identity clause of FR-011 /
SC-008 — the part of byte-identity that is not a dispatch outcome.

## Decomposition of FR-011 / SC-008

FR-011 / SC-008 require four things to stay byte-identical to the pre-feature
state. They are proven by distinct, already-planned evidence:

| Clause | Authority | Task |
|--------|-----------|------|
| Dispatched **message list** parity | Retained-route vs. preserved full-render oracle, structural equality | T017, T018 |
| **Focus outcome** parity | Click-moves-focus parity case | T020 |
| **Dispatch counts** (routing never inflates `FullRenderCount`) | Metrics honesty + corpus goldens | T010, T015, T024 |
| **At-rest rendered output + control geometry** | Standing Scene-parity golden suite (features 091/092/096–103) run under `Dev` | T027 (gate), confirmed in T024 |

## Decision

This feature is a **hot-path routing mechanism change only**; it does not touch
the retained render step, layout evaluation, or any paint path. The at-rest
rendered scene and per-control geometry are therefore unchanged **by
construction**, and the **existing Scene-parity / golden test suite** (run as part
of `./fake.sh build -t Dev`, T027) is the standing authority for that clause — no
new render-equality assertion is introduced for feature 110.

T024 explicitly confirms there is **zero** rendered-scene/geometry golden delta
against the pre-feature state when the corpus goldens are regenerated; any
unexpected scene/geometry golden movement during T027/`Dev` is a **blocking
regression**, not an accepted change.

## Why no dedicated new test

Adding a feature-110-specific render-equality test would duplicate the standing
Scene-parity suite without adding coverage: the mechanism change cannot reach the
render path, and the existing goldens already fail loudly on any pixel/box delta.
The honest record of that reasoning lives here so the SC-008 render/geometry
clause is an explicit, audited decision rather than a silent gap.
