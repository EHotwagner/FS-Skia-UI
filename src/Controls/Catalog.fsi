namespace FS.Skia.UI.Controls

type CatalogAccessibility =
    { Role: string
      NameSource: string
      StateMetadata: string list
      FocusBehavior: string
      KeyboardOperation: string
      ContrastEvidence: string }

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

module Catalog =
    val supportedControls: ControlDefinition list
    val standardSchema: ControlSchema list
    val knownControlKinds: unit -> StandardControlKind list
    val requiredAttributes: kind: StandardControlKind -> StandardAttributeName list
    val supportedAttributes: kind: StandardControlKind -> StandardAttributeName list
    val supportedEvents: kind: StandardControlKind -> StandardEventKind list
    val validateStandardControl: control: Control<'msg> -> ControlDiagnostic list
    val supportedCount: unit -> int
    val categories: unit -> string list
    val validate: unit -> ControlDiagnostic list
    val markdownSummary: unit -> string
