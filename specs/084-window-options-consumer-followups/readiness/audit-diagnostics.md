# Audit diagnostics — real EvidenceAudit stdout legibility (084, T023 / SC-004)

- **Authoritative command**: `./fake.sh build -t EvidenceAudit`
- **Artifact path**: this file + `readiness/logs/evidence-audit.txt`
- **Failure class**: a blocker not legible on stdout (reason/hit-file/base_ref missing) is a governance-observability defect (FR-008/FR-009).

## Final verdict (real, after capturing supported-host launch evidence)

The audit now passes; the diff-scan base ref is resolved (FR-009), not the misleading `null`:

```
verdict=PASS
real-tasks=33
unaccepted-synthetic-tasks=0
auto-synthetic-tasks=0
readiness-contract-hits=0
persistent-launch-hits=0
window-visibility-hits=0
audit-status-hits=0
total-blockers=0
diff-scan base_ref: main (merge-base 3a624ff75ed886f23ef24943f283b4d6f8b8904f)
```

## Per-blocker legibility (FR-008 / SC-004), demonstrated on a real failing run

Before the supported-host launch evidence was captured, the same audit surfaced its one
blocker fully on stdout — validation area, file, one-line reason, and the originating
hit-file path — with no `*-hits.json` sidecar opened. This is the SC-004 proof that a
blocker is identifiable from the audit's own output:

```
diff-scan base_ref: main (merge-base 3a624ff75ed886f23ef24943f283b4d6f8b8904f)
blockers:
  [persistent-launch] supported-host-persistent-launch.txt
    reason: missing supported-host persistent launch evidence
    hit-file: readiness/persistent-launch-hits.json
```

FR-008 (per-blocker area + file + reason + hit-file path) and FR-009 (resolved
`diff-scan base_ref` with merge-base) are both exercised against real `EvidenceAudit`
stdout. The supported-host persistent-launch + visible-window evidence
(`supported-host-persistent-launch.txt`, `interactive-visible-window.md`) was captured on
the display-capable host (`DISPLAY=:1`): every supported startup state opened a real
window with `window-opened=true` / `window-visible=observed:true`.
