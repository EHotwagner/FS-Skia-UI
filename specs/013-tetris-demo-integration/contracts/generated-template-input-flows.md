# Contract: Generated Template Input Flows

## Template Obligations

Generated graphical app templates that include keyboard input must provide
viewer-key-driven flows for:

- Initial screen activation.
- Options navigation and selection when an options screen is generated.
- Main interaction controls.
- Pause, escape, or back behavior where present.
- End-screen restart or exit where present.

## Test Obligations

Generated tests must include at least one flow that starts from the initial
screen through a viewer key event. When options and end/restart screens are
generated, tests must cover those transitions through viewer key events too.

## Guidance Obligations

Generated quickstarts must document:

- Interactive run command.
- Bounded graphical smoke command.
- Headless or scene-level visual evidence command.
- Unsupported-environment diagnostic expectations.

## Evidence

- Generated product validation showing viewer-key start/options/interaction
  and restart flows.
- Template drift evidence for added or changed generated files.
- Readiness: `readiness/generated-template-input-flows.md`.
