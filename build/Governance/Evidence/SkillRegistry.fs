namespace FS.Skia.UI.Build.Evidence

open System.IO
open System.Text.RegularExpressions

type SkillRegistry =
    { Skills: Map<string, string list>
      DirectoryAliases: Map<string, string * string>
      Warnings: string list }

module SkillRegistry =

    let private nn s : string = Option.defaultValue "" (Option.ofObj s)

    let private nameRe =
        Regex(@"^name:\s*['""]?([^'""\n]+)['""]?\s*$", RegexOptions.Compiled ||| RegexOptions.Multiline)

    /// Repo-relative path segments (forward-slash), or [||] if not under root.
    let private relParts (repoRoot: string) (file: string) : string[] =
        let root = Path.GetFullPath(repoRoot).TrimEnd('/', '\\')
        let full = Path.GetFullPath file
        if full.StartsWith(root) then
            full.Substring(root.Length).TrimStart('/', '\\').Replace('\\', '/').Split('/')
        else
            [||]

    let private relString (repoRoot: string) (file: string) : string =
        match relParts repoRoot file with
        | [||] -> file.Replace('\\', '/')
        | parts -> System.String.Join("/", parts)

    let private directoryAlias (repoRoot: string) (file: string) : string =
        let parts = relParts repoRoot file
        if parts.Length >= 4 && parts.[0] = ".agents" && parts.[1] = "skills" then parts.[2]
        elif parts.Length >= 4 && parts.[0] = "src" && parts.[2] = "skill" then parts.[1]
        elif parts.Length >= 5 && parts.[0] = "template" && parts.[1] = "fragments" && parts.[3] = "skill" then
            parts.[2]
        else
            nn (Path.GetFileName(nn (Path.GetDirectoryName file)))

    let build (repoRoot: string) : SkillRegistry =
        let combine parts = Path.Combine(Array.ofList (repoRoot :: parts))

        let roots =
            [ yield combine [ ".agents"; "skills" ]
              let srcParent = combine [ "src" ]
              if Directory.Exists srcParent then
                  for d in Directory.GetDirectories(srcParent) |> Array.sort do
                      let s = Path.Combine(d, "skill")
                      if Directory.Exists s then yield s
              let fragParent = combine [ "template"; "fragments" ]
              if Directory.Exists fragParent then
                  for d in Directory.GetDirectories(fragParent) |> Array.sort do
                      let s = Path.Combine(d, "skill")
                      if Directory.Exists s then yield s ]

        let skills = System.Collections.Generic.Dictionary<string, ResizeArray<string>>()
        let aliases = System.Collections.Generic.Dictionary<string, string * string>()
        let warnings = ResizeArray<string>()

        let skillFiles (root: string) : string list =
            if not (Directory.Exists root) then
                []
            elif nn (Path.GetFileName(root.TrimEnd('/', '\\'))) = "skills" then
                Directory.GetDirectories(root)
                |> Array.sort
                |> Array.choose (fun d ->
                    let f = Path.Combine(d, "SKILL.md")
                    if File.Exists f then Some f else None)
                |> List.ofArray
            else
                let f = Path.Combine(root, "SKILL.md")
                if File.Exists f then [ f ] else []

        for root in roots do
            for skillFile in skillFiles root do
                let mutable text = ""
                let mutable readable = true
                try
                    text <- File.ReadAllText skillFile
                with ex ->
                    readable <- false
                    warnings.Add(sprintf "%s: skill file is not readable: %s" (relString repoRoot skillFile) ex.Message)
                if readable then
                    let m = nameRe.Match text
                    let skillId =
                        if m.Success then m.Groups.[1].Value.Trim()
                        else nn (Path.GetFileName(nn (Path.GetDirectoryName skillFile)))
                    let rel = relString repoRoot skillFile
                    match skills.TryGetValue skillId with
                    | true, lst -> lst.Add rel
                    | _ ->
                        let lst = ResizeArray<string>()
                        lst.Add rel
                        skills.[skillId] <- lst
                    let alias = directoryAlias repoRoot skillFile
                    if alias <> skillId && not (aliases.ContainsKey alias) then
                        aliases.[alias] <- (skillId, rel)

        { Skills = skills |> Seq.map (fun kv -> kv.Key, List.ofSeq kv.Value) |> Map.ofSeq
          DirectoryAliases = aliases |> Seq.map (fun kv -> kv.Key, kv.Value) |> Map.ofSeq
          Warnings = List.ofSeq warnings }
