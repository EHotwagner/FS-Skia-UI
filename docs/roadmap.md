---
title: Roadmap & TODO
category: Roadmap
categoryindex: 7
index: 0
description: Forward-looking, planned and in-progress work for FS Skia UI. Each item links to its design/implementation plan. Distinct from the Design history section, which holds superseded historical documents.
---

# Roadmap & TODO

This page tracks **planned and in-progress** work on the framework. Each item
links to its design or implementation plan. Unlike the
[Design history](design-history.html) section — which holds *superseded*
historical documents — everything here is forward-looking and not yet shipped.

Status legend: 📋 planned · 🚧 in progress · ✅ shipped (moves to Design history).

## Rendering host

- 📋 **Render profiles — readback (debug) vs GPU-direct (release)** —
  [implementation plan](reports/2026-06-07-1720-render-profiles-implementation-plan.html).
  Replace the unconditional per-frame GPU→CPU→GPU round trip with two selectable
  profiles: `ReadbackCapture` (offscreen render → CPU readback → re-upload; keeps
  live-window screenshots free; default in debug builds) and `DirectGpu` (draw
  straight into the swapchain image with semaphore sync, no per-frame round trip;
  default in release builds, with on-demand screenshot fallback). Also fixes the
  latent missing `TransferDstBit` swapchain usage flag.
