# Feature Specification: Fail-Loud Authoring & Audit Robustness

**Feature Branch**: `037-authoring-audit-robustness`  
**Created**: 2026-05-30  
**Status**: Draft  
**Input**: User description: "create specs for the feedback" — friction points reported while building an application on FS.Skia.UI, spanning runtime API ergonomics (name collisions, FSI loading) and Spec Kit governance tooling (feature-directory defaulting, audit substring scanning).

## Overview

An application author building on FS.Skia.UI through the Spec Kit process hit a
cluster of failures that share one trait: they **failed silently or
opaquely**. Name collisions between `FS.Skia.UI.Scene` and
`FS.Skia.UI.Controls` mis-resolved with a diagnostic that pointed nowhere near
the cause; loading the generated app into FSI required spelling out seven
transitive assembly references with no guidance; the governance audit reported
a **false green** because it ran against an auto-generated stub feature instead
of the author's real 34-task feature; and the audit hard-blocked on three
phantom violations that came from prose and markdown bullets rather than real
problems.

This feature hardens those four failure points so the framework and the
governance process **fail loudly, point at the real cause, or refuse to give a
misleading pass**. It does not add new product capability — it removes
footguns and false signals discovered in real authoring.

## Clarifications

### Session 2026-05-30

- Q: Name-collision remedy — contract change vs. guidance-only? → A: Targeted contract change — add `[<RequireQualifiedAccess>]` to `ControlEventOrigin` only; Scene DU and bounds record remain guidance-governed.
- Q: Audit behavior when no real feature resolves — block or warn-and-continue? → A: Hard-fail — the audit blocks (non-zero exit) with a prominent warning; an unresolved feature is never a passable state.
- Q: Authoritative source for machine-readable status values? → A: Only a designated structured region (e.g. fenced code block / declared field) is authoritative; prose mentions are never read as status values.
- Q: FSI load entry point form — generated script vs docs snippet? → A: A generated `.fsx` load script emitted with the app that auto-stays in sync with the assembly set; the author `#load`s/runs it.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Governance audit never silently passes against the wrong feature (Priority: P1)

An author runs the evidence graph and audit on their active feature. The
tooling must validate the feature the author is actually working on, not an
auto-generated placeholder. If it cannot confidently locate the intended
feature, it must say so loudly rather than report a green pass against a stub.

**Why this priority**: A false green on a governance merge-gate is the most
dangerous failure in the set — it certifies work as evidence-complete when it
was never inspected. This undermines the entire purpose of the audit.

**Independent Test**: With a feature directory containing many tasks selected
as the active feature, run the evidence graph/audit and confirm it reports the
real task count for that feature. Then unset/omit the explicit selection and
confirm the tooling resolves the active feature from recorded state (rather
than defaulting to a 1-task stub) or emits a clearly visible warning that no
real feature was found.

**Acceptance Scenarios**:

1. **Given** an active feature with 34 tasks recorded in project state,
   **When** the author runs the graph/audit without manually exporting a
   feature-directory override, **Then** the tooling resolves and reports that
   feature's real task count, not a 1-task generated stub.
2. **Given** no resolvable active feature, **When** the author runs the
   graph/audit, **Then** the tooling hard-fails with a non-zero/blocking result
   and a prominent warning naming the expected feature and the resolution
   failure — never a silent green pass against a generated placeholder.
3. **Given** a mismatch between the recorded active feature and the directory
   the audit scanned, **When** the audit completes, **Then** the discrepancy is
   surfaced in the output.

---

### User Story 2 - Governance audit blocks only on real violations, not prose mentions (Priority: P1)

An author writes specs, plans, and evidence notes that legitimately discuss
governance concepts — including sentences that say a claim is *not* a
taskbar-only claim, or that explain what a package mismatch would be. The audit
must not hard-block merely because a blocker keyword appears inside explanatory
prose or a markdown bullet.

**Why this priority**: Phantom blockers from substring matching make the audit
untrustworthy and force authors to mangle their documentation to appease the
scanner. Three separate false blocks were observed from a single round of
honest writing.

**Independent Test**: Feed the audit a document that mentions blocker terms
("taskbar-only", "package mismatch", "nu1603") exclusively inside explanatory
sentences and negations, plus a clean machine-readable status value. Confirm
the audit does not raise those as blockers, while still blocking when the same
terms describe a genuine violation.

**Acceptance Scenarios**:

1. **Given** a sentence stating "this is **not** a taskbar-only claim",
   **When** the audit runs, **Then** it does not raise a taskbar-only blocker.
2. **Given** a structured status field set to a passing value in a code block,
   **When** a later markdown bullet mentions the same key in prose (e.g.
   `- exact-package-match=true: no ...`), **Then** the audit reads the
   authoritative structured value and does not let the prose bullet override it
   into a false "package mismatch".
