# Category honoring — FR-004 evidence (feature 101, R7, T010)

authoritative-command=dotnet run --project tests/Controls.Tests -c Debug -- --filter-test-list "Feature101"
artifact-path=tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs
status=pass
failure-class=category-channel-regression
next-action=if RED, restore the AttrCategory.Layout / geometry-name independent dirty channels in layoutDirtySet

## The independent category channel (FR-004; FR-003 ↔ FR-004 resolution)

`RetainedRender.layoutDirtySet` dirties a node when its `Update` sets/removes an
`AttrCategory.Layout`-tagged attribute, **independent of** the geometry NAME set
(`layoutAffectingAttrNames`). R7 *pins* this forward-compatibility (it does not change it) so a future
categorised attribute needs no name-set edit. The units assert this through the **EXPOSED**
`RetainedRender.step` path — its `WorkReductionRecord.RemeasuredNodeCount` is the post-propagation count
of nodes the REAL `layoutDirtySet` dirtied — rather than calling the internal `layoutDirtySet` directly.

decision=assert-through-exposed-step
reason=Calling `layoutDirtySet` directly would require adding `val internal layoutDirtySet` to `src/Controls/RetainedRender.fsi`; Route confirmed that even an internal `.fsi` edit there triggers PerPackageSurfaceDiff/per-package-baseline churn. Asserting through the exposed `step` keeps the `.fsi` byte-identical (Tier-2, zero surface delta, SC-005 / T015) while exercising the same real classifier end-to-end (more faithful to the live path than a direct internal call). The contract C3 intent — "assert the category channel is honored by the real classifier" — is fully met.

## Asserted scenarios (all GREEN)

| Scenario | Mechanism | Expectation | Result |
|---|---|---|---|
| `AttrSet` `{Name="elevation"; Category=Layout}`, `"elevation"` ∉ name set | `step` prev→next, value 1→2 | `RemeasuredNodeCount > 0` (category channel dirties) | pass |
| `AttrRemoved "elevation"` where prev carried it as `Category=Layout` | `step` prev→next, attr removed | `RemeasuredNodeCount > 0` (category recovered from prev, FR-004b) | pass |
| `AttrSet` `{Name="background"; Category=Style}` only | `step` prev→next, value 1→2 | `RemeasuredNodeCount = 0` (content/style change does not re-measure, SC-004) | pass |
| name-set gate run with a category-only attr present | probe `nameDrivesLayout`/`discoverLayoutDrivingNames` on `"elevation"` | `"elevation"` is NOT name-driving and NOT discovered → the gate does not demand it appear in `layoutAffectingAttrNames` | pass |

The last row is the FR-003 ↔ FR-004 **independence**: `"elevation"` dirties via the category channel
yet is not a name the probe discovers, and the name-set equality gate operates on names only — so the
two channels are independent and the gate never demands a category-only name appear in the literal.

`"elevation"` is asserted absent from `ControlInternals.layoutAffectingAttrNames` so the category-only
case is genuine. Full suite: `12 passed, 0 failed`.
