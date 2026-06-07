# Feature Specification: FsDocs Documentation Site on GitHub Pages

**Feature Branch**: `076-fsdocs-documentation-site`
**Created**: 2026-06-07
**Status**: Draft
**Input**: User description: "using fsdocs skills create comprehensive and insightful documentation on gh pages. create api docs. create technical docs for each part explaining the architecture. close each section with an analysis of the architecture and implementation listing strengths and weaknesses in implementation, pros and cons in design decisions. especially explain the governance system and the newer control design with penpot integration. how should it be used, where in the speckit process. explain the speckit process where in it custom fs-skia-ui components are used."

## Overview

Publish a single, comprehensive documentation website for FS Skia UI to GitHub
Pages, built with FSharp.Formatting (`fsdocs`). The site combines two pillars:

1. **Generated API reference** for the published library packages, sourced from
   in-code XML documentation comments.
2. **Authored technical/architecture documentation** that explains each part of
   the system, and — uniquely for this project — closes every major section with
   a candid **architecture & implementation analysis** (implementation strengths
   and weaknesses; design-decision pros and cons).

Two subsystems receive deep, deliberate coverage because they are the project's
distinguishing and least-self-evident parts: the **governance system** (the
compiled routing/evidence/gate machinery) and the **newer typed control design
with Penpot / design-token integration**. For each, the documentation must
explain not only what it is, but **how a consumer should use it and where in the
Spec Kit (speckit) process it applies** — including precisely where in the
speckit workflow custom FS Skia UI components are introduced and consumed.

## Clarifications

### Session 2026-06-07

- Q: Is full doc-comment coverage of the entire supported public surface in-scope for this feature, or phased? → A: Full coverage now — every supported public member across all published packages is documented this feature, so SC-001 holds at merge.
- Q: Should the docs include executable literate `.fsx` examples (evaluated by fsdocs) or prose-only snippets? → A: Literate `.fsx` examples — author runnable scripts for key workflows, evaluated at build so examples stay compiler-correct.
- Q: How should the site publish to GitHub Pages? → A: GitHub Actions workflow that builds fsdocs and deploys to Pages on push (regenerates from source; no generated output committed).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Library consumer finds and understands the public API (Priority: P1)

A developer building an application on FS Skia UI visits the documentation site
to learn how to use a published package. They browse a generated API reference,
read the summary and parameter descriptions for a type or function, and follow a
link from the API entry to the technical document that explains the subsystem it
belongs to.

**Why this priority**: API discoverability is the primary, recurring reason an
external consumer visits documentation. Without trustworthy generated reference
content the site fails its most common job, so this is the minimum viable slice.

**Independent Test**: Can be fully tested by browsing the published site for a
known public type (e.g. a typed control or a scene primitive), confirming its
summary, parameters, and return value are present and accurate, and confirming
the cross-link to its subsystem technical page resolves.

**Acceptance Scenarios**:

1. **Given** a consumer on the documentation site, **When** they open the API
   reference for a published package, **Then** every public type and member that
   is part of the supported surface shows a human-readable summary (not an empty
   stub), and parameters/returns are described where applicable.
2. **Given** an API reference entry, **When** the consumer follows its link to
   the related technical page, **Then** they land on the architecture document
   for that subsystem.
3. **Given** a consumer searching the site, **When** they enter a public type
   name, **Then** the corresponding API entry appears in results.

---

### User Story 2 - Newcomer understands the architecture, part by part, with honest analysis (Priority: P1)

A new contributor (or an evaluating engineer) reads the technical documentation
to understand how the framework is built. They move through one document per
part of the system — each explaining that part's architecture — and each
document ends with an analysis section that lists implementation strengths and
weaknesses and weighs the pros and cons of the design decisions. They leave able
to describe how the parts fit together and where the sharp edges are.

**Why this priority**: The explicit, differentiating value the user asked for is
*insightful* documentation that does not just describe but evaluates. A reader
who finishes a section without an honest strengths/weaknesses and pros/cons
analysis has not received the core deliverable.

