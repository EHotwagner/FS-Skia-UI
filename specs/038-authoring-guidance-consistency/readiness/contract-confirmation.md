# Contract Confirmation (T006)

Each of the six contract files names the exact rule, the failing-first fixture,
and the FR/SC it satisfies. Confirmed:

| Contract file | Exact rule | Failing-first fixture | FR / SC |
|---|---|---|---|
| `skill-resolution-contract.md` | every advertised id == some declared `name:`; dir/`name:`/id triple agreement; `.agents`↔`.claude` peer sync | `readiness/skill-resolution-fixtures/` dangling id + dir/`name:` mismatch + peer drift | FR-001/002/003 · SC-007 |
| `generated-api-reference-contract.md` | generated `docs/api-surface/` holds real `.fsi` verbatim per profile from `capabilities.yml contracts:`; missing/drift fails | failing-first generated-project expectation that `docs/api-surface/` present + in lockstep | FR-004 · SC-002 |
| `name-collision-hardening-contract.md` | `[<RequireQualifiedAccess>]` on `ViewerWindowStartupState` (+ consistent qualification of `update`/`init` surfaces) | `readiness/fsi/` consumer defining own `Normal`/`update`/`init`: FAIL before / PASS after | FR-008 · SC-003 |
| `generated-guidance-contract.md` | zero demo ids; ≥1 consumer-runnable snippet/skill; no framework-only paths | failing-first generated-project scan for `tetris`/`score`/`level`/`board`/`piece`, snippet, framework-only paths | FR-005/006/007 · SC-004 |
| `effects-boundary-contract.md` | one `docs/effects-boundary.md`: both categories + boundary + `update`→host wiring | failing-first generated-project expectation page absent before authoring | FR-009 · SC-005 |
| `scene-constructor-contract.md` | additive self-describing `Rectangle`/`PaintedRectangle`/`Text` constructors; no removals | `readiness/fsi/` fixture: existing positional + new self-describing forms compile | FR-010 · SC-006 |

All six are present under `contracts/` and each states rule + fixture + FR/SC.
