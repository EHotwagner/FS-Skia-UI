namespace FS.Skia.UI.Build.AgentValidation

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationGate = string
/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationRuleId = string
/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type FeatureId = string
/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ChangedPath = string

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ChangedPathSourceKind =
    | ActiveFeatureMetadata
    | GitMergeBaseDiff
    | Unavailable

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ChangedPathSource =
    { Kind: ChangedPathSourceKind
      Feature: FeatureId option
      MergeBase: string option
      Paths: ChangedPath list
      Diagnostics: string list }

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationAuthority =
    | InnerLoop
    | FocusedAuthority
    | AgentReadyAuthority
    | MaintainerVerify
    | AutomationFinal

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationSelectionModel =
    { Feature: FeatureId option
      ChangedPathSource: ChangedPathSource option
      SelectedRuleIds: ValidationRuleId list
      RequiredGates: ValidationGate list
      Authority: ValidationAuthority
      Degraded: bool
      Diagnostics: string list }

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationSelectionMsg =
    | LoadActiveFeatureMetadata
    | ActiveFeatureMetadataLoaded of FeatureId * ChangedPath list
    | ActiveFeatureMetadataUnavailable of string
    | LoadGitMergeBaseDiff
    | GitMergeBaseDiffLoaded of mergeBase: string * paths: ChangedPath list
    | GitMergeBaseDiffUnavailable of string
    | ContractLoaded of selectedRuleIds: ValidationRuleId list * requiredGates: ValidationGate list * authority: ValidationAuthority
    | SelectionFailed of string

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationSelectionEffect =
    | ReadActiveFeatureMetadata
    | RunGitMergeBaseDiff
    | LoadValidationContract
    | WriteValidationSelectionReport

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationSelectionInterpreterInputs =
    { RepositoryRoot: string
      FeatureMetadataPath: string
      ValidationContractPath: string
      SelectionReportPath: string
      BaseRef: string option }

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationContractDefaults =
    { BroadFallbackCommand: string
      FinalGates: ValidationGate list }

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationContractTier =
    { Id: string
      Authority: string
      DefaultGates: ValidationGate list }

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationContractRule =
    { Id: ValidationRuleId
      Paths: string list
      FeatureConcerns: string list
      RequiredGates: ValidationGate list
      ExpectedArtifacts: string list
      TimeoutClass: string
      FailureOwner: string }

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationContract =
    { SchemaVersion: int
      Defaults: ValidationContractDefaults
      Tiers: ValidationContractTier list
      RoutingRules: ValidationContractRule list }

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationContractDiagnostic =
    { Code: string
      Path: string
      Message: string
      Owner: string }

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationContractParseResult =
    | ValidationContractAccepted of ValidationContract
    | ValidationContractRejected of ValidationContractDiagnostic list

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationFailureOwner =
    | Product
    | Template
    | Governance
    | Environment
    | UnsupportedHost
    | StalePrerequisite
    | MissingEvidence
    | Unknown

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationFailureClass =
    | ProductFailure
    | TemplateFailure
    | GovernanceFailure
    | EnvironmentFailure
    | UnsupportedHostFailure
    | StalePrerequisiteFailure
    | MissingEvidenceFailure
    | UnknownFailure

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type TimeoutClass =
    | Fast
    | Focused
    | Broad

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationCost =
    | Low
    | Medium
    | High

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
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

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type AgentVerdictStatus =
    | Passed
    | Failed
    | Unsupported
    | Degraded

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
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

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationGateOutcome =
    | GatePassed
    | GateFailed of ValidationFailureOwner * ValidationFailureClass * diagnostic: string
    | GateUnsupportedHost of diagnostic: string
    | GateStalePrerequisite of diagnostic: string
    | GateMissingEvidence of diagnostic: string

/// Public contract type exposed by the FS.Skia.UI.Build governance library.
type ValidationGateResult =
    { Gate: ValidationGate
      Outcome: ValidationGateOutcome
      Artifacts: string list }

/// Public contract module exposed by the FS.Skia.UI.Build governance library.
module AgentVerdict =
    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val aggregate:
        broadFallbackCommand: string ->
        changedPathSource: ChangedPathSource ->
        selectedRuleIds: ValidationRuleId list ->
        requiredGates: ValidationGate list ->
        gateResults: ValidationGateResult list ->
        artifacts: string list ->
        timestampUtc: System.DateTimeOffset ->
            AgentVerdict

    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val toJson: verdict: AgentVerdict -> string
    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val toMarkdown: verdict: AgentVerdict -> string

/// Public contract module exposed by the FS.Skia.UI.Build governance library.
module ValidationContract =
    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val parse: text: string -> ValidationContractParseResult
    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val knownGates: ValidationGate list

/// Public contract module exposed by the FS.Skia.UI.Build governance library.
module ValidationSelection =
    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val init: feature: FeatureId option -> ValidationSelectionModel * ValidationSelectionEffect list
    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val update: msg: ValidationSelectionMsg -> model: ValidationSelectionModel -> ValidationSelectionModel * ValidationSelectionEffect list
    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val selectRules:
        changedPaths: ChangedPath list ->
        contract: ValidationContract ->
            ValidationRuleId list * ValidationGate list * ValidationAuthority

/// Public contract module exposed by the FS.Skia.UI.Build governance library.
module ValidationSelectionInterpreter =
    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val readActiveFeatureMetadata:
        inputs: ValidationSelectionInterpreterInputs -> Result<FeatureId * ChangedPath list, string>

    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val runGitMergeBaseDiff:
        inputs: ValidationSelectionInterpreterInputs -> Result<string * ChangedPath list, string>

    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val loadValidationContract:
        inputs: ValidationSelectionInterpreterInputs ->
        changedPathSource: ChangedPathSource ->
            Result<ValidationRuleId list * ValidationGate list * ValidationAuthority, string>

    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val writeSelectionReport:
        inputs: ValidationSelectionInterpreterInputs ->
        model: ValidationSelectionModel ->
            Result<string, string>

    /// Public contract function exposed by the FS.Skia.UI.Build governance library.
    val interpret:
        inputs: ValidationSelectionInterpreterInputs ->
        effect: ValidationSelectionEffect ->
        model: ValidationSelectionModel ->
            ValidationSelectionMsg option
