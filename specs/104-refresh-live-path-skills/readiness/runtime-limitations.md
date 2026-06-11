# Runtime limitations + permanent non-goals — feature 104 (live-path skill currency, T002/T003)

## Supported runtime

Feature 104 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; these are out of
scope for the framework and therefore for this feature. Feature 104 adds **no** runtime code on any
host path: it is a documentation-currency (skill-honesty) pass over three Markdown skill files plus
the governance-generated `.claude/skills/**` mirror and `skillist-reference.md`. Zero behavior
change, so it is platform-independent and introduces no new runtime, window, GPU, or wall-clock
dependency.

## FR-008 / SC-005 zero-`.fsi`-delta cross-reference

Every claim the refreshed/new skills make *describes existing shipped code* (features 096–103 on
`main`) anchored to a verified source signature in `contracts/currency-claims.md`. No skill claim
motivates a source edit: the skills *describe* the existing `RetainedRender.fsi` / `Focus.fsi` /
`ControlRuntime.fsi` / `ControlsElmish.fsi` surface; they do not alter it. `git diff` touches **no**
`src/**/*.fsi` line and **no** `src/**/*.fs` line — the only `src/**` file edited is
`src/Controls/skill/SKILL.md` (Markdown). Tier 2; no surface baseline moves.

## Out of scope / permanent non-goals

- **Migrating the remaining skill-less packages** (`Color`, `Input`, `SkillSupport`) is deferred
  (spec A3). No full 36-skill corpus migration.
- **Any future supersession past feature 103** is deferred (spec A4) — the refresh is current
  *through 103*, the final live-path roadmap rung.
- **A consumer redesign of `fs-skia-viewer-host`** is out of scope — US3 adds only a cross-link to
  the new maintainer-facing host skill (spec A2), not a rewrite.
- **Permanent roadmap non-goals preserved**: no data binding, no `DataContext`, no
  dependency/attached properties, no CSS selectors, and no lookless template engine. The skills
  *describe* the shipped MVU-retained capabilities; they add none of these.

## Failure diagnostics

No new runtime failure path is introduced. Every edit is a skill-documentation change; no logic, no
`.fsi` signature, and no diagnostic message changes. The existing product suites stay green and
byte-identical, which is the evidence that no skill token was parsed as a behavior change (SC-004).
