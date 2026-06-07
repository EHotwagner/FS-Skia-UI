# XML Documentation Patterns for F#

Comprehensive reference for XML doc comment tags and F#-specific documentation patterns used with FSharp.Formatting.

## Standard XML Doc Tags

### `<summary>`

The most important tag. Every public member should have one. Keep it to one to three sentences.

```fsharp
/// <summary>
/// Parses a JSON string into a strongly-typed value.
/// Returns Error with a diagnostic message if parsing fails.
/// </summary>
let parse<'T> (json: string) : Result<'T, string> = ...
```

For short descriptions, use the single-line form:

```fsharp
/// <summary>The default retry count for failed operations.</summary>
let [<Literal>] DefaultRetryCount = 3
```

### `<param>`

Document each parameter. Describe purpose, valid ranges, and constraints — not the type (which is already visible).

```fsharp
/// <summary>Retries an operation with exponential backoff.</summary>
/// <param name="maxRetries">Maximum number of retry attempts. Must be positive.</param>
/// <param name="baseDelay">Initial delay between retries. Doubles on each attempt.</param>
/// <param name="operation">The operation to retry. Called with the attempt number (0-based).</param>
let retry (maxRetries: int) (baseDelay: TimeSpan) (operation: int -> Async<'T>) : Async<'T> = ...
```

### `<returns>`

Describe what the function returns, especially when the return type is generic or the meaning is non-obvious.

```fsharp
/// <summary>Finds the first element matching the predicate.</summary>
/// <param name="predicate">Function to test each element.</param>
/// <returns>
/// <c>Some value</c> if an element matches, <c>None</c> if no element matches.
/// </returns>
let tryFind (predicate: 'T -> bool) (list: 'T list) : 'T option = ...
```

### `<example>`

Show working usage. Use `<code>` blocks for multi-line examples.

```fsharp
/// <summary>Creates a parser that matches a specific string.</summary>
/// <example>
/// <code>
/// let parser = Parser.string "hello"
/// let result = Parser.run parser "hello world"
/// // result = Ok ("hello", " world")
/// </code>
/// </example>
let string (expected: string) : Parser<string> = ...
```

### `<remarks>`

Extended discussion: algorithms, performance characteristics, thread safety, historical context.

```fsharp
/// <summary>Sorts elements using a stable merge sort.</summary>
/// <remarks>
/// Time complexity is O(n log n) in all cases. Space complexity is O(n)
/// due to temporary arrays during merging. Stable: equal elements preserve
/// their relative order.
///
/// Prefer <see cref="M:MyLib.Collections.Array.sortInPlace"/> when mutation
/// is acceptable and memory usage matters.
/// </remarks>
let sort (comparison: 'T -> 'T -> int) (items: 'T array) : 'T array = ...
```

### `<exception>`

Document exceptions the function may throw. Only document exceptions that callers should handle.

```fsharp
/// <summary>Opens a file for reading.</summary>
/// <param name="path">Absolute or relative file path.</param>
/// <exception cref="T:System.IO.FileNotFoundException">
/// The file at <paramref name="path"/> does not exist.
/// </exception>
/// <exception cref="T:System.UnauthorizedAccessException">
/// Insufficient permissions to read the file.
/// </exception>
let openFile (path: string) : FileStream = ...
```

### `<see>` and `<seealso>`

Inline cross-reference (`<see>`) and "related reading" (`<seealso>`).

```fsharp
/// <summary>
/// Converts a <see cref="T:MyLib.Ast.Expression"/> to a string representation.
/// </summary>
/// <seealso cref="M:MyLib.Ast.Expression.Parse"/>
/// <seealso cref="T:MyLib.Formatting.Printer"/>
let toString (expr: Expression) : string = ...
```

### `<typeparam>`

Document generic type parameters when their constraints or purpose is non-obvious.

