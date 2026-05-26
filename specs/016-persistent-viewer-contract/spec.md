# Feature Specification: Persistent Viewer Contract

**Feature Branch**: `016-persistent-viewer-contract`  
**Created**: 2026-05-26  
**Status**: Draft  
**Input**: User description: "create specs for Mailbox/persistent-viewer-contract-gap-analysis.md"

## Clarifications

### Session 2026-05-26

- Q: What evidence threshold is required when the local or CI host cannot open a desktop window? -> A: Require at least one supported-host persistent launch artifact for completion; unsupported-host diagnostics are valid only as additional evidence.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Launch A Generated Graphical App (Priority: P1)

A developer creates or consumes a generated graphical desktop app profile that includes the Skia viewer. When they run the app's default executable command, they get a persistent interactive window instead of a command that only prints metadata, counts controls, or exits after bounded smoke evidence.

**Independent Test**: Given a generated graphical app that declares viewer support, run its default executable command on a supported host. The command opens a persistent graphical window, keeps the app alive until user exit, renders the app view, accepts keyboard input where the feature declares keyboard behavior, and returns success only after an intentional exit path.

### User Story 2 - Distinguish Evidence From Product Readiness (Priority: P1)

A reviewer evaluates readiness evidence for a generated graphical feature. They can clearly tell whether the feature has a persistent interactive app launch path, a bounded viewer smoke path, deterministic scene evidence, or an unsupported-environment diagnostic, and bounded evidence alone cannot satisfy interactive graphical readiness.

**Independent Test**: Given a readiness package that contains bounded smoke and scene evidence but no persistent graphical launch evidence, the governance audit rejects the feature as incomplete for interactive graphical readiness and names the missing evidence category.

### User Story 3 - Diagnose Missing Capability Versus Unsupported Environment (Priority: P2)

A developer runs a generated graphical app in an environment where persistent windows may not be available. The outcome distinguishes a missing package/product capability from a supported contract that cannot run in the current host environment.

**Independent Test**: Given an app with the persistent viewer contract available but no supported display host, the app reports an unsupported-environment result with the blocked stage and reason. Given an app or consumed package with no persistent viewer capability, the app reports a product or contract capability failure rather than passing through bounded simulation.

### User Story 4 - Preserve Bounded Evidence Workflows (Priority: P3)

A maintainer continues to use bounded viewer and scene evidence for CI, diagnostics, and readiness artifacts, while those paths are clearly documented as evidence helpers and remain separate from the default interactive app launch path.

**Independent Test**: Given a generated graphical app, explicit bounded-evidence commands still produce bounded readiness artifacts, but the default executable path is the persistent viewer path and is evaluated under the separate graphical launch requirement.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST define a first-class persistent viewer contract for graphical scene apps that opens a desktop window, renders app content, remains active until user exit or host failure, and reports a clear outcome.
- **FR-002**: The system MUST define a first-class persistent generated-app host contract for model-driven graphical apps, including initialization, state updates, view production, keyboard input mapping, time-based updates where applicable, diagnostics, and intentional exit handling.
- **FR-003**: Generated graphical app profiles that include viewer support MUST use the persistent generated-app host contract as the default executable path.
- **FR-004**: Generated graphical app profiles MUST keep bounded smoke, first-frame, frame-count, scene metadata, and deterministic evidence flows available only through explicit evidence commands or flags.
- **FR-005**: The default executable path for a generated graphical app MUST NOT satisfy graphical app readiness by only printing metadata, counting controls, rendering static descriptions, running bounded smoke, or exiting without a persistent window attempt.
- **FR-006**: The system MUST expose a runtime capability result that lets generated apps and reviewers distinguish persistent window support, bounded smoke support, keyboard input support, renderer mode, and unsupported-host reasons.
- **FR-007**: Generated graphical apps MUST report failed, unsupported, and successful launch outcomes with enough detail for a reviewer to identify the blocked stage, classification, category, command path, and user-facing message.
- **FR-008**: Generated graphical app templates MUST provide a standard host skeleton covering model initialization, message update, view rendering, keyboard mapping, tick or time progression where applicable, viewer options, and the persistent app host value.
- **FR-009**: Generated app guidance checks MUST fail when a viewer-backed graphical app's default path only prints, counts controls, exposes bounded smoke, lacks persistent launch wiring, or lacks keyboard dispatch for keyboard-capable profiles.
- **FR-010**: Task generation for graphical viewer features MUST include an explicit persistent graphical launch task that cannot be completed with bounded evidence alone.
- **FR-011**: Evidence audit MUST require a distinct persistent graphical app launch artifact for generated interactive graphical features and MUST flag bounded-only substitution as a readiness contract failure.
- **FR-012**: Persistent graphical launch evidence MUST record status, persistent-window mode, command, whether a window opened, whether input dispatch was verified when relevant, whether an exit path was verified, blocked stage, classification, and message.
- **FR-013**: Completion evidence for this feature MUST include at least one supported-host persistent launch artifact; unsupported-host diagnostics may supplement that evidence but MUST NOT replace it.
- **FR-014**: Bounded viewer evidence documentation MUST state that bounded evidence supports CI and diagnostics but does not prove the persistent viewer contract or replace graphical launch evidence.
- **FR-015**: Existing bounded evidence behavior MUST remain available for compatibility unless it conflicts with the separation between evidence helpers and product launch readiness.
- **FR-016**: Migration guidance MUST tell existing generated apps that bounded-only graphical apps must either adopt the persistent host contract, declare themselves non-interactive/headless, or document the missing persistent viewer capability as a blocking gap.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents change for `FS.Skia.UI.SkiaViewer` because the package must expose persistent viewer and generated-app host capabilities. Generated package consumers change because graphical app templates must use the persistent host as their default executable path. Package identities do not need to change. Package versions must be evaluated during planning because adding public viewer capabilities likely requires a preview version bump before distribution.
- **Public contract impact**: Public contracts change for documented Skia viewer capabilities, generated app host contracts, launch outcomes, runtime capability reporting, sample contracts, and surface baselines. The `.fsi` signatures for the Skia viewer package are expected to change if planning confirms the package currently lacks these capabilities.
- **State workflow impact**: Stateful workflow changes for generated graphical apps because persistent app hosting must initialize model state, dispatch input messages, process update results, handle time progression where declared, interpret viewer-edge effects, and support intentional close behavior.
- **Layout/rendering impact**: Rendering and unsupported-environment diagnostics change for generated graphical apps because the default path must attempt or diagnose persistent window launch. The feature does not require changing visual design, layout semantics, chart behavior, DataGrid behavior, or screenshot styling beyond what is necessary to launch and evidence the persistent viewer.
- **Evidence obligations**: Required real evidence paths include `specs/016-persistent-viewer-contract/readiness/persistent-viewer-contract.md`, `specs/016-persistent-viewer-contract/readiness/generated-default-launch.md`, `specs/016-persistent-viewer-contract/readiness/bounded-evidence-separation.md`, `specs/016-persistent-viewer-contract/readiness/runtime-capability-diagnostics.md`, `specs/016-persistent-viewer-contract/readiness/generated-guidance-check.md`, `specs/016-persistent-viewer-contract/readiness/evidence-graph.md`, and `specs/016-persistent-viewer-contract/readiness/evidence-audit.md`.
- **Unsupported scope**: This feature does not promise new platform support, software rendering, mobile support, browser support, macOS support, release distribution changes, game-specific mechanics, or a redesign of existing controls. Unsupported host environments may remain unsupported, but they must be diagnosed distinctly from missing viewer capability.
- **Build-target impact**: `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit` must change or gain coverage for persistent graphical launch readiness. `Dev`, `Verify`, and `Ci` may need updates if they aggregate those checks. `PackLocal` may be required to validate package surface and generated consumer compatibility. `DependencyReport` and `TemplateDrift` should change only if the implementation affects package/template ownership reporting. 

