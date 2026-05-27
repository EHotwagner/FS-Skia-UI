# Research: Breakout Demo Feedback

## Decision: Generated Guidance Uses One Packaged Viewer Launch Contract

Generated source, tests, quickstart text, and guidance checks will reference one
viewer launch contract that exists in the packaged public surface consumed by a
fresh generated app. The current source exposes both `Viewer.runApp` and
`Viewer.runAppWithWindowBehavior`; implementation must prove which name is
available in the locally packed package before generated guidance requires it.

**Rationale**: BreakoutDemo1 exposed drift between generated text and the
consumer package. Generated app authors should not edit placeholders or comments
to satisfy stale guidance.

**Alternatives considered**: Keep both names in generated docs. Rejected
because dual guidance preserves ambiguity and makes governance checks weaker.

## Decision: Add Filled Circle And Ellipse As First-Class Scene Concepts

Scene will expose filled circle and filled ellipse concepts suitable for balls,
bullets, handles, markers, and radial indicators. The public model must support
deterministic evidence for bounds, fill, and placement. Painted variants may be
added if they follow the same `.fsi`-first and evidence model, but filled shapes
are the minimum contract.

**Rationale**: Existing `Ellipse` support is paint-based and there is no obvious
filled circle constructor for generated apps. Rectangular substitutions weaken
game examples and make evidence less representative.

**Alternatives considered**: Use paths or rounded rectangles in every generated
app. Rejected because that pushes basic scene modeling into consumers and makes
evidence less direct.

## Decision: Screenshot Evidence Is Success-Or-Unsupported, Never Implied

Screenshot evidence will produce bounded machine-readable screenshot facts when
viewer/host capture is available. When capture is unavailable, the command must
return an explicit unsupported result with command, status, reason, and
recommended deterministic fallback. Deterministic pixel/readback evidence remains
valid but must not be named screenshot proof.

**Rationale**: The existing framework can prove deterministic render output and
persistent launch separately. Claiming live screenshot proof without screenshot
facts would violate evidence honesty.

**Alternatives considered**: Treat pixel readback as screenshot evidence.
Rejected because it does not prove live desktop capture.

## Decision: Generated Examples Separate App Commands From Viewer Effects

Generated examples will show a pure app `update` that returns app-level commands
and a host boundary that emits viewer effects such as `RenderScene`,
window behavior, screenshot capture, or evidence writes.

**Rationale**: The architecture is correct but easy to misuse because app
commands and viewer effects are both effect-like values. A complete generated
example makes the boundary observable.

**Alternatives considered**: Rely on compiler errors. Rejected because the error
does not explain the layering to generated app authors.

## Decision: Reuse Scene Geometry And Standard Evidence Report Helpers

Generated guidance will prefer Scene geometry types for rendering bounds,
layout evidence, collision bounds, and containment checks when they fit the app
model. Generated evidence commands will use a standard key-value report
convention for parent directory creation, stable ordering, stdout echoing,
normalized statuses, unsupported-host fields, and exit behavior.

**Rationale**: BreakoutDemo1 hit record-label ambiguity with duplicate geometry
records and had to hand-roll repeated report behavior. Standard helpers reduce
generated app drift and governance brittleness.

**Alternatives considered**: Leave geometry and reports app-local. Rejected
because the same patterns recur across generated game apps.
