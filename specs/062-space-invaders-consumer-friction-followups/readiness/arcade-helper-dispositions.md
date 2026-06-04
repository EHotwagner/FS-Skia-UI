# Arcade-helper ship decisions + fourth-prompt candidate dispositions (FR-010/FR-011)

No candidate is silently dropped: every helper and every fourth-prompt skill-gap
candidate has a recorded ship / fold / defer-with-rationale disposition (D10/D11, SC-006).

## FR-010 — per-helper ship decisions

| helper | decision | home / disposition |
|---|---|---|
| seeded RNG (`seedRng`/`nextRng`/`nextBelow`) | **SHIP** | `FS.Skia.UI.SkillSupport.Random` (splitmix64 seed → xorshift64 stream, pure threading, no ambient `System.Random`); skill reference in `fs-skia-elmish`; new per-package surface baseline |
| `reserveHudBand` | **SHIP** | `FS.Skia.UI.SkillSupport.Hud` (plain-`float` band API, no `Scene.Rect` dependency); skill reference in `fs-skia-layout-readability` |
| fixed-step accumulator (`stepFixed`) | **DEFER (documented)** | stays a documented `fs-skia-elmish` convention — not yet at the 3-demo recurrence bar; more shape-variable per game; ship on recurrence |
| collision + single-reflection-per-step | **DEFER (documented)** | stays a documented `fs-skia-elmish` convention — collision geometry varies per game; ship on recurrence |
| paddle-rebound angle with `|Dy|` floor | **DEFER (documented)** | stays a documented `fs-skia-elmish` convention — game-specific; ship on recurrence |

**Why ship only the two:** the seeded RNG and `reserveHudBand` were re-implemented
across three consecutive demos (Asteroids → Breakout → SpaceInvaders) — the spec's
stated bar for escalating 060 FR-008 / 061 FR-011 D8 from documented to shipped. The
three deferred loop primitives have not shown the same cross-demo recurrence and are
more shape-variable, so shipping them now would broaden the SkillSupport surface ahead
of demonstrated demand. They are recorded here, not dropped.

## FR-011 — disposition of the five fourth-prompt skill-gap candidates

| # | candidate | disposition |
|---|---|---|
| 1 | Spec Kit hook execution policy | **FOLDED → FR-001/002** (precedence rule + effective-hooks notice in every hook-bearing phase skill). No new skill. |
| 2 | Generated game simulation core | **PARTIALLY SHIPPED + DEFERRED.** Seeded RNG + `reserveHudBand` ship (FR-010) with skill references; the fixed-step / collision / paddle-rebound loop primitives stay documented in `fs-skia-elmish` + `fs-skia-layout-readability`; the full standalone "simulation core" skill is explicitly deferred (D10/D11). The SI-2 durable-vs-replaceable map (`docs/scaffold-map.md`) is its companion reference. |
| 3 | Speckit task-graph linter/explainer | **FOLDED → FR-006** (generated `docs/skillist-reference.md` resolving id/`name:` + closed `owns:` table) **and FR-007** (effective-DAG render with injected edges + resolved skillist set). No new skill. |
| 4 | Cross-artifact symbol consistency | **FOLDED → FR-008** (compiled `SymbolCrossCheck` set-diff + speckit-analyze detection pass G). No new skill. |
| 5 | Speckit evidence-format authoring | **FOLDED → FR-005** (per-class schema-printing diagnostics + generated `docs/evidence-formats.md`, single-sourced from `EvidenceFormatSchema`). No new skill. |

Skill references for the shipped helpers land in `fs-skia-elmish` (pure-`update`
threading owner → `Random`) and `fs-skia-layout-readability` (HUD/gameplay-region
owner → `Hud`), so an arcade-demo author finds them before re-implementing.
