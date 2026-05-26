# Research: Improve Governance Weaknesses

## Skill-Loading Evidence

Decision: Require a per-task evidence record before a task with non-empty `skillist` can be accepted as complete.

Rationale: The current implementation guidance requires loading declared skills, but a reviewer needs task-linked evidence that the load happened before work began. A structured record can be checked by `EvidenceGraph` or `EvidenceAudit`, can be cited in readiness notes, and avoids relying on a final narrative claim.

Alternatives considered: Trust the agent's final summary; rejected because it cannot prove timing. Store only paths in `tasks.md`; rejected because task lines become too dense and cannot capture timing or exceptions cleanly.

## Skill-Match Confidence

Decision: Keep heuristic matching as a first-pass signal, but report confidence, matched signals, ambiguity, and reviewer disposition.

Rationale: The current regex-based "obviously applicable" logic is useful for common misses, but the spec calls out indirect ownership and ambiguous matches. Confidence reporting lets automation surface risk without pretending it is a proof system.

Alternatives considered: Replace heuristics with a full semantic classifier; rejected as out of scope and likely brittle. Remove automated detection; rejected because it would lose the existing useful readiness guard.

## Governance Risk Levels

Decision: Define small, medium, and broad change levels that map to minimum evidence paths.

Rationale: The repository has heavy governance targets. A risk-level contract lets small documentation or metadata changes use focused evidence while still requiring broad validation for runtime behavior, generated output, public contracts, or package-affecting work.

Alternatives considered: Always require `Dev`; rejected because the user specifically identified governance overhead. Let contributors choose ad hoc checks; rejected because readiness needs a consistent reviewable basis.

## Aggregate Hang Verdicts

Decision: Broad validation hangs should produce a timeout/orchestration verdict with stage details and focused rerun guidance, not a product failure unless a product check failed.

Rationale: The user reported aggregate `Dev` hanging in `Smoke.Tests` while a direct smoke test passed quickly. The readiness record must preserve both facts so maintainers can isolate orchestration behavior without overstating runtime product health.

Alternatives considered: Ignore aggregate hangs when focused checks pass; rejected because the aggregate remains unresolved. Treat every hang as product failure; rejected because it conflates orchestration/environment concerns with deterministic product defects.

## Runtime Limitation Documentation

Decision: Document current runtime platform and fallback constraints as limitations and roadmap boundaries.

Rationale: The feature explicitly excludes platform expansion. Naming the current Vulkan-only desktop, .NET 10, SkiaSharp preview, and fallback gaps prevents governance evidence from implying support the product has not implemented or tested.

Alternatives considered: Open implementation tasks for platform fallback; rejected as unsupported scope. Omit runtime notes because this is governance-only; rejected because the spec requires explicit limitation documentation.
