# close-reason-separation — applicability (feature 116, T002)

status=not-applicable

Feature 116 opens and closes no window, so there is no window-close reason to classify (no
user-close vs evidence-self-close vs error-close separation to make). The existing `runInteractiveApp`
launch/close contract is unchanged (no source edit to the launch seam).
