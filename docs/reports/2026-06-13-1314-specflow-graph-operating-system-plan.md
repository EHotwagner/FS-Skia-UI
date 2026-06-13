---
title: SpecFlow Graph Operating System Implementation Plan
---

# SpecFlow Graph Operating System Implementation Plan

- **Timestamp:** 2026-06-13T13:14:00+02:00
- **Author:** Codex
- **Status:** Proposed complete redesign
- **Scope:** Replace the Spec Kit-shaped artifact workflow with a repo-native,
  typed F# feature graph. No backward compatibility with existing agent
  workflows, `.specify` scripts, phase command names, or hand-authored
  `tasks.md` / `tasks.deps.yml` semantics is required.
- **Rebrand scope added:** If the project is being renamed, use this repository
  as the evidence-backed bootstrapper for a clean new repository, then continue
  active development in the new repository.
- **Constraint for this report:** No governance machinery was run for this
  report. Research used source inspection, upstream Spec Kit release/catalog
  data, official Spec Kit extension documentation, the community extension
  catalog, official GitHub repository rename/transfer documentation, official
  NuGet packaging/deprecation documentation, official .NET template
  documentation, and official FSharp.Formatting content guidance.

## Executive Decision

Design and bootstrap **FS.GG.UI** from this repository. The target is not a
cleaned-up `FS.Skia.UI` tree; it is a new graph-governed repository that carries
the runtime product, product contract, governance kernel, evidence ledger, and
release policy from day one.

The split-kernel decision changes the packaging shape: extract a small
standalone **rule kernel** now, because there are at least two non-FS.GG.UI
projects that may use it if adoption is cheap. Do not extract the whole
FS.GG.UI governance system as a generic platform. The reusable package should
own deterministic fact/rule evaluation, provenance, diagnostics, graph hashing,
and explanation primitives. FS.GG.UI-specific routing, package, template,
product, release, and evidence policy should remain in FS.GG.UI governance
adapters.

If the rebrand is accepted, do not treat this repository as the final host for
that operating system. Treat it as the design, extraction, and provenance
environment for the new repository. The new repository should start with the
graph-first workflow, new package identities, new template identity, new docs URL
assumptions, and no active `.specify` runtime. The old repository should not be
made pristine before the new repository exists; it should become just clean
enough to assemble and prove the new tree.

The feature graph becomes the single authoritative state model for:

- feature identity and lifecycle;
- requirements, success criteria, scenarios, and unsupported scope;
- design decisions, research claims, and source citations;
- implementation tasks, dependencies, ownership, status, and completion proofs;
- evidence artifacts, command runs, generated outputs, approvals, and synthetic
  disclosures;
- route expectations, actual diff classification, required gates, and gate
  results;
- agent context packs and phase-specific file-reading budgets.

The redesign should now be treated as a **full governance integration**, not only
as a Spec Kit replacement. The same graph operating system should also own the
active project governance model: target catalog, routing policy, generated
views, package identity, template identity, docs publishing assumptions, CI
rules, release/publish policy, platform ruleset expectations, and provenance
requirements. The compiled F# rulebook remains the authority, but the graph
becomes the state and intent layer that binds every policy surface together.

There is a second integration layer worth adding: a **product contract graph**.
`ProjectGraph` should say how the repository operates; `FeatureGraph` should say
what a feature is doing; `ProductGraph` should say what the product promises.
It binds controls, capabilities, public API surfaces, docs pages, gallery
samples, screenshots, visual evidence, scenario corpus rows, performance
budgets, accessibility and interaction contracts, ADRs, and toolchain
environment assumptions into one product-facing contract.

Markdown remains useful, but not authoritative. `spec.md`, `plan.md`, `tasks.md`,
readiness reports, Mermaid diagrams, and context prompts become deterministic
projections from the graph. If a projection is edited by hand, the next graph
check fails and names the stale projection. The graph owns the state.

The redesign is intentionally breaking:

1. Delete the active `.specify` runtime shape instead of wrapping it.
2. Retire upstream Spec Kit version and integration metadata as active inputs.
3. Replace `speckit-*` phase skills with graph-aware `specflow-*` skills or
   a single generated context-pack skill.
4. Replace hand-authored `tasks.deps.yml` with graph-owned dependencies.
5. Replace task checkboxes as source of truth with evidence-backed graph
   task completion.
6. Replace readiness file discovery as the audit source with a structured
   evidence index.
7. Replace standalone governance islands with graph-bound policy modules and
   generated platform/configuration views.
8. Replace scattered product contract metadata with a graph-owned product
   surface model.
9. Replace repo-local expert-system machinery with a small standalone rule
   kernel plus project-specific governance adapters.

The destination is not a marketplace, workflow engine, or generic Spec Kit clone.
It is a repo-specific specification, governance, product-contract, release, and
evidence operating system for FS.GG.UI, built on a reusable typed rule substrate
that other projects can consume without adopting FS.GG.UI policy.

## Research Summary

### Upstream Spec Kit

The current upstream Spec Kit release checked for this report is `v0.10.2`,
published 2026-06-11. The release notes show the install path is still a Python
tool via `uv tool install specify-cli --from git+https://github.com/github/spec-kit.git@v0.10.2`.
Relevant release details:

- `v0.10.0` moved toward explicit integrations and removed legacy AI flags.
- `v0.10.0` made the Git extension opt-in.
- `v0.10.0` added per-event hook lists with priorities.
- `v0.10.1` added integration status and catalog payload validation.
- `v0.10.2` added `category` and `effect` as first-class extension fields and
  expanded the community catalog.
- The upstream `speckit` workflow remains a generic sequence:
  `specify -> review-spec -> plan -> review-plan -> tasks -> implement`.

Conclusion: upstream is optimizing a generic multi-project, multi-agent
installer/runtime. This repository should not carry that generality. It already
has a compiled F# governance engine, typed FAKE targets, route selection, and
evidence audits.

### Extension Catalog

The upstream extension README makes the important trust model explicit:
community extensions are independently maintained; maintainers verify catalog
entry shape, not extension code. Organizations are expected to curate their own
catalog if they want trusted extension availability.

The raw community catalog currently contains 113 entries. The observed metadata
shape is useful:

| Field | Observation |
|---|---|
| `category` | Mostly `process`, `docs`, `code`, `integration`, `visibility`; two entries currently lack category. |
| `effect` | Mostly `read-write`; some `read-only`; two entries currently lack effect. |
| `provides.commands` / `provides.hooks` | Useful for status and health reporting. |

Representative patterns worth absorbing locally:

| Catalog idea | Local interpretation |
|---|---|
| Project Status / Status Report / Doctor | Graph-derived status and health commands. |
| Spec Trace / Verify / Verify Tasks | Requirement-to-task-to-evidence traceability and phantom-completion detection. |
| Plan Review Gate / Spec Validate / Staff Review | Structured approval artifacts, not vague prose. |
| Architecture Guard / DocGuard / Coding Standards Drift | Deterministic drift checks over repo-owned sources. |
| Token Budget / Token Analyzer | Phase context packs and file-reading budgets. |
| Worktrees / Worktree Isolation | Worktree-first feature execution with isolated cache state. |
| Research Harness / Version Guard | Typed research claims with URLs, retrieval dates, and checked versions. |

Conclusion: the catalog validates the capability taxonomy, not the plugin
runtime. We should implement capabilities as local graph modules, not install
third-party packages.

### FSharp.Formatting

Official FSharp.Formatting content guidance confirms that `docs/**/*.md` is
processed as content and frontmatter is optional. It also confirms that
frontmatter controls navigation metadata such as `title`, `category`,
`categoryindex`, `index`, `description`, and `keywords`. This report keeps the
existing local report convention: minimal frontmatter with `title`, because
`docs/reports/**` already follows that pattern.

### Local Repository Inventory

The local codebase already contains most of the mechanics needed for a graph
operating system:

| Existing module | Current responsibility | Reuse in redesign |
|---|---|---|
| `build/Governance/Engine/Model.fs` | Build model, active feature resolution, effect DU. | Extend with graph effects and paths, then delete `.specify` active-feature resolution. |
| `build/Governance/Engine/Update.fs` | Pure target-to-effect decision function. | Keep the pure/effect boundary for graph projection, graph validation, and graph mutation commands. |
| `build/Governance/Evidence/TaskParser.fs` | Parses `tasks.md` checkboxes, task IDs, skillist mirrors, synthetic metadata. | Mine for migration/import only; replace checkbox authority with graph task status. |
| `build/Governance/Evidence/DepsParser.fs` | Parses `tasks.deps.yml`. | Replace with graph dependency fields; keep parser only for one-time import or historical tooling. |
| `build/Governance/Evidence/Graph.fs` | Builds task DAG, detects cycles, topological sort, propagates synthetic taint. | Promote the algorithm into graph validation over `FeatureGraph.Tasks`. |
| `build/Governance/Evidence/Render.fs` | Renders task graph JSON, Markdown, Mermaid. | Reuse rendering style, but render from graph-owned state. |
| `build/Governance/Evidence/Audit.fs` | Merges task/deps, validates skills, owns vocabulary, synthetic disclosures, readiness scans. | Split into reusable invariant modules against typed graph data. |
| `build/Governance/Evidence/Scans.fs` | Scans readiness files for required evidence formats. | Replace most scans with structured evidence rows; keep selected scanners as projection sanity checks. |
| `build/Governance/SymbolCrossCheck.fs` | Extracts FR/SC and structural symbols across plan/data-model/tasks. | Generalize into graph traceability checks. |
| `build/Governance/Routing.fs` | Selects tier and gates from working-tree diff. | Add graph-declared impact planning and compare declared route to actual diff route. |
| `build/Governance/TargetMetadata.fs` | Validates runnable target metadata and renders JSON/Markdown. | Attach target metadata to graph route expectations and gate result evidence. |
| `build/Governance/Guidance.fs` | Checks generated guidance obligations. | Replace template prose checks with generated context-pack checks. |
| `SkillTreeGen` / `SkillSync` | `.agents` canonical, `.claude` generated. | Keep generation mechanism if needed, but replace `speckit-*` skills with `specflow-*` skills. |

The local artifact set also exposes the current design problem. In feature 116,
`tasks.md` currently shows tasks marked `[X]`, while
`readiness/task-graph.json` can still contain `declared: "pending"` if the
projection has not been regenerated. That is exactly the failure mode the
redesign should remove: a generated readiness artifact cannot be allowed to
compete with the current source artifact for authority.

### Current Spec Kit Coupling To Delete

The active coupling to delete or archive:

- `.specify/scripts/bash/common.sh`
- `.specify/scripts/bash/create-new-feature.sh`
- `.specify/scripts/bash/setup-plan.sh`
- `.specify/scripts/bash/setup-tasks.sh`
- `.specify/scripts/bash/check-prerequisites.sh`
- `.specify/extensions/**`
- `.specify/presets/**`
- `.specify/templates/**`
- `.specify/workflows/**`
- `.specify/init-options.json`
- `.specify/integration.json`
- `.specify/integrations/**`
- `.specify/feature.json`
- `.agents/skills/speckit-*` as active guidance
- generated `.claude/skills/speckit-*` peers

The replacement can keep `.specify/memory/constitution.md` only if it is renamed
or imported into a repo-owned governance source.

### Rebrand And New Repository Research

The rebrand question changes the recommendation. A complete graph-first
workflow redesign can be built in this repository, but a simultaneous product
rebrand makes an in-place rename a poor destination. The cleanest path is:

1. Use this repository for one final bootstrap feature.
2. Specify the new brand, package namespace, repository layout, docs URL, and
   cutover policy in that feature.
3. Generate or assemble the new repository from this repository's best product
   slices.
4. Continue active development in the new repository.
5. Leave this repository as a bridge, archive, and migration reference.

This is not only an aesthetic preference. External platform behavior and local
identity inventory both point to the same conclusion.

#### GitHub Rename And Transfer Behavior

Official GitHub repository rename and transfer documentation supports renames
for many links, but it does not make a rename equivalent to a clean new product
boundary:

| Platform behavior | Consequence for this project |
|---|---|
| Repository renames redirect issues, wikis, stars, followers, and ordinary git operations. | Rename-in-place is viable if the main goal is preserving social/repository continuity. |
| GitHub Pages project-site URLs are explicitly excluded from automatic rename/transfer redirects. | The current docs URL shape (`https://ehotwagner.github.io/FS-Skia-UI/`) is a real cutover surface, not a free redirect. |
| GitHub does not redirect calls to an action hosted by a renamed repository. | If any workflows or downstream generated products consume repository-hosted actions, rename-in-place can break them. |
| Reusing the old repository name later deletes redirects. | A bridge repository at the old name must be intentional; accidental reuse can destroy expected redirects. |
| Transfers preserve commits, issues, pull requests, stars, watchers, and many settings, but packages may transfer or lose repository linkage depending on registry. | A transfer helps ownership cleanup, not brand/package cleanup. |

Inference: GitHub rename/transfer mechanics reduce the pain of moving code, but
they do not solve product identity. They are compatibility bridges, not the
target architecture for a rebranded framework.

#### NuGet Package Identity

NuGet package identity is the package ID. The official `.nuspec` reference
describes `id` as the case-insensitive unique package identifier for a gallery.
The MSBuild packaging docs likewise treat `PackageId` as the package identity
used by feeds and tooling. NuGet deprecation supports an alternate recommended
package, which is exactly the migration mechanism for renamed packages.

That means a rebrand creates new packages, not renamed packages:

| Current identity | Rebrand implication |
|---|---|
| `FS.Skia.UI.Scene` | New package ID under the new brand. |
| `FS.Skia.UI.Color` | New package ID under the new brand. |
| `FS.Skia.UI.SkiaViewer` | New package ID under the new brand. |
| `FS.Skia.UI.Elmish` | New package ID under the new brand. |
| `FS.Skia.UI.KeyboardInput` | New package ID under the new brand. |
| `FS.Skia.UI.Layout` | New package ID under the new brand. |
| `FS.Skia.UI.Controls` | New package ID under the new brand. |
| `FS.Skia.UI.Controls.Elmish` | New package ID under the new brand. |
| `FS.Skia.UI.Input` | New package ID under the new brand. |
| `FS.Skia.UI.Testing` | New package ID under the new brand. |
| `FS.Skia.UI.SkillSupport` | Either dropped, renamed, or replaced by graph-owned generated skills. |
| `FS.Skia.UI.Build` | Either renamed as the new build/SpecFlow package or kept private to the new repository. |
| `FS.Skia.UI.Template` | New template package ID; old template package deprecated after the new one works. |

Deprecating old package versions should come after the new packages exist. The
old package pages can then tell existing users why the package is legacy and
which alternate package to install. Unlisting alone would hide the old packages
from search but would not communicate the migration as well.

#### `dotnet new` Template Identity

The .NET template engine has its own identity layer. Official template
documentation names `identity`, `name`, `shortName`, and `sourceName` as template
configuration fields, and the template package docs state that `PackageId` is
used by feeds and uninstall tooling.

The local template currently contains these identity anchors:

| Surface | Current value |
|---|---|
| Template package ID | `FS.Skia.UI.Template` |
| Template identity | `FS.Skia.UI.Template` |
| Template display name | `FS Skia UI Governed Project` |
| Template short name | `fs-skia-ui` |
| Template author | `FS-Skia-UI Contributors` |
| Generated package prefix default | `FS.Skia.UI` |
| Generated repository URL default | `https://github.com/FS-Skia-UI/FS-Skia-UI` |
| Template package README install command | `dotnet new install FS.Skia.UI.Template` |
| Template package README scaffold command | `dotnet new fs-skia-ui ...` |

Therefore the rebrand is not a documentation-only change. The template package,
the template metadata, generated docs, generated package pins, generated
skill names, and all generated project instructions need one coherent identity
matrix.

#### Proposed Brand Candidate

Use the following as the current working brand candidate:

```text
FS.GG.UI
Graph-governed UI infrastructure for F# and Skia.
```

Interpretation:

- `FS` keeps the F# ecosystem signal.
- `GG` means **Graph-governed**, matching the new `ProjectGraph`,
  `ProductGraph`, `FeatureGraph`, and evidence-ledger architecture.
- `UI` keeps the product category explicit.

Initial package/template shape:

| Surface | Proposed value |
|---|---|
| Product name | `FS.GG.UI` |
| Tagline | `Graph-governed UI infrastructure for F# and Skia.` |
| Root namespace | `FS.GG.UI` |
| Package prefix | `FS.GG.UI` |
| Example runtime package | `FS.GG.UI.Scene` |
| Example controls package | `FS.GG.UI.Controls` |
| Example Elmish controls package | `FS.GG.UI.Controls.Elmish` |
| Template package ID | `FS.GG.UI.Template` |
| Template short name | `fs-gg-ui` |
| Build/governance package | `FS.GG.UI.Build` if public; otherwise private to the repo. |

The bootstrap feature should still validate this candidate against NuGet
availability, template short-name collision risk, repository availability, docs
URL shape, and whether `GG` reads clearly enough to new users. If the name
survives that validation, it becomes the brand matrix seed for the new
repository.

#### Local Identity Blast Radius

The local repository inventory shows a broad and intentionally tangled identity
surface:

| Local area | Identity currently embedded |
|---|---|
| Root README | `FS.Skia.UI`, NuGet badges, docs links, `Spec Kit` positioning, `speckit-*` workflow text. |
| Docs site | `https://ehotwagner.github.io/FS-Skia-UI/` links throughout the root README and docs. |
| Source namespaces | `FS.Skia.UI.*` namespaces across public `.fsi` and implementation files. |
| Package projects | `PackageId`, `AssemblyName`, and `Title` under `src/**` and `build/Governance/**`. |
| Template package | `.template.package/FS.Skia.UI.Template.fsproj`, package README, and content includes. |
| Template config | `.template.config/template.json` identity, short name, generated replacement symbols, sources, and generated skill targets. |
| Template pins | `template/base/Directory.Packages.props` pins every `FS.Skia.UI.*` package through one `FsSkiaUiVersion`. |
| Capability catalog | `template/capabilities.yml` maps capabilities to `FS.Skia.UI.*` package IDs, surface baselines, and skill names. |
| Generated product docs | `template/base/docs/**` explains `FS.Skia.UI.*`, `FS.Skia.UI.Build`, `.specify`, and generated skills. |
| Skills | Source and generated skills carry names such as `fs-skia-ui-widgets` and references to `FS.Skia.UI.SkillSupport`. |
| Tests and governance fixtures | Template identity tests, generated-product tests, golden task graphs, and baseline fixtures assert current names. |
| Historical specs | Prior specs and readiness logs contain thousands of old identity references that should become archive/provenance, not active truth. |
| Git remote | Actual origin is `https://github.com/EHotwagner/FS-Skia-UI.git`, while some generated/package metadata points at `https://github.com/FS-Skia-UI/FS-Skia-UI`. |

The important finding is the mismatch between active identity and historical
identity. A new repository lets the active tree be coherent on day one while
keeping the old repository's history available for audit and migration. An
in-place rename would force every old and new identity into one branch history,
one docs site, one issue tracker, and one governance system during the same
period when the graph-first workflow is intentionally deleting old assumptions.

#### Repository Strategy Options

| Option | Description | Strength | Weakness | Recommendation |
|---|---|---|---|---|
| A. New clean repository with provenance | Create a new repository with a clean initial history assembled from selected source, docs, template, tests, and graph governance. Record the old repository commit SHA and migration map. | Maximum freedom, lowest active-tree clutter, best fit for breaking agent workflow and brand reset. | Loses ordinary git blame continuity in the new default history unless files are imported with history. | Recommended. |
| B. New repository with filtered history | Use history-filtering to import selected directories and preserve file ancestry where useful. | Preserves some blame and audit trail. | More complex; risks dragging old `.specify`, readiness, generated artifacts, and naming churn into the new project. | Use only if history continuity is a hard requirement. |
| C. Rename/transfer this repository in place | Rename the GitHub repository and update all package/docs/template names in this tree. | Preserves stars, issues, PRs, forks, and local git continuity. | Does not solve NuGet identities, template identities, docs-site URL changes, or active/historical identity confusion. | Not recommended for this redesign. |
| D. Keep this repository as-is and start an unrelated repo manually | Create the new repo by hand outside the current governance flow. | Fastest to start typing. | Throws away the chance to make the cutover itself evidence-backed and repeatable. | Not recommended. |

Option A is the best fit because the user has explicitly allowed breaking
existing agent workflows and no backward compatibility is required. A clean
repository also prevents the graph-first redesign from spending months
explaining exceptions for `.specify`, `speckit-*`, historical generated files,
and old package names.

#### Bootstrap Feature In This Repository

The current repository should still own the transition, but only as a
bootstrapper. Create one final feature here, tentatively:

```text
117-rebrand-new-repo-bootstrap
```

That feature should not implement the entire graph operating system in the old
tree. It should design and create the new tree, prove that it is usable, then
freeze this repository into bridge mode.

The bootstrap feature's deliverables:

1. A brand matrix:
   - product name;
   - repository owner/name;
   - root namespace;
   - package ID prefix;
   - template package ID;
   - template `identity`;
   - template `shortName`;
   - docs domain/path;
   - NuGet owners;
   - GitHub organization/account;
   - CI and package-publishing secret names.
2. An old-to-new identity map:
   - package IDs;
   - assembly names;
   - namespaces;
   - docs URLs;
   - template names;
   - skill names;
   - build-engine package name;
   - generated product property names such as `FsSkiaUiVersion`.
3. A new repository skeleton:
   - `src/**` product libraries under the new namespace;
   - `tests/**` with only live tests and essential fixtures;
   - `build/**` with the graph-first governance kernel;
   - `template/**` with new package/template identity;
   - `docs/**` with new navigation and no active Spec Kit pages;
   - no `.specify` runtime;
   - no active `speckit-*` skills;
   - no historical readiness logs in the active tree.
4. A provenance record:
   - source repository URL;
   - source commit SHA;
   - selected imported directories;
   - intentionally dropped directories;
   - package ID mapping;
   - docs URL mapping;
   - reason for clean-history reset.
5. A cutover checklist:
   - create remote repository;
   - push new initial branch;
   - configure GitHub Pages or custom domain;
   - configure package publishing;
   - publish preview packages under the new package IDs;
   - publish the new template package;
   - deprecate old preview package versions with alternate packages;
   - update this repository's README to bridge users to the new project;
   - archive or freeze this repository after the bridge commit.

The bootstrap feature may generate the new repository under an artifact path
first, for example:

```text
artifacts/rebrand/<new-repo-name>/
```

Once the generated tree passes its own initial gates, it can be copied or pushed
to a new repository. The old repo should not keep evolving in parallel after
that point except for migration notes and emergency bridge fixes.

#### New Repository Minimum Viable Tree

The new repository should start smaller than this one. The current tree has
valuable product code, but it also carries years of process scaffolding,
historical evidence, generated fixtures, and name-specific tests.

Minimum viable tree:

```text
.
  README.md
  LICENSE
  Directory.Packages.props
  <new-solution>.slnx
  build/
    Build.fsproj
    Governance/
      <new-build-package>.fsproj
      Engine/
      SpecFlow/
      Routing/
      Evidence/
  src/
    Scene/
    Color/
    Layout/
    KeyboardInput/
    Input/
    SkiaViewer/
    Elmish/
    Controls/
    Controls.Elmish/
    Testing/
  tests/
    Scene.Tests/
    Layout.Tests/
    Controls.Tests/
    Elmish.Tests/
    Governance.Tests/
    Template.Tests/
  template/
    base/
    fragments/
    product-skills/
  docs/
    index.md
    architecture/
    controls/
    governance/
    migration/
    reports/
  .agents/
    skills/
      specflow-*
      fsharp-*
```

Excluded from the active new tree:

- `.specify/**`;
- generated `.claude/skills/**` if peer generation remains a build output
  rather than source;
- old readiness logs;
- archived specs except a curated migration note;
- old `speckit-*` skills;
- old community-extension catalog snapshots;
- template-generated product artifacts;
- API docs generated from old package names;
- stale docs pages whose only job was to explain the retired Spec Kit process.

#### Namespace And Package Policy

The brand decision must happen before code generation. The new root namespace
and package prefix should be stable, short, and not too tied to implementation
details if the project may later support renderers beyond Skia.

Bad outcomes to avoid:

- naming every package after a temporary renderer if the architecture wants
  backend independence later;
- keeping `FS.Skia.UI.*` package IDs while changing only the repository name;
- changing package IDs but leaving namespaces old;
- changing namespace/package names but leaving template skill names old;
- publishing the new template before published package IDs exist;
- deprecating old packages before the replacement packages can restore from
  nuget.org.

Recommended package policy:

1. Pick one root package prefix and one root namespace.
2. Keep package suffixes aligned with architectural packages:
   `Scene`, `Color`, `Layout`, `Input`, `SkiaViewer`, `Elmish`, `Controls`,
   `Controls.Elmish`, `Testing`, and the build engine if published.
3. Publish all replacement packages on the preview channel first.
4. Install the new template only after the replacement packages are available.
5. Deprecate old preview package versions with alternate package IDs and a
   short migration message.
6. Do not unlist old packages until the alternate-package guidance is visible
   and tested from a consumer project.

#### Docs And GitHub Pages Policy

The current docs site path is repository-name dependent. Because GitHub Pages
project-site URLs are not automatically redirected on repository rename or
transfer, the rebrand should either:

1. move to a custom documentation domain during cutover, or
2. publish a static bridge page from this repository that points to the new docs
   URL, while the new repository owns the new project-site path.

