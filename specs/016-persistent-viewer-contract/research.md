# Research: Persistent Viewer Contract

## Decision: Add persistent `Viewer.run` for scene apps

**Rationale**: The current `FS.Skia.UI.SkiaViewer` surface exposes bounded evidence runs but not a default desktop-window contract. A direct persistent scene entry point gives simple graphical apps a clear product path that opens a window, renders app content, and remains active until close or host failure.

**Alternatives considered**:
- Treat `runUntilFirstFrame` as product launch: rejected because it is bounded by design and can exit without proving interactive readiness.
- Keep persistent launch only in generated templates: rejected because package consumers need a public surface, not template-private behavior.

## Decision: Add persistent `Viewer.runApp` for generated model-driven apps

**Rationale**: Generated graphical apps need a standard model/update/view/input/time contract. `runApp` keeps update logic pure while the viewer interpreter owns native window, render, keyboard, tick, effect, and close handling at the edge.

**Alternatives considered**:
- Let each generated app hand-roll its own loop: rejected because it repeats host logic and weakens governance.
- Require Elmish `Program` directly in SkiaViewer: rejected for now because generated apps can wrap Elmish-style behavior without forcing a runtime dependency into the viewer contract.

## Decision: Preserve bounded APIs as evidence helpers

**Rationale**: CI and unsupported hosts still need deterministic first-frame, frame-count, scene metadata, and diagnostics workflows. Keeping these APIs avoids compatibility churn while documentation and governance make their readiness limits explicit.

**Alternatives considered**:
- Remove bounded APIs: rejected because they are useful for CI and diagnostics.
- Allow bounded APIs to satisfy graphical readiness when live renderer succeeds: rejected because bounded execution does not prove persistent interactive app behavior.

## Decision: Add runtime capability and launch outcome diagnostics

**Rationale**: Generated apps and reviewers must distinguish missing product/package capability from unsupported display environments. Capability and launch outcomes should include persistent window support, bounded smoke support, keyboard support, renderer mode, unsupported reason, blocked stage, classification, category, command path, and message.

**Alternatives considered**:
- Use exceptions alone: rejected because reviewers need structured readiness artifacts.
- Use one failure string: rejected because it cannot reliably distinguish unsupported environment from product defect.

## Decision: Make persistent host the generated graphical default

**Rationale**: The default executable is the user-facing product path. For profiles selecting SkiaViewer, the default path must attempt persistent launch or produce a persistent-launch diagnostic. Evidence helpers remain available only through explicit flags.

**Alternatives considered**:
- Keep count-and-exit default plus docs: rejected because it repeats the Tetris demo failure mode.
- Add a `--launch` flag while default stays bounded: rejected because default execution is the readiness-critical path.

## Decision: Add governance checks for bounded-only substitution

**Rationale**: Task generation and audit must prevent generated graphical features from passing with only model tests, scene evidence, or bounded viewer smoke. `GeneratedGuidanceCheck`, generated product validation, `EvidenceGraph`, and `EvidenceAudit` need a distinct persistent launch artifact class.

**Alternatives considered**:
- Rely on reviewers to notice missing launch evidence: rejected because the current workflow already missed it.
- Treat unsupported-host diagnostics as completion evidence: rejected by the spec clarification; at least one supported-host persistent launch artifact is required.
