# Literate F# Script Directives Reference

Complete reference for FSharp.Formatting literate script directives, including positioning rules, evaluator behavior, and troubleshooting.

## Directive Syntax

All directives use the triple-star comment syntax:

```fsharp
(*** directive-name ***)
```

Or with a value:

```fsharp
(*** directive-name: value ***)
```

Directives must be on their own line. They cannot appear inline with code or within doc comment blocks.

## Output Directives

These directives control what appears in the rendered documentation page.

### `(*** include-output ***)`

Captures and displays `stdout` output from the preceding code block.

```fsharp
printfn "Hello, world!"
(*** include-output ***)
```

Renders as the code block followed by:
```
Hello, world!
```

**Positioning:** Must immediately follow the code block that produces the output. Cannot reference earlier code blocks.

**Behavior:** Only captures `stdout` (`printfn`, `Console.WriteLine`). Does not capture `stderr` or FSI interaction results.

### `(*** include-it ***)`

Displays the FSI evaluation result (`it` value) of the preceding expression.

```fsharp
[1; 2; 3] |> List.map (fun x -> x * x)
(*** include-it ***)
```

Renders as the code block followed by:
```
val it: int list = [1; 4; 9]
```

**Positioning:** Must immediately follow an expression (not a `let` binding or `do` statement). The expression must return a value.

**Behavior:** Uses FSI's default formatting. For custom types, provide a `ToString()` override or use `%A` formatting.

**Common mistake:** Placing `include-it` after a `let` binding. `let x = 5` does not produce an `it` value — use `include-value: x` instead.

### `(*** include-value: name ***)`

Displays the value of a named binding.

```fsharp
let myResult = compute 42
(*** include-value: myResult ***)
```

**Positioning:** Can appear anywhere after the binding is defined. Does not need to immediately follow the binding.

**Behavior:** Evaluates the binding and formats it using FSI's default formatter. The binding must be in scope.

### `(*** include-fsi-output ***)`

Displays the raw FSI output including type information.

```fsharp
let greet name = sprintf "Hello, %s!" name
(*** include-fsi-output ***)
```

Renders as:
```
val greet: name: string -> string
```

**Use case:** When showing the type signature is more important than the value. Good for demonstrating type inference.

### `(*** include-fsi-merged-output ***)`

Combines both the evaluation result and FSI output in a single block.

```fsharp
let x = 21 * 2
(*** include-fsi-merged-output ***)
```

Renders as:
```
val x: int = 42
```

## Visibility Directives

### `(*** hide ***)`

Hides the next code block from the rendered output. The code still executes during `--eval`.

```fsharp
(*** hide ***)
// This setup code won't appear in the rendered page
let testData = [ 1; 2; 3; 4; 5 ]
let config = { Timeout = 30; Retries = 3 }

(**
Now we can use the pre-configured data:
*)
let result = process config testData
(*** include-it ***)
```

**Scope:** Applies only to the immediately following code block. Subsequent blocks are visible again.

**Use case:** Boilerplate setup, test data initialization, imports that would clutter the narrative.

### `(*** do-not-eval ***)`

Shows the code block in the rendered output but does not evaluate it during `--eval`.

```fsharp
(*** do-not-eval ***)
// This requires a running database
let users = Database.query "SELECT * FROM users"
```

**Use case:** Code that depends on external services, destructive operations, or platform-specific APIs that aren't available during doc builds.

**Scope:** Applies only to the immediately following code block.

## Snippet Directives

### `(*** define: name ***)`

Defines a named snippet that can be included elsewhere.

```fsharp
(*** define: setup ***)
open MyLib
let config = Config.defaults
```

The defined snippet does not appear at the point of definition. It only appears where included.

### `(*** include: name ***)`

Includes a previously defined snippet.

```fsharp
(**
First, set up the configuration:
*)
(*** include: setup ***)
```

**Use case:** Show the same code block in multiple places, or define code early but display it later in the narrative.

### `(*** define-output: name ***)`

Captures the output of a code block with a name for later inclusion.

```fsharp
(*** define-output: demo ***)
printfn "Result: %d" (2 + 2)
```

### `(*** include-output: name ***)`

Includes previously captured named output.

```fsharp
(**
The output of the computation is:
*)
(*** include-output: demo ***)
```

### `(*** include-it: name ***)`

Includes the `it` value from a named code block.

```fsharp
(*** define-output: calc ***)
List.sum [1..100]

(**
The sum of 1 to 100 is:
*)
(*** include-it: calc ***)
```

## Conditional Directives

### `(*** condition: prepare ***)`

Content only included during build/evaluation, not in rendered output.

```fsharp
(*** condition: prepare ***)
#r "../src/MyLib/bin/Release/net8.0/MyLib.dll"
(*** condition: fsx ***)
#r "nuget: MyLib, 1.0.0"
```

**Common pattern:** Use `condition: prepare` for local assembly references and `condition: fsx` for the NuGet reference shown in the rendered page.

### `(*** condition: fsx ***)`

Content only included when rendering for `.fsx` output format.

### `(*** condition: ipynb ***)`

Content only included when rendering for Jupyter notebook output.

### `(*** condition: html ***)`

Content only included when rendering for HTML output.

## Raw Content Directive

### `(*** raw ***)`

Passes the next block through as raw HTML/Markdown without processing.

```fsharp
(*** raw ***)
(**
<div class="custom-alert">
This is raw HTML that won't be processed by the literate engine.
</div>
*)
```

## Frontmatter

Literate scripts use YAML frontmatter inside a doc comment at the top of the file:

