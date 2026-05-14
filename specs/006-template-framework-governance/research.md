# Research: Template Framework Governance

## Decision: Use FAKE As The Canonical V1 Command Surface

**Rationale**: The repository already has a .NET solution, F# scripts, samples, test projects, and Spec Kit evidence hooks, but no single command graph. FAKE gives the project an F#-native target graph that can sequence existing commands and be called consistently by humans, agents, and automation.

**Alternatives considered**:

- Raw shell scripts: easy to start, but command order would still be duplicated across platforms.
- CI YAML only: would not help local developers or agents and would duplicate local command order.
- Keep direct `dotnet` command docs only: preserves the current gap where contributors infer sequencing.

## Decision: Add A Repo-Local Tool Manifest And Thin Wrappers

**Rationale**: `.config/dotnet-tools.json`, `fake.sh`, and `fake.cmd` make the command surface reproducible and cross-platform without requiring a globally installed build tool. Wrappers keep the user command stable as `./fake.sh build -t <Target>` or `fake.cmd build -t <Target>`.

**Alternatives considered**:

- Global tool install: weaker reproducibility and harder onboarding.
- Only `dotnet fake`: exposes implementation details and produces less memorable commands.
- A custom executable: unnecessary for a build graph.

## Decision: V1 Target Set

**Rationale**: V1 must match the clarified scope: canonical verification workflow, existing evidence wiring, stable baselines, local package production, docs, automation alignment, and generated task guidance. The target set is:

- `Clean`
- `Restore`
- `Build`
- `Test`
- `Dev`
- `PackageSurfaceCheck`
- `FsiTranscripts`
- `SampleContractSmoke`
- `EvidenceGraph`
- `EvidenceAudit`
- `Verify`
- `Ci`
- `PackLocal`

**Alternatives considered**:

- Add `PackageSmoke`, `TemplateCheck`, `LayoutEvidence`, `Visual`, and `DependencyReport` now: rejected because these are explicitly deferred roadmap categories for v1.
- Only add `Dev` and `Verify`: rejected because package surface, FSI, sample smoke, graph, audit, and local pack are required v1 artifact classes or workflow entries.

## Decision: Root-Level Stable Package Surface Baselines

**Rationale**: Current package surface tests read from a historical feature folder. V1 should move the current package baseline contract to `readiness/surface-baselines/*.txt`, while feature folders can keep feature-specific evidence copies. This prevents future features from editing old readiness folders.

**Alternatives considered**:

- Continue using `specs/002-skia-feature-parity/readiness/surface-baselines`: rejected because it makes historical evidence the current source of truth.
- Store baselines under the test project: rejected because baselines are governance evidence, not test implementation details.
- Store baselines under each feature directory only: rejected because current package surface needs one stable location.

## Decision: Keep Package Consumer Smoke Out Of V1 Verification

**Rationale**: The clarified v1 scope includes local package production and package surface review only. Any package consumer restore/smoke behavior should remain outside `Verify` or be isolated under a future explicit `PackageSmoke` target.

**Alternatives considered**:

- Include existing clean consumer restore tests in `Verify`: rejected because package consumer smoke was explicitly deferred.
- Delete package consumer smoke: rejected because it may remain valuable roadmap evidence.
- Add a new package consumer target now: rejected because it expands v1 beyond the clarified boundary.

## Decision: Use Existing Evidence Extension For Task Graph And Audit

**Rationale**: The evidence extension already provides graph validation and synthetic-evidence audit scripts. V1 should wrap those scripts from canonical targets instead of creating a parallel implementation.

**Alternatives considered**:

- Reimplement graph/audit behavior in FAKE: rejected because it duplicates working extension logic.
- Leave graph/audit as manual commands only: rejected because v1 requires full verification to produce graph and audit verdicts.

## Decision: Update Generated Task Guidance Only

**Rationale**: Clarification limited v1 generated guidance work to task guidance. The existing preset task template should point contributors to canonical targets for verification. Full spec and plan template hardening stays in the roadmap.

**Alternatives considered**:

- Update spec, plan, and task templates now: rejected because it expands v1 into the Spec Kit hardening phase.
- Defer all generated guidance: rejected because future generated tasks would immediately duplicate command order.

## Decision: Document Roadmap Boundaries In Build, Testing, And Evidence Docs

**Rationale**: V1 needs contributors to understand what is delivered now and what remains future work. The docs must state that template packaging, dependency governance, generated spec/plan hardening, new layout/visual gates, package consumer smoke, and release validation are not part of v1 pass/fail criteria.

**Alternatives considered**:

- Keep roadmap only in `docs/template-framework-analysis.md`: rejected because users need operational docs for the implemented command surface.
- Create full template profile docs now: rejected because the feature is limited to Phase 1.
