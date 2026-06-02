module FS.Skia.UI.Build.Program

// Dedicated, compiled FAKE build front-end (feature 045, FR-001/FR-005). Consumes the
// Fake.Core.Target API as an ordinary library from a `dotnet run` exe — no FSX script
// runner, no F# compiler-service dependency. Every target is registered directly off the typed
// Targets.dispatchTargets registry and each body delegates to Engine.Interpret.runTarget;
// the exe contains NO inlined orchestration/validation logic (FR-001/SC-006). A renamed or
// mistyped target is a compile error in Targets.fs, not a silently-unregistered FAKE target.

open Fake.Core
open Fake.Core.TargetOperators
open FS.Skia.UI.Build

[<EntryPoint>]
let main argv =
    // Feature 049: normalize this process's ambient graphics environment once at startup. When the
    // host advertises BOTH a Wayland and an X11 display (the headless dual-display failure
    // condition), this removes WAYLAND_DISPLAY and forces GDK_BACKEND/SDL_VIDEODRIVER=x11; on every
    // other host it is a strict no-op. Because children are spawned with UseShellExecute=false they
    // inherit this environment, and nested generated-product validation re-enters the front-end via
    // `bash ./fake.sh build -t <target>`, so one normalization propagates to every descendant
    // (dotnet test, FSI, nested fake.sh) (FR-002/FR-003). The decision is logged once.
    printfn "%s" (BuildEnvironment.normalizeAmbient ())

    // The `dotnet fake build -t <name>` CLI consumed the leading `build` subcommand and
    // forwarded only `-t <name> [flags]` to the script's runOrDefaultWithArguments. The
    // launchers still pass `build -t <name>`, so strip a leading `build` token here to
    // preserve identical argument semantics (front-end-cli contract).
    let forwarded =
        match List.ofArray argv with
        | "build" :: rest -> rest
        | all -> all

    // Establish a FAKE runtime execution context for the Target API outside the FSX runner.
    let execContext =
        Context.FakeExecutionContext.Create false "build" forwarded

    Context.setExecutionContext (Context.RuntimeContext.Fake execContext)
    Target.initEnvironment ()

    // Register every dispatched target; each body delegates to the relocated library engine.
    Targets.dispatchTargets
    |> List.iter (fun target -> Target.create (Targets.name target) (fun _ -> Engine.Interpret.runTarget target))

    // Wire the ==> dependency edges from the derived dependency rows (FR-001/FR-013).
    Targets.targetDependencyRows
    |> List.iter (fun (targetName, dependencies) ->
        dependencies
        |> List.iter (fun dependency -> dependency ==> targetName |> ignore))

    // Forward the target name + flags verbatim; preserve today's default target (Dev).
    Target.runOrDefaultWithArguments "Dev"
    0
