# Quickstart — Foundations Library Validators (041)

The extraction recipe, in dependency order. FAKE-backed steps run **sequentially** (never
concurrent); safe non-FAKE reads/builds may parallelize.

## 0. Capture the parity oracle FIRST (R1)

Before touching any logic, run the three targets on the pinned pre-extraction baseline and commit
their outputs as golden fixtures — this freezes the oracle against a known-good snapshot.

```sh
./fake.sh build -t CapabilityCheck        # → readiness/capability-catalog.md
./fake.sh build -t TargetMetadata         # → readiness/target-metadata.json
./fake.sh build -t TargetMetadataDrift    # → readiness/target-metadata-drift.md
```

Copy each into `tests/Governance.Tests/fixtures/reports-golden/`. For `target-metadata.json`, note
the captured `generated_at_utc` value (the parity test normalizes that one line — R2).

## 1. FSI-sketch the surface (Principle I)

The four `.fsi` contracts in [contracts/](./contracts/) are the drafted surface. Exercise them in FSI
or as failing-first Governance.Tests (they will not compile until the `.fs` bodies exist).

## 2. Build the library modules (Principle II — `.fsi` before `.fs`)

Add to `build/Governance/FS.Skia.UI.Build.fsproj` `<Compile>`, in order:
`Findings.fsi/.fs`, `Targets.fsi/.fs`, `TargetMetadata.fsi/.fs`, `Capabilities.fsi/.fs`. No access
modifiers in `.fs`. Then:

```sh
dotnet build build/Governance/FS.Skia.UI.Build.fsproj   # clean under TreatWarningsAsErrors (FR-010)
```

## 3. Wire the build in-process (FR-005, FR-001)

- `#load` the four new `.fs` into `build.fsx` (alongside the existing SkillSync/SkillExamples loads).
- Convert **all** `StartTarget "..."` arms to dispatch on `Targets.Target` (FR-001); derive
  `requiredTargets`/`targetDependencyRows`/`directPrerequisites`/metadata from `Targets.spec`.
- Rewrite the `CapabilityCheck`, `TargetMetadata`, `TargetMetadataDrift` interpret cases to call
  `FS.Skia.UI.Build.Capabilities.*` / `TargetMetadata.*`, passing edge-read inputs
  (contract/docs references, surface-baseline existence, `DateTimeOffset.UtcNow`).
- Delete the bespoke `readCapabilityCatalog` parser and the moved validators (SC-005).

## 4. Add the tests (Principle VI; SC-004, FR-008a)

In `tests/Governance.Tests/` (add to `.fsproj` `<Compile>`):
- `TargetMetadataTests.fs` — ≥3 cases asserting `TargetMetadataDrift` typed cases.
- `CapabilityCatalogTests.fs` — ≥3 cases asserting `ValidationFinding` rule ids for catalog errors.
- `ReportParityTests.fs` — byte-equality of the 3 rendered reports vs `fixtures/reports-golden/`
  (metadata: all lines except the `generated_at_utc` value).

## 5. Verify — parity + serialized gates (SC-002/006/007)

```sh
./fake.sh build -t Dev                       # runs Governance.Tests incl. parity + typed-finding tests
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Plus: `PackageSurfaceCheck` shows no baseline diff; `git diff src/**` is empty (SC-007);
record the `build.fsx` line-count delta (`wc -l build.fsx` before/after; ≥800 fewer, SC-001).

## Done when

- 3 reports byte-identical to fixtures (parity diff = 0). 
- ≥6 typed-finding tests pass; bespoke parser grep returns nothing.
- `build.fsx` ≥800 lines smaller; serialized gates green (modulo documented 039 env flakes);
  `src/**` untouched; no new `PackageVersion`.
