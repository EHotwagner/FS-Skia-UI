# Visual-evidence honesty (feature 100, R5, T002)

evidence-kind=visual-evidence-honesty
status=render-only
authoritative-command=./fake.sh build -t Dev (Elmish.Tests/Feature100*)
artifact-path=specs/100-general-navigation-keys/readiness/responds-vs-renders.md
failure-class=inert-or-pre-r5-no-dispatch
next-action=run the host resolver suite through the real routeFocusedKey seam; a no-dispatch result on a focused radio-group arrow is a pre-R5/un-wired failure

## Honesty contract

This feature produces **no** pixel/screenshot artifact and makes **no** desktop-visibility claim
(see [real-image-evidence.md](./real-image-evidence.md) and [window-visibility.md](./window-visibility.md)).
The rendered-output evidence is the **responds-vs-renders** dispatch observation through the real
`runInteractiveApp` seam: a real input (a focused-control arrow press) produces a real dispatched `'msg`
with the moved selection/value/cell. At-rest rendered output is **byte-identical** to the pre-R5 path
(navigation produces a `'msg`, no layout/render algorithm change), so "renders" is never passed off as
"responds": a pre-R5 / un-wired build dispatches nothing and cannot produce the responds artifact. No
benign host warning is reclassified as blocking and no blocking failure is hidden as benign — there is
no host warning in scope (off-window deterministic capture).
