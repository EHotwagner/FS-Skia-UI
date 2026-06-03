# Contract: skill-hint resolution

**Goal (FR-008)**: every skill id referenced by a bundled hint table resolves to
exactly one skill a generated consumer can register.

## Consumer-registerable skill set

Derived from `.template.config/template.json` `sources`:

- **Always (all profiles)**: the 25 canonical `.agents/skills/*` ids, plus the
  two new split skills `fs-skia-evidence-mode` and `fs-skia-layout-readability`
  (replacing `fs-skia-layout-evidence`).
- **Profile-conditional product skills**:
  - `app`: `fs-skia-scene`, `fs-skia-skiaviewer`, `fs-skia-elmish`,
    `fs-skia-keyboard-input`, `fs-skia-ui-widgets`
  - `headless-scene`: `fs-skia-scene`
  - `governed`: `fs-skia-scene`, `fs-skia-testing`
  - `sample-pack`: `fs-skia-scene`, `fs-skia-skiaviewer`, `fs-skia-elmish`,
    `fs-skia-samples`

There is **no** `fs-skia-layout` product skill in any profile.

## Resolution rule

A hint id is **valid** iff it is an always-present skill or a product skill that
*some* profile registers. A hint that names a profile-conditional skill MUST
annotate the profile(s) where it applies. Invalid (current) hints to fix:

| Current hint | Defect | Fix |
|--------------|--------|-----|
| `layout → fs-skia-layout` | no consumer registers `fs-skia-layout` | `layout → fs-skia-layout-readability` |
| `… evidence tasks → fs-skia-layout-evidence` | id removed by split | route evidence-mode → `fs-skia-evidence-mode`; HUD/readability → `fs-skia-layout-readability` |
| deps template example `skillist: ["fs-skia-layout"]` | unresolvable | use a real id, e.g. `["fs-skia-layout-readability"]` |

## Enforcement (governance test)

A `tests/Governance.Tests` check enumerates every skill id appearing in the
bundled hint tables (`.agents/skills/speckit-tasks/SKILL.md` hint sections and
the deps-template example) and asserts each is in the consumer-registerable set,
failing with the offending id and the nearest valid id. This makes FR-008
durable against future drift (SC-004).
