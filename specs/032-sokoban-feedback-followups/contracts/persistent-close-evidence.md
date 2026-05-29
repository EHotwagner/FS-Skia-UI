# Contract: Persistent Interactive Close Evidence

## Scope

Generated graphical apps must be able to prove a real persistent interactive-window launch and clean close path in automated or agent-run evidence.

## Required Behavior

- Default generated launch remains the real persistent viewer-backed host.
- Evidence collection may use an explicit command, but it must launch the same persistent host behavior rather than a bounded scene-only substitute.
- App-level close intent remains product-owned state or message data.
- The generated host translates the accepted close request into a real viewer/window close outcome.
- Bounded smoke, scene evidence, screenshot evidence, and deterministic render evidence remain diagnostic and must not be relabeled as persistent launch proof.

## Required Evidence Fields

- command
- mode
- window-opened fact
- first-frame confirmation
- input-dispatch status
- close request source
- close reason
- exit path
- elapsed time
- status
- failure classification
- log/artifact path

## Pass Conditions

- Evidence reports a real interactive-window launch.
- A first frame is presented before close.
- The close path is app-requested, user-confirmed, or evidence-requested through the documented generated app workflow.
- The session exits cleanly without manual window closing.
- Evidence completes within the target time budget or reports a precise unsupported/failure stage.