The custom-domain option is cleaner if the project is becoming a product with a
stable name. It decouples documentation identity from GitHub repository naming
and avoids repeating this problem on the next repo rename.

#### Issue And History Policy

The current repository has useful history, but for a preview framework the
highest-value migration artifact is not every old commit. It is a precise
mapping from old concepts to new ones:

| Old artifact | New handling |
|---|---|
| Open issues that still apply | Recreate or transfer manually into the new repo with old links. |
| Completed specs/readiness | Archive here; summarize only durable architectural decisions in the new repo. |
| Historical reports | Keep here; copy only the few reports that explain current architecture. |
| ADRs | Copy active ADRs after rewriting identities; retire ADRs about old governance placement. |
| Git blame | Preserve in old repository; optionally include source commit provenance in new files. |
| Release tags | Keep old tags here; start new preview version lineage in the new repo. |

This preserves auditability without forcing the new repository to carry every
obsolete process surface.

#### Cutover Acceptance Criteria

The bootstrap feature is complete only when these are true:

- The new repository has a coherent brand matrix and no active `FS.Skia.UI`
  identifiers except in migration docs.
- The new root namespace, package IDs, assembly names, docs URL, and template
  identity agree.
- The new graph-first workflow owns feature state through `feature.graph.json`
  and generated projections.
- The new tree has no active `.specify` runtime and no active `speckit-*`
  skills.
- The new build can restore, build, and test from a clean checkout.
- The new template can be packed, installed, used with the new `shortName`, and
  restored against replacement package IDs.
- The new docs site builds with the new navigation and URL assumptions.
- Package metadata uses the new repository URL, project URL, readme, tags, and
  icon/license policy.
- A NuGet deprecation plan exists for every old package ID, including alternate
  package IDs.
- This repository has a bridge README/report explaining the move, the source
  commit, the new repository, and the package/template migration.
- No active development task remains assigned to this repository after cutover
  except bridge maintenance.

#### Decision

Yes: if the project is being rebranded anyway, it makes sense to start over in a
new repository. The current repository should not be thrown away casually; it
should be used to design, generate, and prove the replacement. But the long-term
destination should be the new repository because the planned workflow redesign
and the rebrand both benefit from a clean active tree and a single coherent
identity.

### Full Governance Integration Research

The earlier graph redesign already planned to replace the Spec Kit workflow
authority. Further research shows there is a larger opportunity: integrate the
entire governance lifecycle into one project operating model. This does not mean
putting every rule into JSON. It means the graph owns the declared intent and
state, while compiled F# policy modules own deterministic validation and
generation.

#### External Platform Findings

| Source | Relevant finding | Design consequence |
|---|---|---|
| GitHub rulesets | Branch/tag rulesets and push rulesets can enforce merge status checks, path restrictions, file sizes, and other repository constraints. Multiple rulesets can apply together, and the most restrictive overlapping rule wins. | The project should model expected GitHub rulesets as generated policy, not as memory. The graph should produce a platform-policy report and, where credentials allow, compare expected rules with actual repository settings. |
| GitHub required status checks | Required status checks must pass before a protected branch/tag can merge; a ruleset can pin a status check to an expected GitHub App source. | A full governance model should name the required CI checks and their expected source. Local `Route` gates and remote required checks should be mapped explicitly so CI cannot silently diverge from local policy. |
| GitHub Actions reusable workflows | Reusable workflows are called as jobs via `uses`; same-repo calls use the workflow from the same commit, while cross-repo calls should use immutable refs for stability and security. Permissions can only be maintained or reduced through nested reusable workflows. | If the new repo uses reusable workflows, the graph should generate workflow callers or check them. Cross-repo reusable workflow refs should be pinned by SHA or release tag policy, and permission narrowing should be validated. |
| `GITHUB_TOKEN` permissions | GitHub recommends granting the token the least access needed. Actions can access the token even if it is not explicitly passed. | Workflow YAML should be graph-checked for explicit minimal `permissions:`. Release jobs should be the only jobs with publish-grade permissions. |
| GitHub artifact attestations | GitHub Actions can generate provenance attestations for binaries and SBOMs, with `id-token: write`, `contents: read`, and `attestations: write`; attestations can be verified with the GitHub CLI. | Release evidence should include package digests and attestation references. The graph should distinguish local build logs from CI provenance that can be verified later. |
| NuGet Trusted Publishing | nuget.org can exchange a GitHub Actions OIDC token for a short-lived, single-use API key when the repository, workflow, and environment match a trusted publishing policy. | Publish policy belongs in the graph: repository owner/name, workflow file, release environment, package owner, and expected package IDs. The publish gate should verify the declared policy before pushing. |
| NuGet authoring guidance | NuGet metadata affects discoverability, usability, and trust. SDK-style projects should carry package metadata in project files; readme files are a first-class package detail page surface. | Package metadata should be a declared package matrix, not a scattered `.fsproj` convention. Pre-publish checks should derive from that matrix and fail on missing or stale metadata. |
| Central Package Management | NuGet central package management uses `Directory.Packages.props` with `ManagePackageVersionsCentrally` and package-version rows. | Dependency and generated-template pins should be graph-owned or graph-checked. A rebrand should rename the single version property, package IDs, and generated product pins in one policy pass. |
| Source Link and package signing | Source Link embeds repository metadata during package creation; signed packages add authenticity/integrity checks. | Release policy should decide whether each package requires Source Link, signing, artifact attestation, or all three. The decision should be explicit, not accidental. |
| JSON Schema | JSON Schema is useful for editor validation and UI hints, but it is still a projection of validation vocabulary. | JSON Schema should be generated from the compiled graph model for authoring support. It should not replace the F# validator. |
| FAKE targets | FAKE targets can run dependencies unless `--single-target` is used; final and failure targets exist for cleanup/reporting. | The graph should record whether evidence came from a target including dependencies or a single-target run. Gate evidence should include the exact command, target, dependency behavior, and output paths. |
| SLSA provenance | SLSA provenance describes how artifacts were produced so consumers can verify build expectations and rebuild if needed. | Release evidence should move toward provenance-oriented rows: source commit, workflow identity, package digest, builder identity, and attestation location. |

The platform conclusion is straightforward: local governance cannot stop at
`spec.md`/`plan.md`/`tasks.md`. A serious project governance system must connect
local route/gate policy to GitHub merge policy, release policy, package
metadata, workflow permissions, and provenance. Otherwise the graph can say a
feature is ready while CI, publishing, or repository rules enforce a different
truth.

#### Local Governance Inventory

The repository already contains most of the necessary policy engines. The
problem is that they are stitched together by path conventions, target names,
readiness files, and documentation rather than by one governance state model.

| Local subsystem | Current authority | Full-integration destination |
|---|---|---|
| `Targets.fs` | Closed `Target` union, target metadata, prerequisites, cost, timeout, failure owner. | Becomes the compiled target catalog; graph stores target evidence, selected gates, and expected remote status checks. |
| `Routing.fs` | Compiled path-to-tier/gate rules and expected artifact presence. | Remains the route-rule authority; graph stores declared impact, expected route, actual route, rule trace, and route-drift evidence. |
| `TargetMetadata.fs` | Validates target metadata and rendered docs/contract references. | Fold into a broader `PolicyProjectionCheck` that covers target metadata plus GitHub, package, docs, template, and release policy projections. |
| `Engine/Model.fs` and `Update.fs` | Pure build model/effect pattern; active feature still comes from `.specify/feature.json`. | Reuse the pure-update/interpreter architecture; replace `.specify` feature resolution with `.specflow/current.json` and graph workspace state. |
| `Evidence/Engine.fs` | Pure graph/audit orchestration over edge-supplied files. | Promote to graph-native evidence ledger over typed task/evidence/release rows. |
| `Evidence/Graph.fs` | Cycle detection, topo sort, synthetic taint propagation. | Keep algorithms; operate over `TaskNode` in `feature.graph.json`. |
| `Evidence/Audit.fs` | Skill ownership, skill loading, SEH, synthetic verdicts, diff scans. | Keep policies; replace Markdown/YAML task parsing with graph fields and typed evidence rows. |
| `EvidenceFormatSchema.fs` | Single source for evidence-format reference docs. | Extend to a graph-owned evidence schema for gate, CI, package, docs, release, and provenance evidence. |
| `GeneratedProduct.fs` | Template pack/install/instantiate, generated project scans, consumer validation, package-skew checks. | Becomes generated-product policy under `TemplatePolicy` and `ConsumerPolicy`; graph records matrix rows and validation evidence. |
| `Capabilities.fs` | Typed `template/capabilities.yml` model and validation. | Move capability rows into the project graph or make the YAML a generated projection from it. |
| `ApiSurfaceGen.fs`, `PerPackageSurface.fs` | Public surface generation and per-package baseline checks. | Package/surface policy modules derive package matrices and baselines from graph identity and source contracts. |
| `Guidance.fs` | Generated-guidance scans, constitution-check areas, skill-id resolution. | Replace prose heuristics with graph-bound context-pack policy; keep scanners only for generated projection currency. |
| `SkillTreeGen.fs` / `SkillSync.fs` | `.agents` canonical, `.claude` generated mirror. | Make skill surfaces generated from graph/context-pack policy. Keep peer generation only if the new repo still supports multiple agent clients. |
| `PrePublish.fs` / `Publish.fs` | Package pin parity, metadata, publish plan, idempotent push. | Becomes `ReleasePolicy` + `PackagePolicy`; graph declares package IDs, versions, metadata, feed, trusted-publishing policy, and publish evidence. |
| `PackageSkew.fs` | Detects generated source references absent from pinned package surface. | Remains a package-consumer safety check under `ConsumerPolicy`. |
| Governance docs | Explain procedure and known limitations. | Become generated or checked projections from policy, with manual narrative limited to rationale. |

The most important local finding is that the project has already made the hard
architectural move: governance rules are compiled F# with pure cores and
interpreter edges. Full integration should not replace that. It should remove
the remaining fragmented state and turn the graph into the input/state model for
these existing engines.

#### Current Gaps Full Integration Would Close

The existing governance docs and code reveal concrete gaps that the full model
should close:

| Gap | Current behavior | Integrated behavior |
|---|---|---|
| Route output is plain text | Agents parse human text or rely on memory. | `specflow route --json` records route selection, matched rules, expected artifacts, and gate list in the graph. |
| `Route --enforce` checks presence only | A stale artifact can satisfy presence. | Evidence rows bind artifacts to graph hash, commit, command, target, and timestamp; stale evidence is rejected. |
| Whole-worktree route is merge-oriented | Authoring one report file in a dirty tree inherits unrelated gates. | Graph supports both merge route and scoped authoring route; scoped route cannot satisfy merge readiness. |
| Active feature comes from `.specify/feature.json` | Evidence gates depend on old runtime state. | `.specflow/current.json` points to the active graph; workspaces can pin a feature without global `.specify` mutation. |
| Package policy is separate from feature policy | Pre-publish checks live at release time, not design time. | Package/release impact is declared during planning and validated before implementation completion. |
| CI required checks are outside repo policy | GitHub settings can drift from local `Route` gates. | Graph generates or checks expected ruleset/status policy and records actual CI check evidence. |
| Workflow permissions are conventional | `.github/workflows/**` may drift from least-privilege policy. | `CiPolicyCheck` validates permissions, OIDC use, reusable workflow refs, and publish-job isolation. |
| Provenance is optional narrative | Local logs are treated as evidence, but release provenance is not modeled. | Release rows carry package digest, source commit, builder identity, attestation reference, and verification status. |
| Generated guidance still uses prose scanners | English wording can become a brittle contract. | Context packs are generated from graph/policy fields; prose scans are only projection sanity checks. |
| Rebrand cutover spans too many surfaces | Package/template/docs/repo identity changes can diverge. | `ProjectGraph` owns the brand matrix and generates package/template/docs/repo policy projections. |

#### Full Integration Decision

The full approach should be:

```text
ProjectGraph + FeatureGraph + EvidenceLedger + PolicyProjections
```

`FeatureGraph` owns feature-local workflow state. `ProjectGraph` owns
project-wide identity and governance policy. `EvidenceLedger` records every
authoritative proof event. `PolicyProjections` are generated views consumed by
humans, GitHub, NuGet, templates, docs, generated products, and agents.

The active authority split:

| Layer | Authority |
|---|---|
| Policy algorithms | Compiled F# modules in `build/Governance/**`. |
| Project identity and policy state | `.specflow/project.graph.json`. |
| Active feature state | `specs/<feature>/feature.graph.json`. |
| Active feature pointer | `.specflow/current.json`. |
| Evidence rows | Graph-owned ledger rows, with attached files as payloads. |
| Human-readable docs/reports | Generated or checked projections. |
| GitHub/NuGet/template/workflow config | Generated or checked projections from project policy. |

This gives the new repo a single mental model:

1. Author policy or feature intent in graph form.
2. Generate projections and platform files.
3. Run route-selected gates.
4. Record evidence rows.
5. Validate graph, projections, platform policy, and evidence freshness.
6. Release only when package/template/docs/CI/provenance policy all pass.

#### Governance Domains To Integrate

| Domain | Graph-owned fields | Compiled validator/generator |
|---|---|---|
| Brand and repository identity | product name, repo owner/name, docs URL, package prefix, root namespace, template short name. | `IdentityPolicy`, `RepositoryBootstrap`, `DocsPolicy`. |
| Target catalog | target ids, command strings, prerequisites, timeout/cost/failure owner, product-check classification. | Existing `Targets`, extended `TargetCatalogProjection`. |
| Routing and gates | declared impact, expected route, actual route evidence, rule trace, scoped authoring route. | Existing `Routing`, new `RoutePlanning` and `RouteEvidence`. |
| Feature workflow | requirements, scenarios, decisions, tasks, dependencies, approvals. | New `FeatureGraph` modules plus existing graph algorithms. |
| Evidence | gate run rows, CI run rows, package rows, docs rows, screenshots, research, manual observations. | `EvidenceLedger`, existing audit/scans adapted to graph. |
| Public API surface | package IDs, `.fsi` surfaces, baseline locations, source-link requirements. | Existing `PerPackageSurface`, `ApiSurfaceGen`, `PackagePolicy`. |
| Template and generated product | template identity, profiles, capabilities, generated skills, generated product matrix. | Existing `GeneratedProduct`, `Capabilities`, new `TemplatePolicy`. |
| Docs | site URL, generated API docs, navigation policy, report inventory, migration pages. | `DocsPolicy`, FSharp.Formatting projection checks. |
| Skills/context | agent surfaces, context packs, skill generation, tool availability. | Existing `SkillTreeGen`, `SkillSync`, new `ContextPackPolicy`. |
| CI and repository rules | required status checks, ruleset expectations, workflow permissions, reusable workflow refs. | `PlatformPolicy`, `CiPolicy`, optional GitHub API verifier. |
| Release and publish | package matrix, versions, metadata, readme, Trusted Publishing policy, feed, environment, package deprecation map. | Existing `PrePublish`, `Publish`, new `ReleasePolicy` and `DeprecationPolicy`. |
| Provenance | source commit, package digest, builder/workflow identity, attestation path/URL, SBOM reference. | `ArtifactProvenance`, optional GitHub attestation verifier. |
| Rebrand/cutover | old-to-new identity map, imported/dropped paths, bridge policy, archive policy. | `RepositoryBootstrap`, `MigrationPolicy`. |

#### Project Graph

Full integration needs a project-level graph in addition to per-feature graphs:

```text
.specflow/project.graph.json
```

It should carry long-lived state that is not owned by a single feature:

```fsharp
type ProjectGraph =
  { SchemaVersion: SchemaVersion
    Project: ProjectIdentity
    Repository: RepositoryPolicy
    Targets: TargetCatalogPolicy
    Routing: RoutingPolicy
    Packages: PackagePolicy
    Templates: TemplatePolicy
    Docs: DocsPolicy
    Skills: SkillPolicy
    Ci: CiPolicy
    Release: ReleasePolicy
    Provenance: ProvenancePolicy
    Migration: MigrationPolicy option
    Projections: ProjectProjectionState }
```

Rules:

- `ProjectGraph` is edited rarely and always routes as governance impact.
- `FeatureGraph.Impact` references project policy by stable ids, not copied
  strings.
- Package/template/docs/CI release surfaces are generated from or checked
  against `ProjectGraph`.
- If project policy changes, affected feature approvals and route plans become
  stale when their graph hash included old policy.

#### Evidence Ledger

Evidence should become a ledger, not a folder scan. The minimum row shape:

```fsharp
type EvidenceItem =
  { Id: string
    Scope: EvidenceScope
    Kind: EvidenceKind
    Status: EvidenceStatus
    Authoritative: bool
    ProducedBy: Producer
    Command: CommandEvidence option
    Ci: CiEvidence option
    Artifact: ArtifactEvidence option
    AppliesToGraphHash: string option
    AppliesToProjectGraphHash: string option
    AppliesToCommit: string option
    Paths: string list
    CreatedAtUtc: DateTimeOffset option
    Notes: string option }
```

Additional evidence kinds for full integration:

```fsharp
type EvidenceKind =
  | GateRun
  | CiRun
  | RulesetSnapshot
  | WorkflowPolicyCheck
  | PackagePack
  | PackagePublish
  | PackageDeprecation
  | ArtifactAttestation
  | SourceLinkCheck
  | DocsBuild
  | TemplatePack
  | TemplateInstantiate
  | GeneratedProductValidation
  | Research
  | ApprovalEvidence
  | ManualObservation
```

Freshness rules:

- Feature evidence must bind to the feature graph hash or to a commit that
  contains that graph.
- Feature readiness evidence must also bind to the project graph hash used for
  route, target, package, and policy validation.
- Route evidence must bind to the diff or commit range it classified.
- Package/release evidence must bind to package ID, version, digest, source
  commit, and build workflow identity.
- Docs evidence must bind to docs source commit and published target URL or
  generated output path.
- Manual observation can explain a decision but is not authoritative unless a
  policy explicitly allows it.

This removes the main weakness of `Route --enforce`: file presence stops being
proof. An artifact satisfies policy only if the graph row says what produced it,
which graph/commit it applies to, and which requirement or gate it covers.

#### Platform Policy

The graph should not require online GitHub access for normal local checks. Split
platform policy into deterministic projection and optional verification:

| Mode | Behavior |
|---|---|
| Offline deterministic | Validate generated `.github/workflows/**`, expected status-check names, workflow permissions, and local ruleset projection files. |
| Online advisory | With credentials, call GitHub APIs or `gh` to compare actual repository rulesets, branch protection, environments, and latest required checks to the graph projection. |
| Online release gate | For publishing, require CI-provided OIDC/trusted-publishing identity and release-environment approval evidence. |

Expected generated/checkable files:

```text
.github/workflows/ci.yml
.github/workflows/publish.yml
.github/rulesets/*.json
docs/governance/platform-policy.md
readiness/platform-policy.md
```

Rules:

- Workflow jobs default to `permissions: contents: read`.
- Only publish/attestation jobs may request `id-token: write`.
- Only attestation jobs may request `attestations: write`.
- Release jobs must be environment-protected.
- Required status check names must map to `Targets.Target` or declared CI-only
  checks.
- Cross-repository reusable workflows must use SHA or release-tag policy.
- Any workflow that publishes packages must declare its trusted-publishing
  policy fields in `ReleasePolicy`.

#### Release Policy

The graph should model publishing as a governed state transition:

```fsharp
type ReleasePolicy =
  { Packages: ReleasePackage list
    Versioning: VersionPolicy
    Feeds: FeedPolicy list
    TrustedPublishing: TrustedPublishingPolicy option
    RequiredEvidence: ReleaseEvidenceRequirement list
    Deprecations: PackageDeprecationPlan list }
```

Release acceptance requires:

- package matrix matches packable projects and template package;
- package IDs and assembly names match brand policy;
- versions are internally consistent;
- central package pins match package matrix;
- package metadata exists and points at the new repository/docs;
- readme files are included and renderable by nuget.org constraints;
- Source Link policy is satisfied or explicitly not required;
- package signing/attestation policy is satisfied or explicitly deferred;
- Trusted Publishing policy matches repository owner, repository name, workflow
  file, and environment;
- publish plan is idempotent and records `Push`/`Skip` decisions;
- old package deprecations are planned and linked to replacement IDs.

This is a direct extension of `PrePublish.fs` and `Publish.fs`, not a
replacement. The pure rules already exist; the graph supplies a broader package
matrix and evidence model.

#### Template And Generated-Product Policy

Template governance should be fully graph-owned because the rebrand makes
template identity central:

```fsharp
type TemplatePolicy =
  { PackageId: string
    Identity: string
    DisplayName: string
    ShortName: string
    SourceName: string
    Profiles: TemplateProfile list
    CapabilityIds: string list
    GeneratedSkills: GeneratedSkillPolicy list
    GeneratedDocs: GeneratedDocsPolicy
    PackagePins: PackagePinPolicy }
```

Rules:

- template package ID, template `identity`, package README install command, and
  generated docs agree;
- every profile has an expected generated-product row;
- every selected capability maps to package IDs and contract files;
- generated product `Directory.Packages.props` is derived from the package
  matrix;
- generated skill names are derived from context-pack policy, not inherited
  from the old brand;
- generated product scans record typed evidence rows instead of only readiness
  files.

#### Framework-To-Consumer Contract Flow

The two-tier development topology should stay:

```text
framework repo -> template package -> user-generated product
```

But the redesigned model should characterize it more strictly:

```text
framework contract -> template archetype -> consumer product instance
```

This is the right split for FS.GG.UI because the framework remains the upstream
runtime and policy owner, the template remains the recommended consumer entry
point, and generated projects remain the place where real users build product
code. The change is that the template can no longer be an independently
maintained convenience layer. It must be a projection of `ProjectGraph` and
`ProductGraph`, and each generated product must record the exact contract slice
it consumed.

Layer responsibilities:

| Layer | Owns | Must not own |
|---|---|---|
| Framework contract | Package matrix, public surfaces, capabilities, profiles, validation policy, upgrade rules, docs contract, evidence requirements. | Per-application feature decisions. |
| Template archetype | A generated/checkable projection of supported profiles, pins, starter files, docs, skills, and validation commands. | Independent capability names, package pins, or workflow rules. |
| Consumer product instance | User application code plus a lean generated consumer graph recording profile, packages, capabilities, durable files, replaceable files, validation commands, upgrade state, and support-bundle policy. | The full maintainer governance runtime. |

Rules:

- Template metadata, profiles, capability lists, package pins, generated docs,
  generated skills, and validation commands are generated or drift-checked from
  `ProjectGraph` and `ProductGraph`.
- A capability, control, package, generated file, or validation command that is
  absent from the graphs cannot silently appear in the template.
- Generated products carry a lean `.specflow/consumer.graph.json` with the
  selected template profile, FS.GG.UI package matrix, enabled capabilities,
  durable user-owned files, replaceable generated files, validation commands,
  upgrade state, and support-bundle policy. A compact
  `.specflow/consumer.contract.json` may be generated from it for tooling that
  only needs a manifest.
- Generated products get a lean consumer-mode `specflow` surface, not the full
  maintainer graph operating system. Consumer mode should cover package-version
  checks, template-profile checks, generated-product health checks, optional
  feature graph workflow, and upgrade guidance.
- Generated-product validation simulates a real consumer: restore from the
  package feed under test, instantiate the template, build the result, run the
  selected profile smoke/headless checks, and prove that validation does not
  depend on a framework source checkout.
- Upgrade becomes a first-class report or command. It names the current
  consumer graph, target package/template version, changed replaceable files,
  durable files to preserve, and manual migration notes.
- Consumer friction feeds back as typed scenario/product evidence: affected
  profile, capability, missing docs/test/template support, and the ProductGraph
  or ProjectGraph row that must change.

Suggested generated consumer graph shape:

```fsharp
type ConsumerGraph =
  { SchemaVersion: SchemaVersion
    GraphId: string
    Project: ProjectPolicyRef
    Product: ProductContractRef
    TemplateProfile: string
    GeneratedFromPackageMatrix: string
    EnabledCapabilities: string list
    DurableFiles: string list
    ReplaceableFiles: string list
    ValidationCommands: string list
    UpgradeState: ConsumerUpgradeState
    SupportBundlePolicy: ConsumerSupportBundlePolicy }
```

Template profiles should be graph rows, not hand-maintained template folklore:

| Profile | Purpose | Expected graph-owned outputs |
|---|---|---|
| `minimal-scene` | Smallest renderable app. | Runtime package pins, one scene, minimal docs, build validation. |
| `controls-app` | Typical UI app with controls. | Controls packages, typed-control examples, interaction checks, docs links. |
| `governed-product` | App that wants local feature/evidence workflow. | Lean consumer `specflow`, ConsumerGraph, feature graph starter, validation commands. |
| `sample-pack` | Product demos and catalog examples. | Scenario rows, docs samples, screenshot/evidence expectations. |
| `headless-validation` | CI-friendly validation host. | Headless packages, sample smoke checks, generated-product health checks. |

The resulting characterization is:

```text
FS.GG.UI maintains the framework contract.
The template projects the contract.
The generated product records which contract slice it consumes.
```

#### Documentation Policy

Docs are a product surface after a rebrand. The graph should declare:

- docs URL base;
- docs publish mode: GitHub Pages project site, custom domain, or other host;
- generated API-reference source;
- report inventory policy;
- migration/bridge page paths;
- whether old brand links are allowed in active docs.

Checks:

