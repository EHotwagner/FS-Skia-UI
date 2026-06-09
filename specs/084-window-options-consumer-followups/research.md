# Phase 0 Research: Window Startup Options & Invoice1/Spread1 Consumer Follow-ups

This document resolves the open technical questions for spec 084. Every finding
in the spec was re-validated against the current merged source; where the
investigation refined a finding, the refinement is recorded as a decision below.

## R1 — Windowed-fullscreen window mechanics (WIN-1)

**Decision.** Model windowed fullscreen as a **borderless window covering the
primary monitor work area**: `WindowBorder.Hidden` + position at the monitor
work-area origin + size set to the monitor work-area extent, with
`WindowState.Normal` (no exclusive-mode resolution change). "Fullscreen" remains
the existing exclusive `WindowState.Fullscreen`.

**Rationale.** `applyWindowBehaviorToOptions` (`src/SkiaViewer/SkiaViewer.fs:973`)
already maps each `ViewerWindowStartupState` to a Silk.NET `WindowOptions`. It
sets `WindowBorder` (Resizable/Fixed) and `WindowState`, but **no monitor / work-
area / borderless concept is referenced anywhere in the viewer today** (confirmed:
no `IMonitor`, `WorkArea`, `VideoMode`, or `WindowBorder.Hidden` usage). Borderless
windowed coverage is the industry-standard "borderless windowed" convention and
needs only properties Silk.NET already exposes (`WindowBorder.Hidden`, `Position`,
`Size`) plus a monitor bounds query, so it avoids exclusive-mode fragility while
staying inside the existing `WindowOptions` carrier.

**Alternatives considered.**
- *`WindowState.Maximized` + hidden border.* Rejected as the canonical
  representation: maximize semantics vary by platform/compositor and may retain a
  title region; explicit work-area sizing is deterministic and testable.
- *Reusing `WindowState.Fullscreen` for both.* Rejected — the spec explicitly
  requires fullscreen and windowed fullscreen to be **distinct selectable states,
  not aliases** (spec "Fullscreen vs windowed fullscreen").

**Open implementation note (resolved to bounded approach).** Monitor work-area
bounds are read at window setup via the Silk.NET monitor surface
(`window.Monitor` / primary monitor `Bounds`). Only the **default monitor** is
targeted (spec Unsupported scope). When work-area bounds cannot be resolved
(headless / no display), the state degrades to the honest render-only path (R6)
rather than fabricating a geometry.

## R2 — Validation reclassification (WIN-1, FR-002)

**Decision.** In `WindowBehaviorValidation` (`SkiaViewer.fs:554`), reclassify
`Fullscreen` from `UnsupportedOption` to `Honored` and add a `WindowedFullscreen`
arm classified `Honored`, in **both** `validateBehavior` and the launch-aware
`validateLaunch`. Keep `Minimized` as `UnsupportedOption` (it is genuinely not a
visible-interactive-launch state). The stale message *"Fullscreen startup is not
yet supported by the viewer host."* (`SkiaViewer.fs:578`) is replaced with an
honored message.

**Rationale.** `applyWindowBehaviorToOptions` already maps `Fullscreen` to
`WindowState.Fullscreen` (`SkiaViewer.fs:984`); the only thing blocking it was the
validator label — confirming the spec's core insight that fullscreen "reads as a
host limitation when it is really an unwired, mislabeled launch path."

**Conflict honored (Edge case).** Exclusive fullscreen on a host that cannot grant
it must fall back with an honest diagnostic rather than a false "honored" claim;
the launch-aware `validateLaunch` keeps the capability check, and windowed
fullscreen (a plain borderless window) remains the capable default.

## R3 — Default startup state (FR-003)

**Decision.** Change `defaultWindowBehavior.StartupState`
(`SkiaViewer.fs:550`) from `ViewerWindowStartupState.Normal` to
`ViewerWindowStartupState.WindowedFullscreen`. The template's default flag value
(`WindowOptions.fs`, currently `"normal"`) changes to `"windowed-fullscreen"`,
applied only when **no** `--window-startup` flag is supplied (FR-006 override).

