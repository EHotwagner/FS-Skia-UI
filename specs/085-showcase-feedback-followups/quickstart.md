# Quickstart — 085 ControlsShowcase Feedback Follow-ups

## What ships
1. `Control.renderTree` — faithful nested-tree → `Scene` rasterizer (real Yoga layout).
2. `InteractiveAppHost` + `Viewer.runInteractiveApp` — pointer-routing, size-aware host.
3. `KeyboardInput.normalize` — recognizes `Number{n}`/`Digit{n}`/`Keypad{n}`/`Key{n}`/`KeyL`.
4. Docs/skills — new `fs-skia-viewer-host` skill + updates to `fs-skia-typed-controls`,
   `scaffold-map.md`, `spec-template.md`, `evidence-formats.md`, `speckit-specify`.

## Build / validate order (run AFTER the contract-bearing edits exist)

```bash
# 1. Re-confirm Route escalates once src/**/*.fsi + template/** + .agents/skills/** changed
./fake.sh build -t Route            # expect tier=maintainer-verify (was focused-authority on spec-only diff)

# 2. Regenerate skill mirror + skillist after adding .agents/skills/fs-skia-viewer-host
./fake.sh build -t RefreshSurfaceBaselines

# 3. Escalated serialized six-target order (FAKE-backed → run SEQUENTIALLY)
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```
> FAKE-backed commands share `.fake` state — never run them concurrently.
> `GeneratedProductCheck` is known to fail locally for environment reasons
> (non-authoritative; see memory `generated-product-check-env-failure`).

## FSI smoke (Constitution I)
```fsharp
#r "FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Controls
// renderTree distinctness
let a = Control.renderTree Theme.Light { Width = 640; Height = 480 } pageA
let b = Control.renderTree Theme.Light { Width = 640; Height = 480 } pageB
a.Scene <> b.Scene   // true  → SC-001

#r "FS.Skia.UI.KeyboardInput.dll"
open FS.Skia.UI.KeyboardInput
ViewerKeyboard.normalize "Number5"  // Digit 5   → SC-003
ViewerKeyboard.normalize "KeyL"     // Letter 'L'
ViewerKeyboard.normalize "wat"      // Unknown "wat"
```

## Evidence to capture
- **SC-001**: per-page screenshot diff between two distinct pages is non-empty.
- **SC-002**: synthetic pointer press → bound `msg` dispatched + model changed (host path).
- **SC-003**: the five key spellings normalize correctly; unknown still `Unknown raw`.
- **SC-004**: size-aware render is sharp, OR one documented flag yields 1:1 sharp output.
- Window-visibility evidence class authored as **`key=value`** blocks (FR-015), not tables.
