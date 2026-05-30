# Quickstart: Task Validator Feedback Follow-ups

Run checks from the repository root. FAKE-backed commands share `.fake` state and must run sequentially.

## Focused Validation

1. Add failing-first governance tests for:
   - mandated readiness filenames that contain trigger-like substrings
   - readiness-notes prefix suppression
   - all enforced trigger groups documented in task guidance
   - declared skill id versus directory-name diagnostics
   - advisory FS.Skia.UI capability guidance
   - graph-only output labeling

2. Run the focused governance tests with the existing non-FAKE test command for the affected test project when available, or use the narrow FAKE target if that is the repository-supported path.

3. Run a direct validator fixture for graph behavior:

   ```bash
   python3 .specify/extensions/evidence/scripts/python/compute-task-graph.py specs/033-fix-task-validator-feedback
   ```

4. Refresh readiness evidence under:

   ```text
   specs/033-fix-task-validator-feedback/readiness/title-trigger-validation.md
   specs/033-fix-task-validator-feedback/readiness/task-guidance-scan.md
   specs/033-fix-task-validator-feedback/readiness/skill-registry-diagnostics.md
   specs/033-fix-task-validator-feedback/readiness/advisory-capability-guidance.md
   specs/033-fix-task-validator-feedback/readiness/graph-only-output-label.md
   ```

## Sequential FAKE Validation

Use the deterministic order when more than one FAKE-backed target is needed:

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

If a failure looks race-like or concurrent FAKE context is unknown, rerun the affected FAKE target sequentially before debugging product behavior.

## Acceptance Evidence

- `title-trigger-validation.md`: shows setup/readiness filename references no longer require unrelated implementation skills.
- `task-guidance-scan.md`: shows readiness-notes prefix, enforced trigger groups, and at least three safe setup-title examples.
- `skill-registry-diagnostics.md`: shows a directory/id mismatch points authors to the accepted declared skill id.
- `advisory-capability-guidance.md`: shows at least five common FS.Skia.UI task categories are covered without becoming hard validation failures.
- `graph-only-output-label.md`: shows graph-only output is labeled as graph validation and does not imply evidence audit execution.
