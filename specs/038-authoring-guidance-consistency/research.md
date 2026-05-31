# Phase 0 Research: Authoring Guidance Consistency

Decisions resolving the Technical Context, grounded in the current repository.

## R1 — Skill-id resolution model (US1, FR-001/002/003)

**Decision.** Implement a repeatable resolution guard (in `build.fsx`, surfaced
through `GeneratedGuidanceCheck`) that:

1. Collects the **advertised-id set** from the hint / scan-phrase lines in
   `.agents/skills/speckit-tasks/SKILL.md` (and the `.claude/` mirror) — the
   `... -> <id>` mappings around lines 140–149 — plus the harness "available
   skills" surface.
2. Collects the **declared `name:` set** from every `SKILL.md` under
   `src/*/skill/`, `.agents/skills/*/`, `.claude/skills/*/`, and
   `template/fragments/*/skill/`.
3. **Fails** when: any advertised id has no matching declared `name:`; any
   skill's directory name, declared `name:`, and the id advertised for it
   disagree; or the `.agents/` and `.claude/` copies of a skill declare a
   different `name:` or advertise a different id (validated as synchronized
   peers, per the existing repo convention).

**Grounding.** `speckit-debug-loop` is advertised at
`.agents/skills/speckit-tasks/SKILL.md:145,149` and the `.claude/` mirror but no
skill declares that `name:`. `fs-skia-layout` (`src/Layout/skill`) and
`fs-skia-ui-widgets` (`src/Controls/skill`) **do** resolve, confirming the
author-reported dangling ids were generated-project artifacts; the durable
response is the class guard, not a one-off rename.

**Remediation.** Remove the `speckit-debug-loop` reference from both
`speckit-tasks/SKILL.md` copies — there is no debug-loop skill in the repository
to repoint to, so removal (not retarget) is correct.

**Alternatives considered.** Renaming a skill to `speckit-debug-loop` — rejected:
invents a capability that does not exist. A one-off lint of just this id —
rejected: does not cover the directory/`name:`/peer-drift class the spec
requires.

## R2 — Local API-reference form (US2, FR-004)

**Decision.** Bundle the **real public `.fsi` files verbatim** into generated
output under a `docs/api-surface/` tree, selecting the signatures per generated
**profile** from `template/capabilities.yml` — each capability declares its
`contracts:` (exact `.fsi` paths), and a profile declares its capability set.
The generated project therefore ships exactly the signatures for the packages it
references (e.g. an `app` project bundles Scene + SkiaViewer + Elmish +
KeyboardInput + Layout + Controls signatures; a `headless-scene` project bundles
only Scene, optionally Layout/Controls/Testing).

**Derivation is mechanical** (copy-at-generation-time from `src/.../*.fsi`),
never hand-maintained, satisfying the clarification that a derived summary is
acceptable only if generated verbatim and kept in lockstep. The check fails if a
referenced package's signatures are absent from the generated tree or drift from
the source `.fsi`.

**Grounding.** `runGenerateV3Products` does **not** currently copy any `.fsi`
into generated output, yet `docs/reports/generated-apps.md` already promises a
"source-shaped package API reference generated from curated `.fsi` files" — the
guidance describes an artifact that is not actually bundled. This decision makes
the promise real. `capabilities.yml` already maps capability → `contracts:` and
profile → capabilities, so no new mapping is invented.

**Alternatives considered.** A hand-written `api-surface.md` summary — rejected by
the clarification (must be generated verbatim). Pointing authors at the source
`src/.../*.fsi` paths — rejected: those paths do not exist in a generated project
(FR-005). Relying on `GenerateDocumentationFile` XML / DLL reflection — rejected:
that is the exact reflection workflow the feature removes.

## R3 — Name-collision blast radius (US3, FR-008)

**Decision.** Apply `[<RequireQualifiedAccess>]` consistently to the
collision-prone public surface:

- **`ViewerWindowStartupState`** (`src/SkiaViewer/SkiaViewer.fsi:43-48`) — its
  bare `Normal` case collides with a consumer's own `Normal`. Add RQA. This is
  the confirmed, in-scope breaking change.
- **`update`/`init`-bearing surfaces.** Enumerate every public surface a consumer
  could `open` such that a bare `update`/`init` would shadow their MVU
  `update`/`init`: `Viewer.init`/`update` (and the `*Run`/`*EvidenceWorkflow`
  variants) in `SkiaViewer.fsi`, `ElmishAdapter.init`/`update` in `Elmish.fsi`,
  and any input module value bindings in `KeyboardInput.fsi`. For each, determine
  whether it is **already safely module-qualified** (a consumer must write
  `Viewer.update`, so no shadow) or whether the module is `[<AutoOpen>]`/likely
  `open`ed and therefore needs RQA or a documented qualification rule. Apply the
  minimal consistent hardening — the spec requires "at minimum
  `ViewerWindowStartupState` and the viewer/input surfaces that expose
  `update`/`init`."

**Process (Tier 1).** Update `.fs` + `.fsi` together; refresh
`readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`, `FS.Skia.UI.Scene.txt`
(R4), the merged `FS.Skia.UI.txt`, and any input baseline via
`scripts/refresh-surface-baselines.fsx`; update **all generated samples** so a
freshly generated project compiles with the clean surface; record a migration
note and bump package versions on merge.

**Grounding.** `ViewerWindowStartupState.Normal` is bare at `SkiaViewer.fsi:45`.
The `Viewer.*` and `ElmishAdapter.*` `init`/`update` bindings live inside named
modules (qualified as `Viewer.update`, `ElmishAdapter.update`); the precise
collision risk depends on whether those modules are auto-opened — to be settled
during implementation against the actual `.fs`/`.fsi` attributes, applying RQA
only where a real shadow exists, consistently.

