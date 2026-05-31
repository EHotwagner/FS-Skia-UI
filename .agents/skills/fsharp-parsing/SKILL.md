---
name: fsharp-parsing
description: Parse governance inputs (YAML, tasks.md line grammar, audit-status regions, JSON) in compiled F#.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-parsing

Cookbook for porting the Bash/Python parsers into typed, compiled F# in the governance library.
Owns **C1** (YAML), **C2** (`tasks.md` line grammar), **C3** (`audit-status` regions), **C4** (JSON
read), **C5** (JSON write), **C16** (regex over a diff), and **C21** (general regex). It replaces the
two hand-rolled YAML parsers (Python + `build.fsx`) and the regex scanners. Verdicts come from the
capability report (`metadata.source`) §3 and are not re-opened here.

## When to use

- Reading `tasks.deps.yml`, `capabilities.yml`, `validation.contract.yml`, `audit-patterns.yml`.
- Parsing `tasks.md` task lines, annotations (`[P]`/`[US\d+]`/`[T[12]]`/`[SEH]`/`[skillist: …]`),
  phase/checkpoint headers, and Synthetic-Evidence Inventory tables.
- Scanning ` ```audit-status ` fenced regions (key=value, first-region-wins, dup-key = hard error).
- Reading `.specify/feature.json`; emitting `task-graph.json` (schema 1.0).
- Applying `audit-patterns.yml` regexes to a unified diff (C16) and the misc package/version/fence
  regexes (C21).

## Library verdicts

- **YAML (C1) → YamlDotNet 17.1.0** (already pinned). Deserialize into immutable F# records/DUs behind
  the API; retire both bespoke parsers. **Legivel rejected** — YamlDotNet is present and the inputs
  don't need YAML-1.2 conformance.
- **JSON read/write (C4, C5) → System.Text.Json + FSharp.SystemTextJson 1.4.36** for DU/record
  round-trip. **Thoth.Json / Newtonsoft rejected** — STJ is in the BCL and FSharp.SystemTextJson adds
  the F# union/record support.
- **Line/region/diff grammar (C2, C3, C16) → regex port FIRST, then XParsec 1.0.0.** A faithful
  `System.Text.RegularExpressions` port clears the byte-parity gate against the Stage-0 golden
  fixtures; migrate the grammar to **XParsec** (pure F#, MIT, Fable-capable) once parity is signed
  off. **Full Markdown AST (FSharp.Formatting/Markdig) rejected** — the inputs are a constrained line
  grammar, not arbitrary Markdown.
- **General regex (C21) → `System.Text.RegularExpressions`** (BCL). No library.

## Exact grammars to reproduce (parity-critical)

Reproduce these exactly; gate on the Stage-0 golden fixtures (Invariant 6) **before** deleting any
Python/Bash.

Task line (`compute-task-graph.py`):
`^\s*-\s*\[(?<box>[ X\-FS*])\]\s+(?<id>T\d{3,4})\b(?<rest>.*)$`
Boxes: `[ ]` pending · `[X]` done · `[S]` synthetic · `[F]` failed · `[-]` skipped · `[*]` computed-only.
Annotations (order varies): `[P]` parallel · `[US\d+]` user story · `[T[12]]` tier · `[SEH]` +
`synthetic-error-handling-approved` label · `[skillist: [...]]` (empty brackets = no skills).

`tasks.deps.yml` accepts **two shapes** — object `{deps, skillist}` and legacy bare-list. Accept
both; fixture-test both before deleting the Python.

`audit-status` region semantics (`audit-status-scan.py`): first region wins · detect unclosed ·
`#` comments and blank lines ignored · `key=value` · **duplicate key = hard error** (never
last-wins) · key normalize `.lower().Trim()`.

## API walkthrough + runnable examples

### C1 — YAML into a typed model (YamlDotNet)

`DeserializerBuilder().Build()` gives an `IDeserializer`. Deserialize to the loose node model
(`Dictionary<string,obj>`; nested mappings are `Dictionary<obj,obj>`, sequences are `List<obj>`),
then project into immutable F# values so the rest of the library never sees a mutable bag. This is
where you absorb the two `tasks.deps.yml` shapes.

