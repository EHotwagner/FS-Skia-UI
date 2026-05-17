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
    val stalePackageReference: packageId: string -> path: string -> ControlDiagnostic
    val dependencyLeak: packageId: string -> dependencyPath: string -> ControlDiagnostic
    val catalogOmission: controlId: string -> requiredField: string -> ControlDiagnostic
    val duplicateRuntimeDefinition: runtimeName: string -> path: string -> ControlDiagnostic
    val staleEventTarget: controlId: ControlId -> eventKind: string -> ControlDiagnostic
    val unsupportedScopeExpansion: scopeName: string -> owner: string -> ControlDiagnostic
