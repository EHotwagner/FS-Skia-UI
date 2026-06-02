# Contract: Versioned Generated-Product Contract

**Feature**: 046 | **FRs**: FR-004, FR-005, FR-006 | **Host gate**: `GeneratedProductCheck`
(`GeneratedProduct.runScanV3GeneratedProducts` consults a new
`build/Governance/GeneratedProductContract.fs`). Implements **ADR 0003**.

## Schema version (FR-004)

The contract carries an explicit `ContractSchemaVersion { Major; Minor }`, decoupled from
the package marketing `Version` (ADR 0003). It is **discoverable in `GeneratedProductCheck`
output** (rendered header, e.g. `schema_version: 1.0`) — SC-003.

## Rule lifecycle + deprecation window (FR-005)

Each structural rule carries `RuleLifecycle = Required | Deprecated of removalVersion |
Removed`. For a generated product that violates **only** rule `r`:

| `r.Lifecycle` | Schema vs removalVersion | Outcome |
|---------------|--------------------------|---------|
| `Required` | — | **FAIL** |
| `Deprecated removalVersion` | `current < removalVersion` | **WARN**, naming `removalVersion` (passes) |
| `Deprecated removalVersion` | `current >= removalVersion` | **FAIL** (window closed — edge case) |
| `Removed` | — | rule not evaluated |

A rule must stay `Deprecated` for **at least one** schema version before becoming `Required`
or `Removed` (deprecation window). Additive-by-default: prefer deprecate-then-remove over a
hard break (ADR 0003).

## Typed changelog (FR-006)

`ContractChangelogEntry` records each transition (`Added | Deprecated | PromotedToRequired |
RuleRemoved`) with the version, rule id, and note. The changelog is **typed data embedded
in the contract module** — no sidecar file (clarification). It is surfaced in
`GeneratedProductCheck` output. `SchemaVersion` MUST be bumped (and a changelog entry added)
when a breaking structural rule changes.

## Behaviour preservation

A **current** generated project (built against the current schema version) still validates
green — the consumer contract is intact (SC-003). The existing ~800 lines of structural
checks are not rewritten; they are wrapped with lifecycle state.

## Acceptance (SC-002, SC-003)

`warn → promote → fail`: mark a rule `Deprecated` → a product violating only it passes with
a warning naming the removal version; bump `SchemaVersion` and promote the rule to
`Required` → the same product fails; the changelog records both. Typed unit tests in
`tests/Governance.Tests/GeneratedProductContractTests.fs` assert the deprecation-window
transitions and changelog entries on typed values (FR-013); a live `GeneratedProductCheck`
on a real generated project confirms green + the discoverable `schema_version`.
