module FS.Skia.UI.Build.Preflight

open System
open System.Diagnostics
open System.IO
open System.Runtime.InteropServices
open System.Text.RegularExpressions
open BuildPaths
open BuildProcess

// Relocated verbatim from build.fsx (feature 045, T008): process-health / bootstrap
// preflight + verdict value types. Behaviour-preserving; git/process wrapping at the edge.

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

let categoryName category =
    match category with
    | VerificationSuccess -> "success"
    | VerificationProductFailure -> "product-failure"
    | VerificationEnvironmentFailure -> "environment-failure"
    | VerificationDegraded -> "degraded"

let requireFiles (artifactClass: string) (paths: string list) =
    let missing =
        paths
        |> List.filter (fun path -> not (File.Exists path))

    if missing.Length > 0 then
        let detail = String.Join(Environment.NewLine, missing)
        failwithf "Missing %s:%s%s" artifactClass Environment.NewLine detail

let tryParseInt64 value =
    BuildProcessHealth.tryParseInt64 value

let tryReadLinuxMemAvailableMb () =
    BuildProcessHealth.tryReadLinuxMemAvailableMb ()

let tryReadProcLimit (name: string) =
    BuildProcessHealth.tryReadProcLimit name

let tryCountOpenFileDescriptors () =
    BuildProcessHealth.tryCountOpenFileDescriptors ()

let tryZombieProcessCount () =
    BuildProcessHealth.tryZombieProcessCount ()

let runShortCommand workingDirectory fileName arguments timeoutMs =
    BuildProcessHealth.runShortCommand workingDirectory fileName arguments timeoutMs

let thresholdDecision ruleId signal defaultValue comparison actualValue envVar platform =
    // Process-health policy keeps stable diagnostics such as malformed threshold override and env var _REASON checks.
    let (
        ruleId,
        signal,
        defaultValue,
        comparison,
        actualValue,
        overrideValue,
        overrideSource,
        overrideReason,
        platform,
        passed,
        diagnostic
        ) =
        BuildProcessHealth.thresholdDecision ruleId signal defaultValue comparison actualValue envVar platform

    { RuleId = ruleId
      SignalName = signal
      DefaultValue = defaultValue
      Comparison = comparison
      ActualValue = actualValue
      OverrideValue = overrideValue
      OverrideSource = overrideSource
      OverrideReason = overrideReason
      PlatformApplicability = platform
      Passed = passed
      Diagnostic = diagnostic }

let markdownOption value unit =
    BuildProcessHealth.markdownOption value unit

let writeVerificationVerdictReport outputPath verdict =
    ensureParent outputPath
    let exitCode = verdict.ExitCode |> Option.map string |> Option.defaultValue "n/a"
    let productChecksRun = if verdict.ProductChecksRun.IsEmpty then "(none)" else String.Join(", ", verdict.ProductChecksRun)
    let productFailures = if verdict.ProductFailures.IsEmpty then "(none)" else String.Join(", ", verdict.ProductFailures)
    let environmentFailures = if verdict.EnvironmentFailures.IsEmpty then "(none)" else String.Join(", ", verdict.EnvironmentFailures)

    let content =
        [ $"## {verdict.Target} {verdict.Stage}"
          ""
          $"- verdict-category: `{categoryName verdict.Category}`"
          $"- authoritative-product-evidence: `{verdict.AuthoritativeProductEvidence}`"
          $"- exit-code: `{exitCode}`"
          $"- health-snapshot-path: `{verdict.HealthSnapshotPath}`"
          $"- log-path: `{verdict.LogPath}`"
          $"- recommended-rerun-environment: {verdict.RecommendedRerunEnvironment}"
          $"- product-checks-run: {productChecksRun}"
          $"- product-failures: {productFailures}"
          $"- environment-failures: {environmentFailures}"
          "" ]
        |> String.concat Environment.NewLine

    BuildReports.appendOrCreateSection outputPath "# Verification Verdict Evidence" [ content ]

