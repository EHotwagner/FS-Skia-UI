# Generated Guidance Contract

## Scope

Applies to screenshot-ready generated graphical app profiles and documentation.

## Requirements

Generated guidance MUST:

- expose a repeatable screenshot evidence command
- name the expected evidence record and PNG artifact paths
- explain that screenshot evidence is separate from launch, layout,
  deterministic scene, and pixel-readback evidence
- state that unsupported-host outcomes are real negative evidence, not success
- preserve normal interactive launch behavior
- tell reviewers where to find screenshot artifacts in the readiness package

Generated guidance MUST NOT:

- present metadata, deterministic scene output, static fixtures, or manual
  descriptions as screenshot proof
- hide capture failures behind a successful generated product result
- require screenshot capture during the default user launch path

## Validation

`GeneratedGuidanceCheck` and generated product tests must confirm that
screenshot-ready profiles include the command and guidance, and that
non-screenshot-ready/headless profiles are not forced to produce screenshots.
