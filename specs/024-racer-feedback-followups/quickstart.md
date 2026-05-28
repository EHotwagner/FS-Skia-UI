# Quickstart: Racer Feedback Follow-Ups

## 1. Confirm Baseline

```bash
dotnet fsi build.fsx --target Verify
dotnet fsi build.fsx --target GeneratedGuidanceCheck
dotnet fsi build.fsx --target TemplateCheck
```

Record baseline status in:

```text
specs/024-racer-feedback-followups/readiness/baseline-status.md
```

## 2. Add Failing-First Tests

Add semantic/governance tests before implementation for:

- generated geometry naming guidance rejects `Rect`, `Point`, and `Size` as
  app-domain recommendations when scene/layout primitives are in scope
- screenshot success requires PNG path, positive dimensions, first-frame
  presentation, and live-window capture source
- unsupported screenshot results separate viewer-open status from capture
  availability
- known GTK module warnings are benign only when first-frame evidence succeeds
  and original text is preserved
- Linux detached launch guidance includes detached session, log capture, and
  stdin redirection

## 3. Implement Additive Contracts

Follow constitution order:

```text
.fsi signature -> semantic tests/FSI transcript -> .fs implementation -> surface baseline/docs
```

Keep generated app state workflows unchanged. Put host/window/filesystem work at
the evidence interpreter edge and expose capability facts as data.

## 4. Validate Guidance and Template Output

```bash
dotnet fsi build.fsx --target GeneratedGuidanceCheck
dotnet fsi build.fsx --target TemplateCheck
dotnet fsi build.fsx --target TemplateDrift
```

Record checked files, accepted examples, and rejected stale patterns in:

```text
specs/024-racer-feedback-followups/readiness/generated-guidance-validation.md
specs/024-racer-feedback-followups/readiness/detached-launch-guidance.md
```

## 5. Collect Screenshot Evidence

On a supported Windows or Linux desktop host, run the generated screenshot
evidence command and confirm:

```text
status=ok
evidence-kind=screenshot
artifact-path=<png>
width=<positive>
height=<positive>
capture-source=live-viewer-window-after-first-frame
```

Record success in:

```text
specs/024-racer-feedback-followups/readiness/screenshot-success-artifact.md
```

For unsupported capture or an unavailable supported OS, record capability detail
in:

```text
specs/024-racer-feedback-followups/readiness/screenshot-capability-detail.md
```

## 6. Validate Host Warning Classification

Run or replay real launch output that contains the known GTK module warnings and
successful first-frame evidence. Confirm the warnings remain in output and are
classified as benign host warnings without failing launch readiness.

Record results in:

```text
specs/024-racer-feedback-followups/readiness/host-warning-classification.md
```

## 7. Final Governance Checks

```bash
dotnet fsi build.fsx --target EvidenceGraph
dotnet fsi build.fsx --target EvidenceAudit
dotnet fsi build.fsx --target Verify
```

Acceptance requires all required readiness files, passing graph/audit checks,
and no unresolved synthetic screenshot proof.