- active docs do not link to old repo/package names except migration pages;
- package readmes link to current docs;
- template README install/scaffold commands match template policy;
- generated API docs match package matrix;
- FSharp.Formatting frontmatter fields are present where policy requires them.

#### Security And Trust Policy

The graph should make trust assumptions explicit:

| Trust surface | Policy row |
|---|---|
| Maintainer local gates | Allowed as feature evidence, not release provenance. |
| GitHub Actions CI | Required for merge/release evidence when publishing. |
| GitHub rulesets | Expected platform policy; optionally verified online. |
| NuGet Trusted Publishing | Required for production publish once configured. |
| Artifact attestations | Required or deferred per package class. |
| Manual approval | Binds to graph hash and scope. |
| AI review | Advisory unless a human or explicit policy promotes it. |

This resolves a recurring ambiguity: local evidence proves development
correctness; CI provenance proves release origin. They are related, but not the
same proof.

#### Integration Options

| Option | Description | Gain | Cost | Recommendation |
|---|---|---|---|---|
| Workflow-only | Only move spec/plan/tasks/evidence into the graph. | Fixes authoring drift. | Leaves route/package/CI/release/doc policy fragmented. | Too small for the rebrand. |
| Governance-kernel integration | Add project graph, route/target/evidence/package/template/docs policy and generated projections. | One operating model; high drift reduction. | Larger initial design; more schema. | Minimum acceptable scope. |
| Full lifecycle integration | Governance-kernel plus CI rulesets, Trusted Publishing, provenance, deprecation, repository bootstrap. | Rebrand and release are auditable from day one. | Requires staged implementation and some online checks. | Recommended target. |
| Standalone rule kernel now | Extract only the generic typed inference substrate, graph-hash primitives, provenance, diagnostics, and explanation helpers. | Gives the other projects a cheap adoption path and keeps FS.GG.UI from freezing local policy as a public API. | One more package boundary and compatibility matrix. | Recommended, with strict scope. |
| Separate FS.GG.UI governance product now | Extract SpecFlow governance policy into a reusable standalone repo/package immediately. | Reusable outside this framework if other projects adopt the same policy model. | Splits focus during rebrand; premature API freeze; leaks FS.GG.UI package/template/release assumptions. | Defer until FS.GG.UI and one other real project converge on shared policy. |

Decision: implement full lifecycle integration in the new repository, but stage
it through the bootstrap feature. Extract the **generic rule kernel** as a small
standalone package before or alongside the bootstrap. Do not extract
FS.GG.UI-specific governance policy as a standalone product yet.

### Split-Kernel And Standalone Reuse Research

The 2026-06-06 extraction plan and the 2026-06-07 detailed design were correct
about the first boundary: move pure governance logic away from the FAKE/build
edge, keep generated-product compatibility facades stable, and avoid inventing a
new policy language. Their only now-stale assumption is the standalone timing.
They deferred a separate reusable package until a second real context existed.
That condition is now satisfied by the two candidate projects.

The design should therefore split into three package layers:

```text
FS.GG.RuleKernel
  Generic deterministic rule/evidence substrate.
  No FS.GG.UI, FAKE, package, template, release, or repository-layout types.

FS.GG.Governance
  Optional shared governance contracts: project graph, feature graph,
  evidence ledger, route decision DTOs, diagnostics, and context-pack models.
  May remain internal/pre-release until a second project proves the vocabulary.

FS.GG.UI.Governance
  FS.GG.UI policy adapter: package matrix, template profiles, docs policy,
  product graph, generated-product validation, release policy, and bridge
  compatibility with the current `FS.Skia.UI.Build` facade.
```

The incorporation test is intentionally strict: a non-FS.GG.UI project should be
able to reference `FS.GG.RuleKernel`, define its own fact union and small rule
set, and get useful `validate`, `explain`, and `required-evidence` output with
roughly 50-150 lines of adapter code. If adoption requires copying FS.GG.UI
directory layout, FAKE target names, template profiles, package vocabulary, or
readiness file conventions, the extraction failed.

#### What The Rule Kernel Does

The standalone kernel should own:

| Capability | Kernel responsibility |
|---|---|
| Nominal IDs | `RuleId`, `FactId`, `QueryId`, `GraphHash`, `EvidenceId`, `SourceRef`, and validating constructors. |
| Fact store | Deterministic de-duplication by caller-supplied identity function. |
| Rule evaluation | Fixed-point monotonic derivation with max-iteration diagnostics and stable trace order. |
| Provenance | Every derived fact records rule id, input fact ids, source references, and explanation text. |
| Queries | Pure query helpers that convert evaluated facts into decisions and diagnostics. |
| Evidence vocabulary | Generic evidence rows, freshness binding, payload references, and authoritative/informational status. |
| Rendering DTOs | Stable JSON-friendly results for `--json`, projection checks, and downstream tools. |
| Test laws | Idempotence, determinism, convergence, provenance completeness, monotonicity where declared, and round-trip JSON stability. |

It must not own:

- FAKE targets or process execution;
- git diff collection or filesystem walking;
- FS.GG.UI package IDs, template profiles, docs URLs, controls, Skia hosts, or
  product contracts;
- release/publish decisions tied to NuGet, GitHub, or a repository identity;
- generated-product validation.

#### What FS.GG.UI Gets From The Split

For FS.GG.UI, the standalone kernel turns governance from a local build package
into a composed architecture:

```text
repository snapshot
  -> FS.GG.UI adapter facts
  -> RuleKernel evaluation
  -> FS.GG.UI policy queries
  -> route, evidence, product, package, release, and agent decisions
```

That gives FS.GG.UI:

- fast pure tests for route, evidence, freshness, and authorization decisions;
- explainable `why this gate`, `why this artifact`, and `what is stale` output;
- a clean bootstrap slice for the new repository;
- a smaller generated-product facade that can call stable DTO APIs without
  loading the full maintainer runtime;
- a way to compare FS.GG.UI policy against the two pilot projects and move only
  genuinely shared vocabulary upward.

The package boundary should be reviewed as a product promise. `FS.GG.RuleKernel`
can become stable earlier because its surface is small. `FS.GG.Governance` and
`FS.GG.UI.Governance` should stay preview until at least one external project
has completed a real feature using them.

#### Boundary Decision

Use the Hopac-style lesson from the split design: expose a small semantic F#
surface and keep machinery internal. Public users should see facts, rules,
queries, evidence rows, diagnostics, and explanations. They should not see
mutable work queues, repository scanners, FAKE-specific types, or FS.GG.UI
policy internals.

Interfaces are acceptable for binary/effect boundaries such as snapshot
providers and optional renderers. Facts, route conclusions, evidence statuses,
and product-policy decisions should remain closed discriminated unions and
records so the compiler forces exhaustiveness when policy evolves.

### Product Contract Integration Plan

Full governance integration still leaves a product-contract problem: the library
surface is spread across source modules, `.fsi` files, package projects,
readiness baselines, docs pages, screenshots, samples, generated template
fragments, capability YAML, test corpora, skills, and historical specs. The
project already has checks for many of those surfaces, but they are mostly
connected by convention. The next integration layer should make the product
contract explicit.

The proposed shape:

```text
ProjectGraph + ProductGraph + FeatureGraph + EvidenceLedger + PolicyProjections
```

`ProjectGraph` owns repository operation. `ProductGraph` owns product promises.
`FeatureGraph` owns feature work. `EvidenceLedger` proves both policy and
product claims. `PolicyProjections` and `ProductProjections` render the files
that humans, docs, templates, tests, and CI consume.

#### ProductGraph Scope

The product graph should live beside the project graph:

```text
.specflow/product.graph.json
```

It should carry stable product contract rows:

```fsharp
type ProductGraph =
  { SchemaVersion: SchemaVersion
    GraphId: string
    Project: ProjectPolicyRef
    Capabilities: CapabilityContract list
    Controls: ControlContract list
    PublicSurfaces: PublicSurfaceContract list
    VisualEvidence: VisualEvidenceContract list
    Scenarios: ScenarioContract list
    PerformanceBudgets: PerformanceBudgetContract list
    InteractionContracts: InteractionContract list
    AccessibilityContracts: AccessibilityContract list
    ArchitectureTrace: ArchitectureTraceContract list
    Hosts: HostRuntimeContract list
    DesignSystem: DesignSystemContract option
    SupportBundles: SupportBundleContract list
    Toolchain: ToolchainEnvironmentContract
    Projections: ProductProjectionState
    Lifecycle: ProductLifecycleState }
```

Rules:

- `ProductGraph` is product-facing, not workflow-facing. It does not contain
  feature tasks.
- Product rows are referenced by feature impact, tests, docs, samples, template
  profiles, and evidence rows.
- Product rows can be generated from existing source/catalogs during bootstrap,
  but after cutover they become the stable contract source.
- A feature that changes a product row must declare product impact before it can
  satisfy readiness.

#### Candidate 1 - Control And Capability Registry

Current surfaces:

- `src/**` and public `.fsi` files;
- `template/capabilities.yml`;
- `docs/controls/**`;
- `docs/img/controls/**`;
- `samples/*Gallery/**`;
- `readiness/surface-baselines/**`;
- `readiness/per-package-surface/**`;
- `tests/Controls.Tests/**`, `tests/Package.Tests/**`, and generated product
  validation.

Destination:

```fsharp
type CapabilityContract =
  { Id: string
    DisplayName: string
    PackageId: string
    SourceModules: string list
    PublicSurfaceIds: string list
    DocsPages: string list
    TemplateFragments: string list
    SampleIds: string list
    SkillIds: string list
    RequiredEvidenceKinds: EvidenceKind list }

type ControlContract =
  { Id: string
    DisplayName: string
    CapabilityId: string
    WidgetType: string option
    PropsType: string option
    DocsPage: string
    ScreenshotId: string option
    GallerySampleId: string option
    Tests: string list
    AccessibilityContractIds: string list
    InteractionContractIds: string list
    PerformanceBudgetIds: string list }
```

Integration plan:

- Import `template/capabilities.yml` into `ProductGraph.Capabilities`.
- Import `docs/controls/*.md` and `docs/img/controls/*.png` into
  `ControlContract` rows.
- Bind each control row to its typed control surface, widget surface, docs page,
  screenshot, gallery sample, and required tests.
- Generate or check `template/capabilities.yml`, docs catalog pages, and control
  screenshot manifests from the graph.
- Make orphan controls, orphan docs pages, missing screenshots, and missing
  template fragments product-check failures.

Gain:

- A control cannot exist only in code, only in docs, only in screenshots, or
  only in template metadata.
- Package-surface, docs, sample, and generated-template drift become one
  diagnostic instead of several unrelated failures.

#### Candidate 2 - Visual Evidence Pipeline

Current surfaces:

- `docs/img/controls/**`;
- `tests/ControlsPreview.Harness/fixtures/fidelity/**`;
- screenshot gallery samples;
- contrast and design-token tests;
- sample-smoke logs under feature readiness;
- image assets copied into docs output.

Destination:

```fsharp
type VisualEvidenceContract =
  { Id: string
    SubjectId: string
    Kind: VisualEvidenceKind
    SourceScenarioId: string
    ExpectedImagePath: string option
    ActualImagePath: string option
    DiffPath: string option
    EnvironmentId: string
    Tolerance: VisualTolerance
    RequiredForRelease: bool }
```

Integration plan:

- Define visual evidence rows for control docs images, preview harness fidelity
  fixtures, gallery screenshots, and smoke screenshots.
- Bind every visual row to a scenario, environment, and product subject.
- Make generated docs images traceable to a control/scenario row.
- Record visual evidence as typed `EvidenceItem` rows with environment and
  tolerance metadata.
- Keep advisory screenshots possible, but prevent advisory rows from satisfying
  required visual proof.

Gain:

- The docs image, preview fixture, gallery output, and visual test for a control
  become one contract.
- Visual regressions can be diagnosed by product subject and scenario, not by
  whichever PNG happened to differ.

#### Candidate 3 - Scenario Corpus

Current surfaces:

- `docs/testSpecs/**`;
- gallery/sample projects;
- generated product profiles;
- smoke tests;
- consumer-friction historical features;
- performance corpus rows;
- scenario-specific readiness files.

Destination:

```fsharp
type ScenarioContract =
  { Id: string
    Domain: ScenarioDomain
    Title: string
    ProductSubjects: string list
    RequiredPackages: string list
    SamplePath: string option
    TestSpecPath: string option
    GeneratedProfile: string option
    RequiredEvidence: EvidenceRequirement list
    VisualEvidenceIds: string list
    PerformanceBudgetIds: string list
    AccessibilityContractIds: string list }
```

Integration plan:

- Import `docs/testSpecs/**` as scenario seeds.
- Register gallery samples and generated-product profiles as scenario
  implementations.
- Connect smoke tests and sample logs to scenario IDs.
- Let feature plans declare which scenarios are affected by a change.
- Generate a scenario matrix for docs and release readiness.

Gain:

- The project can answer "which user workflow proves this product claim?"
  mechanically.
- Generated product validation and sample smoke evidence can be planned from the
  same scenario corpus instead of separate template/gate conventions.

#### Candidate 4 - Performance And Budget Model

Current surfaces:

- retained render metrics features 109-116;
- performance corpus tests;
- memoization, virtualization, frame scheduler, pointer routing, damage rects,
  cache metrics, and layout hot-path work;
- readiness performance reports and golden text.

Destination:

```fsharp
type PerformanceBudgetContract =
  { Id: string
    ScenarioId: string
    Metric: PerformanceMetric
    Budget: BudgetThreshold
    EvidenceKind: EvidenceKind
    RequiredForMerge: bool
    RequiredForRelease: bool }
```

Integration plan:

- Register stable metrics: frame count, view skips, touched nodes, recomputed
  nodes, shifted nodes, dirty rect count, dirty area, picture-cache hits/misses,
  retained routing fallbacks, allocations where available.
- Bind each budget to a scenario and product subject.
- Store budget evidence as typed rows instead of plain corpus logs.
- Allow advisory exploratory metrics, but keep release budgets explicit.
- Generate performance dashboards/reports from ProductGraph and evidence rows.

Gain:

- Performance is no longer a sequence of feature-local reports. It becomes a
  standing product contract with scenario-owned budgets and evidence freshness.

#### Candidate 5 - Interaction And Accessibility Contract

Current surfaces:

- keyboard input framework;
- pointer routing and retained hit testing;
- focus traversal;
- visual state stamping and cross-fade;
- control diagnostics;
- accessibility tests and docs guidance;
- control-specific interaction tests.

Destination:

```fsharp
type InteractionContract =
  { Id: string
    SubjectId: string
    InputModes: InputMode list
    FocusBehavior: FocusBehavior option
    PointerBehavior: PointerBehavior option
    KeyboardBehavior: KeyboardBehavior option
    VisualStates: string list
    RequiredTests: string list }

type AccessibilityContract =
  { Id: string
    SubjectId: string
    Role: string option
    NameSource: string option
    KeyboardReachable: bool
    ContrastPolicy: string option
    DiagnosticPolicy: string option
    RequiredTests: string list }
```

Integration plan:

- Register per-control interaction and accessibility rows.
- Bind focus, pointer, keyboard, visual state, contrast, and diagnostic tests to
  those rows.
- Generate docs snippets or reference tables from the contract.
- Require affected controls to update interaction/accessibility rows when their
  public behavior changes.

Gain:

- Interaction behavior becomes part of the product contract, not just a test
  suite side effect.
- Controls can be compared for consistency across focus, pointer, keyboard,
  disabled, hover, pressed, and validation states.

#### Candidate 6 - Architecture And ADR Traceability

Current surfaces:

- `docs/adr/**`;
- `docs/architecture/**`;
- implementation plans and reports;
- source modules;
- feature specs;
- governance docs.

Destination:

```fsharp
type ArchitectureTraceContract =
  { Id: string
    Subsystem: string
    SourcePaths: string list
    ArchitectureDocs: string list
    AdrIds: string list
    FeatureIds: string list
    Supersedes: string list
    Status: ArchitectureStatus }
```

Integration plan:

- Register architecture docs and ADRs by subsystem.
- Bind source path groups to architecture trace rows.
- Require high-risk subsystem changes to cite an existing ADR, add a new ADR, or
  record an explicit "no architecture update" decision.
- Generate an architecture traceability report.

Gain:

- Architecture docs become checked product memory instead of a parallel prose
  archive.
- Contributors can navigate from subsystem to ADRs, current docs, historical
  features, and affected source paths.

#### Candidate 7 - Toolchain And Environment Graph

Current surfaces:

- `global.json` or SDK assumptions;
- `Directory.Packages.props`;
- Skia/native startup assumptions;
- CI images;
- screenshot/fidelity environment;
- package sources;
- docs build environment;
- sample smoke environment.

Destination:

```fsharp
type ToolchainEnvironmentContract =
  { DotnetSdk: string option
    TargetFrameworks: string list
    PackageSources: string list
    NativeDependencies: NativeDependency list
    CiImages: CiImageContract list
    VisualEvidenceEnvironments: VisualEnvironment list
    DocsEnvironment: DocsEnvironmentContract option
    GeneratedProductEnvironment: GeneratedProductEnvironment option }
```

Integration plan:

- Record the SDK, target frameworks, package sources, Skia/native assumptions,
  docs build assumptions, and CI image expectations.
- Bind visual evidence to a named visual environment.
- Bind generated product validation to a named generated-product environment.
- Reject release evidence whose environment does not match policy unless
  explicitly allowed.

Gain:

- Native/rendering and docs evidence becomes reproducible enough to audit.
- Environment drift is diagnosed directly instead of appearing as unrelated
  screenshot, package, or smoke failures.

#### ProductGraph Integration Decision

Add `ProductGraph` to the full redesign, but stage it after the project graph
kernel and before the new repository becomes the active development home.

Minimum viable ProductGraph for bootstrap:

- package/capability rows;
- control rows for the existing docs catalog;
- public surface rows;
- scenario rows for current galleries and generated profiles;
- visual evidence rows for docs/control images and preview fixtures;
- performance budget rows for the retained-render corpus;
- interaction/accessibility rows for controls with existing tests;
- architecture trace rows for current architecture docs and ADRs;
- host runtime rows for windowed, headless, screenshot, sample, and
  generated-product hosts;
- design-system rows for token source, generated modules, themes, density,
  visual states, and contrast obligations;
- support-bundle rows for consumer diagnostics and issue reproduction;
- toolchain/environment rows for SDK, target frameworks, package sources, and
  visual evidence environment.

Do not build a generic product-management system. The graph should only model
contract facts that are already enforced, documented, released, or needed for
the rebrand cutover.

### Additional Integrated Contract Surfaces

The same analysis exposes six more implicit contracts that should be promoted
into the redesign. The first is the `ConsumerGraph` described above; the other
five close gaps that would otherwise become parallel systems after FS.GG.UI
ships.

#### Addition 1 - ConsumerGraph

Generated products should not only contain a flat manifest. They should contain
a small, consumer-owned graph:

```text
framework policy -> product contract -> template profile -> consumer graph
```

The consumer graph is intentionally much smaller than the maintainer graph. It
records only the selected template profile, package matrix, enabled
capabilities, durable and replaceable files, validation commands, upgrade
state, and support-bundle policy. It gives generated apps enough structure for
health checks, upgrades, and issue reproduction without forcing them to carry
the full FS.GG.UI governance kernel.

#### Addition 2 - Schema Migration Policy

`SchemaVersion` is necessary but not sufficient. Once FS.GG.UI is live, project,
product, feature, and consumer graphs will evolve. Schema evolution should be a
first-class policy:

```fsharp
type GraphSchemaMigrationPolicy =
  { CurrentVersion: SchemaVersion
    SupportedVersions: SchemaVersion list
    Migrations: GraphMigrationStep list
    Fixtures: GraphMigrationFixture list
    RetentionPolicy: SchemaRetentionPolicy }
```

Rules:

- `specflow graph migrate` upgrades old graph files through deterministic,
  reviewed migration steps.
- Each migration has before/after golden fixtures.
- A graph that uses an unsupported schema fails with a diagnostic naming the
  required migration path.
- Migration commands rewrite graph state, then regenerate projections; they do
  not silently reinterpret old files during readiness checks.

#### Addition 3 - Package And Module Layer Contract

The package matrix owns names and versions, but it should also own architectural
direction. FS.GG.UI should make package/module boundaries explicit:

```fsharp
type PackageLayerContract =
  { Id: string
    Packages: string list
    AllowedReferences: string list
    ForbiddenReferences: string list
    PublicNamespaces: string list
    InternalNamespaces: string list
    TestOnlyReferences: string list }
```

Rules:

- Runtime, controls, Elmish integration, Skia viewer, template, samples, tests,
  and governance modules have declared layers.
- Project references and namespace ownership are checked against layer policy.
- Test-only references cannot leak into packable projects.
- Governance code can inspect product metadata, but product runtime packages do
  not depend on the governance kernel.

This protects the complete-break redesign from creating a new repo with clean
names but tangled project dependencies.

#### Addition 4 - Host Runtime Contract

The ProductGraph models controls and scenarios, but host behavior is also a
product contract. Add host runtime rows for windowed, headless, screenshot,
sample, and generated-product hosts:

```fsharp
type HostRuntimeContract =
  { Id: string
    HostKind: HostKind
    RenderLoop: RenderLoopPolicy
    Scheduler: SchedulerPolicy
    FrameClock: FrameClockPolicy
    InputRouting: InputRoutingPolicy
    DpiPolicy: DpiPolicy
    NativeDependencies: NativeDependency list
    HeadlessModes: string list
    ScreenshotModes: string list
    ResourceLifetime: ResourceLifetimePolicy }
```

Rules:

- Template profiles select host contracts.
- Scenario, visual, and performance evidence cite the host contract they ran
  under.
- Headless validation, screenshot rendering, and live viewer behavior have
  separate contracts instead of hidden test assumptions.
- Host changes route as product impact when they affect input, rendering,
  scheduling, screenshot, native dependency, or generated-product behavior.

#### Addition 5 - Design System Contract

Design tokens are already a disciplined surface in this repository. The new
design should make them part of the product contract, not a side process:

```fsharp
type DesignSystemContract =
  { TokenSource: string
    GeneratedModules: string list
    Themes: string list
    DensityModes: string list
    Typography: TypographyPolicy
    Radii: RadiusPolicy
    VisualStates: string list
    ContrastRequirements: ContrastRequirement list
    EvidenceIds: string list }
```

Rules:

- DTCG token sources, generated F# token modules, theme names, density modes,
  typography, radii, visual states, and contrast obligations are graph-owned or
  graph-checked.
- Control interaction and accessibility rows reference design-system IDs.
- Visual evidence names the theme/density/design-system slice under test.
- Template profiles expose only design-system combinations declared in the
  graph.

#### Addition 6 - Support Bundle And Reproduction Contract

The release/provenance model explains how packages were produced. It does not
yet explain how a user issue becomes reproducible product evidence. Add a
support-bundle contract:

```fsharp
type SupportBundleContract =
  { Id: string
    CommandName: string
    CollectedFields: SupportField list
    RedactionPolicy: RedactionPolicy
    OutputFormat: SupportBundleFormat
    ScenarioBinding: string option
    Attachments: SupportAttachmentPolicy list }
```

Rules:

- Consumer mode provides `specflow consumer support-bundle`.
- The bundle records package versions, project/product/consumer graph hashes,
  selected template profile, enabled capabilities, OS, SDK, Skia/native
  environment, host contract, scenario ID where known, logs, and optional
  screenshots.
- Redaction defaults are part of policy, not left to ad-hoc issue templates.
- Support bundles can be imported as non-authoritative evidence first, then
  promoted to scenario/product evidence when a maintainer links them to a
  reproducible contract gap.

## Corrected Assumptions

The initial radical sketch was directionally right but too casual in several
places. Research corrected these assumptions:

1. **Do not build a second task graph parser.** The repo already has cycle
   detection, topo sort, synthetic propagation, skill registry validation, and
   Mermaid rendering. The redesign should promote those algorithms, not rewrite
   them.
2. **Do not keep Markdown as editable workflow state.** Keeping hand-editable
   Markdown while adding a graph creates two authorities. Markdown must become a
   projection.
3. **Do not preserve upstream extension semantics.** `category` and `effect`
   are useful metadata; external extension installation is not.
4. **Do not make `tasks.md` checkboxes authoritative.** Completion must be
   evidence-backed and graph-owned.
5. **Do not make approvals prose-only.** Review gates need structured approval
   artifacts with scope, author, time, and graph hash.
6. **Do not cache verdicts.** Cache parsed inputs and projections only. Gate
   results and audit verdicts are evidence events, not reusable truths.
7. **Do not derive active feature from git branch.** Feature activation is an
   explicit graph/workspace state.
8. **Do not treat a GitHub rename as a product rebrand.** GitHub redirects are
   useful compatibility behavior, but docs URLs, actions, packages, templates,
   and generated product identity all need deliberate cutover.
9. **Do not assume package names can be renamed in place.** NuGet package IDs
   are package identities. Rebranded packages are new packages, with old package
   versions deprecated toward alternates.
10. **Do not let historical identity govern the new active tree.** Old specs,
    readiness logs, package baselines, and generated fixtures are archive data.
    The new repository should copy durable product code and current decisions,
    not every process artifact.
11. **Do not implement the full graph operating system in the old brand if the
    new repo is accepted.** This repository's next job should become bootstrap
    and cutover, not long-lived workflow evolution.
12. **Do not stop at workflow governance.** The feature graph alone does not
    integrate package, template, docs, CI, release, platform, and provenance
    policy. Full integration requires a project graph plus an evidence ledger.
