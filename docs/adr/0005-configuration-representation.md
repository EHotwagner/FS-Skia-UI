---
title: ADR 0005 — Configuration representation (D6)
category: Design history
categoryindex: 90
---

# ADR 0005 — Configuration representation (D6)

- **Status**: Accepted
- **Date**: 2026-05-31
- **Decision source**: resolved in commit `7a6d65b` ("Incorporate
  config-representation decision (compiled F# over YAML/FCS)") and the
  foundations plan. This ADR records that decision.

## Context

The governance tooling needs a representation for its configuration (target
lists, validation rules, gate routing, feature/tier policy, etc.). Today this
information is spread across `build.fsx` literals, YAML
(`validation.contract.yml`, `extension.yml`, `audit-patterns.yml`), and ad-hoc
script constants. As the tooling moves into a compiled library (D1/D2), the
configuration representation must be chosen deliberately.

## Decision

**Represent governance/build configuration as compiled F#** — typed values and
modules in the governance library — **rather than as external YAML or as FSX
scripts evaluated at runtime via FSharp Compiler Services (FCS).**

## Alternatives considered

- **External YAML (rejected as the primary representation):** stringly-typed,
  parsed at runtime, errors surface late and without compiler help; schema drift
  is uncaught until execution. (Genuinely external *data* YAML, e.g.
  keyboard-input config, is out of scope — this decision concerns
  governance/build configuration.)
- **FSX evaluated via FCS at runtime (rejected):** reintroduces the
  FSharp.Compiler.Service runtime-compile tax that D2/FR-012 explicitly remove,
  and defers all errors to invocation time.
- **Compiled F# (chosen):** configuration is typed, checked by the compiler,
  refactorable with IDE tooling, and carries zero runtime-compile cost. It rides
  the same compiled-library path D1/D2 establish.

## Consequences / rationale

- **Compile-time safety:** misconfiguration is a build error, not a runtime
  surprise — consistent with Principle I (typed surfaces) and Principle VII (safe
  failure).
- **No FCS at runtime:** aligns with FR-012 and the D2 goal of removing the
  per-invocation compile tax (confirmed by the spike: no `FSharp.Compiler.*` in
  the front-end's transitive graph).
- **Single language:** configuration and logic share the F# toolchain; no
  separate YAML schema to keep in sync.
- **Trade-off:** configuration changes require a recompile rather than a text
  edit; acceptable for build/governance tooling that is already compiled, and
  preferable to silent runtime drift.

## Stages shaped

- **Stage 4** ports the Python/YAML-driven evidence configuration into typed
  compiled-F# configuration in the governance library.
- **Stage 6** (single-source generation) generates from the typed F#
  representation rather than from YAML.

## Verification in feature 039

Decision recorded only; no configuration is migrated by this feature (FR-011).
The governance library skeleton demonstrates the compiled-F# path with its one
typed public function (`Spike.run : unit -> string`).