let collectProcessHealth root target outputPath verdictPath =
    let stopwatch = Stopwatch.StartNew()
    let timestamp = DateTimeOffset.UtcNow
    let platform = RuntimeInformation.OSDescription
    let availableMemory = tryReadLinuxMemAvailableMb ()
    let processCount =
        try
            Process.GetProcesses().Length |> Some
        with _ ->
            None

    let zombieCount = tryZombieProcessCount ()
    let threadLimit = tryReadProcLimit "Max processes"
    let threadHeadroom = Option.map2 (fun limit processes -> limit - int64 processes) threadLimit processCount
    let fdLimit = tryReadProcLimit "Max open files"
    let fdHeadroom = Option.map2 (fun limit used -> limit - used) fdLimit (tryCountOpenFileDescriptors ())
    let dotnetExit, dotnetOutput = runShortCommand root "dotnet" "--info" 15000
    let dotnetStartup = if dotnetExit = 0 then "pass" else $"failed: {dotnetOutput}"
    let fakeBootstrap =
        if File.Exists(path [ root; "fake.sh" ])
           && File.Exists(path [ root; ".config"; "dotnet-tools.json" ]) then
            "pass"
        else
            "failed: fake wrapper or tool manifest is missing"

    let thresholds =
        [ thresholdDecision "process-health.available-memory" "available-memory-mb" 128L ">=" availableMemory "FS_SKIA_PROCESS_MIN_AVAILABLE_MEMORY_MB" platform
          thresholdDecision "process-health.process-count" "process-count" 4096L "<=" (processCount |> Option.map int64) "FS_SKIA_PROCESS_MAX_PROCESS_COUNT" platform
          thresholdDecision "process-health.zombie-count" "zombie-process-count" 2048L "<=" (zombieCount |> Option.map int64) "FS_SKIA_PROCESS_MAX_ZOMBIE_COUNT" platform
          thresholdDecision "process-health.file-descriptor-headroom" "file-descriptor-headroom" 64L ">=" fdHeadroom "FS_SKIA_PROCESS_MIN_FD_HEADROOM" platform ]

    let unsupported =
        [ if availableMemory.IsNone then "available-memory-mb"
          if processCount.IsNone then "process-count"
          if zombieCount.IsNone then "zombie-process-count"
          if threadLimit.IsNone then "thread-limit"
          if threadHeadroom.IsNone then "thread-headroom"
          if fdLimit.IsNone then "file-descriptor-limit"
          if fdHeadroom.IsNone then "file-descriptor-headroom" ]

    let diagnostics =
        [ if dotnetExit <> 0 then $"dotnet startup failed: {dotnetOutput}"
          if fakeBootstrap <> "pass" then fakeBootstrap
          yield!
              thresholds
              |> List.choose (fun threshold ->
                  match threshold.Passed, threshold.Diagnostic with
                  | Some false, Some diagnostic -> Some diagnostic
                  | _ -> None) ]

    stopwatch.Stop()

    let failFast = not diagnostics.IsEmpty

    let snapshot =
        { TimestampUtc = timestamp
          TargetName = target
          Stage = "preflight"
          Platform = platform
          AvailableMemoryMb = availableMemory
          ProcessCount = processCount
          ZombieProcessCount = zombieCount
          ThreadLimit = threadLimit
          ThreadHeadroom = threadHeadroom
          FileDescriptorLimit = fdLimit
          FileDescriptorHeadroom = fdHeadroom
          DotnetStartup = dotnetStartup
          FakeBootstrap = fakeBootstrap
          UnsupportedSignals = unsupported
          Thresholds = thresholds
          PreflightElapsedMs = stopwatch.ElapsedMilliseconds
          FailFast = failFast
          Diagnostics = diagnostics }

    let report =
        let availableMemoryText = markdownOption snapshot.AvailableMemoryMb " MB"
        let processCountText = snapshot.ProcessCount |> Option.map string |> Option.defaultValue "unsupported"
        let zombieProcessCountText = snapshot.ZombieProcessCount |> Option.map string |> Option.defaultValue "unsupported"
        let threadLimitText = snapshot.ThreadLimit |> Option.map string |> Option.defaultValue "unsupported"
        let threadHeadroomText = snapshot.ThreadHeadroom |> Option.map string |> Option.defaultValue "unsupported"
        let fileDescriptorLimitText = snapshot.FileDescriptorLimit |> Option.map string |> Option.defaultValue "unsupported"
        let fileDescriptorHeadroomText = snapshot.FileDescriptorHeadroom |> Option.map string |> Option.defaultValue "unsupported"
        let unsupportedSignalsText = if snapshot.UnsupportedSignals.IsEmpty then "(none)" else String.Join(", ", snapshot.UnsupportedSignals)

        [ "# Process Health Evidence"
          ""
          $"## {snapshot.TargetName} {snapshot.Stage}"
          ""
          $"- timestamp-utc: `{snapshot.TimestampUtc:O}`"
          $"- target: `{snapshot.TargetName}`"
          $"- platform: `{snapshot.Platform}`"
          $"- available-memory: `{availableMemoryText}`"
          $"- process-count: `{processCountText}`"
          $"- zombie-process-count: `{zombieProcessCountText}`"
          $"- thread-limit: `{threadLimitText}`"
          $"- thread-headroom: `{threadHeadroomText}`"
          $"- file-descriptor-limit: `{fileDescriptorLimitText}`"
          $"- file-descriptor-headroom: `{fileDescriptorHeadroomText}`"
          $"- dotnet-startup: `{snapshot.DotnetStartup}`"
          $"- fake-bootstrap: `{snapshot.FakeBootstrap}`"
          $"- preflight-elapsed-ms: `{snapshot.PreflightElapsedMs}`"
          $"- fail-fast: `{snapshot.FailFast}`"
          $"- unsupported-signals: {unsupportedSignalsText}"
          ""
          "| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |"
          "|---------|--------|--------|---------|----------|--------|----------|------------|"
          yield!
              snapshot.Thresholds
              |> List.map (fun threshold ->
                  let actual = threshold.ActualValue |> Option.map string |> Option.defaultValue "unsupported"
                  let overrideValue = threshold.OverrideValue |> Option.map string |> Option.defaultValue ""
                  let reason = threshold.OverrideReason |> Option.defaultValue ""
                  let decision =
                      threshold.Passed
                      |> Option.map (fun passed -> if passed then "pass" else "fail")
                      |> Option.defaultValue "unsupported"
                  let diagnostic = threshold.Diagnostic |> Option.defaultValue ""
                  $"| `{threshold.RuleId}` | {threshold.SignalName} | {actual} | {threshold.Comparison} {threshold.DefaultValue} | {overrideValue} | {reason} | {decision} | {diagnostic} |")
          ""
          "Diagnostics:"
          if snapshot.Diagnostics.IsEmpty then
              "- none"
          else
              yield! snapshot.Diagnostics |> List.map (fun diagnostic -> $"- {diagnostic}") ]
        |> String.concat Environment.NewLine

    File.WriteAllText(outputPath, report + Environment.NewLine)

    if failFast then
        let verdict =
            { Category = VerificationEnvironmentFailure
              Target = target
              Stage = "preflight"
              ExitCode = Some 1
              ProductChecksRun = []
              ProductFailures = []
              EnvironmentFailures = diagnostics
              HealthSnapshotPath = outputPath
              LogPath = outputPath
              RecommendedRerunEnvironment = "fresh shell, fresh container, or CI runner"
              AuthoritativeProductEvidence = false }

        writeVerificationVerdictReport verdictPath verdict
        failwithf "%s process-health preflight failed with environment-failure:%s%s" target Environment.NewLine (String.Join(Environment.NewLine, diagnostics))

