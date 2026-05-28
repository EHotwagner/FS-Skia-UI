# Data Model: Upgrade SkiaSharp And Spec Kit

## Version Upgrade Decision

Represents the approved version movement for one governed dependency or asset
family.

- **Fields**: `name`, `currentVersion`, `targetVersion`, `sourceOfTruth`,
  `checkedAt`, `affectedFiles`, `approvalRationale`, `riskNotes`,
  `evidencePath`
- **Relationships**: Drives Dependency Evidence Report, Template Alignment
  Evidence, and Release Policy Note.
- **Validation**: Must identify the source checked immediately before
  implementation, list every repository-owned file requiring an update, and
  record why the selected version is accepted.

## SkiaSharp Package Family

The aligned package set used by renderer, viewer, and native asset consumers.

- **Fields**: `managedPackage`, `linuxNativeAssetPackage`,
  `win32NativeAssetPackage`, `versionFamily`, `packageDeclarations`,
  `validationCommands`, `hostEvidence`
- **Relationships**: Belongs to one Version Upgrade Decision and appears in the
  Dependency Evidence Report.
- **Validation**: All package declarations must share the same approved version
  family unless a documented official compatibility exception exists.

## Spec Kit Asset Set

The repository-owned Spec Kit metadata, templates, extensions, workflows, and
generated product copies.

- **Fields**: `rootMetadataFiles`, `extensionFiles`, `presetFiles`,
  `templateFiles`, `workflowFiles`, `generatedCopies`, `skillFiles`,
  `versionOrCompatibilityRange`, `alignmentEvidencePath`
- **Relationships**: Belongs to one Version Upgrade Decision and produces
  Template Alignment Evidence.
- **Validation**: Root and generated assets must agree on version or
  compatibility range, and any deliberate mismatch must include a reason and
  generated-user impact statement.

## Compatibility Consumer

One repository-owned usage of the broad `FS.Skia.UI` package.

- **Fields**: `path`, `consumerKind`, `usageKind`, `packageMode`,
  `focusedReplacement`, `migrationStatus`, `notes`
- **Relationships**: Included in Compatibility Consumer Inventory and linked to
  one or more Public Surface Classification entries.
- **Validation**: Inventory must cover project references, package references,
  namespace opens, samples, templates, documentation, and packaged-mode usage.

## Public Surface Classification

A classified compatibility-package public area.

- **Fields**: `symbolOrArea`, `classification`, `focusedEquivalent`,
  `compatibilityOwner`, `surfaceBaselineStatus`, `migrationGuidance`,
  `releasePolicyImpact`
- **Classification values**: `primary-only compatibility member`, `duplicate
  of focused package concept`, `facade candidate`, `deprecated candidate`,
  `permanent compatibility surface`
- **Relationships**: Appears in Compatibility Public Surface Map and Package
  Surface Baseline evidence.
- **Validation**: Every public compatibility area touched or reviewed by this
  feature must have one classification and an explicit replacement/gap status.

## Dependency Evidence Report

Before/after package graph proof for dependency governance.

- **Fields**: `path`, `producerCommand`, `beforeVersions`, `afterVersions`,
  `dependencyClosure`, `cycleStatus`, `unexpectedSpread`, `reviewStatus`
- **Relationships**: Consumes Version Upgrade Decisions and SkiaSharp Package
  Family.
- **Validation**: Must show no accidental package cycles and no unexplained
  dependency spread from focused packages into the compatibility package.

## Template Alignment Evidence

Proof that generated projects and template package metadata match the approved
versions and package posture.

- **Fields**: `path`, `profilesChecked`, `packagePins`, `specKitAssets`,
  `selectedSkills`, `broadPackageDependencyStatus`, `validationCommands`,
  `reviewStatus`
- **Relationships**: Consumes Spec Kit Asset Set and Compatibility Consumer
  Inventory.
- **Validation**: Supported generated profiles must validate and must not gain a
  broad `FS.Skia.UI` dependency unless deliberately selected.

## Release Policy Note

User-facing summary of upgrade and compatibility posture.

- **Fields**: `path`, `versionChanges`, `compatibilityPosture`,
  `migrationWindow`, `deferredDecisions`, `knownRisks`, `userGuidance`
- **Relationships**: Consumes Version Upgrade Decisions, Public Surface
  Classification, and Focused Replacement Map.
- **Validation**: Must clearly tell new users which package path to choose and
  tell existing broad-package users what remains stable.
