# Generated Validation Fixtures

SYNTHETIC FIXTURE: these malformed generated evidence packages are design-approved error-handling inputs for T006. They are not success evidence.

| Fixture | Purpose |
|---------|---------|
| `cycle` | Graph rejection for cyclic task dependencies. |
| `dangling` | Graph rejection for a dependency on a missing task id. |
| `missing-readiness` | Audit rejection for a graph-valid package with missing readiness contract evidence. |
| `skipped-authority` | Generated command rejection fixture for skipped authority or completion-only evidence claims. |
