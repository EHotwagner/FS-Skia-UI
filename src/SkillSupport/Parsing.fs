namespace FS.Skia.UI.SkillSupport

open System.Text.Json
open System.Text.Json.Serialization
open System.Text.RegularExpressions
open YamlDotNet.Serialization

module Parsing =

    let private jsonOptions =
        let o = JsonSerializerOptions()
        o.Converters.Add(JsonFSharpConverter())
        o

    let readYaml<'T> (text: string) : Result<'T, string> =
        try
            let deserializer = DeserializerBuilder().Build()
            let value = deserializer.Deserialize<'T>(text)
            if obj.ReferenceEquals(value, null) then
                Error "YAML deserialized to null"
            else
                Ok value
        with ex ->
            Error ex.Message

    let readJson<'T> (text: string) : Result<'T, string> =
        try
            match JsonSerializer.Deserialize(text, typeof<'T>, jsonOptions) with
            | null -> Error "JSON deserialized to null"
            | value -> Ok(value :?> 'T)
        with ex ->
            Error ex.Message

    let matchLines (pattern: string) (lines: string seq) : (int * Match) seq =
        let rx = Regex(pattern)
        lines
        |> Seq.indexed
        |> Seq.choose (fun (i, line) ->
            let m = rx.Match(line)
            if m.Success then Some(i, m) else None)
