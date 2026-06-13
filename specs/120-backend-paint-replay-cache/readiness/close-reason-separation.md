# close-reason-separation — feature 120 (US2)

close-reason=evidence-self-close
user-close-observed=false
evidence-close-observed=true

The feature-120 live-host run closes for a single reason: the bounded evidence tick count was
reached (the host requests `Shutdown` after the idle-skip + timing captures). This is an evidence
self-close (evidence-close-observed=true), kept distinct from a user close
(user-close-observed=false — no user interaction drove the close). The existing
`runInteractiveApp` / `runApp` user-close (window-manager close → `CloseRequested`) contract is
unchanged by feature 120 (the idle-skip never suppresses a close: a close request is a dirty cause).