let validateRunnerBootstrap root target outputPath verdictPath =
    let timestamp = DateTimeOffset.UtcNow
    let dotnetExit, dotnetOutput = runShortCommand root "dotnet" "--info" 15000
    let toolExit, toolOutput = runShortCommand root "dotnet" "tool restore" 60000
    let runnerExit, runnerOutput = runShortCommand root "dotnet" "fake --version" 60000
    let wrapperExists = File.Exists(path [ root; "fake.sh" ]) && File.Exists(path [ root; "fake.cmd" ])
    let passed = dotnetExit = 0 && toolExit = 0 && runnerExit = 0 && wrapperExists

    let recommended =
        if passed then
            None
        else
            Some "Run `dotnet tool restore`, then `dotnet fake --version`; clear stale `.fake/build.fsx/paket-files/paket.restore.cached` only if the FAKE runner still cannot start."

    let validation =
        { TargetName = target
          TimestampUtc = timestamp
          DotnetSdkStatus = if dotnetExit = 0 then "pass" else $"failed: {dotnetOutput}"
          FakeToolRestoreStatus = if toolExit = 0 then "pass" else $"failed: {toolOutput}"
          PackageCacheStatus = if runnerExit = 0 then "pass" else $"failed: FAKE runner did not start after tool restore: {runnerOutput}"
          WrapperStatus = if wrapperExists then "pass" else "failed: fake.sh or fake.cmd missing"
          WarningClassification = "runner-warning-classification: repeated netstandard script-load warning is warning-noise unless target exits nonzero; CoreCLR/VSTest/socket/thread startup failures are environment-failure evidence"
          RecommendedAction = recommended
          LogPath = outputPath
          Passed = passed }

    let remediation = validation.RecommendedAction |> Option.defaultValue "(none)"

    let report =
        [ "# Bootstrap Runner Evidence"
          ""
          $"- target: `{validation.TargetName}`"
          $"- timestamp-utc: `{validation.TimestampUtc:O}`"
          $"- dotnet-sdk-status: `{validation.DotnetSdkStatus}`"
          $"- fake-tool-restore-status: `{validation.FakeToolRestoreStatus}`"
          $"- package-cache-status: `{validation.PackageCacheStatus}`"
          $"- wrapper-status: `{validation.WrapperStatus}`"
          $"- warning-classification: {validation.WarningClassification}"
          $"- passed: `{validation.Passed}`"
          $"- remediation-command: {remediation}" ]
        |> String.concat Environment.NewLine

    File.WriteAllText(outputPath, report + Environment.NewLine)

    if not passed then
        let failures =
            [ validation.DotnetSdkStatus
              validation.FakeToolRestoreStatus
              validation.PackageCacheStatus
              validation.WrapperStatus ]
            |> List.filter (fun status -> status.StartsWith("failed", StringComparison.Ordinal))

        let verdict =
            { Category = VerificationEnvironmentFailure
              Target = target
              Stage = "bootstrap"
              ExitCode = Some 1
              ProductChecksRun = []
              ProductFailures = []
              EnvironmentFailures = failures
              HealthSnapshotPath = outputPath
              LogPath = outputPath
              RecommendedRerunEnvironment = "fresh shell, fresh container, or CI runner"
              AuthoritativeProductEvidence = false }

        writeVerificationVerdictReport verdictPath verdict
        failwithf "%s bootstrap validation failed with environment-failure:%s%s" target Environment.NewLine (String.Join(Environment.NewLine, failures))

