module Product.Tests

open Expecto

[<Tests>]
let tests =
    testList "product" [
        test "generated product test suite is wired" {
            Expect.equal 1 1 "product tests run"
        }
    ]
