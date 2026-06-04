# Research & Design Decisions: Feature 062

Resolves every design choice the spec deferred to `/speckit-plan`. Each decision
records what was chosen, why, and the alternatives rejected. Grounded in the
post-061 source (template `0.1.84`, libs `0.1.65-preview.1`).

---

## D1 — Hook execution precedence rule (FR-001)

**Decision.** Define the rule as: **`settings.auto_execute_hooks` governs the
*mandatory* set only.** A hook with `optional: false` (mandatory) auto-runs when
`auto_execute_hooks: true` and is surfaced for confirmation when it is `false`. A
hook with `optional: true` is **always surfaced** ("To execute: `/command`") and
is **never force-run** by `auto_execute_hooks`. `condition`-guarded hooks are
never evaluated by the skill — evaluation is left to the executor, and the notice
reports the resolved decision rather than forcing a run. The genuinely
always-needed hook (feedback capture) is made mandatory (D-feedback below) so it
auto-fires without relying on `auto_execute_hooks` to override the optional
default.

**Rationale.** This is the only reading that makes the two pulls non-conflicting:
optional side-effecting hooks (git commit) keep a human in the loop, while the one
hook that must always run is promoted out of the optional class entirely — exactly
the 2026-06-04 clarification. It needs no per-hook special-casing and no clarifying
round-trip (SC-001). It matches the existing skill wording (optionals are
"surfaced") rather than contradicting it.

**Alternatives rejected.** (a) *`auto_execute_hooks: true` overrides everything,
auto-running optionals* — would silently `git commit` without consent, the
opposite of the surface-for-consent intent. (b) *Per-hook `auto_execute` flags* —
more surface, more drift, and the clarification already scoped the fix to the
feedback hook.

**Feedback-hook promotion.** Flip all six `optional: true` → `optional: false` in
`template/feedback/extensions/feedback.yml` (`after_specify/clarify/plan/tasks/
analyze/implement`). This is the canonical source copied into generated projects
via `.template.config/template.json` (`template/feedback/extensions/` →
`.specify/extensions/feedback/` under `--feedback true`). The feedback extension
is **not installed in this repo**, so the promotion is verified in the template /
a generated project, not locally (see Assumptions).

---

## D2 — Effective merged hook-set notice (FR-002)

**Decision.** Each phase skill's hook step, after multi-file discovery + dedup by
`(extension, command)`, emits **one consolidated notice** listing every effective
hook for the phase with its resolved decision: `auto-run` (mandatory +
`auto_execute_hooks: true`), `surfaced` (optional), `skipped (disabled)`
(`enabled: false`), or `condition-deferred` (non-empty `condition`, left to the
executor). The now-mandatory feedback hook appears as `auto-run`, never as a
surfaced optional. Delivered as **guidance** (skill prose), not a new gate.

**Rationale.** Removes the manual reconciliation that SI-5 flagged; the operator
reads one table instead of merging files by hand. It is the natural presentation
layer over the D1 rule and the already-shipped 061 multi-file discovery.

**Alternatives rejected.** A compiled "effective-hooks" emitter gate — over-built
for a presentation concern, and the discovery logic is already specified in prose;
a gate would duplicate it and risk drift. (A low-cost *regression* check that the
feedback hook stays `optional: false` is added under D-checks — that is cheap and
worthwhile; rendering the notice is not.)

---

## D3 — Durable-vs-replaceable source map (FR-003, folds SI-9)

**Decision.** Ship a single hand-authored generated doc
`template/base/docs/scaffold-map.md` (peer of `product.md` /
`effects-boundary.md`). It names: which `src/**/*.fs` files are durable vs
replaceable on a scaffold-model swap; that `GovernanceTests.fs` is
durable/model-agnostic while `BehaviorTests.fs` is the replaceable scaffold suite
(lifting the truth out of the in-file Feature-060 comments in
`tests/Product.Tests/GovernanceTests.fs` / `BehaviorTests.fs` into a discoverable
page); the source-text scan strings that must survive a swap; and a **pre-design
pointer** to the `fs-skia-scene` "Common pitfalls" record-label-collision note
(SI-9). A one-line cross-reference is added from `fs-skia-project` /
`fs-skia-layout-readability` so the map is reachable from the skills an author
already loads.

**Rationale.** SI-2's cost was reading six `.fs` files + both test files + several
docs before it was safe to change anything; a single map collapses that. SI-9 is
"already covered" — the only gap is *discoverability before designing records*, so
folding it as a pre-design pointer (not a new pitfall) is the minimal fix.

**Alternatives rejected.** A new skill — heavier than warranted; the content is a
reference page, and FR-011 candidate #2's full "simulation core" skill is
explicitly deferred (D11). Generating the map mechanically from the source tree —
the durable/replaceable split is an editorial judgment, not a derivable fact, so a
hand-authored page is honest; `GeneratedProductCheck`/`TemplateCheck` keep it
present and substituted correctly.

