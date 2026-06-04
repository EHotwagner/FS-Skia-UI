# Evidence formats — required shapes

<!-- GENERATED from FS.Skia.UI.Build.Evidence.EvidenceFormatSchema (feature 062, FR-005).
     Single-sourced from the constants the validators enforce, so this reference, the
     failing-class diagnostics, and the scans/audit/task-parser cannot drift. Do not edit
     by hand; regenerate with ./fake.sh build -t RefreshSurfaceBaselines. Currency-checked
     by TargetMetadataDrift. -->

This reference lists, per evidence-format class, the complete required shape of each
enforced readiness file — so an author can recover the contract **before** triggering a
failure, without decompiling `FS.Skia.UI.Build.dll` or copying a sibling project (FR-005).

## readiness-contract

### `governance-risk-levels.md`

- required tokens: small, medium, broad, required evidence, broad validation
- blocking: true

### `aggregate-hang-diagnostics.md`

- required tokens: verdict, stage, elapsed duration, last observed command, focused rerun, non-authoritative aggregate
- blocking: true

### `runtime-limitations.md`

- required tokens: .NET 10 desktop, Vulkan, SkiaSharp preview, unsupported macOS/mobile/browser, no software-renderer fallback
- blocking: true

## skill-loading-evidence

### `skill-loading-evidence.md`

- required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception
- columns (in order): TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception
- ordering: loaded_at < work_started_at
- resolved-path: .agents/skills/<id>/SKILL.md
- blocking: true

## window-visibility

### `interactive-visible-window.md`

- required tokens: status, mode, window-visible, accessible-window, first-frame-presented, self-closed-for-evidence
- blocking: true

### `window-state-diagnostics.md`

- required tokens: diagnostic-class=environment-session, diagnostic-class=window-visibility, diagnostic-class=app-lifecycle, diagnostic-class=product-defect
- ordering: diagnostic-class ∈ { environment-session, window-visibility, app-lifecycle, product-defect }
- blocking: true

## seh-acceptance

### `tasks.md (Synthetic-Evidence Inventory)`

- required tokens: accepted-seh, synthetic-error-handling-approved
- ordering: acceptance status = accepted-seh; approval label = synthetic-error-handling-approved; no backticks
- blocking: true