13. **Do not make online platform checks mandatory for deterministic local
    gates.** GitHub/NuGet verification should have offline generated projections
    and optional online comparison, except release publishing where CI identity
    is part of the proof.
14. **Do not treat local gate logs as release provenance.** Local logs can prove
    implementation behavior. Published packages need source commit, workflow
    identity, package digest, and preferably artifact attestation evidence.
15. **Do not duplicate existing F# policy engines.** `Targets`, `Routing`,
    `PrePublish`, `Publish`, `GeneratedProduct`, `Capabilities`, `SkillTreeGen`,
    and the evidence algorithms should be reused as compiled policy modules.
16. **Do not put product contract state into feature workflow.** Product
    promises such as controls, screenshots, scenarios, performance budgets,
    accessibility, and architecture traceability deserve `ProductGraph`, not
    overloaded feature tasks.
17. **Do not treat docs screenshots or samples as passive assets.** If a
    screenshot, gallery, docs page, or generated profile proves a product claim,
    it should be a graph-bound product evidence row.
18. **Do not centralize runtime code just because metadata is integrated.** The
    value is in integrated contracts, projections, and evidence around clean
    package/module boundaries.
19. **Do not leave schema evolution as tribal knowledge.** Graph schema changes
    need deterministic migration commands and before/after fixtures.
20. **Do not let a clean rebrand hide tangled project references.** Package and
    namespace direction must be checked as a layer contract.
21. **Do not treat host behavior as test scaffolding.** Windowed, headless,
    screenshot, sample, and generated-product hosts are product contracts.
22. **Do not keep design tokens outside the product graph.** Themes, density,
    visual states, and contrast obligations are product evidence inputs.
23. **Do not make support an issue-template afterthought.** Consumer diagnostics
    should produce structured support bundles that can become scenario evidence.
24. **Do not give generated products the maintainer graph.** Generated products
    need a lean `ConsumerGraph`, not the full repository governance kernel.

## Target Architecture

The complete-break architecture has a reusable kernel layer plus two hosts:

1. **Standalone governance kernel:** small packages that other projects can
   reference without adopting FS.GG.UI policy.
2. **Old repository bootstrap host:** minimal code in this repository that can
   describe, assemble, and prove the new FS.GG.UI repository.
3. **New repository target host:** the real long-lived graph-governed system.

Do not optimize for a polished in-place `FS.Skia.UI` conversion. The old host
exists to produce a coherent new tree and a provenance record.

### Standalone Kernel Package Architecture

The standalone layer should be usable before FS.GG.UI is fully bootstrapped. It
is deliberately smaller than SpecFlow:

```text
src/RuleKernel/FS.GG.RuleKernel.fsproj
  Inference/
    Ids.fs
    Diagnostics.fs
    SourceRef.fs
    FactStore.fs
    Rule.fs
    FixedPoint.fs
    Provenance.fs
    Query.fs
  Evidence/
    EvidenceModel.fs
    EvidenceFreshness.fs
    EvidenceProjection.fs
  Graph/
    GraphHash.fs
    SchemaVersion.fs
    DeterministicJson.fs
  Rendering/
    ExplainDto.fs
    JsonDto.fs

src/Governance/FS.GG.Governance.fsproj
  ProjectGraphContracts.fs
  FeatureGraphContracts.fs
  EvidenceLedgerContracts.fs
  RouteContracts.fs
  AgentDecisionContracts.fs
  ContextPackContracts.fs

src/UI.Governance/FS.GG.UI.Governance.fsproj
  FsGgUiFacts.fs
  FsGgUiPolicyRules.fs
  FsGgUiProjectGraph.fs
  FsGgUiProductGraph.fs
  FsGgUiEvidenceRules.fs
  FsGgUiRouteQueries.fs
  FsGgUiGeneratedProductFacade.fs
```

Initial dependency direction:

```text
FS.GG.UI.Governance -> FS.GG.Governance -> FS.GG.RuleKernel
FS.GG.UI.Build      -> FS.GG.UI.Governance
build executable    -> FS.GG.UI.Build
pilot project       -> FS.GG.RuleKernel
optional pilot      -> FS.GG.Governance
```

Rules:

- `FS.GG.RuleKernel` is packable, pure, and has no FAKE, git, filesystem,
  process, Skia, UI runtime, template, or NuGet publishing dependency.
- `FS.GG.Governance` starts as preview and may be skipped if the two pilot
  projects only need the raw kernel.
- `FS.GG.UI.Governance` owns FS.GG.UI policy and may expose compatibility DTOs,
  but it must not become the public generic API.
- `FS.GG.UI.Build` keeps FAKE registration, process execution, report writing,
  package packing, template installation, and generated-product orchestration.
- Generated products continue to call a stable facade. The facade delegates to
  graph/policy APIs but does not force generated products to reference the full
  maintainer command runtime.

### Old Repository Bootstrap Architecture

The old repository should add only the extraction machinery it needs:

```text
build/Governance/SpecFlowBootstrap/
  BootstrapPlan.fs
  BrandMatrix.fs
  SourceSelection.fs
  RewritePlan.fs
  NewRepoAssembly.fs
  BootstrapEvidence.fs
  BootstrapProvenance.fs
```

This layer may reuse existing governance modules, but it should not make this
repository the final home for `ProjectGraph`, `ProductGraph`, or `FeatureGraph`.
Its outputs are staged files under `artifacts/rebrand/<new-repo-name>/` and a
machine-readable provenance record.

### New Repository Target Architecture

The new FS.GG.UI repository should contain the `SpecFlow` subsystem under its
build/governance project. It is not only a feature-workflow layer; it is the
integration layer over project policy, product contract, evidence, platform
policy, release policy, consumer graphs, schema migration, package/module
layers, host runtime contracts, design-system contracts, support bundles, and
the product runtime.

```text
build/Governance/SpecFlow/
  ProjectGraphModel.fs
  ProjectGraphJson.fs
  ProjectGraphHash.fs
  ProjectPolicyValidation.fs
  GraphMigrationPolicy.fs
  ProductGraphModel.fs
  ProductGraphJson.fs
  ProductGraphHash.fs
  ProductContractValidation.fs
  ProductProjection.fs
  ConsumerGraphModel.fs
  ConsumerGraphJson.fs
  ConsumerGraphHash.fs
  ConsumerContractValidation.fs
  CapabilityRegistry.fs
  ControlCatalogPolicy.fs
  PublicSurfacePolicy.fs
  VisualEvidencePolicy.fs
  ScenarioCorpusPolicy.fs
  PerformanceBudgetPolicy.fs
  InteractionAccessibilityPolicy.fs
  ArchitectureTracePolicy.fs
  ToolchainEnvironmentPolicy.fs
  PackageLayerPolicy.fs
  HostRuntimePolicy.fs
  DesignSystemPolicy.fs
  SupportBundlePolicy.fs
  IdentityPolicy.fs
  TargetCatalogPolicy.fs
  PlatformPolicy.fs
  CiPolicy.fs
  PackagePolicy.fs
  TemplatePolicy.fs
  DocsPolicy.fs
  ReleasePolicy.fs
  DeprecationPolicy.fs
  ArtifactProvenance.fs
  RepositoryBootstrap.fs
  MigrationPolicy.fs
  PolicyProjection.fs
  GraphModel.fs
  GraphJson.fs
  GraphSchema.fs
  GraphHash.fs
  GraphValidation.fs
  GraphMutation.fs
  GraphImport.fs
  Projection.fs
  ProjectionCheck.fs
  RequirementTrace.fs
  EvidenceIndex.fs
  RoutePlanning.fs
  GateEvidence.fs
  Approval.fs
  ResearchClaims.fs
  ContextPack.fs
  Workspaces.fs
  CommandModel.fs
  CommandRender.fs
  CommandInterpret.fs
```

The modules follow the existing build engine rule:

- pure modules compute project policy, feature plans, mutations, projections,
  product contracts, diagnostics, and reports;
- interpreter modules perform filesystem, git, process, and console IO;
- command output supports `--json`, `--plain`, and human-rich modes;
- project-policy projections cover `.github`, package metadata, package/module
  layers, template metadata, docs policy, schema migration policy, release
  policy, and generated governance docs;
- product-contract projections cover control/capability catalogs, docs images,
  scenario matrices, performance budgets, interaction/accessibility contracts,
  architecture traceability, host runtime contracts, design-system contracts,
  support-bundle policy, and environment policy;
- generated files are deterministic and do not contain wall-clock timestamps
  unless the timestamp is an evidence event explicitly committed to the graph.

## Authority Model

### Single Source

The project has one canonical policy graph:

```text
.specflow/project.graph.json
```

The product has one canonical contract graph:

```text
.specflow/product.graph.json
```

Each active feature has one canonical graph:

```text
specs/<feature-id>/feature.graph.json
```

Each generated consumer product has one lean canonical graph:

```text
.specflow/consumer.graph.json
```

The active feature pointer becomes:

```text
.specflow/current.json
```

`project.graph.json` is the only authored project-policy state file.
`product.graph.json` is the only authored product-contract state file.
`feature.graph.json` is the only authored feature-workflow state file. Any graph
`consumer.graph.json` is the only authored generated-product contract state
file inside a consumer application. Any graph may embed Markdown strings for
long-form prose, but those strings are fields in a typed schema, not
free-floating files with implied semantics.

### Generated Projections

The following become generated projections:

```text
.github/workflows/*.yml
.github/rulesets/*.json
Directory.Packages.props
.specflow/schema/migrations.generated.md
.specflow/schema/consumer-graph.schema.json
.template.config/template.json
.template.package/*.fsproj
docs/governance/*.md
docs/distribution.md
docs/governance/package-layers.md
docs/governance/host-runtime.md
docs/governance/design-system.md
docs/governance/support-bundles.md
docs/controls/catalog.md
docs/controls/*.md
docs/img/controls/manifest.generated.json
docs/testSpecs/index.generated.md
docs/architecture/traceability.generated.md
readiness/product-contract.md
readiness/scenario-corpus.md
readiness/performance-budgets.md
specs/<feature-id>/spec.md
specs/<feature-id>/plan.md
specs/<feature-id>/tasks.md
specs/<feature-id>/readiness/index.md
specs/<feature-id>/readiness/task-graph.md
specs/<feature-id>/readiness/task-graph.json
specs/<feature-id>/readiness/traceability.md
specs/<feature-id>/readiness/context/<phase>.json
specs/<feature-id>/readiness/context/<phase>.md
```

`tasks.deps.yml` is deleted as an active artifact. If a YAML dependency view is
still useful for review, it is generated as:

```text
specs/<feature-id>/readiness/task-deps.generated.yml
```

Projection files carry a small generated header where the file format permits
comments:

```text
<!-- GENERATED FROM feature.graph.json sha256:<hash>; DO NOT EDIT -->
```

Project projections cite `project.graph.json`; product projections cite
`product.graph.json` and the project-policy hash they were rendered against;
feature projections cite `feature.graph.json` plus the project-policy and
product-contract hashes they were rendered against; consumer projections cite
`consumer.graph.json` plus the project-policy and product-contract hashes they
were generated from.
`SpecFlowProjectionCheck` recomputes every projection and fails on drift.

### Evidence State

Evidence is stored as structured graph data, not inferred from arbitrary files.
Evidence may point to files, logs, images, generated outputs, and command
transcripts, but the graph row is authoritative.

Example:

```json
{
  "id": "EV-0027",
  "kind": "gate-run",
  "gate": "Dev",
  "command": "./fake.sh build -t Dev",
  "status": "pass",
  "authoritative": true,
  "started_at_utc": "2026-06-13T10:22:11Z",
  "completed_at_utc": "2026-06-13T10:24:03Z",
  "log_paths": [
    "specs/116-paint-cache-damage-rects/readiness/logs/test.txt"
  ],
  "covers": {
    "requirements": ["FR-014", "SC-007"],
    "tasks": ["T026"]
  }
}
```

The file can be missing only if the evidence row says `status = missing` or
`status = external`. A pass row pointing at a missing file is a graph error.

## Graph Schema

Use strict JSON with deterministic field ordering on write. F# owns the schema;
JSON Schema may be generated for editor support, but the compiled F# validator is
the authority.

### Top-Level Shape

Project policy has its own top-level graph:

```fsharp
type ProjectGraph =
  { SchemaVersion: SchemaVersion
    GraphId: string
    Identity: ProjectIdentity
    SchemaMigrations: GraphSchemaMigrationPolicy
    Repository: RepositoryPolicy
    Targets: TargetCatalogPolicy
    Routing: RoutingPolicy
    Packages: PackagePolicy
    PackageLayers: PackageLayerContract list
    Templates: TemplatePolicy
    Docs: DocsPolicy
    Skills: SkillPolicy
    Ci: CiPolicy
    Release: ReleasePolicy
    Provenance: ProvenancePolicy
    Migration: MigrationPolicy option
    Projections: ProjectProjectionState
    Lifecycle: ProjectLifecycleState }
```

Product contract has its own graph:

```fsharp
type ProductGraph =
  { SchemaVersion: SchemaVersion
    GraphId: string
    Project: ProjectPolicyRef
    Capabilities: CapabilityContract list
    Controls: ControlContract list
    PublicSurfaces: PublicSurfaceContract list
    VisualEvidence: VisualEvidenceContract list
    Scenarios: ScenarioContract list
    PerformanceBudgets: PerformanceBudgetContract list
    InteractionContracts: InteractionContract list
    AccessibilityContracts: AccessibilityContract list
    ArchitectureTrace: ArchitectureTraceContract list
    Hosts: HostRuntimeContract list
    DesignSystem: DesignSystemContract option
    SupportBundles: SupportBundleContract list
    Toolchain: ToolchainEnvironmentContract
    Projections: ProductProjectionState
    Lifecycle: ProductLifecycleState }
```

Feature state references the project policy and product contract it was authored
and validated against:

```fsharp
type FeatureGraph =
  { SchemaVersion: SchemaVersion
    GraphId: string
    Project: ProjectPolicyRef
    Product: ProductContractRef
    Feature: FeatureHeader
    Requirements: Requirement list
    Scenarios: Scenario list
    Decisions: Decision list
    ResearchClaims: ResearchClaim list
    Impact: ImpactModel
    Tasks: TaskNode list
    Evidence: EvidenceItem list
    Approvals: Approval list
    Route: RouteState
    Governance: GovernanceState
    Release: FeatureReleaseImpact option
    Projections: ProjectionState
    ContextPacks: ContextPackSpec list
    Lifecycle: LifecycleState }

type ProjectPolicyRef =
  { ProjectGraphPath: string
    ProjectGraphHash: string
    PolicyVersion: string
    IdentityId: string }

type ProductContractRef =
  { ProductGraphPath: string
    ProductGraphHash: string
    ContractVersion: string
    ProductId: string }
```

Generated consumer products use a smaller graph:

```fsharp
type ConsumerGraph =
  { SchemaVersion: SchemaVersion
    GraphId: string
    Project: ProjectPolicyRef
    Product: ProductContractRef
    TemplateProfile: string
    PackageMatrix: ConsumerPackageMatrix
    EnabledCapabilities: string list
    DurableFiles: string list
    ReplaceableFiles: string list
    ValidationCommands: string list
    UpgradeState: ConsumerUpgradeState
    SupportBundlePolicy: ConsumerSupportBundlePolicy
    Projections: ConsumerProjectionState
    Lifecycle: ConsumerLifecycleState }
```

Rules:

- A feature graph with a stale `ProjectGraphHash` can still be inspected, but
  cannot satisfy readiness or release gates.
- A feature graph with a stale `ProductGraphHash` can still be inspected, but
  cannot satisfy product-contract readiness or release gates.
- A consumer graph with a stale `ProjectGraphHash` or `ProductGraphHash` can
  still run advisory diagnostics, but cannot satisfy generated-product health or
  upgrade readiness until it is refreshed or explicitly pinned by policy.
- Project policy changes route as governance impact and invalidate any feature
  approvals whose scope depends on changed policy fields.
- Product contract changes route as product impact and invalidate any feature
  approvals/evidence whose scope depends on changed product fields.
- Feature graphs refer to package, template, target, docs, and workflow policy
  by stable project-policy IDs, not by copied strings.
- Feature graphs refer to controls, capabilities, scenarios, visual evidence,
  performance budgets, and interaction/accessibility contracts by stable
  product-contract IDs.

### Feature Header

```fsharp
type FeatureHeader =
  { Id: string
    Slug: string
    Title: string
    BranchName: string option
    Workspace: WorkspaceState
    CreatedAtUtc: DateTimeOffset
    Status: FeatureStatus
    SourcePrompt: string option
    Owners: string list }

type FeatureStatus =
  | Draft
  | Planned
  | Tasked
  | Implementing
  | ReadyForReview
  | Merged
  | Archived
```

`CreatedAtUtc` is allowed here because feature creation is a real event. It is
not regenerated.

### Requirements

```fsharp
type RequirementKind =
  | Functional
  | SuccessCriterion
  | NonFunctional
  | Constraint
  | UnsupportedScope

type Requirement =
  { Id: string
    Kind: RequirementKind
    Priority: int option
    Text: string
    Rationale: string option
    Acceptance: AcceptanceCheck list
    Parent: string option
    Tags: string list
    Status: RequirementStatus }

type AcceptanceCheck =
  { Id: string
    Text: string
    EvidenceKinds: EvidenceKind list
    Required: bool }
```

Rules:

- Requirement IDs are stable and unique.
- `FR-###` and `SC-###` remain acceptable display IDs, but the graph does not
  infer meaning from prose alone.
- Unsupported scope is represented explicitly, not buried in plan prose.
- Each buildable requirement must link to at least one task or be explicitly
  deferred.

### Decisions

```fsharp
type Decision =
  { Id: string
    Title: string
    Context: string
    Decision: string
    Consequences: string list
    Alternatives: Alternative list
    Supersedes: string list
    AppliesTo: string list }
```

Design decisions replace scattered plan paragraphs when a choice matters for
future maintenance.

### Research Claims

```fsharp
type ResearchClaim =
  { Id: string
    Claim: string
    Source: ResearchSource
    RetrievedAtUtc: DateTimeOffset option
    CheckedVersion: string option
    Confidence: ClaimConfidence
    UsedBy: string list }

type ResearchSource =
  | Url of string
  | LocalPath of string
  | CommandOutput of evidenceId: string
```

Rules:

- Current-version claims need `Url`, `RetrievedAtUtc`, and `CheckedVersion`.
- Network lookups are never part of deterministic validation gates.
- Research claims can be stale; the validator reports staleness by policy, not
  by silently refreshing the internet.

### Impact Model

```fsharp
type ImpactModel =
  { RuntimePackages: string list
    PublicSurfaces: SurfaceImpact list
    Product: ProductImpact
    Packages: PackageImpact list
    Templates: TemplateImpact
    GeneratedProducts: GeneratedProductImpact
    Governance: GovernanceImpact
    Platform: PlatformImpact
    Ci: CiImpact
    Docs: DocsImpact
    Release: ReleaseImpact
    Provenance: ProvenanceImpact
    Risk: RiskLevel
    DeclaredChangedPaths: string list
    ExpectedRoute: ExpectedRoute option }
```

The impact model makes route expectations visible before implementation.

Rules:

- A public `.fsi` change must be declared before implementation tasks can be
  completed.
- A control, capability, docs image, scenario, sample, performance budget,
  interaction, accessibility, architecture, or environment change must declare
  product impact.
- A generated-template change must declare generated-product impact.
- A package ID, version, metadata, Source Link, signing, or publish-policy
  change must declare package and release impact.
- A workflow, required-check, ruleset, or permissions change must declare CI or
  platform impact.
- A governance/build-path change must declare governance impact.
- `RoutePlanning` compares declared impact to actual git diff classification.

### Tasks

```fsharp
type TaskNode =
  { Id: string
    Title: string
    Body: string option
    Phase: string
    Story: string option
    Status: TaskStatus
    Dependencies: string list
    Parallel: bool
    SkillIds: string list
    Owns: EvidenceOwnership list
    ExpectedChanges: ExpectedChange list
    Covers: TraceLink list
    Completion: CompletionProof option }

type TaskStatus =
  | Pending
  | InProgress
  | Done
  | Skipped of reason: string
  | Failed of reason: string
  | Deferred of reason: string
  | SyntheticDone of reason: string

type CompletionProof =
  { CompletedAtUtc: DateTimeOffset
    EvidenceIds: string list
    CommitIds: string list
    Notes: string option }
```

Rules:

- `Done` requires at least one evidence row or an explicit `no-evidence-required`
  reason for administrative tasks.
- `SyntheticDone` is distinct from `Done`; synthetic taint still propagates.
- Task completion cannot be represented by editing `[X]` in Markdown.
- A task that owns graph validation or audit evidence must cite the matching
  evidence row.
- A task that declares `ExpectedChanges` but has no matching git diff or commit
  evidence is suspicious and reported.

### Evidence

```fsharp
type EvidenceItem =
  { Id: string
    Scope: EvidenceScope
    Kind: EvidenceKind
    Status: EvidenceStatus
    Authoritative: bool
    Synthetic: SyntheticClass option
    ProducedBy: Producer
    Paths: string list
    Command: CommandEvidence option
    Ci: CiEvidence option
    Artifact: ArtifactEvidence option
    Gate: TargetId option
    Environment: EvidenceEnvironment option
    AppliesToGraphHash: string option
    AppliesToProjectGraphHash: string option
    AppliesToProductGraphHash: string option
    AppliesToCommit: string option
    Covers: TraceLink list
    CreatedAtUtc: DateTimeOffset option
    Notes: string option }

type EvidenceKind =
  | GateRun
  | TestRun
  | FsiTranscript
  | CiRun
  | RulesetSnapshot
  | WorkflowPolicyCheck
  | SurfaceBaseline
  | GoldenFile
  | GeneratedProduct
  | PackagePack
  | PackagePublish
  | PackageDeprecation
  | ArtifactAttestation
  | SourceLinkCheck
  | DocsBuild
  | TemplatePack
  | TemplateInstantiate
  | Screenshot
  | Research
  | ApprovalEvidence
  | ManualObservation

type EvidenceStatus =
  | Pass
  | Fail
  | Missing
  | NotApplicable
  | Deferred
  | External
```

Rules:

- Evidence rows are typed.
- Evidence files are attachments, not semantic sources.
- `Authoritative = true` means the row can satisfy a requirement.
- Non-authoritative aggregate logs can be stored but cannot satisfy a blocking
  acceptance check unless a policy explicitly allows it.
- Package and release evidence must include package ID, version, digest, source
  commit, and builder/workflow identity.
- Visual, scenario, performance, interaction, and accessibility evidence must
  cite product-contract IDs and the product graph hash.
- CI evidence must name the workflow, job, run attempt, commit, and status-check
  name if it is used to satisfy a required platform rule.
- Attestation evidence must identify the artifact subject and verification
  policy. A link to an attestation is not sufficient unless the verification row
  passes or the policy explicitly allows deferred verification.

### Governance State

```fsharp
type GovernanceState =
  { ProjectPolicyHash: string
    ProductContractHash: string
    TargetCatalog: TargetCatalogSnapshot
    RoutingPolicy: RoutingPolicySnapshot
    ProductContract: ProductContractSnapshot option
    PlatformPolicy: PlatformPolicySnapshot option
    PackagePolicy: PackagePolicySnapshot option
    TemplatePolicy: TemplatePolicySnapshot option
    DocsPolicy: DocsPolicySnapshot option
    ReleasePolicy: ReleasePolicySnapshot option
    ProvenancePolicy: ProvenancePolicySnapshot option
    Drift: GovernanceDrift list }
```

Rules:

- Snapshots record the policy IDs and hashes used when the feature route,
  projections, approvals, and evidence were produced.
- A target, package, template, workflow, docs, or release-policy change can
  invalidate stale feature evidence without rewriting feature history.
- A control, capability, scenario, visual evidence, performance budget,
  interaction/accessibility, architecture, or environment change can invalidate
  stale feature evidence without rewriting feature history.
- Governance drift is blocking for release and merge readiness, but can be
  reported as advisory during early feature authoring.

### Approvals

```fsharp
type Approval =
  { Id: string
    Scope: ApprovalScope
    Decision: ApprovalDecision
    Reviewer: string
    ApprovedAtUtc: DateTimeOffset
    GraphHash: string
    EvidenceIds: string list
    Notes: string option }
```

Rules:

- Approval applies to a graph hash. If the graph changes, the approval becomes
  stale unless its scope is explicitly still valid.
- Approval can be required for high-risk or public-surface features.
- AI review is advisory unless represented as an approval by a human or policy.

### Route State

```fsharp
type RouteState =
  { Declared: ExpectedRoute option
    LastActual: ActualRoute option
    RequiredGates: string list
    GateEvidence: string list
    Drift: RouteDrift list }
```

`RouteState` makes validation obligations visible in the graph. It does not
replace the actual `Route` target; it records expectations and actual evidence.

## Command Surface

Create an internal governance launcher:

```bash
./specflow <command> [args]
```

No `speckit` aliases are required. Keep `specflow` as the precise name for the
feature/project/product graph engine. If the new FS.GG.UI repository wants a
branded convenience command later, it can add a thin `./gg` or `./fs-gg` wrapper
for project/product commands, but the first implementation should not overbrand
the internal workflow tool.

In this repository, command work should be limited to bootstrap commands needed
to assemble the new tree. The full command surface belongs in FS.GG.UI.

### Feature Lifecycle