```fsharp
open System.Collections.Generic
open YamlDotNet.Serialization

let deserializer = DeserializerBuilder().Build()

/// tasks.deps.yml tolerates TWO shapes the typed reader must accept:
///   object  ->  T001: { deps: [T000], skillist: [speckit-evidence-graph] }
///   legacy  ->  T001: [T000]            (the value IS the deps list)
let normaliseEntry (value: obj) : string list * string list =
    let toStrings (xs: List<obj>) = [ for x in xs -> string x ]

    match value with
    | :? Dictionary<obj, obj> as object ->
        let listAt (key: string) =
            match object.TryGetValue(box key) with
            | true, (:? List<obj> as xs) -> toStrings xs
            | _ -> []

        listAt "deps", listAt "skillist"
    | :? List<obj> as bare -> toStrings bare, []
    | _ -> [], []

let parseDepsRoot (yaml: string) =
    let root = deserializer.Deserialize<Dictionary<string, obj>>(yaml)

    match root.TryGetValue "tasks" with
    | true, (:? Dictionary<obj, obj> as tasks) ->
        [ for kv in tasks -> string kv.Key, normaliseEntry kv.Value ]
    | _ -> []
```

### C2 — task-line grammar (regex port first)

Port the Python regex verbatim into a compiled `Regex` with named groups, then project the match
into a typed row. This is the parity-first path.

```fsharp
open System.Text.RegularExpressions

type TaskBox =
    | Pending
    | Done
    | Synthetic
    | Failed
    | Skipped
    | Computed

let taskLine =
    Regex(@"^\s*-\s*\[(?<box>[ X\-FS*])\]\s+(?<id>T\d{3,4})\b(?<rest>.*)$", RegexOptions.Compiled)

let boxOf (token: string) =
    match token with
    | "X" -> Done
    | "S" -> Synthetic
    | "F" -> Failed
    | "-" -> Skipped
    | "*" -> Computed
    | _ -> Pending

let parseTaskLine (line: string) =
    let m = taskLine.Match line

    if m.Success then
        Some(m.Groups.["id"].Value, boxOf (m.Groups.["box"].Value), m.Groups.["rest"].Value.Trim())
    else
        None
```

### C2 — annotations and the skillist mirror

Annotations appear in any order; pull each out independently. `[skillist: []]` means no skills.

```fsharp
open System.Text.RegularExpressions

let private skillistRx = Regex(@"\[skillist:\s*\[(?<items>[^\]]*)\]\]", RegexOptions.Compiled)
let private userStoryRx = Regex(@"\[US(?<n>\d+)\]", RegexOptions.Compiled)

let skillistOf (rest: string) =
    let m = skillistRx.Match rest

    if not m.Success then
        []
    else
        m.Groups.["items"].Value.Split(',')
        |> Array.map (fun s -> s.Trim())
        |> Array.filter (fun s -> s <> "")
        |> Array.toList

let userStoryOf (rest: string) =
    let m = userStoryRx.Match rest
    if m.Success then Some(int m.Groups.["n"].Value) else None
```

### C3 — `audit-status` fenced region (hand parser, dup-key = error)

First region wins; `#` comments and blanks ignored; `key=value`; a **duplicate key is a hard error**
(never last-wins); keys normalise to lower+trim.

```fsharp
open System

type AuditRegion =
    | Parsed of Map<string, string>
    | DuplicateKey of string
    | Unclosed

let parseAuditStatus (text: string) : AuditRegion option =
    let lines = text.Replace("\r\n", "\n").Split('\n')
    let openIdx = lines |> Array.tryFindIndex (fun l -> l.Trim() = "```audit-status")

    match openIdx with
    | None -> None
    | Some start ->
        let rest = lines.[start + 1 ..]
        let closeRel = rest |> Array.tryFindIndex (fun l -> l.Trim() = "```")

        match closeRel with
        | None -> Some Unclosed
        | Some stop ->
            let mutable acc = Map.empty
            let mutable result = None

            for raw in rest.[.. stop - 1] do
                let line = raw.Trim()

                if line <> "" && not (line.StartsWith "#") && result.IsNone then
                    let eq = line.IndexOf '='

                    if eq > 0 then
                        let key = line.Substring(0, eq).Trim().ToLowerInvariant()
                        let value = line.Substring(eq + 1).Trim()

                        if Map.containsKey key acc then
                            result <- Some(DuplicateKey key)
                        else
                            acc <- Map.add key value acc

            Some(defaultArg result (Parsed acc))
```

### C4 / C5 — JSON read & write (FSharp.SystemTextJson)

`JsonFSharpOptions.Default().ToJsonSerializerOptions()` produces options that round-trip F# records
and unions. Reuse one options value; it is the difference between STJ choking on an F# record and
handling it cleanly.

```fsharp
open System.Text.Json
open System.Text.Json.Serialization

let jsonOptions = JsonFSharpOptions.Default().ToJsonSerializerOptions()

type FeatureFile = { feature_directory: string }

/// C4 — read .specify/feature.json into a typed record.
let readFeature (json: string) : FeatureFile =
    JsonSerializer.Deserialize<FeatureFile>(json, jsonOptions)