**Independent Test**: Open each technical/architecture document and confirm it
(a) explains the architecture of one identifiable part of the system, and
(b) ends with a clearly delineated analysis covering implementation
strengths, implementation weaknesses, design pros, and design cons.

**Acceptance Scenarios**:

1. **Given** the technical documentation set, **When** a reader opens any major
   subsystem document, **Then** it contains an architecture explanation followed
   by a closing analysis section.
2. **Given** a closing analysis section, **When** the reader reviews it, **Then**
   it explicitly names both strengths *and* weaknesses of the implementation and
   both pros *and* cons of at least one design decision — not a one-sided
   summary.
3. **Given** the full technical set, **When** a reader enumerates the documents,
   **Then** there is coverage for each major part of the system (rendering/host,
   scene, layout, input, the Elmish/MVU runtime, the controls suite, the
   testing/SkillSupport helpers, and the build/governance front-end), with no
   major published part left undocumented.

---

### User Story 3 - Practitioner learns the governance system and where it fits in speckit (Priority: P1)

A maintainer or speckit practitioner needs to understand the governance system —
the compiled routing that decides which gates run, the evidence model, and the
single-source generation of governance artifacts — and, crucially, **how to use
it and at which points in the speckit process it intervenes**. They read a
dedicated governance section that ties each governance concept to the concrete
speckit phase it governs.

**Why this priority**: The governance system is the project's most novel and
least-self-explanatory subsystem and was explicitly called out by the user. It
is also the part most likely to be misused or misunderstood by a consumer who
adopts the speckit process, so first-class explanation is required.

**Independent Test**: Open the governance documentation and confirm it explains
the routing/tier selection, the evidence/audit model, and single-source
generation, and that it maps these to specific speckit phases (specify → clarify
→ plan → tasks → analyze → implement → merge) with stated usage guidance.

**Acceptance Scenarios**:

1. **Given** the governance documentation, **When** a reader looks for usage
   guidance, **Then** it states how a practitioner runs and responds to the
   routing/gate selection and the evidence audit, not merely what they are.
2. **Given** the governance documentation, **When** a reader asks "where in
   speckit does this apply?", **Then** the document maps governance touchpoints
   to named speckit phases.
3. **Given** the governance section, **When** the reader reaches its end,
   **Then** it closes with the same strengths/weaknesses and pros/cons analysis
   applied to the governance design.

---

### User Story 4 - Designer/consumer learns the typed control + Penpot design-token workflow and its speckit placement (Priority: P2)

A consumer who wants on-brand, design-driven controls reads the documentation
for the newer typed control design and its Penpot / design-token integration.
They learn how design tokens flow from the design source into the typed control
surface, how to author against the typed Props/MVU front door, and **where in
the speckit process this workflow is invoked** — including where custom FS Skia
UI components are created and consumed across the speckit phases.

**Why this priority**: This is the newer, forward-looking control design the user
specifically asked to highlight, but it builds on the API and architecture
pillars (P1) and serves a narrower audience than the core API reference, so it is
P2 rather than P1.

**Independent Test**: Open the typed-controls/Penpot documentation and confirm it
explains the token-to-control flow, shows how to use the typed front door, and
identifies the speckit phase(s) where custom components and the design-token
workflow are applied.

**Acceptance Scenarios**:

1. **Given** the typed-control documentation, **When** a consumer reads it,
   **Then** it explains how design tokens originate from the design source and
   reach the typed control surface, and how to author against the typed front
   door.
2. **Given** the documentation, **When** the consumer asks "where in speckit do I
   use this?", **Then** it names the speckit phase(s) at which custom FS Skia UI
   components and the design-token workflow are introduced and consumed.
3. **Given** the typed-control/Penpot section, **When** the reader reaches its
   end, **Then** it closes with the strengths/weaknesses and pros/cons analysis.

---

### User Story 5 - Maintainer publishes and keeps the site current on GitHub Pages (Priority: P2)

A maintainer builds the documentation locally to preview it, then publishes it to
GitHub Pages. The build is reproducible and the published site reflects the
authored and generated content. Republishing after a content change is a single,
documented action.

