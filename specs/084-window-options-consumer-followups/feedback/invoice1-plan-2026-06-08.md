---
phase: plan
date: 2026-06-08
severity: minor
---

## Process friction

The `docs/scaffold-map.md` durable-vs-replaceable map names every file under
`src/Product/**` (e.g. `src/Product/Model.fs`, `src/Product/View.fs`), but the actual
generated project ships those files under `src/Invoice1/**`. The map and the generated
tree disagree on the directory name, so the map had to be mentally reconciled
file-by-file against the real layout before it could be trusted as authoritative. It
was unambiguous (the file *roles* matched one-to-one), but it cost a reconciliation
pass and a moment of doubt about whether the map was stale or the tree was. What would
have helped: the scaffold map referring to the product directory by its generated name
(or a `<ProductDir>` placeholder) so the path examples match the project verbatim.

A second, smaller friction: the plan template's machine-enforced `GeneratedGuidanceCheck`
requires all 12 governance areas filled with no bare `N/A`, but the "N/A-with-rationale
is allowed" rule lives only in an HTML comment inside the template — easy to miss and
worth surfacing in the plan skill's key-rules list too.

## Generalizable code

none — planning phase only; no F# was written this phase.

## Skill gaps

A dedicated "scaffold-model swap" skill would have helped. The scaffold-map doc covers
*what* is durable vs replaceable, but a skill could encode the swap *procedure* as a
checklist: rewrite `Model.fs`/`View.fs`/`BehaviorTests.fs`; re-point the durable
`Program.fs`/`EvidenceCommands.fs`/`LayoutEvidence.fs`/`WindowOptions.fs`; preserve the
must-survive evidence tokens and the `Invoice1.fsproj` compile order; leave
`GovernanceTests.fs` untouched — with a verification step (grep for each must-survive
token after the swap). This is currently reconstructed by hand from `scaffold-map.md`
plus `GovernanceTests.fs` each time.

## Research links

research blocked — offline planning session; no external lookups were required (all
inputs — spec, constitution, scaffold map, `.fsi` surfaces, skills — were available
in-repo).
