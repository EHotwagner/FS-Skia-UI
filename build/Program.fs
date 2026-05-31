module FS.Skia.UI.Build.Program

// Dedicated, compiled FAKE build front-end (D2 spike). Consumes the
// Fake.Core.Target API as an ordinary library from a `dotnet run` exe — no FSX
// script runner, no FSharp.Compiler.Service. The SpikeHello target's body is
// ONLY a call into the referenced governance library (FS.Skia.UI.Build.Spike),
// proving the body ran from the library and not inlined here.

open Fake.Core

[<EntryPoint>]
let main argv =
    // Establish a FAKE runtime execution context for the Target API outside the
    // FSX runner. argv flows through so target dispatch can read it.
    let execContext =
        Context.FakeExecutionContext.Create false "spike" (List.ofArray argv)
    Context.setExecutionContext (Context.RuntimeContext.Fake execContext)
    Target.initEnvironment ()

    Target.create "SpikeHello" (fun _ ->
        // Body delegates entirely to the library — no inlined logic.
        FS.Skia.UI.Build.Spike.run () |> printfn "%s")

    Target.runOrDefaultWithArguments "SpikeHello"
    0
