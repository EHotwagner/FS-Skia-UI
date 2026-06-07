# Contracts: Governed-Artifact Contracts Touched

This feature exposes **no external API, CLI, or service interface** — it is documentation /
governance only. The applicable "contracts" are the **governed-artifact invariants** the
authored files must satisfy, and the **factual contracts** the skill prose must describe
correctly. Each is machine-checked except the plan-report refresh (checked by the US2
independent test).

## C1 — Skill single-source contract (US1 + US3)

**Invariant**: `.agents/skills/<name>/SKILL.md` is the canonical source; the matching
`.claude/skills/<name>/SKILL.md` is **generated** from it by
`./fake.sh build -t RefreshSurfaceBaselines`. The `.claude` peer MUST NOT be hand-edited.

| Aspect | Contract |
| --- | --- |
| Authoring direction | Edit `.agents` only; regenerate `.claude`. |
| Discovery | New skills are found via SKILL.md frontmatter `name:` (`SkillRegistry`), no hardcoded list. |
| Currency gate | `SkillSyncCheck` FAILS if any `.claude` peer is stale vs. a fresh render of its `.agents` source (zero-drift = pass, SC-002). |
| Quality gate | `SkillQualityCheck` enforces skill-quality heuristics. |
| Contract-path gate | `SkillContractPathCheck` requires referenced contract paths to resolve. |
| Index | `RefreshSurfaceBaselines` also refreshes `.claude/skills/GENERATED.md` / the skillist-reference; a new skill must appear there. |

**Required frontmatter (both skills)**: `name`, `description`; this repo's skills also carry
`compatibility` and `metadata.{author,source}`. For the new `fs-skia-reconciliation` skill,
`name:` MUST be exactly `fs-skia-reconciliation` (drives discovery + the `.claude` path).

## C2 — Catalog-generation pattern the US1 prose MUST describe accurately

The worked example added to `fsharp-code-generation` MUST faithfully describe the
**shipped** feature-066 behavior (it documents, it does not change it). Authoritative facts
from `build/Governance/CatalogGen.fsi` (read-only):

| Fact | Value the prose must state |
| --- | --- |
| Single source | `catalogFacts : TypedCatalogFact list` (six 065-typed controls) |
| Generated artifacts | `catalog.yml` and `Catalog.fs` (`catalogYmlRel`/`catalogFsRel`) |
| Splice boundary | only `typed-catalog/<id>` marked regions; the 41 hand-authored rows outside markers are never read/written |
| Regeneration | `RegenerateCatalog`, run inside `RefreshSurfaceBaselines` |
| Drift gate | `ControlsCatalogGenerationCheck` (backed by `currency`/`isCurrent`/`currencyDrift`) |
| Cross-check | `Module`/required-attribute facts vs. the `FS.Skia.UI.Controls.Typed` surface |
| Failure rule | hand-editing a generated region fails the drift gate; the diagnostic points at `RefreshSurfaceBaselines` |

## C3 — Reconciliation facts the US3 prose MUST describe accurately

The new skill MUST faithfully describe the **shipped** feature-067 module (documents, does
not change it). Authoritative facts from `src/Controls/Reconcile.fsi` (read-only):

| Fact | Value the prose must state |
| --- | --- |
| Accessibility | `module internal Reconcile` — assembly-internal, no public-surface entry |
| Core fns | `diff : prev -> next -> ReconcileResult` (pure/total/deterministic); `apply` (round-trip proof) |
| Matching | key-first, then unkeyed residuals positionally; `Kind` mismatch ⇒ whole-subtree `Replace` |
| Operation set | `NodePatch` (`Keep`/`Replace`/`Update`), `ChildOp` (`ChildKeep`/`ChildMove`/`ChildInsert`/`ChildRemove`), `UpdatePatch`/`FieldChange`/`AttrChange` |
| Diagnostics | `ReconcileResult.Diagnostics` incl. duplicate-key `KeyCollision`; never throws |
| Test reach | `[<assembly: InternalsVisibleTo("Controls.Tests")>]` |
| Disposition | property-tested, deliberately unwired, parked; render-path integration is deferred/out-of-scope future work (name the integration point) |

## C4 — Plan-report provenance contract (US2)

| Region | Rule |
| --- | --- |
| Status header, status-by-feature table, §13 roadmap, §16 skills backlog | MAY be refreshed to match `main` |
| §1 onward original plan body | MUST remain unedited (provenance) |
| Skill references | MUST name only skills that exist — zero `fs-skia-project` references (SC-003) |
| Status claims | MUST match `git log` on `main` (065–073 merged, with squash commits) |

## Non-contracts (explicitly unchanged)

- Public `.fsi` signatures, package surface baselines, sample contracts — **zero delta**
  (SC-005).
- `Routing.fs` rules / `validation.contract.yml` — no new gate or rule; routing is unchanged.
- The `Reconcile` and `CatalogGen` source modules — read-only references, not edited.
