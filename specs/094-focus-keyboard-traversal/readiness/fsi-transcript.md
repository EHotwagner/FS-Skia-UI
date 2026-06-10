# FSI transcript — exercising the public Focus surface (Principle I)

The public `Focus` front door (`Focus.order` / `Focus.traverse` / `Focus.route`) is exercised from
FSI against the **built** `FS.Skia.UI.Controls` assembly (the packed-equivalent surface), proving
the consumer-reachable path works end-to-end — not just internal helpers (T008).

## Script

```fsharp
#r "FS.Skia.UI.Scene.dll" ; #r "FS.Skia.UI.Layout.dll"
#r "FS.Skia.UI.KeyboardInput.dll" ; #r "FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Controls

let withFO n (md: AccessibilityMetadata) = { md with FocusOrder = n }
let focusable kind key fo : Control<int> =
    Control.create kind [ Attr.text key; Attr.accessibility (Accessibility.defaultFor kind key |> withFO fo) ]
    |> Control.withKey key

let view : Control<int> =
    Stack.create [ Stack.children [
        TextBlock.create [ TextBlock.text "heading" ] |> Control.withKey "heading"   // static-text, non-focusable
        focusable "button" "act-none" None
        focusable "slider" "nav-1" (Some 1)
        focusable "button" "act-0" (Some 0) ] ]

let order = Focus.order view
order.Stops |> List.map (fun s -> s.Control)          // focusable-only, FocusOrder-then-doc order
Focus.traverse order None Next                        // first stop
Focus.traverse order None Previous                    // last stop
Focus.traverse order (Some "act-0") Next              // next
Focus.traverse order (Some "act-none") Next           // wraps to first
let buttonKb = Accessibility.keyboard true [ "Enter"; "Space" ] []
let sliderKb = Accessibility.keyboard true [] [ "ArrowLeft"; "ArrowRight" ]
Focus.route buttonKb "Enter" false false              // Activate
Focus.route buttonKb "Tab"   true  false              // Traverse Next (Tab not in button keys)
Focus.route buttonKb "Tab"   true  true               // Traverse Previous
Focus.route sliderKb "ArrowLeft" false false          // Navigate
Focus.route sliderKb "Q" false false                  // Fallthrough
```

## Captured output

```text
order.Stops ids = ["act-0"; "nav-1"; "act-none"]      (heading excluded — non-focusable static text)
first (None+Next) = Some "act-0"
last  (None+Prev) = Some "act-none"
act-0 +Next       = Some "nav-1"
act-none +Next (wrap) = Some "act-0"
route button Enter        = Activate
route button Tab          = Traverse Next
route button Shift+Tab    = Traverse Previous
route slider ArrowLeft    = Navigate
route slider Q (no match) = Fallthrough
```

## Result

PASS — `Focus.order` (focusable-only, FocusOrder-then-document order, non-focusable static text and
layout containers excluded), `Focus.traverse` (None-seeded, cyclic wrap), and `Focus.route`
(Activate / Navigate / Traverse / Fallthrough, Tab not consumed by a default control) all work
through the public surface. The `view : 'model -> Control<'msg>` contract is unchanged for
keyboard-free consumers (additive).
