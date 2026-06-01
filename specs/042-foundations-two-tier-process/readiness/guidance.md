# SC-008 — guidance reframed: run `Route` first

## CLAUDE.md / AGENTS.md (FR-008)

Both now open with a **"Run `Route` first; run only the gates it prints"** section
and reframe the serialized six-target order as the **escalated `maintainer-verify`
path** reserved for consumer-contract and dogfood work — no longer the
unconditional default. Excerpt (AGENTS.md):

> ## Run `Route` first; run only the gates it prints
> Before validating a change, run `./fake.sh build -t Route`. … Run only the
> gates it prints. … A routine framework-internal change … routes to the light
> **inner-loop** tier — `Dev` only.
>
> ## The serialized six-target order (escalated / maintainer-verify path)
> The full serialized order below is the **escalated `maintainer-verify` path**,
> reserved for consumer-contract changes and **dogfood** features … It is no
> longer the unconditional default — run it only when `Route` escalates to it.

The FAKE-sequential safety guidance (`.fake` shared state, not safe to run
concurrently, deterministic numbered order, non-FAKE parallel exception) is
preserved, so `SequentialFakeGuidanceTests` still passes.

## Guidance test (FR-008, SC-008)

`tests/Governance.Tests/SequentialFakeGuidanceTests.fs` gains assertions that both
guidance files contain the `Route`-first instruction and present the six-target
order as the escalated/maintainer-verify path rather than the unconditional
default. Green in the full suite.

## docs/reports (FR-009)

`docs/reports/build.md` and `docs/reports/speckit.md` gain a "Tiered development
process and the `Route` entry point" section documenting the tiers, the
framework-author/consumer-agent axis, how `Route` selects, and `--enforce`.

Authoritative command: `dotnet test tests/Governance.Tests` (guidance test).
Result: green. Next action: none.