3. **Given** an evidence file that genuinely declares a violating status,
   **When** the audit runs, **Then** it still hard-blocks as intended.
4. **Given** a key appears more than once, **When** the audit resolves its
   value, **Then** the resolution rule is deterministic and documented (the
   authoritative source wins, not "last textual occurrence").

---

### User Story 3 - Mixed Scene/Controls code resolves names predictably or fails with an actionable diagnostic (Priority: P2)

An author opens both `FS.Skia.UI.Scene` and `FS.Skia.UI.Controls` and writes a
scene view. Constructing a scene node (e.g. text) must either resolve to the
intended scene construct regardless of `open` ordering, or fail with a
diagnostic that names the collision so the author can fix it in minutes instead
of an hour.

**Why this priority**: This cost real debugging time, but unlike the audit
issues it produced an error rather than a false pass, and a documented
workaround exists today (qualify names / order opens). It is high-value
ergonomics, not a correctness gate.

**Independent Test**: In a file that opens both namespaces in the order that
previously mis-resolved, construct a scene text/group node and a bounds
literal. Confirm it either compiles to the intended scene construct, or fails
with a message that identifies the colliding names and the namespaces involved.

**Acceptance Scenarios**:

1. **Given** both Scene and Controls are opened with Controls last,
   **When** the author constructs a scene text node unqualified, **Then** it
   either resolves to the scene text construct or fails with a diagnostic that
   names the collision (rather than reporting an opaque "value is not a
   function / has type ControlEventOrigin" error).
2. **Given** an author defines their own record with the same field set as the
   shared bounds type, **When** they use shared-bounds literals nearby, **Then**
   the resolution behavior is predictable and the recommended pattern (reuse the
   shared bounds type) is documented at the point of use.
3. **Given** the author follows the documented authoring guidance, **When** they
   write a mixed Scene/Controls view, **Then** it compiles without reordering
   `open` statements by trial and error.

---

### User Story 4 - Authors can load a generated app into FSI from a documented entry point (Priority: P3)

An author wants to explore their generated application interactively in FSI.
There must be a documented, copy-pasteable way to load the app and its
transitive dependencies without manually discovering every assembly reference.

**Why this priority**: Pure convenience; no correctness or governance impact. A
workaround (reference all assemblies manually) exists but is tedious.

**Independent Test**: Follow the documented FSI loading entry point for a freshly
generated app and confirm the app's entry types are usable in FSI without the
author having to hand-author each transitive reference.

**Acceptance Scenarios**:

1. **Given** a freshly generated application, **When** the author follows the
   documented FSI load instructions (or runs the provided load script), **Then**
   the app loads in FSI without an unresolved-transitive-reference error.
2. **Given** the generated app's assembly set changes, **When** the author uses
   the documented entry point, **Then** it continues to resolve the full set
   (the entry point is not a fragile hand-maintained list the author must edit).

---

### Edge Cases

- The active feature is resolvable but its task file is empty or unparseable —
  the audit should report that explicitly rather than fall back to a stub.
- A blocker term appears in a fenced code block that is illustrative (showing
  what a bad value looks like) — treated as prose, not a live violation.
- A structured status key is present but malformed — surfaced as a parse error,
  not silently treated as passing or failing.
- Scene/Controls collision occurs on a name not yet observed (beyond text /
  group / bounds) — guidance should generalize, not enumerate one case.
- FSI load entry point used in an unsupported headless/host environment —
  benign host warnings remain classified as benign, not as load failures.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The evidence graph/audit MUST resolve the feature under audit from
  the project's recorded active-feature state when no explicit override is
  provided, instead of defaulting to an auto-generated single-task placeholder.
- **FR-002**: When the tooling cannot resolve a real active feature, it MUST
  hard-fail (terminate with a non-zero/blocking result) and emit a prominent,
  non-suppressible warning identifying the expected feature and why resolution
  failed. An unresolved feature MUST never be a passable state — it MUST NOT
  silently fall back to auditing a generated placeholder and report a clean
  pass.
- **FR-003**: The audit MUST report the real task count of the feature it
  scanned so an author can immediately detect a wrong-feature mismatch.
- **FR-004**: The audit MUST distinguish blocker conditions that describe a
  genuine violation from occurrences of the same terms inside explanatory prose,
  negations, or illustrative examples, and MUST NOT hard-block on the latter.
- **FR-005**: The audit MUST resolve machine-readable status values only from a
  designated structured region (e.g. a fenced code block or declared field).
  Occurrences of the same key in explanatory prose or markdown bullets MUST
  never be read as status values, so they cannot override the authoritative
  region. The resolution rule MUST be deterministic and documented for authors.
- **FR-006**: The audit MUST continue to hard-block on genuine violations after
  these robustness changes (no regression in true-positive blocking).
- **FR-007**: Mixed Scene/Controls authoring MUST either resolve colliding
  unqualified names to the intended construct independent of `open` ordering, or
  surface a diagnostic that names the colliding symbols and their namespaces.
  The chosen remedy is a targeted contract change: `[<RequireQualifiedAccess>]`
  is added to `ControlEventOrigin` so its `Text` case stops leaking into the
  open namespace and shadowing the scene text construct. The Scene discriminated
  union constructors and the shared bounds record remain guidance-governed.
- **FR-008**: Authoring guidance MUST document the predictable pattern for
  shared structurally-typed types (e.g. reusing the shared bounds type) so
  authors avoid record-field inference hijack.
- **FR-009**: The template MUST emit a generated `.fsx` load script alongside a
  generated application that the author can `#load`/run to load the app and its
  transitive references in FSI without hand-enumerating any. Because it is
  generated, the script MUST stay in sync with the app's assembly set rather
  than being a hand-maintained reference list.
- **FR-010**: All changes MUST preserve existing public-contract and governance
  decisions unless a contract change is explicitly chosen and recorded; where a
  prior recorded decision (e.g. the spec 035 "guidance over attributes" choice
  for name collisions) is reversed, the reversal MUST be documented with
  rationale.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identities, contents, or versions are expected
  to change. If the name-collision remedy chosen alters a published `.fsi`
  surface (e.g. adding qualified-access to an existing type), that is a public
  contract change, not a package-identity change. No legacy Charts migration.
- **Public contract impact**: Yes, scoped to User Story 3 — the chosen remedy
  adds `[<RequireQualifiedAccess>]` to the existing `ControlEventOrigin` type,
  so the `.fsi` surface changes and surface baselines must be updated. This
  reverses spec 035's "no contract change" choice for this one type only; the
  reversal and its rationale are recorded per FR-010. No other contract changes.
- **State workflow impact**: None. No stateful workflow, I/O, command, effect,
  subscription, or interpreter behavior changes.
- **Layout/rendering impact**: None to layout/rendering output. The FSI load
  entry point and Scene/Controls naming touch authoring ergonomics, not visual
  output; benign host-warning classification must remain intact.
- **Evidence obligations**: Real audit runs demonstrating (a) correct active
  feature resolution with true task count, (b) no false block on prose/negation
  fixtures, (c) sustained true blocking on a genuine-violation fixture; a real
  FSI load transcript for a generated app; a real compile of a mixed
  Scene/Controls fixture in the previously-failing `open` order.
- **Unsupported scope**: No new rendering features, no release/distribution
  changes, no new platforms, no broad audit-rule redesign beyond the parsing
  robustness described here.
- **Build-target impact**: `EvidenceGraph` and `EvidenceAudit` behavior changes
  (feature resolution + parsing robustness). `GeneratedGuidanceCheck` /
  `TemplateCheck` / `GeneratedProductCheck` may change if FSI-load guidance or
  Scene/Controls authoring guidance is added to generated output. `Dev` must
  continue to pass.

## Success Criteria *(mandatory)*

- **SC-001**: In 100% of audit runs where a real active feature exists, the
  audit scans that feature and reports its real task count; zero silent stub
  passes.
- **SC-002**: When no real feature is resolvable, 100% of runs hard-fail with a
  visible warning; zero runs complete with a passing result against a stub
  fallback.
- **SC-003**: A fixture set of prose/negation/illustrative mentions of blocker
  terms produces zero false blocks, while a fixture set of genuine violations
  produces a block in 100% of cases (no true-positive regression).
- **SC-004**: A mixed Scene/Controls view written in the previously-failing
  `open` order either compiles or fails with a diagnostic that names the
  colliding symbols — verified on a fixture that previously produced the opaque
  error.
- **SC-005**: An author can load a freshly generated application into FSI by
  following a single documented step, with zero manual transitive-reference
  edits required.
- **SC-006**: Time-to-diagnose the four reported failure classes drops from
  "up to an hour" (as reported) to under five minutes, evidenced by the
  diagnostic/guidance now pointing at the real cause.

## Assumptions

- The project's active feature is recorded in resolvable state (e.g.
  `.specify/feature.json`) that the governance tooling can read as the default.
- "Authoritative source" for a structured status value is a designated
  machine-readable region (e.g. a fenced code block / declared field); prose
  bullets are never read as status (resolved in Clarifications). The concrete
  region format and detection are detailed in planning.
- Benign host-warning classification rules from prior features remain the
  source of truth for FSI/headless environments.
- Fixtures can be added under the feature's readiness directory to evidence the
  false-positive and true-positive audit behavior.

## Dependencies

- Prior recorded decision in `specs/035-api-discovery-names`
  (`readiness/name-collision-safety.md`) that chose consumer guidance over
  contract attributes for Scene/Controls collisions — User Story 3 must either
  honor or explicitly, with rationale, revise it (FR-010).
- Existing `EvidenceGraph` / `EvidenceAudit` build targets and their fixture
  conventions.
