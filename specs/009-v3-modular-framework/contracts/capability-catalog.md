# Contract: Capability Catalog

## Purpose

The capability catalog is the machine-readable source of truth for V3 package,
skill, template, test, dependency, and evidence ownership.

## Required Location

```text
template/capabilities.yml
```

## Required Fields

Each capability entry must provide:

| Field | Required | Meaning |
|-------|----------|---------|
| `id` | yes | Stable lowercase capability identifier. |
| `displayName` | yes | Human-readable name. |
| `packageId` | yes for runtime capabilities | Public package id or explicit non-runtime marker. |
| `project` | yes for runtime capabilities | Project path for the package. |
| `contracts` | yes | Public `.fsi` files or explicit no-public-surface record. |
| `tests` | yes | Test project paths validating the capability. |
| `skill` | yes | Source `SKILL.md` path for the local agent skill. |
| `templateFragment` | yes | Fragment path copied or applied when selected. |
| `dependencies` | yes | Capability ids required by this capability. Empty list is allowed. |
| `profiles` | yes | Profiles that include or allow this capability. |
| `defaultApp` | yes | Whether this capability is included in the default app profile. |
| `evidence` | yes | Readiness artifact classes required when the capability changes. |
| `surfaceBaseline` | yes for public packages | Package-specific baseline path. |

## Default App Contract

The default app profile must resolve to this capability set:

```text
Scene
SkiaViewer
Elmish
KeyboardInput
Layout
Charts
```

Samples must not be part of the default app capability set.

## Validation Contract

`CapabilityCheck` must fail when:

- a capability is missing a required field
- a dependency id does not exist
- dependency traversal contains a cycle
- a default app capability set differs from the required set
- a runtime capability has no package project
- a public capability has no `.fsi` contract and no no-public-surface record
- a capability has no local skill
- a capability has no template fragment
- a capability has no test coverage entry
- a package surface baseline path is missing for a public package

Failures must name the capability id and missing field or invalid relationship.
