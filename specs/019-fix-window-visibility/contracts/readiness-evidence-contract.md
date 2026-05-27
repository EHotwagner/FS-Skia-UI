# Contract: Readiness Evidence

## Required Files

The feature is not implementation-ready until task generation includes these readiness files as explicit obligations:

- `readiness/interactive-visible-window.md`
- `readiness/close-reason-separation.md`
- `readiness/window-state-diagnostics.md`
- `readiness/window-options.md`
- `readiness/real-image-evidence.md`
- `readiness/generated-validation.md`
- `readiness/evidence-audit.md`

## Interactive Visible Window Evidence

Must include:

```text
mode=interactive-window
window-opened=true
window-visible=observed:true
focusable=observed:true|unsupported
first-frame-presented=true
self-closed-for-evidence=false
close-reason=user-close|app-requested-close|host-system-close
```

Acceptance requires proof that first-frame presentation alone did not close the session and that the visible window was accessible on a supported desktop host. Fake loop evidence may support regression tests but cannot replace supported-host visibility evidence.

## Close Reason Separation

Must include exercised close paths:

```text
close-path=<user|app|evidence|framework|host|timeout|failure>
reported-close-reason=<reason>
user-close-observed=true|false
evidence-close-observed=true|false
```

Acceptance requires zero cases where `user-close-observed=true` without a user/native close event.

## Window State Diagnostics

Must include:

```text
diagnostic-class=window-visibility
window-initialized=true|false
native-handle=observed:true|observed:false|unsupported|unavailable
visible=observed:true|observed:false|unsupported|unavailable
focusable=observed:true|observed:false|unsupported|unavailable
focused=observed:true|observed:false|unsupported|unavailable
minimized=observed:true|observed:false|unsupported|unavailable
maximized=observed:true|observed:false|unsupported|unavailable
client-size=<width>x<height>|unsupported|unavailable
renderable-surface=observed:true|observed:false|unsupported|unavailable
input-devices=observed:true|observed:false|unsupported|unavailable
backend=<backend-or-empty>
failure-class=<class-or-empty>
```

Taskbar-only, hidden, unmapped, off-screen, zero-sized, minimized-only, and surface-less cases must be classified as degraded or failed when observed.

## Window Options Evidence

Must include one row per requested option:

```text
option=<resize|maximize|startup-state|startup-position|backend>
requested=<value>
observed=<value-or-empty>
status=honored|degraded|unsupported|failed
message=<reason>
```

Acceptance requires unsupported settings to be named explicitly rather than ignored.

## Real Image Evidence

Preferred:

```text
evidence-kind=image
path=<image-path>
image-decodable=true
proves-scene-rendering=true
proves-desktop-visibility=true|false
```

Fallback:

```text
evidence-kind=pixel-readback
path=<artifact-path>
fallback-reason=screenshot-unavailable
proves-scene-rendering=true
proves-desktop-visibility=false
```

Metadata/hash:

```text
evidence-kind=metadata-hash
path=<artifact-path>
proves-scene-rendering=false
proves-desktop-visibility=false
```

Unsupported host:

```text
evidence-kind=unsupported-host
unsupported-reason=<specific reason>
```

Text-only hashes are never screenshot evidence.

## Generated Validation Evidence

Must include command evidence for generated restore/package resolution, generated test execution when present, default interactive launch validation, bounded evidence validation, window option validation, image evidence validation, and final verdict.

## Audit Expectations

`EvidenceAudit` must fail when required readiness files are missing, when a process/taskbar entry is substituted for visible-window evidence, when evidence close is reported as user close, when requested image evidence is metadata-only, when package mismatch is unresolved, or when generated tests exist but did not run.
