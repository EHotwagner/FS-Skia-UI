# Feature Specification: Deterministic Escalated Validation Path

**Feature Branch**: `049-fix-escalated-flake`  
**Created**: 2026-06-02  
**Status**: Draft  
**Input**: User description: "fix the escalated path flakeyness"

## Context *(non-normative)*

The escalated `maintainer-verify` validation path (the serialized run of `Dev`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
`EvidenceGraph`, `EvidenceAudit`, plus the broader `Verify`/`Ci` gates) is the
authority an autonomous agent operator relies on to decide whether a
consumer-contract change is safe to land. Today that authority is unreliable on
the project's headless host: validation runs intermittently **crash** or **hang**
for reasons unrelated to the change under test.

The cause is confirmed and environmental, not a product defect. The headless host
advertises **both** a Wayland display and an X11 (Xvfb) display at once. The
viewer/sample graphics stack then prefers the Wayland path and tries to load a
desktop-decoration plugin that cannot initialize in the container. Two distinct
symptoms follow from the same cause:

1. **Teardown crash** — the GUI/viewer test process passes its assertions, then
   the *host* aborts on shutdown, turning a green run red.
2. **Startup hang** — a nested generated-product validation step stalls for
   roughly twenty minutes during graphics initialization before the run can make
   progress.

Both symptoms surface only when several validation steps run together (the
escalated aggregate), which is exactly when the operator most needs a trustworthy
verdict. The current coping mechanism — "treat the aggregate as non-authoritative,
set graphics environment variables by hand, and rerun the affected step in
isolation" — depends on operator judgment that an autonomous agent does not
reliably have, and it defeats the purpose of the gate.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Agent gets a trustworthy escalated verdict (Priority: P1)

An autonomous agent operator finishes a consumer-contract change, runs `Route`,
and is told to run the escalated validation path. It runs the path once, on the
standard headless host, with no special environment setup.

**Independent test**: On the headless host, run the escalated path against a known
clean working tree. The run completes with a pass verdict, with no
graphics-backend-related crash and no multi-minute graphics-initialization stall,
repeatably across consecutive runs.

**Why this priority**: This is the whole point of the feature — the gate's verdict
must reflect the change under test, not the host's display configuration.

### User Story 2 - Nested generated-product validation no longer stalls (Priority: P2)

The escalated path validates generated products, each of which runs its own
inner validation. Previously one of these inner runs could hang for ~20 minutes
during graphics initialization, making the whole path appear stuck.

**Independent test**: Run the step that validates a generated product on the
headless host. It completes within its normal time envelope with no
graphics-initialization stall; if a backend genuinely cannot start, the step
fails fast with a clear diagnostic rather than hanging.

**Why this priority**: The hang is the most expensive failure mode for an agent
operator (long wall-clock cost, ambiguous state) and shares the root cause with
P1, so it is fixed by the same mechanism.

### User Story 3 - Headed and non-Linux developer hosts are unaffected (Priority: P3)

A maintainer runs validation, samples, or the viewer on a real desktop (a headed
Linux session, or a non-Linux developer machine) where the default graphics path
already works.

**Independent test**: On a headed/native desktop host, run validation and a
sample viewer. Behavior, backend selection, and visual output are unchanged from
before the feature.

**Why this priority**: The fix must remove the flake without degrading the
environments that already work; it is a safety boundary, not a new capability.

### Edge Cases

- **No usable graphics backend at all**: the path must fail within a bounded time
  with a diagnostic that names the environmental cause, never hang indefinitely.
- **Only one display kind present** (only Wayland, or only X11): backend selection
  must still resolve deterministically to a backend that initializes.
- **A real product/test failure during a previously-flaky step**: must still be
  reported as a failure — the fix must not mask genuine regressions by suppressing
  or ignoring the step's outcome.
- **Concurrent vs serialized runs**: the escalated path remains serialized per the
  shared build-state constraint; the fix must not introduce a dependency on
  parallel execution.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The escalated validation path MUST produce a verdict that reflects
  only the change under test. A clean working tree MUST NOT fail or hang because of
  the host's graphics-backend selection.
- **FR-002**: On a headless host that exposes more than one display kind at once,
  the validation path MUST deterministically select a graphics backend that
  initializes successfully, **without requiring the operator to set environment
  variables or perform any manual step**.
- **FR-003**: The deterministic backend selection MUST propagate to nested
  validation invocations (notably generated-product validation), so those inner
  runs neither crash on teardown nor stall during graphics initialization.
- **FR-004**: A GUI/viewer test process that passes its assertions MUST be reported
  as passing. A host-teardown crash that occurs after assertions have passed MUST
  NOT turn a passing run into a failing one.
- **FR-005**: Any validation step that initializes graphics MUST be bounded by a
  timeout. If a backend cannot initialize, the step MUST fail fast (within the
  bound) with a diagnostic that distinguishes an environment/backend failure from a
  product regression — it MUST NOT hang.
