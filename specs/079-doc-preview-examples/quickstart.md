# Quickstart: Demonstrative Control Preview Images

Author → re-render → verify loop. FAKE-backed commands share `.fake` state — run them
**sequentially** in the order shown.

## 1. Author / edit the sample source (FR-002)

Edit the single `ControlSampleDefinition` list (R1). For a control, set `Kind =
Demonstrative` and construct fixed representative sample state through the typed front door,
coherent with the detail page's documented usage (FR-006). Example shape:

```fsharp
// button — labelled command (demonstrative)
sample "button" Demonstrative (Button.create [ Button.text "Save" ]) defaultCanvas
       "A primary command button with a visible label."
// overlay — single representative static frame; motion stated, not faked
sample "overlay" Demonstrative (/* small composed frame */) defaultCanvas
       "Layered content shown as one representative frame."
// a genuinely non-renderable control:
unsupported "<id>" "Cannot be honestly rendered render-only; declared unsupported."
```

Keep literals fixed — no clock, randomness, or environment data (FR-008). Use a per-control
`Canvas` override only when the demonstration needs more room (still fixed/documented, R4).

## 2. Re-render the previews (render-capable host only)

Run the committed render harness (R2) to regenerate `docs/img/controls/<id>.png`:

```bash
# Render-capable host with Skia native libs on the loader path:
LD_LIBRARY_PATH=<skiasharp linux-x64 native dir> <render-harness invocation>   # see research R2
```

This writes a PNG for every `Demonstrative` entry and **no** image for `Unsupported`
entries. Re-running it MUST yield byte-identical PNGs (FR-008 / P4).

## 3. Pin / confirm the trivial-content floor (R3)

After regenerating, record the smallest demonstrative PNG byte size and the empty-canvas
byte size; confirm the pinned floor `T` sits between them with headroom. The
`TrivialPreview` guard fails any preview under `T`.

## 4. Regenerate the per-control evidence record (FR-010)

Update `specs/079-doc-preview-examples/readiness/controls-preview-evidence.md`: one row per
control (id, name, render-only mode, decodable, dimensions, bytes, classification) plus the
reconciled `rendered = N / unsupported = M` summary (SC-005).

## 5. Reposition the nav (FR-011, one-time)

Apply the R6 `categoryindex` renumber: `docs/controls/*` 2→8; `docs/roadmap.md` 7→9;
`docs/development.md` / `docs/distribution.md` / `docs/migration/v2-to-v3.md` 8→10. Change
only `categoryindex` lines — no file moves, no `index` changes.

## 6. Validate (sequential FAKE order)

`Route` first, then run only the gates it prints (this change escalates — governance +
`docs/**`):

```bash
./fake.sh build -t Route                       # confirm tier + gate list
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
dotnet fsdocs build --strict --eval            # GPU-free: previews present, links resolve, nav order
```

## Acceptance smoke checks

- Open `docs/controls/catalog.md` + several detail pages → previews show recognizable,
  control-specific content (SC-001/SC-002).
- Blank one preview → `ControlsCatalogDocsCheck` reports `TrivialPreview` FAIL (SC-003).
- Re-render twice → 0 byte diffs (SC-004).
- Built sidebar: Examples → **Controls** → Guides; 0 broken links into `docs/controls/`
  (SC-006).
