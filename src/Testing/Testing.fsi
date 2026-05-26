namespace FS.Skia.UI.Testing

open System

type PackageReferenceExpectation =
    { PackageId: string
      Required: bool }

type GeneratedProductExpectation =
    { Profile: string
      RequiredFiles: string list
      ForbiddenPrefixes: string list
      PackageReferences: PackageReferenceExpectation list }

type LocalConsumerPackage =
    { PackageId: string
      Version: string
      FeedPath: string }

type LocalConsumerPackageDrift =
    { PackageId: string
      ExpectedVersion: string
      ActualVersion: string option
      FeedPath: string
      RemediationCommand: string }

type LocalConsumerPackageReport =
    { FeedPath: string
      Packages: LocalConsumerPackage list
      ConsumerConfigSnippet: string
      NuGetConfigSnippet: string option
      RestoreCommand: string
      DriftDiagnostics: LocalConsumerPackageDrift list }

type GeneratedValidationCategory =
    | PackageDrift
    | RestoreFailure
    | SemanticTestFailure
    | ViewerStartupFailure
    | UnsupportedHost
    | SceneEvidenceFailure
    | Completed

type GeneratedValidationResult =
    { Category: GeneratedValidationCategory
      Elapsed: TimeSpan
      CommandContext: string
      EvidencePath: string option
      Diagnostics: string list }

type GeneratedProductLaunchValidationResult =
    { InteractiveLaunchRequired: bool
      Diagnostics: string list }

type PackageResolutionCheck =
    { RequestedPackages: LocalConsumerPackage list
      ResolvedPackages: LocalConsumerPackage list
      PackageSources: string list
      RestoreWarnings: string list }

type PackageResolutionCheckResult =
    { ExactMatch: bool
      FailureReason: string option
      Diagnostics: string list }

type GeneratedTestExecutionCheck =
    { TestsExist: bool
      TestsRan: bool
      VerifyRan: bool }

type GeneratedTestExecutionResult =
    { Authoritative: bool
      NonAuthoritativeReason: string option
      Diagnostics: string list }

type VisualEvidenceKind =
    | Screenshot
    | PixelReadback
    | UnsupportedHost

type VisualEvidenceRequest =
    { ScreenshotAvailable: bool
      PixelReadbackAvailable: bool
      BoardReadable: bool option
      InputOrProgressObserved: bool option
      UnsupportedReason: string option }

type VisualEvidenceResult =
    { EvidenceKind: VisualEvidenceKind
      BoardReadable: bool option
      InputOrProgressObserved: bool option
      FallbackReason: string option
      UnsupportedReason: string option
      Diagnostics: string list }

module GeneratedProductAssertions =
    val summarize: expectation: GeneratedProductExpectation -> string
    val validateDefaultInteractiveLaunch: source: string -> GeneratedProductLaunchValidationResult

module LocalConsumerPackages =
    val report: feedPath: string -> packages: LocalConsumerPackage list -> LocalConsumerPackageReport
    val classifyDrift: expected: LocalConsumerPackage list -> actual: LocalConsumerPackage list -> LocalConsumerPackageDrift list

module GeneratedConsumerValidation =
    val summarize: result: GeneratedValidationResult -> string
    val verifyPackageResolution: check: PackageResolutionCheck -> PackageResolutionCheckResult
    val verifyGeneratedTests: check: GeneratedTestExecutionCheck -> GeneratedTestExecutionResult
    val selectVisualEvidence: request: VisualEvidenceRequest -> VisualEvidenceResult
