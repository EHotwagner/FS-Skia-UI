module FS.Skia.UI.Build.Engine.Interpret

open System.IO
open BuildPaths
open BuildProcess
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Preflight
open FS.Skia.UI.Build.GeneratedProduct
open FS.Skia.UI.Build.Guidance
open FS.Skia.UI.Build.Front.Governance
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Engine.Update

// Relocated verbatim from build.fsx (feature 045, T013): the interpret edge (the ONLY
// I/O module) + runTarget = init -> update (StartTarget t) -> interpret over the effects.

let interpret root effect =
    let model, _ = init root

    match effect with
    | EnsureDirectory directory -> Directory.CreateDirectory directory |> ignore
    | CleanDirectoryContents directory -> cleanDirectoryContents directory
    | RunProcess(label, fileName, arguments, workingDirectory, outputPath, environment) ->
        runProcess label fileName arguments workingDirectory outputPath environment
    | RunDotnetAction(label, action, solutionFile, projects, extraArguments, outputPath) ->
        runDotnetAction label action solutionFile projects extraArguments outputPath root
    | InstallTemplate(label, source, outputPath) -> runTemplateInstall model label source outputPath
    | InstantiateTemplates outputPath -> runTemplateInstantiation model outputPath
    | ScanGeneratedProjects outputPath -> scanGeneratedProjects model outputPath
    | CapabilityCatalogCheck -> runCapabilityCatalogCheck model
    | SkillCatalogCheck -> runSkillCatalogCheck model
    | GenerateV3Products -> runGenerateV3Products model
    | ScanV3GeneratedProducts -> runScanV3GeneratedProducts model
    | ValidateGeneratedConsumer -> runGeneratedConsumerValidation model
    | PackageSurfaceReport -> runPackageSurfaceReport model
    | DependencyOwnershipReport -> runDependencyOwnershipReport model
    | ValidateTemplatePackage outputPath -> validateTemplatePackage model outputPath
    | GeneratedGuidanceScan outputPath -> runGeneratedGuidanceScan model outputPath
    | CollectProcessHealth(target, outputPath, verdictPath) -> collectProcessHealth root target outputPath verdictPath
    | ValidateRunnerBootstrap(target, outputPath, verdictPath) -> validateRunnerBootstrap root target outputPath verdictPath
    | WriteVerificationVerdict verdict -> writeVerificationVerdictReport model.VerificationVerdictsPath verdict
    | WriteFocusedGateSummary contract -> appendFocusedGateSummary model.FocusedGatesReportPath contract
    | CheckFocusedGateAssumptions contract -> checkFocusedGateAssumptions root contract
    | WriteStructuredReport(_, path, content) ->
        ensureParent path
        File.WriteAllText(path, content)
    | WriteStructuredJsonReport(_, path, content) ->
        ensureParent path
        File.WriteAllText(path, content)
    | WriteFile(path, content) ->
        ensureParent path
        File.WriteAllText(path, content)
    | RequireFiles(artifactClass, paths) -> requireFiles artifactClass paths
    | FailWith message -> failwith message
    | WorkflowSelfCheck -> workflowSelfCheck root
    | SkillSyncGate -> runSkillSyncGate model
    | RegenerateSkillTree -> regenerateSkillTree model
    | RegenerateConstitutionFragments -> regenerateConstitutionFragments model
    | RegenerateGovernedBlocks -> regenerateGovernedBlocks model
    | RegenerateCatalog -> regenerateCatalog model
    | RouteSelect -> runRouteSelection root
    | EvidenceGraphCheck -> runEvidenceGraphCheck model
    | EvidenceAuditCheck -> runEvidenceAuditCheck root model
    | PerPackageSurfaceDiffCheck -> runPerPackageSurfaceDiff model
    | SkillQualityScan -> runSkillQualityCheck model
    | RegenerateApiSurface -> regenerateApiSurface model
    | RegenerateSkillistReference -> regenerateSkillistReference model
    | SkillContractPathScan -> runSkillContractPathCheck model
    | TemplateUpdatePackageScan -> runTemplateUpdatePackageCheck model
    | SymbolCrossCheckAnalyze -> runSymbolCrossCheck model
    | PublishPackages -> runPublishPackages model
    | PrePublishValidate -> runPrePublishCheck model

let runTarget (target: Targets.Target) =
    let model, initEffects = init repositoryRoot
    let _, effects = update (StartTarget target) model

    (initEffects @ effects)
    |> List.iter (interpret repositoryRoot)
