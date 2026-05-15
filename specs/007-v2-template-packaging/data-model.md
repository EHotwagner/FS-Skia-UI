# Data Model: Template Packaging and Drift Governance

## TemplateProfile

Represents a supported generated project profile.

**Fields**:
- `profileId`: stable identifier, currently `default` or `minimal`
- `displayName`: human-readable name
- `description`: intended product starting point
- `generationOptions`: symbols accepted by `dotnet new`
- `templateOwnedPaths`: files and directories governed by the template
- `productOwnedPaths`: files and directories expected to diverge after instantiation
- `includedPaths`: path globs included for this profile
- `excludedPaths`: path globs excluded for this profile
- `excludedHistory`: historical feature/evidence paths that must not be distributed
- `validationTargets`: FAKE targets expected to pass in generated output

**Validation Rules**:
- `profileId` must be unique.
- `default` and `minimal` must always exist.
- `minimal` must include core library, one basic sample, core tests, package checks, docs, and Spec Kit governance assets.
- `minimal` must exclude optional layout, charts, parity, and visual sample scope.
- Historical source feature directories must be excluded from generated projects except a minimal template-profile example if explicitly added.

## TemplateArtifact

Represents an installable template source.

**Fields**:
- `artifactKind`: `source-directory` or `local-package`
- `sourcePath`: repository root for source install or package path for local package install
- `packageId`: template package identity for packaged artifact
- `version`: local template package version when packaged
- `installCommand`: command used to install the artifact
- `evidencePath`: log path recording installation result

**Validation Rules**:
- V2 validation must exercise both artifact kinds.
- `local-package` must resolve to a `.nupkg` under `artifacts/templates/`.
- Failed install/uninstall steps must produce actionable diagnostics.

## GeneratedProject

Represents a fresh project created from a template artifact and profile.

**Fields**:
- `projectName`: generated project directory/name
- `profileId`: selected template profile
- `artifactKind`: artifact used to create it
- `outputPath`: isolated generated project location
- `rootNamespace`: generated root namespace
- `packagePrefix`: generated package prefix
- `placeholderScan`: placeholder scan result
- `excludedHistoryScan`: excluded-history scan result
- `devVerification`: generated `./fake.sh build -t Dev` result

**Validation Rules**:
- Project name, package name, author, repository, and namespace placeholders must be replaced.
- Historical source readiness evidence and feature history must be absent.
- The generated `Dev` workflow must pass without manual edits.
- Optional profile choices must not leave broken references, orphaned tests, or docs for disabled features.

## TemplateValidationRun

Represents one full template validation execution.

**Fields**:
- `runId`: stable run label or timestamp
- `featureId`: `007-v2-template-packaging`
- `tempRoot`: isolated output root
- `artifacts`: source and local package artifacts under test
- `generatedProjects`: generated project results for every artifact/profile pair
- `placeholderReportPath`: readiness output for placeholder scan
- `excludedHistoryReportPath`: readiness output for excluded-history scan
- `generatedDevLogPaths`: generated project fast verification logs
- `verdictPath`: final pass/fail summary

**Validation Rules**:
- Must include source-directory/default, source-directory/minimal, local-package/default, and local-package/minimal generated projects.
- Must fail if any generated project lacks required artifact classes.
- Must clean only target-owned temporary output.

## DependencyGovernancePolicy

Represents the central dependency version and ownership policy.

**Fields**:
- `policyPath`: `Directory.Packages.props`
- `metadataPath`: `docs/dependencies.md`
- `dependencies`: list of governed dependency records
- `validationOnlyExceptions`: explicitly allowed inline/local package version cases
- `reportPath`: readiness dependency report

**Validation Rules**:
- Direct external dependency versions must be declared centrally.
- Project files must use versionless `PackageReference` entries unless covered by a validation-only exception.
- Every governed dependency must document purpose, owner, license posture, upgrade expectation, and preview-risk status when applicable.

## DependencyRecord

Represents one governed package.

**Fields**:
- `packageId`: NuGet package id
- `version`: centrally pinned version
- `usedBy`: projects that reference it
- `purpose`: why the package is needed
- `owner`: maintainer or responsibility group
- `licensePosture`: accepted license/status note
- `upgradeExpectation`: how and when upgrades are evaluated
- `previewRisk`: preview/stability note where relevant

**Validation Rules**:
- `packageId`, `version`, `purpose`, `owner`, `licensePosture`, and `upgradeExpectation` are required.
- Preview packages must have explicit `previewRisk`.
- Referenced packages missing metadata fail `DependencyReport`.

## GeneratedArtifactGuidance

Represents the prompts inherited by generated specifications and plans.

**Fields**:
- `artifactType`: `spec` or `plan`
- `templatePath`: active template path
- `requiredPrompts`: prompt names required by V2
- `validationPath`: readiness guidance check output

**Validation Rules**:
- Generated specs must ask about package impact, public contract impact, state workflow impact, layout/rendering impact, evidence obligations, unsupported scope, and build-target impact.
- Generated plans must require template ownership, dependency impact, command-surface impact, generated project impact, and evidence paths.
- Guidance must distinguish V2 obligations from deferred visual, release, and external distribution work.

## DriftReport

Represents template-owned changes requiring alignment.

**Fields**:
- `changedPaths`: template-owned paths changed in the working tree or diff under review
- `requiredAlignment`: template, docs, dependency policy, generated guidance, command-surface, or deferral action
- `acceptedDeferrals`: deferrals accepted for this run
- `missingActions`: unresolved drift items
- `reportPath`: `specs/007-v2-template-packaging/readiness/template-drift.md`
- `verdict`: pass or fail

**Validation Rules**:
- Template-owned source, docs, preset, dependency, sample, and command-surface changes must be covered.
- Missing alignment action fails verification with path-level diagnostics.
- Deferrals missing required fields are rejected.

## DeferralRecord

Represents a bounded exception for intentional source-only or future-roadmap drift.

**Fields**:
- `id`: stable deferral identifier
- `paths`: changed path globs covered by the deferral
- `rationale`: why the change is not reflected in the template yet
- `owner`: accountable maintainer
- `targetPhase`: phase or feature expected to resolve the deferral
- `created`: date recorded
- `trackingIssue`: optional issue or PR reference

**Validation Rules**:
- `id`, `paths`, `rationale`, `owner`, and `targetPhase` are required.
- A deferral only covers paths it names.
- Accepted deferrals must be included in readiness evidence.

## State Transitions

### TemplateValidationRun

```text
not-started
  -> source-installed
  -> source-generated
  -> source-validated
  -> package-built
  -> package-installed
  -> package-generated
  -> package-validated
  -> passed | failed
```

Any failed install, generation, scan, restore, build, test, or missing-artifact step transitions directly to `failed` and writes the failing command plus output path.

### DriftReport

```text
collected
  -> classified
  -> aligned | deferred | failed
```

`deferred` is accepted only when all required deferral fields are present and the covered paths match the changed paths.
