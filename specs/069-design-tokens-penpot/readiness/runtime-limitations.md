# Runtime limitations & unsupported scope (feature 069)

## Platform baseline (inherited, unchanged by this feature)

The framework targets the **.NET 10 desktop** runtime rendering through **Vulkan**
via the **SkiaSharp preview** stack. Unsupported targets remain
**unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**.
This feature is build-time text generation only and changes none of these.

## Unsupported / out-of-scope handling

- **No live Penpot/MCP integration**: DTCG is established here only as the
  interchange format. Network sync, inspect/draft/provenance, and code↔design
  round-trip are deferred to the later "Penpot MCP assist" roadmap item.
- **No remaining-41-controls migration**: only the 10 `Theme` primitives are
  tokenized in this feature. Migrating the rest is `070`+.
- **No motion/animation tokens, runtime theme-switching UI, or new
  color-science/contrast computation**, and **no shipped theme value change**.

## Loud failure diagnostics (no partial emit)

The generator fails **loudly** — naming the offending token — and emits **no** F#
(no partial module) when the DTCG source is:

- malformed JSON,
- an unresolvable or cyclic alias (`{a}→{b}→{a}`),
- missing a token the `Theme` mapping requires.

A missing or unparseable generated `DesignTokens.fs` is reported as all-`Missing`
by `DesignTokenDrift` (loud, never a silent pass).

## Authoritative command

`./fake.sh build -t DesignTokenDrift`