- **FR-006**: After this feature, the escalated aggregate result MUST be
  authoritative for this flake class: obtaining a trustworthy verdict MUST NOT
  depend on a manual focused rerun of the previously-flaky step.
- **FR-007**: Backend selection MUST be safe on headed and non-Linux developer
  hosts. Where the default backend already initializes, behavior, selection, and
  visual output MUST be unchanged.
- **FR-008**: The fix MUST NOT mask genuine failures. A real test or product
  failure in a previously-flaky step MUST continue to be reported as a failure.
- **FR-009**: The project's operator-facing guidance and evidence artifacts (the
  aggregate-hang diagnostics and runtime-limitations notes) MUST be updated to
  describe the deterministic behavior and remove the now-obsolete "non-authoritative
  aggregate / rerun by hand" workaround as the expected procedure.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identities, package contents, or package versions
  change, and generated package consumers are unaffected. No controls/chart/graph/
  DataGrid authoring change; no Charts package migration guidance involved.
- **Public contract impact**: No `.fsi` signatures, documented public APIs, sample
  contracts, or surface baselines change. The change is to how validation/test
  processes select a graphics backend in unsupported/headless environments, not to
  any consumed API.
- **State workflow impact**: No stateful workflow, I/O, command, effect,
  subscription, or interpreter behavior changes.
- **Layout/rendering impact**: No change to layout, charts, DataGrid, visual
  output, or rendering results. The change concerns graphics-backend selection and
  unsupported-environment diagnostics only; golden/parity output MUST be unchanged.
- **Evidence obligations**: Real evidence required at
  `specs/049-fix-escalated-flake/readiness/aggregate-hang-diagnostics.md` (showing a
  deterministic, authoritative aggregate with no rerun caveat),
  `specs/049-fix-escalated-flake/readiness/runtime-limitations.md` (graphics-backend
  selection in headless/unsupported environments), and the escalated-path evidence
  set (`readiness/target-metadata.md`, `readiness/agent-ready-verdict.md`), produced
  voluntarily as corroborating evidence: this change escalates to `maintainer-verify`
  via routing **default-deny** (its `build/**` paths do not match the
  `build-target-contract` rule's `build.fsx` / `scripts/build/**` /
  `validation.contract.yml` globs, and the default-deny path carries no
  `Route --enforce`-required artifacts), so these files are not enforced by routing
  but are recorded to keep the escalated-path evidence complete.
- **Unsupported scope**: Out of scope — adding a software-renderer fallback;
  supporting macOS/mobile/browser hosts; changing visual parity output; making the
  escalated targets safe to run concurrently; broad performance work on the
  validation path beyond removing the hang.
- **Build-target impact**: Process-launch behavior for the escalated path changes
  (how `Dev`, `GeneratedProductCheck`, and the nested `Verify`/generated-product
  runs spawn test and FSI processes / select a backend). `Verify` and `Ci` exercise
  this path. Per routing, a build-target/process change escalates to the
  `maintainer-verify` tier; `TargetMetadataDrift` and the full serialized order
  apply.

## Success Criteria *(mandatory)*

- **SC-001**: On the standard headless host, the escalated validation path
  completes with a single deterministic pass/fail verdict against a clean working
  tree, with **zero** graphics-backend-related host crashes; determinism is
  guaranteed by the pure dual-display guard (unit-proven), not by repetition.
- **SC-002**: **No** validation step stalls during graphics initialization; the
  previously observed ~20-minute startup hang does not recur, and every
  graphics-initializing step either completes within its normal envelope or fails
  within its bounded timeout.
- **SC-003**: An operator obtains an authoritative verdict from a **single** run of
  the escalated path — no manual environment-variable setup and no focused rerun are
  required.
- **SC-004**: On a headed/native desktop host, validation, samples, and viewer
  behavior — including visual output — are **identical** to pre-feature behavior.
- **SC-005**: When no usable graphics backend exists, the affected step fails within
  its bounded timeout and emits a diagnostic that names the environmental cause,
  with **no** indefinite hang.
- **SC-006**: A genuine test/product failure injected into a previously-flaky step
  is still reported as a failure (the fix does not suppress real regressions).

## Assumptions

- The root cause is the host advertising both a Wayland and an X11 display
  simultaneously, leading the graphics stack to choose a backend whose
  desktop-decoration plugin cannot initialize in the container (confirmed
  2026-05-31). The fix targets deterministic backend selection, not the underlying
  library bug.
- Forcing the already-working X11/Xvfb path (and disabling the failing Wayland
  selection) for validation runs is an acceptable, behavior-preserving choice on the
  headless host, because the tests pass on that path today and only teardown/startup
  graphics handling is affected.
- The fix is applied at the outermost validation entry point so that nested
  `./fake.sh`/FSI invocations inherit the deterministic selection, covering both the
  framework's own steps and generated-product inner validation.
- The escalated path remains serialized (shared build state); this feature does not
  change that constraint and does not rely on parallelism.
- This change routes through the escalated `maintainer-verify` tier and is itself
  validated by the full serialized order it is repairing.
