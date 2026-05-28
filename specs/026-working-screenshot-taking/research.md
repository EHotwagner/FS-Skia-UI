# Research: Working Screenshot Taking

## Decision: Use viewer-owned first-frame render-target capture first

**Rationale**: The repo already owns the viewer lifecycle, first-frame evidence,
SkiaSharp image encoding, and screenshot evidence result contracts. Capturing
from the viewer/rendered surface keeps the evidence tied to working product code
without adding OS-specific screenshot dependencies. This also gives the workflow
control over dimensions, pixel validation, blocked stage, and cleanup.

**Alternatives considered**:

- External desktop screenshot tools: rejected for Phase 2 because they add
  platform variance, window matching, permissions, and dependency governance.
- Deterministic scene image generation: rejected as screenshot proof because it
  bypasses the viewer-backed rendered-output path.
- Static PNG fixture: rejected because it is synthetic and cannot satisfy the
  feature's real evidence requirement.

## Decision: Accepted screenshot proof requires readable PNG and non-blank pixels

**Rationale**: A file path alone does not prove screenshot success. The
validator must decode the image, require positive dimensions, and reject blank
or fully transparent output. The evidence record must store the validation
result so reviewers can inspect proof without rerunning locally.

**Alternatives considered**:

- Trust dimensions written by the capture routine: rejected because corrupt,
  empty, or stale files could pass.
- Hash-only proof: rejected because it is not reviewable visual evidence.
- Human-only inspection: rejected because audit needs machine-readable failure
  reasons.

## Decision: Keep screenshot evidence distinct from existing evidence kinds

**Rationale**: The repo already distinguishes persistent launch, bounded runs,
deterministic scene evidence, layout facts, and pixel-readback diagnostics.
Screenshot evidence must not make those proof kinds ambiguous. Existing paths
remain useful diagnostics but cannot substitute for a captured image.

**Alternatives considered**:

- Treat pixel-readback or deterministic render as screenshot proof: rejected
  because the spec requires a screenshot artifact from rendered app output.
- Replace launch/layout evidence with screenshots: rejected because those
  evidence classes answer different readiness questions.

## Decision: Failure diagnostics use blocked-stage classification

**Rationale**: Screenshot capture can fail after successful launch or after
first-frame rendering, so one generic failure message would hide actionable
facts. The record must distinguish desktop prerequisite, launch, first frame,
render, capture/readback, pixel validation, file write, timeout, and unsupported
host prerequisites.

**Alternatives considered**:

- Return only `failed` or `unsupported`: rejected because reviewers need to know
  where the proof path stopped.
- Classify unsupported host as success with fallback evidence: rejected because
  unsupported hosts are real negative evidence, not screenshot proof.

## Decision: Generated app guidance exposes screenshot capture as an explicit evidence command

**Rationale**: Generated graphical apps must preserve normal interactive launch
behavior. Evidence-mode capture may launch, render, write files, and close
itself, but those effects belong to a dedicated command path and report.

**Alternatives considered**:

- Capture during default launch: rejected because it changes product behavior.
- Require manual screenshots only: rejected because it cannot satisfy automated
  readiness and audit requirements.
