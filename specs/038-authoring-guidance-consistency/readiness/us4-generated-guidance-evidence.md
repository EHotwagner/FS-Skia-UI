# US4 — Domain-Agnostic Generated Guidance (FR-007 done; FR-005/006 scope note)

(`generated-guidance.md` in this directory is the live `GeneratedGuidanceCheck`
gate output — note its US1 PASS line. This file is the US4 narrative.)

## FR-007 — domain-agnostic starter (DONE, validated)

The generated starter app and tests carry zero demo-specific (game-title)
identifiers. The generic game-starter shape is retained: a HUD/summary region, a
gameplay/playfield region, and a primary-interaction counter
(`PrimaryInteractions`), so `fs-skia-layout-evidence` stays meaningful.

Neutralization (`template/base/src/Product/{Model,View,LayoutEvidence,EvidenceCommands}.fs`,
`template/base/tests/Product.Tests/Tests.fs`):

| Demo identifier | Domain-agnostic replacement |
|---|---|
| `Score` | `Tally` |
| `Level` | `Stage` |
| `NextPiece` | `NextToken` |
| `board*` (gameplay region) | `playfield*` |
| `active-piece` | `active-token` |
| `board-readable` | `playfield-readable` |
| "Tetris-style board" | "grid-style playfield" |
| display `score:`/`level:`/`next:` | `tally:`/`stage:`/`upcoming:` |

**Scan** (`scanV3GeneratedRow` in `build.fsx`, run by `GeneratedProductCheck`):
lowercases the generated starter app + test files, strips legitimate framework
roots that contain a forbidden substring (`keyboard*` → "board",
`prooflevel`/`proof-level` → "level"), then fails on any of
`tetris`/`score`/`level`/`next piece`/`board`/`piece`.

**Failing-first** (`us4-demo-identifier-scan.txt`): the pre-neutralization
starter trips all six tokens; the neutralized starter trips none.

**Validation:** `./fake.sh build -t GeneratedProductCheck` regenerates all four
profiles, runs Dev/Test/Verify inside each generated product (the neutralized
starter compiles and its own tests pass), and runs the demo-identifier scan —
green. See `logs/generated-product-check.txt`.

## FR-005 / FR-006 — framework-only paths & per-skill snippets (DONE, validated)

**Resolution — consumer skill variants (the `samples` precedent applied to every
runtime capability).** The generated capability skills were copied verbatim from
the dual-purpose `src/<Cap>/skill/SKILL.md` framework skills, which reference
framework-only targets (`CapabilityCheck`, `PackLocal`, `DependencyReport`,
`PackageSurfaceCheck`, `readiness/surface-baselines`, `src/.../*.fsi`) and (for
most) carry no consumer-runnable snippet. Rather than degrade the framework-dev
skills, each runtime capability's `skill:` in `template/capabilities.yml` is
repointed to a consumer-facing fragment skill
`template/fragments/<cap>/skill/SKILL.md` — exactly how `samples` already sourced
its generated skill:

| Capability | Generated dir (`capabilitySkillDestination`) | Consumer fragment skill | `name:` |
|---|---|---|---|
| scene | `fs-skia-scene` | `template/fragments/scene/skill/SKILL.md` | `fs-skia-scene` |
| skiaviewer | `fs-skia-skiaviewer` | `template/fragments/skiaviewer/skill/SKILL.md` | `fs-skia-skiaviewer` |
| elmish | `fs-skia-elmish` | `template/fragments/elmish/skill/SKILL.md` | `fs-skia-elmish` |
| keyboard-input | `fs-skia-keyboard-input` | `template/fragments/keyboard-input/skill/SKILL.md` | `fs-skia-keyboard-input` |
| controls | `fs-skia-ui-widgets` | `template/fragments/controls/skill/SKILL.md` | `fs-skia-ui-widgets` (renamed) |
| testing | `fs-skia-testing` | `template/fragments/testing/skill/SKILL.md` | `fs-skia-testing` |

`layout` is excluded: its generated skill is folded into Controls whenever
Controls is selected, and layout only appears in the app profile (which always
selects Controls), so no layout capability skill ever reaches a consumer. The
framework `src/<Cap>/skill/SKILL.md` files are unchanged and stay framework-dev
oriented (they still validate under `SkillCheck`).

Each consumer fragment skill: points its Public Contract at the bundled
`docs/api-surface/<Pkg>/<Pkg>.fsi` (US2) instead of `src/.../*.fsi`, lists only
consumer build targets (`Dev`/`Test`/`Verify`), carries at least one
consumer-runnable ```fsharp usage snippet, and names no framework-only target or
surface-baseline path. Matching `template/fragments/<cap>/README.md` files carry
the same snippet.

**Enforcement scan (FR-005/FR-006).** `scanV3GeneratedRow` in `build.fsx`
(`GeneratedProductCheck`) now enumerates each generated runtime capability-usage
skill and FAILS when it names a framework-only target
(`CapabilityCheck`/`PackLocal`/`DependencyReport`/`PackageSurfaceCheck`), points
at a `src/.../*.fsi` source path, names `readiness/surface-baselines`, or has no
```fsharp snippet.

**Failing-first** (`us4-consumer-skill-scan.txt`): all six pre-fix framework
`src` skills trip the scan (framework-only refs and/or no snippet); all six
consumer fragment skills pass.
