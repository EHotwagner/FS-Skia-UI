# Governance Risk Levels — 041 Foundations Library Validators

| Field | Value |
|---|---|
| Authoritative command | `./fake.sh build -t Dev` (runs Governance.Tests incl. parity + typed-finding suites), focused and authoritative for this feature |
| Artifact path | `specs/041-foundations-library-validators/readiness/logs/` |
| Failure class | governance-risk-classification |
| Next action on failure | Re-run the named gate sequentially in isolation; aggregate FAKE results are non-authoritative. Fix the byte-drift or the typed finding; never weaken a gate. |

## Risk level for this feature

Governance risk level is **small→medium** (build-tooling only; internal
refactor/extraction; no new FAKE target; reuses the already-pinned YamlDotNet;
new `build/Governance` `.fsi`). The scale:

- **small** — additive, isolated change; required evidence is a clean focused
  build/test of the touched build-tooling.
- **medium** — changes a shared build path (here: the `update`/dispatch
  converts to a typed `Targets.Target` and three interpret cases call the
  library in-process); required evidence adds the relevant gates for the
  affected area — the `Dev` gate plus the serialized order. **This feature sits
  here.**
- **broad** — touches governance prose, consumer contracts, or the
  template/generated-product surface; required evidence is the full serialized
  gate set.

## Focused vs broad validation

- **Focused (authoritative) for this feature:** the `Dev` gate (golden-diff
  parity + ≥6 typed-finding cases), plus the serialized order `Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
  `EvidenceGraph` → `EvidenceAudit`. Required evidence is the byte-identical
  three reports and the green Governance.Tests run.
- **Broad (full `Verify`)** is required only if a gate failure looks race-like
  or the concurrent FAKE context is unknown. Aggregate FAKE results are recorded
  as **non-authoritative**; the focused per-gate rerun is authoritative.

## Runtime / governance-prose risk

- **Runtime risk: none.** No `src/**` edit; runtime surface baselines untouched.
- **Governance-prose risk: none.** No constitution / Spec Kit command /
  consumer-contract text changes. `EvidenceGraph`/`EvidenceAudit` outputs are
  unchanged in meaning (SC-005); the three extracted reports are byte-identical.
- **Build-tooling risk: small→medium (contained).** Four new build-tooling
  `.fsi`/`.fs` module pairs; the typed `Target` DU now single-sources target
  identity, dependencies, and metadata, so a renamed target is a compile error
  rather than a runtime drift (SC-003).
