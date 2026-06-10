# SC-004 — purity / determinism + fixed precedence (FsCheck property)

`tests/Controls.Tests/Feature093StylePropertyTests.fs` — three FsCheck
properties, each over **≥1000** generated `(theme, base, classes, state)`
combinations (`Config.QuickThrowOnFailure.WithMaxTest 1000`):

1. **purity / determinism** — `resolve t b cs s = resolve t b cs s` for every
   generated input (no clock/randomness/Map-iteration dependence).
2. **fixed precedence (state outermost)** —
   `resolve t b cs s = resolve t (resolve t b cs Normal) [] s`: the state layer
   always applies on top of the class-resolved style, so a state's owned field
   wins over any class (`base < classes-in-order < state`, last-writer-wins).
3. **base identity** — `resolve t b [] Normal = b` for every generated base.

Generators cover both themes, all 6 variants, free-form `Custom` names (known +
unknown + empty), 0–4-long class lists, and all 8 `VisualState` cases (including
the three `Validation` severities).

## Result (non-authoritative until re-confirmed sequentially)

PASS — `dotnet test --filter "Feature 093"` ran all three properties green
(3 × 1000+ cases). Recorded here as a focused result; the aggregate `Dev` run is
the authoritative confirmation.