```bash
./specflow new "paint cache damage rects" --id 116-paint-cache-damage-rects
./specflow activate specs/116-paint-cache-damage-rects
./specflow status
./specflow archive 116-paint-cache-damage-rects
```

`new` creates `feature.graph.json` and projections. It does not call upstream
Spec Kit.

### Graph Editing

The first implementation can support structured commands for the common graph
mutations:

```bash
./specflow requirement add FR-001 --kind functional --text "..."
./specflow requirement defer FR-008 --reason "..."
./specflow decision add DEC-001 --title "..."
./specflow task add T001 --phase setup --covers FR-001 --title "..."
./specflow task depend T009 T008
./specflow task complete T009 --evidence EV-004 --commit HEAD
./specflow evidence add-gate EV-026 --gate Dev --status pass --log readiness/logs/test.txt
./specflow approval add plan-reviewed --scope plan --reviewer <name>
```

Direct graph edits are allowed for bulk work, but the validator is strict and
the projection check catches drift.

### Schema Migration

Schema migration commands operate on project, product, feature, and consumer
graphs:

```bash
./specflow graph schema status --json
./specflow graph migrate --path .specflow/project.graph.json --to current
./specflow graph migrate --path .specflow/product.graph.json --to current
./specflow graph migrate --path specs/123-example/feature.graph.json --to current
./specflow consumer migrate --to current
```

Rules:

- Migration commands are explicit writes.
- Readiness checks do not silently reinterpret unsupported schema versions.
- Each migration has before/after fixtures and projection-regeneration tests.
- Unsupported versions fail with the required migration path and the newest
  supported version.

### Project Policy

Project-policy commands operate on `.specflow/project.graph.json`:

```bash
./specflow project status --json
./specflow project validate --json
./specflow project render
./specflow project check
./specflow project identity set --name "<new name>" --package-prefix "<prefix>"
./specflow project package add <package-id> --project src/...fsproj
./specflow project layer add <id> --package <package-id>
./specflow project layer check
./specflow project template set --short-name <name> --identity <identity>
./specflow project ci check
./specflow project release plan --json
./specflow project bootstrap-repo --output artifacts/rebrand/<new-repo-name>
```

Rules:

- `project validate` is pure and offline.
- `project render` writes generated/checkable project-policy projections.
- `project check` recomputes projections and fails on drift.
- `project ci check` validates workflow YAML and generated ruleset projections
  offline; online comparison is a separate advisory or release-only mode.
- `project release plan` does not publish. It explains the package matrix,
  publish/skip decisions, required provenance, and missing release evidence.

### Product Contract

Product-contract commands operate on `.specflow/product.graph.json`:

```bash
./specflow product status --json
./specflow product validate --json
./specflow product render
./specflow product check
./specflow product capability add <id> --package <package-id>
./specflow product control add <id> --capability <id> --docs docs/controls/<id>.md
./specflow product scenario add <id> --sample samples/<name>
./specflow product visual add <id> --subject <control-id> --scenario <scenario-id>
./specflow product budget add <id> --scenario <scenario-id> --metric <metric>
./specflow product host add <id> --kind headless
./specflow product design-system check
./specflow product support-bundle policy
./specflow product trace architecture --subsystem <name>
./specflow product environment set --visual <id>
```

Rules:

- `product validate` is pure and offline.
- `product render` writes generated/checkable product-contract projections.
- `product check` recomputes product projections and fails on drift.
- Product commands do not mark feature tasks done. Feature task completion still
  requires feature evidence rows.
- Product contract rows can be imported during bootstrap, but after cutover
  feature work mutates product state deliberately and routes as product impact.

### Consumer Product

Consumer commands operate inside a generated product and use the lean
`.specflow/consumer.graph.json`:

```bash
./specflow consumer status --json
./specflow consumer validate --json
./specflow consumer health
./specflow consumer profile check
./specflow consumer packages check
./specflow consumer upgrade plan --to <version>
./specflow consumer support-bundle --output artifacts/support/<id>.zip
```

Rules:

- Consumer commands do not require the FS.GG.UI source checkout.
- Health checks validate package pins, selected template profile, enabled
  capabilities, durable/replaceable file state, validation commands, and
  project/product graph hashes.
- Upgrade plans compare the current consumer graph to the target package and
  template policy.
- Support bundles follow graph-owned redaction policy and can be imported into
  maintainer evidence as issue-reproduction input.

### Projection

```bash
./specflow project
./specflow project --check
./specflow graph validate
./specflow graph explain --json
./specflow graph mermaid
```

`project --check` is the replacement for stale Markdown/YAML readiness checks.

### Context Packs

```bash
./specflow context --phase specify --json
./specflow context --phase plan --json
./specflow context --phase tasks --json
./specflow context --phase implement --json
./specflow context --task T012 --json
```

Context packs include:

- graph summary;
- required files to read;
- files not to read unless needed;
- relevant skills;
- route expectations;
- active blockers;
- allowed mutations;
- required evidence shape;
- token estimate if available.

This replaces long agent prompt prose as the primary operational surface.

### Health And Traceability

```bash
./specflow doctor --json
./specflow trace --json
./specflow route-plan --json
./specflow route-actual --json
./specflow evidence status --json
./specflow approvals status --json
```

These commands are read-only except where explicitly named otherwise.

### Workspaces

```bash
./specflow workspace create --layout sibling
./specflow workspace create --layout nested
./specflow workspace status
./specflow workspace dispose
```

Workspaces are graph-owned. The workspace state records:

- git worktree path;
- branch;
- cache namespace;
- FAKE state namespace or confirmed shared-state policy;
- active feature graph path.

## Agent Model

Delete the active `speckit-*` skills and start with one generated context-pack
entry point:

```text
.agents/skills/specflow/SKILL.md
```

The skill stays thin:

1. Run `./specflow context --phase ... --json` or
   `./specflow context --task ... --json`.
2. Read only the files listed in the context pack unless local discovery shows a
   clear need.
3. Mutate the graph through `./specflow` commands or direct graph edits.
4. Run `./specflow project`.
5. Do not hand-edit generated projections.

The skill prose should not duplicate the governance policy. The graph and
context-pack generator are the policy source. Phase-specific skills such as
`specflow-plan` or `specflow-implement` can be generated later as aliases if
they measurably improve ergonomics, but they are not required for the complete
break.

`.claude` mirroring can be retained if the new repo still wants Claude support,
but it is generated from the single `specflow` skill or its generated aliases.

## Build Targets

Add or replace targets in `Targets.fs`:

```fsharp
| SpecFlowProjectCheck
| SpecFlowProductCheck
| SpecFlowConsumerCheck
| SpecFlowSchemaMigrationCheck
| SpecFlowGraphCheck
| SpecFlowProjectionCheck
| SpecFlowPolicyProjectionCheck
| SpecFlowProductProjectionCheck
| SpecFlowLayerContractCheck
| SpecFlowControlCatalogCheck
| SpecFlowHostRuntimeCheck
| SpecFlowDesignSystemCheck
| SpecFlowSupportBundleCheck
| SpecFlowVisualEvidenceCheck
| SpecFlowScenarioCorpusCheck
| SpecFlowPerformanceBudgetCheck
| SpecFlowInteractionContractCheck
| SpecFlowArchitectureTraceCheck
| SpecFlowEnvironmentPolicyCheck
| SpecFlowPlatformPolicyCheck
| SpecFlowPackagePolicyCheck
| SpecFlowTemplatePolicyCheck
| SpecFlowReleasePolicyCheck
| SpecFlowProvenanceCheck
| SpecFlowTraceCheck
| SpecFlowContextCheck
| SpecFlowAudit
| SpecFlowBootstrapCheck
```

Recommended mapping:

| Old target | New target |
|---|---|
| `EvidenceGraph` | `SpecFlowGraphCheck` |
| `EvidenceAudit` | `SpecFlowAudit` |
| `GeneratedGuidanceCheck` | `SpecFlowContextCheck` plus policy/projection checks |
| `GeneratedProductCheck` | `SpecFlowTemplatePolicyCheck` plus generated-product evidence rows |
| `TemplateCheck` | `SpecFlowTemplatePolicyCheck` |
| `CatalogDocsGen` / control docs checks | `SpecFlowControlCatalogCheck` plus `SpecFlowProductProjectionCheck` |
| `ControlsDocCoverage` | `SpecFlowControlCatalogCheck` |
| `ContrastGate` / design-token visual checks | `SpecFlowVisualEvidenceCheck` plus `SpecFlowInteractionContractCheck` |
| performance corpus checks | `SpecFlowPerformanceBudgetCheck` |
| `PrePublishCheck` / publish planning targets | `SpecFlowPackagePolicyCheck` plus `SpecFlowReleasePolicyCheck` |
| `PhaseHookParityCheck` | Delete or replace with `SpecFlowContextCheck` |
| `SkillSyncCheck` | Keep only if `.claude` mirroring remains |
| `TargetMetadataDrift` | Fold into `SpecFlowPolicyProjectionCheck` or keep as the target-catalog subcheck |

The old targets may be removed once the new graph targets exist. No aliasing is
required.

Target policy:

- `SpecFlowProjectCheck` validates `.specflow/project.graph.json` and its hash.
- `SpecFlowProductCheck` validates `.specflow/product.graph.json`, its project
  hash reference, and its own hash.
- `SpecFlowConsumerCheck` validates `.specflow/consumer.graph.json`,
  package/profile/capability consistency, durable/replaceable file policy, and
  upgrade/support-bundle policy when running inside a generated product.
- `SpecFlowSchemaMigrationCheck` validates supported graph schema versions,
  migration fixtures, and generated schema/migration projections.
- `SpecFlowPolicyProjectionCheck` validates generated/checkable project-policy
  projections: target metadata, `.github` policy files, package metadata,
  package/module layers, template metadata, docs policy, skill trees, schema
  migration policy, and governance docs.
- `SpecFlowProductProjectionCheck` validates generated/checkable product
  projections: control catalogs, docs-page indexes, image manifests, scenario
  matrices, performance-budget reports, interaction/accessibility tables,
  architecture traceability, host runtime, design-system, support-bundle, and
  environment reports.
- `SpecFlowLayerContractCheck` validates project references, namespace
  ownership, public/internal boundaries, governance/product dependency
  direction, and test-only references.
- `SpecFlowControlCatalogCheck` validates capability, control, public-surface,
  docs, sample, template, and test bindings.
- `SpecFlowHostRuntimeCheck` validates windowed, headless, screenshot, sample,
  and generated-product host contracts and binds evidence to host IDs.
- `SpecFlowDesignSystemCheck` validates token source, generated token modules,
  themes, density modes, visual states, contrast requirements, and evidence
  bindings.
- `SpecFlowSupportBundleCheck` validates support-bundle collection fields,
  redaction policy, output format, and importability into evidence rows.
- `SpecFlowVisualEvidenceCheck` validates visual evidence contracts and attached
  screenshot/fidelity/doc-image evidence rows.
- `SpecFlowScenarioCorpusCheck` validates scenario rows against docs test specs,
  samples, generated profiles, and smoke evidence.
- `SpecFlowPerformanceBudgetCheck` validates product-owned performance budgets
  and budget evidence freshness.
- `SpecFlowInteractionContractCheck` validates focus, pointer, keyboard, visual
  state, contrast, diagnostics, and accessibility contract coverage.
- `SpecFlowArchitectureTraceCheck` validates source-to-architecture-to-ADR
  traceability.
- `SpecFlowEnvironmentPolicyCheck` validates SDK, target-framework, native,
  visual-evidence, generated-product, and docs environment assumptions.
- `SpecFlowPlatformPolicyCheck` is offline by default. It checks workflow YAML,
  permissions, required status-check names, reusable workflow refs, and generated
  ruleset JSON. Online GitHub comparison is opt-in or release-only.
- `SpecFlowReleasePolicyCheck` validates the package matrix, Trusted Publishing
  declaration, publish plan, deprecation plan, required environment, and required
  evidence.
- `SpecFlowProvenanceCheck` validates package digest rows, Source Link/signing
  policy, attestation policy, and CI builder identity when release evidence is
  present.
- `SpecFlowBootstrapCheck` validates a staged new repository tree without
  treating the old repository as the long-term active destination.

## Validation Invariants

### Project Policy Validity

- `project.graph.json` schema version is supported.
- Project graph schema migration policy names current, supported, and
  unsupported versions and has fixtures for each supported migration.
- Project identity, repository identity, package prefix, root namespace, template
  identity, docs URL, and migration map are internally consistent.
- Every package policy row maps to exactly one packable project or declared
  virtual/template package.
- Every packable project has a package policy row unless explicitly private.
- Every packable project belongs to a package/module layer or is explicitly
  exempt.
- Project references, namespace ownership, and test-only references satisfy
  package/module layer policy.
- Target catalog IDs match the compiled `Targets` union and generated target
  metadata.
- Routing policy references only existing targets and policy IDs.
- Skill/context-pack policy references only active skills and supported phases.
- Old brand/package/repo names are rejected in active project policy except in
  migration/deprecation rows.

### Product Contract Validity

- `product.graph.json` schema version is supported.
- Product graph references the current project graph hash for merge/release
  readiness.
- Every capability maps to a package row or explicit non-runtime declaration.
- Every control maps to a capability, docs page, public surface, test set, and
  optional screenshot/gallery row.
- Every public `.fsi` surface maps to a package row and at least one product
  subject or explicit infrastructure/private declaration.
- Every docs control page maps to exactly one control row or an explicit
  historical/migration page.
- Every required docs/control image maps to a visual evidence contract.
- Every scenario maps to a sample, test spec, generated profile, or explicit
  manual scenario declaration.
- Every performance budget maps to a scenario and a metric source.
- Every interaction/accessibility contract maps to an existing control,
  capability, or scenario.
- Every architecture trace row maps source paths to architecture docs and ADRs
  or records an explicit no-ADR-needed decision.
- Every host runtime contract maps to scenarios, template profiles, samples, or
  generated-product validation rows that use it.
- Design-system contract rows map token sources, generated modules, themes,
  density modes, visual states, and contrast requirements to tests or explicit
  deferrals.
- Support-bundle rows declare collected fields, redaction policy, output format,
  and import rules for evidence.
- Toolchain/environment rows cover SDK, target frameworks, package sources,
  visual evidence environment, docs environment, and generated-product
  environment.

### Consumer Contract Validity

- `consumer.graph.json` schema version is supported.
- Consumer graph references supported project and product graph hashes.
- Selected template profile exists in the current template policy.
- Package pins match a declared package matrix or an explicit pinned legacy
  compatibility policy.
- Enabled capabilities exist in ProductGraph and map to available packages.
- Durable and replaceable file lists are disjoint and cover generated files that
  upgrade policy may touch.
- Validation commands exist, are consumer-safe, and do not require the FS.GG.UI
  source checkout.
- Upgrade state names the current template/package slice and the target slice
  when an upgrade is planned.
- Support-bundle policy satisfies the redaction and field-collection contract.

### Graph Validity

- Graph schema version is supported.
- Feature graph references the current project graph hash for merge/release
  readiness.
- Feature graph references the current product graph hash for product-contract
  readiness.
- Every ID is unique within its namespace.
- Every dependency references an existing task.
- The task graph is acyclic.
- Every task has a phase.
- Every `Done` task has completion proof.
- Every completion proof references existing evidence.
- Every evidence path exists unless explicitly `Missing`, `Deferred`,
  `NotApplicable`, or `External`.
- Every buildable requirement is covered by a task or explicit deferral.
- Every required acceptance check is covered by authoritative evidence or
  explicit deferral.
- Synthetic completion propagates through real dependencies.
- Approval graph hashes are current or reported stale.

### Projection Validity

- Every generated projection matches the current graph.
- No projection contains stale graph hash.
- Project-policy projections match `project.graph.json`.
- Product-contract projections match `product.graph.json`.
- Workflow, ruleset, package, template, docs, and skill projections are checked
  together so identity cannot drift across surfaces.
- Schema migration, package/module layer, consumer graph schema, support-bundle,
  and generated-product projections are checked together so consumer contracts
  cannot drift from project policy.
- Control docs, docs images, scenario matrices, sample references, performance
  budgets, interaction/accessibility reports, and architecture trace reports are
  checked together so product claims cannot drift across surfaces.
- Host runtime and design-system projections are checked with scenario, visual,
  performance, interaction, and accessibility projections because they determine
  evidence meaning.
- No hand-authored `tasks.deps.yml` exists in active feature directories.
- No active `spec.md`, `plan.md`, or `tasks.md` lacks the generated header.

### Platform Policy Validity

- Every required status check maps to a local target or a declared CI-only check.
- Workflow `permissions:` blocks are explicit and least-privilege by policy.
- Jobs that do not publish or attest artifacts do not request `id-token: write`
  or `attestations: write`.
- Release jobs use the declared environment.
- Cross-repository reusable workflows follow the pinning policy.
- Generated ruleset files match the expected branch/tag/protection policy.
- Online GitHub comparisons never run as mandatory local checks, but release
  mode can require CI-recorded platform evidence.

### Release And Provenance Validity

- Package matrix, central package pins, template pins, and packable projects
  agree.
- Package metadata points to the current repository and docs identity.
- Source Link, signing, Trusted Publishing, attestation, and SBOM requirements
  are either satisfied by authoritative evidence or explicitly deferred by
  policy.
- Publish plans are idempotent and distinguish `Push`, `SkipExisting`, and
  `Block`.
- Package deprecation rows map old IDs to replacement IDs and migration docs.
- Release evidence binds package ID, version, digest, source commit, workflow
  identity, and builder identity.
- Local gate evidence cannot satisfy CI provenance requirements.

### Product Evidence Validity

- Visual evidence binds subject ID, scenario ID, environment ID, expected image,
  actual image or external reference, tolerance, and product graph hash.
- Performance evidence binds scenario ID, metric ID, measured value, budget,
  environment, and product graph hash.
- Interaction/accessibility evidence binds control/capability/scenario ID,
  input mode or accessibility policy, test evidence, and product graph hash.
- Scenario evidence binds scenario ID to sample, generated profile, docs test
  spec, smoke run, or manual observation as policy allows.
- Architecture trace evidence binds source paths, architecture docs, ADRs, and
  feature IDs.
- Toolchain/environment evidence binds SDK, package source, CI image, native
  dependency, visual environment, or docs environment to the evidence row.
- Product evidence with stale product graph hash cannot satisfy readiness.

### Traceability

- Requirement -> task -> evidence paths are complete.
- Tests and goldens can be linked to requirements.
- Orphan evidence is reported.
- Orphan tests are advisory unless a policy makes them blocking.
- Tasks marked done without implementation/evidence are blockers.

### Route Consistency

- Declared impact selects an expected route before implementation.
- Actual working-tree diff route is captured after implementation.
- Declared and actual route mismatch is blocking unless explicitly waived.
- Route-required gates have corresponding evidence rows.
- Gate evidence rows reference logs.
- Logs for authoritative pass evidence exist.

### Research Honesty

- Current-version claims have URL, retrieved date, and checked version.
- Local source claims cite local paths or evidence IDs.
- Network-dependent claims cannot be required by deterministic gates.

### Context-Pack Validity

- Each phase has a context pack.
- Context packs list graph, projections, relevant source files, skills, and
  forbidden/generated files.
- Large artifacts are flagged.
- Context packs are deterministic over the graph and repository snapshot.

## Projection Design

### Project Policy Projections

Generated or checked from `ProjectGraph`:

- `.github/workflows/ci.yml`
- `.github/workflows/publish.yml`
- `.github/rulesets/*.json`
- `Directory.Packages.props`
- `.specflow/schema/migrations.generated.md`
- `.specflow/schema/consumer-graph.schema.json`
- `.template.config/template.json`
- `.template.package/*.fsproj`
- `template/capabilities.generated.yml` or replacement graph-owned capability
  projection
- `.agents/skills/specflow-*/SKILL.md`
- `docs/governance/index.md`
- `docs/governance/routing-and-gates.md`
- `docs/governance/package-layers.md`
- `docs/governance/platform-policy.md`
- `docs/governance/release-policy.md`
- `docs/distribution.md`
- `readiness/project-policy.md`

Rules:

- Some files can stay hand-authored during transition, but every project-policy
  surface is either generated from the graph or checked against it.
- Workflow and ruleset projections are deterministic offline artifacts. Actual
  GitHub settings are compared only in optional online/advisory or release
  modes.
- Package/template/docs projections are rendered from the brand and package
  matrix so a rebrand cannot update one surface without the others.
- Human narrative belongs in rationale sections; policy facts belong in graph
  fields.

### Product Contract Projections

Generated or checked from `ProductGraph`:

- `docs/controls/catalog.md`
- `docs/controls/*.md`
- `docs/img/controls/manifest.generated.json`
- `docs/testSpecs/index.generated.md`
- `docs/architecture/traceability.generated.md`
- `docs/governance/product-contract.md`
- `docs/governance/host-runtime.md`
- `docs/governance/design-system.md`
- `docs/governance/support-bundles.md`
- `readiness/product-contract.md`
- `readiness/scenario-corpus.md`
- `readiness/visual-evidence.md`
- `readiness/performance-budgets.md`
- `readiness/interaction-accessibility.md`
- `readiness/host-runtime.md`
- `readiness/design-system.md`
- `readiness/support-bundles.md`
- `readiness/environment-policy.md`
- `template/capabilities.generated.yml` or the replacement generated capability
  projection

Rules:

- Product contract projections cite both `product.graph.json` and the
  project-policy hash used to interpret package/template/docs identity.
- Control docs pages can remain hand-authored during transition, but their
  frontmatter, product IDs, screenshot references, API-surface references,
  scenario references, and required evidence links are checked against the graph.
- Docs images are not just assets. Required images appear in the visual evidence
  projection with subject, scenario, environment, and tolerance metadata.
- Scenario projections connect docs test specs, samples, generated profiles,
  smoke evidence, visual evidence, and performance budgets.
- Architecture trace projections connect ADRs, source path groups, architecture
  pages, and feature IDs.
- Host runtime projections connect template profiles, scenario evidence,
  generated-product validation, screenshot modes, and native/runtime
  assumptions.
- Design-system projections connect token sources, generated token modules,
  themes, density modes, visual states, contrast obligations, and control
  evidence.
- Support-bundle projections describe the consumer diagnostic command,
  redaction policy, collected fields, and evidence-import path.

### Consumer Projections

Generated or checked from `ConsumerGraph` inside a generated product:

- `.specflow/consumer.contract.json`
- `.specflow/consumer-health.md`
- `.specflow/upgrade-plan.md`
- `.specflow/support-bundle-policy.md`

Rules:

- Consumer projections cite `consumer.graph.json`, `project.graph.json`, and
  `product.graph.json` hashes.
- Consumer health projections are advisory until imported as maintainer
  evidence, but stale package/profile/capability state is always reported.
- Upgrade projections classify replaceable generated files, durable user files,
  and manual migration notes.

### `spec.md`

Generated from:

- `FeatureHeader`
- `Requirements`
- `Scenarios`
- `UnsupportedScope`
- selected `ResearchClaims`

It should read like the current feature specs, but the graph owns all IDs and
requirement text.

### `plan.md`

Generated from:

- `ImpactModel`
- `Decisions`
- `RouteState.Declared`
- `ResearchClaims`
- planned evidence obligations
- implementation stages derived from tasks

The current "Repository Governance Decisions" section should become graph data.
The projection can still render it for review.

### `tasks.md`

Generated from `TaskNode list`.

Rules:

- Checkbox status comes from `TaskStatus`.
- Dependencies are rendered inline for humans only.
- Skill lists are rendered from graph `SkillIds`.
- Synthetic disclosures are generated from synthetic evidence rows.
- The file is never edited by agents.

### Readiness

Replace ad-hoc readiness scattering with an index:

```text
readiness/index.md
readiness/evidence.json
readiness/traceability.md
readiness/task-graph.md
readiness/context/*.json
readiness/context/*.md
readiness/logs/**
readiness/attachments/**
```

Specialized readiness files can exist when they carry meaningful narrative, but
they must be referenced by an `EvidenceItem`.

## Clean Repository Assembly Plan

Because backward compatibility is irrelevant, use selective assembly rather than
in-place migration. The new repository is created from chosen source, docs,
tests, template, and governance slices. Old runtime/process surfaces stay behind
unless they are deliberately rewritten into the new graph model.

### Do Not Copy Active Spec Kit Runtime

Do not copy these active old-process surfaces into FS.GG.UI:

```text
.specify/scripts/**
.specify/extensions/**
.specify/presets/**
.specify/templates/**
.specify/workflows/**
.specify/integration.json
.specify/integrations/**
.specify/init-options.json
.specify/feature.json
```

The new repository starts with:

```text
.specflow/project.graph.json
.specflow/product.graph.json
.specflow/current.json
.specflow/config.json
.specflow/schema/project-graph.schema.json
.specflow/schema/product-graph.schema.json
.specflow/schema/feature-graph.schema.json
.specflow/schema/consumer-graph.schema.json
.specflow/schema/migrations.generated.md
```

### Start With New Agent Surface

Do not copy active old workflow skills:

```text
.agents/skills/speckit-*
.claude/skills/speckit-*
```

Start with one generated context-pack entry point:

```text
.agents/skills/specflow/SKILL.md
```

Phase-specific `specflow-*` skills may be generated later if they prove useful,
but they are projections of context-pack policy, not the primary source.
Regenerate `.claude` only if the new repository still wants a Claude mirror.

### Assemble New Feature Artifacts

For the new repository:

