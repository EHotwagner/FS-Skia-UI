# AgentReady Verdict

Status: `degraded`

- authority: `focused-authoritative`
- required-gates: `EvidenceGraph`, `EvidenceAudit`
- completed-gates: `EvidenceGraph`, `EvidenceAudit`
- missing-gates: (none — both run gate-by-gate; see `target-metadata.md`)
- missing-artifacts: (none required for the merge verdict)
- next-command: `./fake.sh build -t EvidenceAudit`
- diagnostic: AgentReady reports `degraded` because the aggregate `Verify`/`Ci` umbrella
  that would mark a `ready` handoff cannot bootstrap the `dotnet-fake` global tool in this
  sandbox (see `runtime-limitations.md`). Every constituent maintainer-verify gate was run
  individually and sequentially and passed; the authoritative merge gate `EvidenceAudit` is
  `verdict=PASS`.
