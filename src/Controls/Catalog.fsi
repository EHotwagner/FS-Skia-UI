namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type CatalogAccessibility =
    { Role: string
      NameSource: string
      StateMetadata: string list
      FocusBehavior: string
      KeyboardOperation: string
      ContrastEvidence: string }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlDefinition =
    { Id: string
      DisplayName: string
      Category: string
      Module: string
      Purpose: string
      RequiredAttributes: string list
      CommonAttributes: string list
      Events: string list
      VisualStates: string list
      Accessibility: CatalogAccessibility
      Examples: string list
      Tests: string list
      Evidence: string list
      SupportStatus: string
      Owner: string }

/// Public contract module exposed by this FS.Skia.UI package.
module Catalog =
    /// Public contract function exposed by this FS.Skia.UI package.
    val supportedControls: ControlDefinition list
    /// Public contract function exposed by this FS.Skia.UI package.
    val standardSchema: ControlSchema list
    /// Public contract function exposed by this FS.Skia.UI package.
    val knownControlKinds: unit -> StandardControlKind list
    /// Public contract function exposed by this FS.Skia.UI package.
    val requiredAttributes: kind: StandardControlKind -> StandardAttributeName list
    /// Public contract function exposed by this FS.Skia.UI package.
    val supportedAttributes: kind: StandardControlKind -> StandardAttributeName list
    /// Public contract function exposed by this FS.Skia.UI package.
    val supportedEvents: kind: StandardControlKind -> StandardEventKind list
    /// Public contract function exposed by this FS.Skia.UI package.
    val validateStandardControl: control: Control<'msg> -> ControlDiagnostic list
    /// Public contract function exposed by this FS.Skia.UI package.
    val supportedCount: unit -> int
    /// Public contract function exposed by this FS.Skia.UI package.
    val categories: unit -> string list
    /// Public contract function exposed by this FS.Skia.UI package.
    val validate: unit -> ControlDiagnostic list
    /// Public contract function exposed by this FS.Skia.UI package.
    val markdownSummary: unit -> string
