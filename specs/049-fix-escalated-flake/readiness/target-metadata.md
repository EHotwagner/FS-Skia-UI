# Target metadata invariants (T014, contract C6)

No FAKE target was added, removed, or renamed by this feature. The change is confined to
**how** existing targets spawn processes and select a graphics backend, not to the target
surface.

- **Targets unchanged**: `build/Governance/Targets.fs` / `Targets.fsi` are untouched
  (`git diff --stat -- 'build/Governance/Targets.*'` is empty). The dispatch registry
  (`Targets.dispatchTargets`) and dependency rows (`Targets.targetDependencyRows`) are
  byte-identical.
- **Contract unchanged**: `validation.contract.yml` is generated from `Routing.fs`; neither
  was edited, so the generated contract and `TargetMetadata` / `TargetMetadataDrift` outputs
  are unchanged. `TargetMetadataDrift`'s currency check therefore stays green.
- **Routing unchanged**: `Routing.fs` is untouched. `Route` reports `tier=agent-ready`,
  `gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`,
  `dogfood-forced=false`, `matched-rules=evidence-governance, specify-catchall, docs-only`.
  The `build/**` edits escalate via routing **default-deny** (they match no allow rule and
  no `build-target-contract` glob), exactly as the plan states.
- **Serialization unchanged**: the escalated path remains serialized and order-sensitive
  (shared `.fake` state); this feature neither relaxes that nor introduces any dependency on
  parallel execution.

Changed surfaces (build front-end internals + its test project only):
`build/Governance/Front/BuildEnvironment.fs` (new),
`build/Governance/Front/BuildProcess.fs`, `build/Governance/Front/BuildProcessHealth.fs`,
`build/Program.fs`, `build/Governance/FS.Skia.UI.Build.fsproj` (compile-order entry),
`tests/Governance.Tests/GraphicsEnvironmentTests.fs` (new),
`tests/Governance.Tests/Governance.Tests.fsproj` (compile entry).

Product surface proof: `git diff --stat -- 'src/**'` is **empty** — product runtime and all
`.fsi` signatures are byte-unchanged (SC-004, Tier 2). No public package / `.fsi` / surface
baseline / `PackageVersion` change.
