# Window Options (099, R4)

status=not-applicable

## Option rows

- option=resize — resize=not-applicable (no window created); status=not-applicable observed=false
- option=maximize — maximize=not-applicable (no window created); status=not-applicable observed=false
- option=startup-state — startup-state=not-applicable (no window created); status=not-applicable observed=false
- option=startup-position — startup-position=not-applicable (no window created); status=not-applicable observed=false
- option=backend — backend=not-applicable (no windowed backend selected; the proofs run through the
  pure clock core and the off-window `RetainedRender.advance`/`step` seam); status=not-applicable observed=false

No unsupported window setting is requested, and no option failure is hidden under another diagnostic
class (an unsupported option would diagnose under diagnostic-class=window-options). This feature opens
no window, so there are no window options to honor or degrade.
