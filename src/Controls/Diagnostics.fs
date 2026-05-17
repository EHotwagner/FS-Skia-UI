namespace FS.Skia.UI.Controls

module Diagnostics =
    let create controlId kind code severity message =
        { ControlId = controlId
          ControlKind = kind
          Code = code
          Severity = severity
          Message = message
          EvidencePath = None }

    let missingRequired controlId kind (name: string) =
        create controlId kind MissingRequiredAttribute Error $"Missing required attribute `{name}`."

    let duplicateAttribute controlId kind (name: string) =
        create controlId kind DuplicateAttribute Warning $"Duplicate attribute `{name}` uses last-value-wins precedence."

    let missingAccessibility controlId kind =
        create controlId kind MissingAccessibilityMetadata Error "Supported interactive control is missing accessibility metadata."

    let keyCollision key kind =
        create (Some key) kind KeyCollision Error $"Duplicate stable key `{key}` in the control tree."

    let unsupportedEnvironment kind (capability: string) =
        create None kind UnsupportedEnvironment Warning $"Host environment does not expose {capability}; operation reports diagnostics instead."

    let stalePackageReference (packageId: string) (path: string) =
        create None packageId StaleGeneratedReference Error $"Stale package reference `{packageId}` found in `{path}`."

    let dependencyLeak (packageId: string) (dependencyPath: string) =
        create None packageId StaleGeneratedReference Error $"Package `{packageId}` leaks dependency `{dependencyPath}` across the declared boundary."

    let catalogOmission (controlId: string) (requiredField: string) =
        create (Some controlId) "catalog" MissingRequiredAttribute Error $"Catalog entry `{controlId}` is missing required field `{requiredField}`."

    let duplicateRuntimeDefinition (runtimeName: string) (path: string) =
        create None runtimeName DuplicateAttribute Error $"Duplicate runtime definition `{runtimeName}` found in `{path}`."

    let staleEventTarget (controlId: ControlId) (eventKind: string) =
        create (Some controlId) "control-runtime" StaleGeneratedReference Warning $"Stale event target `{controlId}` for event `{eventKind}` was ignored."

    let unsupportedScopeExpansion (scopeName: string) (owner: string) =
        create None scopeName UnsupportedEnvironment Error $"Unsupported scope expansion `{scopeName}` requested by `{owner}`."
