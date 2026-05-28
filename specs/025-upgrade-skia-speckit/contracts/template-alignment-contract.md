# Contract: Template Alignment Evidence

## Purpose

Define how generated project assets prove they match the approved package and
Spec Kit version posture.

## Required Fields

| Field | Required | Meaning |
|-------|----------|---------|
| `profile` | yes | Generated profile checked. |
| `package-pins` | yes | Package versions emitted by the generated profile. |
| `spec-kit-assets` | yes | Generated Spec Kit metadata, templates, extensions, workflows, and skills included. |
| `broad-package-dependency` | yes | Whether the generated profile references `FS.Skia.UI` directly. |
| `expected-posture` | yes | Focused-package, compatibility-selected, or docs-only behavior. |
| `validation-command` | yes | Command used to generate or validate the profile. |
| `validation-status` | yes | Pass/fail/unsupported with evidence path. |

## Acceptance Rules

- Supported generated profiles must validate after the version update.
- Generated package pins must match approved repository package posture.
- Generated Spec Kit assets must match approved root asset version/range or
  document a deliberate compatibility exception.
- No generated profile may gain broad `FS.Skia.UI` dependency accidentally.
