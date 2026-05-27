# Contract: Generated Validation

## Generated Runtime Commands

Generated graphical games must expose:

```text
default-command=<interactive launch command>
evidence-command=<explicit bounded/evidence command>
image-evidence-command=<explicit image evidence command or unsupported-host diagnostic>
```

Rules:

- The default command launches the normal interactive visible-window path.
- Evidence commands are explicit and must not run accidentally as the default command.
- Private runtime directory or off-screen fallbacks cannot be presented as visible interactive launch success.

## Package Resolution

Generated verification must record:

```text
requested-package=<id>/<version>
resolved-package=<id>/<version>
package-source=<name-or-path>
exact-match=true|false
restore-warning=<warning-code-or-empty>
```

Rules:

- `exact-match=false` fails validation.
- `NU1603` fails validation.
- Local framework package versions require a generated or workflow-provided package source.

## Generated Test Execution

Generated `Test` and `Verify` targets must run generated tests when present.

Required evidence fields:

```text
generated-tests-exist=true|false
generated-tests-ran=true|false
test-command=<command>
test-result=pass|fail|skipped
authoritative=true|false
```

Rules:

- `generated-tests-exist=true` with `generated-tests-ran=false` fails validation.
- Placeholder or source-scan-only targets must set `authoritative=false` and explain why.
- A target cannot print success without command evidence.

## Window Visibility Validation

Generated validation must record:

```text
mode=interactive-window
window-opened=true|false
window-visible=observed:true|observed:false|unsupported|unavailable
first-frame-presented=true|false
status=ok|degraded|unsupported|failed
failure-class=<class-or-empty>
```

Rules:

- `window-opened=true` alone is not a pass.
- Taskbar-only, hidden, minimized-only, off-screen, zero-sized, unmapped, and surface-less states fail or degrade interactive validation.
- Unsupported hosts must report why visibility could not be proven.

## Close Reason Validation

Generated validation must record each close path it exercises:

```text
close-path=<user|app|evidence|framework|host|timeout|failure>
close-reason=<reported reason>
user-close-observed=true|false
```

Rules:

- `user-close-observed=true` is valid only for real user/native close.
- Evidence self-close and timeout must not be reported as user close.
- App-requested close must not be reported as framework close.

## Visual Evidence Validation

Image evidence must record:

```text
evidence-kind=image|pixel-readback|metadata-hash|unsupported-host
path=<artifact-path-or-empty>
image-decodable=true|false|not-applicable
proves-scene-rendering=true|false
proves-desktop-visibility=true|false
```

Rules:

- Requested image evidence fails if the artifact is not a decodable image.
- Hash or metadata files must be labeled `metadata-hash`, not screenshot/image.
- Pixel readback alone may prove scene rendering but not desktop visibility.
- Unsupported-host evidence must include a specific reason.

## Failure Classes

Generated validation output must classify failures as one of:

- `environment-session`
- `window-visibility`
- `window-options`
- `visual-evidence`
- `package-verification`
- `verification-depth`
- `app-lifecycle`
- `product-defect`

The failure class must be visible in readiness evidence so reviewers can decide whether to fix host setup, runtime code, generated target wiring, or package feeds.
