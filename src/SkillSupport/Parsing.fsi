// Parsing.fsi — fsharp-parsing family.
//
// Generic governance-input parsing utilities for the SkillSupport shipped library:
// typed YAML read (YamlDotNet), typed JSON read (System.Text.Json +
// FSharp.SystemTextJson), and a regex line-grammar helper. Consumers in
// build/Governance keep their exact tasks.md / tasks.deps.yml grammars and call
// these utilities. Visibility lives here (Principle II).
namespace FS.Skia.UI.SkillSupport

module Parsing =

    /// Typed YAML read (YamlDotNet). `Ok value` on a successful deserialization;
    /// `Error message` carries the deserializer's message on failure.
    val readYaml<'T> : text: string -> Result<'T, string>

    /// Typed JSON read (System.Text.Json + FSharp.SystemTextJson, so F# records and
    /// unions deserialize). `Ok value` on success; `Error message` on failure.
    val readJson<'T> : text: string -> Result<'T, string>

    /// Regex line-grammar helper: compile `pattern` once and apply it to each line,
    /// yielding the zero-based line index paired with the successful match.
    val matchLines:
        pattern: string ->
        lines: string seq ->
            (int * System.Text.RegularExpressions.Match) seq
