# Generated Product Usage

status=ok

Generated product defaults now use:

- persistent launch: `Viewer.runApp viewerOptions Product.Program.generatedHost`
- app-owned scene: `Product.Program.view`
- app-owned reducer: `Product.Program.update`
- screenshot evidence command: `--screenshot-evidence`
- shared report convention: leading `status`, `command`, `output`
- shared Scene geometry for layout, containment, collision, and rendering facts

Validation artifacts:

- `generated-viewer-guidance.md`
- `scene-shape-evidence.md`
- `screenshot-evidence.md`
- `effect-boundary-guidance.md`
- `evidence-report-conventions.md`
- `template-check-final.log`
- `generated-guidance-check-final.log`
- `template-drift-final.log`
