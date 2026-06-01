// Preflight.fsi — process-health / bootstrap preflight + verdict value types
// (feature 045, T008, Principle II). Behaviour-preserving relocation.
module FS.Skia.UI.Build.Preflight

open System

type ProcessHealthThreshold =
    { RuleId: string
      SignalName: string
      DefaultValue: int64
      Comparison: string
      ActualValue: int64 option
      OverrideValue: int64 option
      OverrideSource: string option
      OverrideReason: string option
      PlatformApplicability: string
      Passed: bool option
      Diagnostic: string option }

type ProcessHealthSnapshot =
    { TimestampUtc: DateTimeOffset
      TargetName: string
      Stage: string
      Platform: string
      AvailableMemoryMb: int64 option
      ProcessCount: int option
      ZombieProcessCount: int option
      ThreadLimit: int64 option
      ThreadHeadroom: int64 option
      FileDescriptorLimit: int64 option
      FileDescriptorHeadroom: int64 option
      DotnetStartup: string
      FakeBootstrap: string
      UnsupportedSignals: string list
      Thresholds: ProcessHealthThreshold list
      PreflightElapsedMs: int64
      FailFast: bool
      Diagnostics: string list }

type BootstrapValidation =
    { TargetName: string
      TimestampUtc: DateTimeOffset
      DotnetSdkStatus: string
      FakeToolRestoreStatus: string
      PackageCacheStatus: string
      WrapperStatus: string
      WarningClassification: string
      RecommendedAction: string option
      LogPath: string
      Passed: bool }

type VerificationVerdictCategory =
    | VerificationSuccess
    | VerificationProductFailure
    | VerificationEnvironmentFailure
    | VerificationDegraded

type VerificationVerdict =
    { Category: VerificationVerdictCategory
      Target: string
      Stage: string
      ExitCode: int option
      ProductChecksRun: string list
      ProductFailures: string list
      EnvironmentFailures: string list
      HealthSnapshotPath: string
      LogPath: string
      RecommendedRerunEnvironment: string
      AuthoritativeProductEvidence: bool }

type FocusedGateContract =
    { TargetName: string
      DirectPrerequisites: string list
      Command: string
      LogPath: string
      ReadinessPath: string option
      StaleAssumptions: string list
      VerdictCategory: VerificationVerdictCategory }

val requireFiles: artifactClass: string -> paths: string list -> unit
val writeVerificationVerdictReport: outputPath: string -> verdict: VerificationVerdict -> unit
val collectProcessHealth: root: string -> target: string -> outputPath: string -> verdictPath: string -> unit
val validateRunnerBootstrap: root: string -> target: string -> outputPath: string -> verdictPath: string -> unit
val checkFocusedGateAssumptions: root: string -> contract: FocusedGateContract -> unit
val appendFocusedGateSummary: outputPath: string -> contract: FocusedGateContract -> unit
val relativePathFrom: root: string -> filePath: string -> string