---

## D4 — `Dev` self-describing output (FR-004)

**Decision.** Extend the `Dev` target's emitted output in
`build/Governance/Engine/Update.fs` (`StartTarget Targets.Dev`): the
`dev-verdict.txt` message and a console line both state that **`Dev` writes
logs/markers and does not compile**, and that **`Test`/`Verify` (`dotnet test`) is
the authoritative compile/test path**.

**Rationale.** SI-3 is a re-confirmed footgun: 061 FR-006 fixed the *docs*; the
residual is surfacing the caveat from the target's *own output*. The change is a
string in the data the target already writes — no new abstraction (Principle III),
and it directly improves observability (Principle VII).

**Alternatives rejected.** A separate diagnostic file — redundant; the verdict
file and console are where an operator already looks.

---

## D5 — Evidence-format recoverability (FR-005, folds skill-gap #5)

**Decision.** Two complementary mechanisms, both single-sourced from the constants
that already enforce each rule (so neither can drift):

1. **Diagnostics print the full per-file schema** for every failing format class,
   extending the proven 061 pattern (the `Required` token list on the
   readiness-contract scan in `Scans.fs`). Add the same "print the complete
   required shape" treatment to: `skill-loading-evidence.md` (the 8-column table —
   one row per (task,skill), the `loaded_at < work_started_at` ordering rule, the
   resolved `.agents/skills/<id>/SKILL.md` path) in `Audit.fs`; window-visibility /
   `diagnostic-class` rows in `Scans.fs`; and the SEH tokens (`accepted-seh`,
   `synthetic-error-handling-approved`, no backticks) in `TaskParser.fs`.
2. **A generated reference page** `template/base/docs/evidence-formats.md`,
   emitted from the **same schema constants** (ApiSurfaceGen-style generation +
   currency check), so an author can get every file's required shape **before**
   triggering a failure.

**Rationale.** SI-7 (consumer-rated highest-impact) had the consumer run
`strings -el FS.Skia.UI.Build.dll`. Diagnostics-on-failure satisfy SC-002's
"recoverable from the audit/graph output," but authors want the shape *up front*;
the generated, currency-checked reference gives that without a second source of
truth. The spec allows either or both and checks the outcome — both is the
strongest, and single-sourcing keeps it honest.

**Alternatives rejected.** A hand-maintained reference doc — guaranteed to drift
from the enforcing constants; rejected on the repo's "generated from a single
source, not hand-synced" principle. Diagnostics-only — leaves the
author-before-failure gap SI-7 calls the real barrier.

---

## D6 — `skillist`/`owns:` quick reference (FR-006, folds skill-gap #3a)

**Decision.** Generate `template/base/docs/skillist-reference.md` from the live
`SKILL.md` registry (the same `SkillRegistry` the gates already use), listing the
valid `skillist` ids — **resolving the directory-name-vs-`name:` distinction** so
authors never `grep '^name:'` each file — and the closed `owns:`→implied-skill
table. Currency-enforced (folded into `TargetMetadataDrift` or a sibling currency
check, matching the api-surface generation pattern).

**Rationale.** SI-4a's cost was reverse-engineering the id set and the `owns:`
vocabulary from prose/grep. Generating from the registry makes the reference
authoritative and self-updating.

**Alternatives rejected.** Documenting the ids by hand — drifts as skills are
added/renamed (058/059/060 all churned the registry). A runtime resolver command
only — useful but doesn't give the at-a-glance table; the generated page is
cheaper to read and is itself currency-checked.

---

## D7 — `EvidenceGraph` effective-DAG render (FR-007, folds skill-gap #3b)

**Decision.** Extend `Render.taskGraphMd` / `renderMermaid` to render the
**effective** DAG — explicit deps **and** the auto-injected Phase N+1 → Phase N
checkpoint edges (already computed into `PhaseDeps` by `TaskParser.fs` and already
in `Graph.allDeps`), with the **injected edges distinctly labeled** (e.g. a
dashed/`%%`-annotated edge or a separate "injected checkpoint edges" subsection) —
and print the **resolved `skillist`-id set**. Surfaced in `EvidenceGraph` output
alongside the existing 061 `graphVerdictLine`; not a new gate.

**Rationale.** SI-4b's "write-then-run-and-hope" loop exists because the injected
edges are invisible until a full run. The data is already present; the only gap is
*rendering it legibly with the injected edges marked* and echoing the resolved
skillist set so the author reviews the effective graph before trusting it.

**Alternatives rejected.** Injecting the edges into `tasks.deps.yml` on disk —
would conflate authored vs derived structure and churn the file; keeping injection
in-memory and only *rendering* it preserves the single authored source.

