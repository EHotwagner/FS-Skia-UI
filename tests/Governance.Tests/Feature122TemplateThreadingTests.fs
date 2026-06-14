module Feature122TemplateThreadingTests

// Feature 122 (US2, FR-005) — the generated CONTROLS-profile `Program.fs` must thread a parsed window
// flag (e.g. --window-startup normal) into the ACTUAL live launch via
// `ControlsElmish.runInteractiveAppWithWindowBehavior`, not only into the options report
// (`manualWindowOptionResults`). This asserts the production template source the consumer receives.

open Expecto
open GovernanceTestSupport

[<Tests>]
let tests =
    testList
        "Feature 122 template window-behavior threading (US2, FR-005)"
        [ test "generated Program.fs threads the parsed window behavior into the live controls launch (SC-003)" {
              let program = read "template/base/src/Product/Program.fs"

              // The flag-gated live launch routes through the window-behavior overload...
              expectContains
                  program
                  "ControlsElmish.runInteractiveAppWithWindowBehavior viewerOptions windowBehaviorRequest interactiveHost"
                  "the app profile threads windowBehaviorRequest into the live launch"

              // ...gated by the same windowFlagSupplied predicate the game profile uses, so the no-flag
              // default windowed-fullscreen path is preserved (byte-identical).
              expectContains
                  program
                  "if Product.WindowOptions.windowFlagSupplied args then"
                  "the threading is flag-gated; no flag keeps the default path"

              expectContains
                  program
                  "ControlsElmish.runInteractiveApp viewerOptions interactiveHost"
                  "the no-flag default still calls plain runInteractiveApp"
          } ]
