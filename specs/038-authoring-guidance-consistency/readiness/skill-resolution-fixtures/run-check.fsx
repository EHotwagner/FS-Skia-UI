// US1 (FR-001/002/003, SC-007) failing-first fixture harness.
//
// Mirrors the resolution rules implemented authoritatively in
// `build.fsx` `validateSkillIdResolution` and applies them to three controlled
// fixture trees that each violate one rule. Each fixture MUST produce >=1
// finding (the failing-first state). Run: `dotnet fsi run-check.fsx`.

open System
open System.IO
open System.Text.RegularExpressions

let advertisedRegex =
    Regex(@"->\s*((?:fs-skia|speckit)-[a-z0-9]+(?:-[a-z0-9]+)*)", RegexOptions.IgnoreCase)

let readName (file: string) =
    File.ReadAllLines file
    |> Array.tryPick (fun line ->
        let t = line.Trim()
        if t.StartsWith "name:" then Some(t.Substring(5).Trim().Trim('"').Trim('\'')) else None)

let skillFiles (root: string) (registry: string) =
    let regRoot = Path.Combine(root, registry.Replace("/", string Path.DirectorySeparatorChar))
    if Directory.Exists regRoot then
        Directory.GetDirectories regRoot
        |> Array.choose (fun d ->
            let f = Path.Combine(d, "SKILL.md")
            if File.Exists f then Some(Path.GetFileName d, readName f, f) else None)
        |> Array.toList
    else []

// Each fixture's check returns findings.
let checkResolution root =
    let agents = skillFiles root ".agents/skills"
    let claude = skillFiles root ".claude/skills"
    let declared =
        (agents @ claude) |> List.choose (fun (_, n, _) -> n) |> Set.ofList
    // advertised ids from speckit-tasks
    let advertised =
        [ for reg in [ ".agents/skills"; ".claude/skills" ] do
            let f = Path.Combine(root, reg.Replace("/", string Path.DirectorySeparatorChar), "speckit-tasks", "SKILL.md")
            if File.Exists f then
                let lines = File.ReadAllLines f
                for i in 0 .. lines.Length - 1 do
                    for m in advertisedRegex.Matches lines.[i] -> m.Groups.[1].Value, sprintf "%s/speckit-tasks/SKILL.md:%d" reg (i + 1) ]
    [ // dir == name
      for (dir, name, _) in agents @ claude do
        match name with
        | Some n when n <> dir -> yield sprintf "directory `%s` disagrees with declared name `%s`" dir n
        | _ -> ()
      // peer drift
      for (dir, an, _) in agents do
        match claude |> List.tryFind (fun (d, _, _) -> d = dir) with
        | Some(_, cn, _) when cn <> an -> yield sprintf "peer skill `%s` declares different name (%A vs %A)" dir an cn
        | _ -> ()
      // advertised resolution
      for (id, loc) in advertised do
        if not (Set.contains id declared) then
          yield sprintf "%s: advertised id `%s` does not resolve" loc id ]

let fixtures =
    [ "fixture-dangling", "dangling advertised id"
      "fixture-dirname-mismatch", "directory / declared-name disagreement"
      "fixture-peer-drift", ".agents/.claude peer drift" ]

let mutable allFailed = true
for (dir, desc) in fixtures do
    let findings = checkResolution (Path.Combine(__SOURCE_DIRECTORY__, dir))
    printfn "## %s — %s" dir desc
    if List.isEmpty findings then
        printfn "   UNEXPECTED PASS (fixture should fail)"
        allFailed <- false
    else
        findings |> List.iter (printfn "   FAIL: %s")
    printfn ""

if allFailed then
    printfn "RESULT: all three fixtures FAILED as required (failing-first satisfied)."
    exit 0
else
    eprintfn "RESULT: a fixture unexpectedly passed."
    exit 1
