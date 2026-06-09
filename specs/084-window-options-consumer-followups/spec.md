# Feature Specification: Window Startup Options (Fullscreen + Windowed-Fullscreen Default) & Invoice1/Spread1 Consumer Friction Follow-ups

**Feature Branch**: `084-window-options-consumer-followups`
**Created**: 2026-06-08
**Status**: Draft
**Input**: User description: "create specs from the feedback from sibling repos invoice1 and spread1. also add fullscreen and windowed fullscreen as window option. make windowed fullscreen the default."

## Context & Triage *(informative)*

Two consumers built non-game UIs in generated `FS.Skia.UI` projects — an invoice
builder (`Invoice1`) and a spreadsheet editor (`Spread1`) — and captured per-phase
feedback under their `specs/001-*/feedback/` directories. Both projects were
generated from the current package/template line, so every finding below is against
the **merged state**, not a stale build. The user additionally asked for two new
window startup options — **fullscreen** and **windowed fullscreen** — with
**windowed fullscreen as the new default**.

The window-options ask is not separate from the feedback: it is the strongest
framework-facing finding in the corpus. `Invoice1` reported that the generated app
parses `--window-startup fullscreen` into a `ViewerWindowBehaviorRequest`, feeds it
to the *diagnostic report*, then launches with `Viewer.runApp` — which **ignores
the behavior** — so the flag changes only the report, never the real window. The
framework already ships `Viewer.runAppWithWindowBehavior` and a
`ViewerWindowStartupState.Fullscreen` case, but the generated launcher never calls
the former, and `validateBehavior` actively classifies `Fullscreen` as
`UnsupportedOption` ("not yet supported by the viewer host") even though
`applyWindowBehaviorToOptions` already maps it to `WindowState.Fullscreen`. So
fullscreen *reads as a host limitation when it is really an unwired, mislabeled
launch path*.

Per the house pattern (one consolidated "consumer friction follow-ups" feature per
cohort, e.g. 060–063) and the single-feature rule, this is **one** feature
consolidating the Invoice1 + Spread1 feedback **and** the new window options — not
one spec per item. Scope was confirmed with the requester as the **full
consumer-feedback-followups bundle**.

Each finding is triaged against the current source:

