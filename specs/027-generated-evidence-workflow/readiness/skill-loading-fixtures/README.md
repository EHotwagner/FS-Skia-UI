# Skill Loading Fixtures

SYNTHETIC FIXTURE: these malformed skill-loading evidence packages are design-approved error-handling inputs for T008. They are not success evidence.

| Fixture | Malformed class |
|---------|-----------------|
| `collapsed-range` | One row attempts to cover `T001-T002`. |
| `multi-skill-prose` | One row attempts to cover two skills in prose. |
| `duplicate` | Duplicate rows attempt to mask an incomplete required pairing. |
| `late` | `loaded_at` is after `work_started_at`. |
| `equal` | `loaded_at` equals `work_started_at`. |
