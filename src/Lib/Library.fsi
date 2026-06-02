namespace FS.Skia.UI

/// Public contract type exposed by this FS.Skia.UI package.
type ParityStatus =
    | Supported
    | Adapted
    | Excluded
    | NotYetSupported

/// Public contract type exposed by this FS.Skia.UI package.
type EvidenceType =
    | SemanticTest
    | Screenshot
    | Smoke
    | Package
    | Documentation
    | ManualReview

/// Public contract type exposed by this FS.Skia.UI package.
type ParityEvidenceItem =
    { CapabilityId: string
      Capability: string
      Status: ParityStatus
      EvidenceType: EvidenceType
      Command: string
      Path: string
      AdaptationNotes: string
      ConflictsWithConstraints: bool }

/// Public contract type exposed by this FS.Skia.UI package.
type ParityReport =
    { Feature: string
      BaselineCommit: string
      Items: ParityEvidenceItem list }

/// Public contract module exposed by this FS.Skia.UI package.
module Parity =
    /// Public contract function exposed by this FS.Skia.UI package.
    val baselineCommit : string
    /// Public contract function exposed by this FS.Skia.UI package.
    val capabilityIds : string list
    /// Public contract function exposed by this FS.Skia.UI package.
    val createItem :
        capabilityId: string ->
        capability: string ->
        status: ParityStatus ->
        evidenceType: EvidenceType ->
        command: string ->
        path: string ->
        adaptationNotes: string ->
        conflictsWithConstraints: bool ->
            ParityEvidenceItem
    /// Public contract function exposed by this FS.Skia.UI package.
    val createReport : feature: string -> items: ParityEvidenceItem list -> ParityReport
    /// Public contract function exposed by this FS.Skia.UI package.
    val validateMergeReady : report: ParityReport -> string list
    /// Public contract function exposed by this FS.Skia.UI package.
    val toJson : report: ParityReport -> string
    /// Public contract function exposed by this FS.Skia.UI package.
    val writeJson : path: string -> report: ParityReport -> unit
