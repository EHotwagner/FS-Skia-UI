# Contract: Feedback Classification

## Purpose

Maintainers can classify reflection and name-collision feedback consistently
and route it to the right owner.

## Categories

- `PackageDocumentationDiscoverability`: packaged reference, XML docs, compact
  API maps, or package artifact location is missing or hard to find.
- `PublicContractErgonomics`: public names, attributes, modules, or `.fsi`
  shapes make correct authoring ambiguous.
- `GeneratedTemplateWorkflow`: generated template output, generated docs, or
  validation workflow points agents at the wrong authoring path.
- `ConsumerAuthoringGuidance`: examples or product code need explicit
  qualification or better local naming without a package contract change.

## Required Fields

- reported finding
- primary category
- owner
- public contract change required
- generated guidance change required
- runtime behavior in scope
- evidence path
- next action

## Acceptance

Given reflection-based discovery or open-order collision reports, a maintainer
can fill the classification record and identify the next action in under five
minutes.
