module GraphicsEnvironmentTests

// Feature 049 — deterministic escalated validation path.
// US3 (T005): pure unit + FsCheck property tests for BuildEnvironment.normalizeGraphicsEnv,
// proving the dual-display guard is a strict no-op off the failing host.
// US1 (T008): a real process-spawn contract test (added with the spawn-edge wiring).
// US2 (T011): the kill-on-timeout diagnostic-builder unit test (added with T012).

open System
open System.IO
open Expecto
open FsCheck
open BuildEnvironment

// Reuse the production key names (single source) to avoid drift.
let private waylandKey = waylandDisplayKey
let private gdkKey = gdkBackendKey
let private sdlKey = sdlVideoDriverKey

/// FsCheck's default string generator can emit null; drop null keys/values so the
/// generated Map<string,string> is a valid member of the production input domain.
let private cleanMap (pairs: (string * string) list) =
    pairs
    |> List.choose (fun (k, v) -> if isNull (box k) || isNull (box v) then None else Some(k, v))
    |> Map.ofList

[<Tests>]
let graphicsEnvironmentTests =
    testList "Graphics environment normalization" [
        test "DualDisplay removes WAYLAND_DISPLAY and forces the x11 backends" {
            let input = Map [ waylandKey, "wayland-0"; displayKey, ":1"; "PATH", "/usr/bin" ]
            let output = normalizeGraphicsEnv input
            Expect.isFalse (output.ContainsKey waylandKey) "WAYLAND_DISPLAY is removed"
            Expect.equal (Map.tryFind gdkKey output) (Some "x11") "GDK_BACKEND forced to x11"
            Expect.equal (Map.tryFind sdlKey output) (Some "x11") "SDL_VIDEODRIVER forced to x11"
            Expect.equal (Map.tryFind displayKey output) (Some ":1") "DISPLAY preserved"
            Expect.equal (Map.tryFind "PATH" output) (Some "/usr/bin") "unrelated keys preserved"
        }

        test "WaylandOnly is identity (real Wayland desktop is unaffected)" {
            let input = Map [ waylandKey, "wayland-0"; "PATH", "/usr/bin" ]
            Expect.equal (normalizeGraphicsEnv input) input "WaylandOnly unchanged"
        }

        test "X11Only is identity (already on the working path)" {
            let input = Map [ displayKey, ":1"; gdkKey, "wayland"; "PATH", "/usr/bin" ]
            Expect.equal (normalizeGraphicsEnv input) input "X11Only unchanged"
        }

        test "Neither is identity (non-Linux / headless-no-display)" {
            let input = Map [ "PATH", "/usr/bin" ]
            Expect.equal (normalizeGraphicsEnv input) input "Neither unchanged"
        }

        test "empty map is total and unchanged" {
            Expect.equal (normalizeGraphicsEnv Map.empty) Map.empty "empty map handled"
        }

        test "classifyDisplay distinguishes the four states; empty value counts as absent" {
            Expect.equal (classifyDisplay (Map [ waylandKey, "w"; displayKey, ":1" ])) DualDisplay "dual"
            Expect.equal (classifyDisplay (Map [ waylandKey, "w" ])) WaylandOnly "wayland-only"
            Expect.equal (classifyDisplay (Map [ displayKey, ":1" ])) X11Only "x11-only"
            Expect.equal (classifyDisplay Map.empty) Neither "neither"
            Expect.equal (classifyDisplay (Map [ waylandKey, ""; displayKey, ":1" ])) X11Only "empty WAYLAND_DISPLAY treated as absent"
        }

        test "normalizeGraphicsEnv is idempotent over arbitrary maps" {
            let prop (pairs: (string * string) list) =
                let m = cleanMap pairs
                normalizeGraphicsEnv (normalizeGraphicsEnv m) = normalizeGraphicsEnv m
            Check.QuickThrowOnFailure prop
        }

        test "normalizeGraphicsEnv preserves every key outside the three named keys" {
            let named = Set [ waylandKey; gdkKey; sdlKey ]
            let prop (pairs: (string * string) list) =
                let m = cleanMap pairs
                let out = normalizeGraphicsEnv m
                m |> Map.forall (fun k v -> named.Contains k || Map.tryFind k out = Some v)
            Check.QuickThrowOnFailure prop
        }
    ]