| # | Sev | Finding | Source / current-state evidence |
|---|-----|---------|----------------------------------|
| WIN-1 | major (USER) | No **windowed-fullscreen** (borderless, work-area) startup state exists; `Fullscreen` is classified **`UnsupportedOption`** in `validateBehavior` despite `applyWindowBehaviorToOptions` already mapping it to `WindowState.Fullscreen`; the default startup state is `Normal`. The user wants both states supported and **windowed fullscreen as the default**. | `src/SkiaViewer/SkiaViewer.fs` `ViewerWindowStartupState` = `Normal\|Maximized\|Minimized\|Fullscreen` (no windowed-fullscreen); `validateBehavior` returns `UnsupportedOption` for `Fullscreen`; `defaultWindowBehavior` StartupState = `Normal`. |
| WIN-2 | major | Generated `Program.fs` parses window flags into a `ViewerWindowBehaviorRequest` and uses it only for the diagnostic report, then launches with `Viewer.runApp viewerOptions generatedHost`, ignoring the request. The framework ships `runAppWithWindowBehavior` but the generated app never calls it. The durable `GovernanceTests.fs` asserts the literal `Viewer.runApp viewerOptions generatedHost`, so wiring must keep that literal reachable rather than a straight swap. | `Invoice1/.../feedback/implement-2026-06-08.md` §Process friction #4. Confirmed against `src/SkiaViewer/SkiaViewer.fs` (`runAppWithWindowBehavior` exists, ~line 2118). |
| EV-1 | major | `docs/evidence-formats.md` (shipped via the template) has **drifted behind the shipped engine**: it documents only `interactive-visible-window.md` + `window-state-diagnostics.md`, but the engine hard-requires five more (`close-reason-separation.md`, `window-options.md`, `real-image-evidence.md`, `generated-validation.md`, feature-local `evidence-audit.md`) plus per-file token sub-checks. The doc is stamped "regenerate with RefreshSurfaceBaselines" but is stale, so consumers recover the contract only by decompiling `FS.Skia.UI.Build.dll` or copying a sibling. | Both repos' `implement` feedback (Spread1 marks this **major**). |
| EV-2 | major | The `EvidenceAudit` summary surfaces only `total-blockers=N` / `unaccepted-synthetic-tasks=0`; the actionable per-blocker reasons live only in `*-hits.json` sidecars never echoed to stdout, and `base_ref` stays `null` even when the default branch is a strict ancestor of HEAD — so the (empty) diff-scan is mistaken for the source of blockers. | `Spread1/.../feedback/implement-2026-06-08.md` §Process friction #1. |
| SM-1 | minor | `docs/scaffold-map.md` describes the durable/replaceable split with `src/Product/**` paths, but generated trees use `src/<ProjectName>/**` (`src/Invoice1`, `src/Spread1`). The map must be reconciled file-by-file before it can be trusted. | Both repos' `plan` feedback. |
| SM-2 | minor | "durable" vs "re-pointable" is ambiguous: `LayoutEvidence.fs`/`EvidenceCommands.fs` are listed as durable yet read scaffold model fields and **must** be edited (re-pointed) on a model swap; "durable" reads as "do not touch." The HUD→headers / gameplay→grid remap onto a non-game UI is also non-obvious. | `Spread1/.../feedback/plan-2026-06-08.md`. |
| DEV-1 | minor | `./fake.sh build -t Dev` only writes a readiness log; it does **not** compile. The first real compile is in `Test`/`Verify`. "Build with `Dev`" guidance produces a false-green when iterating. | `Invoice1/.../feedback/implement-2026-06-08.md` §1. |
| VFY-1 | minor | `Verify` embeds `EvidenceGraph` + `EvidenceAudit` **before** the tests and the audit hard-blocks until every task is `[X]`, so `Verify` cannot produce a green test run mid-implementation. The documented order (`Dev → Verify → EvidenceGraph → EvidenceAudit`) implies the audit is a separate, later step. | `Invoice1/.../feedback/implement-2026-06-08.md` §2. |
| ANL-1 | minor | `/speckit-analyze` mandates `./fake.sh build -t SymbolCrossCheck` ("do not hand-derive"), but generated projects do not ship that target; the command fails with `Unknown … target: SymbolCrossCheck`, forcing a non-authoritative manual pass. The skill should probe target availability and degrade gracefully, as `EvidenceGraph` already resolves the feature from `.specify/feature.json`. | `Invoice1/.../feedback/analyze-2026-06-08.md`. |
| GEN-1 | n/a (generalizable) | Reusable shapes flagged by consumers, **not** shipped here: a parse→AST→topo-evaluate→propagate-error recipe (cookbook candidate for `fsharp-graph-algorithms`); a `KeyCommand` note unifying raw/normalized key paths (candidate for `fs-skia-keyboard-input`); the round-half-up invariant money formatter (single use, below the recurrence bar). | Spread1/Invoice1 `implement` feedback §Generalizable code. |
| SKILL-1 | n/a (skill gap) | A "scaffold-model swap" procedure skill (rewrite Model/View/BehaviorTests; re-point durable Program/EvidenceCommands/LayoutEvidence/WindowOptions; preserve must-survive tokens; leave GovernanceTests untouched; post-swap grep verification) was wanted in both repos at plan **and** implement. | Both repos. Recorded as a follow-up candidate; not a deliverable of this feature. |

**Change classification.** **Escalated / `maintainer-verify` (Tier 1).** This change
adds a public union case to `src/SkiaViewer/SkiaViewer.fsi` (`ViewerWindowStartupState`)
and changes a default value, edits `template/**` (generated `Program.fs` launch wiring
and the regenerated `docs/evidence-formats.md` / `docs/scaffold-map.md`), and touches
governance/build surfaces (`FS.Skia.UI.Build` audit output, `Dev`/`Verify` semantics or
docs, the `speckit-analyze` skill). `Route` is expected to escalate it; run the
serialized six-target order and recapture surface baselines.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Window starts in windowed fullscreen, and the chosen state actually takes effect (Priority: P1)

A developer runs a freshly generated `FS.Skia.UI` app with no flags. The window
opens **borderless, covering the monitor work area** (windowed fullscreen) — the new
default. The same developer passes a window-startup option to choose plain windowed,
maximized, exclusive fullscreen, or windowed fullscreen, and the **live window**
adopts that state — not merely the diagnostic report. No supported state is reported
as "unsupported."

**Independent test**: From a generated project, launch with no flags and capture
real visible-window evidence showing a borderless work-area-covering window; then
launch once per supported startup state and confirm each produces the corresponding
real window state, with `validateWindowLaunchBehavior` reporting every supported
state as honored (none `UnsupportedOption`).

### User Story 2 - The readiness contract is discoverable, and audit blockers are legible (Priority: P1)