**Rationale.** `runApp` delegates to `runAppWithWindowBehavior options
defaultWindowBehavior host` (`SkiaViewer.fs:2228`), so changing the single default
value flows to every no-flag launch. This is a public **value** change (not a
signature change), so the cross-package + per-package surface baselines move and
must be recaptured.

## R4 — Wiring the behavior into the generated launch without breaking the durable test (WIN-2, FR-004, FR-005)

**Decision.** In the generated `template/base/src/Product/Program.fs`, keep the
existing `Viewer.runApp viewerOptions generatedHost` call **reachable on a guarded
branch** (the no-window-flag default path) and add a sibling branch that calls
`Viewer.runAppWithWindowBehavior viewerOptions windowBehaviorRequest generatedHost`
when a window-startup flag is present.

**Rationale.** `GovernanceTests.fs:105` asserts the literal source text
`"Viewer.runApp viewerOptions generatedHost"` ("normal launch remains the
persistent interactive path"). It reads source text, not behavior, so the literal
must remain present and reachable — a straight swap to `runAppWithWindowBehavior`
would break the durable test. A guarded `if hasWindowFlag then
runAppWithWindowBehavior … else runApp …` branch satisfies FR-004 (live window
honors the request) while keeping the FR-005 literal reachable. The windowed-
fullscreen **default** still takes effect through `defaultWindowBehavior` (R3)
even on the `runApp` branch, so a no-flag launch is windowed fullscreen without
needing the flag.

**Alternatives considered.** Updating the test to a new literal — rejected;
`GovernanceTests.fs` is durable and model-agnostic by design (do not rewrite).

## R5 — Single-sourcing the evidence-formats contract (EV-1, FR-007)

**Decision.** Extend `EvidenceFormatSchema.fs` (`build/Governance/Evidence/`) so
the `WindowVisibility` format class enumerates **every** file the engine's
`Scans.requiredFiles` set hard-requires, with each file's required tokens, and
regenerate `template/base/docs/evidence-formats.md` from it via
`RefreshSurfaceBaselines`.

**Rationale (refines EV-1).** The engine's authoritative required set is
**seven** files (`Scans.fs:276`): `interactive-visible-window.md`,
`close-reason-separation.md`, `window-state-diagnostics.md`, `window-options.md`,
`real-image-evidence.md`, `generated-validation.md`, `evidence-audit.md`. The
generated doc currently renders only **two** of them under `## window-visibility`
(`interactive-visible-window.md`, `window-state-diagnostics.md`). The doc is
already generated from `EvidenceFormatSchema` and currency-checked by
`TargetMetadataDrift`, but the schema's window-visibility class is incomplete, so
the generator faithfully emits an incomplete contract. The fix is to complete the
schema (the single source) so generation covers all seven files and their tokens —
not to hand-edit the doc. Per-file tokens to encode come from `Scans.fs`:
interactive-visible keys (`EvidenceFormatSchema.interactiveVisibleWindowKeys`),
diagnostic-class set (`windowDiagnosticClasses`), the `window-options.md`
`option=` row set, and the presence requirements for the remaining files.

**Drift guard.** A Governance.Tests assertion (or reuse of `TargetMetadataDrift`)
must confirm the rendered doc's window-visibility file list equals
`Scans.requiredFiles`, so the two lists cannot diverge again.

## R6 — Audit stdout legibility (EV-2, FR-008)

**Decision.** On a failing audit, echo per-blocker `reason` + originating hit-file
path to **stdout** in the audit summary, not only the aggregate
`total-blockers=N`. Reuse the existing per-area diagnostic renderers
(`Render.readinessContractDiagnostics`, `Render.fs:459`, and its siblings) which
already format `reason` / `full-required-set` / `absent-from-file` per hit — wire
their output into the summary block in `GeneratedRunner.fs` / `Front/Governance.fs`
alongside the counts.

**Rationale.** The blocker detail already exists in the `ScanHit` records
(`StatusRegion.fs`: `Path`, `Reason`, `Missing`, `Required`, `ValidationArea`) and
is serialized to the `*-hits.json` sidecars (`Render.fs:371`), but only the counts
reach stdout (`GeneratedRunner.fs:242`). The legibility gap is purely a surfacing
gap — the data is present, so no new computation is needed, only echoing the
existing per-blocker renderers to stdout.

## R7 — base_ref reporting (EV-2, FR-009)

**Decision.** Thread the already-resolved merge-base / base ref into the audit
result so the summary reports it. `DiffScanResult.BaseRef` (`DiffScan.fs:190`,
hardcoded `None`) is populated from the base ref the caller already computes, and
when no base resolves the summary states that explicitly ("base_ref: none — no
default-branch ancestor; diff-scan empty by absence, not by clean diff").

**Rationale.** `runEvidenceAuditCheck` (`Front/Governance.fs:746`) **already**
resolves `baseRef` (`main`→`master`→`HEAD~1`) and `merge-base baseRef HEAD`, then
builds the unified diff from it (`:752`). But `buildEvidenceInputs` does not pass
the resolved base into `EvidenceInputs`, and `DiffScan.scan` hardcodes `BaseRef =
None` (`:190`), so the diff-scan reports `base_ref: null` even when `main` is a
strict ancestor of HEAD — making an empty diff-scan look like the source of
blockers. The "strict ancestor" resolution already works; only the **reporting**
is broken. Fix = thread the resolved value through `EvidenceInputs` →
`DiffScanResult.BaseRef` → stdout/JSON, and add the explicit-absence message for
the genuinely-unresolvable case (brand-new repo).

## R8 — Scaffold-map reconciliation (SM-1, SM-2, FR-010, FR-011)

**Decision.** Hand-edit `template/base/docs/scaffold-map.md` (it is **hand-
authored**, not generated): (a) replace literal `src/Product/**` paths with the
`<ProductDir>` / `<ProjectName>` placeholder convention plus a note that the
generated tree uses `src/<ProjectName>/**`; (b) split the "Durable" section into
*durable-and-model-agnostic* vs *durable-but-must-re-point*, moving
`LayoutEvidence.fs` and `EvidenceCommands.fs` (which read scaffold model fields)
into the re-point class with the definition "keep the file and its scanned
evidence tokens while re-pointing model-field references"; (c) add a worked
example mapping the HUD region → headers/toolbar and the gameplay region → the
main content grid for a non-game UI.

**Rationale.** `scaffold-map.md` has no generator (confirmed — only
`evidence-formats.md` is generated under `docs/`), so the fix is a direct edit. The
`<ProjectName>` placeholder makes the cited paths match a generated tree verbatim
(SC-005). `LayoutEvidence.fs`/`EvidenceCommands.fs` are currently flat-listed under
"Durable … keep them and re-point them at your own model" yet sit next to truly
model-agnostic files, so "durable" reads as "do not touch" (SM-2) — the explicit
re-point class removes that ambiguity. The doc's content is presence-tested by
`Feature062GovernanceTests.fs`; new asserted phrases must be added there to lock
the contract.

## R9 — Honest build signals (DEV-1, FR-012) — mostly already satisfied

**Decision.** No template behavior change for FR-012; verify and (lightly) extend
the existing guidance. Confirm the framework-repo guidance is consistent.

**Rationale (refines DEV-1).** The template **already** carries the FR-012
disclosure: `template/base/README.md:27` and `template/base/docs/product.md:143`
both state *"`Dev` is a completion-marker / log-writer target, not a compiler … the
first real compile/test path is `-t Test` / `-t Verify` … Do not infer 'it
compiles' from a green `Dev`."* The `Dev` target itself is `writeLog` only
(`build.fsx:212`). So FR-012's "remove the false-green via guidance" is met in the
template; the residual obligation is consistency and the Verify relationship (R10).

## R10 — Verify/audit relationship documentation (VFY-1, FR-013)

**Decision.** Add to the generated guidance (`README.md` / `docs/product.md`) an
explicit statement that `Verify` **embeds the merge-gate audit** (`EvidenceGraph`
then `EvidenceAudit`) **before** running the tests and **hard-blocks until the
feature is complete** (every task `[X]`), and that the **mid-implementation green-
test path is `-t Test`** (the first real compile, audit-free).

**Rationale.** `Verify` (`build.fsx:224`) runs `EvidenceGraph` → `EvidenceAudit`
(each `failwithf` on non-zero) **before** `runGeneratedTests`, so it cannot produce
a green test run mid-implementation — exactly the VFY-1 confusion. `Test`
(`build.fsx:223`) runs `dotnet test` only and is the first real compile. The
guidance gap is that the documented order implies the audit is a separate later
step; the fix is documentation, naming `-t Test` as the mid-implementation path.

## R11 — Graceful analyze when SymbolCrossCheck is absent (ANL-1, FR-014)

**Decision.** Edit the canonical `.agents/skills/speckit-analyze/SKILL.md`
(SymbolCrossCheck mandate, lines ~158–167) so the step **probes target
availability first** (e.g. `./fake.sh build --list` / target-resolution check) and,
when `SymbolCrossCheck` is absent (generated projects do not ship it),
**skips-with-documented-notice** and records the skip in the analysis output —
mirroring how `EvidenceGraph` resolves the feature from `.specify/feature.json` and
fails loudly only when nothing resolves. Regenerate the `.claude` mirror via
`RefreshSurfaceBaselines` (`SkillSyncCheck`-enforced).

**Rationale.** The skill currently mandates `./fake.sh build -t SymbolCrossCheck`
("do not hand-derive"), which fails with `Unknown … target: SymbolCrossCheck` in
generated projects that lack the target, forcing a non-authoritative manual pass.
`SymbolCrossCheck` is a framework-repo-only governance target, not a generated
target, so graceful degradation (not target addition) is correct — confirmed by the
spec Build-target impact note. `.agents` is canonical; `.claude` is the generated
mirror; both must stay in sync via `RefreshSurfaceBaselines`, and `SkillQualityCheck`
substring/heading detectors must still pass after the edit.

## R12 — Window-visibility trigger & this feature's own evidence obligations

**Decision.** This feature **is** a window-visibility feature and must itself
produce the full seven-file readiness set under its `feedback/`/readiness paths,
plus real visible-window image evidence for the new default and each supported
state.

**Rationale.** The window-visibility scan triggers when the concatenated feature
text contains any of `"fix window visibility"`, `"interactive-visible-window.md"`,
`"close-reason-separation.md"`, `"real-image-evidence.md"`,
`"generated-validation.md"` (`Scans.fs:276`, substring match on lowercased
spec+plan+tasks). The spec already cites several of these literals (EV-1, FR-007),
so the contract is triggered — consistent with the spec's Evidence obligations.
Authoring note (memory `evidence-readiness-authoring-gotchas`): cite the canonical
`.agents` paths, keep required tokens one-line, one skill-loading row per `[X]`
task.

## Cross-cutting notes

- **Tier / routing.** Escalated Tier 1 (`maintainer-verify`): public `.fsi` union-
  case addition + default value change, `template/**` edits, governance/build
  surface edits, and a skill edit. Run `./fake.sh build -t Route` to confirm and
  run only the gates it prints; the escalated six-target order applies.
- **Baselines.** Recapture cross-package and per-package
  (`FS.Skia.UI.SkiaViewer`) surface baselines after the `.fsi` change, and the
  `.claude` skill mirror + `validation.contract.yml` after governance edits, all
  via `RefreshSurfaceBaselines`.
- **Out of scope (recorded follow-ups).** GEN-1 cookbook helpers (parse→AST→topo
  recipe, KeyCommand note, money formatter) and the SKILL-1 scaffold-swap procedure
  skill are not delivered here.
