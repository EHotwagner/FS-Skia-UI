# Research: Targeted Refactor and Governance Diagnostics

No unresolved clarifications remain from the specification. The decisions below resolve the planning unknowns and set implementation boundaries.

## Runtime Split Boundary

**Decision**: Split `src/Lib/Library.fs` only through compile-tested, signature-governed helper files, while keeping `src/Lib/Library.fsi` stable as the consumer-facing facade. If F# signature/file pairing makes a physical split brittle for a responsibility area, group that area under a named section in `Library.fs` and record fallback evidence.

**Rationale**: The constitution requires public visibility to live in `.fsi`. New helper files can accidentally expand public surface unless they have paired signatures or remain single-file details. A conservative split reduces review risk without changing the consumer contract.

**Alternatives considered**: A broad source rewrite was rejected because it raises regression risk. Moving public types out of `Library.fs` was rejected because it would change the signature ownership model. Using top-level `internal` or `private` modifiers in `.fs` files was rejected because it conflicts with the repository visibility rule.

## Native Resource Ownership Helpers

**Decision**: Introduce small ownership helpers or equivalent scoped ownership rules for Vulkan instance, device, surface, swapchain, command pool, fence, and staging-buffer resources. Each rule records acquire stage, owner, transfer point, release action, release state, and diagnostic stage.

**Rationale**: The current Vulkan host has many dependent handles and staged failures. Explicit ownership records make cleanup order reviewable and make deterministic failure tests possible.

**Alternatives considered**: Leaving cleanup embedded in nested startup branches was rejected because failure ownership remains hard to audit. A generic disposal framework was rejected as too much abstraction for this codebase.

## Staged Startup Flow

**Decision**: Flatten `VulkanHost.run` initialization into a sequence of named startup stages using `Result` or a standard `result` computation expression. Each stage returns acquired state or a `RenderDiagnostic`; cleanup unwinds acquired ownership in reverse order when a later stage fails.

**Rationale**: The constitution permits the standard `result` computation expression. A stage pipeline makes initialization order, failure propagation, and cleanup obligations readable without changing the public viewer API.

**Alternatives considered**: Keeping nested `match`/bind blocks was rejected because it keeps failure paths hard to inspect. Introducing a custom operator or non-standard computation expression was rejected because it adds unnecessary F# complexity.

## Deterministic Acquisition Failure Evidence

**Decision**: Use deterministic injectable acquisition tests for each native resource category, disclose fake/instrumented acquisition where used, and pair that evidence with existing real native smoke coverage where the environment supports it.

**Rationale**: Real native failure modes are difficult to force reliably, but every staged cleanup branch must be testable. Synthetic disclosure keeps the fake parts visible, while real smoke coverage guards against tests that only prove the instrumented model.

**Alternatives considered**: Relying only on real Vulkan smoke was rejected because it cannot deterministically cover each failure stage. Relying only on fake handles was rejected because synthetic-only native evidence is not merge-ready under the constitution.

## Build Script Organization

**Decision**: Attempt a physical split of `build.fsx` by concern only if the FAKE entry script remains canonical and `Dev`, `Verify`, and `Ci` load cross-platform. If loading is brittle, keep one `build.fsx` and group path model, effects, interpreter, validation, governance, guidance, and target graph concerns under named sections.

**Rationale**: The repository already has a working build MVU/effect model. Organization should improve reviewability without adding multiple entry points or platform-sensitive FAKE loading behavior.

**Alternatives considered**: Splitting unconditionally was rejected because FAKE script loading can be sensitive to relative paths and script references. Leaving the script unorganized was rejected because the feature explicitly targets build reviewability.

## Structured Generated Guidance Validation

**Decision**: Convert `GeneratedGuidanceCheck` from substring-only checks to structured Markdown validation that verifies required section headings, required prompts inside the correct sections, deferred-scope placement, and active/preset parity for generated spec and plan templates.

**Rationale**: Substring checks can pass when required prompts are renamed, moved to the wrong section, or present only in deferred scope. Section-aware validation gives maintainers actionable failures.

**Alternatives considered**: Full Markdown AST dependency was rejected unless already available, because the repository can parse headings and section spans with small plain F# helpers. Keeping substring tests was rejected as too shallow.

## Semantic Template Drift Validation

**Decision**: Make `scripts/template-drift.fsx` classify changed template-owned paths into path classes and map each class to required alignment classes. A changed path passes only with same-diff alignment files plus active feature spec/plan/readiness evidence naming the changed path or affected feature area, or an accepted deferral record with required fields.

**Rationale**: The current path-only alignment signal proves that some alignment file changed, not that the right responsibility was addressed. Class mapping makes failures describe the missing work.

**Alternatives considered**: Requiring every template-owned change to update `.template.config/template.json` was rejected because docs, tests, scripts, dependency policy, and command-surface changes have different alignment needs. Trusting any docs change was rejected as too broad.

## Yoga Fallback Diagnostics

**Decision**: Use the existing `FS.Skia.UI.Layout.LayoutDiagnostic` surface when it can carry recoverable Yoga execution failure information without `.fsi` changes. The planned encoding is `Code = FallbackBoundsApplied`, `Severity = Warning`, `Constraint = Some "yoga"`, `FallbackApplied = true`, with an actionable message and available node/context. If the implementation proves this surface is insufficient, record a follow-up API proposal and do not change the signature in this feature.

**Rationale**: `LayoutDiagnostic` already exposes code, severity, message, constraint, node id, and fallback flag. That is likely enough to distinguish Yoga execution fallback while preserving the public contract.

**Alternatives considered**: Adding a new `LayoutDiagnosticCode` case or new fields was rejected for this feature because it changes the public surface. Silently falling back to pure layout was rejected because recoverable engine failures must be observable.

## Public Record Invariant Review

**Decision**: Produce a structured readiness inventory for every public record exposed by the library packages, including record name, invariant, decision, rationale, and follow-up ID when helper constructors or validation-first APIs are recommended.

**Rationale**: Public records are intentionally convenient in F#, but some carry invariants such as positive sizes, non-empty identifiers, bounded opacity, or valid paths. A review inventory separates documentation recommendations from accidental API changes.

**Alternatives considered**: Adding helper constructors directly was rejected because it changes public API scope. Reviewing only `FS.Skia.UI` records was rejected because layout, charts, and keyboard input records also expose construction surfaces.
