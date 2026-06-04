namespace FS.Skia.UI.Controls

/// Public contract module exposed by this FS.Skia.UI package.
module Diagnostics =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create:
        controlId: ControlId option ->
        kind: ControlKind ->
        code: ControlDiagnosticCode ->
        severity: ControlDiagnosticSeverity ->
        message: string ->
            ControlDiagnostic

    /// Public contract function exposed by this FS.Skia.UI package.
    val missingRequired: controlId: ControlId option -> kind: ControlKind -> name: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val duplicateAttribute: controlId: ControlId option -> kind: ControlKind -> name: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val missingAccessibility: controlId: ControlId option -> kind: ControlKind -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val keyCollision: key: ControlId -> kind: ControlKind -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedEnvironment: kind: ControlKind -> capability: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedStandardAttribute: kind: StandardControlKind -> name: StandardAttributeName -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedStandardEvent: kind: StandardControlKind -> eventKind: StandardEventKind -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val missingStandardAttribute: kind: StandardControlKind -> name: StandardAttributeName -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val customExtension: kind: string -> extensionName: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val stalePackageReference: packageId: string -> path: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val dependencyLeak: packageId: string -> dependencyPath: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val catalogOmission: controlId: string -> requiredField: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val duplicateRuntimeDefinition: runtimeName: string -> path: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val staleEventTarget: controlId: ControlId -> eventKind: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedScopeExpansion: scopeName: string -> owner: string -> ControlDiagnostic
