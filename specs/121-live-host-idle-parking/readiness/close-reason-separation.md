# close-reason-separation — applicability (feature 121)

status=not-applicable
close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Feature 121 opens and observes no window in CI, so there is no window-close reason to classify in this
feature's own evidence — user-close and evidence-self-close stay separated by the **unchanged**
`runInteractiveApp` / `runInteractiveViewer` close contract (evidence-close-observed is never reported
as user-close-observed). Feature 121 only **reconciles** the already-shipped graceful-quit path: a host
`update` returning `[ ViewerEffect.CloseWindow ]` propagates to `AppRequestedClose` + `Shutdown`
(`SkiaViewer.fs`), the `AppRequestedClose` reason — distinct from `UserClose` and
`EvidenceRequestedClose`. No source edit to the close / lifecycle seam.
</content>
