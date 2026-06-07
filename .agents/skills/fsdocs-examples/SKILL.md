---
name: fsdocs-examples
description: This skill should be used when the user asks to "write documentation examples", "create literate scripts", "add fsx docs", "write a tutorial", "create a how-to guide", "add example scripts to docs", "write literate F# documentation", or needs to create `.fsx` literate scripts in the `docs/` directory that teach library usage through executable, narrative-driven examples.
version: 0.1.0
---

# Write Literate F# Documentation Scripts

Create `.fsx` literate scripts in `docs/` that teach library usage through executable examples with interleaved narrative.

**Key capabilities:**
- Create literate `.fsx` files with proper FSharp.Formatting frontmatter
- Interleave narrative documentation with executable F# code
- Use directives to control output display (`include-output`, `include-it`, `include-value`)
- Organize examples by feature and use-case with proper navigation ordering

## Overview

FSharp.Formatting processes `.fsx` files in `docs/` as literate scripts. These combine Markdown narrative (in `(**  **)` doc comments) with executable F# code. When built with `--eval`, the code runs and its output can be captured and displayed in the generated pages.

Literate scripts are organized by feature or use-case, not by namespace or API surface. They answer "How do I accomplish X?" rather than "What does function Y do?".

## Workflow

### 1. Identify What to Demonstrate

Determine the topic: a feature, use-case, pattern, or integration. Each script should focus on one coherent topic. Ask:
- What problem does this solve for the user?
- What is the simplest working example?
- What are the important variations or advanced uses?

### 2. Plan the Narrative Arc

Structure the script as a progressive tutorial:
1. **Introduction** — what this example covers and why it matters
2. **Setup** — assembly references, namespace opens, any prerequisite configuration
3. **Basic usage** — simplest possible working example
4. **Intermediate patterns** — common variations, composition, error handling
5. **Advanced usage** — complex scenarios, performance considerations (optional)
6. **Summary** — recap key points, link to related topics

### 3. Create the .fsx File with Frontmatter

Place the file in `docs/` with a descriptive name. The first lines must be an fsdocs frontmatter block:

```fsharp
(**
---
title: Working with Parsers
category: Tutorials
categoryindex: 2
index: 3
description: Learn how to build and compose parsers.
---
*)
```

Frontmatter fields:
- `title` — page title (appears in navigation and browser tab)
- `category` — navigation group name
- `categoryindex` — sort order of the category in the sidebar (lower = higher)
- `index` — sort order within the category (lower = higher)
- `description` — brief summary for search and metadata

### 4. Write Narrative with Interleaved Code

Alternate between Markdown narrative in `(**  **)` blocks and F# code:

```fsharp
(**
## Getting Started

First, reference the library and open the namespace:
*)

#r "../src/MyLib/bin/Release/net8.0/MyLib.dll"
open MyLib.Parsers

(**
Create a simple parser that matches a single character:
*)

let charParser = Parser.char 'a'
let result = Parser.run charParser "abc"
(*** include-it ***)
```

The `(*** include-it ***)` directive inserts the FSI evaluation result of the preceding expression.

### 5. Use Output Directives

Control what appears in the rendered page:

| Directive | Effect |
|---|---|
| `(*** include-output ***)` | Show `stdout` output from preceding code |
| `(*** include-it ***)` | Show FSI evaluation result of preceding expression |
| `(*** include-value: x ***)` | Show the value of a named binding |
| `(*** include-fsi-output ***)` | Show raw FSI output including types |
| `(*** hide ***)` | Hide the next code block from rendered output |
| `(*** do-not-eval ***)` | Show code but skip evaluation |
| `(*** define: name ***)` | Define a named snippet for later inclusion |
| `(*** include: name ***)` | Include a previously defined snippet |
| `(*** condition: prepare ***)` | Only include when building (not in output) |
| `(*** condition: ipynb ***)` | Only include in notebook output |

Place output directives **immediately after** the code block they reference. The directive applies to the code block directly above it.

### 6. Add Assembly References

Literate scripts run as FSI scripts. They need explicit assembly references:

```fsharp
(*** condition: prepare ***)
#r "../src/MyLib/bin/Release/net8.0/MyLib.dll"
(*** condition: fsx ***)
#r "nuget: MyLib"
```

Use `condition: prepare` for local development paths and `condition: fsx` for NuGet references shown to the reader. This way the script evaluates with local DLLs but the rendered page shows the NuGet install instruction.

### 7. Verify with Build

Build with `--eval` to execute the scripts and verify output:

```
dotnet fsdocs build --clean --eval
```

Check that:
- All scripts compile and evaluate without errors
- Output directives show expected values
- Navigation ordering is correct in the generated site

## Writing Good Examples

**Teach one concept per script.** A script about "parsing" should not also cover serialization. Create separate scripts for separate topics.

**Build complexity gradually.** Start with the simplest example that works, then layer on complexity. Every code block should build on what came before.

**Show realistic use cases.** Avoid toy examples that don't resemble real usage. If the library parses JSON, parse actual JSON structures, not `"hello"`.

**Show expected output.** Use `include-it` or `include-value` after expressions so readers see results. Invisible code that "just works" is less helpful than seeing concrete values.

