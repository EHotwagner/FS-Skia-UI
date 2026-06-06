// FSI exercise of the feature-073 animation surface (T009). Loads the locally
// built FS.Skia.UI.Scene + FS.Skia.UI.Elmish assemblies and drives the
// author-facing entry points: Easing.apply, Tween.sample, Animation.applyAt,
// AnimationState.create/retarget/advance, and the Elmish tick subscription.

#r "../../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "/home/developer/.nuget/packages/fable.elmish/4.2.0/lib/netstandard2.0/Fable.Elmish.dll"
#r "../../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"
#r "../../../../src/Elmish/bin/Debug/net10.0/FS.Skia.UI.Elmish.dll"

open System
open FS.Skia.UI.Scene

printfn "== Easing endpoints + default =="
[ Linear; EaseIn; EaseOut; EaseInOut ]
|> List.iter (fun e -> printfn "%A: f(0)=%.3f f(0.5)=%.3f f(1)=%.3f" e (Easing.apply e 0.0) (Easing.apply e 0.5) (Easing.apply e 1.0))
printfn "Easing.Default = %A" Easing.Default

printfn "\n== Tween.sample (opacity 0->1, ease-out, 300ms) =="
let opacity = { Start = 0.0; End = 1.0; Duration = TimeSpan.FromMilliseconds 300.0; Easing = EaseOut }
for ms in [ 0.0; 150.0; 300.0 ] do
    printfn "t=%.0fms opacity=%.4f" ms (Tween.sample Animation.lerpFloat (TimeSpan.FromMilliseconds ms) opacity)

printfn "\n== Animation.applyAt (entrance) — identity-at-rest =="
let panel = Scene.group [ Scene.rectangleWithPaint { X = 16.0; Y = 16.0; Width = 288.0; Height = 120.0 } (Paint.fill Colors.white) ]
let entrance =
    { Animation.empty with
        Opacity = Some opacity
        Transform = Some { Start = { Transform.identity with TranslateY = 24.0 }; End = Transform.identity; Duration = TimeSpan.FromMilliseconds 300.0; Easing = EaseOut } }
printfn "start node kind:   %A" (Scene.describe { Nodes = [ Animation.applyAt TimeSpan.Zero entrance panel ] })
printfn "settled node kind: %A" (Scene.describe { Nodes = [ Animation.applyAt (TimeSpan.FromMilliseconds 300.0) entrance panel ] })
printfn "settled == static node: %b" (Animation.applyAt (TimeSpan.FromMilliseconds 300.0) entrance panel = (match panel.Nodes with [ n ] -> n | _ -> Empty))
printfn "isSettled at 300ms: %b" (Animation.isSettled (TimeSpan.FromMilliseconds 300.0) entrance)

printfn "\n== AnimationState retarget (no snap-back) =="
let s0 = AnimationState.create Animation.lerpFloat 0.0 (TimeSpan.FromMilliseconds 200.0) Linear |> AnimationState.retarget 100.0
let s1 = AnimationState.advance (TimeSpan.FromMilliseconds 100.0) s0
printfn "displayed mid-flight = %.2f (active=%b)" (AnimationState.value s1) (AnimationState.isActive s1)
let s2 = AnimationState.retarget 0.0 s1
printfn "after retarget: Start=%.2f Current=%.2f Target=%.2f (no jump back to 0 start)" s2.Start s2.Current s2.Target

printfn "\n== Elmish tick subscription shape =="
let sub = FS.Skia.UI.Elmish.Animation.tickSubscription (fun running -> running) FS.Skia.UI.Elmish.AnimationTick (TimeSpan.FromMilliseconds 16.0)
printfn "active model -> %d sub entrie(s)" (List.length (sub true))
printfn "settled model -> %d sub entrie(s)" (List.length (sub false))

printfn "\nFSI animation surface exercise: OK"
