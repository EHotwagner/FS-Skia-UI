# Contract: Generated Verification

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

- `exact-match=false` fails verification.
- `NU1603` fails verification.
- Local framework package versions require a generated or workflow-provided package source.
- Generated projects may alternatively request only versions available from configured public sources.

## Generated Test Execution

Generated `Test` and `Verify` targets must run the generated test project when it exists.

Required evidence fields:

```text
generated-tests-exist=true|false
generated-tests-ran=true|false
test-command=<command>
test-result=pass|fail|skipped
authoritative=true|false
```

Rules:

- `generated-tests-exist=true` with `generated-tests-ran=false` fails verification.
- Placeholder or source-scan-only targets must set `authoritative=false` and explain why.
- A target cannot print success without command evidence.

## Generated Game Runtime Verification

Generated graphical games must expose:

- Default interactive command with no evidence flag.
- Explicit bounded evidence command.
- Explicit visual evidence command when supported.
- Generated tests for board/grid presentation, keyboard-driven updates, and time-based progression.

The default executable path must not silently switch to bounded evidence, text-only metadata, or private runtime fallback when the user requested interactive play.

## Verification Failure Classes

Generated verification output must classify failures as one of:

- `environment-session`
- `package-resolution`
- `verification-depth`
- `app-lifecycle`
- `product-defect`

The failure class must be visible in readiness evidence so reviewers can decide whether to fix host setup, package feeds, target wiring, or runtime code.
