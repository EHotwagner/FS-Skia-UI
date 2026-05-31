---
name: fs-skia-testing
description: Assert generated-product expectations and evidence in a governed FS.Skia.UI product.
---

# Testing Capability

## Scope

Use this skill for product test and evidence helpers: declaring
generated-product expectations, classifying local package drift, and building
evidence reports from pure inputs.

## Public Contract

The signatures you consume are bundled with this product at
`docs/api-surface/Testing/Testing.fsi`. The helper modules
(`GeneratedProductAssertions`, `LocalConsumerPackages`, `EvidenceReports`) are
pure functions over value records.

## Usage

```fsharp
open FS.Skia.UI.Testing

// Declare what this product expects of its own generated output.
let expectation =
    { Profile = "governed"
      RequiredFiles = [ "src/Product/Product.fsproj"; "docs/effects-boundary.md" ]
      ForbiddenPrefixes = [ "samples/" ]
      PackageReferences =
        [ { PackageId = "FS.Skia.UI.Scene"; Required = true }
          { PackageId = "FS.Skia.UI.Testing"; Required = true } ] }

let summary = GeneratedProductAssertions.summarize expectation
```

## Build Commands

Run `./fake.sh build -t Dev` then `./fake.sh build -t Verify` in this product.

## Test Commands

Run `./fake.sh build -t Test` to evaluate product expectations and evidence
reports.

## Evidence

Build and write evidence with `EvidenceReports.build` / `write` into this
product's `readiness/` paths. Do not copy framework readiness reports into the
product.

## Package Boundary

Keep assertion and evidence logic pure over value records; let your test runner
and `Verify` target perform the actual file and process I/O.

## Generated Product

The governed profile selects Testing alongside Scene so product tests can assert
their own generated structure and package pins.