- create a seed `feature.graph.json` only for the first FS.GG.UI feature;
- generate `spec.md`, `plan.md`, and `tasks.md` from that seed graph;
- do not import old `tasks.deps.yml` into active state;
- use `readiness/evidence.json` plus
  generated reports.

Historical feature directories remain in the old repository. The new repository
may carry a curated migration note, selected ADRs, and source provenance, but it
should not begin life with old feature workflow state.

## Spec Implementation Roadmap

Implement the plan as a small kernel track plus ten FS.GG.UI specs. The kernel
track can start in this repository because it is deliberately independent of the
rebrand destination. The first FS.GG.UI spec is still the final old-repository
bootstrap feature. Specs 2-10 belong in FS.GG.UI. The six additional contract
surfaces are assigned to the existing specs instead of creating a second
roadmap: schema migration and package layers land with ProjectGraph,
host/design/support rows land with ProductGraph and product hardening, and
ConsumerGraph lands with generated-product integration. This keeps the complete
break honest: the old repository extracts the reusable substrate, produces the
new repository and provenance, then stops owning active product development.

Roadmap dependency shape:

```text
K01 standalone rule kernel
  -> K02 governance contract and pilot adapters
      -> S01 old-repo bootstrapper
          -> S02 FS.GG.UI repository seed
              -> S03 ProjectGraph kernel
              -> S04 ProductGraph kernel
              -> S05 FeatureGraph kernel
                  -> S06 Evidence ledger and traceability
                      -> S07 Routing and target integration
                      -> S08 Package/template/docs/generated-product integration
                      -> S09 Platform/CI/release/provenance integration
                      -> S10 Product contract hardening
```

Cross-spec rules:

- Do not add compatibility aliases for `.specify`, `speckit-*`, old package IDs,
  or old template identity.
- Do not import historical feature workflow state into FS.GG.UI active state.
- Every spec that writes generated files also adds the projection check that
  catches hand edits.
- Every spec that introduces authoritative evidence also defines freshness,
  graph-hash binding, and diagnostic wording.
- Keep implementation units spec-sized: each spec must produce a repo state that
  can be reviewed without understanding all later specs.

### Spec K01 - Standalone Rule Kernel

Repository: this repository first; package identity should be carried into
FS.GG.UI unchanged unless the brand decision changes it.

Purpose:

- Extract the generic inference and evidence substrate into a small packable
  library that the two non-FS.GG.UI pilot projects can consume without adopting
  FS.GG.UI policy.

Deliverables:

- `FS.GG.RuleKernel` library project.
- Nominal ID and diagnostic modules.
- Deterministic fact store and fixed-point evaluator.
- Provenance model and explanation DTOs.
- Generic evidence row and freshness helpers.
- Deterministic JSON/hash helpers needed by graph projections.
- `RuleKernel.Tests` with unit, property, round-trip, and toy-domain tests.
- One tiny sample adapter using a non-governance fact domain.

Exit criteria:

- Kernel has no references to FAKE, git, filesystem scanning, process
  execution, Skia/UI runtime packages, NuGet publishing, FS.GG.UI package IDs,
  or template/profile vocabulary.
- A toy domain can derive facts, query decisions, render explanations, and
  round-trip JSON deterministically.
- Property tests cover idempotence, stable ordering, convergence diagnostics,
  provenance completeness, and graph-hash determinism.
- Public surface is small enough to document in one page and review as an API.

Not in scope:

- FS.GG.UI route rules.
- ProjectGraph/ProductGraph/FeatureGraph schema.
- Generated-product validation.
- Publishing a stable 1.0 package.

### Spec K02 - Governance Contracts And Pilot Adapters

Repository: this repository first, with pilot-project smoke fixtures either as
local fixtures or separate example adapters that do not import private project
code.

Purpose:

- Prove the standalone kernel is easy to incorporate and decide which governance
  vocabulary is genuinely shared across projects.

Deliverables:

- Optional `FS.GG.Governance` preview package with shared contracts only:
  project graph, feature graph, evidence ledger, route decision, agent decision,
  context pack, and projection DTOs.
- `FS.GG.UI.Governance` preview adapter that maps current FS.Skia.UI/FS.GG.UI
  route/evidence facts into `RuleKernel`.
- Two pilot adapter fixtures, one per candidate project, each using either
  `FS.GG.RuleKernel` alone or `FS.GG.Governance` plus local facts.
- Incorporation report measuring adapter size, required local assumptions,
  useful queries, and vocabulary that should not be generalized.
- Compatibility facade plan for `FS.Skia.UI.Build.Evidence.GeneratedRunner.run`.

Exit criteria:

- Each pilot can define a local fact domain and get useful validation/explain
  output without copying FS.GG.UI folder layout, target names, package IDs,
  template profiles, or readiness-file conventions.
- Shared vocabulary moves into `FS.GG.Governance` only when at least two
  projects use it without FS.GG.UI-specific assumptions.
- FS.GG.UI-specific rules stay in `FS.GG.UI.Governance`.
- Generated-product and FAKE compatibility remain owned by `FS.GG.UI.Build` or
  its successor facade.

Not in scope:

- Forcing either pilot project onto ProjectGraph or FeatureGraph.
- Extracting FS.GG.UI release/template/product policy as a standalone product.

### Spec 01 - Old Repo Bootstrapper

Repository: this repository.

Purpose:

- Build the final old-repository feature that assembles FS.GG.UI and records why
  each selected surface was copied, rewritten, dropped, or archived.

Deliverables:

- `SpecFlowBootstrap` modules for brand matrix, source selection, rewrite plan,
  new repository assembly, bootstrap evidence, and provenance.
- FS.GG.UI brand matrix seeded with `FS.GG.UI` and the tagline
  "Graph-governed UI infrastructure for F# and Skia."
- Source selection manifest for product libraries, tests, docs, template
  slices, active ADRs, and governance kernel slices.
- Rewrite plan for namespace, package IDs, template identity, docs URLs, skill
  names, and package-version property names.
- Staged new repository tree under `artifacts/rebrand/<new-repo-name>/`.
- Bootstrap provenance file with source commit, copied paths, rewritten paths,
  dropped paths, and migration/deprecation map.

Exit criteria:

- Staged tree exists and contains no active `.specify` runtime.
- Staged tree has no active `FS.Skia.UI` identity outside migration/deprecation
  notes.
- Staged tree includes enough source, tests, docs, template, and governance
  code to become the first FS.GG.UI commit.
- This repository has a bridge/archive plan and no planned long-lived workflow
  conversion work.

Not in scope:

- Full `ProjectGraph`, `ProductGraph`, or `FeatureGraph` implementation inside
  the old repository.
- Historical feature import.
- NuGet publishing.

### Spec 02 - FS.GG.UI Repository Seed

Repository: FS.GG.UI.

Purpose:

- Turn the staged tree into the first coherent FS.GG.UI repository.

Deliverables:

- New solution/project layout under `FS.GG.UI.*` namespaces and package IDs.
- Initial package matrix and template identity:
  `FS.GG.UI.Scene`, `FS.GG.UI.Controls`, `FS.GG.UI.Controls.Elmish`,
  `FS.GG.UI.Template`, and any other carried packages.
- Initial docs identity and docs URL policy.
- Initial template short name `fs-gg-ui`.
- No active `.specify` runtime, no active `speckit-*` skills, no old active
  workflow metadata.
- Basic restore/build/test/pack command wiring appropriate to the new repo.

Exit criteria:

- Fresh checkout restores and builds with the new identity.
- Package/template/docs metadata all agree on FS.GG.UI identity.
- Old identity appears only in migration/provenance/deprecation docs.
- First commit provenance points back to this repository and source commit.

Not in scope:

- Graph validation kernel beyond minimal seed files.
- Release publishing.
- Product contract completeness.

### Spec 03 - ProjectGraph Kernel

Repository: FS.GG.UI.

Purpose:

- Make project policy graph-owned.

Deliverables:

- `.specflow/project.graph.json`.
- `ProjectGraphModel`, deterministic JSON parser/writer, graph hash, schema
  projection, and validator.
- Identity, package, template, docs, target-catalog, routing-policy, release,
  platform, and provenance policy stubs.
- Graph schema migration policy and migration fixture validation.
- Package/module layer policy for project references, namespace ownership,
  public/internal boundaries, governance/product dependency direction, and
  test-only references.
- `specflow project validate`, `specflow project render`, and
  `specflow project check`.
- `SpecFlowProjectCheck` and initial policy projection check.

Exit criteria:

- Project graph round-trips deterministically.
- Project graph hash changes only for semantic project-policy changes.
- Schema migrations are explicit, fixture-backed, and projection-checked.
- Package/template/docs identity drift is reported from graph policy.
- Package/module layer violations fail with source and target project IDs.
- Hand-edited project-policy projections fail with a clear diff.

Not in scope:

- Feature task workflow.
- Product control/scenario contracts.
- CI online verification.

### Spec 04 - ProductGraph Kernel

Repository: FS.GG.UI.

Purpose:

- Make product promises graph-owned.

Deliverables:

- `.specflow/product.graph.json`.
- `ProductGraphModel`, deterministic JSON parser/writer, graph hash, schema
  projection, and validator.
- Imported seed rows for packages/capabilities, controls, public surfaces, docs
  pages, docs images, samples, generated profiles, test specs, performance
  corpus rows, interaction/accessibility rows, architecture trace rows, host
  runtime rows, design-system rows, support-bundle rows, and
  toolchain/environment policy.
- `specflow product validate`, `specflow product render`, and
  `specflow product check`.
- `SpecFlowProductCheck` and `SpecFlowProductProjectionCheck`.

Exit criteria:

- Product graph references current project graph hash.
- Product projections cover the initial control/capability/scenario surface.
- Product projections cover host runtime, design-system, and support-bundle
  policy stubs.
- Orphan controls, docs pages, screenshots, public surfaces, and sample rows are
  diagnosed.
- Hand-edited product projections fail with a clear diff.

Not in scope:

- Full visual/performance/accessibility enforcement. Spec 10 hardens those.
- Feature workflow.

### Spec 05 - FeatureGraph Kernel

Repository: FS.GG.UI.

Purpose:

- Replace Markdown task/spec/plan authority with a typed feature graph.

Deliverables:

- `feature.graph.json` model, parser/writer, hash, schema projection, and
  validator.
- Generated `spec.md`, `plan.md`, `tasks.md`, task graph JSON/Markdown, and
  context-pack skeletons.
- `specflow new`, `specflow activate`, `specflow status`,
  `specflow graph validate`, and `specflow project --check`.
- `SpecFlowGraphCheck` and `SpecFlowProjectionCheck`.
- Seed first FS.GG.UI feature graph.

Exit criteria:

- `spec.md`, `plan.md`, and `tasks.md` are generated projections.
- `tasks.deps.yml` is not an authored active artifact.
- Feature graph references current project and product graph hashes.
- A fresh checkout can explain the active feature without `.specify`.

Not in scope:

- Gate evidence recording.
- Route integration.
- Approval/research refresh ergonomics beyond schema fields.

### Spec 06 - Evidence Ledger And Traceability

Repository: FS.GG.UI.

Purpose:

- Make evidence authoritative, typed, fresh, and graph-bound.

Deliverables:

- Evidence ledger model for gate, CI, package, docs, visual, scenario,
  performance, interaction/accessibility, architecture, environment, research,
  approval, and manual observation rows.
- Evidence freshness rules for project/product/feature graph hashes and commit
  binding.
- `specflow evidence add-*`, `specflow task complete`, and evidence status
  commands.
- Traceability reports for requirement -> task -> evidence and product subject
  -> scenario -> evidence.
- `SpecFlowAudit`.

Exit criteria:

- Done task requires graph-owned completion proof.
- Stale evidence cannot satisfy current readiness.
- Missing evidence payloads fail unless explicitly external/deferred.
- Local development evidence is distinct from release provenance.

Not in scope:

- Route gate selection.
- CI/release attestation verification.

### Spec 07 - Routing And Target Integration

Repository: FS.GG.UI.

Purpose:

- Bind declared feature impact, actual diff classification, target catalog, and
  required gates.

Deliverables:

- Target catalog policy over compiled targets.
- Route planning over project, product, and feature impact.
- Actual route capture from git diff.
- JSON route output and route evidence rows.
- Drift diagnostics for undeclared public surface, package, template, docs,
  product, platform, and governance impact.
- `SpecFlowTraceCheck`, route subcheck, and target metadata projection check.

Exit criteria:

- Feature plans show expected route before implementation.
- Actual route is compared to declared impact after implementation.
- Required gates map to target IDs or declared CI-only checks.
- Missing or stale gate evidence blocks readiness.

Not in scope:

- Package/template generated-product deep integration.
- CI platform verification.

### Spec 08 - Package, Template, Docs, And Generated Product Integration

Repository: FS.GG.UI.

Purpose:

- Bind project package policy and product capability policy to generated product
  behavior.

Deliverables:

- Package matrix validation.
- Template identity validation and generated template profile matrix.
- Template archetype generation/drift checks from ProjectGraph and ProductGraph.
- ConsumerGraph emission for generated products as
  `.specflow/consumer.graph.json`, with `.specflow/consumer.contract.json` as a
  generated compact projection when useful.
- Lean consumer-mode `specflow` command surface for generated-product health,
  package/profile checks, optional feature workflow, upgrade guidance, schema
  migration, and support bundles.
- Consumer simulation validation that restores from the package feed under test,
  instantiates the template, builds the generated product, and runs selected
  profile smoke/headless checks without requiring a framework source checkout.
- Upgrade report/command that compares the current consumer graph to target
  package/template policy and classifies replaceable, durable, and manually
  migrated files.
- Generated product pack/install/instantiate evidence rows.
- Docs policy projections for package readmes, docs navigation, API reference
  assumptions, migration pages, and active old-brand-link restrictions.
- Capability/template/profile projections from ProductGraph and ProjectGraph.
- `SpecFlowPackagePolicyCheck`, `SpecFlowTemplatePolicyCheck`,
  `SpecFlowConsumerCheck`, and docs projection subchecks.

Exit criteria:

- Package IDs, template IDs, central pins, generated product pins, and docs
  identity agree.
- Template profiles are graph-owned and cover `minimal-scene`, `controls-app`,
  `governed-product`, `sample-pack`, and `headless-validation` or explicitly
  defer them.
- Every generated profile has validation evidence.
- Every generated profile records selected profile, package matrix,
  capabilities, durable files, replaceable files, validation commands, and
  upgrade/support-bundle policy in the consumer graph.
- Generated-product validation works from package artifacts and template output,
  without depending on the framework source checkout.
- Upgrade output identifies changed generated files, preserved user files, and
  manual migration notes.
- Support-bundle output follows graph-owned redaction policy and can be imported
  as issue-reproduction evidence.
- Product capability rows map to package/template/docs surfaces.
- Active docs reject old identity except migration/deprecation pages.

Not in scope:

- NuGet publishing.
- GitHub ruleset verification.
- Full visual/performance hardening.

### Spec 09 - Platform, CI, Release, And Provenance Integration

Repository: FS.GG.UI.

Purpose:

- Make merge and release policy graph-owned and provenance-aware.

Deliverables:

- Offline workflow/ruleset policy projections.
- Workflow permissions checks.
- Required status-check mapping to local targets or declared CI-only checks.
- Trusted Publishing policy declaration.
- Release plan command that emits `Push`, `SkipExisting`, and `Block` decisions.
- Package digest, Source Link, signing, attestation, SBOM, and builder identity
  evidence model.
- Deprecation plan for old `FS.Skia.UI.*` package IDs.
- `SpecFlowPlatformPolicyCheck`, `SpecFlowReleasePolicyCheck`, and
  `SpecFlowProvenanceCheck`.

Exit criteria:

- Release plan is deterministic and idempotent.
- Publish jobs use release environment and least privileges.
- Trusted Publishing fields match repository/workflow/environment policy.
- Release evidence cannot be satisfied by local gate logs.
- Old package IDs have alternate-package deprecation rows.

Not in scope:

- Publishing the first package unless explicitly chosen as a follow-up release
  spec.
- Generic reusable governance product extraction.

### Spec 10 - Product Contract Hardening

Repository: FS.GG.UI.

Purpose:

- Turn the seeded ProductGraph into an enforced product contract.

Deliverables:

- Control/capability/public-surface hardening.
- Visual evidence policy for docs images, preview harness fixtures, gallery
  screenshots, and smoke screenshots.
- Scenario corpus policy over docs test specs, samples, generated profiles, and
  smoke evidence.
- Performance budget policy over retained-render and future layout/viewer
  metrics.
- Interaction/accessibility policy over focus, pointer, keyboard, visual state,
  contrast, diagnostics, and accessibility expectations.
- Architecture trace policy over source path groups, ADRs, architecture docs,
  and feature IDs.
- Toolchain/environment policy over SDK, target frameworks, package sources, CI
  images, native dependencies, docs environment, visual environment, and
  generated-product environment.
- Host runtime policy over windowed, headless, screenshot, sample, and
  generated-product hosts.
- Design-system policy over DTCG token sources, generated token modules, themes,
  density modes, visual states, and contrast obligations.
- Support-bundle policy over consumer diagnostics, redaction, collected fields,
  output format, and evidence import.
- `SpecFlowControlCatalogCheck`, `SpecFlowVisualEvidenceCheck`,
  `SpecFlowScenarioCorpusCheck`, `SpecFlowPerformanceBudgetCheck`,
  `SpecFlowInteractionContractCheck`, `SpecFlowArchitectureTraceCheck`,
  `SpecFlowHostRuntimeCheck`, `SpecFlowDesignSystemCheck`,
  `SpecFlowSupportBundleCheck`, and `SpecFlowEnvironmentPolicyCheck`.

Exit criteria:

- Product subjects have complete docs/sample/test/evidence bindings or explicit
  deferrals.
- Required visual/performance/interaction/accessibility evidence is graph-bound
  and fresh.
- Architecture trace gaps are diagnosed for high-risk source changes.
- Host runtime, design-system, and support-bundle gaps are diagnosed where they
  affect scenarios, template profiles, evidence, or generated products.
- Environment mismatch is diagnosed before visual, generated-product, docs, or
  release evidence is trusted.

Not in scope:

- Adding new controls or runtime features for their own sake.
- Turning ProductGraph into a generic product-management database.

## Implementation Stages

Full integration changes the order of attack. The workflow-only stages below are
still useful implementation detail, but the first deliverable should be a
project-governance kernel that can create the new repository correctly and keep
identity, package, template, docs, CI, release, and evidence policy in one
model.

### Stage K0 - Define The Standalone Kernel Contract

Deliverables:

- ADR: "RuleKernel is generic; FS.GG.UI governance is an adapter".
- Package names, namespace roots, and preview-versioning policy for
  `FS.GG.RuleKernel`, optional `FS.GG.Governance`, and
  `FS.GG.UI.Governance`.
- Public surface sketch for facts, rules, provenance, evidence rows,
  diagnostics, graph hashes, queries, and rendering DTOs.
- Explicit non-goals: no FAKE, filesystem, git, process execution, package
  publishing, template, Skia/UI runtime, or FS.GG.UI policy in the kernel.
- Pilot-project incorporation criteria.

Tests:

- Public API approval/golden surface for the kernel.
- Dependency check proves the kernel has only BCL/FSharp.Core and approved pure
  serialization/hash dependencies.
- A source scan fails if FS.GG.UI package names, template names, or FAKE target
  names appear in the kernel source.

### Stage K1 - Implement RuleKernel Core

Deliverables:

- `FS.GG.RuleKernel` project.
- `RuleId`, `FactId`, `QueryId`, `EvidenceId`, `GraphHash`, `SourceRef`, and
  diagnostics modules.
- Deterministic `FactSet`, `Rule<'fact>`, fixed-point evaluator, rule trace,
  provenance chain, and query helpers.
- Generic evidence row and freshness helpers.
- Stable JSON DTOs for explanation and evaluation output.
- Toy-domain sample.

Tests:

- Fixed-point evaluation is deterministic for shuffled input/rule order when
  ids are equal.
- Re-running evaluation over its own derived facts is idempotent.
- Non-converging rules produce a bounded diagnostic instead of hanging.
- Every derived fact has non-empty provenance.
- Evidence freshness rejects mismatched graph hash, stale commit, and missing
  required payload.

### Stage K2 - Add Governance Contracts And Pilot Adapters

Deliverables:

- Optional `FS.GG.Governance` contracts package if two pilots need shared graph
  DTOs.
- `FS.GG.UI.Governance` adapter over current `Targets`, `Routing`, and
  `Evidence` concepts, without moving effectful build execution.
- Two pilot adapters or fixtures that define local facts and call
  `RuleKernel`.
- Incorporation report documenting adapter LOC, local assumptions, useful
  output, and vocabulary rejected as too FS.GG.UI-specific.

Tests:

- Pilot adapters compile and run without referencing `FS.GG.UI.Governance`.
- FS.GG.UI adapter preserves current route selection and evidence graph/audit
  behavior through golden parity.
- Generated-product facade compatibility test still resolves
  `FS.Skia.UI.Build.Evidence.GeneratedRunner.run` or its planned FS.GG.UI
  successor.

### Stage K3 - Carry The Kernel Into The Bootstrap

Deliverables:

- Bootstrap source-selection rules classify `RuleKernel`, optional governance
  contracts, and FS.GG.UI adapter separately.
- New-repository assembly plan keeps the generic kernel in a reusable package
  and FS.GG.UI policy in the UI governance package.
- Package dependency policy ensures generated products do not transitively
  receive the full maintainer governance runtime unless they opt into governed
  product mode.

Tests:

- Staged new repository contains the expected package split.
- `FS.GG.RuleKernel` still has no FS.GG.UI policy references after namespace
  rewrite.
- Consumer-mode generated product validation references only the lean facade and
  declared consumer graph contracts.

### Stage G0 - Commit Full Governance Scope

Deliverables:

- ADR: "Project graph, product graph, and feature graph are the governance
  authorities".
- Decide the new project name, package prefix, root namespace, template short
  name, docs URL, and repository owner/name.
- Record old-to-new identity mapping and package deprecation intent.
- Decide which platform policies are generated only, checked offline, checked
  online, or release-gated.

Exit criteria:

- Maintainers accept that existing agent workflows, `.specify`, and old
  package/template identity can be broken.
- The first new-repo feature is allowed to import only the governance/product
  pieces that serve the new identity.

### Stage G1 - Add Project Graph And Identity Policy

Deliverables:

- `ProjectGraphModel.fs(i)`.
- `ProjectGraphJson.fs`.
- `ProjectGraphHash.fs`.
- `IdentityPolicy.fs`.
- `GraphMigrationPolicy.fs`.
- `PackageLayerPolicy.fs`.
- `.specflow/project.graph.json` fixture with the new brand matrix.
- Generated JSON Schema for editor support.

Tests:

- Project graph round-trip is deterministic.
- Hash ignores JSON field-order variation.
- Schema migration fixtures prove supported old graph versions upgrade
  deterministically.
- Old brand references outside migration/deprecation rows are rejected.
- Package prefix, namespace, docs URL, template identity, and repository identity
  are mutually consistent.
- Project references and namespaces satisfy package/module layer policy.

### Stage G2 - Bind Existing Target And Routing Catalogs

Deliverables:

- `TargetCatalogPolicy.fs`.
- `RoutingPolicy` graph bindings over existing `Targets` and `Routing`.
- `SpecFlowProjectCheck`.
- `SpecFlowPolicyProjectionCheck` target-catalog subcheck.
- JSON route output and graph route evidence shape.

Tests:

- Every graph target ID maps to the compiled `Target` union.
- Every routing gate maps to a target or declared CI-only check.
- Target metadata projection catches drift against compiled target metadata.
- Route evidence records diff scope, matched rules, required gates, and expected
  artifacts.

### Stage G3 - Add Evidence Ledger And Freshness Rules

Deliverables:

- `EvidenceLedger.fs`.
- `GateEvidence.fs`.
- `ArtifactProvenance.fs` skeleton.
- Evidence row parser/writer, hash binding, and path validation.
- Bootstrap adapter that can summarize selected old readiness files as
  migration provenance without importing old feature workflow state.

Tests:

- Pass evidence with missing payload fails.
- Evidence bound to an old graph hash cannot satisfy current readiness.
- Local gate evidence cannot satisfy release provenance.
- Package/release evidence must include package ID, version, digest, commit, and
  builder/workflow identity when release policy requires it.

### Stage G4 - Integrate Package, Template, Docs, And Generated Products

Deliverables:

- `PackagePolicy.fs`.
- `TemplatePolicy.fs`.
- `DocsPolicy.fs`.
- `ConsumerGraphModel.fs`.
- `ConsumerContractValidation.fs`.
- `SupportBundlePolicy.fs` consumer-mode projection.
- Port `GeneratedProduct`, `Capabilities`, `ApiSurfaceGen`,
  `PerPackageSurface`, and `PackageSkew` under graph-owned policy.
- Project-policy projections for package metadata, template identity, capability
  rows, docs governance, generated skills, consumer graph schema, and generated
  consumer projections.

Tests:

- Every packable project has a graph package row or explicit private
  declaration.
- Template package ID, template identity, short name, install command, and
  generated docs agree.
- Generated product pins match the package matrix.
- Generated products emit and validate `.specflow/consumer.graph.json`.
- Generated-product support bundles follow graph-owned redaction policy.
- Active docs reject old identity outside migration pages.
- Public-surface baselines resolve through package policy.

### Stage G5 - Add Platform And CI Policy

Deliverables:

