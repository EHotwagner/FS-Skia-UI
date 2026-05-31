# Governance Risk Levels — 040 Foundations Capability Skills

| Field | Value |
|---|---|
| Authoritative command | `./fake.sh build -t SkillSyncCheck` then `./fake.sh build -t SkillExamplesCheck` (focused, authoritative for this feature) |
| Artifact path | `specs/040-foundations-capability-skills/readiness/logs/` |
| Failure class | governance-risk-classification |
| Next action on failure | Re-run the named gate sequentially in isolation; aggregate FAKE results are non-authoritative. Fix the skill/block or the byte-drift; never weaken a gate. |

## Risk level for this feature

Governance risk level is **small→medium** (build-tooling only; two new FAKE
targets, seven build-tooling packages, new `build/Governance` `.fsi`). The
scale:

- **small** — additive, isolated change; required evidence is a clean focused
  build/test of the touched projects.
- **medium** — changes an inter-project contract or a shared build path
  (here: `Dev` gains two dependencies); required evidence adds the relevant
  gates for the affected area — the two new gates plus the serialized order.
  **This feature sits here.**
- **broad** — touches governance prose, consumer contracts, or the
  template/generated-product surface; required evidence is the full serialized
  gate set.

## Focused vs broad validation

- **Focused (authoritative) for this feature:** `SkillSyncCheck` +
  `SkillExamplesCheck`, plus the serialized order `Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
  `EvidenceGraph` → `EvidenceAudit`.
- **Broad (full `Verify`)** is required only if a gate failure looks race-like
  or the concurrent FAKE context is unknown. Aggregate FAKE results are recorded
  as **non-authoritative**; the focused per-gate rerun is authoritative.

## Runtime / governance-prose risk

- **Runtime risk: none.** No `src/**` edit; runtime surface baselines untouched.
- **Governance-prose risk: none.** No constitution / Spec Kit command /
  consumer-contract text changes. `EvidenceGraph`/`EvidenceAudit` outputs are
  unchanged (SC-005).
- **Build-tooling risk: small (contained).** Seven new central package pins in a
  build-tooling `ItemGroup`; two new FAKE targets that no existing target's
  meaning depends on.
