---
name: readiness-contract-required-tokens
description: EvidenceAudit's readiness-contract blocks unless governance-risk-levels.md and runtime-limitations.md contain fixed token sets
metadata:
  type: reference
---

`EvidenceAudit` (FR-004 readiness-contract checks) hard-blocks (`readiness-contract-hits`) unless
specific readiness files contain a fixed required-token set — content, not just the filename:

- `readiness/governance-risk-levels.md` must contain: **small**, **medium**, **broad**,
  **required evidence**, **broad validation**. Shape: a risk-level table with small/medium/broad
  rows + a "Required evidence and broad validation" section.
- `readiness/runtime-limitations.md` must contain: **.NET 10 desktop**, **Vulkan**,
  **SkiaSharp preview**, **unsupported macOS/mobile/browser**, **no software-renderer fallback**.
  Even a pure/managed feature must carry an "Inherited product runtime limitations (unchanged by
  this feature)" paragraph with these exact tokens.

The audit prints the `full-required-set` and `absent-from-file` on failure, so the missing tokens
are named. Copy the shape from an existing feature (e.g. `specs/064-publish-nuget-distribution/`
runtime-limitations.md, `specs/044-foundations-single-source-generation/` governance-risk-levels.md).
Learned feature 083.
