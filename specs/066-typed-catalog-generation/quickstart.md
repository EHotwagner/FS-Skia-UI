# Quickstart: Typed Catalog Generation

How a maintainer uses the single-source catalog after this feature lands.

## Change a catalog fact for a typed control (the SC-001 workflow)

1. Edit the fact in **one** place — the fact table in
   `build/Governance/CatalogGen.fs` (`catalogFacts`). For example, change
   `Button`'s `DisplayName` or add an event:

   ```fsharp
   { Id = "button"; DisplayName = "Button"; Category = "input"; Module = "Button"
     Purpose = "Pointer and keyboard activatable command."
     RequiredAttributes = [ "text" ]; Events = [ "onClick" ]
     AccessibilityRole = "Button" }
   ```

2. Regenerate both catalog artifacts in one command:

   ```bash
   ./fake.sh build -t RefreshSurfaceBaselines
   ```

   This rewrites the `typed-catalog/button` region in **both**
   `src/Controls/catalog.yml` and `src/Controls/Catalog.fs`. Zero manual edits to
   either generated file.

3. Validate — run `Route` first, then only the gates it prints:

   ```bash
   ./fake.sh build -t Route          # lists ControlsCatalogGenerationCheck for src/Controls/** changes
   ```

## Confirm the drift gate works (the SC-003 check)

1. Hand-edit a generated region in `src/Controls/Catalog.fs` (e.g. change the
   `button` row's display name) **without** regenerating.
2. Run the gate:

   ```bash
   ./fake.sh build -t ControlsCatalogGenerationCheck
   ```

   It **fails** and names the divergent control:
   `src/Controls/Catalog.fs is stale — its typed-catalog/button region no longer
   matches CatalogGen.catalogFacts. Regenerate via ./fake.sh build -t
   RefreshSurfaceBaselines`.
3. Regenerate (`RefreshSurfaceBaselines`) → the gate passes (clean tree).

## Confirm the migration is non-behavioral (the SC-002 / SC-005 check)

```bash
./fake.sh build -t Dev                       # builds; CatalogTests parity + drift + correspondence tests pass
./fake.sh build -t ControlsCatalogCheck      # unchanged catalog validation passes
```

The parity test asserts each of the six generated rows equals its captured
pre-migration row; the correspondence test asserts the fact table covers exactly
the six `FS.Skia.UI.Controls.Typed` modules.

## Escalated validation order (consumer-contract change)

Because `src/Controls/**` escalates via `controls-public-surface`, run the
Route-printed gates plus the serialized FAKE-backed order **sequentially** (never
concurrently — shared `.fake` state):

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

`TargetMetadataDrift` (run via the escalated path) confirms
`validation.contract.yml` lists the new gate; `Route --enforce` blocks a stale
generated catalog as a missing obligation.
