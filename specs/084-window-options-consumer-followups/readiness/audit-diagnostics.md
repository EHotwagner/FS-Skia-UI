# Audit diagnostics — real EvidenceAudit stdout legibility (084, T023 / SC-004)

- **Authoritative command**: `./fake.sh build -t EvidenceAudit`
- **Artifact path**: this file + `readiness/logs/evidence-audit.txt`
- **Failure class**: a blocker that is *not* legible on stdout (reason/hit-file/base_ref missing) is a governance-observability defect (FR-008/FR-009).
- **Next action**: the single remaining blocker (supported-host persistent launch) is captured on a display-capable host at merge — see `interactive-visible-window.md` / `aggregate-hang-diagnostics.md`.

SC-004 is demonstrated by the audit's own stdout below: every blocker names its
**validation area**, **file**, **one-line reason**, and **originating hit-file path**,
and the **diff-scan base_ref** line is resolved (not the misleading `null`) — all
without opening any `*-hits.json` sidecar.

## Real `EvidenceAudit` stdout (verbatim excerpt)

```
verdict=FAIL
real-tasks=34
accepted-seh-tasks=0
unaccepted-synthetic-tasks=0
auto-synthetic-tasks=0
late-seh-tasks=0
diff-scan-hits=0
readiness-contract-hits=0
persistent-launch-hits=1
window-visibility-hits=0
audit-status-hits=0
total-blockers=1
diff-scan base_ref: main (merge-base 007a2c2a821f0c4729d699d1ea5645d4ccecd1f7)
blockers:
  [persistent-launch] supported-host-persistent-launch.txt
    reason: missing supported-host persistent launch evidence
    hit-file: readiness/persistent-launch-hits.json
```

## Why the remaining blocker is honest, not a defect

`persistent-launch` is triggered by the feature text "persistent graphical launch"
(T014). The scan requires a real **supported-host** window launch
(`window-opened=true`). The framework repo ships libraries + a template (no runnable
windowed product) and the local `GeneratedProductCheck` is a documented
**non-authoritative environment-failure**, so the authoritative launch + decodable
windowed-fullscreen screenshot are captured on a **display-capable host** at merge.
The framework-level behavior (new state, default, validation reclassification, and the
`applyWindowBehaviorToOptions` mapping) is proven against the built library in
`readiness/fsi-session.txt` and `tests/SkiaViewer.Tests` (54 passing).
