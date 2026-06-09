---
phase: implement
date: 2026-06-08
severity: minor
---

## Process friction

Two real frictions surfaced during the scaffold-model swap:

1. **`Dev` does not compile.** `build.fsx`'s `Dev` target is `writeLog "Dev"` — it
   only writes a readiness log and never invokes the F# compiler. The first *actual*
   compile happens inside the `Test` target's `dotnet test`. So `./fake.sh build -t Dev`
   reports "Dev completed" on code that does not compile, which is misleading when you
   are iterating on a swap — you only learn about compile errors one target later. A
   one-line note in the quickstart ("Dev logs only; run `-t Test` for the first real
   compile") or a real `dotnet build` in `Dev` would remove the false-green.

2. **The merge-gate audit is wired *inside* `Verify`.** `Verify` runs
   `EvidenceGraph` + `EvidenceAudit` *before* the Expecto tests, and the audit
   hard-blocks (exit 2) until every task is `[X]` and all blocking window-visibility
   readiness files exist. That means you cannot use `Verify` to get a green test run
   mid-implementation — you must use `-t Test` directly until the feature is finished.
   The canonical order in the docs (`Dev → Verify → EvidenceGraph → EvidenceAudit`)
   implies the audit is a *separate, later* step, but `Verify` already embeds it.

3. **Window-visibility readiness contract is under-documented.** `docs/evidence-formats.md`
   only specifies `interactive-visible-window.md` and `window-state-diagnostics.md`,
   but the audit also requires `close-reason-separation.md`, `window-options.md`,
   `generated-validation.md`, and a feature-local `evidence-audit.md`, plus specific
   tokens (`exact-package-match`, `generated-tests-ran`, `authoritative`,
   `failure-class`; the five `observed:`/`unsupported` native facts). These were
   discovered only by reading the audit's `*-hits.json` after a failure, then
   iterating. Listing the full window-visibility file set + token requirements in
   `evidence-formats.md` would let an author satisfy the contract before triggering a
   gate failure (the doc even claims to be the single source for exactly this).

4. **Parsed window behavior is reported but never applied to the launch.** The
   generated `Program.fs` default branch parses `--window-resize/-maximize/-startup/
   -position/-backend` into a `ViewerWindowBehaviorRequest` and feeds it to
   `manualWindowOptionResults` / `Viewer.validateWindowLaunchBehavior` for the
   diagnostic report — but then launches with `Viewer.runApp viewerOptions
   generatedHost`, which ignores the behavior. The framework ships
   `Viewer.runAppWithWindowBehavior options behavior host` (and
   `ViewerWindowStartupState.Fullscreen`) for exactly this, but the generated app
   never calls it. Net effect: `--window-startup fullscreen` (or maximized, or a
   backend preference) changes only the *report*, not the actual window. A user asking
   to "run the app fullscreen" therefore has no built-in path — and the app's own
   `manualWindowOptionResults` even classifies fullscreen as `unsupported`, which
   reads as a host limitation when it's really an unwired launch path. Worse, the
   durable `GovernanceTests.fs` asserts the literal `Viewer.runApp viewerOptions
   generatedHost` in the default branch, so wiring the behavior through
   `runAppWithWindowBehavior` requires keeping that literal reachable (e.g. a guarded
   `--fullscreen`/behavior branch) rather than a straight swap. The generated launcher
   should apply the parsed `windowBehaviorRequest` (via `runAppWithWindowBehavior`)
   when any window-behavior flag is present, and the governance scan should permit
   that wiring.

## Generalizable code

none shipped to the framework this phase — the work is invoice1-internal product
code (`Model.fs`/`View.fs` rewrite, `LayoutEvidence.fs`/`EvidenceCommands.fs`
re-point). The `round2` (half-up away-from-zero) + invariant two-decimal money
formatter pair is a plausible future SkillSupport helper if a second money-bearing
demo appears, but a single use does not clear the next-recurrence bar.

## Skill gaps

The "scaffold-model swap" procedure-skill flagged in the plan-phase feedback would
have paid off here: the swap's hardest parts were mechanical and repeatable — the
`ValidationState`/`RenderScene` cross-module name-collision (`open`-order trap from
`fs-skia-scene` / `fs-skia-keyboard-input`, but for `FS.Skia.UI.Controls` types), the
must-survive-token preservation, and the window-visibility readiness file set. A
checklist skill with a post-swap `grep` verification step and the full readiness-file
roster would have front-loaded all three.

## Research links

research blocked — offline implementation session; no external lookups were required.
All inputs (spec, plan, contracts, `.fsi` surfaces, skills, audit hit-files) were
available in-repo. The host unexpectedly *did* support live rendering, so visual
evidence (`--screenshot-evidence` → `LiveViewerWindow`, `--image-evidence` → decodable
73 KB PNG) was real rather than unsupported-classified.
