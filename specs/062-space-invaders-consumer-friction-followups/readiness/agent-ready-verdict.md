# AgentReady Verdict

Status: `degraded`

- authority: `focused-authoritative`
- tier: `Tier 1` (driven solely by FR-010; consumer-contract change-sets escalate the rest)
- required-gates: `EvidenceGraph`, `EvidenceAudit` (plus the Route-escalated gates per
  change-set: `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `SkillSyncCheck`, `TargetMetadataDrift`, `SkillQualityCheck`, and — FR-010 —
  `PackageSurfaceCheck`/`PerPackageSurfaceDiff`)
- completed-gates: (stamped gate-by-gate; see `target-metadata.md` per-target table)
- missing-gates: (none — both Evidence gates run gate-by-gate)
- missing-artifacts: (none required for the merge verdict)
- elmish-mvu: `N/A` (no framework `Model`/`Msg`/`Effect`; FR-010 helpers are pure value types)
- next-command: `./fake.sh build -t EvidenceAudit`
- diagnostic: AgentReady reports `degraded` because the aggregate `Verify`/`Ci` umbrella
  that would mark a `ready` handoff cannot bootstrap the `dotnet-fake` global tool in this
  sandbox (see `runtime-limitations.md`). Every constituent gate Route prints is run
  individually and sequentially; the authoritative merge gate `EvidenceAudit` is the
  verdict. This is a non-authoritative aggregate limitation, not a gate failure.
