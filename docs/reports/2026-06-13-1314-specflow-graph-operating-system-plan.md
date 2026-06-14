---
title: SpecFlow graph operating system plan (superseded)
category: Reports
categoryindex: 9
index: 20260613
description: Superseded report pointer for the earlier monolithic SpecFlow graph operating system plan.
---

# SpecFlow graph operating system plan (superseded)

This report has been split and superseded by the FS.GG project split
documentation.

Start with [FS.GG project split](../FS.GG/index.md).

The previous report proposed a monolithic SpecFlow graph operating system with a
custom project graph, product graph, feature graph, evidence ledger, generated
projections, context packs, and release policy. That design captured real drift
risks, but it also made the rendering framework depend on a changing governance
platform.

The current direction is to split rendering and governance into separate
projects, use standard Spec Kit in both, keep rendering development independent
of governance experiments, and let governance tooling mature as optional
external tooling.
