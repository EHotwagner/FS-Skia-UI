# ADR 0008 — Scene-vocabulary single source

- **Status**: Accepted
- **Date**: 2026-06-02
- **Decision source**: the V3 modular-distribution implementation plan
  (`docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md`, §0.4 + Stage 1)
  and feature `048-v3-retirement-baseline` (research.md D8).

## Context

The scene vocabulary is defined **twice**: in the split `src/Scene/Scene.fsi`
(structured `Scene = { Nodes: SceneNode list }`) and in the monolith
`src/Lib/Library.fsi` (an opaque `Scene` plus duplicate value types). The before-baseline
(`docs/reports/_baselines/2026-06-02-v3-before.md` §4) records **34** types defined in
both files, plus duplicate `Scene`/`Picture`/`SceneNode` and the `Colors`/`Paint`/`Path`/
`Scene` modules. The host bridges the two representations via
`src/SkiaViewer/SceneConversion.fs`.

## Decision

**`FS.Skia.UI.Scene` is the canonical, single source of the scene vocabulary.** The
monolith's duplicate scene types are **deleted**, the host is retyped directly onto the
`FS.Skia.UI.Scene` types, and `SceneConversion.fs` is removed — **no permanent conversion
shim**. A temporary build flag may gate the cut-over until parity is signed off
(ADR 0011), then it is removed.

## Alternatives considered

- **Keep `SceneConversion.fs` as a permanent adapter (rejected).** Institutionalizes the
  duplication and a per-frame conversion cost; the duplication is the very thing the
  retirement removes.
- **Make the monolith's `Scene` the canonical type (rejected).** The monolith is being
  deleted; the canonical vocabulary must live in a surviving split package, and the
  structured `FS.Skia.UI.Scene.Scene` is the richer, already-public representation.

## Rationale

A single structured `Scene` type eliminates the 34-type duplication, the opaque/structured
split, and the conversion shim in one move. It is the precondition for deleting `src/Lib`.

## Affected stages

- **Stage 1** (keystone): delete the monolith's duplicate vocabulary; retype the host onto
  `FS.Skia.UI.Scene`; remove `SceneConversion.fs`.
- Verified byte-identically by the **parity oracle** (ADR 0011, SC-003).