A consumer hits an `EvidenceAudit` failure. From the **shipped docs** they can read
the complete window-visibility readiness contract — every required file and its
required tokens/fields — without decompiling the build DLL or copying a sibling
project. The audit's own stdout names each blocker, its one-line reason, and the
hit-file path, and reports the diff-scan base (or explains why it is absent) so an
empty diff-scan is not mistaken for the cause.

**Independent test**: In a generated project, read `docs/evidence-formats.md` and
confirm all seven window-visibility files and their token sets are listed; trigger a
deliberate readiness gap and confirm the audit stdout enumerates the blocker(s) with
reason + file path and a non-misleading base-ref line, with no need to open the JSON
sidecars.

### User Story 3 - The scaffold-map is trustworthy for a model swap (Priority: P2)

A consumer planning a scaffold-model swap reads `docs/scaffold-map.md`. Its path
examples match the generated tree verbatim (project-named, not `src/Product/**`), and
each durable file is unambiguously marked either model-agnostic or "keep the file +
its scanned tokens but re-point its model references," with a worked example mapping
the layout-evidence regions onto a non-game UI.

**Independent test**: Diff the scaffold-map's cited paths against a generated
project's real tree — they match with zero manual reconciliation; confirm
`LayoutEvidence.fs`/`EvidenceCommands.fs` are marked re-pointable (not "do not touch")
and a non-game remap example is present.

### User Story 4 - Honest build signals and graceful analyze (Priority: P2)

A consumer iterating mid-implementation gets truthful build feedback: `Dev` either
compiles or is clearly labeled log-only with the first-real-compile path named; the
docs state that `Verify` embeds the merge-gate audit and name the mid-implementation
green-test path; and `/speckit-analyze` completes in a project that lacks the
`SymbolCrossCheck` target without an unhandled failure.

**Independent test**: In a generated project, confirm the guidance/behavior for `Dev`
removes the false-green, the `Verify`/audit relationship is documented with a stated
mid-implementation test path, and `/speckit-analyze` skips-with-notice (or uses a
documented fallback) when `SymbolCrossCheck` is absent.

## Requirements *(mandatory)*

### Functional Requirements

**Window startup options (WIN-1, WIN-2)**

- **FR-001**: The framework MUST provide a **windowed-fullscreen** startup state — a
  borderless window covering the monitor work area, with no title bar or resize chrome
  and no exclusive-mode resolution change — in addition to the existing normal,
  maximized, minimized, and (exclusive) fullscreen states.
- **FR-002**: The framework MUST treat both **fullscreen** and **windowed fullscreen**
  as **supported** startup states for visible-interactive-launch validation; neither
  may be reported as `UnsupportedOption`, and each MUST produce a launch-behavior
  result classified as honored.
- **FR-003**: The default startup behavior MUST be **windowed fullscreen** (replacing
  the current `Normal` default), so a generated app launched with no window flags opens
  in windowed fullscreen.
- **FR-004**: A generated app MUST apply its parsed window-behavior request to the
  **actual launched window**, so any window-startup option changes the real window —
  not only the diagnostic report. When a window-behavior flag is present, the launch
  MUST honor it.
- **FR-005**: The generated-project source/governance scan MUST permit the
  behavior-wired launch path while keeping the must-survive launch literal reachable
  (resolving the `GovernanceTests.fs` assertion of `Viewer.runApp viewerOptions
  generatedHost`), e.g. via a guarded behavior branch rather than removing the literal.
- **FR-006**: A user MUST be able to select any supported startup state — including
  explicitly choosing plain **windowed (normal)** to override the windowed-fullscreen
  default — through the existing window-option flag surface.

**Evidence contract & audit legibility (EV-1, EV-2)**

- **FR-007**: The shipped `docs/evidence-formats.md` MUST document the **complete**
  window-visibility readiness contract the engine enforces — every required file
  (`interactive-visible-window.md`, `window-state-diagnostics.md`,
  `close-reason-separation.md`, `window-options.md`, `real-image-evidence.md`,
  `generated-validation.md`, and the feature-local `evidence-audit.md`) and each file's
  required tokens/fields — and MUST
  be regenerated from its single source so it cannot silently drift behind the engine
  again.
- **FR-008**: On a failing `EvidenceAudit`, the audit's stdout summary MUST surface,
  per blocker, a one-line reason and the originating hit-file path (not only an
  aggregate `total-blockers=N`), so the actionable detail is recoverable without opening
  the JSON sidecars or decompiling the build assembly.
- **FR-009**: The audit MUST resolve the diff-scan base ref when the default branch is a
  strict ancestor of HEAD, and when no base can be resolved MUST report that explicitly
  so an empty diff-scan is not mistaken for the source of blockers.

**Scaffold-map clarity (SM-1, SM-2)**

