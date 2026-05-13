# Feature Specification: Keyboard Input Framework

**Feature Branch**: `003-keyboard-input-framework`  
**Created**: 2026-05-13  
**Status**: Draft  
**Input**: User description: "add keyboard centric input framework modelled after https://github.com/EHotwagner/SystemAdmin/tree/main/KiFSharpAstTutorial/reports/ki-fsautocomplete-completion-stabilization-2026-05-05 the standard input does not need to use any command grammar, that is an optional advanced feature. the standard input schema is modelled after ki-editor and uses similar modes: https://ki-editor.org/docs/introduction stateful modes like selection, that always has a state, once modes that popup like space, temporary modes that are active as long as is held like copy or delete, bigram optimization is important, config in yaml, optional display of layout state. do extensive online research before creating the specs."

## Clarifications

### Session 2026-05-13

- Q: How should nested stateful, popup, and temporary held modes compose when more than one is active? -> A: Use a stack: base/stateful mode remains underneath popup and held modes; closing the top mode restores the previous context.
- Q: What trust boundary should apply to YAML input configuration? -> A: YAML may reference only application-registered command identifiers and validated input policies.
- Q: Should bigram optimization modify keymaps or only report recommendations in v1? -> A: Analysis only: report scores, risks, and suggested improvements without changing keymaps.

## Change Classification

- **Tier**: Tier 1 contracted change.
- **Public API impact**: Adds a new public `FS.Skia.UI.KeyboardInput` module with a curated `.fsi` surface for command registries, YAML-derived input configuration, canonical input models, runtime state, input messages, effects, diagnostics, replay, layout-state views, bigram reports, and optional command-intent data contracts.
- **Dependency impact**: Adds `YamlDotNet` pinned to `17.1.0` in the core library to parse declarative YAML configuration.
- **Verification approach**: Validate the `.fsi` shape through F# Interactive/prelude transcripts, semantic tests through the public API, surface-area baseline tests, invalid YAML/command registry tests, deterministic replay tests, layout-state tests, bigram report tests, sample smoke evidence, and performance evidence for input resolution, replay, and bigram analysis.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Define Keyboard-First Input Maps (Priority: P1)

As an application author, I want to define a keyboard-centric input map with positional key bindings, mode-specific bindings, and layout metadata so users can operate an application without relying on pointer-first workflows.

**Why this priority**: A usable input schema is the foundation for every other mode, display, and optimization feature.

**Independent Test**: Can be tested by loading a valid input configuration, pressing representative key sequences in each configured mode, and verifying that each sequence resolves to the expected application command or state transition.

**Acceptance Scenarios**:

1. **Given** a configuration with normal bindings and a persistent selection mode, **When** the user presses a configured movement key, **Then** the system reports the movement command together with the active selection mode state.
2. **Given** a configuration with a popup space mode, **When** the user presses the space-mode key, **Then** the system pushes the popup mode onto the active mode stack, resolves the next valid key inside that mode, pops the popup mode, and restores the prior state.
3. **Given** a configuration with a temporary copy or delete mode, **When** the user holds the configured key, **Then** the temporary mode remains active only while the key is held and is released when the key is released.

---

### User Story 2 - Preserve Stateful Modes (Priority: P1)

As a user, I want selection-like modes to always have an explicit state so actions are predictable and visible across repeated commands.

**Why this priority**: Ki-inspired editing depends on combining movements and actions with a current selection mode rather than treating line, character, or syntax operations as special cases.

**Independent Test**: Can be tested by switching between selection-like states, executing movements and actions, and verifying that the active state is preserved, changed, or reset only according to the configured transition rules.

**Acceptance Scenarios**:

1. **Given** a stateful selection mode with a current state, **When** the user performs an action that depends on the selected unit, **Then** the emitted command includes the selected unit and the state remains inspectable.
2. **Given** a stateful mode that has a default state, **When** an application initializes the input framework, **Then** the mode starts with a valid state rather than an empty or undefined state.
3. **Given** a stateful mode and a popup mode, **When** the popup mode closes, **Then** the prior stateful mode state is restored unless the popup action explicitly changed it.