// US1 (T008): a REAL process-spawn contract test. It synthesizes a DualDisplay ambient by
// passing both display variables in the caller map, launches a real `bash` child through
// BuildProcess, and inspects the child's REAL inherited environment and REAL exit code — no
// mock. Asserts C2 (no WAYLAND_DISPLAY; GDK_BACKEND=x11) and C5/FR-008 (a nonzero exit is
// still reported as a failure with its code propagated).
[<Tests>]
let graphicsSpawnContractTests =
    let probeScript =
        "-c \"echo WD=[$WAYLAND_DISPLAY]; echo GB=[$GDK_BACKEND]; echo SD=[$SDL_VIDEODRIVER]\""

    let dualDisplay = Map [ waylandKey, "wayland-0"; displayKey, ":1" ]

    testList "Graphics environment spawn edge (C2/C5)" [
        test "child launched under a DualDisplay ambient observes no WAYLAND_DISPLAY and the forced x11 backends" {
            let output = Path.GetTempFileName()

            try
                BuildProcess.runProcess
                    "graphics-env-spawn-probe"
                    "bash"
                    probeScript
                    AppContext.BaseDirectory
                    output
                    dualDisplay

                let text = File.ReadAllText output
                Expect.stringContains text "WD=[]" "child sees no WAYLAND_DISPLAY"
                Expect.stringContains text "GB=[x11]" "child sees GDK_BACKEND=x11"
                Expect.stringContains text "SD=[x11]" "child sees SDL_VIDEODRIVER=x11"
            finally
                if File.Exists output then File.Delete output
        }

        test "a nonzero child exit under the normalized spawn is still reported as a failure with its code propagated" {
            let output = Path.GetTempFileName()

            try
                Expect.throws
                    (fun () ->
                        BuildProcess.runProcessWithAllowedExitCodes
                            "graphics-env-failure-probe"
                            "bash"
                            "-c \"exit 7\""
                            AppContext.BaseDirectory
                            output
                            dualDisplay
                            (Set.singleton 0))
                    "a genuine nonzero exit must surface as a failure (C5 / FR-008 / SC-006)"

                let text = File.ReadAllText output
                Expect.stringContains text "exit-code=7" "the child's real exit code is propagated unchanged"
            finally
                if File.Exists output then File.Delete output
        }
    ]

// US2 (T011): unit test for the kill-on-timeout diagnostic builder. A process killed at its
// WaitForExit bound must produce a message that names a probable graphics-backend initialization
// failure and points to runtime-limitations.md, while staying distinct from an ordinary
// nonzero-exit message (C4 / FR-005 / SC-005).
[<Tests>]
let graphicsTimeoutDiagnosticTests =
    testList "Graphics environment timeout diagnostic (C4)" [
        test "kill-on-timeout diagnostic names a probable graphics-backend init failure and points to runtime-limitations" {
            let message =
                BuildEnvironment.graphicsTimeoutDiagnostic "GeneratedProductCheck" "readiness/logs/generated-product-check.log"

            Expect.stringContains message "GeneratedProductCheck" "names the killed step"
            Expect.stringContains message "timed out" "identifies it as a timeout/kill"
            Expect.stringContains (message.ToLowerInvariant()) "graphics backend" "names a probable graphics-backend initialization failure"
            Expect.stringContains message "runtime-limitations.md" "points to the runtime-limitations note"
            Expect.stringContains message "readiness/logs/generated-product-check.log" "includes the output path"
        }

        test "the timeout diagnostic is distinct from an ordinary nonzero-exit message" {
            let message = BuildEnvironment.graphicsTimeoutDiagnostic "GeneratedProductCheck" "out.log"
            Expect.isFalse (message.Contains "failed with exit code") "distinct from the nonzero-exit message (does not mask a product regression)"
        }
    ]