**Handle errors explicitly.** Show what happens with invalid input. Document error cases with the same care as success cases.

**Keep setup minimal.** Use `(*** hide ***)` for boilerplate setup that would distract from the main narrative. But always show assembly references and namespace opens — readers need to know these.

**Cross-reference related pages.** End each script with links to related examples and API docs. This helps readers find the next thing they need.

**Name files descriptively.** Use kebab-case names that describe the content: `working-with-parsers.fsx`, `error-handling.fsx`, `async-patterns.fsx`. Avoid generic names like `tutorial1.fsx`.

## Anatomy of a Literate Script

A complete literate script follows this structure:

```fsharp
(** Frontmatter block — title, category, index *)

(** Assembly references — condition:prepare for local, condition:fsx for NuGet *)

(** Opening narrative — what this page covers *)

(* Namespace opens *)

(** Section heading + explanation *)
(* Code block *)
(*** output directive ***)

(** Next section + explanation *)
(* Code block *)
(*** output directive ***)

(** Summary + cross-references *)
```

Each section alternates between explanation (doc comments) and demonstration (code + output). The narrative should flow naturally so a reader can follow from top to bottom without jumping around.

## Navigation Ordering Conventions

Categories and pages are ordered by their index values (lower numbers appear first):

| `categoryindex` | Typical use |
|---|---|
| 1 | Overview / Getting Started |
| 2 | Tutorials |
| 3 | How-To Guides |
| 4 | Advanced Topics |
| 5 | Reference |

Within each category, use `index` values with gaps (1, 3, 5, 7...) so new pages can be inserted without renumbering.

## Best Practices

**DO:**
- ✅ Organize scripts by feature or use-case, not by API namespace
- ✅ Start with the simplest working example and build up
- ✅ Use `(*** include-it ***)` after expressions to show concrete results
- ✅ Use `(*** hide ***)` for boilerplate that distracts from the narrative
- ✅ Test with `--eval` to verify all examples actually work
- ✅ Use `condition: prepare` / `condition: fsx` for assembly reference switching

**DON'T:**
- ❌ Create one massive script covering everything — split by topic
- ❌ Show code without showing its output — readers want to see results
- ❌ Use toy examples that don't resemble real usage
- ❌ Forget assembly references — `.fsx` files need explicit `#r` directives
- ❌ Place output directives before the code block — they must follow it
- ❌ Skip the `--eval` build — unevaluated scripts may contain errors

## Additional Resources

### Reference Files
- **`references/literate-directives.md`** — Complete directive reference with positioning rules, edge cases, evaluator behavior, and troubleshooting

### Templates
- **`templates/example.fsx`** — Starter literate script template with frontmatter, assembly references, and section patterns

## Implementation Workflow

1. Determine the feature or use-case to document
2. Plan the narrative arc: introduction, setup, basic, advanced, summary
3. Create `.fsx` file in `docs/` with proper frontmatter
4. Write interleaved narrative and code sections
5. Add output directives to show evaluation results
6. Verify with `dotnet fsdocs build --clean --eval`

Focus on creating examples that teach through working code, progressing from simple to complex.

## Scope / when to use

Use this skill to **author literate `.fsx` scripts in `docs/`** — executable,
narrative-driven tutorials and how-to guides built with `--eval`. For reference
documentation generated from source, use [[fsdocs-api-doc]]; for architecture and
design prose, use [[fsdocs-technical]]; to build and preview the scripts, use
[[fsdocs-build]]; and if the project lacks a docs pipeline, run [[fsdocs-setup]]
first. This skill is for use-case-oriented teaching content, not API reference.

## Driven-library API

This skill drives the external **FSharp.Formatting** literate-scripting pipeline —
there is no FS.Skia.UI backing `.fsi` surface. The driven contract is the literate
directive set placed in `(*** … ***)` blocks — `include-output`, `include-it`,
`include-value`, `include-fsi-output`, `hide`, `do-not-eval`, `define`/`include`,
and `condition:` (`prepare`/`fsx`/`ipynb`) — plus the `(** … *)` frontmatter
(`title`, `category`, `categoryindex`, `index`, `description`) and `#r` assembly
references. Scripts evaluate only under `dotnet fsdocs build --eval`.

## Persistent-problem mandate

Literate directives, their positioning rules, and `condition:` semantics change
across FSharp.Formatting versions and fail quietly (output simply does not appear).
Consult the **official online docs first** for the literate-scripting guide of the
pinned version before relying on a directive documented here, and always verify
with an `--eval` build rather than assuming the script rendered.

## Related

- [[fsdocs-setup]] — scaffold the `docs/` directory these scripts live in
- [[fsdocs-build]] — `--eval` build that executes and renders the scripts
- [[fsdocs-api-doc]] — API reference that tutorials cross-link to
- [[fsdocs-technical]] — design prose that complements example-driven docs

## Sources

- FSharp.Formatting literate-scripting guide: https://fsprojects.github.io/FSharp.Formatting/literate.html
- FSharp.Formatting documentation: https://fsprojects.github.io/FSharp.Formatting/
- FSharp.Formatting repository: https://github.com/fsprojects/FSharp.Formatting
