module TestingCapabilityTests

open Expecto
open FS.Skia.UI.Testing

[<Tests>]
let tests =
    testList "Testing helper contract" [
        test "summaries include profile and packages" {
            let summary =
                GeneratedProductAssertions.summarize
                    { Profile = "app"
                      RequiredFiles = [ "src/Product/Product.fsproj" ]
                      ForbiddenPrefixes = [ "samples/" ]
                      PackageReferences = [ { PackageId = "FS.Skia.UI.Scene"; Required = true } ] }

            Expect.stringContains summary "app" "profile is included"
            Expect.stringContains summary "FS.Skia.UI.Scene" "package is included"
        }
    ]
