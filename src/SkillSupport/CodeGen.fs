namespace FS.Skia.UI.SkillSupport

open System.Text

module CodeGen =

    let mermaidGraph (nodes: string list) (edges: (string * string) list) : string =
        let sb = StringBuilder()
        sb.Append("graph TD\n") |> ignore
        for n in nodes do
            sb.Append("    ").Append(n).Append("\n") |> ignore
        for (a, b) in edges do
            sb.Append("    ").Append(a).Append(" --> ").Append(b).Append("\n") |> ignore
        sb.ToString()

    let markdownTable (headers: string list) (rows: string list list) : string =
        let sb = StringBuilder()
        let renderRow (cells: string list) =
            sb.Append("| ").Append(String.concat " | " cells).Append(" |\n") |> ignore
        renderRow headers
        renderRow (headers |> List.map (fun _ -> "---"))
        for r in rows do
            renderRow r
        sb.ToString()

    let asciiTree (roots: string list) (children: string -> string list) : string =
        let sb = StringBuilder()
        let rec renderChildren (prefix: string) (nodes: string list) =
            let count = List.length nodes
            nodes
            |> List.iteri (fun i node ->
                let isLast = (i = count - 1)
                let connector = if isLast then "└─ " else "├─ "
                sb.Append(prefix).Append(connector).Append(node).Append("\n") |> ignore
                let childPrefix = prefix + (if isLast then "   " else "│  ")
                renderChildren childPrefix (children node))
        for root in roots do
            sb.Append(root).Append("\n") |> ignore
            renderChildren "" (children root)
        sb.ToString()
