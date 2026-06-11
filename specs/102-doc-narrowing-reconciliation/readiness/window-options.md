# Window options — applicability (feature 102, R8)

R8 opens no window and configures no window options; every option is recorded not-applicable with an
honest status (no option is silently ignored).

option=resize status=not-applicable observed=false note=no window created by R8
option=maximize status=not-applicable observed=false note=no window created by R8
option=startup-state status=not-applicable observed=false note=no host launch in R8
option=startup-position status=not-applicable observed=false note=no host launch in R8
option=backend status=not-applicable observed=false note=no GPU/window backend exercised; R8 edits prose and comments

See [window-visibility.md](./window-visibility.md) — R8 is a documentation/internal-comment honesty
pass with no graphical surface.
