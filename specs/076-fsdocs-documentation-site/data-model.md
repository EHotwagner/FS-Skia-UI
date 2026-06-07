# Phase 1 Data Model: Documentation Site Information Model

This feature ships no runtime data types; the "model" is the information
architecture of the documentation site — the entities fsdocs assembles and the
relationships a reader navigates. Each entity lists its fields, the requirements
it satisfies, and its validation rules (the conditions a gate or review checks).

## Entity: Documentation Site

The single published static artifact.

| Field | Description |
|---|---|
| `landingPage` | `docs/index.md` — role-based entry (consumer / contributor / speckit practitioner) |
| `navigation` | Section structure exposed by the fsdocs template + `index.md` links |
| `sections` | API Reference, Architecture set, Governance deep-dive, Controls-design deep-dive, Speckit, Examples |
| `baseRoot` | Project-Pages subpath (`/FS-Skia-UI/`) used to resolve absolute links |

- **Satisfies**: FR-001, FR-016, SC-008.
- **Validation**: a first-time visitor reaches the correct role entry point in
  ≤ 2 navigation steps from `landingPage` (SC-008); the built site contains
  `index.html` and all section roots (strict build, no broken links).

## Entity: API Reference (generated)

Per-package reference generated from compiled assemblies + XML doc files.

| Field | Description |
|---|---|
| `packages` | The 10 published packages (`Scene`, `SkiaViewer`, `Elmish`, `Input`, `KeyboardInput`, `Layout`, `Controls`, `Controls.Elmish`, `Testing`, `SkillSupport`) |
| `members` | Supported public types/members per package, from the surface baselines |
| `summary` | `<summary>` text sourced from `///` on the member's **`.fsi`** declaration |
| `params/returns` | `<param>`/`<returns>` where applicable |
| `crossLinks` | Links from a member to its subsystem architecture page |

- **Satisfies**: FR-002, FR-003, FR-011, FR-014, SC-001.
- **Validation**: zero supported public members render as empty stubs (SC-001 /
  `api-coverage.md`); internal/unsupported members are absent (FR-014); each
  member's API page links to its architecture page (FR-011). Source of truth for
  "supported" = the per-package surface baselines.
- **State/derivation**: regenerated from source on every build — never
  hand-edited, never committed (FR-012, "Stale generated content" edge case).

## Entity: Technical / Architecture Document

One authored Markdown page per major part (see research.md R6).

| Field | Description |
|---|---|
| `part` | The system part it covers (host, scene, layout, input, …) |
| `architectureBody` | Explanation of that part's architecture |
| `analysis` | **Required closing section** with four named facets |
| `analysis.implStrengths` | Implementation strengths (≥1) |
| `analysis.implWeaknesses` | Implementation weaknesses (≥1) |
| `analysis.designPros` | Design-decision pros (≥1) |
| `analysis.designCons` | Design-decision cons (≥1) |
| `crossLinks` | Links to the API entries for the part's public types |

- **Satisfies**: FR-005, FR-006, FR-011, SC-002.
- **Validation** (gate, research.md R4): every page has a delineated analysis
  section naming **both** strengths and weaknesses **and both** pros and cons; a
  one-sided analysis fails (spec "Analysis honesty" edge case).

## Entity: Governance Documentation (deep dive)

Dedicated treatment of the compiled governance machinery.

| Field | Description |
|---|---|
| `routingAndGates` | Tier selection + minimal gate list (the `Route` selector) |
| `evidenceAndAudit` | Evidence model, `[S]`/`[S*]` propagation, merge-gate audit |
| `singleSourceGeneration` | `validation.contract.yml` from `Routing.fs`; `.claude` from `.agents` |
| `usageGuidance` | How a practitioner **runs and responds to** routing + audit |
| `speckitMapping` | Each touchpoint → named speckit phase (specify→…→merge) |
| `analysis` | Closing strengths/weaknesses + pros/cons |

- **Satisfies**: FR-007, FR-008, SC-003; closes per FR-006.
- **Validation**: a practitioner can, from this section alone, state which speckit
  phase each governance touchpoint applies to and how to respond (SC-003).

## Entity: Typed-Control / Penpot Documentation (deep dive)

Dedicated treatment of the newer typed control design + design-token flow.

| Field | Description |
|---|---|
| `tokenFlow` | Design source (Penpot / DTCG) → typed control surface |
| `typedFrontDoor` | Authoring against the typed Props/MVU front door |
| `speckitPlacement` | Phase(s) where custom components + token workflow are used |
| `analysis` | Closing strengths/weaknesses + pros/cons |

- **Satisfies**: FR-009, FR-010, SC-004; closes per FR-006.
- **Validation**: a consumer can describe how a token reaches a typed control and
  identify the speckit phase(s) for custom components (SC-004).

## Entity: Speckit-Placement Guidance

The process explainer tying custom FS Skia UI components to speckit phases.

| Field | Description |
|---|---|
| `phases` | specify → clarify → plan → tasks → analyze → implement → merge |
| `componentTouchpoints` | Where custom components are **created** and **consumed** |

- **Satisfies**: FR-010 (the "explain the speckit process itself" clause).

## Entity: Literate Example

An executable `.fsx` evaluated by fsdocs at build.

| Field | Description |
|---|---|
| `script` | `docs/examples/*.fsx` (literate: narrative + runnable code) |
| `workflow` | The key consumer workflow it teaches |
| `evaluated` | `true` — compiled/evaluated at build against the real API |

- **Satisfies**: FR-017, SC-009. **Minimum set**: typed control / MVU front door;
  design-token flow.
- **Validation**: every required example evaluates cleanly in the strict build; a
  broken example **fails the build** (no non-evaluated stand-in for these).

## Entity: Publish Path

The repeatable build-and-deploy route to GitHub Pages.

| Field | Description |
|---|---|
| `workflow` | `.github/workflows/docs.yml` (build + `deploy-pages`) |
| `trigger` | push to `main` (+ `workflow_dispatch`) |
| `regeneratesFromSource` | `true`; **no committed generated output** |

- **Satisfies**: FR-012, FR-013, SC-005, SC-006.
- **Validation**: a local `fsdocs build` produces a complete site (FR-013); a push
  republishes with no manual file shuffling (SC-006); published == locally built
  (SC-005).

## Cross-cutting invariant: Embedded visual evidence

Any screenshot/visual embedded in a page (under `docs/img/`) is render-only
evidence: no fabricated visuals, benign degradation where rendering is
unsupported.

- **Satisfies / enforces**: FR-015 (evidence-mode rules; see
  `readiness/runtime-limitations.md`).

## Cross-cutting invariant: Contract preservation

Adding `///` doc comments to `.fsi` files changes doc text only; the
surface-baseline normalizer strips comments, so baselines are unchanged.

- **Satisfies / enforces**: FR-004, SC-007 (verified by
  `PackageSurfaceCheck` / `PerPackageSurfaceDiff`).
