# Research: Sokoban Feedback Follow-ups

## Decision: Unify screenshot text rendering with glyph-capable rendering

**Rationale**: The package rendering path already contains deterministic vector text fallback for default text, while the current SkiaViewer screenshot scene path paints text as filled rectangles. The feature should make screenshot evidence use glyph-capable rendering or an equivalent fallback so default HUD/status text is reviewable without custom font setup.

**Alternatives considered**: Requiring generated apps to specify a font was rejected because FR-001 and FR-002 require readable default behavior. Keeping rectangle placeholders was rejected because it directly causes unreadable evidence. Adding a bundled font dependency was deferred unless existing SkiaSharp/default-typeface plus vector fallback cannot satisfy supported hosts.

## Decision: Detect screenshot text readability with image-based capability checks

**Rationale**: The user-facing failure is visible in the captured image, not only in scene metadata. A capability check should render representative default text, inspect the output for glyph-shaped coverage, and fail on solid blocks or placeholder-only output. The report should include artifact path, host facts, classification, and the failing metric.

**Alternatives considered**: Manual screenshot review was rejected because the spec requires automated validation. Text-bound metadata alone was rejected because it can pass while the rendered output remains a block.

## Decision: Treat generated persistent close evidence as real host evidence, not bounded substitution

**Rationale**: The generated app already separates default persistent launch from explicit evidence commands. The accepted proof must launch the real viewer-backed host, observe first frame/window-opened facts, request or emit app-level close, and record a clean exit path. Bounded smoke, scene evidence, screenshot evidence, and deterministic render proof are useful diagnostics but cannot substitute for persistent-window evidence.

**Alternatives considered**: Using a bounded evidence-only run was rejected because FR-006 distinguishes it from accepted persistent launch. Manual close was rejected because SC-002 requires an automated or agent-run close path under 60 seconds.

## Decision: Publish a compact generated consumer API map instead of making authors inspect internals

**Rationale**: Generated demo authors need the practical shape of keyboard keys, host callbacks, viewer effects, adapter commands, and scene nodes before coding. A compact map in generated guidance can point to the stable public package fronts and reduce incorrect framework-internal guesses.

**Alternatives considered**: Asking authors to reflect over assemblies or browse source was rejected by FR-008. Duplicating full API reference docs in generated products was rejected because the need is a compact authoring map for common demo flows.

## Decision: Make readiness and task validator pitfalls visible before implementation

**Rationale**: Evidence audit and task graph failures are currently discoverability failures. Generated and repository task guidance should name the feature-scoped readiness directory, required files and terms, known title trigger phrases, and `tasks.deps.yml` indentation/shape rules before authors run the validator.

**Alternatives considered**: Leaving discovery to audit failures was rejected by FR-015. Relaxing graph validation was rejected because this feature is about guidance and preflight clarity, not replacing governance.

## Decision: Keep FAKE-backed validation serialized and recorded

**Rationale**: Repository guidance states that `./fake.sh`, `fake.cmd`, and `dotnet fake` share `.fake` state and are not safe to run concurrently. Validation evidence for this feature must run multiple FAKE-backed targets one at a time and record the order.

**Alternatives considered**: Running focused targets in parallel was rejected due to known `.fake` state races. Running only broad `Verify` was rejected as the sole plan because the feature needs focused generated guidance, template, generated product, graph, and audit evidence.
