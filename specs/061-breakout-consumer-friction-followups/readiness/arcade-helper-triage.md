# Arcade-Helper Triage (FR-011 / BD-8 / SC-008)

Per-helper ship-vs-document decision for the generalizable arcade primitives the
BreakoutDemo2 consumer re-implemented. Decision (per plan D8): **document all four as
canonical conventions** in the capability skill that already owns the domain, rather than
shipping new public API in `FS.Skia.UI.SkillSupport`. Rationale: `src/SkillSupport` is a
build-/governance-authoring library (`Globbing`, `Graph`, `Parsing`, `CodeGen`,
`ShellProcess`) — the F# peers of the `fsharp-*` authoring skills — **not** a consumer
game/runtime library. Adding game-loop/collision primitives there is a category mismatch
and would expand a governance tool's public `.fsi` surface (surface-baseline churn) for a
consumer-runtime concern. FR-011 explicitly allows "documented in the relevant skill as the
canonical convention" as a full satisfier, and 060 FR-008 set this precedent (HUD/gameplay
pattern documented, not shipped).

| Helper | Disposition | Home skill | Canonical-convention reference |
|--------|-------------|------------|--------------------------------|
| Fixed-step accumulator (`1/120 s`, capped steps/tick) deterministic `step` driver | **document** | `fs-skia-elmish` | `src/Elmish/skill/SKILL.md` → "Canonical arcade game-loop conventions" §1 (`stepFixed` snippet) |
| AABB / circle-vs-rect collision + single-reflection-per-step (axis by normalized penetration) | **document** | `fs-skia-elmish` | `src/Elmish/skill/SKILL.md` → "Canonical arcade game-loop conventions" §2 |
| Paddle-rebound angle with a `|Dy|` floor | **document** | `fs-skia-elmish` | `src/Elmish/skill/SKILL.md` → "Canonical arcade game-loop conventions" §3 |
| HUD-band reservation (`reserveHudBand`: gameplay = surface − reserved band, clamp, overdraw HUD last) | **document** | `fs-skia-layout-readability` | `.agents/skills/fs-skia-layout-readability/SKILL.md` → "Canonical convention: `reserveHudBand`" |

## Reversibility (D8 gate)

None of the four was elected to **ship**; therefore no task escalated to Tier 1 and no
`.fsi` / per-module surface baseline was added or changed. If a later feature decides to
ship any helper as real `FS.Skia.UI` API, the documented convention above is its spec —
nothing here forecloses that promotion.
