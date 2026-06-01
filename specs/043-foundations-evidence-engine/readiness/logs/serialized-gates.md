# Serialized six-target dogfood gate run (T032 / FR-015)

Captured: 2026-06-01T10:35:16Z. Branch: 043-foundations-evidence-engine.
Run sequentially in the canonical order (FAKE state is shared — never concurrent).
All gates passed first time; no race-like or environment-flaky failure occurred,
so no focused-isolation rerun was needed. The graph + audit were re-run a final
time after the last task-status change so the committed readiness artifacts
reflect the all-[X] state (real-tasks=34) (SC-008).

| # | Target | Result |
|---|--------|--------|
| 1 | Dev | Ok |
| 2 | GeneratedGuidanceCheck | Ok |
| 3 | TemplateCheck | Ok |
| 4 | GeneratedProductCheck | Ok (post-decommission: generated consumers run the packaged engine in-process; no Python/Bash scripts in the repo) |
| 5 | EvidenceGraph (T033) | verdict=ok, tasks=34, acyclic, no dangling refs |
| 6 | EvidenceAudit (T034) | verdict=PASS, 0 blockers |

## EvidenceGraph (T033)
```
=== speckit.evidence.graph (in-process) ===
feature: 043-foundations-evidence-engine
tasks: 34
verdict: ok
```

## EvidenceAudit (T034) — SC-008 (verdict=PASS, 0 unaccepted-synthetic / 0 auto-synthetic / 0 late-seh / 0 diff-scan / 0 readiness-contract)
```
=== speckit.evidence.audit (in-process) ===
feature: 043-foundations-evidence-engine
verdict=PASS
real-tasks=34
accepted-seh-tasks=0
unaccepted-synthetic-tasks=0
auto-synthetic-tasks=0
late-seh-tasks=0
diff-scan-hits=0
readiness-contract-hits=0
persistent-launch-hits=0
persistent-gui-runtime-hits=0
window-visibility-hits=0
audit-status-hits=0
total-blockers=0
```
