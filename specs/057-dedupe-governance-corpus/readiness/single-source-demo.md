# Single-Source Change Demonstration (US1, SC-001/SC-003)

A governed phrase set now changes in **exactly one** canonical source, and every
derived copy regenerates identically with the gates green.

## The one canonical source

`build/Governance/GovernedBlocks.fs` → `visualProofPhrases` / `ownerPhrases`
`GovernedBlock` values. Each declares one `CanonicalText` and a `Targets` list of
home files. `RefreshSurfaceBaselines` splices the canonical text into every
`gov/<id>` region; `TargetMetadataDrift` proves each region matches.

## Demonstration

1. **Edit one place.** Change the `CanonicalText` of a `GovernedBlock` (the single
   source) — e.g. reword the visual-proof rejection phrasing.
2. **Regenerate.** `./fake.sh build -t RefreshSurfaceBaselines` splices the new
   text into both home files (`.agents/skills/fs-skia-layout-evidence/SKILL.md`
   and `template/base/docs/product.md`) and regenerates the `.claude` peer.
3. **Confirm no home file was hand-edited.** `git diff` shows only the canonical
   `GovernedBlocks.fs` source plus the regenerated outputs differ — never a
   hand-edited copy.
4. **Gates green.**
   - `./fake.sh build -t GeneratedGuidanceCheck` → `Status: Ok` (every token /
     obligation still present over the regenerated corpus — SC-003).
   - `./fake.sh build -t TargetMetadataDrift` → `Status: Ok` (every generated copy
     current with its canonical source).

This session exercised the splice and currency end-to-end: the regeneration is
idempotent (`git diff` after `RefreshSurfaceBaselines` showed only the intended
marker edits), and the new-failure-class red→green (`dedupe-red-green.md`) proves
a hand-edited copy is caught.

## Verified live this session (2026-06-03) — constitution canonical edit (N=3→1)

The strongest single-source case (the constitution triple) was re-exercised
end-to-end against the real corpus:

1. **Edited one canonical source** — a single principle-body sentence in
   `.specify/templates/constitution-template.md`
   (`Rationale: FSI is the honest audience.` → `… (single-source-demo).`).
2. `./fake.sh build -t RefreshSurfaceBaselines` → `Status: Ok`.
3. **Both derived copies carried the edit** from the one source:
   - `.specify/memory/constitution.md:22` (concrete render) — present.
   - `.specify/presets/fsharp-opinionated/templates/constitution-template.md:27`
     (preset twin, verbatim) — present.
4. **No home file was hand-edited** — the only new changes were exactly the 3
   constitution files (1 hand-edited canonical + 2 regenerated); `git diff
   --name-only` showed no other corpus file touched by the edit.
5. **Gates green** — `GeneratedGuidanceCheck` → `Status: Ok`;
   `TargetMetadataDrift` → `Status: Ok`.
6. Reverted via `git checkout` (no residue; corpus byte-clean).

This is the FR-002/SC-001 result at its largest N: one canonical edit, three
files regenerated identically, gates green.

## Files-touched-per-rule-change (N → 1, SC-001)

| Rule | Before | After |
| --- | --- | --- |
| visual-proof rejection phrases | 3 hand-carried home files | **1** canonical `GovernedBlock` + regeneration |
| owner / host-warning phrases | 3 hand-carried home files | **1** canonical `GovernedBlock` + regeneration |

See `structural-reduction.md` for the full maintenance-surface accounting.