```fsharp
/// <summary>A thread-safe, bounded buffer.</summary>
/// <typeparam name="'T">
/// The element type. Must support structural equality for deduplication.
/// </typeparam>
type BoundedBuffer<'T when 'T : equality>(capacity: int) = ...
```

### `<c>` and `<code>`

Use `<c>` for inline code references, `<code>` for multi-line blocks.

```fsharp
/// <summary>
/// Returns <c>true</c> when the input is a valid email address.
/// </summary>
```

```fsharp
/// <example>
/// <code>
/// let config = {
///     Host = "localhost"
///     Port = 8080
/// }
/// </code>
/// </example>
```

### `<paramref>` and `<typeparamref>`

Reference parameters by name within descriptions.

```fsharp
/// <summary>
/// Applies <paramref name="transform"/> to each element, returning a new list.
/// Elements for which <paramref name="transform"/> returns <c>None</c> are excluded.
/// </summary>
/// <param name="transform">Function applied to each element.</param>
let choose (transform: 'T -> 'U option) (items: 'T list) : 'U list = ...
```

## F#-Specific Patterns

### Documenting Discriminated Unions

Each case is a separate entry in API docs. Document the type and each case individually:

```fsharp
/// <summary>
/// Represents an HTTP response status.
/// </summary>
/// <remarks>
/// Use pattern matching to handle each status category.
/// See <see cref="T:MyLib.Http.Response"/> for the full response type.
/// </remarks>
type HttpStatus =
    /// <summary>Request succeeded (200-299).</summary>
    /// <param name="code">The specific HTTP status code.</param>
    | Success of code: int
    /// <summary>Client error (400-499).</summary>
    /// <param name="code">The specific HTTP status code.</param>
    /// <param name="message">Human-readable error description.</param>
    | ClientError of code: int * message: string
    /// <summary>Server error (500-599).</summary>
    /// <param name="code">The specific HTTP status code.</param>
    | ServerError of code: int
    /// <summary>Request timed out before receiving a response.</summary>
    | Timeout
```

Named fields on DU cases produce better documentation than unnamed tuple members.

### Documenting Computation Expression Builders

Focus on what the CE syntax looks like from the user's perspective. Document the builder type with a usage example, then document each operation method:

```fsharp
/// <summary>
/// Builder for the <c>validation</c> computation expression.
/// Accumulates all errors instead of short-circuiting on the first failure.
/// </summary>
/// <example>
/// <code>
/// let result = validation {
///     let! name = validateName input
///     and! email = validateEmail input
///     and! age = validateAge input
///     return { Name = name; Email = email; Age = age }
/// }
/// </code>
/// </example>
type ValidationBuilder() =
    /// <summary>Wraps a value in a successful validation result.</summary>
    member _.Return(x) = Validation.ok x

    /// <summary>
    /// Sequences two validations, accumulating errors from both.
    /// Corresponds to <c>and!</c> in the CE syntax.
    /// </summary>
    member _.MergeSources(v1, v2) = Validation.zip v1 v2

    /// <summary>
    /// Unwraps a validation result and passes the value to the continuation.
    /// Corresponds to <c>let!</c> in the CE syntax.
    /// </summary>
    member _.Bind(m, f) = Validation.bind f m
```

### Documenting Active Patterns

#### Complete Active Patterns

```fsharp
/// <summary>
/// Classifies a character as a letter, digit, or other symbol.
/// </summary>
/// <param name="c">The character to classify.</param>
/// <example>
/// <code>
/// match 'A' with
/// | Letter -> "letter"
/// | Digit -> "digit"
/// | Symbol -> "symbol"
/// </code>
/// </example>
let (|Letter|Digit|Symbol|) (c: char) =
    if Char.IsLetter c then Letter
    elif Char.IsDigit c then Digit
    else Symbol
```

#### Partial Active Patterns