---

### User Story 3 - Tune Ergonomic Layouts (Priority: P2)

As an application author or power user, I want the input map to include layout metadata and bigram scoring evidence so common command pairs can be placed ergonomically across supported keyboard layouts.

**Why this priority**: Positional, layout-aware bindings are central to the requested Ki-style model and prevent the framework from becoming a mnemonic-only shortcut layer.

**Independent Test**: Can be tested by loading a keymap with usage weights, running the layout analysis report, and verifying that frequent command pairs, same-finger risks, hold transitions, and travel-distance warnings are shown.

**Acceptance Scenarios**:

1. **Given** usage frequencies for command pairs, **When** the layout report is generated, **Then** it ranks the highest-impact bigrams and highlights ergonomic risks for the configured layout.
2. **Given** multiple supported keyboard layouts, **When** the same positional binding map is inspected under each layout, **Then** the report identifies the physical positions and displayed labels without changing the command identity.
3. **Given** a possible binding change that would improve a high-frequency bigram, **When** the report is generated, **Then** the report suggests the improvement and shows the expected score impact without modifying the configured keymap.

---

### User Story 4 - Configure and Inspect Input Behavior (Priority: P2)

As an application author, I want all standard input behavior to be described in YAML and validated before use so the input layer is reviewable, shareable, and safe to load.

**Why this priority**: The feature explicitly requires YAML configuration and early validation to avoid hidden runtime surprises.

**Independent Test**: Can be tested by loading valid and invalid YAML files, confirming typed validation messages for invalid files, and confirming that valid files produce a complete input model.

**Acceptance Scenarios**:

1. **Given** a YAML configuration with modes, bindings, hold behavior, one-shot behavior, and layout metadata, **When** the configuration is loaded, **Then** the system validates it and exposes a complete canonical input model.
2. **Given** an invalid binding, missing default state, duplicate key sequence, or impossible mode transition, **When** the configuration is loaded, **Then** the system rejects it with actionable messages that identify the affected mode and binding.
3. **Given** a YAML configuration that references an unregistered command identifier, **When** the configuration is loaded, **Then** the system rejects it before activation and identifies the unregistered command.
4. **Given** an application that enables layout-state display, **When** the active mode or held keys change, **Then** the application can show the current mode stack, active stateful mode, held temporary modes, and pending popup mode.

---

### User Story 5 - Support Advanced Command Intent (Priority: P3)

As an advanced application author, I want an optional command-intent layer that can expand concise commands into inspectable plans without being required for standard key input.

**Why this priority**: The referenced stabilization report recommends layered command handling, but the user explicitly says the standard input does not need a command grammar.

**Independent Test**: Can be tested by disabling the command-intent layer and verifying that standard key input still works, then enabling it and verifying that a concise command can produce an intent, plan, execution state, and event log.

**Acceptance Scenarios**:

1. **Given** command intent is disabled, **When** users operate the standard keymap, **Then** all configured modes and bindings work without requiring command grammar parsing.
2. **Given** command intent is enabled for a risky action, **When** a concise command is submitted, **Then** the system can expose the interpreted intent, planned steps, approval state, and resulting events.
3. **Given** an application has supplied command-intent status data for a failed advanced command, **When** the status view is requested, **Then** the user can see the requested intent, current plan state, failure reason, and relevant event history.

### Edge Cases

