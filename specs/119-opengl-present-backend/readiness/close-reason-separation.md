# close-reason-separation — feature 119 (US1)

close-reason=evidence-self-close
user-close-observed=false
evidence-close-observed=true

The feature-119 live-host run closes for a single reason: the bounded evidence frame count was
reached (the host requests `Shutdown` at frame 60). This is an evidence self-close
(evidence-close-observed=true), kept distinct from a user close (user-close-observed=false — no
user interaction drove the close). The evidence close is not reported as a user close; the two
reasons stay separated. The existing `runInteractiveApp` / `runApp` user-close (window-manager
close → `CloseRequested`) contract is unchanged by the backend swap.
