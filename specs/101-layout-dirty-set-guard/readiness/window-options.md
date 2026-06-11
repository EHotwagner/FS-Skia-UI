# Window options — applicability (feature 101, R7)

R7 opens no window and configures no window options; every option is recorded not-applicable with an
honest status (no option is silently ignored).

option=resize status=not-applicable observed=false note=no window created by R7
option=maximize status=not-applicable observed=false note=no window created by R7
option=startup-state status=not-applicable observed=false note=no host launch in R7
option=startup-position status=not-applicable observed=false note=no host launch in R7
option=backend status=not-applicable observed=false note=no GPU/window backend exercised; tests are in-process

See [window-visibility.md](./window-visibility.md) — R7 is a framework-internal classifier guard with
no graphical surface.
