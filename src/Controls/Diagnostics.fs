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
