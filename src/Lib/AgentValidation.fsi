namespace FS.Skia.UI.AgentValidation

type ValidationGate = string
type ValidationRuleId = string
type FeatureId = string
type ChangedPath = string

type ChangedPathSourceKind =
    | ActiveFeatureMetadata
    | GitMergeBaseDiff
    | Unavailable

type ChangedPathSource =
    { Kind: ChangedPathSourceKind
      Feature: FeatureId option
      MergeBase: string option
      Paths: ChangedPath list
      Diagnostics: string list }

type ValidationAuthority =
    | InnerLoop
    | FocusedAuthority
    | AgentReadyAuthority
    | MaintainerVerify
    | AutomationFinal

type ValidationSelectionModel =
    { Feature: FeatureId option
      ChangedPathSource: ChangedPathSource option
      SelectedRuleIds: ValidationRuleId list
      RequiredGates: ValidationGate list
      Authority: ValidationAuthority
      Degraded: bool
      Diagnostics: string list }

type ValidationSelectionMsg =
    | LoadActiveFeatureMetadata
    | ActiveFeatureMetadataLoaded of FeatureId * ChangedPath list
    | ActiveFeatureMetadataUnavailable of string
    | LoadGitMergeBaseDiff
    | GitMergeBaseDiffLoaded of mergeBase: string * paths: ChangedPath list
    | GitMergeBaseDiffUnavailable of string
    | ContractLoaded of selectedRuleIds: ValidationRuleId list * requiredGates: ValidationGate list * authority: ValidationAuthority
    | SelectionFailed of string

type ValidationSelectionEffect =
    | ReadActiveFeatureMetadata
    | RunGitMergeBaseDiff
    | LoadValidationContract
    | WriteValidationSelectionReport

type ValidationSelectionInterpreterInputs =
    { RepositoryRoot: string
      FeatureMetadataPath: string
      ValidationContractPath: string
      SelectionReportPath: string
      BaseRef: string option }

type ValidationContractDefaults =
    { BroadFallbackCommand: string
      FinalGates: ValidationGate list }

type ValidationContractTier =
    { Id: string
      Authority: string
      DefaultGates: ValidationGate list }

type ValidationContractRule =
    { Id: ValidationRuleId
      Paths: string list
      FeatureConcerns: string list
      RequiredGates: ValidationGate list
      ExpectedArtifacts: string list
      TimeoutClass: string
      FailureOwner: string }

type ValidationContract =
    { SchemaVersion: int
      Defaults: ValidationContractDefaults
      Tiers: ValidationContractTier list
      RoutingRules: ValidationContractRule list }

type ValidationContractDiagnostic =
    { Code: string
      Path: string
      Message: string
      Owner: string }

type ValidationContractParseResult =
    | ValidationContractAccepted of ValidationContract
    | ValidationContractRejected of ValidationContractDiagnostic list

type ValidationFailureOwner =
    | Product
    | Template
    | Governance
    | Environment
    | UnsupportedHost
    | StalePrerequisite
    | MissingEvidence
    | Unknown

type ValidationFailureClass =
    | ProductFailure
    | TemplateFailure
    | GovernanceFailure
    | EnvironmentFailure
    | UnsupportedHostFailure
    | StalePrerequisiteFailure
    | MissingEvidenceFailure
    | UnknownFailure

type TimeoutClass =
    | Fast
    | Focused
    | Broad

type ValidationCost =
    | Low
    | Medium
    | High

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

type AgentVerdictStatus =
    | Passed
    | Failed
    | Unsupported
    | Degraded

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

type ValidationGateOutcome =
    | GatePassed
    | GateFailed of ValidationFailureOwner * ValidationFailureClass * diagnostic: string
    | GateUnsupportedHost of diagnostic: string
    | GateStalePrerequisite of diagnostic: string
    | GateMissingEvidence of diagnostic: string

type ValidationGateResult =
    { Gate: ValidationGate
      Outcome: ValidationGateOutcome
      Artifacts: string list }

module AgentVerdict =
    val aggregate:
        broadFallbackCommand: string ->
        changedPathSource: ChangedPathSource ->
        selectedRuleIds: ValidationRuleId list ->
        requiredGates: ValidationGate list ->
        gateResults: ValidationGateResult list ->
        artifacts: string list ->
        timestampUtc: System.DateTimeOffset ->
            AgentVerdict

    val toJson: verdict: AgentVerdict -> string
    val toMarkdown: verdict: AgentVerdict -> string

module ValidationContract =
    val parse: text: string -> ValidationContractParseResult
    val knownGates: ValidationGate list

module ValidationSelection =
    val init: feature: FeatureId option -> ValidationSelectionModel * ValidationSelectionEffect list
    val update: msg: ValidationSelectionMsg -> model: ValidationSelectionModel -> ValidationSelectionModel * ValidationSelectionEffect list
    val selectRules:
        changedPaths: ChangedPath list ->
        contract: ValidationContract ->
            ValidationRuleId list * ValidationGate list * ValidationAuthority

module ValidationSelectionInterpreter =
    val readActiveFeatureMetadata:
        inputs: ValidationSelectionInterpreterInputs -> Result<FeatureId * ChangedPath list, string>

    val runGitMergeBaseDiff:
        inputs: ValidationSelectionInterpreterInputs -> Result<string * ChangedPath list, string>

    val loadValidationContract:
        inputs: ValidationSelectionInterpreterInputs ->
        changedPathSource: ChangedPathSource ->
            Result<ValidationRuleId list * ValidationGate list * ValidationAuthority, string>

    val writeSelectionReport:
        inputs: ValidationSelectionInterpreterInputs ->
        model: ValidationSelectionModel ->
            Result<string, string>

    val interpret:
        inputs: ValidationSelectionInterpreterInputs ->
        effect: ValidationSelectionEffect ->
        model: ValidationSelectionModel ->
            ValidationSelectionMsg option
