---
title: Single-Source Generation
category: Governance
categoryindex: 5
index: 23
description: How governance artifacts are generated from one canonical source instead of hand-synced — validation.contract.yml from Routing.fs, the .claude skill tree from .agents, and the other generated docs — with the RefreshSurfaceBaselines entry point and the currency gates that enforce it.
---

# Single-Source Generation

Several files in this repository look like policy but are not: they are
**generated views** of a single canonical source. `validation.contract.yml` is
rendered from the compiled routing rules; the `.claude/skills/**` tree is rendered
from the canonical `.agents/skills/**` tree; `docs/evidence-formats.md`,
`docs/skillist-reference.md`, the typed controls catalog, the design-token module,
and the generated API-surface docs are all derived from upstream sources. None of
them is hand-synced. A single FAKE target — `RefreshSurfaceBaselines` —
regenerates every one of them, and a set of **currency gates** fail the build if a
committed view drifts from what its source would regenerate. This page explains
the pattern, the concrete generated artifacts, the regeneration entry point, and
why nothing is kept in sync by hand. For the routing rules that drive the contract
view, see [routing and gates](./routing-and-gates.html); for the evidence model,
see [evidence and audit](./evidence-and-audit.html).

## The pattern: one source, a rendered view, a currency check

Every generated artifact follows the same three-part shape:

```text
canonical source  ──render──▶  generated view  ──currency check──▶  PASS / FAIL
```

- The **canonical source** is the single place a fact is authored. For routing it
  is a compiled F# module; for skills it is the `.agents` tree; for design tokens
  it is a DTCG JSON file.
- The **generated view** is a committed file that consumers, agents, or other
  tools read. It is committed (so it can be diffed in a PR and read without
  running a build) but it is *output*, not source.
- The **currency check** is a governance gate that re-renders the view from the
  source in-process and fails if the committed bytes differ, naming both the
  stale file and the command that fixes it.

This is stronger than two-way drift comparison between peer files. There is only
one source of truth; the view is a function of it. You edit the source and
regenerate — you never edit the view.

## `validation.contract.yml` is generated from `Routing.fs`