---

## D8 — Mechanical cross-artifact symbol consistency (FR-008, folds skill-gap #4)

**Decision.** Implement a **compiled, deterministic symbol set-difference** rather
than agent eyeballing. Add a small pure function (SkillSupport `Parsing` family or
a focused governance helper) that extracts named symbols from `plan.md`,
`data-model.md`, and `tasks.md` — `Msg` cases, union/`Screen` variants, entity
record names, and FR-/SC- IDs — and reports set-differences (a symbol present in a
proper subset of the three). `speckit-analyze` gains a new **detection pass G**
that runs/interprets it and reports findings; intentionally design-only symbols
are reported for **human judgment, not hard-failed** (edge case in the spec).
Delivered as diagnostics/guidance, unit-tested, not a hard merge gate.

**Rationale.** SI-6's highest-value finds (`ViewerKeyEventReceived` in
data-model+tasks but not plan; an `Initial` start-state in design but not a spec
FR) were caught only by close reading. "Mechanical/deterministic" is the explicit
ask, so a compiled set-diff (testable, reusable in generated projects since
SkillSupport ships there) beats prose instructions that vary run-to-run.

**Alternatives rejected.** Pure agent-prose pass — non-deterministic, exactly the
"found by eyeballing" failure mode. A hard gate — the design-only edge case would
produce false failures; guidance with human judgment is correct here.

---

## D9 — `Result.Ok`/`Result.Error` shadowing pitfall (FR-009)

**Decision.** Create a "Common pitfalls" section in the canonical
`fs-skia-skiaviewer` skill (the source skill currently has none) with a one-line
entry: `open FS.Skia.UI.SkiaViewer` brings `ViewerDiagnosticLevel.Error` (and
peers) into scope, so bare `Ok`/`Error` bind to the union case instead of
`Result`; remedy — qualify as `Result.Ok`/`Result.Error`. Cross-reference the
existing `Unknown`-collision note (which lives in the `fs-skia-keyboard-input`
skill) rather than moving it.

**Rationale.** SI-8 is a one-line addition; renaming any DU case is explicitly out
of scope. Cross-referencing the existing `Unknown` note keeps both notes near
their owning skill without restructuring.

**Alternatives rejected.** Moving the `Unknown` note into skiaviewer — touches two
skills and their sync for no benefit; a cross-reference is enough.

---

## D10 — FR-010 per-helper ship decisions (folds skill-gap #2, partial)

**Decision — SHIP both leading helpers into `FS.Skia.UI.SkillSupport`** (the only
Tier-1 escalation):

- **`Random` module** — `seedRng: uint64 -> RngState`, `nextRng: RngState ->
  uint64 * RngState`, `nextBelow: int -> RngState -> int * RngState`. **splitmix64**
  to expand the seed, **xorshift64** for the stream; pure threading (`state ->
  (value, nextState)`), **no ambient `System.Random`**, so a consumer's `update`
  stays pure and replayable. Internal `mutable` only inside the step function,
  disclosed at the use site.
- **`Hud` module** — `reserveHudBand`: given the surface height (or full
  `float`-rect dimensions) and a band size/edge, return the reserved HUD band and
  the clamped gameplay remainder as plain `float` values, with the documented
  "overdraw HUD last" convention in the skill text.

**Home = SkillSupport, surface kept dependency-light.** SkillSupport is currently
authoring-scoped (graph/parsing/globbing/codegen/shell-process) and ships
unconditionally to the template (`IsPackable`, pinned in
`template/base/Directory.Packages.props`), so it is a working, already-consumable
home — and the spec names it. To avoid pulling `FS.Skia.UI.Scene`/`Layout` into
SkillSupport's dependency set, `reserveHudBand` takes/returns plain `float`s rather
than `Scene.Rect`; the consumer converts to their geometry type at the call site
(consistent with the `fs-skia-scene` record-label pitfall guidance). A new
per-package surface baseline `readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt`
is created (none exists today) and updated with the `.fsi` in the same change-set
(FR-012, Principle II). A skill reference is added (see D11).

**Decision — DOCUMENT-with-rationale (defer ship):** the fixed-step accumulator,
collision/reflection, and paddle-rebound candidates stay documented conventions in
`fs-skia-elmish` / `fs-skia-layout-readability`. **Rationale:** unlike the seeded
RNG and HUD band (re-implemented across Asteroids→Breakout→SpaceInvaders, three
demos), these have not yet shown the same cross-demo recurrence pattern, are more
shape-variable per game (different collision geometries), and shipping them now
would broaden SkillSupport's surface ahead of demonstrated demand. They ship on a
later recurrence; the decision is recorded per SC-006.

