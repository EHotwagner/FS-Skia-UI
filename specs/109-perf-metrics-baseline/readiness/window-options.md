# Window options — applicability (feature 109, T001/T028)

Feature 109 creates no new window, so every option is not-applicable (none is silently ignored — each
is recorded).

option=resize status=not-applicable observed=false
option=maximize status=not-applicable observed=false
option=startup-state status=not-applicable observed=false
option=startup-position status=not-applicable observed=false
option=backend status=not-applicable observed=false (no new Vulkan/SkiaSharp surface is opened)