**Alternatives considered.** Guidance-only ("qualify these names"), as spec 035
chose for some Scene/Controls names — rejected here for `ViewerWindowStartupState`
because the clarification accepts a breaking change and the author hit the
collision in practice. Renaming `Normal` → `NormalWindow` — rejected: RQA is the
idiomatic, non-renaming fix already used by sibling DUs in the repo.

## R4 — Additive scene constructors (US6, FR-010)

**Decision.** Add self-describing, `Rect`-consistent constructors/helpers for
the tuple-heavy cases, **retaining** the existing positional DU cases and
helpers:

- `Rectangle of (float*float*float*float)*Color` and
  `Text of (float*float)*string*Color` (`Scene.fsi:322,332`) keep working.
- Add helpers in the `Scene` module consistent with the existing
  `rectangleWithPaint: bounds: Rect -> paint: Paint -> Scene` (`Scene.fsi:412`)
  and `text: position -> text -> color -> Scene` (`Scene.fsi:430`) — e.g. a
  `Rect`-based / named-argument rectangle and text constructor so the
  "tuple of length 5" arity slip the author hit twice is prevented or yields a
  clear error.

Helper functions are preferred over new DU cases (smaller surface, no pattern-
match-exhaustiveness churn for consumers); named-field DU additions are an
acceptable alternative if a constructor must be matchable. Both are additive.

**Grounding.** `PaintedRectangle of Rect*Paint` and `rectangleWithPaint` already
show the safe `Rect`-based pattern; the inconsistency is only on the positional
`Rectangle`/`Text`. `Scene.rectangle`/`Scene.text` helpers exist but still take
tuples. The fix aligns the tuple cases with the `Rect`/named-argument style.

**Alternatives considered.** Replacing the positional DU cases — rejected:
breaking, and the spec forbids removals (additive-only). A full Scene DU redesign
— out of scope per spec Assumptions.

## R5 — Demo-identifier neutralization (US4, FR-007)

**Decision.** The `app`-profile starter renders a Tetris-style demo (`Model.fs`
carries `Score`, `Level`, board-style state; `Tests.fs:430-434` asserts a
"Tetris-style board", `score`, `level`, `next`). Replace **game-title-specific**
identifiers (`Tetris`, `board`, `piece`, `score`, `level`) across the generated
starter app (`Model.fs`, `View.fs`, `EvidenceCommands.fs`, `LayoutEvidence.fs`)
and tests (`Tests.fs`) with domain-agnostic equivalents, **preserving the generic
game-starter shape** — a HUD region, a gameplay region, and a primary-interaction
counter — so `fs-skia-layout-evidence` (HUD/gameplay-region readiness) stays
meaningful and the starter still demonstrates real layout evidence.

Define a forbidden-identifier scan list (`tetris`, `score`, `level`, `next piece`,
`board`, `piece`) enforced by `GeneratedGuidanceCheck`/`TemplateCheck` against
generated output.

**Grounding.** "HUD region" and "gameplay region" are generic concepts already
named by the layout-evidence capability and the constitution's skill inventory;
only the game *title* and *scoring nouns* are demo-specific. Neutralizing nouns
(not the layout structure) keeps the starter runnable and the evidence valid.

**Alternatives considered.** Editing only `Tests.fs` while leaving the app a
Tetris demo — rejected: a domain-agnostic test cannot meaningfully assert against
a Tetris app; the demo nouns would remain in the generated project (SC-004
requires zero leftover demo identifiers). Replacing the starter with a
non-game app — rejected: removes the layout-evidence demonstration the framework
is built around and is broader than the spec requires.

## R6 — Canonical effects page + FR-011 regression guard

**Decision (FR-009).** Author one canonical page —
`template/base/docs/effects-boundary.md`, bundled into every generated project —
that names both effect categories (application commands at the MVU edge, e.g.
`DispatchHostCommand`, vs viewer effects at the host boundary, e.g. `OpenWindow`,
`RenderScene`, `CaptureScreenshot`), explains the boundary, and shows the
canonical `update`→host wiring (`Viewer.runApp viewerOptions
Product.Program.generatedHost`). Repoint/align the scattered framework-repo
mentions (`docs/reports/generated-apps.md`, `runtime-design.md`) to this page so
there is a single source of truth, reachable locally (SC-005) without reading
reports or source. `GeneratedGuidanceCheck` asserts the page is present in
generated output and covers both categories + the wiring.

**Decision (FR-011).** Add a regression guard (in the evidence path / `build.fsx`
gate) asserting the gates continue to resolve the audited feature from
`.specify/feature.json` and do **not** fire required evidence solely from a
filename mention in `tasks.md` — the behavior feature 037 established
(`build.fsx` resolves via `feature.json` and refuses placeholder fallback). The
guard echoes the resolved feature id and demonstrates that a bare filename
mention in a fixture does not trigger an evidence obligation. P3,
framework-repo-process only; must never block consumer work.

**Grounding.** Both effect categories already appear in
`docs/reports/generated-apps.md` ("App-command boundary" vs "Viewer effects")
but scattered, tetris-tagged, and not bundled into a generated project — failing
the "single page, local, no scattered reports" bar. Feature 037 already fixed the
targeting bug; FR-011 is a regression guard, not new behavior.

**Alternatives considered.** Leaving the effects content in `docs/reports/` —
rejected: not reachable from a generated project (violates SC-001/SC-005). Re-
implementing the 037 targeting fix — rejected: already shipped; only a guard is
in scope.
