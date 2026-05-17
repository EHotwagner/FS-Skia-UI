# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-17T17:48:32.6966645+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `41653 MB`
- process-count: `1838`
- zombie-process-count: `1823`
- thread-limit: `253241`
- thread-headroom: `251403`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `202`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 41653 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 1838 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 1823 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
