---
name: fsdocs-api-doc
description: This skill should be used when the user asks to "add doc comments", "improve XML docs", "document the API", "add XML documentation", "write doc comments for F#", "document public API", "add summary comments", or needs to write or improve `///` XML documentation comments in F# source files. This skill edits .fs source files, not docs/ content files.
version: 0.1.0
---

# Improve XML Doc Comments in F# Source Files

Add and improve `///` XML documentation comments in F# source files so FSharp.Formatting generates high-quality API reference pages.

**Key capabilities:**
- Audit public API surface for missing or low-quality doc comments
- Write idiomatic F# XML doc comments with proper tag usage
- Document F#-specific constructs: discriminated unions, computation expressions, active patterns
- Add namespace-level documentation via sentinel modules
- Organize API docs with categories

## Overview

FSharp.Formatting generates API reference pages from compiled assemblies and their XML documentation files. The quality of these pages depends entirely on the `///` comments in source code. This skill focuses on editing `.fs` source files to improve those comments — it does not create files in `docs/`.

Ensure `<GenerateDocumentationFile>true</GenerateDocumentationFile>` is set in project properties before starting. Without this, the compiler does not emit XML doc files.

## Workflow

### 1. Build to Verify Compilation

Build the project first to confirm it compiles cleanly:

```
dotnet build -c Release
```

Fix any compilation errors before documenting. Documentation work requires a compilable codebase.

### 2. Identify Public API Surface

Scan for all public declarations that will appear in generated API docs:
- Modules and their public functions/values
- Types (records, DUs, classes) and their members
- Type extensions
- Active patterns
- Computation expression builders

Focus on items visible to library consumers. Internal or private members do not appear in standard API docs.

Prioritize by impact: types and functions that most users will encounter first deserve the best documentation. Start with the main entry points and work outward.

### 3. Audit Existing Documentation

For each public API member, check:
- Is there a `<summary>` at minimum?
- Do functions with parameters have `<param>` tags?
- Do functions returning non-obvious types have `<returns>` tags?
- Are there `<example>` blocks for non-trivial usage?
- Are discriminated union cases individually documented?

### 4. Write XML Doc Comments

Apply `///` comments to all public members. Follow the tag priority:

**Essential (every public member):**
- `<summary>` — one to three sentences describing purpose and behavior

**Important (when applicable):**
- `<param name="x">` — describe each parameter's purpose and constraints
- `<returns>` — describe return value, especially for non-obvious types
- `<example>` — show typical usage with a code block
- `<remarks>` — additional context, algorithms, performance notes

**Supplemental:**
- `<exception cref="T:System.ArgumentException">` — document thrown exceptions
- `<see cref="T:Namespace.Type"/>` — cross-reference related types
- `<seealso cref="T:Namespace.Type"/>` — related reading
- `<typeparam name="'a">` — describe generic type parameters

### 5. Add Namespace Documentation

Create a sentinel module for each namespace to provide namespace-level docs:

```fsharp
namespace MyLib.Core

/// <namespacedoc>
/// <summary>
/// Core types and functions for MyLib. Contains the primary domain
/// model and essential operations.
/// </summary>
/// </namespacedoc>
module internal NamespaceDoc = ()
```

Place this at the top of the first file in each namespace. The module must be `internal` — it exists only to carry the `<namespacedoc>` tag.

### 6. Organize with Categories

Group related members using `<category>` tags on types and modules:

```fsharp
/// <summary>String manipulation utilities.</summary>
/// <category>Text Processing</category>
module StringOps =
    // ...
```

Categories create groupings on the namespace page, making large APIs navigable.

### 7. Build and Verify

Build the documentation to verify comments render correctly:

```
dotnet fsdocs build --clean
```

Check the generated API pages in `output/reference/` for formatting issues, broken cross-references, and missing content.

## Quick Reference: Common Tags

```fsharp
/// <summary>Brief description of the member.</summary>
/// <param name="input">Description of the parameter.</param>
/// <returns>Description of the return value.</returns>
/// <example>
/// <code>
/// let result = MyModule.transform "hello"
/// // result = "HELLO"
/// </code>
/// </example>
/// <remarks>Additional detail about behavior or performance.</remarks>
/// <exception cref="T:System.ArgumentException">When input is null.</exception>
/// <see cref="T:MyLib.RelatedType"/>
/// <seealso cref="M:MyLib.OtherModule.relatedFunction"/>
```

## F#-Specific Documentation Patterns

### Discriminated Unions

Document each case separately — they appear as individual entries in API docs:

```fsharp
/// <summary>Represents a validation result.</summary>
type ValidationResult =
    /// <summary>Validation passed successfully.</summary>
    | Valid
    /// <summary>Validation failed with error messages.</summary>
    /// <param name="errors">List of validation error messages.</param>
    | Invalid of errors: string list
```

### Computation Expression Builders

Document the builder type and its key methods. Focus on the user-facing CE syntax, not internal builder mechanics:

```fsharp
/// <summary>
/// Builder for the <c>result</c> computation expression.
/// Enables railway-oriented programming with Result types.
/// </summary>
/// <example>
/// <code>
/// let validated = result {
///     let! name = validateName input
///     let! email = validateEmail input
///     return { Name = name; Email = email }
/// }
/// </code>
/// </example>
type ResultBuilder() =
    /// <summary>Wraps a value in <c>Ok</c>.</summary>
    member _.Return(x) = Ok x
    /// <summary>Unwraps a <c>Result</c> and passes the <c>Ok</c> value to the continuation.</summary>
    member _.Bind(m, f) = Result.bind f m
```

### Active Patterns

Document the pattern and what it matches:

