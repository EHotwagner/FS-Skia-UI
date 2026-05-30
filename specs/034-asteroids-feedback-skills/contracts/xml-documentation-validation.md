# Contract: XML Documentation Validation

## Scope

Packable FS.Skia.UI framework packages ship useful XML documentation for every public `.fsi` surface without changing runtime API shapes.

## Required Behavior

- Every public `.fsi` file compiled by a packable `src/*/*.fsproj` is scanned as part of documentation validation.
- Public modules, types, union cases, records, fields, and values require non-empty XML documentation summaries.
- Parameters and returns require documentation where the public signature exposes them.
- Non-obvious modules, workflows, and factory functions require remarks or examples.
- Generated XML documentation files for packable framework assemblies must exist, be non-empty, and contain member documentation for the public `.fsi` surface.
- Packed NuGet artifacts for packable framework packages must include the XML documentation file that corresponds to the packaged assembly.

## Acceptance Cues

- Validation reports the package id, project path, `.fsi` path, member id, missing documentation category, generated XML file path, packed `.nupkg` path, packed XML entry, failure classification, and next action.
- Missing summaries, missing parameter or return docs, empty generated XML files, missing generated XML files, and missing packed XML entries are hard failures.
- The validation can use bounded malformed fixtures for error-path tests, but real passing evidence must scan repository `.fsi` files, generated XML docs, and packed NuGet artifacts.
