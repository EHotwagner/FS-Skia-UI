# Phase 1 Data Model: Window Startup Options & Audit Legibility

Entities are the values the change adds or alters. Most are extensions of
existing types, not new types — the change is deliberately additive.

## Window startup state (extended)

`ViewerWindowStartupState` (`src/SkiaViewer/SkiaViewer.fsi:45`, `.fs:44`) — gains
one union case.

| Case | Meaning | Window mechanics | Validation status |
|------|---------|------------------|-------------------|
| `Normal` | Plain windowed | `WindowState.Normal` | Honored |
| `Maximized` | Maximized | `WindowState.Maximized` | Honored |
| `Minimized` | Minimized | `WindowState.Minimized` | UnsupportedOption (not a visible-interactive-launch state) |
| `Fullscreen` | Exclusive fullscreen | `WindowState.Fullscreen` | **Honored** (was UnsupportedOption) |
| `WindowedFullscreen` **(NEW)** | Borderless coverage of the monitor work area | `WindowBorder.Hidden` + work-area Position/Size + `WindowState.Normal` | **Honored** |

- **Default** is now `WindowedFullscreen` (was `Normal`), carried by
  `defaultWindowBehavior.StartupState` (`SkiaViewer.fs:550`).
- **Distinctness invariant.** `Fullscreen` (exclusive) and `WindowedFullscreen`
  (borderless work-area) are distinct selectable states, never aliases.

## Window-behavior request (unchanged shape, new carried value)

`ViewerWindowBehaviorRequest` (`SkiaViewer.fsi:71`) — record is structurally
unchanged; its `StartupState` field can now carry `WindowedFullscreen`, and
`defaultWindowBehavior` (`SkiaViewer.fsi:534`) returns it by default. Flows
through `init` → `ApplyWindowOptions` effect → interpreter exactly as today; **no
new effect type** is introduced (`ApplyWindowOptions of ViewerWindowBehaviorRequest`
remains the carrier).

## Window option result (unchanged shape, new classification)

`ViewerWindowOptionResult` / `ViewerWindowOptionStatus` (`SkiaViewer.fsi:64,79`) —
unchanged. The `startup-state` result for `Fullscreen` and `WindowedFullscreen`
now reports `Honored` (with an honored message) instead of `UnsupportedOption`.
`validateWindowBehavior` and `validateWindowLaunchBehavior` both reflect this.

## Template window-flag value (extended)

`WindowOptions.fs` (`template/base/src/Product/`) parsing surface — the
`--window-startup` flag value set extends from `{normal, maximized, minimized,
fullscreen}` to add `windowed-fullscreen`; the default value (no flag) changes
from `"normal"` to `"windowed-fullscreen"`. The manual status mapping reclassifies
`fullscreen`/`windowed-fullscreen` to `honored`.

- **Conflict resolution invariant (Edge case).** When multiple/conflicting window-
  startup flags are supplied, the **explicit, last-specified** selection wins; this
  resolution is deterministic and documented.

## Evidence-format schema entry (extended single source)

`EvidenceFormatSchema` `WindowVisibility` class
(`build/Governance/Evidence/EvidenceFormatSchema.fs`) — the rendered set of
window-visibility files extends from 2 to the full engine-required **7**, each with
its required tokens. The rendered list MUST equal `Scans.requiredFiles`.

| File | Required tokens (source) |
|------|--------------------------|
| `interactive-visible-window.md` | `status, mode, window-visible, accessible-window, first-frame-presented, self-closed-for-evidence` (`interactiveVisibleWindowKeys`) |
| `window-state-diagnostics.md` | `diagnostic-class ∈ {environment-session, window-visibility, app-lifecycle, product-defect}` + native facts (`native-handle, visible, focusable, renderable-surface, input-devices`) |
| `window-options.md` | `option=` rows for `resize, maximize, startup-state, startup-position, backend` |
| `close-reason-separation.md` | presence (close-reason vs evidence-close separation) |
| `real-image-evidence.md` | presence (decodable image/screenshot evidence) |
| `generated-validation.md` | presence (generated-project validation record) |
| `evidence-audit.md` | presence (feature-local audit record) |

## Audit blocker record (existing shape, newly surfaced on stdout)

`ScanHit` (`build/Governance/Evidence/StatusRegion.fs`) — fields already present;
this change surfaces them on stdout (was JSON-sidecar-only).

| Field | Role on stdout |
|-------|----------------|
| `Path` | originating hit-file path (printed per blocker) |
| `Reason` | one-line reason (printed per blocker) |
| `Required` / `MissingTerms` | full-required-set / absent-from-file detail |
| `ValidationArea` | groups the blocker under its area |

## Diff-scan result (existing shape, populated field)

`DiffScanResult` (`build/Governance/Evidence/DiffScan.fs:16`) — `BaseRef: string
option`, currently hardcoded `None` at construction (`:190`). Populated from the
caller's already-resolved merge-base.

| State | `BaseRef` value | Stdout |
|-------|-----------------|--------|
| Default branch is a strict ancestor of HEAD | `Some "<merge-base sha or ref>"` | reports the resolved base |
| No base resolvable (brand-new repo) | `None` | explicit "base_ref: none — diff-scan empty by absence, not by clean diff" |

## Scaffold-map entry (documentation classification)

A `scaffold-map.md` entry classifies a generated-project file as one of:

| Class | Meaning | Examples |
|-------|---------|----------|
| Replaceable | rewrite on a model swap | `<ProjectName>/Model.fs`, `View.fs`, `BehaviorTests.fs` |
| Durable — model-agnostic | keep, do not touch | `GovernanceTests.fs`, `Program.fs` host wiring |
| Durable — must re-point **(clarified class)** | keep the file + its scanned evidence tokens, re-point model-field references | `LayoutEvidence.fs`, `EvidenceCommands.fs`, `WindowOptions.fs` |

All paths use the `<ProjectName>`/`<ProductDir>` placeholder so they match a
generated tree verbatim.
