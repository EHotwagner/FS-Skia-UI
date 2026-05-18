# Generated Consumer Validation Readiness

## Scope

Readiness evidence for end-to-end generated consumer validation from fresh
local package output through restore, generated semantic tests, bounded real
viewer smoke where supported, deterministic scene evidence, and readiness
writing.

## Setup Notes

- Tier: Tier 1 contracted generated-consumer validation change.
- Affected areas: build targets, generated product checks, generated guidance,
  smoke tests, scene evidence, package reporting, and readiness logs.
- Command-surface impact: `PackLocal`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck`, `TemplateCheck`, `TemplateDrift`, `Verify`, and
  `Ci` may change.
- Synthetic policy: final generated consumer readiness cannot rely solely on
  synthetic fixtures; unsupported-host outcomes must be explicit diagnostics.

## Evidence

- Focused Testing helper tests:
  `readiness/logs/testing-us5-local-consumer-tests.txt`
  - Verifies generated consumer validation summaries preserve category,
    elapsed time, command context, evidence path, scene evidence diagnostics,
    and unsupported-host diagnostics.
- Real local package preparation:
  `readiness/logs/pack-local-us5.txt`
  - Fresh local packages were produced before generated consumer validation.
- Generated consumer validation:
  `readiness/logs/generated-product-us5-consumer-validation.txt`
  - `./fake.sh build -t GeneratedProductCheck` passed with generated-consumer
    validation enabled.
  - `readiness/generated-product-validation.md` records category
    `UnsupportedHost`, elapsed time `00:00:16.8930664`, local feed path,
    restore log, semantic test log, bounded smoke evidence, and scene evidence
    output.
  - The generated consumer restore and semantic tests passed from local package
    output.
  - Bounded real-viewer smoke returned explicit unsupported-host diagnostics in
    `readiness/generated-consumer-validation/bounded-smoke.txt`.
  - Deterministic scene evidence was captured in
    `readiness/generated-consumer-validation/headless-scene-evidence.txt`.

## Independent Validation

Run:

```bash
./fake.sh build -t PackLocal
./fake.sh build -t GeneratedProductCheck
dotnet run --project tests/Testing.Tests/Testing.Tests.fsproj
```

The validation summary must distinguish package drift, restore failure,
semantic test failure, viewer startup failure, unsupported host, scene evidence
failure, and completed outcomes. Unsupported desktop viewer smoke remains a
valid generated-consumer outcome only when the diagnostic is explicit and
scene-level evidence remains separate.

Current local result: generated consumer restore and semantic tests passed;
bounded real-viewer smoke is `UnsupportedHost` on this host, and deterministic
scene evidence was captured separately.

## Requirement Mapping

- FR-015 through FR-017: validation starts from fresh local package output and
  records package/feed setup.
- FR-019: generated validation summaries preserve category, command context,
  evidence path, and diagnostics.
- SC-010: generated product checks verify package-driven generated product
  surfaces after local package preparation.
- SC-011: unsupported-host diagnostics remain explicit while deterministic
  scene evidence is tracked separately.
