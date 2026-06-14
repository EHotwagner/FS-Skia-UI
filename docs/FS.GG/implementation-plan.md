---
title: FS.GG split implementation plan
category: FS.GG
categoryindex: 6
index: 8
description: Single implementation plan for splitting rendering and governance projects while using standard Spec Kit.
---

# FS.GG split implementation plan

This plan replaces the previous monolithic SpecFlow graph operating system plan
as the active direction. The goal is to split rendering and governance into
separate projects, keep standard Spec Kit as the workflow baseline, and prevent
experimental governance from becoming a prerequisite for runtime development.

## Objectives

- Create or designate a rendering repository that owns the UI framework product.
- Create or designate a governance repository that owns optional tooling
  experiments.
- Keep both repositories on standard Spec Kit unless a small, proven tool earns
  adoption.
- Keep rendering build, test, docs, package, and release work independent of the
  governance project.
- Make controls, design-system primitives, concrete themes, and design-specific
  kits explicit rendering-owned layers.
- Preserve useful research from the previous report without carrying forward
  the monolithic graph operating system.

## Non-goals

- Do not implement a mandatory `ProjectGraph`, `ProductGraph`, or
  `FeatureGraph` before the split.
- Do not make generated `spec.md`, `plan.md`, or `tasks.md` the rendering
  workflow authority.
- Do not require governance tooling to run ordinary rendering build/test/release
  work.
- Do not split templates into a separate repository until release cadence or
  ownership requires it.
- Do not rebrand package IDs as part of the split unless that decision is made
  explicitly.

## Stage 1 - Confirm repository map

Decide the repository names, owners, and first branches.

Deliverables:

- rendering repository name and owner;
- governance repository name and owner;
- decision on whether this repository remains the bridge/archive;
- initial README purpose statement for each destination;
- list of source directories that move to rendering;
- list of governance modules and reports that move to governance or remain
  archived.

Exit criteria:

- maintainers can explain which repository owns runtime product work;
- maintainers can explain which repository owns governance experiments;
- no product feature is blocked on creating a custom governance platform.

## Stage 2 - Extract rendering as the product

Move or stage the runtime product into the rendering repository.

Rendering-owned surfaces:

- `src/**` runtime libraries;
- runtime tests;
- docs that describe the product;
- controls catalog and examples;
- design-system primitives and theme definitions;
- concrete themes such as Ant Design, Fluent, Material, or project-specific
  themes;
- optional design-specific kits for patterns that go beyond styling;
- templates and generated-product checks, unless split later;
- package metadata and release notes;
- focused checks for API surface, package skew, docs, template, and packaging.

Workflow:

- initialize standard Spec Kit;
- keep existing useful build targets if they are product checks;
- remove active dependency on `.specify` customizations that belong only to the
  old monolithic workflow;
- document the minimal local validation path;
- document release validation separately from local development checks.

Exit criteria:

- fresh checkout can restore, build, and test;
- docs can be built by the documented command;
- packages can be packed locally;
- template can be packed, installed, instantiated, and built;
- control, design-system, theme, and kit ownership boundaries are documented;
- no governance repository code is required for the above.

Design/control boundary:

- define one semantic control set before adding branded design variants;
- define token and theme extension points separately from control behavior;
- implement Ant Design, Fluent, Material, or product-specific visuals as themes
  over shared controls by default;
- create design-specific kit modules only for composition or workflow patterns
  that cannot be represented as visual theming;
- keep accessibility, focus, pointer, keyboard, and value behavior attached to
  the semantic control contract, not duplicated per theme.

## Stage 3 - Extract governance as a tool product

Move governance experiments into their own repository only after the rendering
product path is clear.

Governance-owned surfaces:

- rule/evidence helper experiments;
- route explanation experiments;
- optional Spec Kit extension ideas;
- report generators;
- external validators;
- examples that inspect the rendering repository without becoming required by
  it.

Workflow:

- initialize standard Spec Kit;
- keep the first product small;
- build tooling as normal libraries or command-line tools;
- treat rendering as one external customer, not as the tool's internal
  filesystem layout.

Exit criteria:

