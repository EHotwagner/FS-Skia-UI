# Implementation Plan: Deterministic Escalated Validation Path

**Branch**: `049-fix-escalated-flake` | **Date**: 2026-06-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/049-fix-escalated-flake/spec.md`

## Summary

The escalated `maintainer-verify` validation path intermittently crashes (test
teardown) or hangs (~20 min, generated-product graphics init) on the headless
host because the host advertises **both** a Wayland and an X11 (Xvfb) display, and
the graphics stack prefers the Wayland path whose `libdecor-gtk.so` plugin cannot
initialize in the container. The fix makes graphics-backend selection
**deterministic and self-applied** by the compiled build front-end: at process
startup the front-end normalizes its own environment to force the working X11 path
(remove `WAYLAND_DISPLAY`, set `GDK_BACKEND=x11`, `SDL_VIDEODRIVER=x11`) **only when
the dual-display condition holds**, and the process-spawn edge re-applies the same
normalization to every child it launches. Because `BuildProcess` spawns children
with `UseShellExecute=false` (children inherit the parent env) and nested
generated-product validation is launched as `bash ./fake.sh build -t <target>`,
one startup normalization propagates to every descendant (dotnet test, FSI, nested
`fake.sh`). On headed/native/non-Linux hosts the condition is false and the change
is a no-op. A graphics-initializing step that still cannot start fails fast within
its existing timeout with an enriched diagnostic naming the environmental cause.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: none new (uses `System.Diagnostics`, `System.Environment` already in `build/Governance`)
**Testing**: Expecto (`tests/Governance.Tests`) for the pure normalization function and a real process-spawn contract test; the escalated serialized FAKE order for end-to-end real evidence
**Target Platform**: Linux headless (CI/container) is the affected host; headed Linux and non-Linux developer hosts must be unaffected

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification**: **Tier 2 (internal change)** per the constitution — no
public API / `.fsi` / package surface changes; this alters build-tooling process-
launch behavior only. (Note: it still *routes* to the escalated `maintainer-verify`
tier per `Routing.fs`, because changed paths under `build/**`, `tests/**`, and the
launchers fall outside the inner-loop `src/**` allowance. Constitution Tier and
Routing tier are independent axes; both are satisfied here.)

### Repository Governance Decisions

- **Template ownership**: No change. `.template.config/template.json` is untouched;
  no source/docs/samples/Spec-Kit/package-policy/command-surface assets that the
  template ships are altered. (N/A — build front-end internals only.)
- **Dependency impact**: No change. No new packages; `Directory.Packages.props`,
  `docs/dependencies.md`, generated-template inclusion, and `DependencyReport`
  coverage are unaffected. (N/A.)
- **Command-surface impact**: **Yes — behavioral, not contractual.** The compiled
  front-end (`build/Governance/Front/BuildProcess.fs`,
  `BuildProcessHealth.fs`, `build/Program.fs`) changes how it normalizes its own and
  child-process environments. No targets are added, removed, or renamed, so
  `validation.contract.yml` and `TargetMetadata`/`TargetMetadataDrift` outputs are
  unchanged. `Dev`, `GeneratedProductCheck`, `Verify`, and `Ci` all exercise the
  changed path. FAKE-backed targets remain serialized and order-sensitive; this
  change does not relax that. Serialized verification order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: Generated project *contents* are unchanged. Only the
  *environment* under which generated-product `Dev`/`Test`/`Verify` are invoked by
  `GeneratedProduct.fs` changes (inherited deterministic graphics env), removing the
  ~20-min FSI graphics-init hang. No change to default/minimal contents, selected
  Controls guidance, local skills, placeholder/excluded-history scans, or generated
  `Dev` logic.
- **Evidence paths**:
  - `specs/049-fix-escalated-flake/readiness/aggregate-hang-diagnostics.md` — real escalated run showing deterministic PASS, no graphics crash/hang, no rerun caveat.
  - `specs/049-fix-escalated-flake/readiness/runtime-limitations.md` — graphics-backend selection in headless/unsupported environments.
  - `specs/049-fix-escalated-flake/readiness/logs/` — captured target logs (Dev/GeneratedProductCheck) evidencing no `libdecor-gtk` crash and bounded durations.
  - `specs/049-fix-escalated-flake/readiness/graphics-env-contract.md` — the normalization decision table (mirrors `contracts/`).
- **`.fsi` / contract impact**: **None.** No public signatures, surface baselines,
  sample contracts, or compatibility notes change. The new normalization logic lives
  in the build front-end, which (consistent with every existing `build/Governance`
  module) ships **no `.fsi`** — it is an internal compiled application, not a packed
  library with a curated public surface, so Principle II's `.fsi` requirement does
  not apply here (N/A with rationale).
- **MVU/effect boundary**: The build front-end already has an Elmish-style
  `Engine` (`Model`/`Update`/`Interpret`). Process spawning is the existing
  interpreter edge (`Interpret.fs` → `BuildProcess.runProcess`). The new graphics-
  environment normalization is a **pure function** (`Map<string,string> ->
  Map<string,string>`, plus a read of the ambient environment) consumed only at that
  edge and at `Program` startup. No new `Model`/`Msg`/`Effect` is required; `update`
  stays pure and I/O stays at the edge.
- **Synthetic evidence**: The pure-normalization unit tests use literal environment
  maps, but these are **genuine unit tests of the production function over its actual
  input domain (string→string maps)**, not substitutes for an unavailable dependency
  — they are *not* `[S]`. Real evidence is provided at two further levels: a real
  child-process spawn whose inherited environment is inspected (interpreter
  evidence), and a real escalated-path run on the headless host
  (`aggregate-hang-diagnostics.md`). No mocks/fakes/placeholders are introduced; no
  `[S]` disclosure is anticipated.
- **Test evidence**:
  - Failing-first unit tests for the normalization function (dual-display →
    normalized; single-display → safe; headed/no-display → no-op).
  - A process-spawn contract test asserting a child launched by
    `runProcessWithAllowedExitCodes` does **not** see `WAYLAND_DISPLAY` and sees the
    forced backend vars when the condition held.
  - A regression-style assertion that a passing GUI/viewer test process is reported
    as passing even if the host aborts on teardown (FR-004) — exercised through the
    real `SkiaViewer.Tests` focused control as corroboration.
  - End-to-end real evidence via the serialized escalated order.
- **Observability**: When a graphics-initializing child is killed at its timeout,
  the diagnostic is enriched to name the likely environmental cause (graphics
  backend could not initialize) and point to `runtime-limitations.md`, satisfying
  Principle VII (fail fast, actionable diagnostic, no silent stall). The
  normalization decision (which vars were forced/removed, or "no-op: condition not
  met") is logged once at front-end startup.
- **Deferred scope**: Wiring `build/**` into an explicit `Routing.fs` rule (today it
  escalates via default-deny, which is correct but implicit) is **out of scope** —
  noted as a follow-up to avoid expanding governance surface in a flake fix. No
  software-renderer fallback, no macOS/mobile/browser support, no concurrency-safety
  work on FAKE targets, and no visual-output change are in scope.

**Gate result**: PASS. No principle is violated; the one applicable exemption
(`.fsi` not required for the internal build front-end) is justified above.

## Project Structure

```
specs/049-fix-escalated-flake/
├── spec.md
├── plan.md                       # this file
├── research.md                   # Phase 0
├── data-model.md                 # Phase 1
├── quickstart.md                 # Phase 1
├── contracts/
│   └── graphics-env-contract.md  # normalization decision table + spawn guarantee
├── checklists/
│   └── requirements.md
└── readiness/                    # real evidence (produced during implementation)
    ├── aggregate-hang-diagnostics.md
    ├── runtime-limitations.md
    ├── graphics-env-contract.md
    └── logs/

build/Governance/Front/
├── BuildEnvironment.fs   # NEW — pure `normalizeGraphicsEnv` + `applyToProcess`/`normalizeAmbient`
├── BuildProcess.fs       # EDIT — merge normalized graphics env into child startInfo; enrich timeout diagnostic
└── BuildProcessHealth.fs # EDIT — same normalization for runShortCommand

build/
└── Program.fs            # EDIT — normalize ambient environment once at startup (propagates to all descendants)

tests/Governance.Tests/
└── GraphicsEnvironmentTests.fs   # NEW — pure unit tests + real process-spawn contract test
```

(`fake.sh` / `fake.cmd` are intentionally **not** edited — normalization at the
front-end entry point is the single cross-platform source; see research.md R2.)

## Phase 2 note

Tasks are produced by `/speckit-tasks`. Expected ordering: (US3 safety boundary &
pure function first) → normalization module + unit tests → spawn-edge wiring +
contract test → startup normalization → timeout diagnostic → evidence capture via
the serialized escalated order.
