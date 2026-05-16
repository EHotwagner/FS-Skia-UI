namespace FS.Skia.UI.Controls

module Diagnostics =
    val create:
        controlId: ControlId option ->
        kind: ControlKind ->
        code: ControlDiagnosticCode ->
        severity: ControlDiagnosticSeverity ->
        message: string ->
            ControlDiagnostic

    val missingRequired: controlId: ControlId option -> kind: ControlKind -> name: string -> ControlDiagnostic
    val duplicateAttribute: controlId: ControlId option -> kind: ControlKind -> name: string -> ControlDiagnostic
    val missingAccessibility: controlId: ControlId option -> kind: ControlKind -> ControlDiagnostic
    val keyCollision: key: ControlId -> kind: ControlKind -> ControlDiagnostic
    val unsupportedEnvironment: kind: ControlKind -> capability: string -> ControlDiagnostic
