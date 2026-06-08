# Route / Route --enforce (feature 077, T026)

- **Authoritative command**: `./fake.sh build -t Route` and `./fake.sh build -t Route --enforce`.
- **Artifact**: this file.
- **Failure class**: governance (a `.agents/skills/**` / governance diff misrouted, or an
  escalated change missing a required evidence artifact).

## Result — both `Status: Ok`

```
tier=maintainer-verify
gates=Dev, GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck, PhaseHookParityCheck,
      SkillContractPathCheck, TemplateUpdateSkillPackageCheck, TemplateDrift, EvidenceGraph,
      EvidenceAudit, AgentReady, TargetMetadataDrift, Verify, Ci
matched-rules=evidence-governance, specify-catchall, docs-only, skill-quality, build-target-contract
```

- The `.agents/skills/**` edit routes through the **`skill-quality`** rule
  (`FocusedAuthority`); combined with the `build/Governance/**` + `validation.contract.yml`
  edits it escalates to **`maintainer-verify`**.
- **`PhaseHookParityCheck` appears in the printed gate list**, immediately after
  `SkillQualityCheck` (its position in the `skill-quality` rule's `RequiredGates`).
- `Route --enforce` returns **`Status: Ok`** — no escalated rule is missing its required
  evidence artifact. The skill-quality rule's required artifact
  `readiness/skill-quality-check.md` is present, and the new gate's own report
  `readiness/phase-hook-parity-check.md` (9/9 PASS) is present.
