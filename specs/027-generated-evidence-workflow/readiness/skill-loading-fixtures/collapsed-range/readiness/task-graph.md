# Task Graph — collapsed-range

## ✗ Graph validation failed

### Errors
- collapsed task range row is invalid: T001-T002
- multi-task prose row is invalid: T001-T002
- multi-skill prose row is invalid: speckit-tasks, speckit-implement
- T001: declared skill speckit-tasks has no pre-work load evidence
- T002: declared skill speckit-implement has no pre-work load evidence

## Skill Match Assessments

| Task | Candidate | Confidence | Signals | Reviewer disposition | Diagnostic |
|------|-----------|------------|---------|----------------------|------------|
| T001 | (none) | none |  | declared | T001: no high-confidence capability signal detected |
| T002 | (none) | none |  | declared | T002: no high-confidence capability signal detected |

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 2 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |
| accepted [SEH] synthetic | 0 |
| unaccepted synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Synthetic row fixture task one"]:::done
  T002["T002 Synthetic row fixture task two"]:::done
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Synthetic row fixture task one
T002 [X] Synthetic row fixture task two
```