let checkFocusedGateAssumptions root (contract: FocusedGateContract) =
    contract.StaleAssumptions
    |> List.iter (fun assumption ->
        if assumption.StartsWith("requires-restored-project:", StringComparison.Ordinal) then
            let project = assumption.Substring("requires-restored-project:".Length)
            let projectDir =
                Path.GetDirectoryName(path [ root; project ]) |> Option.ofObj |> Option.defaultValue ""
            let assets = path [ projectDir; "obj"; "project.assets.json" ]

            if not (File.Exists assets) then
                failwithf "stale-build-restore-assumption: affected-gate=%s remediation-command=`dotnet restore %s` missing `%s`" contract.TargetName project assets
        elif assumption.StartsWith("requires-built-project:", StringComparison.Ordinal) then
            let project = assumption.Substring("requires-built-project:".Length)
            let projectDir =
                Path.GetDirectoryName(path [ root; project ]) |> Option.ofObj |> Option.defaultValue ""
            let projectName = Path.GetFileNameWithoutExtension project
            let assembly = path [ projectDir; "bin"; "Debug"; "net10.0"; $"{projectName}.dll" ]

            if not (File.Exists assembly) then
                failwithf "stale-build-restore-assumption: affected-gate=%s remediation-command=`dotnet build %s` missing `%s`" contract.TargetName project assembly)

let appendFocusedGateSummary outputPath (contract: FocusedGateContract) =
    ensureParent outputPath

    let prerequisites =
        if contract.DirectPrerequisites.IsEmpty then
            "(none)"
        else
            String.Join(", ", contract.DirectPrerequisites)

    let assumptions =
        if contract.StaleAssumptions.IsEmpty then
            "(none)"
        else
            String.Join(", ", contract.StaleAssumptions)

    let readiness =
        contract.ReadinessPath |> Option.defaultValue "(none)"

    let content =
        [ $"## {contract.TargetName}"
          ""
          $"- command: `{contract.Command}`"
          $"- direct-prerequisites: {prerequisites}"
          $"- timestamp-utc: `{DateTimeOffset.UtcNow:O}`"
          $"- log-path: `{contract.LogPath}`"
          $"- readiness-path: `{readiness}`"
          $"- verdict-category: `{categoryName contract.VerdictCategory}`"
          $"- stale-build-restore-assumptions: {assumptions}"
          $"- failure-rule: `stale-build-restore-assumption`"
          $"- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active"
          $"- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists"
          $"- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared"
          $"- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure"
          $"- affected-gate: `{contract.TargetName}`"
          $"- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale"
          "" ]
        |> String.concat Environment.NewLine

    BuildReports.appendOrCreateSection outputPath "# Focused Gates Evidence" [ content ]

let relativePathFrom root filePath =
    let rootPath =
        Path.GetFullPath root
        |> fun value -> value.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + string Path.DirectorySeparatorChar

    let filePath = Path.GetFullPath filePath
    let relative = Uri(rootPath).MakeRelativeUri(Uri(filePath)).ToString()
    Uri.UnescapeDataString(relative).Replace('\\', '/')

