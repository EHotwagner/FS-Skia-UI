# Quickstart: Asteroids Feedback Skill Guidance

Run checks from the repository root. FAKE-backed commands share `.fake` state and must run sequentially.

## Focused Validation

1. Add failing-first governance or generated-guidance tests for:
   - specialized skill assignment patterns for generated visual demo tasks
   - multiple skills on one task with matching `tasks.deps.yml` and `tasks.md` mirrors
   - readiness scaffold enumeration for audit-required visual demo files
   - expected readiness fields for real-image, window, governance, aggregate hang, runtime limitation, and generated validation evidence
   - rejection wording for metadata-only screenshots, layout-only bounds proof, and fallback or placeholder images
   - feedback owner classification for framework, template/evidence workflow, documentation/discoverability, and consumer-authoring findings
   - XML documentation coverage for public `.fsi` members in packable framework packages
   - generated XML documentation files and packed NuGet XML documentation entries for each packable framework package
   - advisory FS.Skia.UI guidance remaining non-blocking for otherwise valid task lists

2. Run the narrow non-FAKE test command for the affected governance project when available. If repository-supported validation requires FAKE, use the sequential order below.

3. Refresh readiness evidence under:

   ```text
   specs/034-asteroids-feedback-skills/readiness/skill-assignment-guidance.md
   specs/034-asteroids-feedback-skills/readiness/readiness-scaffold-coverage.md
   specs/034-asteroids-feedback-skills/readiness/visual-evidence-honesty.md
   specs/034-asteroids-feedback-skills/readiness/feedback-classification.md
   specs/034-asteroids-feedback-skills/readiness/generated-guidance-validation.md
   specs/034-asteroids-feedback-skills/readiness/xml-documentation-validation.md
   ```

## Sequential FAKE Validation

Use this deterministic order when more than one FAKE-backed target is needed:

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t PackLocal
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

If a failure looks race-like or concurrent FAKE context is unknown, rerun the affected FAKE target sequentially before debugging product behavior.

## Acceptance Evidence

- `skill-assignment-guidance.md`: shows generated visual demo tasks list applicable implementation, layout evidence, graph, audit, template, generated validation, and debug-loop skills or a no-skill rationale.
- `readiness-scaffold-coverage.md`: shows every audit-required visual-demo readiness file and required field cue is discoverable from generated tasks or guidance.
- `visual-evidence-honesty.md`: shows screenshot, scene image, fallback image, and layout evidence claims are classified without overstating proof.
- `feedback-classification.md`: classifies at least four framework-attributable findings and at least three non-framework findings from the Asteroids feedback report.
- `generated-guidance-validation.md`: shows generated guidance exposes the skill assignment and readiness scaffold guidance before `/speckit-implement` begins.
- `xml-documentation-validation.md`: shows public `.fsi` XML docs are complete, generated XML files are non-empty, and packed NuGet artifacts include the corresponding XML documentation files.
