module FS.Skia.UI.Build.SkillistView

open System.Text.RegularExpressions

type SkillistCurrencyItem =
    { TaskId: string
      Canonical: string list
      DerivedNow: string list option
      ExpectedAnnotation: string
      IsStale: bool }

let private regenCommand = "./fake.sh build -t RefreshSurfaceBaselines"

// Anchored on the SAME token grammar TaskParser parses (`[skillist: []]` empty,
// `[skillist: a, b]` otherwise) so the splice never disturbs other line content.
let private skillistRe =
    Regex(@"\[skillist:\s*(?:\[\]|[^\]]*)\]", RegexOptions.Compiled)

let renderAnnotation (canonical: string list) =
    match canonical with
    | [] -> "[skillist: []]"
    | _ -> "[skillist: " + (String.concat ", " canonical) + "]"

let spliceAnnotation (canonical: string list) (taskLine: string) =
    if not (skillistRe.IsMatch taskLine) then
        failwithf
            "SkillistView.spliceAnnotation: task line carries no [skillist: …] annotation token (omitted metadata is invalid): %s"
            taskLine

    // Replace only the first occurrence; a literal replacement avoids $-token expansion.
    skillistRe.Replace(taskLine, (fun _ -> renderAnnotation canonical), 1)

let currency
    (canonicalByTask: (string * string list) list)
    (derivedByTask: (string * (string list option)) list)
    : SkillistCurrencyItem list =
    let derivedMap = derivedByTask |> Map.ofList

    canonicalByTask
    |> List.map (fun (taskId, canonical) ->
        let derivedNow =
            match derivedMap.TryFind taskId with
            | Some mirror -> mirror
            | None -> None

        let isStale =
            match derivedNow with
            | Some mirror -> mirror <> canonical
            | None -> true // absent annotation is invalid -> stale (reported, not inserted)

        { TaskId = taskId
          Canonical = canonical
          DerivedNow = derivedNow
          ExpectedAnnotation = renderAnnotation canonical
          IsStale = isStale })

let staleDiagnostic (taskId: string) (canonical: string list) =
    sprintf
        "%s: the tasks.md [skillist: …] view is stale relative to its canonical tasks.deps.yml source; regenerate via %s (expected %s)"
        taskId
        regenCommand
        (renderAnnotation canonical)

let currencyDrift (items: SkillistCurrencyItem list) : string option =
    let stale = items |> List.filter (fun i -> i.IsStale)

    if List.isEmpty stale then
        None
    else
        let lines = stale |> List.map (fun i -> staleDiagnostic i.TaskId i.Canonical)
        Some(System.String.Join(System.Environment.NewLine, lines))
