# Quickstart: Package API Discovery And Name Safety

Run FAKE-backed commands sequentially; they share repository `.fake` state.

1. Build the current framework and tests:

   ```bash
   ./fake.sh build -t Dev
   ```

2. Pack local packages for generated consumers:

   ```bash
   ./fake.sh build -t PackLocal
   ```

3. Validate package surface/reference expectations:

   ```bash
   ./fake.sh build -t PackageSurfaceCheck
   ```

4. Run public FSI transcripts for source-shaped authoring examples:

   ```bash
   ./fake.sh build -t FsiTranscripts
   ```

5. Validate generated guidance for API discovery and qualification rules:

   ```bash
   ./fake.sh build -t GeneratedGuidanceCheck
   ```

6. Validate generated template and package consumer scenarios:

   ```bash
   ./fake.sh build -t TemplateCheck
   ./fake.sh build -t GeneratedProductCheck
   ```

7. Refresh Spec Kit evidence checks:

   ```bash
   ./fake.sh build -t EvidenceGraph
   ./fake.sh build -t EvidenceAudit
   ```

Expected readiness artifacts:

- `specs/035-api-discovery-names/readiness/api-discovery.md`
- `specs/035-api-discovery-names/readiness/name-collision-safety.md`
- `specs/035-api-discovery-names/readiness/generated-consumer-validation.md`
- `specs/035-api-discovery-names/readiness/feedback-classification.md`
- `specs/035-api-discovery-names/readiness/package-reference-material.md`
- `specs/035-api-discovery-names/readiness/package-surface-baseline.md`
- `specs/035-api-discovery-names/readiness/evidence-graph.md`
- `specs/035-api-discovery-names/readiness/evidence-audit.md`

Implementation order:

1. Add failing tests/scanners for source-shaped reference coverage and mixed
   Scene/Controls compilation.
2. Add or update `.fsi` contracts only for selected name-safety decisions.
3. Implement reference generation and package/guidance reporting.
4. Update generated guidance, docs, package surface baselines, and readiness
   evidence.
5. Run the sequential FAKE-backed validation order above.
