// Capabilities.fsi — typed capability-catalog model, YamlDotNet-backed reader, and
// validation (feature 041, FR-003; Principle II). Build-tooling only.
//
// Replaces build.fsx's hand-rolled line-by-line YAML state machine
// (`readCapabilityCatalog`). template/capabilities.yml is retained as the
// source-of-truth data file and read THROUGH the typed model via the already-present
// YamlDotNet (no new dependency, FR-012). `validateRows` is pure; the surface-baseline
// file-existence probe is injected so it is testable without disk.
module FS.Skia.UI.Build.Capabilities

open FS.Skia.UI.Build.Findings

/// One capability's typed row (moved verbatim from build.fsx ~37–52).
type CapabilityRow =
    { Id: string
      DisplayName: string
      PackageId: string option
      Project: string option
      Contracts: string list
      Tests: string list
      Skill: string option
      TemplateFragment: string option
      Dependencies: string list
      Profiles: string list
      DefaultApp: bool
      Evidence: string list
      SurfaceBaseline: string option
      Docs: string option
      NonRuntime: bool }

/// Read template/capabilities.yml via YamlDotNet and project to typed rows (I/O edge).
val readCatalog: yamlPath: string -> CapabilityRow list

/// Validate the catalog, returning typed findings (rule ids preserved exactly).
/// `catalogPath` is the offending path used for the catalog-level `default-app` finding;
/// `surfaceBaselineExists` resolves a baseline relative path to disk presence (injected). Pure.
val validateRows:
    catalogPath: string -> surfaceBaselineExists: (string -> bool) -> capabilities: CapabilityRow list -> ValidationFinding list

/// The `# Capability Catalog` PASS report body (the metadata table). Pure.
val renderReport: capabilities: CapabilityRow list -> string
