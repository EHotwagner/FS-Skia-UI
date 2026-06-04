# Research: Breakout-Demo Consumer Friction Follow-ups & Feedback-Prompt Expansion

**Feature**: `061-breakout-consumer-friction-followups`
**Date**: 2026-06-04

This feature consolidates BreakoutDemo2 consumer feedback (BD-1…BD-8) plus the
new fourth feedback prompt. Most FRs have a single obvious implementation; this
document resolves the decisions the spec explicitly defers to planning (FR-004
mechanism, FR-005 spelling, FR-011 per-helper ship-vs-document) and the
cross-cutting "duplicate template" question discovered during grounding.

All file locations below were confirmed against the current merged state
(post-060, `7d4a06d`) by source inspection.

---

## D1 — Multi-file hook discovery (FR-001, FR-002)

**Decision.** Rewrite the "Check for extension hooks" block in **every**
`/speckit-*` phase skill so discovery is multi-file: after reading
`.specify/extensions.yml`, also enumerate `.specify/extensions/*/*.yml`, parse
each, and **merge** all `hooks.<before|after>_<phase>` entries deduped by
`(extension, command)`. Edit only the canonical `.agents/skills/speckit-*/SKILL.md`
sources; regenerate `.claude/**` via `RefreshSurfaceBaselines`. Add FR-002's
one-line "optional hook registered but not run" notice to the same block.

**Rationale.** Grounding confirmed the gap exactly as the spec describes: every
phase skill (`speckit-{specify,clarify,plan,tasks,analyze,checklist,implement}`)
documents a **single-file** scan of `.specify/extensions.yml`, while the template
ships the feedback hook **separately** under
`template/feedback/extensions/feedback.yml` (→ generated project
`.specify/extensions/feedback/feedback.yml`). A hook in the per-extension file is
therefore invisible, and being `optional: true` the omission is silent. The fix
must live in the prose the skill executes, because these skills are interpreted
instructions, not code.

**Dedup key & double-run safety.** `(extension, command)` is the dedup key (an
edge case in the spec). If the same hook appears in both `.specify/extensions.yml`
and a per-extension file, it is run once. This matches the existing single-file
semantics for projects that only use the central file.