- `PlatformPolicy.fs`.
- `CiPolicy.fs`.
- Offline workflow/ruleset validators.
- Generated `.github/workflows/*.yml` and `.github/rulesets/*.json` or checked
  hand-authored equivalents.
- Optional online verifier behind an explicit command or CI-only release mode.

Tests:

- Workflows have explicit minimal `permissions:`.
- Publish/attestation permissions appear only on release jobs.
- Required status-check names map to local targets or declared CI-only checks.
- Cross-repository reusable workflow refs satisfy pinning policy.
- Online verifier absence does not fail deterministic local checks.

### Stage G6 - Add Release, Deprecation, And Provenance Policy

Deliverables:

- `ReleasePolicy.fs`.
- `DeprecationPolicy.fs`.
- Trusted Publishing policy declaration.
- Source Link/signing/attestation/SBOM policy rows.
- Release plan command that outputs `Push`, `SkipExisting`, and `Block`
  decisions without publishing.
- `SpecFlowReleasePolicyCheck` and `SpecFlowProvenanceCheck`.

Tests:

- Publish plan is deterministic and idempotent.
- Trusted Publishing fields match repository, workflow file, environment, and
  package owner policy.
- Source Link/signing/attestation requirements are either satisfied or explicitly
  deferred.
- Deprecation rows link old package IDs to new package IDs and migration docs.

### Stage G7 - Bootstrap The New Repository

Deliverables:

- `RepositoryBootstrap.fs`.
- `MigrationPolicy.fs`.
- `specflow project bootstrap-repo --output artifacts/rebrand/<new-repo-name>`.
- Generated new repository tree with new identity, package matrix, template
  identity, docs metadata, workflows, ruleset projections, and SpecFlow kernel.
- Provenance file recording source repository URL, source commit, copied paths,
  rewritten paths, dropped paths, and package/template migration map.

Tests:

- Staged new repository has no active `.specify` runtime.
- New identity is present in packages, namespaces, docs, template, generated
  skills, and workflows.
- Old identity appears only in migration/deprecation docs.
- Staged repo restore/build/test/pack/template-instantiation commands are
  planned as evidence rows even if the old repo does not run them locally in the
  report-only phase.

### Stage G8 - Prepare The Active-Development Move

Deliverables:

- Bridge README/report update in this repository.
- Archive or freeze old readiness artifacts.
- New repository first feature uses `ProjectGraph + ProductGraph +
  FeatureGraph + EvidenceLedger + PolicyProjections` from the start.
- No active feature work continues in this repository except migration notices
  and emergency bridge fixes.

Exit criteria:

- This stage is ready to execute after `P7`.
- New repository owns the active project graph, product graph, and feature graph.
- This repository no longer has to support long-lived workflow evolution after
  `P7` and this prepared move execute.

### Stage P0 - Commit ProductGraph Scope

Deliverables:

- ADR: "Product graph owns product contract metadata".
- Decide whether ProductGraph is mandatory in the first new repository commit or
  introduced immediately after repository bootstrap.
- Define product subject namespaces: capability IDs, control IDs, scenario IDs,
  visual evidence IDs, performance budget IDs, and environment IDs.

Exit criteria:

- Maintainers accept that docs images, samples, scenarios, and performance
  budgets are product evidence, not passive files.

### Stage P1 - Add Product Graph Model

Deliverables:

- `ProductGraphModel.fs(i)`.
- `ProductGraphJson.fs`.
- `ProductGraphHash.fs`.
- `ProductContractValidation.fs`.
- `HostRuntimePolicy.fs`.
- `DesignSystemPolicy.fs`.
- `SupportBundlePolicy.fs`.
- Minimal `.specflow/product.graph.json` fixture.

Tests:

- Product graph round-trip is deterministic.
- Product hash changes when semantic product contract fields change.
- Product graph rejects stale project graph hash for readiness.
- Duplicate capability/control/scenario/evidence IDs fail clearly.
- Host runtime, design-system, and support-bundle policy rows validate as
  product contract subjects.

### Stage P2 - Import Capability, Control, And Surface Contracts

Deliverables:

- `CapabilityRegistry.fs`.
- `ControlCatalogPolicy.fs`.
- `PublicSurfacePolicy.fs`.
- Import from `template/capabilities.yml`, `docs/controls/**`, docs images,
  public `.fsi` surfaces, sample galleries, surface baselines, and tests.
- `SpecFlowControlCatalogCheck`.

Tests:

- Every imported capability maps to a package row.
- Every control docs page maps to a control row.
- Every required control screenshot maps to a visual evidence row.
- Every public surface maps to a package row and product subject or explicit
  private/infrastructure declaration.

### Stage P3 - Add Visual Evidence And Scenario Corpus

Deliverables:

- `VisualEvidencePolicy.fs`.
- `ScenarioCorpusPolicy.fs`.
- Import docs images, preview harness fidelity fixtures, gallery screenshots,
  `docs/testSpecs/**`, generated product profiles, and smoke scenarios.
- `SpecFlowVisualEvidenceCheck`.
- `SpecFlowScenarioCorpusCheck`.

Tests:

- Required visual evidence rows name subject, scenario, environment, and
  tolerance.
- Scenario rows map to at least one test spec, sample, generated profile, or
  explicit manual declaration.
- Orphan docs images and orphan preview fixtures are reported.
- Advisory visual evidence cannot satisfy required product proof.

### Stage P4 - Add Performance Budget Contracts

Deliverables:

- `PerformanceBudgetPolicy.fs`.
- Budget rows for retained-render corpus metrics and future layout/viewer
  budgets.
- Product-owned performance report projection.
- `SpecFlowPerformanceBudgetCheck`.

Tests:

- Budget rows require scenario ID, metric ID, threshold, environment, and
  evidence kind.
- Evidence with stale product graph hash cannot satisfy a budget.
- Advisory metrics are rendered but cannot satisfy blocking budgets.

### Stage P5 - Add Interaction And Accessibility Contracts

Deliverables:

- `InteractionAccessibilityPolicy.fs`.
- Control interaction rows for focus, pointer, keyboard, visual-state, disabled,
  hover, pressed, validation, and diagnostics behavior where applicable.
- Accessibility rows for role/name/keyboard/contrast/diagnostic expectations.
- `SpecFlowInteractionContractCheck`.

Tests:

- A control with keyboard behavior must cite keyboard/focus evidence.
- A visual-state behavior change without product impact is blocking.
- Contrast/accessibility obligations map to tests or explicit deferrals.

### Stage P6 - Add Architecture Trace And Environment Policy

Deliverables:

- `ArchitectureTracePolicy.fs`.
- `ToolchainEnvironmentPolicy.fs`.
- `HostRuntimePolicy.fs` hardening.
- `DesignSystemPolicy.fs` hardening.
- `SupportBundlePolicy.fs` hardening.
- Architecture trace rows for `docs/architecture/**`, `docs/adr/**`, source
  path groups, and historical features.
- Environment rows for SDK, target frameworks, package sources, CI images,
  native dependencies, visual evidence environment, docs environment, and
  generated-product environment.
- `SpecFlowArchitectureTraceCheck`.
- `SpecFlowEnvironmentPolicyCheck`.

Tests:

- High-risk source path changes require an ADR/docs trace or explicit no-update
  decision.
- Visual evidence with mismatched environment cannot satisfy required proof.
- Generated-product evidence cites the expected package source and SDK policy.
- Scenario, visual, performance, and generated-product evidence cite host
  runtime IDs where required.
- Design-system obligations map to token generation, visual state, contrast, and
  accessibility evidence.
- Support bundles can be imported as structured reproduction evidence.

### Stage P7 - ProductGraph Cutover

Deliverables:

- `.specflow/product.graph.json` becomes mandatory for active feature readiness.
- Product projections are generated or checked in the new repository.
- Product impact becomes a first-class route dimension.
- Existing docs/control/sample/performance/surface checks are folded under
  product graph checks or retained as subchecks.

Exit criteria:

- A feature touching controls, capabilities, samples, docs images, public
  surfaces, scenario corpus, performance budgets, interaction/accessibility, or
  architecture trace must declare product impact and cite product evidence.
- New repository release readiness includes project policy, product contract,
  feature workflow, evidence ledger, and release provenance.
- After this stage, execute the prepared `G8` move so active development shifts
  to FS.GG.UI and this repository enters bridge/archive mode.

The stages below are **fallback appendix stages** for the case where the rebrand
is deferred and this repository remains the final destination. In the complete
break path they are not the primary plan and should not run ahead of `G0-G8` and
`P0-P7`.

### Fallback Stage A0 - Commit The Breaking Decision

Deliverables:

- This report.
- ADR: "Feature graph is the workflow authority".
- Update AGENTS.md to stop mentioning `.specify` scripts and `speckit-*`
  commands.
- Decide whether `.claude` remains a generated mirror.

Exit criteria:

- Maintainers accept deletion of old Spec Kit surfaces.
- No compatibility window is expected.

### Fallback Stage A1 - Add Core Graph Model

Deliverables:

- `SpecFlow/GraphModel.fs(i)` with the core records and discriminated unions.
- `SpecFlow/GraphJson.fs` deterministic parser/writer.
- `SpecFlow/GraphHash.fs` canonical hash computation.
- `SpecFlow/GraphSchema.fs` generated JSON Schema output.
- Golden fixture for a minimal feature graph.

Tests:

- JSON round-trip is stable.
- Hash is stable over field-order normalization.
- Unknown schema version fails clearly.
- Duplicate IDs fail clearly.

Design constraints:

- No runtime reflection for schema behavior.
- No generated F# quotations.
- Keep JSON formatting deterministic and reviewable.

### Fallback Stage A2 - Add Graph Validation Kernel

Deliverables:

- `GraphValidation.fs`.
- Port task DAG cycle/toposort/synthetic propagation from
  `Evidence/Graph.fs` to operate on `TaskNode`.
- Port owns/skill validation from `Audit.fs` to graph task fields.
- Add requirement coverage validation.
- Add evidence path validation.
- Add approval freshness validation.

Tests:

- Acyclic graph passes.
- Cycle fails with named cycle.
- Dangling dependency fails.
- Done task without completion proof fails.
- Completion proof with missing evidence fails.
- Synthetic taint propagates through real dependencies.
- Accepted synthetic deferral stops taint only when approval is current.

### Fallback Stage A3 - Add Projection Generator

Deliverables:

- `Projection.fs` renders `spec.md`, `plan.md`, `tasks.md`, readiness index,
  task graph JSON, task graph Markdown, and Mermaid.
- `ProjectionCheck.fs` recomputes and diffs projections.
- Generated header format with graph hash.

Tests:

- Golden projections for the minimal fixture.
- Projection check passes immediately after generation.
- Hand-edited projection fails with a small diff.
- Projection output does not include nondeterministic timestamps.

### Fallback Stage A4 - Add `specflow` CLI

Deliverables:

- Root `specflow` launcher.
- `build/SpecFlow.Tool/SpecFlow.Tool.fsproj` or equivalent command path.
- Commands:
  - `new`
  - `activate`
  - `status`
  - `graph validate`
  - `project`
  - `project --check`
  - `trace --json`
  - `context --phase`

Tests:

- CLI returns pure JSON in `--json` mode.
- Non-TTY plain output contains no ANSI.
- Status reads `.specflow/current.json`.
- Missing active feature fails with clear next command.

### Fallback Stage A5 - Import One Active Feature And Cut Over

Deliverables:

- `GraphImport.fs` imports one active feature from current
  `spec.md` / `plan.md` / `tasks.md` / `tasks.deps.yml`.
- Import is one-way and allowed to be lossy only when it reports every lost
  field.
- Generate `feature.graph.json` for the active feature.
- Regenerate projections from the graph.
- Delete active `tasks.deps.yml`.

Tests:

- Import feature 116 fixture.
- Imported task count matches source.
- Imported dependencies match `tasks.deps.yml`.
- Imported requirements include every FR/SC found by symbol extraction.
- Projection check passes after cutover.

Important:

This is not compatibility support. The importer exists to bootstrap the new
authority from the current active feature, then can be removed or left as a
historical tool.

### Fallback Stage A6 - Replace Evidence Graph And Audit Targets

Deliverables:

- Add `SpecFlowGraphCheck`.
- Add `SpecFlowAudit`.
- Wire them into `Targets.fs`, `TargetMetadata.fs`, and `Routing.fs`.
- Remove or stop routing old `EvidenceGraph` / `EvidenceAudit`.
- Write graph reports to:
  - `readiness/task-graph.md`
  - `readiness/task-graph.json`
  - `readiness/evidence.json`
  - `readiness/traceability.md`

Tests:

- New graph check catches every old graph fixture class.
- New audit catches missing readiness/evidence path rows.
- Route selects new graph/audit targets for graph and evidence changes.
- Old target names no longer appear in validation contract unless deliberately
  retained.

### Fallback Stage A7 - Add Route Planning To The Graph

Deliverables:

- `RoutePlanning.fs`.
- Graph-declared impact to expected route selection.
- Actual git diff route capture.
- Drift diagnostics:
  - undeclared public surface change;
  - declared public surface but no actual diff;
  - missing expected gate evidence;
  - gate evidence for gate not selected by route;
  - dogfood escalation mismatch.

Tests:

- Public `.fsi` declared impact selects surface gates.
- Template declared impact selects template/generated gates.
- Governance impact selects graph/audit gates.
- Actual diff mismatch is blocking.
- Expected gates render in `plan.md` projection.

### Fallback Stage A8 - Add Evidence Command Rows

Deliverables:

- `GateEvidence.fs`.
- Commands:
  - `evidence add-gate`
  - `evidence add-file`
  - `evidence add-research`
  - `evidence add-manual`
  - `task complete`
- Optional helper to run a gate and record evidence:
  - `specflow gate run Dev`

Tests:

- Gate evidence row with missing log fails.
- Gate evidence row for failing command cannot satisfy requirement.
- Task completion records evidence and graph hash.
- Re-running projection updates task status in generated `tasks.md`.

### Fallback Stage A9 - Add Context Packs And New Skills

Deliverables:

- `ContextPack.fs`.
- Generated `specflow-*` skills.
- Delete active `speckit-*` skills.
- `specflow context --phase` returns:
  - graph hash;
  - active route expectations;
  - current blockers;
  - required reads;
  - optional reads;
  - generated files not to edit;
  - allowed graph mutations;
  - required evidence shape.

Tests:

- Each phase has a context pack.
- Context pack includes the active graph and excludes stale projections as
  authority.
- Generated skills mention `specflow context`.
- No active skill references `.specify/scripts`.
- No active skill instructs hand-editing generated projections.

### Fallback Stage A10 - Delete Spec Kit Runtime Surfaces

Deliverables:

- Delete `.specify/scripts/**`.
- Delete `.specify/extensions/**`.
- Delete `.specify/presets/**`.
- Delete `.specify/templates/**`.
- Delete `.specify/workflows/**`.
- Delete `.specify/init-options.json`.
- Delete `.specify/integration.json`.
- Delete `.specify/integrations/**`.
- Delete `.specify/feature.json`.
- Add `.specflow/current.json` and `.specflow/config.json`.
- Update docs and AGENTS.md.

Tests:

- Repository search finds no active `.specify/scripts` references.
- Repository search finds no active `speckit` command references except
  historical reports.
- `specflow status` works from a fresh checkout.
- `SpecFlowProjectionCheck` passes.

### Fallback Stage A11 - Worktree-First Execution

Deliverables:

- `Workspaces.fs`.
- `specflow workspace create`.
- Feature workspace state in graph.
- Cache namespace per feature:
  - `.fs-skia-cache/specflow/<feature-id>/`
- Explicit policy for FAKE state:
  - either isolate `.fake` by workspace, or prove shared `.fake` state is safe.

Tests:

- Sibling worktree creation.
- Nested worktree creation if supported.
- Cache writes are feature-namespaced.
- Two workspaces for two features do not share generated-product output paths.
- Workspace disposal does not delete committed evidence.

### Fallback Stage A12 - Approval And Review Gates

Deliverables:

- `Approval.fs`.
- Approval scopes:
  - spec;
  - plan;
  - public surface;
  - security;
  - architecture;
  - release.
- Commands:
  - `approval add`
  - `approval status`
  - `approval revoke`
- Route policy can require approvals for selected impact classes.

Tests:

- Approval with stale graph hash is stale.
- Public surface impact requires approval when policy says so.
- Revoked approval cannot satisfy route.
- Approval projection renders in `plan.md` and readiness index.

### Fallback Stage A13 - Research Claim Verification

Deliverables:

- `ResearchClaims.fs`.
- Commands:
  - `research add-url`
  - `research status`
  - `research stale`
- Optional non-gate helper:
  - `research refresh` that performs network lookup and records a new claim.

Tests:

- Current-version claim without URL fails.
- URL claim without retrieval date fails if marked current.
- Old retrieval date reports stale warning by policy.
- Network refresh is never required by deterministic gates.

### Fallback Stage A14 - Historical Import And Archive

Deliverables:

- Optional import of recent historical features to graph format.
- Archive old readiness files not referenced by evidence rows.
- Generate historical feature index from graphs.

Tests:

- Import can skip unsupported historical shapes with explicit diagnostics.
- Archive inventory lists moved files.
- Active feature graph remains unaffected by historical import failures.

## Testing Strategy

### Unit Tests

- JSON parse/write round-trip.
- Project graph hash determinism.
- Product graph hash determinism.
- Feature graph hash determinism.
- Consumer graph hash determinism.
- Graph schema migration validation.
- Identity policy validation.
- Package/module layer validation.
- Capability/control registry validation.
- Public surface to product subject validation.
- Host runtime contract validation.
- Design-system contract validation.
- Support-bundle contract validation.
- Visual evidence contract validation.
- Scenario corpus validation.
- Performance budget validation.
- Interaction/accessibility contract validation.
- Architecture trace validation.
- Toolchain/environment validation.
- Target catalog binding to compiled target IDs.
- Package matrix validation.
- Template identity validation.
- Docs identity validation.
- Workflow permissions policy validation.
- Trusted Publishing policy validation.
- Release evidence freshness validation.
- DAG cycle detection and topo order.
- Synthetic taint propagation.
- Requirement coverage.
- Evidence path validation.
- Route declared/actual drift.
- Approval freshness.
- Projection rendering.
- Context-pack generation.

### Property Tests

- Any generated acyclic task graph topologically sorts all nodes.
- Adding a dependency never removes an existing cycle diagnostic.
- Projection render followed by parse/import does not invent IDs.
- Graph hash changes when semantic fields change.
- Graph hash does not change when JSON object field order changes.
- Project policy hash changes when package/template/docs/CI/release policy
  changes.
- Project policy hash changes when schema migration or package/module layer
  policy changes.
- Product contract hash changes when controls/capabilities/scenarios/visual
  evidence/performance/interaction/accessibility/architecture/environment policy
  changes.
- Product contract hash changes when host runtime, design-system, or
  support-bundle policy changes.
- Feature readiness cannot be satisfied by evidence bound to a different project
  policy hash.
- Product readiness cannot be satisfied by evidence bound to a different product
  contract hash.
- Consumer health cannot be satisfied by a consumer graph bound to unsupported
  project or product graph hashes.
- Generated package/template/docs identity stays consistent across arbitrary
  valid brand matrices.
- Generated product docs/images/scenario/performance projections stay
  internally consistent across arbitrary valid product subject graphs.

### Golden Tests

- Minimal project graph projections.
- Rebrand project graph projections.
- Minimal product graph projections.
- Consumer graph projections.
- Schema migration projections.
- Package/module layer projections.
- Control catalog product graph projections.
- Host runtime product graph projections.
- Design-system product graph projections.
- Support-bundle product graph projections.
- Visual evidence product graph projections.
- Scenario corpus product graph projections.
- Performance budget product graph projections.
- Interaction/accessibility product graph projections.
- Architecture trace product graph projections.
- Platform policy projection.
- Release policy projection.
- Template/package matrix projection.
- Minimal feature graph projections.
- Public-surface feature graph projections.
- Template-impact feature graph projections.
- Synthetic-deferral feature graph projections.
- Approval-required feature graph projections.
- Context-pack JSON for each phase.

### Integration Tests

- `specflow project validate` validates `.specflow/project.graph.json`.
- `specflow product validate` validates `.specflow/product.graph.json`.
- `specflow consumer validate` validates `.specflow/consumer.graph.json` inside
  a generated product.
- `specflow graph migrate` upgrades supported graph fixtures and rejects
  unsupported schema versions.
- `specflow project check` fails after hand-editing package/template/docs or
  workflow projections.
- `specflow project layer check` fails on forbidden project references,
  namespace ownership violations, and test-only reference leaks.
- `specflow product check` fails after hand-editing control catalog, image
  manifest, scenario, performance, interaction/accessibility, architecture, or
  environment projections.
- `specflow product check` fails after hand-editing host-runtime,
  design-system, or support-bundle projections.
- `specflow consumer support-bundle` emits a redacted bundle that can be
  imported as structured issue-reproduction evidence.
- `specflow project release plan --json` emits deterministic publish/skip/block
  decisions without publishing.
- `specflow project bootstrap-repo` creates a staged new repository tree with
  new identity and migration provenance.
- `specflow new` creates graph and projections.
- `specflow activate` changes `.specflow/current.json`.
- `specflow task complete` updates graph and projections.
- `specflow project --check` fails after hand-editing generated `tasks.md`.
- `SpecFlowGraphCheck` and `SpecFlowAudit` run through FAKE target dispatch.
- `SpecFlowPolicyProjectionCheck`, `SpecFlowPlatformPolicyCheck`,
  `SpecFlowPackagePolicyCheck`, `SpecFlowTemplatePolicyCheck`,
  `SpecFlowReleasePolicyCheck`, and `SpecFlowProvenanceCheck` run through FAKE
  target dispatch.
- `SpecFlowProductCheck`, `SpecFlowProductProjectionCheck`,
  `SpecFlowControlCatalogCheck`, `SpecFlowVisualEvidenceCheck`,
  `SpecFlowScenarioCorpusCheck`, `SpecFlowPerformanceBudgetCheck`,
  `SpecFlowInteractionContractCheck`, `SpecFlowArchitectureTraceCheck`, and
  `SpecFlowEnvironmentPolicyCheck` run through FAKE target dispatch.
- Worktree command creates isolated workspace paths.

## Failure Modes And Diagnostics

| Failure | Diagnostic requirement |
|---|---|
| Projection edited by hand | Name projection, graph hash, and first differing hunk. |
| Unsupported graph schema | Name graph path, found version, supported versions, and required migration command. |
| Schema migration fixture drift | Name migration ID, fixture path, and first differing hunk. |
| Done task lacks evidence | Name task, expected evidence kinds, and suggested `specflow evidence` command. |
| Evidence path missing | Name evidence ID, path, and task/requirement it was meant to satisfy. |
| Route mismatch | Show declared impact, actual diff rule matches, expected gates, actual gates. |
| Project policy drift | Show old project hash, current project hash, changed policy domains, and affected feature approvals/evidence. |
| Product contract drift | Show old product hash, current product hash, changed product subjects, and affected feature approvals/evidence. |
| Consumer contract drift | Show old consumer hash, current consumer hash, changed profile/package/capability/file fields, and suggested health or upgrade command. |
| Package layer violation | Show source project, target project, layer rule, and allowed reference direction. |
| Orphan control docs page | Show docs page, nearest control IDs, and suggested product row or migration declaration. |
| Missing control screenshot | Show control ID, docs page, expected visual evidence row, and screenshot path. |
| Scenario corpus gap | Show scenario ID, missing sample/test spec/generated profile/smoke evidence, and affected product subjects. |
| Performance budget failure | Show scenario ID, metric, measured value, budget, environment, and evidence ID. |
| Interaction/accessibility coverage gap | Show control/capability/scenario ID, missing input/accessibility contract, and expected tests. |
| Architecture trace gap | Show changed source path, subsystem, missing ADR/docs trace, and allowed no-update decision format. |
| Host runtime gap | Show scenario/template profile/evidence row, missing host contract, and allowed host IDs. |
| Design-system gap | Show control or scenario ID, missing token/theme/visual-state/contrast row, and required evidence. |
| Support-bundle policy violation | Show field, redaction policy, output path, and evidence-import rule. |
| Environment mismatch | Show evidence ID, expected environment, actual environment, and affected product proof. |
| Package identity drift | Show package row, project file, generated pins, template pins, and stale metadata path. |
| Workflow permission violation | Show workflow job, requested permission, allowed permission, and policy row. |
| Required status check mismatch | Show ruleset/check name, expected source, local target mapping, and CI evidence state. |
| Release provenance missing | Show package ID, version, missing digest/commit/workflow/builder/attestation field, and required policy. |
| Stale approval | Show approval ID, approved graph hash, current graph hash. |
| Uncovered requirement | Show requirement ID, text, and nearest candidate tasks if any. |
| Orphan evidence | Show evidence ID, path, and no covering requirement/task. |
| Synthetic taint | Show root synthetic task and downstream affected tasks. |
| Context pack stale | Show phase, projection path, expected graph hash. |

## Selective Import Policy

No backward compatibility is required, and the new repository should not inherit
old workflow state by default. Data loss should still be explicit: every copied,
rewritten, dropped, or archived surface gets a provenance decision.

Rules:

- Import durable product source, public `.fsi` contracts, tests, template
  fragments, docs pages, active ADRs, and selected architecture docs.
- Import package/template/docs identity only after rewriting it to the new
  `FS.GG.UI` brand matrix.
- Import product contract state into `product.graph.json`, not into feature
  tasks.
