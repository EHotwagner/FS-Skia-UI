#r "../../../../src/Testing/bin/Debug/net10.0/FS.Skia.UI.Testing.dll"

open FS.Skia.UI.Testing

let sourcePath = "specs/021-persistent-launch-evidence/readiness/logs/t040-repeated-launches.txt"
let raw = System.IO.File.ReadAllText(sourcePath)

let firstAttempt =
    raw.Split("## attempt 02", System.StringSplitOptions.None).[0].Trim()

let warningText =
    firstAttempt.Split('\n')
    |> Array.filter (fun line -> line.Contains("Gtk-Message"))
    |> String.concat "\n"

let launchSucceeded =
    firstAttempt.Contains("status=ok")
    && firstAttempt.Contains("first-frame-presented=true")

let result =
    HostWarningClassification.classify
        { RawMessage = warningText
          KnownBenignMarkers = [ "colorreload-gtk-module"; "window-decorations-gtk-module" ]
          LaunchSucceeded = launchSucceeded
          RenderingSucceeded = launchSucceeded
          LayoutReadable = Some true
          ExplicitlyUnsupportedWithoutReadabilityClaim = false
          PackageSucceeded = true
          EvidencePath = Some sourcePath }

printfn "source=%s" sourcePath
printfn "source-kind=real-preserved-launch-transcript"
printfn "current-feature-launch-attempt=specs/024-racer-feedback-followups/readiness/logs/t025-real-launch-output.txt"
printfn "launch-succeeded=%b" launchSucceeded
printfn "raw-warning-lines=%d" (warningText.Split('\n', System.StringSplitOptions.RemoveEmptyEntries).Length)
printfn "warning-class=%A" result.WarningClass
printfn "fatal=%b" result.Fatal

for fact in result.SupportingFacts do
    printfn "fact=%s" fact

for diagnostic in result.Diagnostics do
    printfn "diagnostic=%s" diagnostic
