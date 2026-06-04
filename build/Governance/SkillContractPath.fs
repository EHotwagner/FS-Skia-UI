module FS.Skia.UI.Build.SkillContractPath

open System
open System.Text.RegularExpressions
open FS.Skia.UI.Build.ApiSurfaceGen

type SkillContractClaim =
    { SkillPath: string
      ClaimedPath: string
      ClaimsNoReflection: bool }

// Any `docs/api-surface/<segments>/<file>.fsi` token, however quoted/backticked. The
// path is allowed to be prefixed (e.g. `./docs/...`); we normalize to the bare
// `docs/api-surface/...` form so it compares to ApiSurfaceEntry.ProjectRelPath.
let private claimPattern = Regex(@"docs/api-surface/[A-Za-z0-9._/-]+\.fsi", RegexOptions.Compiled)

// A skill that promises local API discovery uses some variant of "no DLL reflection
// needed" / "without DLL reflection". Match loosely on the two anchor tokens so a
// reworded promise still counts.
let private noReflectionPattern =
    Regex(@"no\s+DLL\s+reflection|without\s+DLL\s+reflection|no\s+reflection\s+needed", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

let parseClaims (skillPath: string) (skillText: string) : SkillContractClaim list =
    let claimsNoReflection = noReflectionPattern.IsMatch skillText

    claimPattern.Matches skillText
    |> Seq.map (fun m -> m.Value.Replace('\\', '/'))
    |> Seq.distinct
    |> Seq.map (fun claimed ->
        { SkillPath = skillPath
          ClaimedPath = claimed
          ClaimsNoReflection = claimsNoReflection })
    |> Seq.toList

let check (entries: ApiSurfaceEntry list) (claims: SkillContractClaim list) : string list =
    let emitted = entries |> List.map (fun e -> e.ProjectRelPath) |> Set.ofList

    claims
    |> List.collect (fun claim ->
        if emitted.Contains claim.ClaimedPath then
            []
        elif claim.ClaimsNoReflection then
            [ sprintf
                  "MISS api-surface: %s claimed by %s is not emitted, yet the skill asserts \"no DLL reflection needed\""
                  claim.ClaimedPath
                  claim.SkillPath ]
        else
            [ sprintf "MISS api-surface: %s claimed by %s is not emitted" claim.ClaimedPath claim.SkillPath ])

let orphans (entries: ApiSurfaceEntry list) (claims: SkillContractClaim list) : string list =
    let claimed = claims |> List.map (fun c -> c.ClaimedPath) |> Set.ofList

    entries
    |> List.map (fun e -> e.ProjectRelPath)
    |> List.distinct
    |> List.filter (fun p -> not (claimed.Contains p))
    |> List.map (fun p -> sprintf "orphan api-surface: %s is emitted but no skill claims it" p)
