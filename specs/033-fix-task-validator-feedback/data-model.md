# Data Model: Task Validator Feedback Follow-ups

## Task Title Signal

- **Fields**: `task_id`, `title`, `normalized_tokens`, `matched_group`, `matched_terms`, `confidence`, `source_span`, `is_filename_context`
- **Relationships**: belongs to a parsed task; may produce zero or more skill match assessments.
- **Validation rules**: Trigger terms embedded only inside longer words or filenames are not high-confidence matches. Complete command names, exact phrases, and whole-word workflow terms may be high-confidence.

## Capability Expectation Rule

- **Fields**: `skill_id`, `group_name`, `blocking_terms`, `command_terms`, `phrase_terms`, `safe_filename_exclusions`, `description`
- **Relationships**: maps title signals to expected Spec Kit skill ids.
- **Validation rules**: Existing required groups remain graph validation, evidence audit, task generation, implementation loading, and constitution work. Advisory FS.Skia.UI hints are not blocking rules.

## Skill Registry Entry

- **Fields**: `declared_skill_id`, `skill_path`, `directory_name`, `readable`, `source_root`
- **Relationships**: discovered from `.agents/skills`, `src/*/skill`, and `template/fragments/*/skill`.
- **Validation rules**: `declared_skill_id` comes from the `name:` field in `SKILL.md` when present; directory name is fallback only. Duplicate declared ids remain ambiguous and blocking.

## Skill Declaration

- **Fields**: `task_id`, `declared_skillist`, `visible_skillist_mirror`, `resolved_paths`, `diagnostics`
- **Relationships**: declared in `tasks.deps.yml` and mirrored in `tasks.md`.
- **Validation rules**: Structured and visible skill lists must match exactly and in order. Unregistered, unreadable, ambiguous, or incorrectly ordered skills remain blocking.

## Guidance Coverage Record

- **Fields**: `guidance_path`, `required_group`, `documented_terms`, `safe_examples`, `advisory_terms`, `scan_status`
- **Relationships**: one record per guidance file and enforced trigger group.
- **Validation rules**: All enforced Spec Kit trigger groups must appear in task-generation guidance. Advisory examples must be labeled non-blocking.

## Graph Validation Run

- **Fields**: `command`, `mode_label`, `feature_dir`, `exit_code`, `stdout`, `stderr`, `artifact_paths`, `next_action`
- **Relationships**: writes `readiness/task-graph.json`, `readiness/task-graph.md`, and feature readiness evidence.
- **Validation rules**: Graph-only output must label itself as graph validation and must not imply full evidence audit execution.

## Readiness Evidence

- **Fields**: `evidence_path`, `command`, `fixture_or_real_scan`, `status`, `observed_behavior`, `failure_classification`, `next_action`
- **Relationships**: supports one or more success criteria.
- **Validation rules**: Synthetic validator fixtures are acceptable for title and diagnostic edge cases; guidance coverage must scan real repository files.
