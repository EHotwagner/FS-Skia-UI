// Contract sketch (Phase 1) for build/Governance/ConstitutionFragments.fsi — US3.
// Extracts a fixed set of principle-summary fragments from .specify/memory/constitution.md and
// splices them between BEGIN/END GENERATED markers in templates, preserving all out-of-marker
// prose byte-for-byte (FR-010). Pure; the file read/write stays at the build.fsx edge.
namespace FS.Skia.UI.Build

module ConstitutionFragments =

    /// A generated principle-summary unit.
    type PrincipleFragment =
        { FragmentId: string
          SourceHeading: string
          RenderedText: string }

    /// A located BEGIN/END GENERATED region inside a template.
    type MarkerRegion =
        { FragmentId: string
          StartLine: int
          EndLine: int
          CurrentInner: string }

    /// Currency result for one template.
    type FragmentCurrency =
        { TemplatePath: string
          StaleFragments: string list
          UnknownMarkers: string list
          MissingMarkers: string list }

    /// The fixed, authoritative fragment inventory (ids the generator and templates agree on).
    val fragmentIds: string list

    /// Deterministically derive the fixed fragment set from the constitution source text.
    /// Raises if a required principle heading is missing (Principle VII).
    val extract: constitutionText: string -> PrincipleFragment list

    /// Locate all BEGIN/END GENERATED constitution regions in a template.
    val regions: templateText: string -> MarkerRegion list

    /// Replace the inner text of each matched region with the corresponding fragment's
    /// RenderedText; every byte outside every marker pair is preserved (FR-010).
    val splice: fragments: PrincipleFragment list -> templateText: string -> string

    /// Compare a template's committed marker regions against the re-derived fragments.
    val currency:
        templatePath: string ->
        fragments: PrincipleFragment list ->
        templateText: string ->
            FragmentCurrency

    /// Actionable currency diagnostic naming the regeneration command; None when current.
    val currencyDrift: currency: FragmentCurrency -> string option
