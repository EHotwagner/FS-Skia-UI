(**
---
title: FEATURE_NAME
category: Tutorials
categoryindex: 2
index: 1
description: Learn how to use FEATURE_NAME.
---
*)

(** # FEATURE_NAME

Brief introduction: what this feature does and why it matters.
*)

(*** condition: prepare ***)
// Local assembly reference for evaluation
#r "../src/PROJECT_NAME/bin/Release/net8.0/PROJECT_NAME.dll"
(*** condition: fsx ***)
// NuGet reference shown to readers
#r "nuget: PROJECT_NAME"

(**
## Setup

Open the required namespaces:
*)

open PROJECT_NAME

(**
## Basic Usage

Start with the simplest working example:
*)

let result = example ()
(*** include-it ***)

(**
## Working with FEATURE_NAME

Build on the basic example with a more realistic scenario:
*)

let processed =
    result
    |> transform
    |> validate
(*** include-value: processed ***)

(**
## Summary

Key takeaways:
- Point one
- Point two

## Next Steps

- [Related Feature](related-feature.html) — builds on concepts from this page
- [API Reference](../reference/index.html) — detailed type and function documentation
*)
