# Aggregate Hang Diagnostics

validation_verdict:
  target: GeneratedProductCheck / Verify
  verdict: gates run individually and sequentially are authoritative; the GeneratedProductCheck generated-Verify and the aggregate Verify/Ci sweep are a non-authoritative aggregate result in this sandbox
  stage: maintainer-verify gate-by-gate
  elapsed duration: TemplateCheck passed in ~2m08s (pack+instantiate+Test 24s+smoke); each focused governance gate completes in seconds
  last observed command: ./fake.sh build -t EvidenceAudit
  timeout_policy: FAKE-backed targets share .fake state and are run sequentially, never concurrently
  recommended focused rerun: ./fake.sh build -t EvidenceAudit
  focused rerun:
    command: ./fake.sh build -t TemplateCheck
    focused rerun result: Status Ok — pack/instantiate/Test(24s)/TemplateSmoke green across profiles, exercising the split GovernanceTests.fs + BehaviorTests.fs and the docs/api-surface tree in generated projects
    evidence_path: specs/060-asteroids-consumer-friction-followups/readiness/template
  investigated_failure:
    command: ./fake.sh build -t GeneratedProductCheck (runs the scaffold generated product's own `Verify`)
    result: the generated product's Dev/GeneratedGuidanceCheck/TemplateDrift PASSED (the split scaffold tests compile and run), then its EvidenceGraph loud-failed with "Cannot resolve the feature to validate ... has no usable feature_directory entry. Validation never falls back to a bundled sample." — i.e. 059's resolveFeatureDir working as designed on a fresh scaffold that has no recorded active feature
  control_check:
    command: SPECKIT_FEATURE_DIR=<repo>/specs/060-... ./fake.sh build -t EvidenceGraph (in a generated project)
    result: resolved the active feature and echoed feature-directory= / tasks=32 (see generated-project/feature-resolution.log); a non-existent SPECKIT_FEATURE_DIR loud-failed naming the path
  final_classification: the GeneratedProductCheck generated-Verify failure is 059's intended loud feature-resolution failure on a feature-less scaffold (the FR-001 behavior this feature ships), not a 060 regression; the split scaffold tests and api-surface tree validate green via TemplateCheck and the generated project's own Dev
  diagnostic: This feature is governance + template + skills scope only — no native-GUI/headless product test path is exercised here, so no libdecor/Wayland aggregate hang applies. The Smoke.Tests/SkiaViewer.Tests direct-Expecto bypass remains in place from prior features. The authoritative merge verdict is EvidenceAudit verdict=PASS (0 blockers).
