// PrePublish.fsi — the pre-publish consistency validator (feature 064, FR-006/FR-010).
//
// Build-tooling only. Every check here is PURE over the assembled `PrePublishInputs`: the
// interpreter reads `Directory.Packages.props`, `build.fsx`, the emitted `NuGet.config`, and
// each packable + template `.fsproj` (with `Directory.Build.props` fallback), assembles the
// inputs, calls `check`, and aborts the publish on any finding — naming the offending
// package/field.
module FS.Skia.UI.Build.PrePublish

/// Which consistency rule produced a finding.
type PrePublishRule =
    | PinParity
    | EnginePinMatch
    | NoMachineLocalPath
    | RequiredMetadata

/// One consistency failure, naming the offending package + field with an actionable detail.
type PrePublishFinding =
    { Package: string
      Field: string
      Rule: PrePublishRule
      Detail: string }

/// Resolved package metadata (each field already merged with the `Directory.Build.props`
/// fallback at the edge). `None`/blank ⇒ a RequiredMetadata finding (FR-010).
type PackageMetadata =
    { PackageId: string
      LicenseExpression: string option
      RepositoryUrl: string option
      Authors: string option
      Description: string option
      ReadmeFile: string option }

/// Everything the four rules need, assembled at the interpret edge so the checks stay pure.
type PrePublishInputs =
    { /// shipped (packageId, version) — packProjects + FS.Skia.UI.Template.
      ShippedVersions: (string * string) list
      /// shipped FS.Skia.UI.Build version (single-source target for EnginePinMatch).
      EngineShippedVersion: string
      /// template/base/Directory.Packages.props content.
      TemplateProps: string
      /// the version build.fsx resolves at runtime (literal `#r` pre-T024, or the
      /// `<FsSkiaUiVersion>` property value post-T024); None when unresolvable.
      EngineVersionFromBuildFsx: string option
      /// the consumer-emitted NuGet.config content (from GeneratedProduct.fs).
      ConsumerNuGetConfig: string
      /// per-package resolved metadata (incl. the template package).
      Metadata: PackageMetadata list }

/// Human-readable rule name (for the report + the abort message).
val ruleName: rule: PrePublishRule -> string

/// Rule 1 — every FS.Skia.UI.* pin in the template props resolves to the shipped version.
val checkPinParity: inputs: PrePublishInputs -> PrePublishFinding list

/// Rule 2 — build.fsx's resolved engine version equals the shipped FS.Skia.UI.Build version.
val checkEnginePinMatch: inputs: PrePublishInputs -> PrePublishFinding list

/// Rule 3 — the consumer-emitted NuGet.config carries no machine-local feed path.
val checkNoMachineLocalPath: inputs: PrePublishInputs -> PrePublishFinding list

/// Rule 4 — every package carries non-blank license / repo-url / authors / description / README.
val checkRequiredMetadata: inputs: PrePublishInputs -> PrePublishFinding list

/// All four rules; a non-empty result aborts the publish (FR-006).
val check: inputs: PrePublishInputs -> PrePublishFinding list

/// Render the findings (or the pass line) as a markdown report.
val render: findings: PrePublishFinding list -> string
