# Quickstart: Working Screenshot Taking

## Planning Scope

This feature is complete only when supported-host screenshot capture produces a
real PNG artifact from working viewer/generated-app code and evidence audit
rejects metadata-only, blank, synthetic, or untraceable substitutes.

## Implementation Order

1. Update `src/SkiaViewer/SkiaViewer.fsi` with any additive screenshot capture
   result, capture mode, artifact validation, or blocked-stage contract changes.
2. Add failing semantic tests in `tests/SkiaViewer.Tests` for:
   - accepted first-frame screenshot result with live capture source
   - positive dimensions and non-blank artifact validation
   - launch/render/capture/write failure diagnostics
   - unsupported host reporting without success claims
3. Update `src/Testing/Testing.fsi` with screenshot record/artifact validator
   changes.
4. Add failing semantic tests in `tests/Testing.Tests` for:
   - accepted screenshot evidence records
   - missing/unreadable/zero-dimension/blank artifact rejection
   - deterministic scene, metadata, manual, and synthetic substitute rejection
5. Implement the SkiaViewer capture interpreter and PNG write/validation path.
6. Wire generated product `--screenshot-evidence` commands and guidance.
7. Refresh surface baselines and FSI transcripts.
8. Run generated guidance, template, graph, audit, and readiness checks.

## Local Verification Commands

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
dotnet test tests/Testing.Tests/Testing.Tests.fsproj
dotnet test tests/Governance.Tests/Governance.Tests.fsproj
./fake.sh build -t FsiTranscripts
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
```

## Required Readiness Evidence

Record final evidence under `specs/026-working-screenshot-taking/readiness/`:

- `screenshot-capture-evidence.md`: command, host facts, result fields, and
  supported-host outcome
- `screenshot-artifacts.md`: PNG artifact path, decoded dimensions, content
  validation, and reviewer notes
- `capture-failure-diagnostics.md`: unsupported/failure cases and blocked
  stages
- `generated-guidance.md`: generated command and documentation validation
- `package-surface-baseline.md`: public surface and baseline updates
- `evidence-graph.md`: task graph output
- `evidence-audit.md`: final audit result

## Acceptance Checklist

- At least one supported-host run produces a readable, non-blank PNG artifact.
- The screenshot evidence record includes every required traceability field.
- Unsupported-host or failure runs explain the blocked stage and do not claim
  screenshot proof.
- Existing launch, layout, scene, and pixel-readback evidence remain separate.
- Synthetic fixtures are used only for rejection/error-path tests and are
  disclosed as `[SEH]` during task generation.
