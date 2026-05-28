# Research: Racer Feedback Follow-Ups

## Geometry Naming Guidance

**Decision**: Generated guidance and examples will avoid app-domain records
named only `Rect`, `Point`, or `Size` when `FS.Skia.UI.Scene` or layout
concepts are in scope. Use domain-specific names such as `WorldRect`,
`WorldPoint`, `TrackBounds`, `CarPose`, and `CheckpointBounds`.

**Rationale**: The racer consumer feedback showed F# inference friction when
local domain records collided with scene/layout types and constructors. Naming
domain concepts by coordinate role keeps generated code readable without extra
annotations in common scenarios.

**Alternatives considered**: Require explicit type annotations everywhere
scene/layout values cross domain boundaries. Rejected because it makes generated
samples noisy and leaves new users to discover avoidable ambiguity.

## Screenshot Success Semantics

**Decision**: Screenshot success means a live viewer window was captured after
first-frame presentation and produced a PNG artifact with a reported path,
positive width, positive height, `status=ok`, `evidence-kind=screenshot`, and a
live-window capture source.

**Rationale**: Deterministic render evidence proves rendering logic, but it is
not desktop visibility proof. The acceptance criteria require a real screenshot
artifact from the viewer window on at least one supported Windows or Linux
desktop host.

**Alternatives considered**: Treat deterministic render output as screenshot
proof when live capture is unavailable. Rejected because it would collapse two
different evidence classes and create false-positive visual proof.

## Screenshot Unsupported Capability Detail

**Decision**: Unsupported screenshot results will preserve separate facts for
viewer open status and capture availability whenever the host can determine
them. At minimum, results distinguish "viewer could not open" from "viewer
opened but screenshot capture is unavailable".

**Rationale**: The consumer feedback accepted unsupported screenshot reporting,
but asked for more actionable capability detail. Separating launch/open and
capture facts lets reviewers tell whether the issue is desktop/session launch,
viewer startup, or platform screenshot capability.

**Alternatives considered**: Keep only one `unsupported-host-reason` field.
Rejected because it is audit-friendly but too coarse for diagnosing supported
host gaps.

## Host Warning Classification

**Decision**: The known GTK module messages for `colorreload-gtk-module` and
`window-decorations-gtk-module` are classified as benign host warnings only when
first-frame launch evidence succeeds. The original warning text remains in the
evidence output.

**Rationale**: The warnings are host decoration/module noise in the feedback
case and did not block first-frame evidence. Preserving text keeps the audit
trail intact while avoiding false launch failures.

**Alternatives considered**: Suppress the warnings or classify all GTK messages
as benign. Rejected because suppression hides useful host context and broad GTK
matching could mask real launch failures.

## Detached Linux GUI Launch Guidance

**Decision**: Generated Linux background-launch guidance will recommend a
detached-session pattern that redirects stdout/stderr to a log and stdin from
`/dev/null`, for example `setsid dotnet run --project ... > logs/app.txt 2>&1 <
/dev/null &`.

**Rationale**: The feedback showed simple `nohup ... &` exited immediately with
no useful log, while `setsid` with explicit redirection stayed running. Guidance
should set expectations around process/session behavior and preserve logs.

**Alternatives considered**: Present `nohup` or shell backgrounding as the
default reliable method. Rejected because it was the exact unreliable pattern in
the consumer evidence.

## Dependency Strategy

**Decision**: Do not add a dependency during planning. Use existing viewer,
windowing, Testing, and platform capability surfaces first. Revisit dependency
governance only if implementation proves live screenshot capture cannot be done
with existing APIs on supported hosts.

**Rationale**: The feature is primarily additive evidence/reporting and
guidance work. New platform packages would increase maintenance and generated
template impact, so they need concrete implementation evidence before adoption.

**Alternatives considered**: Preselect a screenshot package. Rejected because
the host/capture gap should be proven in implementation against the existing
SkiaViewer stack first.
