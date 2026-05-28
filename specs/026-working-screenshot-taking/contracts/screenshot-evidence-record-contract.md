# Screenshot Evidence Record Contract

## Format

The screenshot evidence record is a machine-readable line-oriented report using
`key=value` fields, consistent with existing generated evidence reports.

## Required Fields

All screenshot records MUST include:

- `status`
- `command`
- `app-or-sample`
- `host-facts`
- `capture-mode`
- `evidence-kind`
- `artifact-path`
- `image-width`
- `image-height`
- `pixel-content-validation`
- `blocked-stage`
- `classification`
- `category`
- `message`
- `timestamp`

For `status=ok`, `artifact-path`, `image-width`, and `image-height` MUST name
the validated PNG and its decoded dimensions. For `status=unsupported` or
`status=failed`, missing artifact fields MUST be represented explicitly as
`none` or equivalent and the blocked stage must explain why.

## Acceptance

A record is accepted as screenshot proof only when:

- `status=ok`
- `evidence-kind=screenshot`
- `capture-source=live-viewer-window`
- `proves-screenshot=true`
- artifact path exists in the readiness package
- decoded dimensions are positive
- pixel validation reports non-blank content
- diagnostics do not reveal hidden warnings or fallback-only proof

Any metadata-only, deterministic-scene-only, manual, synthetic, unreadable,
blank, missing, or untraceable record is rejected.
