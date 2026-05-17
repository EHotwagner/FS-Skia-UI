# Quickstart: Fix Refactor Process Reliability

This quickstart describes the validation workflow expected after
implementation of the process reliability follow-up.

## 1. Validate Runner Bootstrap And Process Health

```bash
./fake.sh build -t VerifyPreflight
./fake.sh build -t Dev
./fake.sh build -t Verify
```

Expected outcome:

- broad verification records process-health evidence before high-pressure work
  starts
- missing runner dependencies fail bootstrap with an environment-failure
  verdict
- explicit threshold overrides report rule id, default, override value, and
  reason
- repeated runner warnings are separated from target failures

Threshold overrides are environment variables with matching reason variables:

```bash
FS_SKIA_PROCESS_MAX_ZOMBIE_COUNT=4096 \
FS_SKIA_PROCESS_MAX_ZOMBIE_COUNT_REASON="fresh CI image has known transient zombies" \
./fake.sh build -t VerifyPreflight
```

Every override report must include the rule id, repository default, override
value, override source, and human-readable reason. A malformed override or an
override without a reason is an `environment-failure`.

## 2. Validate Focused Gates

Run the focused gates directly:

```bash
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t FsiTranscripts
./fake.sh build -t ControlsCatalogCheck
./fake.sh build -t ControlsInteractionCheck
./fake.sh build -t ControlsRenderingCheck
./fake.sh build -t DependencyReport
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Expected outcome:

- each focused gate can run without invoking `Verify` or `Ci`
- any direct prerequisite is documented and visible in command-contract tests
- each gate writes its own log and verdict
- stale build or restore assumptions name the affected gate and remediation

## 3. Validate Scanner Accuracy

```bash
dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --no-restore
./fake.sh build -t DependencyReport
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedGuidanceCheck
```

Expected outcome:

- dependency scanner uses project XML or anchored dependency syntax rather
  than arbitrary substring matching
- generated product scanners allow intended `sample-pack` content while
  rejecting copied framework content in ordinary profiles
- generated inventories include source and tests for claimed public guidance
- stale active ownership references are reported before final audit completion

## 4. Validate Broad Verdict Semantics

```bash
./fake.sh build -t Verify
./fake.sh build -t Ci
```

Expected outcome:

- healthy broad runs report `success`
- product or governance check failures report `product-failure`
- preflight, bootstrap, CoreCLR, VSTest, socket/thread, or process exhaustion
  failures report `environment-failure`
- environment failures fail the aggregate but are marked non-authoritative for
  product behavior unless product checks actually ran and failed

If `Verify` or `Ci` reports `environment-failure`, do not treat focused passing
evidence as final product proof. Re-run the broad aggregate in one of:

- a fresh shell after clearing stale local FAKE restore markers
- a fresh local container
- a CI runner with a clean process table and package cache

## 5. Validate Readiness Evidence

Expected readiness evidence under
`specs/012-fix-refactor-process/readiness/`:

- `process-health.md`
- `focused-gates.md`
- `governance-scanners.md`
- `stale-boundary-scan.md`
- `generated-product-validation.md`
- `bootstrap-runner.md`
- `verification-verdicts.md`
- `evidence-graph.md`
- `evidence-audit.md`

Final readiness must clearly state whether broad aggregate evidence is
authoritative, failed because of environment conditions, or is waiting for a
fresh healthy broad pass. A previous broad aggregate environment failure keeps
final readiness blocked until a later healthy broad aggregate pass is recorded.
