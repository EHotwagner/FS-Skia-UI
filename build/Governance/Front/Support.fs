module FS.Skia.UI.Build.Front.Support

open System

// Relocated verbatim from build.fsx (feature 045): build-front-end value types.

type TemplateInstallSource =
    | SourceDirectory
    | PackageArtifact

type TemplateRow =
    { Artifact: string
      Profile: string
      ProjectName: string
      Root: string
      EvidenceDir: string }

type V3GeneratedRow =
    { Artifact: string
      Profile: string
      ProjectName: string
      Root: string
      Capabilities: string list
      EvidenceDir: string
      FileListPath: string }

// CapabilityRow (FS.Skia.UI.Build.Capabilities) and ValidationFinding
// (FS.Skia.UI.Build.Findings) are now owned by the compiled governance library
// (feature 041, FR-003/FR-004); the local copies were retired to make the typed
// model the single source of truth.

type ValidationSelectionModel = { selectedRuleIds: string list }
type ValidationSelectionMsg =
    | ReadActiveFeatureMetadata
    | RunGitMergeBaseDiff
    | ChangedPathSourceActiveFeature
    | ChangedPathSourceGitMergeBase
    | ChangedPathSourceUnavailable
    | ValidationSelectionDegraded

type ValidationSelectionEffect =
    | LoadValidationContractForSelection

let unionSelectedGates rules =
    rules |> List.distinct

let multiRuleGateUnion = "multi-rule gate union"

type AgentVerdict =
    { status: string
      authority: string
      changedPathSource: string
      selectedRuleIds: string list
      requiredGates: string list
      completedGates: string list
      missingGates: string list
      missingArtifacts: string list
      failureOwner: string
      failureClass: string
      nextCommand: string option }

let AgentVerdictJson = "AgentVerdictJson"
let AgentVerdictMarkdown = "AgentVerdictMarkdown"
let focusedAuthority = "focused authority"
let nonAuthoritativeAggregate = "non-authoritative aggregate"
