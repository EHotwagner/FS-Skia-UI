# Quickstart: Targeted Refactor and Governance Diagnostics

Run commands from the repository root.

## Baseline Before Implementation

```bash
./fake.sh target Dev
./fake.sh target Verify
```

Capture baseline output under:

- `specs/008-targeted-refactor-governance/readiness/public-surface.txt`
- `specs/008-targeted-refactor-governance/readiness/semantic-tests.txt`

## Focused Verification During Implementation

```bash
dotnet test tests/Lib.Tests/Lib.Tests.fsproj --no-restore
dotnet test tests/Layout.Tests/Layout.Tests.fsproj --no-restore
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore
dotnet test tests/Package.Tests/Package.Tests.fsproj --no-restore
```

Use the focused tests for native cleanup, Yoga fallback diagnostics, generated guidance, template drift, command contract, public record invariant inventory, and surface stability.

## Build Organization Acceptance

After any physical `build.fsx` split attempt, prove the documented targets load:

```bash
./fake.sh target Dev
./fake.sh target Verify
./fake.sh target Ci
```

If any target fails to load because of the split, revert to one canonical `build.fsx` with named concern sections and record fallback evidence in:

```text
specs/008-targeted-refactor-governance/readiness/build-organization.md
```

## Governance Checks

```bash
./fake.sh target GeneratedGuidanceCheck
./fake.sh target TemplateDrift
```

Expected reports:

- `specs/008-targeted-refactor-governance/readiness/generated-guidance.md`
- `specs/008-targeted-refactor-governance/readiness/template-drift.md`

## Native and Layout Evidence

Record deterministic startup failure cleanup evidence in:

```text
specs/008-targeted-refactor-governance/readiness/native-startup-cleanup-tests.txt
specs/008-targeted-refactor-governance/readiness/native-startup-cleanup.md
```

Record real native smoke evidence, or explicit unsupported-environment diagnostics, in:

```text
specs/008-targeted-refactor-governance/readiness/native-smoke.txt
```

Record Yoga fallback diagnostics evidence or follow-up API blocker evidence in:

```text
specs/008-targeted-refactor-governance/readiness/yoga-fallback-diagnostics.txt
specs/008-targeted-refactor-governance/readiness/follow-ups.md
```

## Final Verification

```bash
./fake.sh target Ci
```

The feature is not merge-ready unless required readiness artifacts are real or explicitly disclosed under repository synthetic evidence policy, public surface baselines remain stable, and every public record appears in:

```text
specs/008-targeted-refactor-governance/readiness/record-invariants.md
```
