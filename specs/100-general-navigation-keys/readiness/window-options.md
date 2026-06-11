# Window Options (100, R5)

status=not-applicable

## Option rows

- option=resize — resize=not-applicable (no window created); status=not-applicable observed=false
- option=maximize — maximize=not-applicable (no window created); status=not-applicable observed=false
- option=startup-state — startup-state=not-applicable (no window created); status=not-applicable observed=false
- option=startup-position — startup-position=not-applicable (no window created); status=not-applicable observed=false
- option=backend — backend=not-applicable (no windowed backend selected; the proofs run through the
  pure `Focus.route` classifier and the off-window `routeFocusedKey` resolver seam); status=not-applicable observed=false

No unsupported window setting is requested, and no option failure is hidden under another diagnostic
class (an unsupported option would diagnose under diagnostic-class=window-options). This feature opens
no window, so there are no window options to honor or degrade.