```fsharp
/// <summary>
/// Matches strings that represent valid integers.
/// </summary>
/// <param name="input">The string to parse.</param>
/// <returns>
/// <c>Some int</c> if the string is a valid integer, <c>None</c> otherwise.
/// </returns>
/// <example>
/// <code>
/// match "42" with
/// | Int n -> printfn "Got integer: %d" n
/// | _ -> printfn "Not an integer"
/// </code>
/// </example>
let (|Int|_|) (input: string) =
    match System.Int32.TryParse(input) with
    | true, n -> Some n
    | _ -> None
```

#### Parameterized Active Patterns

```fsharp
/// <summary>
/// Matches strings using a regular expression pattern.
/// Captures are returned as a list of strings.
/// </summary>
/// <param name="pattern">The regex pattern to match against.</param>
/// <param name="input">The string to test.</param>
/// <returns><c>Some captures</c> if the pattern matches, <c>None</c> otherwise.</returns>
let (|Regex|_|) (pattern: string) (input: string) =
    let m = System.Text.RegularExpressions.Regex.Match(input, pattern)
    if m.Success then Some [ for g in m.Groups -> g.Value ]
    else None
```

### Documenting Type Providers (Erased Types)

Type providers generate types at compile time. Document the provider itself and give usage examples:

```fsharp
/// <summary>
/// Type provider for reading CSV files with column types inferred from a sample.
/// </summary>
/// <param name="Sample">Path to a sample CSV file or inline CSV data.</param>
/// <param name="Separators">Column separators (default: comma).</param>
/// <param name="HasHeaders">Whether the first row contains headers (default: true).</param>
/// <example>
/// <code>
/// type MyCsv = CsvProvider&lt;"data/sample.csv"&gt;
/// let data = MyCsv.Load("data/actual.csv")
/// for row in data.Rows do
///     printfn "%s: %d" row.Name row.Age
/// </code>
/// </example>
[<TypeProvider>]
type CsvProvider = ...
```

### Documenting Module Functions (Curried Style)

F# functions are curried. Document the logical operation, not individual partial applications:

```fsharp
/// <summary>
/// Maps a function over the values of a map, preserving keys.
/// </summary>
/// <param name="mapping">Function to apply to each value.</param>
/// <param name="table">The input map.</param>
/// <returns>A new map with the same keys and transformed values.</returns>
/// <example>
/// <code>
/// Map.ofList [ "a", 1; "b", 2 ]
/// |> Map.mapValues (fun v -> v * 10)
/// // map [("a", 10); ("b", 20)]
/// </code>
/// </example>
let mapValues (mapping: 'V1 -> 'V2) (table: Map<'K, 'V1>) : Map<'K, 'V2> = ...
```

### Documenting Type Extensions

```fsharp
/// Extensions for <see cref="T:System.String"/> providing
/// option-based alternatives to nullable methods.
[<AutoOpen>]
module StringExtensions =

    type System.String with
        /// <summary>
        /// Splits the string and returns <c>None</c> for empty segments.
        /// </summary>
        /// <param name="separator">The separator character.</param>
        member this.SplitNonEmpty(separator: char) : string list = ...
```

## Cross-Referencing with `cref`

### Syntax Reference

| Target | cref Syntax |
|---|---|
| Type / Record / DU | `cref:T:Namespace.TypeName` |
| Module | `cref:T:Namespace.ModuleName` |
| Method / Function | `cref:M:Namespace.ModuleName.functionName` |
| Property | `cref:P:Namespace.TypeName.PropertyName` |
| Constructor | `cref:M:Namespace.TypeName.#ctor` |

### Tips

- Always use fully qualified names — partial names may not resolve.
- For generic types, omit the type parameter: `cref:T:MyLib.Result` not `cref:T:MyLib.Result<'T, 'E>`.
- Module functions use `M:` (method), not `T:` (type).
- Test cross-references by building and checking for warnings.

## Markdown-in-Comments Mode

FSharp.Formatting supports Markdown inside XML doc comments when the project sets `<UsesMarkdownComments>true</UsesMarkdownComments>`. This allows:

