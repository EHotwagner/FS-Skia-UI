# Contract: Skill-Match Assessment

## Record Shape

```yaml
skill_match_assessments:
  - task_id: T009
    declared_skillist: [speckit-evidence-graph]
    candidate_skill_id: speckit-evidence-graph
    matched_signals: [task-text, command-name, skill-description]
    confidence: high
    ambiguity: null
    reviewer_disposition: accepted
    diagnostic: "Task updates EvidenceGraph validation."
```

## Required Behavior

- Detection reports confidence and signals instead of treating regex matches as proof.
- High-confidence omitted skills block readiness.
- Medium, low, indirect, and ambiguous matches require reviewer disposition.
- Valid empty `skillist` entries remain valid only when no high-confidence capability signal exists, or when reviewer disposition explains why the signal is not applicable.

## Calibration Cases

Readiness examples must cover:

- Obvious match accepted.
- Ambiguous match requiring reviewer choice.
- Indirect capability ownership match.
- False positive rejected by reviewer.
- Valid empty skill list.
