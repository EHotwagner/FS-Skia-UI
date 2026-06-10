// Feature 090 — capture REAL responds-proof artifacts (FR-006/FR-007, T021/T024) over the
// production Control.renderTree path via ControlsElmish.captureRespondsProof / routeFocusedText.
// Render-only: no live Vulkan window (plan §Technical Context). Run with the test bin on the
// load path so SkiaSharp native + all package deps resolve.

#I "/home/developer/projects/FS-Skia-UI/tests/Elmish.Tests/bin/Debug/net10.0"
#r "FS.Skia.UI.Scene.dll"
#r "FS.Skia.UI.Layout.dll"
#r "FS.Skia.UI.KeyboardInput.dll"
#r "FS.Skia.UI.SkiaViewer.dll"
#r "FS.Skia.UI.Elmish.dll"
#r "FS.Skia.UI.Controls.dll"
#r "FS.Skia.UI.Controls.Elmish.dll"

open System
open System.IO
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.SkiaViewer

type Msg =
    | Increment
    | SetName of string

type Model = { Count: int; Name: string }

let size = { Width = 320; Height = 200 }

let update (msg: Msg) (model: Model) : Model * ViewerEffect list =
    match msg with
    | Increment -> { model with Count = model.Count + 1 }, []
    | SetName n -> { model with Name = n }, []

let hostOf view : InteractiveAppHost<Model, Msg> =
    { Init = fun () -> { Count = 0; Name = "" }, []
      Update = update
      View = view
      Theme = Theme.light
      MapKey = fun _ _ -> None
      MapPointer = fun _ -> None
      Tick = fun _ -> None
      Diagnostics = Viewer.defaultDiagnostics }

let pointer phase x y : ViewerPointerInput =
    { Phase = phase; X = x; Y = y; Button = Some ViewerPointerButtonKind.Primary; DeltaX = 0.0; DeltaY = 0.0 }

let centreOf (host: InteractiveAppHost<Model, Msg>) model nodeId =
    let rendered = Control.renderTree host.Theme size (host.View size model)
    let available: FS.Skia.UI.Layout.AvailableSpace =
        { Width = float size.Width; WidthMode = FS.Skia.UI.Layout.Exactly
          Height = float size.Height; HeightMode = FS.Skia.UI.Layout.Exactly }
    let result = FS.Skia.UI.Layout.Layout.evaluate available rendered.Layout
    let b = result.Bounds |> List.find (fun b -> b.NodeId = nodeId)
    b.Bounds.X + b.Bounds.Width / 2.0, b.Bounds.Y + b.Bounds.Height / 2.0

// Decodable text projection of a Scene: every rendered text run, in order. The before/after
// difference is human-readable (e.g. the counter label "0" -> "1").
let rec sceneTexts (scene: Scene) : string list = scene.Nodes |> List.collect nodeTexts
and nodeTexts (node: SceneNode) : string list =
    match node with
    | SceneNode.Group scenes -> scenes |> List.collect sceneTexts
    | SceneNode.ClipNode(_, inner)
    | SceneNode.ColorSpaceNode(_, inner)
    | SceneNode.PerspectiveNode(_, inner)
    | SceneNode.Translate(_, inner) -> sceneTexts inner
    | SceneNode.PictureNode pic -> sceneTexts pic.Scene
    | SceneNode.Text(_, t, _) -> [ t ]
    | SceneNode.TextRun run -> [ run.Text ]
    | SceneNode.SizedText(_, t, _, _) -> [ t ]
    | _ -> []

let outDir = Path.Combine(__SOURCE_DIRECTORY__, "responds-proof")