- governance repo builds and tests independently;
- first tool can run against a fixture or external repository;
- rendering does not need the tool to build or release;
- no rendering package/template vocabulary appears in generic governance code
  unless it is in an adapter or example.

## Stage 4 - Define lightweight contracts

Add only the cross-repo contracts that are needed.

Candidate contracts:

- command-line arguments and exit codes for optional validators;
- JSON report shape for optional tooling;
- support-bundle format if governance tooling creates support artifacts;
- package version compatibility ranges;
- docs links and migration pages.
- optional design-token or theme report formats, if governance tooling later
  checks design drift from outside the rendering repository.

Avoid:

- shared graph schemas as a prerequisite for rendering work;
- generated workflow projections across repos;
- shared mutable build state;
- governance-owned release decisions for runtime packages.

Exit criteria:

- each contract has an owner and versioning rule;
- rendering can ignore optional governance reports without breaking routine
  work;
- governance tools fail clearly when a contract version is unsupported.

## Stage 5 - Bridge the old repository

Once the rendering repository is active, convert this repository into bridge and
archive mode.

Deliverables:

- bridge README or report pointing to the rendering and governance repos;
- source commit and path migration notes;
- package/template migration notes if identities change;
- deprecation plan for old packages if replacements are published;
- archived historical reports retained as history, not active workflow state.

Exit criteria:

- new product features are opened in the rendering repository;
- governance experiments are opened in the governance repository;
- this repository receives only bridge, archive, provenance, or emergency
  migration fixes.

## Stage 6 - Decide rebrand separately

The split and the rebrand are related but should not be forced into one change.
After the rendering project is stable, decide whether to keep `FS.Skia.UI` or
move to a new identity such as `FS.GG.UI`.

If rebranding:

- choose root namespace and package prefix;
- choose template package ID and short name;
- choose docs URL and bridge policy;
- publish replacement packages before deprecating old packages;
- update template identity as one coherent matrix.

Exit criteria:

- package, namespace, template, docs, and repository names agree;
- old package IDs have explicit deprecation guidance only after replacement
  packages exist;
- migration docs explain old-to-new identity mapping.

## Checks to keep in rendering

Keep checks that protect product contracts directly:

- unit and integration tests;
- API surface drift checks;
- design-token and theme smoke checks;
- control behavior and accessibility checks;
- package skew checks;
- template pack/install/instantiate checks;
- generated-product restore/build checks;
- docs build checks;
- release package checks.

Do not keep checks whose main purpose is to prove the custom governance platform
itself.

## Governance adoption tests

Before rendering adopts governance tooling as anything stronger than an optional
helper, the tool should pass these tests:

- it works outside the rendering repository;
- it does not require rendering directory layout;
- it does not require rendering package IDs or template names;
- it provides a clear benefit over standard Spec Kit plus existing checks;
- it can be removed without breaking rendering build/test/release.

## Risks and mitigations

| Risk | Mitigation |
|---|---|
| Cross-repo drift grows after the split. | Keep explicit package, docs, template, and command contracts. Add focused checks only where drift is observed. |
| Rendering loses useful governance rigor. | Keep narrow product checks and let governance tooling return as optional validators once stable. |
| Governance becomes too generic to be useful. | Develop against real repositories and require concrete reports, diagnostics, or fixes. |
| Rebrand and split collide. | Split ownership first; rebrand package/template/docs identity as a separate release decision unless already approved. |
| Old repository keeps receiving product work. | Add bridge guidance and move new feature intake to the rendering repository once it builds and tests. |

## Acceptance criteria

The split is complete when:

- rendering has an active repository with standard Spec Kit, product tests, docs,
  package, and template validation;
- rendering documents and enforces the boundary between semantic controls,
  design-system primitives, concrete themes, and design-specific kits;
- governance has an independent repository with standard Spec Kit and no
  required role in rendering's ordinary workflow;
- this repository is documented as bridge/archive;
- routine rendering changes can be built, tested, and released without
  governance tooling;
- governance tools can inspect rendering only through documented optional
  interfaces;
- any rebrand is handled by an explicit package/template/docs migration plan.
