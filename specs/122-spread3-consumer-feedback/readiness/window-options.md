# window-options — applicability (feature 122)

Feature 122 opens no new window in this evidence, so every option is not-applicable here (none is
silently ignored). Note: US2/FR-005 now threads the **startup-state** option into the live controls
launch via `runInteractiveAppWithWindowBehavior` — the option that was previously inert for the controls
profile — but no window is driven in CI to observe it.

option=resize status=not-applicable observed=false
option=maximize status=not-applicable observed=false
option=startup-state status=not-applicable observed=false (now threaded into the live controls launch, FR-005; not driven in headless CI)
option=startup-position status=not-applicable observed=false
option=backend status=not-applicable observed=false (no new OpenGL/SkiaSharp surface is opened)
