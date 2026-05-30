namespace FS.Skia.UI.AgentValidation

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationGate = string
/// Public contract type exposed by this FS.Skia.UI package.
type ValidationRuleId = string
/// Public contract type exposed by this FS.Skia.UI package.
type FeatureId = string
/// Public contract type exposed by this FS.Skia.UI package.
type ChangedPath = string

/// Public contract type exposed by this FS.Skia.UI package.
type ChangedPathSourceKind =
    | ActiveFeatureMetadata
    | GitMergeBaseDiff
    | Unavailable

/// Public contract type exposed by this FS.Skia.UI package.
type ChangedPathSource =
    { Kind: ChangedPathSourceKind
      Feature: FeatureId option
      MergeBase: string option
      Paths: ChangedPath list
      Diagnostics: string list }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationAuthority =
    | InnerLoop
    | FocusedAuthority
    | AgentReadyAuthority
    | MaintainerVerify
    | AutomationFinal

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationSelectionModel =
    { Feature: FeatureId option
      ChangedPathSource: ChangedPathSource option
      SelectedRuleIds: ValidationRuleId list
      RequiredGates: ValidationGate list
      Authority: ValidationAuthority
      Degraded: bool
      Diagnostics: string list }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationSelectionMsg =
    | LoadActiveFeatureMetadata
    | ActiveFeatureMetadataLoaded of FeatureId * ChangedPath list
    | ActiveFeatureMetadataUnavailable of string
    | LoadGitMergeBaseDiff
    | GitMergeBaseDiffLoaded of mergeBase: string * paths: ChangedPath list
    | GitMergeBaseDiffUnavailable of string
    | ContractLoaded of selectedRuleIds: ValidationRuleId list * requiredGates: ValidationGate list * authority: ValidationAuthority
    | SelectionFailed of string

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationSelectionEffect =
    | ReadActiveFeatureMetadata
    | RunGitMergeBaseDiff
    | LoadValidationContract
    | WriteValidationSelectionReport

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationSelectionInterpreterInputs =
    { RepositoryRoot: string
      FeatureMetadataPath: string
      ValidationContractPath: string
      SelectionReportPath: string
      BaseRef: string option }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationContractDefaults =
    { BroadFallbackCommand: string
      FinalGates: ValidationGate list }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationContractTier =
    { Id: string
      Authority: string
      DefaultGates: ValidationGate list }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationContractRule =
    { Id: ValidationRuleId
      Paths: string list
      FeatureConcerns: string list
      RequiredGates: ValidationGate list
      ExpectedArtifacts: string list
      TimeoutClass: string
      FailureOwner: string }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationContract =
    { SchemaVersion: int
      Defaults: ValidationContractDefaults
      Tiers: ValidationContractTier list
      RoutingRules: ValidationContractRule list }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationContractDiagnostic =
    { Code: string
      Path: string
      Message: string
      Owner: string }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationContractParseResult =
    | ValidationContractAccepted of ValidationContract
    | ValidationContractRejected of ValidationContractDiagnostic list

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationFailureOwner =
    | Product
    | Template
    | Governance
    | Environment
    | UnsupportedHost
    | StalePrerequisite
    | MissingEvidence
    | Unknown

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationFailureClass =
    | ProductFailure
    | TemplateFailure
    | GovernanceFailure
    | EnvironmentFailure
    | UnsupportedHostFailure
    | StalePrerequisiteFailure
    | MissingEvidenceFailure
    | UnknownFailure

/// Public contract type exposed by this FS.Skia.UI package.
type TimeoutClass =
    | Fast
    | Focused
    | Broad

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationCost =
    | Low
    | Medium
    | High

/// Public contract type exposed by this FS.Skia.UI package.
type TargetMetadata =
    { Name: ValidationGate
      Description: string
      Dependencies: ValidationGate list
      DirectPrerequisites: ValidationGate list
      ExpectedOutputs: string list
      StaleAssumptions: string list
      TimeoutClass: TimeoutClass
      Cost: ValidationCost
      Authority: ValidationAuthority
      DefaultFailureOwner: ValidationFailureOwner
      Command: string }

