# FS Skia UI

**FS Skia UI** is an F# desktop UI framework: an Elmish/MVU runtime driving a
Skia-on-Vulkan renderer, with a typed control suite, a Yoga-backed layout engine,
and a compiled, single-source **governance system** that decides which validation
gates run for any change.

This site combines a **generated API reference** (from the published packages' XML
doc comments) with **authored architecture documentation** — and, distinctively,
every architecture page closes with an honest *strengths/weaknesses + pros/cons*
analysis rather than a one-sided summary.

## Find your entry point

Pick the path that matches what you are doing. Each lands in one step.

### I'm building an app on FS Skia UI (consumer)

- [**API Reference**](reference/index.html) — every supported public type and
  member, with summaries, parameters, and returns.
- [Typed controls & the design-token / Penpot flow](controls-design/typed-front-door.html)
  — author against the typed Props/MVU front door.
- [Runnable examples](examples/typed-control-mvu.html) — literate `.fsx` scripts,
  compiler-verified at build time.

### I'm contributing to the framework (contributor)

- [**Architecture overview**](architecture/host-skiaviewer.html) — one page per
  major part (host, scene, layout, input, Elmish/MVU, controls, testing,
  governance), each with a closing analysis.
- [Governance system](governance/index.html) — routing, evidence/audit, and
  single-source generation.
- [Roadmap & TODO](roadmap.html) — planned and in-progress work, each item linked
  to its design/implementation plan.

### I'm running the Spec Kit process (speckit practitioner)

- [**Governance & speckit placement**](governance/speckit-placement.html) — which
  speckit phase each governance touchpoint governs and how to respond.
- [The Spec Kit process](speckit/process.html) — where custom FS Skia UI
  components are created and consumed across the phases.

## The published packages

The API reference covers the ten published packages:

| Package | What it provides |
|---|---|
| `FS.Skia.UI.Scene` | Scene primitives and the drawing vocabulary |
| `FS.Skia.UI.SkiaViewer` | The Skia/Vulkan rendering host |
| `FS.Skia.UI.Elmish` | The Elmish/MVU runtime and animation tick |
| `FS.Skia.UI.Input` | Keyboard binding/command/mode runtime (pointer/mouse events live in the host's `ViewerEvent` + `Controls.Pointer`) |
| `FS.Skia.UI.KeyboardInput` | Lightweight key→command reducer |
| `FS.Skia.UI.Layout` | Yoga-backed flexbox layout |
| `FS.Skia.UI.Controls` | The control suite + typed front door |
| `FS.Skia.UI.Controls.Elmish` | Elmish bindings for the controls |
| `FS.Skia.UI.Testing` | Test/evidence helpers |
| `FS.Skia.UI.SkillSupport` | Skill-support helpers |

## Building this site

```bash
dotnet tool restore
dotnet fsdocs build --strict --eval   # full site to output/
dotnet fsdocs watch                   # live-reload while authoring
```

The site publishes to [GitHub Pages](https://ehotwagner.github.io/FS-Skia-UI/)
automatically on push to `main` (see `.github/workflows/docs.yml`); no generated
output is committed.
