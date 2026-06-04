# Contract: `SymbolCrossCheck` FAKE target (FR-003)

## Command

```
./fake.sh build -t SymbolCrossCheck
```

Takes **no file arguments**. Resolves the active feature directory from the
`BuildModel` (the same resolution `DependencyReport` and the evidence engine use),
reads `plan.md`, `data-model.md`, and `tasks.md` from it, and:

1. computes `SymbolCrossCheck.diff plan dataModel tasks : Symbol list`
   (existing, `build/Governance/SymbolCrossCheck.fs`),
2. renders `SymbolCrossCheck.render findings : string` (existing — already emits the
   `## Symbol consistency (analyze pass G)` header),
3. **prints** the markdown to the console, and **writes** it to
   `readiness/symbol-cross-check.md`.

A read-only `/speckit-analyze` pass G runs this command instead of hand-deriving a
set-difference.

## Output shape

```
## Symbol consistency (analyze pass G)

<one line per symbol present in a proper subset of {plan, data-model, tasks},
 naming the symbol, its kind, and which artifacts contain / omit it>
```

When there is no drift, the section is well-formed and empty-bodied (parity with the
"no findings" path) — never absent.

## Wiring (files touched)

| File | Change |
|---|---|
| `build/Governance/Targets.fs` | `SymbolCrossCheck` in `Target` DU, `allTargets`, `name`, `directPrerequisites` |
| `build/Governance/AgentValidation.fs` | `"SymbolCrossCheck"` in `ValidationContract.knownGates` |
| `build/Governance/Engine/Model.fs` | `SymbolCrossCheckAnalyze` `BuildEffect` |
| `build/Governance/Engine/Update.fs` | `StartTarget Targets.SymbolCrossCheck` → effect + `RequireFiles` |
| `build/Governance/Engine/Interpret.fs` | interpret `SymbolCrossCheckAnalyze`: read feature artifacts, render, print + write |
| `build/Governance/Front/Helpers.fs` | `focusedGateContract` case |
| `validation.contract.yml` | regenerated via `RefreshSurfaceBaselines` |
| `.agents/skills/speckit-analyze/SKILL.md` | pass G points at `./fake.sh build -t SymbolCrossCheck` |

## Verification

- Seed a drift (a `Msg` case in `data-model.md` + `tasks.md` but absent from
  `plan.md`); run the target from a read-only checkout; confirm the proper-subset
  finding prints in the documented format and `readiness/symbol-cross-check.md` is
  written — **no throwaway harness** (SC-003).
- `./fake.sh build -t TargetMetadataDrift` stays green (contract regenerated);
  `Governance.Tests` has no unknown-gate diagnostic (knownGates updated).

## Out of scope

- **Not** a hard merge gate — never added to a routing rule's `RequiredGates`;
  design-only symbols are reported for human judgment.