- A key is pressed while a temporary held mode is active and then released in a different order than it was pressed.
- A popup mode is opened while another popup mode is pending and must be pushed onto the same mode stack without losing the underlying stateful mode.
- A stateful mode has no valid default state in configuration.
- A key sequence is a prefix of another configured key sequence and the timeout expires before disambiguation.
- A keyboard layout changes after configuration has been loaded.
- A YAML file defines duplicate bindings, conflicting mode transitions, invalid layout positions, or unknown command names.
- A YAML file attempts to define an unregistered command or arbitrary host action instead of referencing a registered command identifier.
- A key-up event is lost or arrives after the application has lost focus.
- Optional layout-state display is disabled but diagnostics still need to expose input state for tests and debugging.
- Advanced command intent is enabled but produces an unsatisfiable plan.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST load a human-editable YAML input configuration that defines modes, mode states, key bindings, key sequence timing, hold behavior, one-shot behavior, layout metadata, command identifiers, and optional display preferences.
- **FR-002**: System MUST validate input configuration before activation and reject invalid configurations with messages that identify the affected mode, binding, transition, command identifier, or layout entry.
- **FR-003**: System MUST convert every valid configuration into a canonical input model that applications can inspect without parsing raw YAML.
- **FR-004**: System MUST support stateful modes that always have a current state, including a valid initial state, explicit state transitions, and state restoration after temporary or popup modes close.
- **FR-005**: System MUST support one-shot popup modes that push onto the active mode stack for a bounded next input or explicit cancellation and then pop to restore the prior mode context.
- **FR-006**: System MUST support temporary held modes that push onto the active mode stack on key press, remain active while the key is held, and pop on key release or focus-loss recovery.
- **FR-007**: System MUST distinguish standard input binding resolution from optional advanced command grammar so applications can use the keyboard framework without enabling command-intent parsing.
- **FR-008**: System MUST resolve key inputs using physical position, displayed key label, active keyboard layout, active mode context, and configured disambiguation rules.
- **FR-009**: System MUST support positional keymaps so command identity can remain stable across QWERTY, Dvorak, Colemak-style, and custom layout labels.
- **FR-010**: System MUST provide bigram analysis for configured command sequences using declared usage weights, physical key positions, same-finger or awkward transitions, hold interactions, and travel distance.
- **FR-011**: System MUST provide an ergonomic report that ranks the most important command pairs, identifies bindings whose configured placement is likely to harm keyboard-centric use, and suggests improvements without modifying the configured keymap.
- **FR-012**: System MUST expose current layout state for optional display, including the active mode stack, active stateful mode, temporary held modes, pending one-shot or popup mode, active keyboard layout, and pending key sequence.
- **FR-013**: System MUST emit an ordered input event history suitable for debugging and deterministic replay of mode transitions and binding resolution.
- **FR-014**: System MUST provide clear cancellation behavior for popup modes, pending key sequences, temporary held modes, and optional command-intent plan status data supplied by the host application.
- **FR-015**: System MUST allow applications to map resolved input commands to their own domain messages without requiring the input framework to own application state.
- **FR-016**: System MUST preserve the existing application state ownership boundary: input resolution may report commands and mode transitions, but domain state remains owned by the consuming application.
- **FR-017**: System MUST provide diagnostics for ignored, ambiguous, stale, repeated, or out-of-order input events.
- **FR-018**: System MUST support optional advanced command-intent data contracts that can expose interpreted intent, generated plan, execution status, event log, failure report, and user approval state when an application opts in; v1 MUST NOT require the framework to parse command grammar or execute command plans.
- **FR-019**: System MUST allow command-intent policies and templates to be configured separately from standard key bindings so advanced automation can be adopted incrementally.
- **FR-020**: System MUST treat YAML as declarative configuration that may reference only application-registered command identifiers and validated input policies; it MUST NOT define arbitrary host actions such as shell or process execution.
- **FR-021**: System MUST include examples that demonstrate a standard modal input map, a stateful selection mode, a popup space mode, temporary copy/delete modes, bigram analysis, YAML validation failure, unregistered command rejection, and optional layout-state display.

### Key Entities