let writeCase (name: string) (proof: RespondsProof) (note: string) =
    let dir = Path.Combine(outDir, name)
    Directory.CreateDirectory dir |> ignore
    let beforeText = sceneTexts proof.Before |> String.concat " | "
    let afterText = sceneTexts proof.After |> String.concat " | "
    File.WriteAllText(Path.Combine(dir, "before.txt"), sprintf "render-only before-frame text runs:\n%s\n" beforeText)
    File.WriteAllText(Path.Combine(dir, "after.txt"), sprintf "render-only after-frame text runs:\n%s\n" afterText)
    let verdict = match proof.Verdict with | Responsive -> "Responsive" | Inert -> "Inert"
    let body =
        sprintf "# Responds-proof — %s (feature 090, FR-006)\n\nverdict=%s\nmode=render-only\nartifact-decodable=true\nproves-scene-rendering=true\nproves-desktop-visibility=false\nbefore-after-differ=%b\n\n%s\n\nbefore: %s\nafter:  %s\n"
            name verdict (proof.Before <> proof.After) note beforeText afterText
    File.WriteAllText(Path.Combine(dir, "responds-proof.txt"), body)
    printfn "%s: verdict=%s  before=[%s]  after=[%s]" name verdict beforeText afterText

// --- Case 1: leaf-keyed onClick (US1) ---
let leafView (_: Size) (m: Model) : Control<Msg> =
    Stack.create [ Stack.children [ Button.create [ Button.text (string m.Count); Button.onClick Increment ] |> Control.withKey "inc" ] ]
let leafHost = hostOf leafView
let m0 = fst (leafHost.Init ())
let lx, ly = centreOf leafHost m0 "inc"
let leafPressed, _ = ControlsElmish.routeInteractivePointer leafHost (Pointer.init ()) size m0 (pointer ViewerPointerPhaseKind.Pressed lx ly)
let leafProof = ControlsElmish.captureRespondsProof leafHost leafPressed size m0 (pointer ViewerPointerPhaseKind.Released lx ly)
writeCase "leaf" leafProof "A leaf-keyed Button.onClick increments the counter; the rendered button label changes 0 -> 1."

// --- Case 2: container-keyed composite (US2, routed via nearestAuthored) ---
let containerView (_: Size) (m: Model) : Control<Msg> =
    Stack.create [ Stack.children [ TextBlock.create [ TextBlock.text (string m.Count) ] ]; Attr.on "onClick" Increment ]
    |> Control.withKey "panel"
let containerHost = hostOf containerView
let cx, cy = centreOf containerHost m0 "0.0"   // an inner positional node inside the keyed container
let cPressed, _ = ControlsElmish.routeInteractivePointer containerHost (Pointer.init ()) size m0 (pointer ViewerPointerPhaseKind.Pressed cx cy)
let containerProof = ControlsElmish.captureRespondsProof containerHost cPressed size m0 (pointer ViewerPointerPhaseKind.Released cx cy)
writeCase "container" containerProof "A click inside a container keyed 'panel' resolves via nearestAuthored to the container id and fires its onClick; the inner counter label changes 0 -> 1."

// --- Case 3: focused-text seam (US4) ---
let textView (_: Size) (m: Model) : Control<Msg> =
    Stack.create [ Stack.children [ TextBox.create [ TextBox.value m.Name; TextBox.onChanged SetName ] |> Control.withKey "name" ] ]
let textHost = hostOf textView
let tRendered = Control.renderTree textHost.Theme size (textHost.View size m0)
let beforeText = (Control.renderTree textHost.Theme size (textHost.View size m0)).Scene
let models = [ "name", fst (TextInput.init "name" SingleLine "") ] |> Map.ofList
let models', tMsgs = ControlsElmish.routeFocusedText tRendered (Some "name") models (InsertText "h")
let mText = tMsgs |> List.fold (fun m msg -> fst (update msg m)) m0
let afterText = (Control.renderTree textHost.Theme size (textHost.View size mText)).Scene
let textProof = ControlsElmish.respondsProofOf beforeText afterText
writeCase "text" textProof "A keystroke delivered to the focused TextBox 'name' through the focus-aware seam folds SetName 'h'; the rendered field text changes '' -> 'h'."

printfn "DONE"
