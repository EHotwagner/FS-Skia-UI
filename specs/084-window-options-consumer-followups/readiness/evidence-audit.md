# Evidence Audit Evidence (084)

This feature-local record accompanies the merge-gate audit. The audit `verdict`,
per-blocker reasons, hit-file paths, and the resolved diff-scan `base_ref` line are
captured from real `EvidenceAudit` stdout in `readiness/audit-diagnostics.md` and
`readiness/logs/evidence-audit.txt`.

See `readiness/logs/evidence-audit.txt` for the `verdict`, `accepted-seh-tasks`,
`unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, and `late-seh-tasks` counts, the
`diff-scan base_ref:` line (FR-009), and the per-blocker `blockers:` section (FR-008).
Accepted `[SEH]` evidence remains synthetic and is reported separately from real task
evidence; this feature declares no `[SEH]` tasks.
