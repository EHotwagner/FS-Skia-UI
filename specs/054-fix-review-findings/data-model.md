# Phase 1 Data Model: Fix Implementation-Completeness Review Findings

This feature changes no runtime domain types and introduces no `.fsi` surface. The "entities"
below are the **governance/config artifacts** the fixes act on and the invariants that bind
them. They exist to make the gate assertions concrete, not to model application state.

## Entity: TemplateEnginePin

The pair of version references that must stay equal across the generated template.

| Field | Location | Form | Role |
|---|---|---|---|
| `PropsVersion` | `template/base/Directory.Packages.props` | `<PackageVersion Include="FS.Skia.UI.Build" Version="X" />` | **Source of truth** |
| `ScriptVersion` | `template/base/build.fsx` (line 1) | `#r "nuget: FS.Skia.UI.Build, X"` | **Derived** value |

**Invariant (INV-1)**: `ScriptVersion == PropsVersion`, compared as an **exact string** (no
numeric coercion; tolerates `-preview.N` suffix and any digit/segment count). Today violated
(`0.1.45-preview.1` ≠ `0.1.56-preview.1`).

**Transition (pin bump)**: the `fs-skia-template-update` flow sets `PropsVersion := newVer`; the
extended flow MUST also set `ScriptVersion := newVer` in the same step so INV-1 holds after every
bump (FR-002). The `#r` literal is matched by its own regex (the `, <ver>` form), distinct from
the props `Version="<ver>"` form that the existing `sed` targets.

**Gate (FR-003)**: `GeneratedProjectValidationTests` asserts INV-1 — extracts both versions and
requires exact equality (replacing the prefix-only `stringContains "#r \"nuget: FS.Skia.UI.Build"`
assertion). A deliberate mismatch fails `TemplateCheck`/`GeneratedProductCheck`.

## Entity: NullnessSite

A single FS3261 emission in `FS.Skia.UI.Build`. 34 distinct sites across 8 files (see
`research.md` R2). Each has a resolution class:

| Class | Shape | Resolution |
|---|---|---|
| `NullableBclString` | `Environment.GetEnvironmentVariable`, regex `Groups[n].Value`, `Path.GetDirectoryName`, etc. → non-nullable `string` | pattern-match `null` / `nonNull` / `Option.ofObj` + explicit default |
| `NullableRef` | `Process.Start` → `Process \| null`, `char seq \| null` | pattern-match `null`; **fail-fast** on unexpected null (Constitution VII) |
| `SignatureNullness` | impl infers `string \| null` but `.fsi` declares `string` (`Engine/Model.fs:72`) | make impl value provably non-null to match the `.fsi` — **no `.fsi` change** |

**Invariant (INV-2)**: post-change, the clean (`--no-incremental`) build emits **0** FS3261;
behaviour of every affected function is unchanged (INV-3, verified by green `Governance.Tests`).

**Gate (FR-009)**: removing `<WarningsNotAsErrors>$(WarningsNotAsErrors);FS3261</...>` from
`FS.Skia.UI.Build.fsproj` makes FS3261 an **error** for this project — the compiler enforces
INV-2 thereafter. Project-local only; `Directory.Build.props` is untouched.

## Entity: ReadinessScratch

The stray pack-flow artifact and the rule that suppresses its recurrence.

| Field | Value |
|---|---|
| `StrayFile` | `specs/053-v3-monolith-retirement/readiness/package/local-packages.md` |
| `Disposition` | pack-flow scratch (regenerable) → removed |
| `IgnoreRule` | `specs/*/readiness/package/` added to `.gitignore` (Feature-046 hygiene block) |

**Invariant (INV-4)**: after the standard pin-bump/pack flow, `git status --porcelain` is empty
(FR-008); a routine framework-internal diff routes to `inner-loop` (SC-007), confirming the
governance-path escalation is gone. The ignore rule is scoped to the `package/` scratch subdir —
authored `.md` evidence elsewhere stays tracked.

## Relationships

```
TemplateEnginePin ──(INV-1 exact-equal)── enforced by GeneratedProjectValidationTests (FR-003)
        │
        └──(set together)── fs-skia-template-update pin-bump flow (FR-002)

NullnessSite (×34) ──(INV-2 zero)── enforced by removed escape hatch / compiler (FR-009)

ReadinessScratch ──(INV-4 clean tree)── enforced by .gitignore rule + removal (FR-007/008)
```

No state machine, no MVU model: every change is either a static-text invariant (pins, ignore
rule) or a behaviour-preserving null-handling edit. Constitution IV (Elmish boundary) does not
apply — there is no new stateful or I/O-bearing workflow.
