# Research: Persistent GUI Runtime

## Decision: `Viewer.runApp` is the interactive API

**Rationale**: Generated app entry points currently call `Viewer.runApp`, so changing it to mean true interactive play fixes the default user path without adding opt-in requirements for normal runs. The observed failure was that first-frame evidence completed the loop and closed the window; that behavior belongs in a bounded evidence API, not the default app path.

**Alternatives considered**:
- Keep `runApp` as evidence and add `runInteractive`: rejected because existing generated apps would keep defaulting to evidence behavior.
- Environment variable override for keep-open behavior: rejected because it hides the contract and makes outcomes ambiguous.

## Decision: Evidence launch is explicit and self-describing

**Rationale**: CI/readiness still needs bounded first-frame, input-dispatch, timeout, screenshot, and pixel-readback checks. These should use explicit API/CLI choices and report `mode=persistent-evidence` or another evidence-specific mode with `self-closed-for-evidence=true`.

**Alternatives considered**:
- Infer evidence mode from timeout settings: rejected because normal users should not accidentally switch modes.
- Use current `mode=persistent-window`: rejected because it does not distinguish interactive availability from launch capability.

## Decision: Launch outcome fields separate mode, close source, frame, and input state

**Rationale**: Reviewers need to distinguish first-frame success, interactive availability, user close, evidence self-close, and input dispatch. Required fields are `mode`, `window-opened`, `first-frame-presented`, `user-close-observed`, `self-closed-for-evidence`, `input-dispatch`, `blocked-stage`, `classification`, `category`, and `message`.

**Alternatives considered**:
- Preserve `exit-path=true`: rejected as too ambiguous; it can mean evidence self-close or user close.
- Only write human-readable logs: rejected because governance checks need stable field names.

## Decision: Desktop-session diagnostics run before app lifecycle diagnosis

**Rationale**: A set `DISPLAY` with missing `XDG_RUNTIME_DIR` or missing sockets is an environment/session failure, not a generated app lifecycle failure. The diagnostic must validate runtime directory presence, owner suitability, permissions, display availability, display socket, session bus when provided, and fallback status before launching the app.

**Alternatives considered**:
- Let the native window library fail first: rejected because it produces app-looking failures and slows diagnosis.
- Always create `/tmp/runtime-$UID`: rejected as a normal interactive fallback because it is not equivalent to the host desktop session.

## Decision: Generated verification fails exact package drift

**Rationale**: `NU1603` fallback allowed a generated game to run with a different viewer package than requested. Verification must fail when requested `FS.Skia.UI.*` versions do not resolve exactly and must record requested versions, resolved versions, and package sources.

**Alternatives considered**:
- Treat restore warnings as advisory: rejected because package drift changes launch behavior.
- Require manual local feed setup only: rejected because generated projects must either include required source configuration or request available published versions.

## Decision: Generated `Test` and `Verify` must execute generated tests

**Rationale**: A target that prints success without running the generated test project is non-authoritative. Generated verification must run the generated test project when it exists and label placeholder checks as non-authoritative.

**Alternatives considered**:
- Keep generated tests as optional examples: rejected because the feature requires verification-depth evidence.
- Use source scans only: rejected because source scans cannot prove generated tests execute.

## Decision: Screenshot preferred, pixel-readback fallback

**Rationale**: A generated game needs visual proof that the board is readable and interactive. Screenshot evidence is most reviewable. Pixel-readback is acceptable only when screenshot capture is unavailable but rendering output can still be inspected.

**Alternatives considered**:
- Text-only scene metadata: rejected as insufficient visual game proof.
- Require screenshots on every host: rejected because some CI/container hosts lack graphical support.

## Decision: Task workflow supports batches plus red-green logs

**Rationale**: Cohesive runtime changes may complete multiple tasks with shared evidence. Batches are acceptable when they record task names, shared evidence, and before/after graph validation. Red-green logs preserve failing-first intent for related test clusters without inventing meaningless per-task reruns.

**Alternatives considered**:
- Force graph refresh after each checked task only: rejected as noisy for cohesive batches.
- Allow final green evidence only: rejected because it loses the required red-to-green proof.
