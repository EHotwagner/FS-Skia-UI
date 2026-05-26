# Data Model: Improve Governance Weaknesses

## Task Skill Evidence

Reviewer-visible record that declared skills were resolved and loaded before task work began.

- `task_id`: required task identifier, for example `T017`.
- `declared_skill_id`: required skill id from the task's structured `skillist`.
- `resolved_skill_path`: required repository-relative path to exactly one readable `SKILL.md`.
- `load_result`: required enum: `loaded`, `missing`, `unreadable`, `ambiguous`, `exception-approved`.
- `loaded_at`: required timestamp for successful loads.
- `work_started_at`: required timestamp or recorded task-work marker used to verify order.
- `evidence_path`: required repository-relative readiness artifact path.
- `exception`: optional reviewer exception record.

Validation rules:

- `loaded_at` must be before `work_started_at` for `load_result: loaded`.
- Missing, unreadable, ambiguous, or late evidence blocks task completion unless `exception` is present and complete.
- `exception` must name task, skill, reason, approving reviewer, and compensating evidence.

## Skill Match Assessment

Readiness result for determining whether a task's `skillist` is plausible.

- `task_id`: required task identifier.
- `declared_skillist`: required ordered list from `tasks.deps.yml`.
- `candidate_skill_id`: skill being assessed.
- `matched_signals`: required list, for example `task-text`, `file-path`, `command-name`, `capability-owner`, `skill-description`.
- `confidence`: required enum: `high`, `medium`, `low`, `none`.
- `ambiguity`: optional explanation when multiple skills or weak signals exist.
- `reviewer_disposition`: required for medium, low, and ambiguous results; enum: `accepted`, `rejected`, `deferred`.
- `diagnostic`: reviewer-facing message.

Validation rules:

- High-confidence missing skills block readiness.
- Medium, low, and ambiguous matches require reviewer disposition before implementation.
- A valid empty `skillist` must include a recorded reason when any capability-owned signal was seen.

## Governance Risk Level

Classification mapping change scope to the minimum evidence path.

- `level`: required enum: `small`, `medium`, `broad`.
- `scope_signals`: required list of changed areas and risk indicators.
- `required_checks`: required ordered list of focused or broad checks.
- `broad_required`: required boolean.
- `rationale`: required reviewer-readable reason.
- `non_authoritative_results`: optional list of aggregate results that should not decide product readiness alone.

Validation rules:

- Small changes may use focused governance checks when they do not affect runtime behavior, generated outputs, packages, public contracts, or command orchestration.
- Medium changes require affected focused targets plus evidence graph/audit when task/readiness state changes.
- Broad changes require aggregate validation or a recorded timeout/orchestration verdict plus focused rerun evidence.

## Validation Verdict

Outcome category for a focused or aggregate validation run.

- `target`: required command or stage name.
- `verdict`: required enum: `pass`, `product-failure`, `environment-failure`, `timeout`, `orchestration-concern`, `non-authoritative`.
- `stage`: optional stage name for aggregate runs.
- `elapsed_duration`: optional elapsed time for timeout or hang cases.
- `last_observed_command`: optional command captured before timeout.
- `focused_rerun`: optional focused rerun command and result.
- `diagnostic`: required actionable explanation.
- `evidence_path`: required readiness artifact path.

Validation rules:

- Timeout verdicts must include `stage`, `elapsed_duration`, `last_observed_command`, and focused rerun guidance.
- If a focused product check passes after an aggregate hang, the aggregate verdict must not be recorded as `product-failure` unless another product check failed.

## Runtime Limitation

Documented current product boundary.

- `category`: required enum: `platform`, `renderer`, `dependency`, `fallback`, `toolchain`.
- `current_scope`: required supported or tested scope.
- `unsupported_scope`: required unsupported platform, renderer, fallback, or maturity gap.
- `risk`: required reviewer-readable impact.
- `future_feature`: optional follow-up feature reference.

Validation rules:

- Runtime limitation notes must distinguish current support from future expansion.
- Limitation documentation must not claim new support introduced by this feature.
