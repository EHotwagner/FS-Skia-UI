# Research: Fix Window Visibility

## Decision: Interactive success requires accessible native visibility

**Rationale**: A launch that creates a process or taskbar entry but no selectable, visible, focusable window is not usable by the developer. Success must require a native window that is created, mapped/visible where observable, attached to a renderable surface, and available for user close/input. First-frame rendering is necessary but not sufficient.

**Alternatives considered**:
- Treat process startup as launch success: rejected because it masks taskbar-only and hidden-window failures.
- Treat first-frame scene render as desktop visibility proof: rejected because off-screen or metadata render paths can draw without proving a native window is accessible.

## Decision: Close reasons are first-class outcome values

**Rationale**: The observed report class includes misleading `user close observed` claims. Launch outcome must separate user close, app-requested close, evidence-requested close, framework-requested close, host/system close, timeout, and failure-driven close so governance can reject false positives.

**Alternatives considered**:
- Keep boolean `user-close-observed`: rejected as insufficient by itself, though it can remain as a derived field.
- Infer close reason from mode: rejected because interactive apps may close themselves and evidence runs may fail or time out.

## Decision: Window-state diagnostics use observed/unsupported semantics

**Rationale**: Native visibility signals vary across Windows, X11, Wayland, compositors, and container hosts. Diagnostics must report stable fields but may mark values as unsupported or unobservable rather than guessing. This preserves actionable output without overpromising cross-host introspection.

**Alternatives considered**:
- Require identical native window inspection across all platforms: rejected as unrealistic with Wayland/compositor restrictions.
- Omit unobservable fields: rejected because reviewers need to know whether the runtime checked them.

## Decision: Window behavior requests are public options with honor/degrade results

**Rationale**: Resize policy, maximize policy, startup state, position, and backend preference affect the visible user experience. They belong in public viewer options and generated app configuration, with per-option diagnostics showing honored, degraded, unsupported, or failed.

**Alternatives considered**:
- Hardcode generated defaults: rejected because the feature explicitly requires configurable expected window behavior.
- Silently ignore unsupported options: rejected because unsupported hosts would appear to pass while not honoring user intent.

## Decision: Image evidence means a decodable image artifact

**Rationale**: A hash or text report cannot be opened as a screenshot. When users request screenshot or image evidence, validation must confirm the artifact is an actual image file using format/header/decode checks. Metadata and hashes remain allowed only when labeled as metadata/hash evidence.

**Alternatives considered**:
- Keep hash files under screenshot names: rejected as misleading evidence.
- Require screenshots for every host: rejected because some CI/container hosts cannot capture desktop images without compositor permissions.

## Decision: Scene-rendering evidence and desktop-window evidence are separate claims

**Rationale**: Off-screen scene rendering or pixel readback can prove the game scene draws, but it does not prove the native desktop window was visible to a user. Evidence results must state whether they prove scene rendering, desktop visibility, or both.

**Alternatives considered**:
- Treat any rendered pixels as visible-window proof: rejected because hidden/off-screen windows can still produce pixels.
- Ignore scene evidence when desktop capture is unavailable: rejected because scene evidence remains useful for non-interactive validation when labeled accurately.

## Decision: Generated validation rejects misleading evidence depth

**Rationale**: The generated app verification path must fail when package resolution drifts, generated tests do not run, launch diagnostics are incomplete, image evidence is not an image, or bounded evidence is substituted for interactive visibility. Otherwise the workflow reports success without proving the user-visible outcome.

**Alternatives considered**:
- Leave evidence review manual: rejected because this feature is a governance and runtime contract change.
- Use source scans only: rejected because scans cannot prove generated tests ran or image artifacts are valid.
