// One-shot helper (feature 071, T010): capture the per-fact parity fixtures
// (`Catalog.fs.<id>.txt` + `catalog.yml.<id>.txt`) from the REAL CatalogGen renderer
// output — golden bytes, not hand-typed literals. The `066` fixture-iteration test reads
// these by `fact.Id` and compares `renderFSharpRow`/`renderYamlRow` to the fixture after
// `.TrimEnd('\n')`. Each fixture is the render output plus a single trailing newline,
// matching the existing six fixtures' format.
#r "../build/Governance/bin/Debug/net10.0/FS.Skia.UI.Build.dll"
open System.IO
open FS.Skia.UI.Build

let root = __SOURCE_DIRECTORY__ |> Path.GetDirectoryName
let dir = Path.Combine(root, "specs", "066-typed-catalog-generation", "readiness", "parity-fixtures")
Directory.CreateDirectory dir |> ignore

let mutable n = 0
for fact in CatalogGen.catalogFacts do
    File.WriteAllText(Path.Combine(dir, sprintf "Catalog.fs.%s.txt" fact.Id), CatalogGen.renderFSharpRow fact + "\n")
    File.WriteAllText(Path.Combine(dir, sprintf "catalog.yml.%s.txt" fact.Id), CatalogGen.renderYamlRow fact + "\n")
    n <- n + 1

printfn "captured %d fixture pairs (%d files) into %s" n (n * 2) dir
