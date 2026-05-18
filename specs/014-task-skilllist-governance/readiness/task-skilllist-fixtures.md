# Task Skilllist Validation Fixtures

These fixtures exercise the production `compute-task-graph.py` readiness path.

| Fixture | Expected verdict | Purpose |
|---------|------------------|---------|
| `valid` | pass | Explicit empty and non-empty `skillist` values with matching mirrors |
| `missing-skillist` | fail | Missing structured `skillist` field |
| `non-list-skillist` | fail | Non-list structured `skillist` value |
| `missing-mirror` | fail | Missing `tasks.md` mirror |
| `mirror-mismatch` | fail | Structured and visible values differ |
| `omitted-obvious` | fail | Obvious task-generation skill omitted |
| `invalid-order` | fail | Dependent audit skill listed before graph prerequisite |
| `legacy-bare-list` | fail | Existing bare-list metadata requires migration |
| `missing-skill` | fail | Declared skill cannot be resolved |
