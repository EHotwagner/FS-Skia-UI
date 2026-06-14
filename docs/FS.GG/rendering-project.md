---
title: Rendering project
category: FS.GG
categoryindex: 6
index: 3
description: Scope and operating model for the rendering/runtime repository in the FS.GG split.
---

# Rendering project

The rendering project owns the UI framework as a product. It should be possible
to build, test, package, document, and release it with normal repository tools
and standard Spec Kit, without depending on an experimental governance platform.

## Scope

The rendering project owns:

- scene and drawing primitives;
- layout;
- input and keyboard abstractions;
- Skia viewer and host behavior;
- Elmish integration;
- controls and typed control front doors;
- testing helpers that are part of the product contract;
- runtime packages and package metadata;
- product documentation;
- templates and generated-product smoke tests, unless template cadence later
  justifies a separate repository.

## Workflow

Use standard Spec Kit for feature specification, planning, and task breakdown.
Keep the workflow boring and recognizable. The rendering repository may retain
repo-owned checks that are already valuable, but those checks should stay
narrow:

- API surface drift checks;
- package skew checks;
- template pack/install/instantiate checks;
- docs build checks;
- selected visual or scenario smoke checks;
- release packaging checks.

Do not introduce a custom feature graph as the source of truth for ordinary
rendering work. `spec.md`, `plan.md`, and `tasks.md` may remain authored Spec
Kit artifacts unless a future lightweight tool proves a better path without
raising the cost of contribution.

## Governance boundary

The rendering project can consume optional governance tools, but it must not be
blocked by them. A useful test is:

> Could a contributor clone the rendering repository, read the standard feature
> artifacts, run the documented build/test commands, and ship a routine
> rendering change without understanding the governance repository?

If the answer is no, the boundary has failed.

## Release posture

The rendering project should own its package and docs release policy directly.
Governance tooling can help check that policy, but release identity belongs to
the rendering product:

- package IDs;
- package versions;
- template identity;
- docs URL;
- supported target frameworks;
- release notes;
- migration guidance.

The release path should be conservative and explicit, not generated from an
unproven governance schema.

## What not to carry forward

Do not carry the following as active runtime-repository requirements:

- a mandatory custom `ProjectGraph`;
- a mandatory custom `ProductGraph`;
- a custom `FeatureGraph` replacing standard Spec Kit artifacts;
- graph-bound task completion as the only accepted source of task status;
- generated `spec.md`, `plan.md`, or `tasks.md` as the initial workflow;
- governance workspaces and FAKE concurrency policy as a prerequisite for
  normal product changes.

These ideas can be revisited later if the separate governance project proves a
small, stable implementation.
