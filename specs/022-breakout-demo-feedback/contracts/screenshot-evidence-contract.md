# Contract: Screenshot Evidence

Generated apps may request live viewer screenshot evidence, but reports must be
honest about host capability.

## Success Result Fields

- `status=ok`
- `command`
- `evidence-kind=screenshot`
- `output`
- `screenshot-path`
- `width`
- `height`
- `renderer-mode`
- `frames-rendered`
- diagnostics or message

## Unsupported Result Fields

- `status=unsupported`
- `command`
- `evidence-kind=screenshot`
- `unsupported-host-reason`
- `fallback=deterministic-scene-evidence`
- diagnostics or message

## Acceptance

- A supported desktop host produces a bounded screenshot artifact with
  machine-readable dimensions and output path.
- An unsupported host does not fail as a product defect solely because capture
  is unavailable.
- Unsupported results never claim screenshot proof and always name the
  deterministic fallback.
