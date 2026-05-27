# Data Model: Asteroids Integration Feedback

## HUD Region

Fields:

- `Id`: stable region identifier, for example `hud`.
- `Bounds`: rectangle in scene coordinates.
- `Purpose`: score, lives, wave, status, or equivalent generated-game state.
- `MinimumReadableSize`: documented minimum supported size for generated validation.

Validation rules:

- Bounds must have positive width and height.
- Bounds must not overlap gameplay region unless the evidence explicitly marks
  intentional layering and does not claim non-overlap readability.
- HUD text bounds must be contained in the HUD region for readability claims.

## Gameplay Region

Fields:

- `Id`: stable region identifier, for example `gameplay`.
- `Bounds`: rectangle in scene coordinates.
- `MovementPolicy`: wrap, clamp, spawn, and collision coordinate policy.
- `Entities`: active gameplay bounds relevant to overlap detection.

Validation rules:

- Bounds must have positive width and height.
- Active gameplay entities must stay within gameplay bounds in validation
  scenarios unless intentionally layered and disclosed.
- Movement and wrap calculations must use gameplay bounds, not the full scene.

## Text Bound

Fields:

- `Label`: semantic name such as `score`, `lives`, `wave`, or `status`.
- `Text`: rendered text or redacted equivalent.
- `Bounds`: rectangle in scene coordinates.
- `Measurement`: `Exact`, `Approximate`, or `Unsupported`.
- `UnsupportedReason`: required when measurement is unsupported.

Validation rules:

- Exact or approximate bounds are required for readability claims.
- Unsupported text bounds fail readability claims unless the report explicitly
  says layout proof is unsupported.
- Text bounds must not overlap other HUD text bounds or gameplay entity bounds.

## Scene Layout Evidence

Fields:

- `SceneName`: generated or sample scene identifier.
- `OutputSize`: validated width and height.
- `HudRegion`: HUD region facts.
- `GameplayRegion`: gameplay region facts.
- `TextBounds`: HUD/status text bounds.
- `GameplayBounds`: active gameplay entity bounds used for overlap detection.
- `ProofLevel`: `ReadableLayout`, `DeterministicRenderOnly`, or `UnsupportedLayoutInspection`.
- `Diagnostics`: actionable messages for missing facts, overlaps, and unsupported
  host or metric capability.

Validation rules:

- `ReadableLayout` requires HUD region, gameplay region, text bounds, and
  non-overlap checks.
- `DeterministicRenderOnly` may include hashes or metadata but must not claim
  HUD readability.
- `UnsupportedLayoutInspection` must include an unsupported reason.

## Public Contract Guidance

Fields:

- `SceneFunctionName`: qualified app-owned scene function.
- `HostValueName`: qualified generated host value.
- `UpdateFunctionName`: qualified app-owned reducer.
- `AmbiguityNotes`: guidance for avoiding unqualified framework/app conflicts.

Validation rules:

- Guidance must use consistent names across docs, generated examples, and tests.
- Tests must qualify app update calls when framework namespaces are opened.

## Host Warning Classification

Fields:

- `RawMessage`: host warning text or normalized warning code.
- `Class`: `BenignEnvironmentWarning`, `LaunchFailure`, `RenderingFailure`,
  `LayoutFailure`, `PackageFailure`, or `UnknownWarning`.
- `Fatal`: whether readiness fails because of the message.
- `Evidence`: launch/render/layout/package evidence that supports the
  classification.

Validation rules:

- Benign warnings are non-fatal only when usable launch and required evidence
  are present.
- Unknown warnings remain visible and cannot suppress real failures.

## Layout Evidence Skill

Fields:

- `Id`: must be `fs-skia-layout-evidence`.
- `Path`: readable repo-local `SKILL.md`.
- `Scope`: generated game HUD readability, layout evidence, public contract
  guidance, generated validation, and host warning classification.

Validation rules:

- `template/capabilities.yml` or the task skill inventory must resolve the id
  to exactly one readable skill file.
- `tasks.deps.yml` and matching `tasks.md` lines must list the skill for all
  applicable tasks.
