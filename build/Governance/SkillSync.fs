module FS.Skia.UI.Build.SkillSync

open System
open System.IO
open System.Security.Cryptography
open System.Text

type SkillPairResult =
    { Slug: string
      ClaudePath: string
      AgentsPath: string
      ClaudeHash: string option
      AgentsHash: string option }

let expectedSlugs =
    [ "fsharp-parsing"
      "fsharp-graph-algorithms"
      "fsharp-code-generation"
      "fsharp-io-globbing"
      "fsharp-shell-process"
      "fsharp-build-orchestration" ]

let claudeRelPath (slug: string) = $".claude/skills/{slug}/SKILL.md"
let agentsRelPath (slug: string) = $".agents/skills/{slug}/SKILL.md"

let sha256Hex (bytes: byte[]) =
    use sha = SHA256.Create()
    let digest = sha.ComputeHash bytes
    let sb = StringBuilder(digest.Length * 2)

    for b in digest do
        sb.Append(b.ToString("x2")) |> ignore

    sb.ToString()

let inSync (result: SkillPairResult) =
    match result.ClaudeHash, result.AgentsHash with
    | Some c, Some a -> c = a
    | _ -> false

let drifted (results: SkillPairResult list) =
    results |> List.filter (inSync >> not)

let private hashFile (fullPath: string) =
    if File.Exists fullPath then
        Some(sha256Hex (File.ReadAllBytes fullPath))
    else
        None

let checkPair (root: string) (slug: string) =
    let claudeRel = claudeRelPath slug
    let agentsRel = agentsRelPath slug
    let toFull (rel: string) = Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar))

    { Slug = slug
      ClaudePath = claudeRel
      AgentsPath = agentsRel
      ClaudeHash = hashFile (toFull claudeRel)
      AgentsHash = hashFile (toFull agentsRel) }

let checkAll (root: string) =
    expectedSlugs |> List.map (checkPair root)

let private digestText =
    function
    | Some h -> h
    | None -> "<missing>"

let renderReport (results: SkillPairResult list) =
    let sb = StringBuilder()
    sb.AppendLine "# SkillSyncCheck" |> ignore
    sb.AppendLine "" |> ignore
    let bad = drifted results

    if List.isEmpty bad then
        sb.AppendLine $"PASS: all {List.length results} capability-skill pairs are byte-identical across `.claude/skills/**` and `.agents/skills/**`." |> ignore
        sb.AppendLine "" |> ignore
        sb.AppendLine "| Skill | SHA-256 (shared) |" |> ignore
        sb.AppendLine "|-------|------------------|" |> ignore

        for r in results do
            sb.AppendLine $"| `{r.Slug}` | `{digestText r.ClaudeHash}` |" |> ignore
    else
        sb.AppendLine $"FAIL: {List.length bad} capability-skill pair(s) drifted between the two trees." |> ignore
        sb.AppendLine "" |> ignore
        sb.AppendLine "| Skill | .claude SHA-256 | .agents SHA-256 |" |> ignore
        sb.AppendLine "|-------|-----------------|-----------------|" |> ignore

        for r in bad do
            sb.AppendLine $"| `{r.Slug}` | `{digestText r.ClaudeHash}` | `{digestText r.AgentsHash}` |" |> ignore

    sb.ToString()

let renderFailureMessage (results: SkillPairResult list) =
    let bad = drifted results

    if List.isEmpty bad then
        ""
    else
        let lines =
            bad
            |> List.map (fun r ->
                $"SkillSyncCheck drift: {r.Slug} — {r.ClaudePath}={digestText r.ClaudeHash} {r.AgentsPath}={digestText r.AgentsHash}")

        String.Join(Environment.NewLine, "SkillSyncCheck FAILED — byte-identity drift between the two skill trees:" :: lines)
