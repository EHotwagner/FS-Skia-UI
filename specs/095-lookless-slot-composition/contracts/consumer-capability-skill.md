# Contract: Consumer Capability Skill (E1–E5 guidance, FR-010 / FR-011)

The consumer-facing deliverable folded into feature 095: the two **shipping** consumer skills
are expanded to teach the full landed **E1–E5** architecture-evolution surface with runnable
examples. This contract pins *what each skill must contain* and *which gates verify it*. It is
the channel by which a `dotnet new fs-skia-ui` consumer (and an agent authoring against the
package) discovers capabilities the framework already has.

## Files (the only two edited; no new `.agents/skills/<id>`)

| File | Skill id | Audience / channel |
|---|---|---|
| `src/Controls/skill/SKILL.md` | `fs-skia-ui-widgets` | package-owned; agent/package-facing |
| `template/fragments/controls/skill/SKILL.md` | `fs-skia-generated-controls-guidance` | template fragment; selected into a generated project's Controls capability (reaches the real `dotnet new` consumer, SC-009) |

The `.claude/skills/**` peers are **regenerated** from the canonical `.agents` tree via
`RefreshSurfaceBaselines`, never hand-edited (`SkillSyncCheck`).

## Required content — each skill names + shows a runnable example for all five rungs

| Rung | Capability | Must teach (runnable consumer example) | Honesty constraint |
|---|---|---|---|
| E1 | Live event dispatch | a control's `onClick`/event binding dispatches a `'msg` through the consumer's single `update` (flat per-`ControlId`) | events dispatch into the one `update`; no second channel |
| E2 | Retained identity | why a control's focus/text **survives** a sibling-shifting re-render, and how to key a control for stable identity | identity is a property of the **keyed tree**, *not* a binding |
| E3 | Style class / visual state | attach a typed `StyleVariant` / free-form `Custom` class and how `VisualState` drives the look via the single resolver (closed precedence base < classes < state) | resolves to token-derived values; no CSS selectors/cascade |
| E4 | Focus / keyboard traversal | a control is focusable and reachable in Tab order; how keys route | focus model, not data binding |
| E5 | Slot composition | fill a control's **named** slot (`Button.Leading/Trailing`, `Panel.Header/Footer`) with a `Widget<'msg>` to re-skin its shape | a slot lowers to `Control<'msg>`; it is **not** a data-bound template; unfilled = today's chrome |

Each example MUST be runnable (compiles against the public `FS.Skia.UI.Controls` /
`FS.Skia.UI.Controls.Typed` surface), and the guidance MUST be honest per the constraints above
(the E1 lesson: the doc must match the code).

## Verification gates (all green)

| Gate | Checks |
|---|---|
| `SkillSyncCheck` | the `.claude` peer matches the canonical `.agents` source (regenerated, not hand-edited) |
| `SkillQualityCheck` | the rubric passes (literal substring/heading detectors — the mandate phrasing is one-line) |
| `GeneratedGuidanceCheck` | generated-guidance currency is green |
| generated-project check | a project that selects the Controls capability receives the updated E1–E5 guidance (SC-009) |

## Guarantees

| ID | Guarantee | How |
|---|---|---|
| CS-1 | All five rungs E1–E5 are named with a runnable consumer example in **both** skills | content edit; verified by inspection + `SkillQualityCheck` (SC-008) |
| CS-2 | Guidance is **honest** — a slot is not a data-bound template; retained identity is not a binding | honesty constraints above; the E1 contract-matches-code lesson |
| CS-3 | **No** Principle V synthetic-evidence disclosure on the skill content | E1–E5 are all shipped by this feature (FR-009) — every documented rung is real |
| CS-4 | Reaches a real `dotnet new fs-skia-ui` consumer, not only the repo agent | the template-fragment skill is selected into the generated Controls capability (SC-009) |
| CS-5 | No new `.agents/skills/<id>` governance skill | that audience does not ship into generated products (FR-011) |
