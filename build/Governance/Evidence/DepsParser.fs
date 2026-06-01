namespace FS.Skia.UI.Build.Evidence

open System.IO
open System.Text.RegularExpressions
open YamlDotNet.RepresentationModel

type DepsEntry =
    { Deps: string list option
      Skillist: string list option
      LegacyBareList: bool }

type DepsModel =
    { Order: string list
      Map: Map<string, DepsEntry>
      Errors: string list }

module DepsParser =

    let private idRe = Regex(@"^T\d{3,4}$", RegexOptions.Compiled)

    let private scalarValue (n: YamlNode) : string option =
        match n with
        | :? YamlScalarNode as s -> Option.ofObj s.Value
        | _ -> None

    let private seqItems (n: YamlNode) : string list option =
        match n with
        | :? YamlSequenceNode as s ->
            s.Children
            |> Seq.choose scalarValue
            |> Seq.map (fun v -> v.Trim())
            |> Seq.filter (fun v -> v <> "")
            |> List.ofSeq
            |> Some
        | _ -> None

    let parse (depsYml: string) : DepsModel =
        let errors = ResizeArray<string>()
        let order = ResizeArray<string>()
        let map = System.Collections.Generic.Dictionary<string, DepsEntry>()

        let stream = YamlStream()
        let mutable loaded = true
        try
            use reader = new StringReader(depsYml)
            stream.Load reader
        with ex ->
            loaded <- false
            errors.Add(sprintf "tasks.deps.yml: unparseable YAML: %s" ex.Message)

        if loaded then
            if stream.Documents.Count = 0 then
                errors.Add "tasks.deps.yml: empty document"
            else
                match stream.Documents.[0].RootNode with
                | :? YamlMappingNode as root ->
                    let tasksNode =
                        root.Children
                        |> Seq.tryPick (fun kv ->
                            match scalarValue kv.Key with
                            | Some "tasks" -> Some kv.Value
                            | _ -> None)
                    match tasksNode with
                    | Some (:? YamlMappingNode as tasksMap) ->
                        for kv in tasksMap.Children do
                            match scalarValue kv.Key with
                            | Some key when idRe.IsMatch key ->
                                if map.ContainsKey key then
                                    errors.Add(sprintf "tasks.deps.yml: duplicate key %s" key)
                                else
                                    let entry =
                                        match kv.Value with
                                        | :? YamlSequenceNode ->
                                            // legacy bare-list form: value IS the deps list
                                            { Deps = seqItems kv.Value
                                              Skillist = None
                                              LegacyBareList = true }
                                        | :? YamlMappingNode as objNode ->
                                            let field name =
                                                objNode.Children
                                                |> Seq.tryPick (fun f ->
                                                    match scalarValue f.Key with
                                                    | Some n when n = name -> Some f.Value
                                                    | _ -> None)
                                            let depsField =
                                                match field "deps" with
                                                | Some v -> seqItems v
                                                | None -> None
                                            let skillField =
                                                match field "skillist" with
                                                | Some v -> seqItems v
                                                | None -> None
                                            { Deps = depsField
                                              Skillist = skillField
                                              LegacyBareList = false }
                                        | _ ->
                                            // empty / scalar value -> awaiting fields, both None
                                            { Deps = None; Skillist = None; LegacyBareList = false }
                                    map.[key] <- entry
                                    order.Add key
                            | _ -> ()
                    | _ -> errors.Add "tasks.deps.yml: missing or malformed 'tasks' mapping"
                | _ -> errors.Add "tasks.deps.yml: root is not a mapping"

        { Order = List.ofSeq order
          Map = map |> Seq.map (fun kv -> kv.Key, kv.Value) |> Map.ofSeq
          Errors = List.ofSeq errors }
