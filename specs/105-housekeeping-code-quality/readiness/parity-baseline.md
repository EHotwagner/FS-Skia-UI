# Parity baseline (T006) and parity guard (T007) — SC-006

`Control<'msg>` has no structural equality (its `AttrValue` DU carries a function case),
so parity is compared through `sprintf "%A"` of the order-normalized, event-canonicalized
control — the established 096/097/101 pattern.

## What is pinned

The parity guard is `tests/Controls.Tests/Feature105ParityTests.fs` (testList
"Feature 105 lowering-helper parity (SC-006)", 8 tests). It pins, for the consolidated
surfaces, the exact pre-change behaviour so it goes RED on any perturbation of:

- key application (`WidgetLowering.withKeyOpt (Some k)` == `Control.withKey k`; `None` == identity);
- the string-event adapters (`onString`/`onStringList`): bound event-kind name, payload
  pass-through, and the absent-payload defaults (`""` / `[]`);
- the shared accessibility-metadata builder (`a11y`): role + name + Enter/Space keyboard
  affordance + navigation keys (byte-identical to the inline `Accessibility.metadata` shape);
- the collapsed `onChanged` adapters (FR-003): bool `"true"/"false"/absent`, float parse of
  valid / unparseable / empty / absent (`tryParseFloat` fallback to `0.0`), string pass-through.

## Baseline capture

The baseline is the pre-change behaviour of the helpers (the verbatim originals). The guard
was authored and run **green** against the current (post-rewire) source, where every helper
body is the single-sourced verbatim original — so the captured baseline and the live result
are identical by construction. The guard would diverge only if a future edit perturbs a
consolidated body.

```
EXPECTO! 8 tests run for "Feature 105 lowering-helper parity (SC-006)" – 8 passed, 0 failed.
```

## Regression net

The pre-existing `TypedLoweringTests` (typed `view` lowers structurally equal to the legacy
`*.create`) and the full Controls + Controls.Elmish suites are the broader regression net;
they exercise every consolidated-helper widget end-to-end and stay green and unchanged.

- Controls suite: 337 passed, 0 failed.
- Controls.Elmish (Elmish.Tests) suite: 69 passed, 0 failed.