- **Input Configuration**: User-editable configuration containing modes, bindings, layout metadata, timing rules, display options, command identifiers, and optional command-intent policies.
- **Command Registry**: Application-provided list of command identifiers that YAML bindings and optional command-intent policies are allowed to reference.
- **Canonical Input Model**: Validated representation of input behavior used by applications and tests after configuration is loaded.
- **Mode**: A named input context that may be stateful, one-shot popup, temporary held, or standard.
- **Mode Stack**: Ordered active input contexts where popup and temporary held modes sit above the base stateful context and are removed to restore the previous context.
- **Mode State**: The current value for a stateful mode, such as a selection unit or other active input context.
- **Binding**: A mapping from a physical key position or key label, within a mode context, to a command, state transition, popup, temporary mode, or cancellation.
- **Layout Profile**: Keyboard-layout metadata that maps physical positions to labels and provides ergonomic information for display and analysis.
- **Bigram Profile**: Usage-weighted command-pair and key-pair data used to evaluate layout ergonomics and generate non-mutating recommendations.
- **Input Event**: A recorded key press, key release, focus change, timeout, mode transition, command resolution, cancellation, or diagnostic event.
- **Layout State View**: Optional user-facing summary of active modes, held keys, pending one-shot mode, active layout, and pending sequence.
- **Command Intent**: Optional advanced representation of a concise high-level command.
- **Command Plan**: Optional advanced plan generated from command intent, policies, and current state.
- **Failure Report**: Diagnostic summary explaining why input resolution or optional command execution failed.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 95% of valid key press or release events resolve to a command, state transition, pending sequence, or explicit no-op diagnostic within 16 ms during automated input tests.
- **SC-002**: 100% of invalid sample YAML configurations are rejected before activation with at least one actionable diagnostic identifying the affected configuration location.
- **SC-002A**: 100% of sample YAML configurations that reference unregistered commands or arbitrary host actions are rejected before activation.
- **SC-003**: Users can inspect the active mode stack, active stateful mode state, held temporary modes, pending popup mode, and active layout within one visible status element when layout-state display is enabled.
- **SC-004**: 100% of configured replay tests reproduce the same command, diagnostic, layout-state, and mode-transition sequence from the same recorded input events.
- **SC-005**: Bigram analysis reports the top 20 usage-weighted command pairs, flags same-finger, awkward hold, or long-travel risks for each supported sample layout, and makes no automatic changes to the configured keymap.
- **SC-006**: A first-time application author can configure a basic keyboard map with one stateful mode, one popup mode, and one temporary held mode in under 30 minutes using the provided examples.
- **SC-007**: Standard key input remains fully usable when the optional command-intent layer is disabled, with no command-grammar configuration required.
- **SC-008**: Optional command-intent examples expose configured intent data, plan status, current state, event log, and failure information for 100% of demonstrated advanced command-intent records without requiring command grammar execution.

## Assumptions

- The feature is added to the core UI library surface because keyboard input is a cross-cutting viewer capability, while consuming applications continue to own domain model updates.
- YAML is required for user-edited configuration even though the runtime model should be typed and validated before activation.
- YAML configuration is declarative and cannot directly execute host actions; applications decide what registered command identifiers mean.
- The default standard schema is inspired by Ki-style modal input: stateful selection-like modes, popup space-like modes, held temporary modes, stack-based mode restoration, positional keymaps, and optional layout-state display.
- Command grammar and declarative command planning are advanced opt-in capabilities, not prerequisites for standard key input.
- The first release supports desktop keyboard input and does not attempt touch, gamepad, or mobile soft-keyboard workflows.
- Bigram analysis uses configured or sample usage weights when real user telemetry is unavailable and produces recommendations only; applying changes is a user or application-author decision.
- The input framework records diagnostic events, but persistent long-term analytics are outside the baseline scope.
- Research basis included Ki Editor documentation on selection modes, positional keymaps, bigram optimization, space menus, actions, and configuration; the Ki + FSAutocomplete stabilization report on layered command intent, plans, state, event logs, and YAML policies; Helix documentation on normal/select/popup modes and statusline mode display; Kakoune documentation on multiple selections as a central editing primitive; and QMK/ZMK documentation on one-shot and hold-tap behavior.