- **FR-010**: `docs/scaffold-map.md` MUST refer to product source/test paths by the
  generated project name or an explicit `<ProjectName>`/`<ProductDir>` placeholder, so
  its path examples match a generated project's tree verbatim.
- **FR-011**: `docs/scaffold-map.md` MUST distinguish durable-and-model-agnostic files
  from durable-but-must-re-point files, stating that "durable" means *keep the file and
  its scanned evidence tokens while re-pointing model-field references*, and MUST
  include a worked example mapping the layout-evidence regions (HUD/gameplay) onto a
  non-game UI.

**Honest build signals & graceful analyze (DEV-1, VFY-1, ANL-1)**

- **FR-012**: The `Dev` target MUST remove the false-green: either it performs a real
  compile, or its log output and quickstart guidance MUST state that `Dev` is log-only
  and that the first real compile is `-t Test`.
- **FR-013**: Documentation MUST state that `Verify` embeds the merge-gate audit
  (`EvidenceGraph` + `EvidenceAudit`), which hard-blocks until the feature is complete,
  and MUST name the mid-implementation path (`-t Test`) for a green test run before
  completion.
- **FR-014**: `/speckit-analyze` MUST degrade gracefully when a mandated target (e.g.
  `SymbolCrossCheck`) is absent from the generated project's build surface — probing
  available targets and skipping-with-documented-notice rather than failing the
  invocation — mirroring how the evidence-graph step resolves the feature from
  `.specify/feature.json`.

> **Interacting / conflicting requirements.**
> - **FR-003 (windowed-fullscreen default) vs FR-006 (explicit selection):** an explicit
>   window-startup option always overrides the default; the windowed-fullscreen default
>   applies only when no window-startup flag is supplied.
> - **FR-003 (windowed-fullscreen default) vs render-only evidence honesty:** the default
>   governs *interactive* launch only. In a headless / unsupported environment the
>   render-only evidence mode is unaffected and MUST continue to report the environment
>   honestly (no false claim of a visible windowed-fullscreen window).
> - **Fullscreen vs windowed fullscreen:** "fullscreen" is the existing exclusive
>   `WindowState.Fullscreen`; "windowed fullscreen" is borderless coverage of the work
>   area. They are distinct selectable states, not aliases.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identities* change. Package **contents** change in
  the SkiaViewer package (new public `ViewerWindowStartupState` case + changed default
  behavior value) and in `FS.Skia.UI.Build` (audit summary output). Generated **package
  consumers** change via the template's `Program.fs` launch wiring and regenerated docs.
  All packable projects — including `FS.Skia.UI.Build` — bump and pack on merge per the
  house rule. No legacy Charts migration is involved.
- **Public contract impact**: Yes. `src/SkiaViewer/SkiaViewer.fsi` gains a
  `WindowedFullscreen` union case on `ViewerWindowStartupState`, and the
  `defaultWindowBehavior` value changes (value, not signature). Per-package **and**
  cross-package surface baselines move and MUST be recaptured
  (`RefreshSurfaceBaselines` / `PerPackageSurface`). The template's `Program.fs`
  contract and the documented window-option surface change.
- **State workflow impact**: The window-behavior request now flows into the **actual**
  launch path (`runAppWithWindowBehavior`) for generated apps; the default behavior value
  changes; `validateBehavior` reclassifies fullscreen and windowed fullscreen from
  unsupported to honored. The existing `ApplyWindowOptions` effect and viewer
  init/update remain the carrier — no new effect type is required.
- **Layout/rendering impact**: Windowed fullscreen alters the real interactive window
  geometry/state (borderless, work-area). The render-only evidence path and Vulkan/Skia
  rendering pipeline are unchanged. Unsupported-environment diagnostics update so
  fullscreen / windowed fullscreen are no longer labeled host-unsupported on a capable
  host.
- **Evidence obligations**: Real interactive **visible-window** evidence for the new
  windowed-fullscreen default and for each supported startup state; the full
  window-visibility readiness file set (the regenerated seven-file contract) for this
  feature; `generated-validation.md`, `close-reason-separation.md`, and a
  `window-options.md` reflecting the new states; decodable image/screenshot evidence of
  a windowed-fullscreen launch. Audit-legibility (FR-008/009) verified from real audit
  stdout on a deliberately-failing run.
- **Unsupported scope**: No new platforms or rendering backends. Multi-monitor
  monitor-selection for fullscreen is out of scope (default monitor only). No
  resolution/refresh-rate switching for exclusive fullscreen beyond the existing
  `WindowState.Fullscreen`. The GEN-1 cookbook helpers and the SKILL-1 scaffold-swap
  procedure skill are **out of scope** here (recorded as follow-up candidates). Release
  and distribution flows are unchanged.