**Alternatives considered.**
- *Move the feedback hook into the central `.specify/extensions.yml`.* Rejected:
  the central file is owned by the Spec Kit core, the feedback extension is
  opt-in (`--feedback true`) and self-contained; co-locating it breaks the
  extension's isolation and the `--feedback false` story. The discovery side is
  the correct place to fix (the spec's BD-1 framing).
- *A compiled gate that fails when a hook is undiscovered.* Rejected for FR-001
  (the discovery is skill prose, not buildable behavior) but see D6 for the
  low-cost executable check that protects the **prompt-count** invariant.

---

## D2 — Readiness-contract discoverability (FR-004)

**Decision.** Satisfy FR-004 by the **audit-prints-schema** path (primary),
optionally backstopped by extending the shipped template set. Concretely: when
`EvidenceAudit`'s readiness-contract scan records a missing/partial file, the
emitted hit MUST carry the **full required shape** for that file — exact file
name, the complete required-token list, required fields, and (where applicable)
the required table header — and the audit front-end MUST print that shape per
failing file, not just a count. The grammar is already data in
`build/Governance/Evidence/Scans.fs`; we surface it instead of hiding it.

**Rationale.** The grammar already exists as F# data:
- `Scans.fs` enumerates required files and their literal tokens — e.g.
  `governance-risk-levels.md` → `["small"; "medium"; "broad"; "required
  evidence"; "broad validation"]`; `aggregate-hang-diagnostics.md` → `["verdict";
  "stage"; "elapsed duration"; "last observed command"; "focused rerun";
  "non-authoritative aggregate"]`; `runtime-limitations.md`,
  `supported-host-persistent-launch.txt` (field list), and the window-visibility
  file set (`window-state-diagnostics.md` etc.).
- Each missing-file hit already constructs `MissingTerms = Some terms` — the
  required tokens are **in hand** at the point of failure (`Scans.fs` ~lines
  85–95). Today the front-end (`build/Governance/Front/Governance.fs`) collapses
  this to `readiness-contract-hits=%d`. The fix is to **print what we already
  computed**: the per-file name + `MissingTerms` (+ required fields/table header).

This is the lowest-risk, highest-impact satisfier: it requires no new generated
artifacts in consumer projects, makes the requirement self-describing at the
exact moment a consumer hits it, and structurally guarantees the printout can
never drift from the enforced grammar (single source = the same `terms` list).

**Why not "ship templates for every readiness file" as the primary?** The engine
ships exactly one template (`skill-loading-evidence.template.md`) and the spec's
BD-2 names the asymmetry as the bug. Generating a full template set is viable
(FR-004's alternative satisfier) but: (a) it adds N new generated files to every
project whether or not the consumer ever fails that file; (b) the templates would
themselves need a currency check against `Scans.fs` to avoid drift; (c) the
audit-prints-schema path subsumes it — a consumer who can read the required shape
from the failing diagnostic does not need the template. **Backstop (optional,
low-cost):** if a template proves cheap, extend the existing
`skill-loading-evidence.template.md` emitter to also drop a
`readiness-contract.template.md` index listing each file + its required shape,
generated **from the same `Scans.fs` data** so it cannot drift. SC-003 checks the
*outcome* (passing `EvidenceAudit` reachable without decompiling), not the
mechanism.

**Alternatives considered.** Decompile-the-DLL (status quo, rejected — it is the
bug); copy-a-sibling (rejected — the spec's "no sibling" test case).

---

## D3 — Defect-class spelling (FR-005)

**Decision.** Resolve to a **single literal spelling**: `product-defect`, used by
both the readiness audit and any source governance scan. Grounding found the
readiness audit requires the literal `product-defect`
(`Scans.fs` `requiredClasses = [... "product-defect"]`, matched as
`diagnostic-class=product-defect` / `failure-class=product-defect`), while the
project-name-prefixed `<project>-defect` (`breakoutdemo2-defect`) spelling came
from a **source diff-scan expectation**, not from a second hard-coded literal in
`audit-patterns.yml` (which carries only generic syntax patterns: todo, mocks,
skipped tests). The fix is to ensure the **one** authoritative spelling is
`product-defect` everywhere a consumer must type it, and to confirm no governance
rule requires the project-prefixed form.

**Rationale.** A consumer should type the defect class once, the same way, in the
one file that needs it (`window-state-diagnostics.md`). `product-defect` is
project-agnostic (correct for a generated template — it must not bake in the
project name) and is already the audited literal. If a residual source-scan rule
or doc anywhere expects `<project>-defect`, that is the side to correct/remove.

**Validation step (planning → tasks).** A discovery task confirms whether any
rule, template, doc, or test still requires the project-prefixed spelling; if
found it is changed to `product-defect`; if it turns out the two are genuinely
distinct concepts (unlikely per BD-2a) the distinction is documented at both
sites. SC-004 accepts either "one spelling" or "documented-as-distinct".

**Alternatives considered.** Standardize on `<project>-defect` (rejected — bakes
the project name into a generated/template contract and into the
project-agnostic `Scans.fs` literal); leave both (rejected — the whipsaw the spec
calls out).

---

## D4 — `EvidenceGraph` terminal verdict line (FR-007)

**Decision.** Add an explicit terminal verdict line to the `EvidenceGraph`
in-process output, e.g.
`verdict=ok (no cycles, no dangling refs, no [S*])` on success and a
corresponding `verdict=error (...)` on failure.

**Rationale.** Grounding located the emitter in
`build/Governance/Front/Governance.fs` (the
`=== speckit.evidence.graph (in-process) ===` block) where `status` is already
computed from `gr.Verdict` (`Ok` → `"ok"`). It currently prints
`feature` / `tasks` / `verdict:`; BD-7 is that a clean pass has no self-evident
terminal line and must be inferred from exit code 0. Append a single, greppable
`verdict=ok (...)` token so success reads at a glance. Keep it consistent with
`EvidenceAudit`'s existing `verdict=PASS|FAIL` token style for cross-target
symmetry.

**Note on existing output.** The current block already prints `verdict: ok` —
the spec's BD-7 was filed against the consumer's run which printed only
`feature-source`/`feature-directory`/`tasks=N`. Planning treats FR-007 as
"guarantee a single, unambiguous, greppable terminal `verdict=…` token with the
clean-pass reasons inline", reconciling whatever the consumer actually saw with
the current emitter, and a governance test pins the token.

---

## D5 — Authoring-template self-description & duplication (FR-008, FR-009, FR-006)

**Decision.** Treat the **`fsharp-opinionated` preset copy as authoritative** for
plan/tasks templates; the generic `.specify/templates/*` copy carries a one-line
pointer to it (FR-009 for tasks; apply the same discipline to plan). Edits:

- **FR-008** — add the `GeneratedGuidanceCheck` pass-criteria as an inline
  template comment inside the *Repository Governance Decisions* block of the
  plan template the author edits (no empty/boilerplate/`NEEDS
  CLARIFICATION`/`TODO`; `N/A`-with-rationale counts as filled).
- **FR-009** — name the exact preset-relative paths
  (`.specify/presets/fsharp-opinionated/templates/tasks-template.md` and
  `…/tasks-deps-template.yml`) in `speckit-tasks` SKILL.md, and add a one-line
  "authoritative copy lives at … — edit there" pointer to the generic
  `.specify/templates/tasks-template.md`.
- **FR-006** — in the generated quickstart/tasks guidance, state that `Dev` is a
  completion-marker / log-writer target (`readiness/logs/Dev.txt`, no real
  compile feedback) and that `Test`/`Verify` (`dotnet test`) is the
  authoritative compile/test path. Sites: `template/base/README.md`,
  `template/base/docs/product.md`, and the tasks-template "build" guidance.

**Rationale (correcting a grounding miss).** The two template copies are **not**
identical — they already differ: the generic `.specify/templates/*` copies carry
generated constitution-fragment blocks (spliced by `RefreshSurfaceBaselines`),
the preset copies do not. Since this repo uses the `fsharp-opinionated` preset
and FR-009 explicitly says the generic copy should *point at* the preset copy,
the preset copy is the natural authoritative source. Where a block is
generation-owned (constitution fragments), edits go through the generator, not by
hand; the FR-008 pass-criteria comment is author-facing prose and is added to the
template body directly (and kept in sync across both copies, with the generic one
also bearing the pointer).

**Open confirmation for tasks phase.** Confirm exactly which template
`setup-plan.sh` / `setup-tasks` copy from for a `fsharp-opinionated` project so
the FR-008 comment lands in the copy a consumer actually instantiates. If both
are reachable, both get the comment.

**Alternatives considered.** Delete one duplicate (rejected — the generic copy is
the Spec Kit fallback for non-preset projects; removing it changes Spec Kit core
behavior, out of scope). Generate one from the other (possible future cleanup;
out of scope here — FR-009 only asks for the pointer).

---

## D6 — Prompt-count consistency for the fourth feedback prompt (FR-003)

**Decision.** Land the 3→4 prompt expansion across **all** coupled surfaces in
one change and pin the count with a low-cost check. Surfaces (grounded):
1. `template/feedback/skill/SKILL.md` — prompts + record schema (already shows
   the four-prompt wording and a `## Skill gaps` section in the working tree;
   verify/finish).
2. `specs/058-skills-quality-feedback/contracts/feedback-capture.md` — the
   sourcing contract (already updated in the working tree; verify the attribution
   note credits 061).
3. **Stale "three prompts" references to fix** (grounded):
   `specs/058-skills-quality-feedback/spec.md`,
   `specs/058-skills-quality-feedback/readiness/template-feedback-true.md`
   (T032 evidence captured "three exact prompts"),
   `specs/058-skills-quality-feedback/{research.md,plan.md,tasks.md,readiness/task-graph.*}`.
4. Governance: `build/Governance/SkillQuality.fs` includes the feedback skill in
   the quality-bar scope but does **not** assert the prompt count today.

**Rationale.** BD/USER ask is purely additive but spans a doc cluster; an
unsynced "three prompts" reference is exactly the silent inconsistency SC-002
forbids ("no surviving 'three prompts' reference"). The 058 contract is the
single source the skill is "sourced from"; updating historical 058 spec/readiness
prose to say "four (skill-gaps added by 061)" keeps the audit trail honest
without rewriting 058's outcome.

**Executable check (low-cost, satisfies the FR-003 "test/governance assertion"
clause).** Add a `TemplateCheck`/`GeneratedProductCheck`-adjacent assertion (or a
small `SkillQuality.fs` addition) that the generated feedback skill enumerates
exactly four `1.`–`4.` prompts and that the record schema contains the
`## Skill gaps` section — so a future drop of the prompt fails a gate. This is the
"low-cost executable check" the spec's Assumptions allow.

---

## D7 — Consumer-internal duplicate-DU pitfalls note (FR-010)

**Decision.** **Extend** the existing note (do not re-add). Grounding located it
in `template/product-skills/fs-skia-keyboard-input/SKILL.md` ("Common pitfalls" →
"Duplicate DU case names across co-opened modules", with the
`ViewerKey.Unknown` vs `ViewerRunBlockedStage.Unknown` framework-vs-framework
example). Add a second bullet/paragraph covering the **consumer's own** two DUs
(`GameMode.Launch` vs `Msg.Launch`) — where a bare `Launch` binds to the
last-declared type and yields misleading "expected GameMode but has type Msg"
errors — with the fully-qualified resolution.

**Canonical-source caveat.** `template/product-skills/**` is shipped to generated
projects. Confirm in the tasks phase whether this skill is authored directly here
or generated from `.agents`/`src`; FR-012 requires edits land in the canonical
source and regenerate cleanly (`SkillSyncCheck`/`TargetMetadataDrift`/
`SkillQualityCheck` green).

**Rationale.** 060 FR-007 already established the note and example; BD-4 is the
not-yet-covered consumer-internal cross-module case. Extending keeps one note,
two examples (framework-vs-framework + consumer-vs-consumer), matching the spec's
"Extend, don't re-add."

---

## D8 — Arcade-helper triage: ship vs document, per helper (FR-011)

**Decision.** **Document each helper as a canonical convention in the relevant
capability skill** (the FR-011 "document" satisfier) rather than shipping them as
new public API in `FS.Skia.UI.SkillSupport`, recording the per-helper decision in
this feature's readiness. Per-helper:

| Helper | Disposition | Home skill |
|--------|-------------|------------|
| Fixed-step accumulator (`1/120 s`, capped steps/tick) deterministic `step` driver | Document convention | `fs-skia-elmish` (MVU update/game-loop) |
| AABB / circle-vs-rect collision + single-reflection-per-step (axis by normalized penetration) | Document convention | `fs-skia-elmish` (or a scene-geometry skill if one fits better) |
| Paddle-rebound angle with `|Dy|` floor | Document convention | `fs-skia-elmish` |
| HUD-band reservation (`reserveHudBand`: gameplay = surface − reserved band, clamp, overdraw HUD last) | Document convention | `fs-skia-layout-readability` (extends 060 FR-008's HUD/gameplay pattern doc) |

**Rationale (the key category finding).** `src/SkillSupport` is **not** a
consumer/runtime game library — its public surface is build-/governance-authoring
support (`Globbing`, `Graph`, `Parsing`, `CodeGen`, `ShellProcess`), the F#
peers of the `fsharp-*` authoring skills (`fsharp-io-globbing`,
`fsharp-graph-algorithms`, `fsharp-parsing`, `fsharp-code-generation`,
`fsharp-shell-process`). It has no `SKILL.md` and no game/runtime API. Adding
deterministic game-loop and collision primitives there would be a category
mismatch and would expand a governance-tool package's public `.fsi` surface
(triggering surface-baseline churn) for consumer-runtime concerns.

The spec anticipates this: BD-8 is the *consumer's* SkillSupport flag, FR-011
explicitly allows "documented in the relevant skill as the canonical convention"
as a full alternative, and "Unsupported scope" says the triage is delivered "as
guidance/SkillSupport, not as new hard merge gates." Documenting each helper as a
canonical pattern (with the small reference F# snippet) in the skill that already
owns its domain is the lower-risk, in-grain choice. The 060 precedent is exactly
this: 060 FR-008 documented the HUD/gameplay *pattern* in
`fs-skia-layout-readability` rather than shipping a layout helper. `reserveHudBand`
extends that same doc.

**Reversibility.** If a later feature decides to ship these as real API, the
documented convention is the spec for it — nothing here forecloses that. The
per-helper decision row in readiness is the record SC-008 asks for.

**Alternatives considered.** Ship all four in `FS.Skia.UI.SkillSupport`
(rejected — category mismatch + surface-baseline cost, see above). Create a new
`FS.Skia.UI.GameKit` runtime package (rejected — far exceeds a consumer-friction
follow-up's scope; "no new framework runtime capability" in Unsupported scope).

---

## D9 — Routing, gates, and the `.agents` → `.claude` regeneration discipline (FR-012)

**Decision.** Run `./fake.sh build -t Route` after each change-set and run only
the gates it prints; make all skill edits in canonical sources
(`.agents/skills/**`, or `template/feedback/skill/SKILL.md` for the
template-only feedback skill) and regenerate `.claude/**` with
`RefreshSurfaceBaselines` before validating, keeping `SkillSyncCheck` /
`TargetMetadataDrift` / `SkillQualityCheck` green.

**Rationale.** Route on the current working tree prints
`tier=focused-authority`, gates `Dev, TemplateCheck, GeneratedProductCheck,
GeneratedGuidanceCheck, SkillContractPathCheck, TemplateDrift, EvidenceGraph`.
This will **broaden** once the change touches `build/Governance/**` (FR-004/005/007),
the readiness tree, and more template surfaces — the spec's "Build-target impact"
anticipates `EvidenceAudit`, `TemplateCheck`, `GeneratedProductCheck` changes.
Route is the single source of truth for the gate list (compiled `Routing.fs`); a
mistyped gate is a compile error. FAKE-backed targets run **sequentially**
(shared `.fake` state). The canonical `.agents` → generated `.claude` rule (and
the template-only status of the feedback skill, which `SkillSyncCheck` does *not*
govern but `TemplateCheck`/`GeneratedProductCheck` may pin) is load-bearing for
FR-001 and FR-003.

---

## Resolved unknowns summary

| Spec-deferred decision | Resolution |
|------------------------|------------|
| FR-004 mechanism (ship templates vs audit-prints-schema) | **Audit-prints-schema** (primary), optional same-source template index backstop (D2) |
| FR-005 spelling | **Single literal `product-defect`** everywhere; confirm no residual `<project>-defect` rule (D3) |
| FR-011 per-helper ship-vs-document | **Document all four as canonical conventions** in `fs-skia-elmish` / `fs-skia-layout-readability`; SkillSupport is a governance lib, not a game lib (D8) |
| Duplicate plan/tasks templates | **Preset copy authoritative**; generic copy gets a pointer; copies already differ (D5) |
| Prompt-count enforcement | Land 3→4 across all surfaces + low-cost gate assertion; fix stale "three prompts" refs (D6) |

No `NEEDS CLARIFICATION` remains.