```fsharp
/// <summary>
/// Classifies an integer as even or odd.
/// </summary>
/// <param name="n">The integer to classify.</param>
/// <returns><c>Even</c> when divisible by 2, <c>Odd</c> otherwise.</returns>
let (|Even|Odd|) n =
    if n % 2 = 0 then Even else Odd
```

### Type Extensions

Document extension members the same as regular members — they appear alongside the extended type in API docs:

```fsharp
type System.String with
    /// <summary>
    /// Returns <c>None</c> if the string is null or whitespace,
    /// <c>Some value</c> otherwise.
    /// </summary>
    member this.ToOption() =
        if System.String.IsNullOrWhiteSpace(this) then None
        else Some this
```

## Writing Good Summaries

The `<summary>` tag is the most visible part of API documentation. It appears in namespace listings, search results, and tooltips. Write summaries that help users decide whether this is what they need.

**Focus on behavior and purpose:**
```fsharp
/// <summary>
/// Retries a failing operation with exponential backoff,
/// doubling the delay after each attempt.
/// </summary>
```

**Avoid restating the signature:**
```fsharp
// BAD: Tells the reader nothing they can't see from the type
/// <summary>Takes a string and returns an int option.</summary>
let tryParseInt (s: string) : int option = ...

// GOOD: Explains purpose and behavior
/// <summary>
/// Parses an integer from a string, returning None for invalid input.
/// Handles leading/trailing whitespace but not thousands separators.
/// </summary>
let tryParseInt (s: string) : int option = ...
```

**Include key constraints and edge cases in the summary when short enough:**
```fsharp
/// <summary>
/// Returns the first N elements. If the collection has fewer than N elements,
/// returns all elements without error.
/// </summary>
```

For longer discussion of edge cases, algorithms, or performance, use `<remarks>` to keep the summary concise.

## Quality Checklist

Before considering API docs complete:
- [ ] Every public module has a `<summary>`
- [ ] Every public function has `<summary>` and `<param>` tags
- [ ] Every DU has case-level documentation
- [ ] Every namespace has a `<namespacedoc>` sentinel module
- [ ] Cross-references use correct `cref` syntax
- [ ] An `<example>` exists for non-trivial functions
- [ ] `dotnet fsdocs build` produces no doc-related warnings

## Best Practices

**DO:**
- ✅ Write summaries that explain purpose and behavior, not implementation
- ✅ Document each DU case individually — they are separate API entries
- ✅ Use `<example>` liberally — working examples are the most valuable doc
- ✅ Cross-reference related types with `<see cref="..."/>`
- ✅ Keep summaries concise — use `<remarks>` for extended discussion

**DON'T:**
- ❌ Restate the function signature in prose ("Takes a string and returns an int")
- ❌ Leave DU cases undocumented — each case needs its own `///`
- ❌ Use `<c>` for multi-line code — use `<code>` blocks instead
- ❌ Skip the build verification — broken `cref` links are silent failures
- ❌ Document private or internal members unless building with `--nonpublic`

## Additional Resources

### Reference Files
- **`references/xml-doc-patterns.md`** — Comprehensive XML doc tag reference with F#-specific examples, cross-referencing patterns, markdown-in-comments mode, and common pitfalls

## Implementation Workflow

1. Build the project to verify it compiles (`dotnet build -c Release`)
2. Identify all public API members that need documentation
3. Audit existing doc comments for gaps and quality issues
4. Add or improve `///` XML doc comments in `.fs` source files
5. Add `<namespacedoc>` sentinel modules for each namespace
6. Add `<category>` tags to organize large APIs
7. Build docs (`dotnet fsdocs build --clean`) and verify rendered output

Focus on writing doc comments that help library consumers understand purpose and usage, not implementation details.

## Scope / when to use

Use this skill to **write or improve `///` XML doc comments in `.fs` source files**
so the generator produces high-quality API reference pages. It edits source, not
`docs/` content. For narrative tutorials use [[fsdocs-examples]]; for architecture
and design prose use [[fsdocs-technical]]; to actually render and verify the pages
use [[fsdocs-build]]; and if the project is not yet wired for docs, run
[[fsdocs-setup]] first.

## Driven-library API

This skill drives the external **FSharp.Formatting** API-docs pipeline — there is
no FS.Skia.UI backing `.fsi` surface. The driven contract is the F# XML
documentation tag set consumed by the generator: `<summary>`, `<param>`,
`<returns>`, `<example>`/`<code>`, `<remarks>`, `<exception>`, `<see>`/`<seealso>`
with `cref:T:`/`cref:M:`/`cref:P:` targets, `<typeparam>`, `<category>`, and the
`<namespacedoc>` sentinel-module convention. It requires
`<GenerateDocumentationFile>true</GenerateDocumentationFile>` so the compiler emits
the XML the tool reads.

## Persistent-problem mandate

`cref` target syntax and supported XML-doc tags are version-sensitive and fail
silently (broken links, dropped sections) rather than erroring. Consult the
**official online docs first** — both the FSharp.Formatting API-docs guide and the
Microsoft F# XML-documentation reference — before relying on a tag or `cref` form
documented here, and verify rendered output rather than trusting the source builds.

## Related

- [[fsdocs-setup]] — enable `GenerateDocumentationFile` and scaffold the site
- [[fsdocs-build]] — render and verify the API pages this skill feeds
- [[fsdocs-examples]] — literate tutorials that complement API reference
- [[fsdocs-technical]] — narrative docs that `cref`-link back to these API pages

## Sources

- FSharp.Formatting API-docs guide: https://fsprojects.github.io/FSharp.Formatting/apidocs.html
- F# XML documentation reference (Microsoft Learn): https://learn.microsoft.com/dotnet/fsharp/language-reference/xml-documentation
- FSharp.Formatting repository: https://github.com/fsprojects/FSharp.Formatting
