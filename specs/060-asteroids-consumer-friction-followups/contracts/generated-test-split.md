# Contract: Generated Test Split (FR-005)

## Purpose
Separate durable, model-agnostic governance scans from replaceable scaffold-behavior
tests in generated projects so swapping the model does not break governance coverage.

## Change
Split `template/base/tests/Product.Tests/Tests.fs` (570 lines) into:

- `GovernanceTests.fs` — durable scans: `productSource`/`productSources` source-text
  assertions, `visualEvidenceGuidance` presence, source-structure / placeholder /
  excluded-history checks, evidence-command discoverability. Model-agnostic; survives a
  scaffold-model swap.
- `BehaviorTests.fs` — replaceable: scaffold `Product.Program.view` / `update` /
  scene-text behavior, `RenderCount`, effect-emptiness. Rewritten by the consumer when
  they replace the scaffold model.

`Product.Tests.fsproj` compiles `GovernanceTests.fs` **before** `BehaviorTests.fs`.
Both files keep their original `//#if (profile == ...)` conditionals.

## Governance gate updates
`TemplateCheck` / `GeneratedProductCheck` source-structure assertions that named
`Tests.fs` now name the two files (or the directory + both filenames).

## Test evidence
- SC-003: after replacing the scaffold model in a generated project, `GovernanceTests.fs`
  still compiles and runs; only `BehaviorTests.fs` requires rewriting — captured in
  `generated-project/test-split.log`.

## Acceptance
SC-003.
