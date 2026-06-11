# FSI transcript — public navigation surface (feature 100, R5, T021)

evidence-kind=fsi-transcript
status=observed

Exercised the **public** navigation classification (`Focus.route` -> `NavIntent`) from FSI against the
compiled `FS.Skia.UI.Controls` assembly, per `quickstart.md`. Navigation is derived **purely** from the
declared role + `NavigationKeys` (+ `NavRange`) — no consumer key-handling code, no per-kind branch.
The full host vertical slice (a focused control driven through `runInteractiveApp` producing the
dispatched `'msg`) is proven through the **real** `routeFocusedKey` seam in
`tests/Elmish.Tests/Feature100NavigationTests.fs` (the seam is module-internal, reached by the test
assembly via `InternalsVisibleTo`); see [responds-vs-renders.md](./responds-vs-renders.md) and
[declared-step.md](./declared-step.md).

## Script (`/tmp/nav100.fsx`)

```fsharp
#r ".../FS.Skia.UI.Scene.dll"
#r ".../FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Controls

let arrowKb = Accessibility.keyboard true [] [ "ArrowUp"; "ArrowDown"; "ArrowLeft"; "ArrowRight" ]
let sliderKb = Accessibility.keyboard true [] [ "ArrowLeft"; "ArrowRight" ]
let range : NavRange option = Some { Step = 0.1; Min = 0.0; Max = 1.0 }

Focus.route AccessibilityRole.Slider    sliderKb range "ArrowRight" false false
Focus.route AccessibilityRole.RadioGroup arrowKb None  "ArrowDown"  false false
Focus.route AccessibilityRole.Grid       arrowKb None  "ArrowRight" false false
Focus.route AccessibilityRole.Button     (Accessibility.keyboard true [ "Enter"; "Space" ] []) None "ArrowRight" false false
Focus.route AccessibilityRole.Button     (Accessibility.keyboard true [ "Enter"; "Space" ] []) None "Enter"      false false
Focus.route AccessibilityRole.Slider     sliderKb None  "ArrowRight" false false
```

## Observed output

```
value  role (Slider)     ArrowRight -> Navigate (ValueStep 0.1)
select role (RadioGroup)  ArrowDown  -> Navigate (SelectionMove Next)
grid   role (Grid)        ArrowRight -> Navigate (GridMove (0, 1))
button (non-navigable)    ArrowRight -> Fallthrough
button activation         Enter      -> Activate
value role w/o NavRange   ArrowRight -> Fallthrough
```

Each intent class is reproduced from declared metadata alone; a non-navigable role and a value role
without a `NavRange` form **no** intent (`Fallthrough`, FR-008); activation (E4) is unaffected.
