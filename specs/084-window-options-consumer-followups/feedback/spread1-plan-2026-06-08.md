---
phase: plan
date: 2026-06-08
severity: minor
---

## Process friction

Two points of friction during planning:

1. **scaffold-map path mismatch.** `docs/scaffold-map.md` describes the durable/replaceable
   split using generic paths (`src/Product/Model.fs`, `tests/Product.Tests/`), but the
   actual generated project uses the project name (`src/Spread1/`, `tests/Spread1.Tests/`).
   I had to cross-check the real tree to map each named file. What would have helped: the
   scaffold-map referring to the generated project name, or stating explicitly that
   `Product` is a placeholder for `<ProjectName>`.

2. **"durable" vs. "re-pointable" ambiguity.** `LayoutEvidence.fs`/`EvidenceCommands.fs`
   are listed as *durable* plumbing, yet they read scaffold model fields
   (`ActiveColumn`/`Tally`/`Stage`/`Screen`) and must be edited when the model is swapped.
   The map does say "keep them and re-point them at your own model," but the word "durable"
   initially read as "do not touch." Clarifying that durable = *keep the file + its scanned
   tokens, but re-point its model references* would remove the hesitation. The HUD→headers
   and gameplay→grid mapping for `LayoutEvidence` was also non-obvious and worth an example.

## Generalizable code

none — planning phase only; no F# was written.

## Skill gaps

A short **scaffold-swap planning** skill (or a scaffold-map addendum) covering: (a) the
`Product`↔`<ProjectName>` path substitution, (b) which durable files carry model-field
references that must be re-pointed (vs. genuinely model-agnostic ones like `WindowOptions.fs`),
and (c) a worked example of remapping the HUD/gameplay layout-evidence regions onto a
non-game UI (here: fixed chrome vs. scrollable content). This would have shortened the
durable-file reconnaissance.

## Research links

research blocked — offline environment; relied on in-repo `docs/scaffold-map.md`,
`docs/api-surface/*.fsi`, and the existing scaffold source as ground truth.