- Import project policy state into `project.graph.json`, not into old
  governance prose.
- Do not import old active feature workflow state unless a row is needed for
  migration provenance.
- Do not import historical readiness logs as active evidence. Keep them in the
  old repository or summarize them in migration notes.
- Do not preserve old requirement IDs, task IDs, or `tasks.deps.yml` unless a
  specific migration note needs them.
- Preserve long-form durable decisions by copying or rewriting ADRs and
  architecture docs, not by importing old feature plans wholesale.
- Record import/drop/rewrite decisions in the bootstrap provenance file.

The old repository remains the audit archive. FS.GG.UI starts from a curated
contract and a clean active workflow.

## Route Policy Changes

Current `Route` reads working-tree diff. Keep that, but add graph planning.

New flow:

1. During planning, graph `ImpactModel` declares expected impact.
2. `RoutePlanning` computes expected gates from declared impact and the current
   project-policy hash.
3. During implementation, actual git diff is classified by existing
   `Routing.fs`.
4. `SpecFlowAudit` compares expected route to actual route.
5. Required local gates map to typed target evidence.
6. Required remote checks map to CI evidence or declared CI-only checks.
7. Missing or stale evidence is blocking.

This makes validation obligations visible before implementation and catches
under-declared features.

## Evidence Policy Changes

Current readiness scanning infers too much from file names and tokens. Replace it
with structured evidence rows.

Keep token scans only for:

- legacy import diagnostics;
- validating generated reports include required summary tokens;
- selected environment evidence formats where key-value text remains the most
  practical attachment format.

Every blocking evidence obligation should have a typed `EvidenceItem`.

Release evidence is stricter than development evidence:

- Local gate rows can satisfy implementation readiness, but not release
  provenance.
- CI rows can satisfy merge/release policy only when they bind to the expected
  commit, workflow, job, and required status-check name.
- Package rows can satisfy release policy only when they bind to package ID,
  version, digest, source commit, package matrix row, and builder identity.
- Attestation rows can satisfy provenance policy only when the artifact subject
  and verification policy match the package row.
- Manual observations can explain decisions but cannot replace package, CI, or
  attestation proof unless a policy row explicitly says so.

Product evidence is stricter than asset presence:

- A docs image satisfies a visual obligation only when it is referenced by a
  visual evidence row with subject, scenario, environment, tolerance, and product
  graph hash.
- A sample smoke log satisfies a scenario obligation only when it cites the
  scenario ID and expected generated profile or sample path.
- A performance corpus row satisfies a budget only when it cites the scenario,
  metric, threshold, environment, measured value, and product graph hash.
- An interaction/accessibility test satisfies a contract only when it cites the
  affected control/capability/scenario and input or accessibility policy.
- A host-runtime-sensitive evidence row satisfies a scenario, visual,
  performance, or generated-product obligation only when it cites the expected
  host contract.
- A design-system-sensitive evidence row satisfies a visual, interaction, or
  accessibility obligation only when it cites the expected theme, density,
  token generation state, and contrast policy.
- An architecture doc or ADR satisfies traceability only when it is linked to
  the changed source path group and feature impact.
- A support bundle starts as non-authoritative issue-reproduction input. It can
  become authoritative only when a maintainer links it to a scenario/product
  evidence row and redaction policy has passed.

## Context Budgeting

Context budgeting is not file compaction. It is phase-specific file selection.

Each context pack includes:

```json
{
  "phase": "implement",
  "graph_hash": "...",
  "required_reads": [],
  "optional_reads": [],
  "generated_do_not_edit": [],
  "allowed_mutations": [],
  "required_evidence": [],
  "route_expectations": [],
  "blockers": [],
  "large_artifacts": []
}
```

Rules:

- Generated files are marked `do_not_edit`.
- Large logs and readiness attachments are opt-in unless needed.
- Skills are selected from task graph `SkillIds`.
- The context pack is deterministic and projection-checked.

## Security And Trust Model

- No third-party extension code is installed by default.
- No community catalog entry is trusted as executable input.
- All executable behavior is repo-owned F# or existing repo scripts.
- Network lookup helpers are opt-in and non-gating.
- Approval artifacts bind to graph hash.
- Evidence rows point to local files or explicit external references.
- Workflow jobs default to minimal permissions.
- Publish and attestation jobs are isolated behind release policy and
  environment protection.
- Trusted Publishing is preferred over long-lived NuGet API keys once the new
  repository/package owner policy is configured.
- Release provenance is CI-backed. Local maintainer logs remain useful
  development evidence, but they do not prove package origin.
- Online platform verification is optional for local development and may become
  mandatory only inside release CI.

## Performance And Caching

Cache only derived data:

- parsed graph;
- graph hash;
- projection render results;
- context-pack render results;
- schema migration fixture parse results;
- consumer health input enumeration;
- skill registry enumeration;
- target metadata enumeration.

Do not cache:

- gate pass/fail verdicts;
- route actual diff results;
- evidence audit verdicts;
- product contract verdicts;
- consumer health verdicts;
- visual evidence verdicts;
- performance budget verdicts;
- support-bundle redaction verdicts;
- approval status against current graph hash.

Cache path:

```text
.fs-skia-cache/specflow/<graph-hash>/
```

Workspace-specific cache path:

```text
.fs-skia-cache/specflow/<feature-id>/<workspace-id>/
```

## Acceptance Criteria

The redesign is complete when:

- `FS.GG.RuleKernel` exists as a small packable pure library with no FS.GG.UI,
  FAKE, git, filesystem, process, Skia/UI runtime, template, or publishing
  dependency.
- At least two non-FS.GG.UI pilot adapters can use `FS.GG.RuleKernel` without
  adopting FS.GG.UI path layout, package IDs, template profiles, target names,
  or readiness-file conventions.
- Shared governance contracts move above FS.GG.UI only when more than one
  project uses them without local-policy leakage.
- FS.GG.UI-specific policy lives in `FS.GG.UI.Governance` or its build adapter,
  not in the generic kernel.
- The FS.GG.UI repository exists as the active development home.
- Project policy is stored in FS.GG.UI `.specflow/project.graph.json`.
- Product contract is stored in FS.GG.UI `.specflow/product.graph.json`.
- Active feature state is stored in FS.GG.UI `feature.graph.json`.
- Generated products store lean consumer state in
  `.specflow/consumer.graph.json`.
- FS.GG.UI has no active `.specify` runtime or `.specify/feature.json`.
- Feature graphs reference the current project-policy hash for readiness and
  release.
- Feature graphs reference the current product-contract hash for product
  readiness and release.
- `spec.md`, `plan.md`, and `tasks.md` are generated projections.
- Project-policy projections cover target metadata, route policy, package
  matrix, package/module layers, template identity, docs policy, schema
  migrations, skills/context packs, workflow policy, ruleset expectations,
  release policy, and provenance policy.
- Product-contract projections cover capability/control registry, public
  surfaces, docs pages, docs images, scenario corpus, visual evidence,
  performance budgets, interaction/accessibility contracts, architecture trace,
  host runtime, design-system, support-bundle, and environment policy.
- `tasks.deps.yml` is not an active authored artifact.
- Task completion is graph-owned and evidence-backed.
- Requirement-to-task-to-evidence traceability is machine-checked.
- Route expectations are graph-visible before implementation.
- Actual diff route is compared with declared impact.
- Gate evidence is stored as typed evidence rows.
- Required GitHub status checks map to local targets or declared CI-only checks.
- Workflow permissions and reusable workflow refs are checked against CI policy.
- Package IDs, versions, central package pins, template pins, generated-product
  pins, and package metadata are graph-owned or graph-checked.
- The template is a generated or drift-checked projection of ProjectGraph and
  ProductGraph, not a separately maintained source of capability or package
  truth.
- Generated products contain a lean ConsumerGraph that records selected
  profile, FS.GG.UI package matrix, enabled capabilities, durable files,
  replaceable files, validation commands, upgrade policy, and support-bundle
  policy.
- Generated products can run consumer-mode health/profile/package validation
  without carrying the full maintainer governance runtime.
- Template validation simulates real consumption from package artifacts and does
  not require a framework source checkout.
- Generated-product upgrade guidance reports changed generated files, preserved
  durable files, and manual migration notes.
- Graph schema migrations are deterministic, fixture-backed, and explicit.
- Package/module layer policy checks project references, namespace ownership,
  public/internal boundaries, and test-only references.
- Host runtime contracts bind windowed, headless, screenshot, sample, and
  generated-product evidence to declared host behavior.
- Design-system contracts bind token sources, generated token modules, themes,
  density modes, visual states, contrast obligations, and evidence.
- Consumer support bundles are graph-owned, redacted by policy, and importable
  as structured issue-reproduction evidence.
- Controls, capabilities, public surfaces, docs pages, screenshots, samples,
  generated profiles, tests, and template fragments are product-graph-owned or
  product-graph-checked.
- Scenario, visual, performance, interaction/accessibility, architecture, and
  environment evidence binds to product-contract IDs and product graph hash.
- Docs and package readmes point to the current project identity; old identity is
  limited to migration/deprecation pages.
- Release plans are deterministic and idempotent.
- Trusted Publishing, Source Link, signing, attestation, SBOM, and deprecation
  requirements are explicit policy decisions.
- Release evidence binds package ID, version, digest, source commit, workflow
  identity, builder identity, and attestation status when required.
- Approval artifacts bind to graph hash.
- Context packs replace long phase prompt instructions as operational input.
- Active `speckit-*` skills and `.specify/scripts/**` are gone.
- New graph and policy FAKE targets replace `EvidenceGraph` / `EvidenceAudit`
  and fragmented guidance/template/publish checks.
- Product graph targets replace fragmented control-doc, visual-evidence,
  scenario-corpus, performance, accessibility, architecture-trace, and
  environment checks where a product contract exists.
- Projection drift is a hard failure.
- Staged new-repository bootstrap can generate a coherent new tree with new
  identity and migration provenance.
- A fresh checkout of FS.GG.UI can validate the project graph, product graph,
  first feature graph, package matrix, template identity, docs identity, release
  plan, and evidence ledger without consulting old active workflow state.
- This repository is in bridge/archive mode after cutover and does not receive
  new product feature work except migration notices or emergency bridge fixes.

## Risks And Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Scope explosion from full integration | Bootstrap never lands | Stage the generic rule kernel, project graph, target/routing binding, evidence ledger, and rebrand bootstrap first; defer optional online verification and full reusable governance-policy extraction. |
| Standalone kernel is too hard to incorporate | Other projects do not adopt it, proving the package boundary was wrong | Require two pilot adapters before treating the package as validated; measure adapter LOC and rejected assumptions. |
| FS.GG.UI policy leaks into the generic kernel | The kernel becomes unusable outside this framework | Add dependency/source scans that forbid FS.GG.UI package IDs, target names, template vocabulary, and repo paths in `FS.GG.RuleKernel`. |
| Shared governance contracts freeze too early | Pilot projects inherit unstable DTOs or semantics | Keep `FS.GG.Governance` preview/internal until at least two projects use the same contract without project-specific exceptions. |
| Polishing the old repo delays the new repo | Energy goes into in-place cleanup instead of extraction | Clean only enough to assemble/prove FS.GG.UI; do not make old-repo workflow conversion a prerequisite. |
| Project graph becomes a mega-config file | Hard to reason about policy | Split `ProjectGraph` from `FeatureGraph`; keep algorithms in compiled F# modules; generate human projections for review. |
| Product graph becomes a product-management database | Hard to maintain and too abstract | Model only enforceable/released/documented contract facts; keep runtime architecture in code and use generated projections for review. |
| Graph schema becomes too large | Hard to author and review | Keep prose fields simple; generate projections for human review; add focused mutation commands. |
| Generated Markdown becomes unreadable | Maintainers stop trusting projections | Golden-review projections early; keep current spec/plan readability as a design constraint. |
| Direct JSON edits are painful | Slower authoring | Provide `specflow requirement/task/evidence/approval` mutation commands. |
| Route planning duplicates `Routing.fs` | Drift between declared and actual route | Reuse `Routing.rules`; do not create a parallel rule set. |
| Evidence rows become busywork | Agents add low-value evidence | Context packs list required evidence per task; audit distinguishes authoritative and informational evidence. |
| Product evidence rows become asset bookkeeping | Maintainers ignore visual/sample/docs evidence | Require product rows only for contract-bearing assets; let purely decorative/advisory assets remain informational. |
| ProductGraph duplicates source truth | Code and graph disagree | Source remains implementation authority; graph owns published contract references and projection/evidence links. Drift is reported instead of silently choosing one. |
| Visual evidence is environment-sensitive | False failures on screenshots | Bind visual evidence to named environments and tolerances; separate advisory visual rows from required release proof. |
| Performance budgets become brittle | Minor runtime variance blocks work | Use deterministic scenario metrics where possible; keep wall-clock/byte-size metrics advisory unless CI environment is pinned. |
| Platform API checks are flaky or credential-dependent | Local gates become unreliable | Make online GitHub/NuGet comparison opt-in or release-only; keep deterministic offline projection checks mandatory. |
| Local logs are mistaken for release provenance | Published packages have weak origin proof | Require CI-backed release rows for package digest, source commit, workflow identity, builder identity, and attestation policy. |
| Package/template identity churn during rebrand | New repo ships inconsistent names or pins | Make brand matrix and package/template policy the first project graph content; check all generated projections together. |
| Template drifts from framework/product contract | Generated products consume unsupported capabilities, stale pins, or misleading docs | Generate or drift-check template profiles, package pins, skills, docs, and validation commands from ProjectGraph and ProductGraph. |
| Generated products inherit maintainer governance complexity | Consumers reject the template or cannot maintain generated apps | Ship a lean consumer-mode `specflow` surface and a small ConsumerGraph instead of the full maintainer graph operating system. |
| Generated-product upgrades are underspecified | Users cannot move between FS.GG.UI versions safely | Make upgrade reporting a first-class command that classifies replaceable generated files, durable user files, and manual migration notes. |
| Graph schema changes become breaking by accident | Old project/product/feature/consumer graphs cannot be opened or upgraded reliably | Require explicit schema migration policy, fixture-backed migrations, and unsupported-version diagnostics. |
| Clean package names hide tangled dependencies | Runtime, template, samples, and governance layers become coupled again | Add package/module layer policy for references, namespaces, public/internal boundaries, and test-only dependencies. |
| Host behavior stays implicit | Evidence passes in one host but fails or misleads in another | Model windowed, headless, screenshot, sample, and generated-product hosts as product contracts cited by evidence. |
| Design tokens drift from product evidence | Themes, density, visual state, or contrast behavior breaks without product-level diagnosis | Put design-system rows in ProductGraph and bind visual, interaction, and accessibility evidence to them. |
| Support reports are unstructured | Consumer issues cannot be reproduced or routed back to product contract gaps | Generate redacted support bundles from ConsumerGraph and import them as structured scenario evidence. |
| Governance package public/private boundary is unclear | New repo freezes a premature external API | Publish only the small `RuleKernel` surface early; keep FS.GG.UI policy and broader governance contracts preview until pilot evidence proves them. |
| Accidentally copying `.specify` into FS.GG.UI | Old workflow assumptions survive the break | Treat `.specify/**` as old-repo archive/provenance only; new repo starts with `.specflow/**` and generated context packs. |
| Worktrees fight shared state | Parallel runs corrupt cache or FAKE state | Namespace caches; explicitly audit FAKE state before default worktree mode. |
| Approval hashes churn too often | Review gates become noisy | Scope approvals; allow narrow approval reuse only when validator proves affected graph fields unchanged. |

## Recommended First Feature Cut

If the rebrand/new-repository decision is accepted, the first cut changes. Do
not start by landing the whole graph operating system in the old tree. The first
cut is the final old-repo feature and the first FS.GG.UI seed. Its purpose is to
create the new repository correctly, not to make this repository clean.

Recommended first cut for the rebrand path:

1. Create a small kernel feature in this repository before or as the first
   slice of `117-rebrand-new-repo-bootstrap`.
2. Add `FS.GG.RuleKernel` with the generic fact/rule/provenance/evidence
   substrate and toy-domain tests.
3. Add two pilot adapters or fixtures proving non-FS.GG.UI incorporation is
   cheap and does not require FS.GG.UI layout, package, target, template, or
   readiness vocabulary.
4. Add the `FS.GG.UI.Governance` adapter plan that maps current
   `Targets`/`Routing`/`Evidence` behavior into the kernel while preserving the
   generated-product facade.
5. Create `117-rebrand-new-repo-bootstrap` in this repository.
6. Add `.specflow/project.graph.json` with the brand matrix, package matrix,
   template identity, docs identity, repository identity, target/routing policy
   references, schema migration policy, package/module layer policy, release
   policy, and old-to-new identity map.
7. Add `.specflow/product.graph.json` with the minimal product contract:
   capabilities, controls, public surfaces, docs pages, docs images, scenarios,
   visual evidence, performance budgets, interaction/accessibility contracts,
   architecture trace, host runtime, design-system, support-bundle, and
   toolchain/environment rows imported from existing surfaces.
8. Add a deterministic new-repository assembly plan that names imported,
   rewritten, generated, and intentionally dropped paths.
9. Add policy projections for packages, template, docs, skills/context packs,
   workflows, ruleset expectations, release plan, product contract, control
   catalog, scenario corpus, performance budgets, architecture trace,
   environment policy, and migration/deprecation map.
10. Generate the new repository tree under `artifacts/rebrand/<new-repo-name>/`
   or an equivalent staging path.
11. Port the core product libraries with new namespaces, package IDs, assembly
   names, and docs metadata.
12. Port the integrated governance/product kernel: `FS.GG.RuleKernel`,
   optional `FS.GG.Governance`, `FS.GG.UI.Governance`, `ProjectGraph`,
   `ProductGraph`, `FeatureGraph`, `EvidenceLedger`, target/routing bindings,
   policy projections, product projections, package policy, template policy,
   docs policy, release policy, and provenance policy.
13. Create the new template package identity, `template.json` identity,
   `shortName`, graph-owned profiles, package pins, generated docs, generated
   skills, `.specflow/consumer.graph.json`, compact consumer contract
   projection, consumer-mode validation commands, support-bundle command, and
   generated-product upgrade report.
14. Add a provenance file that records the old repository URL, source commit,
   copied paths, rewritten paths, dropped paths, package/template migration map,
   project graph hash, and product graph hash.
15. Plan, and later record, typed evidence rows for restore/build/test/pack,
    generated product instantiation, docs generation, control catalog, visual
    evidence, scenario corpus, performance budgets, interaction/accessibility,
    architecture trace, host runtime, design-system, support-bundle,
    environment policy, release plan, and provenance policy.
    Do not confuse local bootstrap logs with final CI release provenance.
16. Prove the staged new repository can restore, build, test, pack packages, and
    instantiate the template from the new identity.
17. Add the bridge README/report change in this repository that points to the
    new repository and documents the package/template migration.

After that bootstrap feature, active feature development moves to FS.GG.UI. This
repository should only receive bridge, archive, provenance, or migration notice
changes.

If the rebrand is deferred and this repository remains the destination, use the
fallback graph-first cut below. That path is no longer the preferred plan.

Because the redesign is breaking, the first cut should still be narrow and
mechanically decisive:

1. Add project graph, product graph, and feature graph models, JSON
   parser/writer, hash, and validator.
2. Bind project policy to existing `Targets`, `Routing`, package, template,
   docs, and release engines.
3. Bind product contract to existing capability, control docs, public surface,
   sample, screenshot, scenario, performance, interaction/accessibility, ADR,
   and environment surfaces.
4. Add projection generator for project-policy and product-contract views plus
   `spec.md`, `plan.md`, `tasks.md`,
   `task-graph.json`, and `task-graph.md`.
5. Add `specflow status`, `specflow project validate`,
   `specflow product validate`, `specflow graph validate`, and
   `specflow project --check`.
6. Import the active feature into `feature.graph.json`.
7. Regenerate projections and delete `tasks.deps.yml` for the active feature.
8. Add `SpecFlowProjectCheck`, `SpecFlowProductCheck`,
   `SpecFlowGraphCheck`, `SpecFlowProjectionCheck`,
   `SpecFlowPolicyProjectionCheck`, and `SpecFlowProductProjectionCheck`.
9. Delete `.specify/scripts/**` and active `speckit-*` skill references in the
   same feature if the graph commands are usable.

Do not start with worktrees, approvals, research refresh, or historical import.
Those are valuable, but the core authority transfer must land first.

## Source Notes

- Governance kernel extraction implementation plan:
  `docs/reports/2026-06-06-1055-governance-kernel-extraction-implementation-plan.md`
- Governance kernel split detailed design:
  `docs/reports/2026-06-07-0838-governance-kernel-split-detailed-design.md`
- GitHub Spec Kit latest release API, checked 2026-06-13:
  <https://api.github.com/repos/github/spec-kit/releases/latest>
- GitHub Spec Kit `v0.10.2` release page:
  <https://github.com/github/spec-kit/releases/tag/v0.10.2>
- Spec Kit extensions README:
  <https://github.com/github/spec-kit/blob/main/extensions/README.md>
- Spec Kit community extension catalog:
  <https://speckit-community.github.io/extensions/all-extensions>
- Spec Kit raw community catalog:
  <https://github.com/github/spec-kit/blob/main/extensions/catalog.community.json>
- Spec Kit built-in workflow:
  <https://github.com/github/spec-kit/blob/main/workflows/speckit/workflow.yml>
- GitHub repository rename documentation:
  <https://docs.github.com/en/repositories/creating-and-managing-repositories/renaming-a-repository>
- GitHub repository transfer documentation:
  <https://docs.github.com/en/repositories/creating-and-managing-repositories/transferring-a-repository>
- NuGet package deprecation documentation:
  <https://learn.microsoft.com/en-us/nuget/nuget-org/deprecate-packages>
- NuGet `.nuspec` package metadata reference:
  <https://learn.microsoft.com/en-us/nuget/reference/nuspec>
- NuGet package creation with MSBuild:
  <https://learn.microsoft.com/en-us/nuget/create-packages/creating-a-package-msbuild>
- NuGet Trusted Publishing:
  <https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing>
- NuGet Central Package Management:
  <https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management>
- NuGet package signing:
  <https://learn.microsoft.com/en-us/nuget/create-packages/sign-a-package>
- .NET Source Link guidance:
  <https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink>
- .NET custom template documentation:
  <https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates>
- .NET project template tutorial:
  <https://learn.microsoft.com/en-us/dotnet/core/tutorials/cli-templates-create-project-template>
- FSharp.Formatting content guidance:
  <https://fsprojects.github.io/FSharp.Formatting/content.html>
- GitHub rulesets documentation:
  <https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets>
- GitHub protected branches and required status checks documentation:
  <https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches>
- GitHub reusable workflow documentation:
  <https://docs.github.com/en/actions/how-tos/reuse-automations/reuse-workflows>
- GitHub `GITHUB_TOKEN` permissions documentation:
  <https://docs.github.com/en/actions/tutorials/authenticate-with-github_token>
- GitHub artifact attestations documentation:
  <https://docs.github.com/en/actions/concepts/security/artifact-attestations>
- GitHub artifact attestation build-provenance workflow documentation:
  <https://docs.github.com/actions/security-for-github-actions/using-artifact-attestations/using-artifact-attestations-to-establish-provenance-for-builds>
- FAKE target execution documentation:
  <https://fake.build/guide/core-targets.html>
- JSON Schema getting-started documentation:
  <https://json-schema.org/learn/getting-started-step-by-step>
- SLSA provenance specification:
  <https://slsa.dev/spec/v1.0/provenance>
- Local modules inspected:
  `build/Governance/Engine/{Model,Update,Interpret}.fs`,
  `build/Governance/Evidence/{Engine,TaskParser,DepsParser,Graph,Audit,Render,Scans}.fs`,
  `build/Governance/{Routing,Targets,TargetMetadata,GeneratedProduct,PrePublish,Publish,Capabilities,ApiSurfaceGen,PerPackageSurface,PackageSkew,Guidance,SkillTreeGen,SkillSync,SymbolCrossCheck}.fs`,
  `.specify/**`, `.agents/skills/**`, and current feature artifacts under
  `specs/116-paint-cache-damage-rects/**`.
- Local governance docs inspected:
  `docs/governance/index.md`, `docs/governance/routing-and-gates.md`,
  `docs/governance/single-source-generation.md`,
  `docs/governance/evidence-and-audit.md`,
  `docs/governance/speckit-placement.md`, `docs/distribution.md`, and
  `docs/development.md`.
- Local rebrand identity surfaces inspected:
  `README.md`, `.template.config/template.json`,
  `.template.package/FS.Skia.UI.Template.fsproj`, `.template.package/README.md`,
  `template/base/Directory.Packages.props`, `template/capabilities.yml`,
  `template/base/**`, `template/product-skills/**`, `src/**/*.fsproj`,
  `build/Governance/FS.Skia.UI.Build.fsproj`, docs links under `docs/**`, and
  the configured git remote.
- Local product-contract surfaces inspected:
  `docs/controls/**`, `docs/img/controls/**`, `docs/testSpecs/**`,
  `docs/architecture/**`, `docs/adr/**`, `samples/**`,
  `readiness/surface-baselines/**`, `readiness/per-package-surface/**`,
  `tests/ControlsPreview.Harness/fixtures/fidelity/**`,
  `tests/Controls.Tests/**`, `tests/Elmish.Tests/**`, `tests/Package.Tests/**`,
  `template/fragments/**`, `template/profiles/**`, and
  `template/capabilities.yml`.
