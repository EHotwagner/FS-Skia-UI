# Phase 1 Data Model: Authoring Guidance Consistency

These are governance / guidance entities (not runtime MVU state). They define
what the guards and generated artifacts operate on.

## Skill Identity

The three facets of a skill that MUST agree.

| Field | Source | Rule |
|---|---|---|
| `directory` | folder name under `src/*/skill`, `.agents/skills/*`, `.claude/skills/*`, `template/fragments/*/skill` | — |
| `declaredName` | `name:` in that skill's `SKILL.md` | MUST equal the advertised id |
| `advertisedId` | the id used for it in hints/scan phrases and the harness list | MUST resolve to some `declaredName` |

Validation: `directory`, `declaredName`, and every `advertisedId` for the skill
are mutually consistent, or the resolution guard fails (FR-002).

## Advertised-Id Set

The set of skill ids the task generator / hints / scan phrases tell an author to
reach for. Derived from `speckit-tasks/SKILL.md` `... -> <id>` mappings and the
harness "available skills" surface. Every member MUST resolve to a
`declaredName` (FR-001). Current dangling member: `speckit-debug-loop` (to be
removed).

## Skill Peer Pair

A `.agents/skills/<x>` and `.claude/skills/<x>` pair for the same skill. MUST
declare the same `name:` and advertise the same id; drift fails the guard
(FR-003). Validated as synchronized peers (existing repo convention).

## Bundled API Reference

The local authoritative signatures shipped into a generated project.

| Field | Value |
|---|---|
| `location` | generated `docs/api-surface/` tree |
| `members` | the `.fsi` files for each capability the generated profile includes (from `capabilities.yml` `contracts:`) |
| `derivation` | verbatim copy-at-generation from `src/.../*.fsi`; never hand-maintained |
| `failure` | a referenced package's signatures absent or drifted from source |

Satisfies: an author reads any union case's exact field order locally, no DLL
reflection (FR-004, SC-002).

## Collision-Prone Public Name

A public name that shadows ordinary consumer code after `open`.

| Name | Location | Remedy |
|---|---|---|
| `ViewerWindowStartupState.Normal` | `src/SkiaViewer/SkiaViewer.fsi:43-48` | `[<RequireQualifiedAccess>]` (breaking, accepted) |
| viewer/input `update`/`init` surfaces | `SkiaViewer.fsi`, `Elmish.fsi`, `KeyboardInput.fsi` | RQA or confirmed module-qualification (R3) |

State after hardening: a consumer's own `Normal`/`update`/`init` resolve to the
consumer's definitions; the framework's require qualification (FR-008, SC-003).

## Scene Constructor Variant

| Variant | Shape | Status |
|---|---|---|
| positional `Rectangle` | `(float*float*float*float)*Color` | retained (additive) |
| positional `Text` | `(float*float)*string*Color` | retained (additive) |
| self-describing rectangle | `Rect`-based / named-argument | added (FR-010) |
| self-describing text | `Rect`/named-argument | added (FR-010) |

Invariant: existing code keeps compiling; a consistent self-describing form
exists for `Rectangle`/`PaintedRectangle`/`Text` (SC-006).

## Canonical Effects Page

| Field | Value |
|---|---|
| `location` | `template/base/docs/effects-boundary.md`, bundled into generated output |
| `categories` | application commands at the MVU edge; viewer effects at the host boundary |
| `wiring` | canonical `update`→host (`Viewer.runApp viewerOptions generatedHost`) |
| `reachability` | present and self-contained in a generated project (no scattered reports/source) |

Satisfies FR-009, SC-005.

## Feature-Targeting Guard

| Field | Value |
|---|---|
| `target source` | `.specify/feature.json` `feature_directory` |
| `assertion` | gates audit the resolved feature; a bare filename mention in `tasks.md` does NOT trigger required evidence |
| `provenance` | behavior established by feature 037; this is a regression guard only |

Satisfies FR-011, SC-008.
