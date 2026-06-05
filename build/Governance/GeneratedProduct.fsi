// GeneratedProduct.fsi — generated-product structural validation (feature 045, T011,
// Principle II). Behaviour-identical relocation; the curated surface is the set of
// gate entry points the interpreter edge dispatches to.
module FS.Skia.UI.Build.GeneratedProduct

open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Front.Support

/// Feature 064 (FR-003): the exact consumer-emitted `NuGet.config` content. Single source
/// for both `writeLocalNuGetConfig` (what ships) and the pre-publish `NoMachineLocalPath`
/// check (what is validated), so the two cannot drift.
val consumerNuGetConfigContent: model: BuildModel -> string

val validateTemplatePackage: model: BuildModel -> outputPath: string -> unit
val runTemplateInstall: model: BuildModel -> label: string -> source: TemplateInstallSource -> outputPath: string -> unit
val runTemplateInstantiation: model: BuildModel -> outputPath: string -> unit
val scanGeneratedProjects: model: BuildModel -> outputPath: string -> unit
val runCapabilityCatalogCheck: model: BuildModel -> unit
val runSkillCatalogCheck: model: BuildModel -> unit
val runGenerateV3Products: model: BuildModel -> unit
val runScanV3GeneratedProducts: model: BuildModel -> unit
val runGeneratedConsumerValidation: model: BuildModel -> unit
val runDependencyOwnershipReport: model: BuildModel -> unit
val runPackageSurfaceReport: model: BuildModel -> unit