### Key Entities

- **Persistent Viewer Contract**: The user-facing capability that opens and maintains a desktop graphical window until the user exits or a host failure occurs.
- **Generated App Host**: The standard model-driven app contract connecting initialization, updates, view output, input mapping, time progression, diagnostics, and exit behavior.
- **Runtime Capability Result**: A diagnostic record that states which viewer capabilities are available in the current package and host environment.
- **Graphical Launch Evidence**: A readiness artifact proving or diagnosing the default persistent graphical app launch path.
- **Bounded Evidence Artifact**: A CI or diagnostic artifact proving bounded rendering behavior without claiming persistent interactive readiness.
- **Generated Guidance Check**: A governance check that validates generated graphical templates and apps do not substitute bounded or print-only behavior for default persistent launch.

### Assumptions

- The source gap analysis in `Mailbox/persistent-viewer-contract-gap-analysis.md` is the authoritative input for this specification.
- The default generated graphical app profile is expected to be interactive when it includes Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Controls, and Controls.Elmish capabilities.
- Unsupported display hosts are acceptable only when reported as unsupported-environment outcomes; they cannot be counted as successful persistent launch evidence.
- Unsupported-host diagnostics are not sufficient completion evidence unless paired with at least one supported-host persistent launch artifact.
- Planning may choose exact naming and shape for public contracts, but the resulting user-visible capability must satisfy the persistent launch, input, diagnostics, and evidence separation requirements.

### Edge Cases

- A generated graphical app runs on a host with no display server; readiness must report unsupported environment rather than product success.
- A package exposes bounded viewer APIs but no persistent host contract; readiness must fail as a missing product/package capability.
- A feature declares graphical output but is intentionally headless or non-interactive; it must explicitly declare that scope and must not be assessed as an interactive graphical app.
- Keyboard input is irrelevant to a specific generated graphical feature; launch evidence may mark input dispatch as not applicable only when the feature scope excludes keyboard behavior.
- Bounded simulation is enabled for CI; it may produce bounded evidence but must not satisfy persistent launch evidence.
- Existing generated apps using bounded-only paths are evaluated under migration guidance rather than silently grandfathered as interactive apps.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of generated viewer-backed graphical app templates launch through the persistent app host on the default executable path, or produce a failed/unsupported diagnostic explaining why the persistent host could not be used.
- **SC-002**: 0 generated viewer-backed graphical apps pass generated guidance checks when the default executable only prints metadata, counts controls, runs bounded smoke, or exits without a persistent launch attempt.
- **SC-003**: 100% of interactive graphical readiness packages include at least one supported-host persistent graphical launch artifact, distinct from bounded smoke, deterministic scene evidence, and unsupported-host diagnostics.
- **SC-004**: Evidence audit rejects bounded-only substitution for interactive graphical readiness in all covered fixture and real generated-app cases.
- **SC-005**: On a supported desktop host, a generated Tetris-style app launched with its default executable command opens a persistent window, accepts declared keyboard input, renders model-derived state, and remains active until user exit.
- **SC-006**: On an unsupported host, the launch result identifies the outcome as unsupported environment, names the blocked stage, and gives an actionable reason instead of being counted as successful graphical readiness.
- **SC-007**: Reviewers can classify launch outcomes as successful, unsupported environment, or missing product/package capability within 2 minutes using the generated readiness artifacts.
- **SC-008**: Existing bounded viewer evidence commands continue to produce their intended bounded artifacts, with documentation that they do not replace persistent graphical launch evidence.