**Why this priority**: Publishing is what makes the documentation reachable, but
it depends on the content existing first (P1) and is a one-time-per-change
operational concern, so it is P2.

**Independent Test**: Build the site locally and confirm it produces a complete
static site; trigger the publish path and confirm the GitHub Pages site serves
the same content.

**Acceptance Scenarios**:

1. **Given** the documentation sources, **When** a maintainer runs the local
   build, **Then** a complete static site is produced without build errors.
2. **Given** a successful build, **When** the publish path runs, **Then** the
   GitHub Pages site serves the generated API reference and the authored
   technical documentation.
3. **Given** a content change, **When** the maintainer republishes, **Then** the
   live site reflects the change without manual file shuffling.

---

### Edge Cases

- **Undocumented public members**: a public, supported API member has no XML doc
  comment. The documentation effort MUST surface and close such gaps for the
  supported surface rather than silently shipping empty API stubs.
- **Internal/unsupported surface**: internal-only or explicitly unsupported APIs
  must not be presented to consumers as supported public reference.
- **Analysis honesty**: a closing analysis that lists only strengths (or only
  weaknesses) fails the deliverable; both sides are required.
- **Stale generated content**: if API reference is generated but the build is not
  re-run, the site could drift from the code; the publish path must regenerate
  from source rather than serve hand-edited generated output.
- **Unsupported-environment rendering**: any embedded visual/screenshot evidence
  used in docs must follow the project's evidence-honesty rules (render-only,
  no fabricated visuals) and degrade benignly where rendering is unsupported.
- **Doc comments must not alter the public contract**: adding XML documentation
  comments must not change `.fsi` signatures or surface baselines.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The documentation site MUST be built with the project's
  FSharp.Formatting (`fsdocs`) toolchain using the project's fsdocs skills, and
  MUST produce a single static site combining generated API reference and
  authored technical content.
- **FR-002**: The site MUST include a generated API reference covering the
  published library packages' supported public surface, sourced from in-code XML
  documentation comments.
- **FR-003**: Public, supported API members that lack documentation comments MUST
  be documented (doc comments added at the source) so the generated reference
  presents human-readable summaries rather than empty stubs. Coverage is **full
  for this feature**: every supported public member across all published packages
  MUST be documented within this feature (no phased deferral), so SC-001 holds at
  merge.
- **FR-004**: Adding documentation comments MUST NOT change any `.fsi`
  signature, public contract, or surface baseline (documentation-only change to
  source).
- **FR-005**: The site MUST include authored technical/architecture
  documentation with at least one document per major part of the system, each
  explaining that part's architecture.
- **FR-006**: Every major technical/architecture document MUST close with an
  analysis section that explicitly lists implementation **strengths** and
  **weaknesses** and the **pros and cons** of its key design decisions.
- **FR-007**: The documentation MUST include a dedicated, in-depth treatment of
  the **governance system**, explaining the routing/tier-and-gate selection, the
  evidence/audit model, and single-source generation of governance artifacts.
- **FR-008**: The governance documentation MUST provide **usage guidance** (how a
  practitioner runs and responds to it) and MUST map governance touchpoints to
  the specific **speckit phases** they govern.
- **FR-009**: The documentation MUST include a dedicated treatment of the **newer
  typed control design with Penpot / design-token integration**, explaining the
  design-token flow from design source to typed control surface and how to author
  against the typed Props/MVU front door.
- **FR-010**: The typed-control/Penpot documentation MUST state **how it should be
  used and where in the speckit process** it applies, and MUST explain the
  **speckit process itself with the specific phase(s) where custom FS Skia UI
  components are created and consumed**.
- **FR-011**: The documentation MUST cross-link the generated API reference and
  the authored technical content so a reader can move between a type and its
  subsystem explanation.
- **FR-012**: The site MUST be publishable to **GitHub Pages** via a **GitHub
  Actions workflow** that builds the fsdocs site and deploys it to Pages on push;
  the workflow regenerates content from source and MUST NOT serve generated output
  committed to the repository.
