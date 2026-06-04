# Contract: Cross-Artifact Symbol Consistency (FR-008)

**Surface type:** a compiled, deterministic helper (SkillSupport `Parsing` family
or a focused governance helper) + a `speckit-analyze` detection pass. Delivered as
diagnostics/guidance, **not** a hard merge gate (D8/D12). Unit-tested.

## C1 — Inputs

`plan.md`, `data-model.md`, `tasks.md` for the current feature.

## C2 — Extraction (deterministic)

Extract named symbols by kind:

| Kind | Pattern (informative) |
|---|---|
| `msg-case` | `Msg` DU cases referenced across artifacts |
| `union-or-screen-variant` | union / `Screen` variant names |
| `entity-record` | entity record type names |
| `fr-id` | `FR-\d+` |
| `sc-id` | `SC-\d+` |

## C3 — Output (set-difference)

For each kind, report symbols whose presence set is a **proper subset** of the
artifacts where that kind is expected — e.g. a `Msg` case in `data-model.md` +
`tasks.md` but missing from `plan.md`, or an `Initial` start-state in design but
absent from a spec FR.

```
## Symbol consistency (analyze pass G)
- msg-case ViewerKeyEventReceived — in {data-model, tasks}, missing from {plan}
- start-state Initial — in {data-model}, no matching spec FR   [design-only? human judgment]
```

## C4 — Judgment, not hard-fail

Intentionally design-only symbols (present in design before a spec FR exists) are
reported for **human judgment**, never hard-failed (spec edge case). The
`speckit-analyze` skill gains **detection pass G** that runs the helper and folds
its findings into the analysis report at appropriate severity.

## Acceptance (SC-005)

Seed a deliberate drift (a `Msg` case in `data-model.md`/`tasks.md` but not
`plan.md`); the analyze cross-check reports the set-difference mechanically.
