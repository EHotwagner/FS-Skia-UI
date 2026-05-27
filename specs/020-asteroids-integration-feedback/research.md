# Research: Asteroids Integration Feedback

## HUD Readability Proof

Decision: Treat readable HUD layout as a separate proof from deterministic
scene rendering. Evidence must include a HUD region, gameplay region, relevant
text bounds, and overlap status before it can claim layout readability.

Rationale: Existing scene evidence can prove deterministic render metadata or a
hash, but a hash does not explain whether text is readable or whether gameplay
content covers it. Explicit region and bound facts make failures actionable and
auditable.

Alternatives considered: Reuse deterministic hashes as readability proof. This
was rejected because two unreadable layouts can still produce stable metadata.

## Text Bounds

Decision: Use exact text bounds when exposed by the rendering/layout host. When
exact metrics are unavailable, use deterministic conservative approximations
and mark the evidence as approximate. If neither exact nor conservative bounds
are available, report an unsupported layout-inspection reason and do not claim
readability.

Rationale: The spec accepts approximate bounds, but only when they are stable
and conservative enough to catch likely collisions. Unsupported facts are safer
than silently passing unverifiable claims.

Alternatives considered: Require exact font metrics for all hosts. This was
rejected because it would make generated validation unavailable on otherwise
usable hosts before the framework has a universal metric API.

## Region Ownership

Decision: Generated game samples reserve a named HUD/status region and a named
gameplay region. Gameplay entity movement, wrapping, collision bounds, and
spawn positions must be computed inside the gameplay region.

Rationale: Readability cannot be preserved if gameplay still treats the whole
scene as its movement area. Region ownership gives both implementation and
evidence a shared coordinate contract.

Alternatives considered: Draw the HUD above gameplay with z-order only. This
was rejected because z-order can preserve visibility while still allowing
gameplay content to obscure or crowd text.

## Public Name Guidance

Decision: Public guidance and generated examples use app-owned qualified names:
`Product.Program.view` for the scene-producing function, `Product.Program.generatedHost`
for the generated viewer host, and `Product.Program.update` when tests or
signatures call the reducer.

Rationale: Qualified names prevent accidental binding to framework helpers with
common names such as `update`, while keeping the generated app contract easy to
find from source and docs.

Alternatives considered: Rely on unqualified `view`, `generatedHost`, and
`update`. This was rejected because open framework namespaces can make common
names ambiguous.

## Host Warning Classification

Decision: Classify known benign desktop host warnings as non-fatal environment
noise only when the launch remains usable and layout/render/package checks
pass. Real launch, rendering, layout, package, and missing evidence failures
remain fatal.

Rationale: Readiness should not fail solely because a desktop host prints a
known non-fatal module warning, but warning classification must not mask
actionable failures.

Alternatives considered: Ignore all host warnings after successful launch. This
was rejected because some warnings indicate missing modules or rendering paths
that should remain visible.

## Skill Governance

Decision: Add a repo-local capability skill named `fs-skia-layout-evidence` and
require it in task metadata for generated game layout readability, scene layout
evidence, public scene/host/update guidance, generated validation, and host
warning classification work.

Rationale: The feature combines layout design, evidence claims, and generated
validation. A required skill keeps future implementation and task generation
aligned with the public contract.

Alternatives considered: Reuse only generic Spec Kit task guidance. This was
rejected because the spec explicitly requires a feature-specific layout/evidence
capability.