- **FR-013**: The site MUST be buildable locally for preview before publishing.
- **FR-014**: Internal-only or explicitly unsupported APIs MUST NOT be presented
  to consumers as supported public reference.
- **FR-015**: Any visual/screenshot evidence embedded in the documentation MUST
  comply with the project's evidence-honesty rules (render-only, no fabricated
  visuals, benign degradation in unsupported environments).
- **FR-016**: The documentation MUST include a navigable structure (landing page
  plus organized sections) so consumers, contributors, and speckit practitioners
  can each find their entry point.
- **FR-017**: The documentation MUST include **executable literate `.fsx`
  examples** (authored with the `fsdocs-examples` skill) for the key consumer
  workflows — at minimum the typed control / MVU front door and the design-token
  flow — that are **evaluated by fsdocs at build time**, so example code is
  compiler-verified against the real API. A literate example that fails to compile
  or evaluate MUST fail the documentation build (no non-evaluated stand-ins for
  these key workflows).

> Interacting / conflicting requirements: FR-003 (fill documentation gaps at the
> source) vs. FR-004 (do not change the public contract) — resolution: doc
> comments may be added freely because they do not alter signatures; if a true
> documentation gap can only be closed by a signature change, that change is out
> of scope for this feature and is recorded as a follow-up, not made here.
> FR-006 (honest strengths/weaknesses analysis on a public site) vs. the desire
> for a polished public face — resolution: the analysis is retained and
> published as specified; candor is the requested value and takes precedence over
> marketing tone.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, package contents, or package version
  changes are intended. Published package **assemblies** are unchanged; only
  source-level XML doc comments are added (no signature change). No legacy Charts
  package migration is involved. If the chosen fsdocs configuration adds
  documentation-generation MSBuild properties to packable projects, that is a
  build-config change, not a package-content/version change.
- **Public contract impact**: No `.fsi` signatures, documented public API
  shapes, sample contracts, or surface baselines change. Documentation comments
  are additive to `.fs` source and do not alter the surface; this MUST be held as
  an invariant (see FR-004) and verified against surface baselines.
- **State workflow impact**: None. No stateful workflow, I/O, commands, effects,
  subscriptions, or interpreter behavior changes; this is documentation and
  documentation-build configuration only.
- **Layout/rendering impact**: No runtime layout, charts, DataGrid, rendering,
  Vulkan, or Skia behavior changes. The only rendering touchpoint is any
  screenshot/visual evidence embedded in docs, which is governed by the
  evidence-honesty rules (render-only, benign degradation).
- **Evidence obligations**: Required real evidence paths — a successful local
  `fsdocs` build producing the complete static site; the published GitHub Pages
  output (or the publish workflow run) demonstrating the live site; confirmation
  that surface baselines are unchanged after doc-comment additions; and `Route`
  output for the change set. Evidence is stored under the feature's `readiness/`
  directory per project convention.
- **Unsupported scope**: Out of scope — changing any public API signature to
  improve documentability (recorded as follow-up instead); authoring localized or
  translated documentation; versioned/multi-release documentation hosting;
  changing package versions or publishing packages; and any runtime/product
  behavior change. Visual redesign of controls and net-new Penpot tooling are out
  of scope (the feature documents the existing design, it does not extend it).
- **Build-target impact**: A documentation build path (fsdocs) and a GitHub Pages
  publish path are added. Because the change touches build configuration and CI
  (and potentially packable-project properties and governance-adjacent doc
  surfaces), it is expected to **escalate** under `Route`; the authoritative tier
  and minimal gate list MUST be taken from `./fake.sh build -t Route` for the
  actual diff. Existing product gates (`Dev`, `Verify`, `TemplateCheck`,
  `GeneratedGuidanceCheck`, `EvidenceGraph`, `EvidenceAudit`) are not expected to
  change in behavior; whether a new docs-build gate is warranted is a planning
  decision, not assumed here.

## Success Criteria *(mandatory)*

- **SC-001**: A reader can find any supported public type or member in the site's
  API reference and read a non-empty, human-readable description of it; zero
  supported public members are presented as empty stubs.
