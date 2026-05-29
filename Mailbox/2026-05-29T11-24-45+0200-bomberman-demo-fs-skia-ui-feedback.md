# Bomberman Demo FS.Skia.UI Feedback

Date: 2026-05-29T11:24:45+0200
Source app: `/home/developer/projects/BMLightDemo1`
Validation context: Bomberman Lite generated app implementation, evidence commands, `Dev`, `Test`, `Verify`, and Spec Kit evidence audit.

## Summary

The Bomberman Lite demo was implemented successfully against the current FS.Skia.UI packages. The app can launch through the persistent interactive viewer path, run deterministic gameplay evidence, produce layout evidence, and pass the feature evidence audit.

The integration did expose a few framework and tooling friction points that are worth improving in FS.Skia.UI or its generated-app support:

- Evidence graph script invocation depends on executable file mode.
- `Verify` log capture can contain NUL bytes.
- Scene/layout record types are easy for F# inference to confuse.
- The app-level screenshot command must actually call the FS.Skia.UI screenshot API; a hardcoded unsupported fallback can hide working host support.
- Generated apps need more boilerplate than ideal to connect pure MVU state to viewer effects.

## Observed Issues

### 1. Evidence Graph Script File Mode

Running the graph script directly failed from the generated app checkout:

```text
.specify/extensions/evidence/scripts/python/compute-task-graph.py: Permission denied
```

The same script succeeded when invoked explicitly through Python:

```bash
python .specify/extensions/evidence/scripts/python/compute-task-graph.py specs/001-bomberman-lite-demo
```

Impact: generated projects or docs that instruct direct execution can fail even though the script logic is valid.

Suggested improvement: make the wrapper consistently call `python` or ensure generated/checked-out scripts have executable mode preserved.

### 2. Verify Log NUL Bytes

One redirected `./fake.sh build -t Verify > readiness/logs/Verify.txt 2>&1` run produced a valid pass message but appended a large NUL byte block to the text log. The log needed cleanup with `tr -d '\000'`.

Impact: readiness artifacts become noisy or difficult to review, even when validation passes.

Suggested improvement: audit the `Verify` target and process/log capture code for binary or preallocated buffer writes to stdout/stderr. Build targets intended for readiness should produce clean UTF-8 text.

### 3. Record Field Inference Friction

The Scene/layout APIs expose several records with overlapping labels such as `X`, `Y`, `Width`, `Height`, `Diagnostics`, `State`, and `Position`. In app code, F# inference frequently chose the wrong record type until explicit annotations were added.

Examples of places that needed annotations:

- `Rect` comparisons in layout evidence helpers.
- Model helpers returning `Model` vs evidence/report records with `Diagnostics`.
- Scene drawing helpers for `Player`, `Enemy`, `Bomb`, and `Powerup`.

Impact: consumer app code becomes more annotation-heavy and compile errors point at surprising types, especially in generated examples that should be easy to follow.

Suggested improvements:

- Provide small constructor/helper functions for common Scene/layout records.
- Consider more domain-specific labels in evidence records where practical.
- Add generated-app guidance that annotates helper arguments at Scene/layout boundaries.

### 4. App-Level Screenshot Command Initially Hid Working Host Support

The first Bomberman implementation incorrectly hardcoded the screenshot evidence command to report:

```text
status=unsupported
supported-host=false
proves-screenshot=false
```

That was not a framework capability result. After wiring the command to `Viewer.captureScreenshotEvidence`, the same host produced:

```text
status=ok
screenshot-path=specs/001-bomberman-lite-demo/readiness/bomberman-screenshot-evidence.png
capture-source=LiveViewerWindow
supported-host=true
proves-screenshot=true
```

Impact: generated apps can accidentally mask working screenshot support if the evidence command uses a policy fallback instead of the real viewer capture API.

Suggested improvement: generated screenshot evidence templates should call `Viewer.captureScreenshotEvidence` by default, and tests should assert that the command attempts the real capture path before accepting an unsupported report.

Root cause: the app command was implemented around the allowed fallback contract before it attempted the real screenshot contract. The spec permits `status=unsupported` when capture is unavailable, and that escape path was accidentally treated as sufficient evidence-command behavior. This was an app-level implementation error, not an FS.Skia.UI host limitation.

Prevention:

- Evidence commands should attempt the real evidence path first, then fall back to `unsupported` only from the API result.
- Tests should reject screenshot commands that hardcode `status=unsupported` without referencing `Viewer.captureScreenshotEvidence`.
- Any `unsupported` result should be treated as a capability claim that needs a direct API probe before it is documented as a framework or host issue.
- Feedback notes should distinguish observed API results from app-command results.
- The evidence audit could add a pattern for `--*-screenshot-evidence` implementations: require `Viewer.captureScreenshotEvidence` or an explicit reviewer-visible rationale.

### 5. Viewer/MVU Wiring Boilerplate

The app-owned pure model/update path worked well, but each generated app still needs to wire:

- `MapKey`
- `Tick`
- `RenderScene`
- host update adapter
- app effects vs viewer effects separation
- default persistent `Viewer.runApp` launch handling

Impact: every generated game app repeats similar adapter code, and small mistakes can blur pure app effects with viewer effects.

Suggested improvement: provide a generated-app helper for the common pattern:

```fsharp
pure init/update/view + key mapper + tick mapper -> ViewerHost
```

The helper should preserve the current MVU boundary: pure update returns app effects, and viewer/file/native work remains at the host/interpreter edge.

## Positive Findings

- Persistent launch worked in this environment and reported `window-opened=true`, `first-frame-presented=true`, and `user-close-observed=true`.
- Live screenshot capture worked after the app command called `Viewer.captureScreenshotEvidence`, producing a non-blank PNG from `LiveViewerWindow`.
- `RenderScene(view model)` was straightforward once the host boundary was explicit.
- Layout evidence contracts were flexible enough to report HUD region, arena region, tile size, categories, proof level, and overlap status.
- Unsupported screenshot reporting remains a useful contract path, but this host should not be classified as screenshot-unsupported.

## Recommended Priority

1. Fix script invocation/file-mode robustness for evidence graph commands.
2. Investigate NUL byte output in `Verify` readiness logs.
3. Add generated-app helper APIs or templates for MVU-to-viewer wiring.
4. Make generated screenshot evidence templates exercise `Viewer.captureScreenshotEvidence` before falling back to unsupported reporting.
5. Add consumer-facing examples showing type annotations around Scene/layout record-heavy code.
