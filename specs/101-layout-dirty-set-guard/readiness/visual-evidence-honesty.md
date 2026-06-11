# Visual-evidence honesty (feature 101, R7, T002/T003)

evidence-kind=visual-evidence-honesty
status=not-applicable
authoritative-command=./fake.sh build -t Dev (Controls.Tests/Feature101*)
artifact-path=specs/101-layout-dirty-set-guard/readiness/drift-guard.md
failure-class=layout-dirty-set-drift
next-action=run the drift-report + behavioral-probe suite under Dev; a named Uncovered/OverBroad finding is the drift failure

## Honesty contract

R7 produces **no** pixel/screenshot artifact and makes **no** desktop-visibility claim (see
[real-image-evidence.md](./real-image-evidence.md) and [window-visibility.md](./window-visibility.md)).
There is nothing to misrepresent as visual proof: metadata-only reports do not satisfy visual proof,
1x1 fallback images do not satisfy visual proof, and layout-only bounds claims do not satisfy visual
proof — none of which R7 asserts.

The user-reachable surface for this hardening feature is the **build/test gate itself**: a contributor
who introduces dirty-set drift gets a fast, explicit, named Expecto failure under `Dev`
(`un-covered layout input: 'x' …` / `over-broad classifier entry: 'x' …`). Rendering output is
**byte-identical** to the pre-R7 path (R2 INV-1), so "renders" is never passed off as "responds" and no
render/visual claim is made at all. No benign host warning is reclassified as blocking and no blocking
failure is hidden as benign — there is no host warning in scope (in-process, off-window, deterministic
tests).
