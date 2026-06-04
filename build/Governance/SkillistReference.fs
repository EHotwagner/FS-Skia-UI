module FS.Skia.UI.Build.SkillistReference

open System.Text
open FS.Skia.UI.Build.Evidence

let referenceDocPath = "template/base/docs/skillist-reference.md"

let render (registry: SkillRegistry) (ownsVocabulary: (string * string) list) : string =
    let sb = StringBuilder()
    let line (s: string) = sb.Append(s).Append('\n') |> ignore

    line "# Skillist reference"
    line ""
    line "<!-- GENERATED from the live SkillRegistry + Audit.ownsVocabulary (feature 062, FR-006)."
    line "     The valid `skillist` ids are the SKILL.md `name:` values (NOT the directory name),"
    line "     resolved here so authors never grep '^name:' each file. Do not edit by hand;"
    line "     regenerate with ./fake.sh build -t RefreshSurfaceBaselines. Currency-checked by"
    line "     TargetMetadataDrift. -->"
    line ""
    line "A task's declared `skillist` ids (and the `[skillist: …]` mirror in `tasks.md`) are the"
    line "`name:` value from the owning `SKILL.md`, not the directory name. This page lists the valid"
    line "ids resolved from the live registry and the closed `owns:`→implied-skill table."
    line ""

    line "## Valid `skillist` ids"
    line ""
    line "| skillist id (`name:`) | resolved SKILL.md path |"
    line "|---|---|"
    for (id, paths) in registry.Skills |> Map.toList |> List.sortBy fst do
        let resolved = paths |> List.sort |> String.concat ", "
        line (sprintf "| `%s` | %s |" id resolved)
    line ""

    // Directory-name vs name: — resolve the distinction so an author who only knows
    // the folder name finds the accepted declared id.
    let aliases = registry.DirectoryAliases |> Map.toList |> List.sortBy fst
    line "## Directory-name → accepted `skillist` id"
    line ""
    if List.isEmpty aliases then
        line "_(none — every skill's directory name equals its `name:` id)_"
    else
        line "| directory-like name | accepted id (`name:`) | SKILL.md |"
        line "|---|---|---|"
        for (dirName, (acceptedId, path)) in aliases do
            line (sprintf "| `%s` | `%s` | %s |" dirName acceptedId path)
    line ""

    line "## Closed `owns:` vocabulary → implied skill"
    line ""
    line "An `owns:` value (in `tasks.deps.yml`) requires its implied skill in the task's `skillist`."
    line "The vocabulary is a closed set; an unknown value is a directive error."
    line ""
    line "| `owns:` value | implied skill |"
    line "|---|---|"
    for (ownsValue, impliedSkill) in ownsVocabulary do
        line (sprintf "| `%s` | `%s` |" ownsValue impliedSkill)
    line ""

    sb.ToString()
