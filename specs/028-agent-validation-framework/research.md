# Research: Agent Validation Framework

## Decision: Store validation routing in a repository-owned YAML contract

**Rationale**: The source analysis identifies implicit gate routing as the core agent-consumer risk. A YAML contract is easy for F# governance tests, scripts, and agents to read without adding a dependency or hiding behavior in prose. It can map paths, feature concerns, risk categories, gates, expected artifacts, timeout class, authority, and failure owner in one place.

**Alternatives considered**:

- Markdown tables in `docs/build.md`: readable, but too weak for drift validation and agent-ready selection.
- Hardcoded routing in `build.fsx`: easy to execute, but not externally discoverable and repeats the current implicit-routing problem.
- JSON only: machine-readable, but less maintainable for humans editing path/glob rules.

## Decision: Prefer active feature metadata, then git merge-base diff, then degraded broad fallback

**Rationale**: The feature clarification requires active feature metadata first and git merge-base diff as fallback. This keeps agent work aligned with Spec Kit planning when metadata exists, while still supporting ad hoc local changes. If both sources are absent or ambiguous, the system must not claim focused authority.

**Alternatives considered**:

- Always use git diff: misses planned concerns and readiness obligations that may not yet be represented by changed files.
- Always use active feature metadata: fails for local edits outside an active feature.
- Run `Verify` by default: safe but slow and obscures focused failure ownership.

## Decision: Make `AgentReady` a selected-gate target, not a static `Verify` alias

**Rationale**: The agent-ready tier is meant to be the minimum feature-complete proof for the current changed concerns. It should run required focused gates from the contract plus evidence graph/audit obligations, then emit one verdict. A static alias to `Verify` would preserve broad authority but would not solve latency or routing ambiguity.

**Alternatives considered**:

- Static `Verify` alias: simpler but fails the feature's smallest-authoritative-gate goal.
- Separate shell script only: avoids build changes but weakens discoverability and target metadata parity.
- Manual task instructions: too easy for agents to under-run or over-run.

## Decision: Emit one compact verdict as JSON plus Markdown

**Rationale**: Agents need a deterministic handoff artifact, while reviewers benefit from readable summaries. JSON should be the authoritative compact contract because it can represent missing gates, selected rules, failure classes, next command, changed-path source, and evidence artifacts without prose parsing.

**Alternatives considered**:

- Markdown only: readable, but brittle for downstream automation.
- Console output only: not durable evidence.
- One verdict per target: preserves local detail but fails the one-agent-verdict requirement.

## Decision: Separate generated evidence policy from normal product launch

**Rationale**: Normal generated launch should remain product-focused and interactive. Evidence policy, report wording, audit orchestration, and command aggregation should live in explicit evidence workflows so policy changes do not churn product `Program.fs` unnecessarily or create accidental proof claims.

**Alternatives considered**:

- Keep all evidence flags in `Program.fs`: already works, but couples product executable semantics to governance policy.
- Move all evidence to repository-only targets: would leave generated consumers without explicit evidence commands.
- Remove generated evidence commands: violates the agent-consumer framework contract.

## Decision: Add typed controls front doors while preserving the lowered generic form

**Rationale**: The renderer and diagnostics benefit from a generic `Control<'msg>` and `Attr<'msg>` representation, but generated standard controls need compile-time and schema-backed guardrails. Add known control kinds, known event kinds, typed chart/grid values, and schema-backed diagnostics as additive public APIs. Keep custom constructors visibly named as custom escape hatches.

**Alternatives considered**:

- Replace the generic representation with a large typed control hierarchy: more disruptive and less flexible for rendering/catalogs.
- Leave string APIs only: preserves compatibility but fails early-diagnostic and misspelling-prevention goals.
- Remove custom extension APIs: violates compatibility and extension requirements.

## Decision: Migrate validation targets to native FAKE registration with separate metadata records

**Rationale**: FAKE already provides target registration, dependencies, target listing, and standard invocation semantics. Native registration improves external discoverability. Separate metadata records keep planning data testable and allow drift checks between runnable targets, documented targets, validation contract references, and expected outputs.

**Alternatives considered**:

- Keep the custom target interpreter: preserves current pure update tests, but external FAKE tooling remains blind.
- Move all target metadata into comments/docs: discoverable to humans only and hard to validate.
- Generate targets from metadata at runtime only: attractive long term, but riskier than preserving explicit registration plus metadata parity checks.

## Decision: Treat environment and stale-prerequisite failures as first-class verdict outcomes

**Rationale**: Agent validation frequently runs in hosts with missing desktop sessions, stale generated packages, or missing generated artifacts. These are not product defects and should not be collapsed into generic failure. The verdict must identify owner and next command so the agent can recover safely.

**Alternatives considered**:

- Use target exit code only: too coarse for agent routing.
- Keep environment classification in individual logs only: requires manual log inspection and prevents one-verdict handoff.
- Mark unsupported cases as pass: creates false authority claims.