The authoritative routing policy — tiers, path globs, required gates, expected
artifacts, failure owners — lives in the compiled F# module
[`build/Governance/Routing.fs`](https://github.com/EHotwagner/FS-Skia-UI/tree/main/build/Governance/Routing.fs).
`validation.contract.yml` at the repository root is a YAML *rendering* of those
rules, produced by `ContractView.render` so that consumers which scan a contract
file (and the compatibility `AgentValidation` selector) see the same rules the
typed selector enforces.

The generated file says so in its own header:

```yaml
# GENERATED from build/Governance/Routing.fs (feature 042). Do not hand-edit;
# regenerate via ./fake.sh build -t RefreshSurfaceBaselines. The compiled
# Routing module is the single source of truth for tiers and routing rules.
```

`ContractView.render` walks the typed `Routing.rules` and emits each rule's `id`,
`tier`, `paths`, `required_gates`, `expected_artifacts`, `timeout_class`, and
`failure_owner` — and because it renders the *same* `Paths` list that the typed
`Matches` predicate is derived from, the rendered `paths:` view and the compiled
matcher cannot diverge.

**Currency is enforced by `TargetMetadataDrift`.** That gate reads the committed
`validation.contract.yml`, calls `ContractView.currencyDrift` against the live
`Routing.rules`, and fails when the on-disk file does not match a fresh render —
with the diagnostic *"validation.contract.yml is stale — regenerate from
Routing.fs via ./fake.sh build -t RefreshSurfaceBaselines"*. A missing file is
also a failure. The comparison normalises line endings and trailing whitespace,
so the only way to satisfy it is to regenerate. The gate's pure currency
computation lives in `ContractView`; only the file read happens at the
interpreter edge.

## The `.claude` skill tree is generated from `.agents`

Skills exist in two trees. The canonical, FS-authored source is
`.agents/skills/**`; the `.claude/skills/**` tree is its **generated mirror** for
the Claude Code surface. You author and edit a skill under `.agents`; you never
hand-edit its `.claude` peer.

**Currency is enforced by `SkillSyncCheck`**, which asserts that
`.claude/skills/**` is a current regeneration of `.agents/skills/**`. The two
trees are treated as synchronized peers, but only one of them is editable — the
other is output. Routing also reflects this: an edit under `.agents/skills/**`
routes `SkillSyncCheck` (alongside the skill-quality rubric) so the generated
mirror cannot silently fall behind a source edit. The vendored `speckit-*` skills
are excluded *inside* the gate, not by path.

## The other generated artifacts

`RefreshSurfaceBaselines` regenerates a family of single-source artifacts in one
operation, each with its own currency gate (the doc-reference and metadata
currency checks all fold into `TargetMetadataDrift`):

| Generated view | Canonical source | Currency gate |
|---|---|---|
| `validation.contract.yml` | `Routing.rules` (`Routing.fs`) | `TargetMetadataDrift` |
| `.claude/skills/**` | `.agents/skills/**` | `SkillSyncCheck` |
| `docs/evidence-formats.md` (under `template/base/docs/`) | `Evidence.EvidenceFormatSchema` | `TargetMetadataDrift` |
| `docs/skillist-reference.md` | live `SkillRegistry` + the closed `owns` vocabulary | `TargetMetadataDrift` |
| `src/Controls/catalog.yml` + `Catalog.fs` typed rows | `CatalogGen.catalogFacts` | `ControlsCatalogGenerationCheck` |
| `src/Controls/DesignTokens.fs` | the DTCG `design-tokens.tokens.json` | `DesignTokenDrift` |
| `template/base/docs/api-surface/**` | `template/capabilities.yml` `contracts:` | `TargetMetadataDrift` (via `ApiSurfaceGen`) |
| constitution principle fragments + governed prose blocks | `.specify/memory/constitution.md` / `GovernedBlocks.governedBlocks` | `TargetMetadataDrift` |

The pattern is the point, not the exact list: a fact that must appear in more than
one place is authored once and *rendered* into the others, and a gate re-renders
and compares so a hand-edit to a generated copy is caught.

## `RefreshSurfaceBaselines`: the single regeneration entry point

There is exactly one command to regenerate every generated artifact:

```bash
./fake.sh build -t RefreshSurfaceBaselines
```

A single target body emits the full set of regeneration effects. In source order,
`RefreshSurfaceBaselines`:

- refreshes the stable package-surface baselines;
- re-renders `validation.contract.yml` from `Routing.rules`;
- splices every canonical `GovernedBlock` into its home files (done *before* the
  skill-tree regen, so a governed block spliced into an `.agents` `SKILL.md`
  propagates into its `.claude` peer in the same pass);
- regenerates the typed controls catalog from `CatalogGen.catalogFacts`;
- regenerates `src/Controls/DesignTokens.fs` from the DTCG source;
- regenerates the `.claude` skill tree from `.agents`;
- regenerates the constitution principle fragments;
- regenerates the emitted `docs/api-surface/` tree from the capability catalog;
- regenerates `docs/evidence-formats.md` from `EvidenceFormatSchema`;
- regenerates `docs/skillist-reference.md` from the live `SkillRegistry`;
- then requires the regenerated baseline files exist.

Because all generation flows through this one target, the regeneration order is
deterministic and the dependency between regen steps (for example, governed-block
splicing *before* skill-tree mirroring) is encoded once, in the target body — not
left to whoever happens to be editing.

## Why nothing is hand-synced

Hand-syncing two files that must agree is a standing invitation to drift: a change
lands in one and not the other, and the disagreement is silent until something
downstream breaks. Single-source generation removes the failure mode by
construction:

- **There is one editable source.** You cannot forget to update "the other copy"
  because the other copy is output, not a peer you maintain.
- **Drift is a build failure, not a latent bug.** If a generated view is committed
  stale — or someone hand-edits it — the currency gate fails with a diagnostic
  naming the stale file and the regeneration command. The check re-renders from
  the source in-process; there is no way to satisfy it except by regenerating.
- **The fix is mechanical and uniform.** Every drift diagnostic points at the same
  command, `./fake.sh build -t RefreshSurfaceBaselines`. There is one thing to run
  and one source to have edited first.

The practical loop is therefore: edit the **canonical source** (the routing rules,
the `.agents` skill, the DTCG tokens, the catalog facts), run
`RefreshSurfaceBaselines`, and commit the source together with the regenerated
views. If you skip the regeneration, `TargetMetadataDrift`, `SkillSyncCheck`,
`ControlsCatalogGenerationCheck`, or `DesignTokenDrift` will stop you — by design.

---

See also: [governance index](./index.html) ·
[routing and gates](./routing-and-gates.html) ·
[evidence and audit](./evidence-and-audit.html) ·
[speckit placement](./speckit-placement.html) ·
[API reference](../reference/index.html).