**Rationale (ship the two).** Three consecutive demos re-implementing the same two
primitives is the spec's stated bar for escalating 060 FR-008 / 061 FR-011 D8 from
documented to shipped; the seeded RNG is also a prerequisite for FR-023-style
deterministic replay.

**Alternatives rejected.** (a) *Document a third time* — the spec explicitly leans
ship on 3rd recurrence; documenting again repeats the friction. (b) *Ship into
Scene/Layout* — pulls those package baselines into scope and couples SkillSupport
to runtime geometry; the spec names SkillSupport and the `float` API keeps it
decoupled. (c) *Ship all five helpers* — over-reach beyond demonstrated
recurrence; the three deferred are recorded, not dropped.

---

## D11 — Disposition of the five fourth-prompt skill-gap candidates (FR-011)

Each candidate is dispositioned; none is silently dropped, and each is findable by
family/topic:

1. **Spec Kit hook execution policy** → **FOLDED into FR-001/002** (the precedence
   rule + effective-hooks notice in every phase skill). No new skill.
2. **Generated game simulation core** → **PARTIALLY SHIPPED + DEFERRED.** The
   seeded RNG ships (D10/FR-010) with a skill reference; `reserveHudBand` ships;
   the held-key-continuous-movement / fixed-step accumulator / documented
   collision-resolution-order / bounded-headless-evidence loop stays **documented**
   in `fs-skia-elmish` + `fs-skia-layout-readability`, with the full standalone
   "simulation core" skill **explicitly deferred** (rationale: D10 — not yet at the
   3-demo recurrence bar for the loop primitives; ship on recurrence). The SI-2
   durable-vs-replaceable map (D3) is its companion reference.
3. **Speckit task-graph linter/explainer** → **FOLDED into FR-006** (the generated
   `skillist`/`owns:` reference resolving id/`name:` and the closed vocabulary)
   **and FR-007** (the effective-DAG render with injected edges). No new skill.
4. **Cross-artifact symbol consistency** → **FOLDED into FR-008** (the compiled
   symbol set-diff + analyze detection pass G). No new skill.
5. **Speckit evidence-format authoring** → **FOLDED into FR-005** (per-class
   schema-printing diagnostics + the generated `evidence-formats.md` reference).
   No new skill.

**Where helper-skill references land (D10 skill reference).** The shipped RNG +
HUD band are referenced from `fs-skia-layout-readability` (HUD/gameplay-region
owner) and `fs-skia-elmish` (pure-`update` threading owner), pointing at the new
`FS.Skia.UI.SkillSupport.Random` / `.Hud` surface — so a consumer building an
arcade demo finds them before re-implementing.

---

## D12 — Guidance vs. gate, and the low-cost checks (FR-001/006/008; spec assumptions)

**Decision.** Deliver the effective-hooks notice (FR-002), the symbol cross-check
(FR-008), and the effective-DAG render (FR-007) as **diagnostics/guidance, not new
hard merge gates** (per spec assumptions — a hard symbol gate would false-fail on
intentionally design-only symbols). Add the following **low-cost executable
checks**, which are cheap and prevent regression:

- A check that `template/feedback/extensions/feedback.yml` hooks are all
  `optional: false` (FR-001 regression guard), folded into `TemplateCheck` /
  `GeneratedGuidanceCheck`.
- **Currency checks** for the three generated docs
  (`evidence-formats.md` FR-005, `skillist-reference.md` FR-006) since they are
  generated-from-source and must not drift — matching the api-surface generation
  pattern (`TargetMetadataDrift`-style).

**Rationale.** Generated artifacts must be currency-checked or they rot; a static
`optional: false` assertion is one comparison. Everything else stays guidance so
intentional human judgment (design-only symbols, optional-hook consent) is
preserved.

---

## Resolved unknowns summary

| Spec deferral | Resolution |
|---|---|
| FR-001 precedence (which wins) | D1 — `auto_execute_hooks` scopes mandatory set only; optionals always surfaced; feedback → mandatory |
| FR-002 gate vs guidance | D2/D12 — guidance notice; low-cost feedback-hook regression check only |
| FR-005 mechanism (diagnostics vs reference vs both) | D5 — both, single-sourced from enforcing constants |
| FR-006 mechanism | D6 — generated from live registry, currency-checked |
| FR-007 render scope | D7 — effective DAG, injected edges labeled, skillist set printed |
| FR-008 compiled vs agent-prose; gate vs guidance | D8/D12 — compiled set-diff, analyze pass G, guidance not gate |
| FR-010 per-helper ship/document; package home | D10 — ship RNG + reserveHudBand into SkillSupport (float API), new baseline; defer the other three with rationale |
| FR-011 five candidates | D11 — all folded into existing FRs; full "simulation core" skill deferred with rationale |
