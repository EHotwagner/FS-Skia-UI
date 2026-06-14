# close-reason-separation — applicability (feature 122)

status=not-applicable
close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Feature 122 opens and observes no window in CI, so there is no window-close reason to classify in this
feature's own evidence — user-close and evidence-self-close stay separated by the **unchanged**
`runInteractiveApp` / `runInteractiveViewer` close contract (evidence-close-observed is never reported as
user-close-observed). The additive `runInteractiveAppWithWindowBehavior` overload delegates to the same
`runInteractiveViewerWithWindowBehavior` launch/close seam; no edit is made to the close / lifecycle path.
