module BuildEnvironment

// Feature 049 — deterministic escalated validation path.
//
// The headless host advertises BOTH a Wayland display (WAYLAND_DISPLAY) and an X11/Xvfb
// display (DISPLAY) at once. The graphics stack then prefers the Wayland path and tries to
// load libdecor-gtk.so, which cannot initialize in the container — causing a teardown crash
// and a ~20-minute graphics-init stall. This module classifies the ambient display state and,
// ONLY when both displays are advertised, forces the already-working X11 path. It is a strict
// no-op on every other host (real Wayland desktop, X11-only, or non-Linux / no-display), so
// behavior and visual output on already-working hosts are unchanged (FR-002 / FR-007).
//
// The core (`classifyDisplay`, `normalizeGraphicsEnv`) is a PURE Map<string,string> ->
// Map<string,string> function consumed at the interpreter edge (process spawn) and at Program
// startup; it adds no new Model/Msg/Effect.

open System

let waylandDisplayKey = "WAYLAND_DISPLAY"

let displayKey = "DISPLAY"

let gdkBackendKey = "GDK_BACKEND"

let sdlVideoDriverKey = "SDL_VIDEODRIVER"

/// The four display classifications derived from WAYLAND_DISPLAY / DISPLAY presence.
type GraphicsDisplayState =
    | DualDisplay
    | WaylandOnly
    | X11Only
    | Neither

/// A variable is "present" only when it is set and non-empty.
let private present (env: Map<string, string>) (key: string) =
    match Map.tryFind key env with
    | Some value -> not (String.IsNullOrEmpty value)
    | None -> false

/// Classify the display state of an environment map.
let classifyDisplay (env: Map<string, string>) : GraphicsDisplayState =
    match present env waylandDisplayKey, present env displayKey with
    | true, true -> DualDisplay
    | true, false -> WaylandOnly
    | false, true -> X11Only
    | false, false -> Neither

/// Pure normalization: only the DualDisplay failure condition forces the X11 path
/// (remove WAYLAND_DISPLAY; set GDK_BACKEND=x11, SDL_VIDEODRIVER=x11). Every other
/// classification is the identity. Total, idempotent (one pass yields X11Only -> no-op),
/// and non-destructive beyond the three named keys.
let normalizeGraphicsEnv (env: Map<string, string>) : Map<string, string> =
    match classifyDisplay env with
    | DualDisplay ->
        env
        |> Map.remove waylandDisplayKey
        |> Map.add gdkBackendKey "x11"
        |> Map.add sdlVideoDriverKey "x11"
    | WaylandOnly
    | X11Only
    | Neither -> env

/// Human-readable description of the normalization decision, logged once at startup.
let describeDecision (state: GraphicsDisplayState) : string =
    match state with
    | DualDisplay ->
        sprintf
            "graphics-env normalization: DualDisplay detected — removed %s; set %s=x11, %s=x11"
            waylandDisplayKey
            gdkBackendKey
            sdlVideoDriverKey
    | WaylandOnly -> "graphics-env normalization: no-op (condition not met: WaylandOnly)"
    | X11Only -> "graphics-env normalization: no-op (condition not met: X11Only)"
    | Neither -> "graphics-env normalization: no-op (condition not met: Neither)"

/// Enriched diagnostic for a graphics-initializing child killed at its WaitForExit bound. It
/// names a probable graphics-backend initialization failure as a candidate cause and points to
/// runtime-limitations.md, so an environment failure is distinguishable from a product regression.
/// Deliberately distinct from the ordinary "failed with exit code" message: this fails fast and
/// legibly without masking a real regression (C4 / FR-005 / SC-005). Retains the historical
/// "timed out. See <path>" phrasing so existing callers/contract checks keep matching.
let graphicsTimeoutDiagnostic (label: string) (outputPath: string) : string =
    sprintf
        "%s timed out. See %s. A probable cause is that a graphics backend could not initialize in this environment (for example, a dual Wayland/X11 display whose libdecor plugin cannot start); see specs/049-fix-escalated-flake/readiness/runtime-limitations.md"
        label
        outputPath

/// Snapshot the current process environment as a string->string map (interpreter edge).
let currentEnvironment () : Map<string, string> =
    Environment.GetEnvironmentVariables()
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.choose (fun entry ->
        match entry.Key, entry.Value with
        | (:? string as key), (:? string as value) -> Some(key, value)
        | _ -> None)
    |> Map.ofSeq

/// Normalize the ambient process environment in place when DualDisplay holds, so every
/// descendant (dotnet test, FSI, nested `bash ./fake.sh build -t <target>`) inherits the
/// deterministic selection. Returns the decision log line. Called once at Program startup.
let normalizeAmbient () : string =
    let state = classifyDisplay (currentEnvironment ())

    match state with
    | DualDisplay ->
        Environment.SetEnvironmentVariable(waylandDisplayKey, null)
        Environment.SetEnvironmentVariable(gdkBackendKey, "x11")
        Environment.SetEnvironmentVariable(sdlVideoDriverKey, "x11")
    | WaylandOnly
    | X11Only
    | Neither -> ()

    describeDecision state