```fsharp
(**
---
title: Page Title
category: Category Name
categoryindex: 1
index: 1
description: Brief page description.
---
*)
```

### Frontmatter Fields

| Field | Required | Purpose |
|---|---|---|
| `title` | Yes | Page title in navigation and `<title>` tag |
| `category` | Yes | Sidebar group name |
| `categoryindex` | Yes | Sort order of the category (lower = first) |
| `index` | Yes | Sort order within category (lower = first) |
| `description` | No | Meta description for search engines |

### Navigation Ordering

Pages are grouped by `category` and sorted by `index`. Categories are sorted by `categoryindex`. The first page in a category (lowest `index`) becomes the category landing page.

Recommended index gaps: use 1, 3, 5, 7... to allow insertions without renumbering.

## FSI Evaluator Behavior

### How `--eval` Works

When `dotnet fsdocs build --eval` runs:
1. Each `.fsx` file is processed by an FSI session
2. Code blocks execute sequentially in a shared session (bindings persist across blocks)
3. Output directives capture the FSI state at each point
4. If any code block fails, the entire file fails

### Session State

All code blocks in a single `.fsx` file share one FSI session. Bindings from earlier blocks are available in later blocks:

```fsharp
let x = 42        // Block 1: defines x

(**
Later we can use x:
*)

let y = x + 8     // Block 2: uses x from Block 1
(*** include-it ***)
```

### Assembly References

`.fsx` files need explicit assembly references. These are not inherited from the project:

```fsharp
#r "../src/MyLib/bin/Release/net8.0/MyLib.dll"
```

The path is relative to the `.fsx` file location (in `docs/`). Always reference Release builds.

For NuGet references:

```fsharp
#r "nuget: Newtonsoft.Json, 13.0.3"
```

### Evaluation Failures

When `--eval` fails:

**Compilation error in script:**
```
error FS0039: The value or constructor 'foo' is not defined.
```
Fix: Ensure all references are present and namespaces are opened.

**Runtime error during evaluation:**
```
System.NullReferenceException: ...
```
Fix: Add null checks or use `do-not-eval` for code requiring external state.

**Missing assembly:**
```
error FS0074: Could not load file or assembly 'MyLib'
```
Fix: Verify the `#r` path points to a built assembly. Run `dotnet build -c Release` first.

## Common Patterns

### Assembly Reference Switching

Show NuGet reference to readers, use local path for evaluation:

```fsharp
(*** condition: prepare ***)
#r "../src/MyLib/bin/Release/net8.0/MyLib.dll"
(*** condition: fsx ***)
#r "nuget: MyLib"

(**
---
title: Getting Started
category: Tutorials
categoryindex: 2
index: 1
---
*)
```

Note: the `condition: prepare` / `condition: fsx` block can appear before or after frontmatter.

### Hidden Setup with Visible Output

```fsharp
(*** hide ***)
let data = createTestData 100

(**
## Processing Results
*)
let summary = analyze data
(*** include-value: summary ***)
```

### Showing Multiple Outputs from One Block

```fsharp
(*** define-output: step1 ***)
let result = pipeline input
printfn "Processed %d items" result.Count

(**
The pipeline produced:
*)
(*** include-output: step1 ***)

(**
With the final value:
*)
(*** include-it: step1 ***)
```

### Platform-Specific Code

```fsharp
(*** condition: html ***)
(**
> **Note:** This example requires a running PostgreSQL instance.
*)

(*** do-not-eval ***)
let conn = Sql.connect "Host=localhost;Database=test"
```

## Troubleshooting

### Directive has no effect
Directives must be on their own line with exact whitespace: `(*** name ***)`. Extra spaces inside the delimiters or trailing content on the same line will cause the directive to be treated as a regular comment.

### `include-it` shows nothing
The preceding block must be an expression, not a `let` binding or `do` statement. Change `let x = 5` to just `5` or use `include-value: x`.

### Output appears in wrong place
Output directives apply to the immediately preceding code block. If there's a doc comment between the code and the directive, the association breaks.

### `condition: prepare` code appears in output
Check for typos in the directive. The colon and space are required: `(*** condition: prepare ***)`.

### Script works in FSI but fails in fsdocs
FSharp.Formatting uses a clean FSI session. Interactive state from a manual FSI session (like `#load` history) is not available. Ensure all dependencies are declared in the script.

### Large output truncated
FSI has default formatting limits. For large collections, explicitly format a subset:

```fsharp
data |> List.truncate 5 |> List.iter (printfn "%A")
(*** include-output ***)
```

### Multiple output directives for one block
Each code block can only have one output directive. To show both `stdout` and the `it` value, use named outputs:

```fsharp
(*** define-output: demo ***)
printfn "Processing..."
42
```

Then include each separately:

```fsharp
(*** include-output: demo ***)
(*** include-it: demo ***)
```

### Directive order in the file
Directives are processed top-to-bottom. A `define` must appear before its corresponding `include`. Forward references to not-yet-defined snippets produce empty output.

## Best Practices Summary

- Place output directives immediately after the code they reference
- Use `hide` for setup boilerplate, not for error-handling code readers should see
- Use `condition: prepare` for local paths, `condition: fsx` for NuGet references shown to readers
- Test with `--eval` regularly — scripts can silently break when APIs change
- Use named outputs (`define-output` / `include-output`) for complex scenarios requiring multiple output captures from one block
- Prefer `include-it` for expressions that return a value, `include-output` for code that prints
- Keep scripts focused on one topic — create separate files for separate concepts
