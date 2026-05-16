---
name: fs-skia-samples
description: Work on optional generated product sample-pack content.
---

# Samples Capability

## Scope

Owns optional sample-pack template content under `template/fragments/samples/`.

## Public Contract

Samples are non-runtime generated content and have `no-public-surface` in the capability catalog.

## Build Commands

Run `./fake.sh build -t GeneratedProductCheck` and `./fake.sh build -t TemplateCheck`.

## Test Commands

Run generated product `./fake.sh build -t Verify` for the sample-pack profile.

## Evidence

Record sample-pack file lists under `specs/009-v3-modular-framework/readiness/generated-file-lists/`.

## Package Boundary

Do not include samples in default consumer products.

## Generated Product

Samples are copied only when the sample-pack profile or sample capability is selected.