- **SC-002**: Every major part of the system has a technical/architecture
  document, and 100% of those documents end with an analysis section that names
  both implementation strengths and weaknesses and both design pros and cons.
- **SC-003**: A practitioner unfamiliar with the project can, using only the
  governance documentation, correctly state which speckit phase each governance
  touchpoint applies to and how to respond to it.
- **SC-004**: A consumer can, using only the typed-control/Penpot documentation,
  describe how a design token reaches a typed control and identify the speckit
  phase(s) at which custom FS Skia UI components are created and consumed.
- **SC-005**: The site builds locally with no build errors and publishes to
  GitHub Pages, where the published content matches the locally built content.
- **SC-006**: Re-running the publish path after a content change updates the live
  site with no manual file manipulation.
- **SC-007**: Surface baselines are unchanged after the documentation work,
  confirming the public contract was not altered (FR-004 held).
- **SC-008**: A first-time visitor reaches the right entry point for their role
  (consumer, contributor, or speckit practitioner) from the landing page in no
  more than two navigation steps.
- **SC-009**: Every literate `.fsx` example for a key workflow evaluates cleanly
  during the documentation build; a broken example fails the build rather than
  publishing stale or non-compiling code.

## Assumptions

- "fsdocs skills" refers to the project's existing `fsdocs-setup`, `fsdocs-api-doc`,
  `fsdocs-technical`, `fsdocs-examples`, and `fsdocs-build` skills, which are the
  intended authoring/build tooling for this feature.
- "Each part" of the system maps to the published source areas (host/SkiaViewer,
  Scene, Layout, Input/KeyboardInput, Elmish/MVU runtime, Controls and
  Controls.Elmish suite, Testing/SkillSupport) plus the build/governance
  front-end; the planning phase will finalize the exact document list.
- The supported public surface is defined by the project's existing surface
  baselines; "API docs" covers that surface, not internal-only APIs.
- GitHub Pages is the single hosting target; the publish path is a **GitHub
  Actions workflow** that builds fsdocs and deploys to Pages on push (no manual
  upload, no committed generated output).
- The closing "analysis" sections are authored, opinionated assessments grounded
  in the actual implementation and design records (ADRs, design reports), and are
  intended for publication.
- Embedded visuals, if any, are produced under the existing evidence-mode rules;
  the documentation does not require new rendering capability.
- This feature documents the *existing* governance system and *existing* typed
  control/Penpot design; it does not modify either subsystem.

## Dependencies

- The project's FSharp.Formatting (`fsdocs`) toolchain and the five fsdocs skills.
- The existing published library packages and their surface baselines.
- The existing authored material under `docs/` (ADRs, design reports, subsystem
  design notes) as source input for the technical/analysis content.
- A GitHub Pages target and repository permission to publish to it.
- The governance routing (`Route`) output to determine the gate set for the change.

## Key Entities

- **Documentation Site**: the single published static site (landing page +
  navigation) hosted on GitHub Pages.
- **API Reference**: generated per-package reference of the supported public
  surface, sourced from XML doc comments.
- **Technical/Architecture Document**: one authored page per system part,
  explaining architecture and closing with a strengths/weaknesses + pros/cons
  analysis.
- **Governance Documentation**: dedicated coverage of routing, evidence/audit,
  and single-source generation, mapped to speckit phases with usage guidance.
- **Typed-Control / Penpot Documentation**: dedicated coverage of the typed
  Props/MVU front door and the design-token flow from design source to control,
  with speckit-phase placement.
- **Speckit-Placement Guidance**: the explanation of the speckit process and the
  phase(s) where custom FS Skia UI components and the design-token workflow are
  created and consumed.
- **Publish Path**: the repeatable build-and-deploy route from documentation
  sources to the live GitHub Pages site, implemented as a GitHub Actions workflow
  that regenerates content from source (no committed generated output).
- **Literate Example**: an executable `.fsx` script (authored with
  `fsdocs-examples`) for a key consumer workflow, evaluated by fsdocs at build so
  the example stays compiler-correct against the real API.
