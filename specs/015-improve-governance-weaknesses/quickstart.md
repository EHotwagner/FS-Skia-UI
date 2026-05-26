# Quickstart: Improve Governance Weaknesses

1. Generate tasks for this feature with `/speckit.tasks`.

2. Validate task metadata and skill matching:

   ```bash
   .specify/extensions/evidence/scripts/bash/run-audit.sh specs/015-improve-governance-weaknesses --graph-only
   ```

3. Confirm implementation evidence records are written before completing any task with non-empty `skillist`:

   ```text
   specs/015-improve-governance-weaknesses/readiness/skill-loading-evidence.md
   ```

4. Run focused governance checks for the changed area:

   ```bash
   ./fake.sh build -t EvidenceGraph
   ./fake.sh build -t EvidenceAudit
   ./fake.sh build -t GeneratedGuidanceCheck
   ```

5. If broad validation is required by the selected risk level, run `Dev` with the timeout guidance from the feature readiness notes:

   ```bash
   ./fake.sh build -t Dev
   ```

6. When an aggregate hang occurs, record the timeout verdict and run the recommended focused rerun before classifying readiness:

   ```text
   specs/015-improve-governance-weaknesses/readiness/aggregate-hang-diagnostics.md
   ```

7. Before final readiness, verify runtime limitation notes do not claim new platform, renderer, fallback, dependency, or package support:

   ```text
   specs/015-improve-governance-weaknesses/readiness/runtime-limitations.md
   ```
