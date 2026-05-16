module Product.Tests

open Expecto
open Product.Program

[<Tests>]
let tests =
    testList "product" [
        test "generated product test suite is wired" {
            Expect.equal 1 1 "product tests run"
        }

        test "product-owned controls example is wired" {
            let view = controlsExampleView { Name = "Product"; CanSave = true }
            Expect.equal (FS.Skia.UI.Controls.Control.count view) 4 "product example owns a representative controls view"
        }
    ]
