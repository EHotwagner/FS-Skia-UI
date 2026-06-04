# Data Model: Feature 062

This feature is governance/authoring + a small public helper surface; the
"entities" are the value shapes the new code and contracts manipulate. No
framework `Model`/`Msg`/`Effect` is added (Principle IV N/A — see plan).

---

## 1. EffectiveHookDecision (FR-002 — conceptual; rendered as a notice, not stored)

The resolved decision for one merged hook in a phase.

| Field | Type | Notes |
|---|---|---|
| `extension` | string | e.g. `git`, `evidence`, `feedback` — dedup key part 1 |
| `command` | string | e.g. `speckit.git.commit` — dedup key part 2 |
| `phaseKey` | string | `before_<phase>` / `after_<phase>` |
| `optional` | bool | from the merged registration |
| `enabled` | bool | `enabled: false` → skipped |
| `hasCondition` | bool | non-empty `condition` → deferred to executor |
| `decision` | enum | `auto-run` \| `surfaced` \| `skipped (disabled)` \| `condition-deferred` |

**Derivation (D1/D2).** `decision = `
- `skipped (disabled)` if `enabled = false`;
- else `condition-deferred` if `hasCondition`;
- else `auto-run` if `optional = false` **and** `auto_execute_hooks = true`;
- else `surfaced` (all `optional = true`, and mandatory hooks when
  `auto_execute_hooks = false`).

Deduped by `(extension, command)`, first occurrence wins (multi-file discovery
already specified in the phase skills). The promoted feedback hook is
`optional = false` → `auto-run`.

---

## 2. EvidenceFormatSchema (FR-005)

The per-file required shape, single-sourced from the constants that enforce each
rule (so the printed diagnostic and the generated `evidence-formats.md` reference
cannot drift from the validator).

| Field | Type | Notes |
|---|---|---|
| `fileName` | string | e.g. `skill-loading-evidence.md`, `interactive-visible-window.md` |
| `formatClass` | enum | `readiness-contract` \| `skill-loading-evidence` \| `window-visibility` \| `seh-acceptance` |
| `requiredTokens` | string list | the full enforced token/key list (already carried as `Required` for readiness-contract) |
| `tableColumns` | string list option | for tabular formats (the 8-column skill-loading-evidence row) |
| `orderingRules` | string list | e.g. `loaded_at < work_started_at` |
| `resolvedPathPattern` | string option | e.g. `.agents/skills/<id>/SKILL.md` |
| `blocking` | bool | whether a violation hard-blocks |

**skill-loading-evidence row** (the 8 columns the validator parses):
`TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt |
WorkStartedAt | EvidencePath | Exception`, one row per `(task, skill)`,
`loaded_at < work_started_at`, `EvidencePath` = `.agents/skills/<id>/SKILL.md`.

**window-visibility / `diagnostic-class`**: `key=value` rows; `diagnostic-class`
∈ `{ environment-session, window-visibility, app-lifecycle, product-defect }`
plus the per-file required keys (status/mode/window-visible/… per file).

**SEH acceptance**: tokens `accepted-seh` (acceptance status) and
`synthetic-error-handling-approved` (label) — **no backticks**.

---

## 3. SymbolSet & SymbolDiff (FR-008)

Extracted from `plan.md`, `data-model.md`, `tasks.md` for the cross-artifact check.

| Field | Type | Notes |
|---|---|---|
| `kind` | enum | `msg-case` \| `union-or-screen-variant` \| `entity-record` \| `fr-id` \| `sc-id` |
| `name` | string | the symbol token (e.g. `ViewerKeyEventReceived`, `Initial`, `FR-016`) |
| `presentIn` | set of artifact | subset of `{ plan, data-model, tasks }` |

`SymbolDiff` = symbols whose `presentIn` is a **proper subset** of the artifacts
where that `kind` is expected. Reported as findings; intentionally design-only
symbols (e.g. present in design but not yet a spec FR) are reported for human
judgment, never hard-failed.

---

## 4. RngState (FR-010 — `FS.Skia.UI.SkillSupport.Random`)

Pure, replayable seeded RNG. **No ambient `System.Random`.**

| Field | Type | Notes |
|---|---|---|
| `state` | `uint64` | the xorshift64 stream word (the whole durable state) |

Transitions (pure `state -> (value, nextState)` threading):
- `seedRng (seed: uint64) : RngState` — splitmix64-expand the seed to the initial
  stream word (avoids the all-zero xorshift fixed point).
- `nextRng (s: RngState) : uint64 * RngState` — one xorshift64 step.
- `nextBelow (n: int) (s: RngState) : int * RngState` — uniform-ish `[0, n)` via
  the next word; `n > 0` precondition.

**Determinism invariant (tested):** same seed + same call sequence ⇒ identical
value stream (replay equality), independent of platform/wall-clock.

---

## 5. HudLayout (FR-010 — `FS.Skia.UI.SkillSupport.Hud`)

Reserve a fixed HUD band; clamp gameplay to the remainder. Plain `float`s — **no
`Scene.Rect` dependency** (keeps SkillSupport dependency-light; consumer converts
to their geometry at the call site). Type names are authoritative in
`contracts/skillsupport-api.md` C2 (`BandEdge`/`Band`/`HudLayout`).

`reserveHudBand : surface: float -> bandSize: float -> edge: BandEdge -> HudLayout`

| Type | Shape | Notes |
|---|---|---|
| `BandEdge` | `Top` \| `Bottom` | which edge the reserved band occupies |
| `Band` | `{ Offset: float; Size: float }` | one region along the reserved axis |
| `HudLayout` | `{ HudBand: Band; Gameplay: Band }` | reserved band + clamped gameplay remainder |

Inputs: `surface` (full extent along the axis), `bandSize` (reserved HUD
thickness), `edge`. **Clamp invariant (tested):** `HudBand.Size = min bandSize
surface`, `Gameplay.Size = surface − HudBand.Size ≥ 0`, the two `Band`s are
non-overlapping and partition `surface`. Convention (skill text): **overdraw the
HUD last**.

---

## Relationships

- `EvidenceFormatSchema` is the single source for both the FR-005 failing-class
  **diagnostic** and the generated `evidence-formats.md` **reference** (D5).
- `RngState` and the `Hud` types (`BandEdge`/`Band`/`HudLayout`) are the only
  entities adding public `.fsi` surface (FR-010), hence the new
  `FS.Skia.UI.SkillSupport.txt` surface baseline and the Tier-1 escalation (FR-012).
- `SymbolSet`/`SymbolDiff` and `EffectiveHookDecision` are computed-and-reported,
  not persisted — diagnostics/guidance, not gates (D12).
