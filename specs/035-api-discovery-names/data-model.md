# Data Model: Package API Discovery And Name Safety

## PackageApiReference

- **Fields**: `packageId`, `version`, `sourceSignaturePaths`,
  `referencePath`, `packageEntryPath`, `generatedAt`, `symbolCount`,
  `exampleCount`, `omittedSymbols`, `diagnostics`
- **Relationships**: Owns many `ApiSymbol` records and links to one package
  surface baseline.
- **Validation**: `packageId`, `version`, and `referencePath` are required.
  `sourceSignaturePaths` must point at `.fsi` files. Omitted symbols require a
  reason.

## ApiSymbol

- **Fields**: `packageId`, `namespaceOrModule`, `kind`, `sourceName`,
  `qualifiedName`, `signature`, `parameters`, `returnShape`, `recordFields`,
  `unionCases`, `xmlSummary`, `examples`
- **Relationships**: Belongs to `PackageApiReference`; may appear in one or
  more `NameCollisionFinding` records.
- **Validation**: `sourceName` must preserve F# authoring spelling. Union cases
  and record fields must be represented as source authoring names. Public
  values/functions require parameter names when present in `.fsi`.

## NameCollisionFinding

- **Fields**: `collisionName`, `symbolKinds`, `ownerPackages`,
  `ownerNamespaces`, `exampleSurfaces`, `riskLevel`, `observedFailure`,
  `decisionId`
- **Relationships**: References two or more `ApiSymbol` records and exactly
  one `QualificationDecision`.
- **Validation**: Every P1 collision-prone group must have a decision. A
  finding cannot be closed by namespace open order alone.

## QualificationDecision

- **Fields**: `decisionId`, `collisionName`, `decisionType`,
  `contractChangeRequired`, `fsiPaths`, `surfaceBaselinePaths`,
  `guidancePaths`, `compatibilityNotes`, `status`
- **Relationships**: Resolves one or more `NameCollisionFinding` records.
- **State transitions**: `Proposed` -> `Validated` -> `Implemented` or
  `DeferredWithGuidance`.
- **Validation**: Contract changes require `.fsi`, tests, implementation, and
  baseline updates. Guidance-only decisions require explicit qualification in
  generated examples.

## GeneratedGuidanceRule

- **Fields**: `ruleId`, `audience`, `guidancePath`, `requiredTerms`,
  `forbiddenTerms`, `exampleSnippet`, `validationCommand`
- **Relationships**: May reference `PackageApiReference` and
  `QualificationDecision` records.
- **Validation**: Required terms must include discovery source, no-reflection
  guidance, and Scene/Controls qualification rule.

## ConsumerValidationScenario

- **Fields**: `scenarioId`, `projectPath`, `packageFeed`, `packages`,
  `sourceFiles`, `openOrdersTested`, `commands`, `logPaths`,
  `reflectionUsageDetected`, `repositorySourceUsageDetected`, `result`
- **Relationships**: Produces `generated-consumer-validation.md` readiness
  evidence and validates one or more guidance rules.
- **Validation**: Must restore from package feed, compile successfully, and
  report no reflection/source-copy authoring path for positive scenarios.

## FeedbackClassificationRecord

- **Fields**: `recordId`, `reportedFinding`, `category`, `owner`,
  `publicContractChange`, `generatedGuidanceChange`, `runtimeChange`,
  `evidencePath`, `nextAction`, `classifiedAt`
- **Allowed categories**: `PackageDocumentationDiscoverability`,
  `PublicContractErgonomics`, `GeneratedTemplateWorkflow`,
  `ConsumerAuthoringGuidance`
- **Validation**: Exactly one primary category is required. Runtime changes
  must be `false` unless the finding identifies behavior outside this feature's
  declared scope.
