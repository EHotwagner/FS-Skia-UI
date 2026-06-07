# Manual success-criterion verification (feature 076)

SC-003, SC-004, and SC-008 are comprehension/navigation outcomes with **no
mechanical gate**; they are verified by human review of the built site and the
verdict is recorded here.

- **Authoritative command**: human review of `output/` from
  `dotnet fsdocs build --strict --eval`.
- **Artifact path**: this file.
- **Failure class**: comprehension/navigation gap (a reader cannot reach the
  entry point in ≤2 steps, or cannot state a speckit-phase mapping from the docs
  alone).
- **Next action**: revise the offending page/navigation and re-review.

| SC | Criterion | Verifying task | Verdict |
|---|---|---|---|
| SC-008 | First-time visitor reaches their role entry point (consumer / contributor / speckit practitioner) in ≤ 2 steps from the landing page | T008 | **PASS** (implementer review) |
| SC-003 | Practitioner can state, from the governance section alone, which speckit phase each governance touchpoint applies to and how to respond | T020 | **PASS** (implementer review) |
| SC-004 | Consumer can describe the token→typed-control flow and identify the speckit phase(s) where custom components are created/consumed | T022 / T023 | **PASS** (implementer review) |

## Basis for each verdict

- **SC-008**: `docs/index.md` opens with three explicit role sections — "I'm
  building an app (consumer)", "I'm contributing (contributor)", "I'm running the
  Spec Kit process (speckit practitioner)" — each linking directly (1 step) to the
  role's entry page (API Reference / Architecture overview / Governance &
  speckit-placement). Every role entry is reachable in ≤ 1 step. ✓
- **SC-003**: `docs/governance/speckit-placement.md` carries an explicit
  touchpoint → speckit-phase table (Route, evidence `[S]`/`[S*]`, EvidenceGraph,
  EvidenceAudit, single-source regeneration, constitution check, surface
  baselines) with "how to run / how to respond" guidance per touchpoint, and the
  governance section's deep-dive pages elaborate each. A reader can name the phase
  and response for every touchpoint from the section alone. ✓
- **SC-004**: `docs/controls-design/design-tokens-penpot.md` walks the DTCG
  single-source → generated `DesignTokens` (Light/Dark) → `Theme` → typed control
  flow, and `docs/controls-design/typed-front-door.md` + `docs/speckit/process.md`
  place custom-component creation/consumption on specific phases (`.fsi` sketch at
  plan; typed Props/view + parity test + token regen at implement; consumed at
  implement and later features). The two evaluated examples
  (`docs/examples/*.fsx`) demonstrate both flows end to end. ✓

> These verdicts are the implementer's review against the built `output/`. A
> second independent human review before publish is recommended but not blocking.