- **Build-target impact**: `Dev` (FR-012) and `Verify` (FR-013) guidance/behavior;
  `EvidenceAudit` summary output (FR-008/009); `TemplateCheck` / `TemplateDrift` (the
  template `Program.fs` and regenerated docs change); `GeneratedGuidanceCheck`; surface
  baselines via `RefreshSurfaceBaselines`; `EvidenceGraph` + `EvidenceAudit` run as part
  of the escalated order. The `SymbolCrossCheck` handling (FR-014) is a `speckit-analyze`
  skill change, not a new framework target (generated projects do not ship that target).

## Success Criteria *(mandatory)*

- **SC-001**: A freshly generated project launched with no flags opens in windowed
  fullscreen (borderless, covering the work area), shown by real visible-window
  evidence.
- **SC-002**: Each supported startup state (windowed/normal, maximized, exclusive
  fullscreen, windowed fullscreen) selected via the window option produces the matching
  real window state in launch evidence, and none is reported as "unsupported."
- **SC-003**: A consumer can enumerate the complete window-visibility readiness contract
  — every required file and its required tokens — from the shipped docs alone, with no
  decompilation and no sibling-project copy.
- **SC-004**: On a failing `EvidenceAudit`, an operator can identify every blocker, its
  reason, and its hit-file path from the audit's own stdout, without opening any JSON
  sidecar, and the diff-scan base ref is either resolved or explicitly reported as
  absent.
- **SC-005**: The scaffold-map's path examples match a generated project's real tree
  verbatim (zero manual reconciliation), and every durable file is unambiguously marked
  model-agnostic or must-re-point.
- **SC-006**: A consumer can obtain a green test run mid-implementation via a documented
  path and understands that `Dev` alone does not prove compilation.
- **SC-007**: `/speckit-analyze` completes its symbol cross-check step in a generated
  project lacking the `SymbolCrossCheck` target without an unhandled failure (graceful
  skip-with-notice or documented fallback).

## Edge Cases

- **Headless / unsupported environment with the windowed-fullscreen default**: the app
  MUST degrade to honest render-only / evidence behavior and MUST NOT crash or claim a
  visible window that does not exist.
- **Conflicting window flags** (e.g. both a fullscreen and a maximized flag): the
  resolution MUST be deterministic and documented (the explicit, last-specified
  window-startup selection wins).
- **`base_ref` genuinely unresolvable** (e.g. a brand-new repo with no default-branch
  ancestor): the audit reports the absence explicitly rather than emitting a silent
  empty diff-scan.
- **Exclusive fullscreen on a host that cannot grant it**: falls back with an honest
  diagnostic rather than a false "honored" claim — windowed fullscreen, being a plain
  borderless window, remains available as the capable default.

## Assumptions

- "Windowed fullscreen" means a **borderless window covering the monitor work area** (no
  exclusive mode, no resolution change); "fullscreen" is the existing exclusive
  `WindowState.Fullscreen`. This matches the common "borderless windowed" convention.
- Window-startup selection continues to use the existing `--window-startup` flag family,
  extended with a windowed-fullscreen value; the default applies when no flag is given.
- Only the default monitor is targeted; no monitor-selection UI is introduced.
- `docs/evidence-formats.md` and `docs/scaffold-map.md` are template/generated docs
  produced from a single regeneration source; "fixing" them means correcting that source
  and regenerating, not hand-editing the generated copies.
- GEN-1 (generalizable code) and SKILL-1 (scaffold-swap skill) are recorded as
  follow-up candidates and are **not** delivered by this feature, to keep scope bounded.
- All findings are validated against the current merged source; the cited consumer
  feedback files are snapshotted under this feature's `feedback/` directory for
  provenance.

## Key Entities

- **Window startup state**: the requested initial window mode — windowed (normal),
  maximized, minimized, exclusive fullscreen, or **windowed fullscreen** (new). Carried
  by the existing window-behavior request; windowed fullscreen is the new default.
- **Window-visibility readiness contract**: the set of evidence files (seven) plus their
  required tokens that the merge-gate audit enforces for a visible-window feature; this
  feature makes the set fully discoverable from shipped docs.
- **Audit blocker record**: a single failing readiness/diff-scan finding with a reason
  and an originating hit-file path; this feature makes each one legible on stdout.
- **Scaffold-map entry**: a generated-project file classified durable (model-agnostic or
  must-re-point) or replaceable; this feature makes paths project-named and the
  durable-vs-re-point distinction explicit.
