# Skill-Loading Evidence Workflow — 059-speckit-tasks-validation-feedback

This note records the authoritative skill-loading evidence contract for the
feature's implementation phase.

- **Authoritative command**: `./fake.sh build -t EvidenceAudit` validates the
  per-task skill-loading evidence rows (`readiness/skill-loading-evidence.md`).
- **Artifact path**: `specs/059-speckit-tasks-validation-feedback/readiness/skill-loading-evidence.md`.
- **Contract**: each Done/Synthetic task with a non-empty `skillist` requires one
  pre-work load row per declared skill id, with task id, skill id, resolved
  `SKILL.md` path, load result, `loaded_at`, `work_started_at` (ISO-8601,
  `loaded_at` earlier than `work_started_at`), evidence path, and reviewer
  exception. Duplicate rows do not mask missing required rows.
- **Failure class**: a missing, late-loaded, ambiguous, or unreadable skill row
  blocks the task and is reported with the task id plus unresolved skill id.
- **Next action**: record load rows as tasks complete; re-run
  `./fake.sh build -t EvidenceGraph` after every status change to refresh `[S*]`
  propagation.
