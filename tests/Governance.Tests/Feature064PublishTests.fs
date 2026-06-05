module Feature064PublishTests

// Feature 064 (publish NuGet distribution). Failing-first governance tests for the four
// user stories: US1 public-feed consumer config (T010), US2 publish capability — pure update
// + 12-row plan + config validation (T014/T015), US3 single-source version pin (T022), and
// US4 pre-publish consistency rules (T027). Pure assertions over the real governance library.

open System.IO
open System.Text.RegularExpressions
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Engine.Update
open FS.Skia.UI.Build.Front.Helpers
open GovernanceTestSupport

let private model = fst (init repositoryRoot)
let private readRepo relative = File.ReadAllText(Path.Combine(repositoryRoot, relative))

[<Tests>]
let feature064PublishTests =
    testList "Feature 064 publish distribution" [

        // --- US1 / SC-001 (T010): consumer NuGet.config is public-feed only --------------
        test "T010 consumer NuGet.config carries no machine-local feed path (SC-001)" {
            let config = GeneratedProduct.consumerNuGetConfigContent model

            Expect.isFalse
                (config.Contains "nuget-local")
                "the emitted consumer NuGet.config must not reference a machine-local nuget-local directory"

            Expect.isFalse
                (Regex.IsMatch(config, "key=\"local\""))
                "the emitted consumer NuGet.config must not declare a local feed source"

            Expect.isFalse
                (Regex.IsMatch(config, "value=\"(/|[A-Za-z]:\\\\)[^\"]*\""))
                "the emitted consumer NuGet.config must not carry an absolute path value"

            Expect.isTrue
                (config.Contains "https://api.nuget.org/v3/index.json")
                "the emitted consumer NuGet.config references the public nuget.org feed"
        }

        // --- US2 / SC-002 (T014): pure update emits the publish effects ------------------
        test "T014 StartTarget Publish emits exactly the PublishPackages effect" {
            let next, effects = update (StartTarget Targets.Publish) model
            Expect.equal effects [ PublishPackages ] "Publish delegates to the PublishPackages interpret edge"
            Expect.equal next model "update does not mutate the model for Publish"
        }

        test "T014 StartTarget PrePublishCheck emits exactly the PrePublishValidate effect" {
            let _, effects = update (StartTarget Targets.PrePublishCheck) model
            Expect.equal effects [ PrePublishValidate ] "PrePublishCheck delegates to the pure validator edge"
        }

        test "T014 a non-dry-run with no API key aborts before any push (FR-002)" {
            let realEnv = function
                | "FSSKIA_PUBLISH_FEED" -> Some "/tmp/staging-feed"
                | _ -> None

            let config = Publish.configFromEnv realEnv
            Expect.isFalse config.DryRun "no dry-run env set"
            Expect.isFalse config.ApiKeyPresent "no API key env set"
            Expect.isSome (Publish.validateConfig config) "a real push with no credential must abort, naming the missing key"

            let dryEnv = function
                | "FSSKIA_PUBLISH_DRYRUN" -> Some "1"
                | _ -> None

            let dry = Publish.configFromEnv dryEnv
            Expect.isNone (Publish.validateConfig dry) "dry-run needs no credential"
        }

        // --- US2 / SC-002 (T015): the dry-run plan is exactly 12 rows --------------------
        test "T015 the publish plan is exactly 12 packages (11 libs + template)" {
            Expect.equal (List.length packProjects) 11 "packProjects is the 11 packable libraries"

            let packages =
                (packProjects |> List.map (fun (_, packageId) -> packageId, "0.1.67-preview.1"))
                @ [ "FS.Skia.UI.Template", "0.1.86-preview.1" ]

            let rows = Publish.buildPlan packages (fun _ _ -> false)
            Expect.equal (List.length rows) 12 "exactly 12 plan rows"
            Expect.all rows (fun r -> r.Decision = Push) "with no feed versions present, every row is a Push"
        }

        test "T015 a version already on the feed is a Skip (idempotency, SC-003)" {
            let packages = [ "FS.Skia.UI.Scene", "0.1.67-preview.1"; "FS.Skia.UI.Build", "0.1.67-preview.1" ]
            let rows = Publish.buildPlan packages (fun id _ -> id = "FS.Skia.UI.Build")
            let scene = rows |> List.find (fun r -> r.PackageId = "FS.Skia.UI.Scene")
            let build = rows |> List.find (fun r -> r.PackageId = "FS.Skia.UI.Build")
            Expect.equal scene.Decision Push "absent version is pushed"
            Expect.equal build.Decision Skip "present version is skipped"
        }

        // --- US3 / SC-004 (T022): exactly one literal FS.Skia.UI version -----------------
        test "T022 the template carries exactly one literal FS.Skia.UI version value (SC-004)" {
            let props = readRepo "template/base/Directory.Packages.props"

            Expect.isTrue
                (Regex.IsMatch(props, "<FsSkiaUiVersion>[^<]+</FsSkiaUiVersion>"))
                "Directory.Packages.props defines the single-source <FsSkiaUiVersion> property"

            // No FS.Skia.UI.* pin may carry a literal version; each must reference $(FsSkiaUiVersion).
            let literalPins =
                Regex.Matches(props, "<PackageVersion\\s+Include=\"FS\\.Skia\\.UI[^\"]*\"\\s+Version=\"([^\"]+)\"")
                |> Seq.cast<Match>
                |> Seq.map (fun m -> m.Groups.[1].Value)
                |> Seq.filter (fun v -> not (v.Trim().Equals("$(FsSkiaUiVersion)", System.StringComparison.OrdinalIgnoreCase)))
                |> Seq.toList

            Expect.isEmpty literalPins "every FS.Skia.UI.* pin references $(FsSkiaUiVersion), not a second literal"
        }

        test "T022 build.fsx has no literal #r engine version (single-source, SC-004)" {
            let buildFsx = readRepo "template/base/build.fsx"

            Expect.isFalse
                (Regex.IsMatch(buildFsx, "#r\\s+\"nuget:\\s*FS\\.Skia\\.UI\\.Build\\s*,\\s*[^\"]+\""))
                "build.fsx must not pin a literal engine version; it resolves <FsSkiaUiVersion> at runtime"
        }

        // --- US4 / SC-005 (T027): each pre-publish rule names the offender ---------------
        let baseInputs : PrePublish.PrePublishInputs =
            { ShippedVersions = [ "FS.Skia.UI.Scene", "0.1.67-preview.1"; "FS.Skia.UI.Build", "0.1.67-preview.1" ]
              EngineShippedVersion = "0.1.67-preview.1"
              TemplateProps =
                "<FsSkiaUiVersion>0.1.67-preview.1</FsSkiaUiVersion>\n"
                + "<PackageVersion Include=\"FS.Skia.UI.Scene\" Version=\"$(FsSkiaUiVersion)\" />\n"
                + "<PackageVersion Include=\"FS.Skia.UI.Build\" Version=\"$(FsSkiaUiVersion)\" />"
              EngineVersionFromBuildFsx = Some "0.1.67-preview.1"
              ConsumerNuGetConfig = "<add key=\"nuget\" value=\"https://api.nuget.org/v3/index.json\" />"
              Metadata =
                [ { PackageId = "FS.Skia.UI.Scene"
                    LicenseExpression = Some "MIT"
                    RepositoryUrl = Some "https://github.com/EHotwagner/FS-Skia-UI"
                    Authors = Some "FS.Skia.UI"
                    Description = Some "Scene graph"
                    ReadmeFile = Some "README.md" } ] }

        test "T027 a consistent set produces no findings" {
            Expect.isEmpty (PrePublish.check baseInputs) "the baseline set is internally consistent"
        }

        test "T027 PinParity names a template pin that diverges from the shipped version" {
            let skewed =
                { baseInputs with
                    TemplateProps =
                        "<FsSkiaUiVersion>0.1.66-preview.1</FsSkiaUiVersion>\n"
                        + "<PackageVersion Include=\"FS.Skia.UI.Scene\" Version=\"$(FsSkiaUiVersion)\" />" }

            let findings = PrePublish.checkPinParity skewed
            Expect.isNonEmpty findings "a pin that resolves to the wrong version is a PinParity finding"
            Expect.all findings (fun f -> f.Rule = PrePublish.PinParity) "rule is PinParity"
            Expect.exists findings (fun f -> f.Package = "FS.Skia.UI.Scene") "the offending package is named"
        }

        test "T027 EnginePinMatch names a build.fsx engine version that diverges" {
            let skewed = { baseInputs with EngineVersionFromBuildFsx = Some "0.1.66-preview.1" }
            let findings = PrePublish.checkEnginePinMatch skewed
            Expect.isNonEmpty findings "a mismatched engine version is an EnginePinMatch finding"
            Expect.all findings (fun f -> f.Rule = PrePublish.EnginePinMatch) "rule is EnginePinMatch"
        }

        test "T027 NoMachineLocalPath names a machine-local path in the emitted config" {
            let skewed =
                { baseInputs with
                    ConsumerNuGetConfig = "<add key=\"local\" value=\"/home/developer/.local/share/nuget-local\" />" }

            let findings = PrePublish.checkNoMachineLocalPath skewed
            Expect.isNonEmpty findings "a machine-local path is a NoMachineLocalPath finding"
            Expect.all findings (fun f -> f.Field = "NuGet.config:local") "the offending field is named"
        }

        test "T027 RequiredMetadata names a blank required field" {
            let skewed =
                { baseInputs with
                    Metadata = [ { baseInputs.Metadata.Head with RepositoryUrl = None; ReadmeFile = None } ] }

            let findings = PrePublish.checkRequiredMetadata skewed
            Expect.isNonEmpty findings "blank required metadata is a RequiredMetadata finding"
            Expect.exists findings (fun f -> f.Field = "repository URL") "the blank repository URL is named"
            Expect.exists findings (fun f -> f.Field = "README") "the blank README is named"
        }
    ]
