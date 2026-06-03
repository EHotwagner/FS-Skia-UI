# Drift-Detection Red→Green Proof (US2, SC-004)

Real mutations against the real regenerated corpus. The **new** failure class this
feature adds — a hand-edited *generated copy* that diverges from its canonical
source — is demonstrated red→green below. The three preserved 056 mutations
(deleted obligation concept, removed contract token, reintroduced forbidden term)
remain enforced by the unchanged `Guidance.fs` rule set via `GeneratedGuidanceCheck`
(see `contract-tokens.md`); single-sourcing did not weaken them.

## NEW case — generated copy edited out of sync with its source (FR-005, SC-004)

Authoritative command: `./fake.sh build -t TargetMetadataDrift`
Canonical source: `GovernedBlocks visual-proof-phrases` (`build/Governance/GovernedBlocks.fs`)
Generated copy: the `gov/visual-proof-phrases` region of `template/base/docs/product.md`

### RED — hand-edit the generated copy

The `gov/visual-proof-phrases` region inner was replaced with
`TAMPERED: this generated copy was hand-edited out of sync with its canonical source.`
and `TargetMetadataDrift` was run:

```
Starting target 'TargetMetadataDrift'
Finished (Failed) 'TargetMetadataDrift' in 00:00:00.05
TargetMetadataDrift  (template/base/docs/product.md is stale — its generated
  `gov/visual-proof-phrases` region no longer matches its canonical source
  (GovernedBlocks `visual-proof-phrases`). Regenerate via
  ./fake.sh build -t RefreshSurfaceBaselines.)
Status:               Failure
```

The diagnostic names the **drifted file** (`template/base/docs/product.md`), the
**generated region** (`gov/visual-proof-phrases`), the **canonical source**
(`GovernedBlocks visual-proof-phrases`), and the **repair command** — exactly the
`ConstitutionFragments.currencyDrift` / `SkillTreeGen.currencyDrift` diagnostic
shape (FR-003, observability).

### GREEN — restore the in-sync copy

The region was restored to its canonical render (the idempotent splice produced by
`RefreshSurfaceBaselines`) and the gate reran:

```
Finished (Success) 'TargetMetadataDrift' in 00:00:00.05
Status:               Ok
```

## NEW case (2) — constitution generated copy edited out of sync (class 4, FR-007)

Authoritative command: `./fake.sh build -t TargetMetadataDrift`
Canonical source: the placeholder-bearing twin `.specify/templates/constitution-template.md`
Generated copy: the concrete render `.specify/memory/constitution.md`

### RED — hand-edit the generated constitution.md

The version line was changed (`1.3.0` → `9.9.9`) and the gate run:

```
Finished (Failed) 'TargetMetadataDrift'
TargetMetadataDrift  (.specify/memory/constitution.md is stale — it no longer
  matches the render of its canonical source
  .specify/templates/constitution-template.md. Regenerate via
  ./fake.sh build -t RefreshSurfaceBaselines.)
Status:               Failure
```

Names the drifted file and its canonical source. (GREEN: restoring the file to its
render — `git checkout` / `RefreshSurfaceBaselines` — returns the gate to `Ok`.)

The render relationship itself is golden-tested: `GovernedBlocksTests` asserts
`renderConstitution Concrete canonical = constitution.md` byte-for-byte and the two
twins are byte-identical, so a wrong substitution edit fails in CI, not silently.

## Preserved 056 cases (unchanged Guidance.fs rule set, FR-004/FR-005)

`Guidance.fs` (`taskSkillistGuidanceCheck`, `controlsBoundaryGuidanceCheck`) is
byte-unchanged in its `ContractToken` / `GuidanceObligation` / `Forbidden`
inventory, so the three 056 negatives still bite under `GeneratedGuidanceCheck`
(`evaluateGuidanceCheck`):

1. **Deleted obligation concept** → `<file>: obligation '<id>' (<source>) not reflected [<tag>]`.
2. **Removed contract token** → `<file>: missing \`<token>\` [<tag>]`.
3. **Reintroduced forbidden term** → `generated controls guidance contains stale term \`<token>\` [<tag>]`.

These are exercised by the preserved negatives in
`tests/Governance.Tests/GuidanceValidatorTests.fs` and the green
`GeneratedGuidanceCheck` over the regenerated corpus (recorded in
`contract-tokens.md`).
