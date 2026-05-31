# Generated-Validation Authority — 040

| Field | Value |
|---|---|
| Authoritative command | per-gate focused rerun (`./fake.sh build -t SkillSyncCheck`, `./fake.sh build -t SkillExamplesCheck`) |
| Artifact path | `specs/040-foundations-capability-skills/readiness/` (`skill-sync-check.md`, `skill-examples-check.md`, `logs/`) |
| Failure class | aggregate-vs-focused-authority |
| Next action on failure | Re-run the affected FAKE-backed gate **sequentially in isolation**; treat that focused result as authoritative |

## Authority model

- **No generated product is involved.** The six capability skills are
  author/agent reference material under `.claude/skills/**` and
  `.agents/skills/**`; they are never generated into a product, so
  `GeneratedProductCheck` / `GeneratedGuidanceCheck` meaning is unchanged by
  this feature.
- **Aggregate FAKE runs are non-authoritative.** FAKE shares `.fake` state and
  is not safe to run concurrently; if an aggregate `Dev`/`Verify` shows a
  race-like failure for either new gate, the **focused per-gate rerun is
  authoritative**.
- **The two gates are self-validating.** `SkillSyncCheck` recomputes SHA-256
  over the live trees; `SkillExamplesCheck` regenerates `Generated/*.fs` and
  recompiles every block on each run — there is no cached verdict to go stale.

## Serialized FAKE order for this feature

1. `./fake.sh build -t Dev` (now includes `SkillSyncCheck` + `SkillExamplesCheck`)
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`