```fsharp
/// **Parses** a JSON string into a typed value.
///
/// Returns `Error` with a message if parsing fails.
///
/// ## Example
///
/// ```fsharp
/// let result = parse<int> "42"
/// ```
let parse<'T> (json: string) : Result<'T, string> = ...
```

This mode replaces XML tags with Markdown equivalents. It is simpler to write but less structured. Trade-offs:
- **Pro:** More natural to write, better readability in source
- **Con:** Less precise than XML tags, no `<param>` equivalent
- **Recommendation:** Use standard XML tags for public libraries, Markdown mode for internal projects

Enable per-project in `.fsproj`:

```xml
<PropertyGroup>
    <UsesMarkdownComments>true</UsesMarkdownComments>
</PropertyGroup>
```

## The `<exclude/>` Tag

Exclude a member from generated API docs entirely:

```fsharp
/// <exclude/>
module internal Helpers =
    let internalOnly () = ...
```

Use sparingly. Prefer making members `internal` or `private` instead. `<exclude/>` is for cases where a member must be public (e.g., for serialization) but should not appear in API docs.

## Namespace Documentation with `<namespacedoc>`

Create a sentinel module in each namespace:

```fsharp
namespace MyLib.Parsing

/// <namespacedoc>
/// <summary>
/// Types and functions for parsing structured text.
/// Includes combinators for building complex parsers from simple primitives.
/// </summary>
/// </namespacedoc>
module internal NamespaceDoc = ()
```

Rules:
- The module must be `internal` — it carries metadata only
- Name it `NamespaceDoc` by convention
- Place it in the first file of the namespace (alphabetically)
- One per namespace

## Common Pitfalls

### Missing closing tags
XML doc comments must be well-formed XML. Missing a closing `</summary>` or `</param>` produces compiler warnings and may cause FSharp.Formatting to skip the member.

### `<c>` vs `<code>` confusion
`<c>` is inline code (like Markdown backticks). `<code>` is a block (like Markdown triple backticks). Using `<code>` inline or `<c>` for blocks produces poor formatting.

### Documenting module-level `let` bindings
Module-level `let` bindings without type annotations may not generate parameter documentation correctly. Add explicit type annotations to documented public functions.

### Generic type references in `cref`
Do not include generic parameters in `cref` attributes: use `cref:T:MyLib.Option` not `cref:T:MyLib.Option<'T>`. The backtick notation (`Option`1`) is not supported.

### Multiline summaries with blank lines
Blank lines inside `<summary>` may cause FSharp.Formatting to truncate the summary. Keep summaries as continuous text without blank lines.

### Private members in public types
FSharp.Formatting only documents public members by default. Do not add `///` comments to private members unless building with `--nonpublic`.

### `let` bindings vs `member` in doc comments
Module-level `let` bindings and type `member` declarations both support `///` comments. However, `let` bindings in modules without explicit type annotations may produce less precise parameter documentation. Always add type annotations to documented public `let` bindings.

### Special characters in XML
XML doc comments are XML. Characters like `<`, `>`, and `&` must be escaped as `&lt;`, `&gt;`, `&amp;` in text content (but not inside `<code>` blocks, which are treated as CDATA by most processors). When in doubt, use `<c>` or `<code>` to wrap code that contains these characters.

## Tag Priority Guide

When documenting a new member, apply tags in this order:

1. `<summary>` — always (every public member)
2. `<param>` — for each parameter on functions and methods
3. `<returns>` — when the return type is non-obvious
4. `<example>` — for any member that would benefit from a usage demonstration
5. `<remarks>` — for extended discussion of algorithms, edge cases, threading
6. `<exception>` — for each exception the member may throw
7. `<see>` / `<seealso>` — for related types and functions
8. `<typeparam>` — for generic type parameters with non-obvious constraints

Do not add tags just to have them. An empty `<returns>Returns the result.</returns>` adds no value. Only include tags that provide information beyond what the type signature already communicates.
