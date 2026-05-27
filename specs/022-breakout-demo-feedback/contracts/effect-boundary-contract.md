# Contract: Effect Boundary Guidance

Generated app guidance must distinguish app transition commands from viewer
rendering, window behavior, screenshot capture, and host-side effects.

## Required Contract

- `update` is a pure transition from `Msg` and `Model` to next `Model` plus
  app-level commands.
- Generated host update maps application messages to next model state and
  viewer effects.
- Rendering is produced by `view` or host boundary code, not by app reducers.
- Filesystem, window, screenshot, and process effects are interpreted at the
  host or evidence command boundary.
- Generated docs and tests use consistent names for app commands, viewer
  effects, and host interpretation.

## Acceptance

- Reviewers can identify where app commands and viewer effects belong using the
  generated example alone.
- Generated guidance checks fail when examples append viewer render effects to
  app command lists.
