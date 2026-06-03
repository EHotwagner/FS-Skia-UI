# Governance Generation Contract

This feature has **no public product API surface** (`.fsi`) change — the contract it
changes is the *internal governance contract*: which files must carry which
governed content, and how the per-file copies are generated and currency-checked.
This document is that internal contract. It is enforced inside `build/Governance/**`
and `tests/Governance.Tests/**`, not shipped to consumers.

## C1 — Canonical-source invariant (FR-002, FR-004)

- `build/Governance/Guidance.fs` remains the **single home of the rule set**:
  `ContractToken`, `GuidanceObligation`, `GuidanceCheck`, `evaluateGuidanceCheck`.
  The *set* of tokens/obligations/forbidden terms is unchanged by this feature.
- Each catalogued duplication has **exactly one** `CanonicalSource`. Every other
  occurrence is either deleted (in-repo scanner reads canonical) or a generated
  copy (splice/render).

## C2 — Generation invariant (FR-002, FR-007)

- Generated copies are produced **only** by `RefreshSurfaceBaselines` effects, via:
  - the generalized `ConstitutionFragments` splice (`BEGIN/END GENERATED: gov/<id>`
    and `constitution/<id>` markers), and
  - the placeholder-bearing constitution-principle render (Verbatim for the two
    `constitution-template.md` twins; Substituted for `.specify/memory/constitution.md`).
- Bytes outside a generated region are never rewritten. Unknown markers are left in
  place (honest drift), matching `ConstitutionFragments.splice`.
- No generated copy is hand-edited. The two constitution twins remain
  byte-identical to each other.

## C3 — Currency invariant: no silent drift hole (FR-003, FR-005, SC-005)

- **Every** generated artifact is paired with a currency gate that **fails** when
  the copy diverges from its canonical source. The pairing is enumerated in
  `readiness/silent-drift-audit.md`; no artifact may have an empty guard cell.
- Currency diagnostics name the **drifted file and its canonical source** and the
  repair command (`./fake.sh build -t RefreshSurfaceBaselines`), matching the
  existing `SkillTreeGen.currencyDrift` / `ConstitutionFragments.currencyDrift`
  diagnostic shape.
- Guards: `TargetMetadataDrift` (governed-block + constitution-twin currency),
  `SkillSyncCheck` (`.agents`→`.claude` peers), `GeneratedGuidanceCheck`
  (token/obligation presence over the regenerated corpus). No new top-level FAKE
  target unless an artifact needs isolated failure ownership.

## C4 — Drift-strength invariant (FR-005, SC-004)

The following must each still fail a gate with a file+rule diagnostic, and revert to
green after `git checkout` / regeneration (red→green proof in
`readiness/dedupe-red-green.md`):

1. Deleted obligation concept (056 case) → `…: obligation '<id>' (<source>) not reflected [<tag>]`.
2. Removed contract token (056 case) → `…: missing \`<token>\` [<tag>]`.
3. Reintroduced forbidden term (056 case) → `… contains stale term \`<token>\` [<tag>]`.
4. **New:** generated copy hand-edited out of sync with its source → currency gate
   fails naming the file and its source.

## C5 — Generated-consumer invariant (FR-010, SC-007)

- `SkillSyncCheck` and `TemplateDrift` stay green; the `.agents`↔`.claude` peers and
  template-owned files remain valid and synchronized.
- A generated `dotnet new fs-skia-ui` project receives correct, non-stale governance
  guidance (constitution + skills), demonstrated by `GeneratedProductCheck`.

## C6 — Routing invariant (FR-008)

- The change escalates on `Route` (governance paths touched).
- `Routing.fs` is expected unedited; therefore `validation.contract.yml` stays
  byte-identical (`TargetMetadataDrift` green). Any routing edit is regenerated and
  re-checked — never hand-synced.

## Verification surface (no `.fsi` change)

This contract is verified by FAKE gates, not by a public signature file:

| Invariant | Verified by |
| --- | --- |
| C1, C2 | `GeneratedGuidanceCheck`; `ConstitutionFragmentsTests`; new `GovernedBlock` tests |
| C3, C4 | `TargetMetadataDrift`; `SkillSyncCheck`; red→green readiness proof |
| C5 | `SkillSyncCheck`; `TemplateCheck`/`TemplateDrift`; `GeneratedProductCheck` |
| C6 | `Route`; `TargetMetadataDrift` |
