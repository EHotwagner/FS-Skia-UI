# Governance-Internal Contracts: Governance Gate Hardening

This feature changes **governance-internal** contracts (no `src/**/*.fsi` public
surface). Each shape below is generated from a single source and currency-checked
(`TargetMetadataDrift` / `SkillSyncCheck`). These are the *consumers* of the
engine value types in [data-model.md](../data-model.md).

## C1 — `seh-audit-summary.json` verdict schema (FR-007/008)

Written by `Render.sehAuditSummaryJson` (`Evidence/Render.fs`). Adds a verdict
state and separated synthetic counts.

```jsonc
{
  "verdict": "Pass | PassWithAcceptedDeferrals | Fail",   // NEW: three-state (was Pass|Fail)
  "acceptedSyntheticCount": 0,                            // NEW: separated count
  "unacceptedSyntheticCount": 0,                          // NEW: separated count
  "acceptedDeferrals": [                                  // NEW: durable structured records
    {
      "taskId": "T0xx",
      "justification": "…written rationale…",
      "realEvidencePath": "specs/<feat>/readiness/…",
      "awaitedHostCapability": "…"
    }
  ],
  "acceptedSehTasks": [ "…" ],        // existing
  "unacceptedSyntheticTasks": [ ],    // existing (must be empty for any Pass*)
  "autoSyntheticTasks": [ ],          // existing — now reflects ExplicitDeps-only propagation
  "lateSehTasks": [ ],                // existing
  "diagnostics": [ "…" ]              // existing
}
```

**Contract rule (FR-011)**: `verdict` is `PassWithAcceptedDeferrals` **only if**
`unacceptedSyntheticCount == 0` AND every blocking-hit array (diff-scan,
readiness-contract, persistent-launch, persistent-gui-runtime, window-visibility,
audit-status, invalid-seh) is empty. Otherwise `Fail`. `Pass` requires
`acceptedDeferrals` also empty.

## C2 — `readiness/synthetic-evidence.json` accepted-deferral record (FR-008)

The durable record `--accept-synthetic` writes (not solely a logged flag):

```jsonc
{
  "acceptedDeferrals": [
    {
      "taskId": "T0xx",
      "justification": "non-empty written rationale (required)",
      "realEvidencePath": "where real evidence lands once the capability exists",
      "awaitedHostCapability": "the host capability the artifact awaits"
    }
  ]
}
```

`--accept-synthetic` requires the written justification (retained from
Constitution §V); the audit reads these records to compute C1's verdict and
counts.

## C3 — skill-loading-evidence row schema (FR-010)

Single source `Evidence/EvidenceFormatSchema.fs`, mirrored in
`docs/evidence-formats.md`. The existing 8-column row gains a 9th column.

```
| Task | Skill id | Resolved skill path | Load result | loaded_at | work_started_at | Evidence path | Exception | provenance |
```

- `provenance ∈ { captured, asserted }` (NEW). `captured` = observed during the
  run (recorded at the load action, before code changes); `asserted` =
  hand-authored.
- Existing rules unchanged: ISO-8601 timestamps, `loaded_at < work_started_at`
  (equal/reverse rejected), one row per task/skill.
- **Gap surfacing (NEW)**: a declared-but-unloaded skill is reported at the
  declaring task's implementation point, not deferred to the `[X]` flip.

## C4 — package-skew finding (FR-003/004)

Emitted by the static skew check (sub-check of `TemplateCheck` /
`GeneratedProductCheck`, `Front/Governance.fs`). Blocking when non-empty.

```jsonc
{
  "packageSet": "LocalPacked | Pinned",   // FR-004: which set this report used
  "skewFindings": [
    {
      "symbol": "ControlRenderResult.Bounds",
      "file": "generated/.../SomeTest.fs",
      "pinnedVersion": "0.1.91-preview.1",
      "localVersion": "0.1.92-preview.1"
    }
  ]
}
```

- Computed statically from surface baselines (no network restore). Empty
  `skewFindings` on the real tree; non-empty blocks before merge naming
  symbol + file + version gap.

## C5 — per-step generated-product classification (FR-002/004)

Each generated-product report states, per step, its pass/fail and classification:

```jsonc
{
  "packageSet": "Pinned",
  "steps": [
    { "step": "Build",  "passed": true,  "classification": null,            "packageSet": "Pinned" },
    { "step": "Test",   "passed": true,  "classification": null,            "packageSet": "Pinned" },
    { "step": "Verify", "passed": false, "classification": "Environment",   "packageSet": "Pinned" }
  ],
  "verdict": "pass-or-fail by ProductDefect steps only"
}
```

**Contract rule (FR-002)**: overall fail iff any step `passed=false` AND
`classification=ProductDefect`. An `Environment` classification on one step never
suppresses a `ProductDefect` on another in the same run.

## C6 — `validation.contract.yml` / target metadata (regenerated)

`validation.contract.yml` is regenerated from `Routing.fs` (`ContractView.render
Routing.rules`); target metadata regenerates with it. Any new skew sub-check or
changed target routing flows from `Routing.fs` and is currency-checked by
`TargetMetadataDrift`. No hand-syncing.
