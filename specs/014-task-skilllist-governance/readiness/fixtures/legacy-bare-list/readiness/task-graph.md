# Task Graph — legacy-bare-list

## ✗ Graph validation failed

### Errors
- tasks.deps.yml: T001: existing bare-list metadata must be migrated to object form with deps and skillist
- T001: missing structured skillist in tasks.deps.yml

## Status counts (effective)

| Status | Count |
|--------|-------|
| [ ] pending | 1 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Record feature scope"]:::pending
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [ ] Record feature scope
```