/// C5 — emit a typed record back as canonical JSON.
let writeFeature (feature: FeatureFile) : string =
    JsonSerializer.Serialize(feature, jsonOptions)
```

For hand-built artifacts where you want byte-stable layout (e.g. `task-graph.json` schema 1.0),
`Utf8JsonWriter` over a `MemoryStream` gives explicit control of ordering and indentation:

```fsharp
open System.IO
open System.Text
open System.Text.Json

let renderTaskGraphJson (tasks: (string * string) list) : string =
    use stream = new MemoryStream()
    use writer = new Utf8JsonWriter(stream, JsonWriterOptions(Indented = true))
    writer.WriteStartObject()
    writer.WriteString("schema_version", "1.0")
    writer.WriteStartArray("tasks")

    for id, effective in tasks do
        writer.WriteStartObject()
        writer.WriteString("id", id)
        writer.WriteString("effective_status", effective)
        writer.WriteEndObject()

    writer.WriteEndArray()
    writer.WriteEndObject()
    writer.Flush()
    Encoding.UTF8.GetString(stream.ToArray())
```

### C16 / C21 — regex over a diff and general rewrites

C16 applies the `audit-patterns.yml` regexes to added/removed diff lines; C21 covers the misc
package-version rewrite and fence detection. Both are plain BCL regex.

```fsharp
open System.Text.RegularExpressions

/// C16 — scan only the added lines of a unified diff for a forbidden pattern.
let addedLineHits (pattern: Regex) (diff: string) =
    diff.Replace("\r\n", "\n").Split('\n')
    |> Array.filter (fun l -> l.StartsWith "+" && not (l.StartsWith "+++"))
    |> Array.filter (fun l -> pattern.IsMatch l)
    |> Array.toList

/// C21 — rewrite a central package version pin.
let bumpPackageVersion (packageId: string) (newVersion: string) (props: string) =
    let rx = Regex($"""(Include="{Regex.Escape packageId}"\s+Version=")[^"]*(")""")
    rx.Replace(props, (fun m -> m.Groups.[1].Value + newVersion + m.Groups.[2].Value))
```

### C2 / C3 — the XParsec migration target (post-parity)

Once the regex port has cleared the golden gate, the grammar's long-term home is **XParsec**:
combinators over a `Reader`, with `Ok`/`Error` results. The parsed value comes straight out of the
`Ok` case.

```fsharp
open XParsec
open XParsec.Parsers
open XParsec.CharParsers

/// A task id like `T024` as an XParsec combinator: 'T' then 3-4 digits.
let taskId =
    parser {
        let! _ = pchar 'T'
        let! digits = many1Chars (satisfy System.Char.IsDigit)
        return "T" + digits
    }

let parseTaskId (input: string) =
    let reader = Reader.ofString input ()

    match taskId reader with
    | Ok success -> Some success
    | Error _ -> None
```

## Cautions

- **Two `tasks.deps.yml` shapes.** Object `{deps, skillist}` and legacy bare-list — accept both and
  fixture-test both before deleting the Python. This is one of the two most likely silent
  divergences in the whole port (the other is .NET-glob vs `fnmatch`, see [[fsharp-io-globbing]]).
- **Parity over elegance.** Match the Python's exact tie-breaks/ordering; golden-gate (Invariant 6)
  before deleting any script.
- **`audit-status` duplicate key is a hard error**, never last-wins — keep it.
- **Determinism.** Parsers must be pure over their input — no env/clock reads at parse time.
- **Build-tooling scope only.** These parsers live under `build/Governance`; nothing here ships in a
  generated product, and no `FSharp.Compiler.*` is used.

## Consuming stages

Stage 3.3 (YAML→typed model migration), Stage 4 (Python parser port: task-graph + audit-status),
Stage 5 (compiled build front-end reading `feature.json` / emitting `task-graph.json`). See the plan
referenced from `metadata.source`.

## Sources / links

- YamlDotNet: <https://github.com/aaubry/YamlDotNet/wiki>
- System.Text.Json: <https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/>
- FSharp.SystemTextJson: <https://github.com/Tarmil/FSharp.SystemTextJson>
- XParsec: <https://github.com/roboz0r/XParsec>
- .NET regex: <https://learn.microsoft.com/dotnet/standard/base-types/regular-expressions>
- Capability report: `docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md` §3.

## Related

[[fsharp-graph-algorithms]] (consumes the parsed model), [[fsharp-code-generation]] (emits
JSON/Markdown), [[fsharp-io-globbing]] (discovers the files to parse), [[fsharp-shell-process]]
(produces the diff scanned in C16).
