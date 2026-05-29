# Governance Scope

Recorded: 2026-05-28T17:17:57+02:00

## Tier 1 Scope

This feature is Tier 1 because it changes contracted validation behavior,
public controls surface area, generated template behavior, build command
metadata, readiness evidence, and reviewer handoff contracts.

## Public API Impact

- Additive `.fsi` changes are expected for controls front doors, typed values,
  catalog access, diagnostics, and validation/verdict model contracts.
- Visibility remains in `.fsi` files. Implementation files must not add
  top-level `private`, `internal`, or `public` modifiers.
- Package surface baselines must be refreshed only through governed commands
  when intentional public additions are complete.

## Command-Surface Impact

- Existing stable validation target names remain available.
- `AgentReady` is added as the focused agent validation path.
- In-scope validation targets migrate toward native FAKE registration while
  preserving discoverable metadata and command compatibility.

## Generated Template Impact

- Normal generated app launch remains persistent, interactive, and
  evidence-free.
- Explicit generated evidence commands own governed reports, authority wording,
  skipped gates, unsupported outcomes, and next-command guidance.
- Generated controls guidance should prefer typed standard front doors and use
  visibly custom APIs only for deliberate extension scenarios.

## MVU And Effect Applicability

- Agent validation routing and generated evidence workflows are I/O-bearing and
  require pure `Model` / `Msg` / `Effect` boundaries plus edge interpreters.
- `update` paths must not perform filesystem, process, network, wall-clock, or
  mutable-global I/O.
- Pure controls helper additions do not require a new MVU shell unless they
  introduce stateful workflow or I/O.
