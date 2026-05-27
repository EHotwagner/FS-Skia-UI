# Close Reason Separation Evidence

status=ok
mode=interactive-window
close-reason=UserClose
user-close-observed=true
app-close-observed=false
evidence-close-observed=false
self-closed-for-evidence=false
source-log=specs/019-fix-window-visibility/readiness/logs/t024-supported-host-persistent-attempt.txt

## Evidence Paths

- Interactive generated default command:
  `readiness/logs/t024-supported-host-persistent-attempt.txt`
- Interactive lifecycle close transitions:
  `readiness/logs/t017-skiaviewer-interactive-tests.txt`
- Generated host close/input transitions:
  `readiness/logs/t021-generated-host-interpreter-tests.txt`
- Evidence-mode close separation:
  `readiness/logs/t021-generated-host-interpreter-tests.txt`

## Separation Rules Exercised

The interactive generated command reports `self-closed-for-evidence=false`.
Focused tests prove first frame does not close the window, `EvidenceClose` is
reported only by explicit evidence APIs, app close is not user close, host close
is not user close, and failure close emits diagnostics rather than evidence or
user-close compatibility fields.

## Synthetic Disclosure

No synthetic close-transition fixture was used for this readiness record.