/// Public contract type exposed by this FS.Skia.UI package.
type AgentVerdictStatus =
    | Passed
    | Failed
    | Unsupported
    | Degraded

/// Public contract type exposed by this FS.Skia.UI package.
type AgentVerdict =
    { Status: AgentVerdictStatus
      Authority: ValidationAuthority
      ChangedPathSource: ChangedPathSource
      SelectedRuleIds: ValidationRuleId list
      RequiredGates: ValidationGate list
      CompletedGates: ValidationGate list
      MissingGates: ValidationGate list
      SkippedGates: ValidationGate list
      MissingArtifacts: string list
      FailureOwner: ValidationFailureOwner
      FailureClass: ValidationFailureClass
      NextCommand: string option
      Artifacts: string list
      Diagnostics: string list
      TimestampUtc: System.DateTimeOffset }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationGateOutcome =
    | GatePassed
    | GateFailed of ValidationFailureOwner * ValidationFailureClass * diagnostic: string
    | GateUnsupportedHost of diagnostic: string
    | GateStalePrerequisite of diagnostic: string
    | GateMissingEvidence of diagnostic: string

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationGateResult =
    { Gate: ValidationGate
      Outcome: ValidationGateOutcome
      Artifacts: string list }

/// Public contract module exposed by this FS.Skia.UI package.
module AgentVerdict =
    /// Public contract function exposed by this FS.Skia.UI package.
    val aggregate:
        broadFallbackCommand: string ->
        changedPathSource: ChangedPathSource ->
        selectedRuleIds: ValidationRuleId list ->
        requiredGates: ValidationGate list ->
        gateResults: ValidationGateResult list ->
        artifacts: string list ->
        timestampUtc: System.DateTimeOffset ->
            AgentVerdict

    /// Public contract function exposed by this FS.Skia.UI package.
    val toJson: verdict: AgentVerdict -> string
    /// Public contract function exposed by this FS.Skia.UI package.
    val toMarkdown: verdict: AgentVerdict -> string

/// Public contract module exposed by this FS.Skia.UI package.
module ValidationContract =
    /// Public contract function exposed by this FS.Skia.UI package.
    val parse: text: string -> ValidationContractParseResult
    /// Public contract function exposed by this FS.Skia.UI package.
    val knownGates: ValidationGate list

/// Public contract module exposed by this FS.Skia.UI package.
module ValidationSelection =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: feature: FeatureId option -> ValidationSelectionModel * ValidationSelectionEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: ValidationSelectionMsg -> model: ValidationSelectionModel -> ValidationSelectionModel * ValidationSelectionEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val selectRules:
        changedPaths: ChangedPath list ->
        contract: ValidationContract ->
            ValidationRuleId list * ValidationGate list * ValidationAuthority

/// Public contract module exposed by this FS.Skia.UI package.
module ValidationSelectionInterpreter =
    /// Public contract function exposed by this FS.Skia.UI package.
    val readActiveFeatureMetadata:
        inputs: ValidationSelectionInterpreterInputs -> Result<FeatureId * ChangedPath list, string>

    /// Public contract function exposed by this FS.Skia.UI package.
    val runGitMergeBaseDiff:
        inputs: ValidationSelectionInterpreterInputs -> Result<string * ChangedPath list, string>

    /// Public contract function exposed by this FS.Skia.UI package.
    val loadValidationContract:
        inputs: ValidationSelectionInterpreterInputs ->
        changedPathSource: ChangedPathSource ->
            Result<ValidationRuleId list * ValidationGate list * ValidationAuthority, string>

    /// Public contract function exposed by this FS.Skia.UI package.
    val writeSelectionReport:
        inputs: ValidationSelectionInterpreterInputs ->
        model: ValidationSelectionModel ->
            Result<string, string>

    /// Public contract function exposed by this FS.Skia.UI package.
    val interpret:
        inputs: ValidationSelectionInterpreterInputs ->
        effect: ValidationSelectionEffect ->
        model: ValidationSelectionModel ->
            ValidationSelectionMsg option
