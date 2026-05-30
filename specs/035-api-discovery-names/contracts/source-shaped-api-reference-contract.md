# Contract: Source-Shaped API Reference

## Purpose

Package consumers can discover the F# authoring surface for every packable
FS.Skia.UI framework package without assembly reflection or repository source
inspection.

## Required Coverage

- Public namespaces and modules from curated `.fsi` signatures
- Public types, records, discriminated unions, union cases, constructors, and
  fields
- Public values/functions with parameter names and return shapes
- XML documentation summaries where present
- Common construction patterns for Scene primitives, `Paint`, viewer records,
  geometry records, Controls front doors, and Controls.Elmish adapters

## Required Report Fields

Each generated package reference report must include:

- `packageId`
- `version`
- `sourceSignaturePaths`
- `referencePath` or `packageEntryPath`
- `symbolCount`
- `sampledSymbols`
- `omittedSymbols`
- `diagnostics`

## Acceptance

- F# source spelling is preserved when reflection spelling differs.
- A clean consumer can locate the reference from package validation output or
  package artifacts.
- The sampled public API coverage reaches the feature success criterion without
  using reflection as the authoring source.
