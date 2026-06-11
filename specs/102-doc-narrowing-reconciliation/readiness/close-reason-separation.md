# Close-reason separation — applicability (feature 102, R8)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

R8 launches no window, so there is no window close to classify. Neither a user close nor an
evidence/self close occurs; the two are trivially separated (both absent). The evidence-close and
user-close channels are kept distinct: an evidence-close-observed value is never reported as a
user-close-observed value. See [window-visibility.md](./window-visibility.md) for the not-applicable
decision.
