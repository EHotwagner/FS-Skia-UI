# Governance Risk Levels — feature 093 (visual-state / style layer)

This is a **Tier 1** change: it moves public surface on `FS.Skia.UI.Controls`
(new `StyleVariant`/`StyleClass`/`ResolvedStyle` types and `StyleClassesValue`/
`VisualStateValue` `AttrValue` arms on `Types.fsi`, a new public `Style.fsi`,
`Attributes.styleClasses`/`Attributes.visualState`, two new `DesignTokens`
tokens, and typed-`Props` `Classes` deltas). `Route` reports
`tier=agent-ready` and matches `controls-public-surface`, so the escalated
gate set applies.

## Scope

- Tier: Tier 1 (public `src/Controls/*.fsi` surface move).
- Public F# API impact: additive — new types/builders/tokens; the
  `view : 'model -> Control<'msg>` contract is unchanged.
- Package identity/content/version impact: none (version bump happens on merge).
- Product MVU/runtime state impact: not applicable — the resolver is a pure,
  total function; it reads but never owns `VisualState`.
- Runtime support expansion: none (deterministic render-only evidence).

## Required Evidence Paths

| Risk level | Minimum evidence (required evidence) |
|------------|--------------------------------------|
| small | Internal `Style.fs` fold / token-delta change: `Dev` + the targeted `Controls.Tests` resolver suites. |
| medium | Typed-`Props` + `ControlInternals` migration: `Dev` + parity/regression suites + `DesignTokenDrift` + `ContrastCheck`. |
| broad | The public `*.fsi` surface move escalates to **controls-public-surface**: the full serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit` path plus `PackageSurfaceCheck`/`PerPackageSurfaceDiff`/`DesignTokenDrift`/`ContrastCheck` is the **broad validation** required here. |

This feature finishes at **broad** risk because it moves public surface. The
broad validation runs FAKE-backed targets **sequentially** (shared `.fake`
state); aggregate results are recorded as **non-authoritative** unless
re-confirmed sequentially (see `aggregate-hang-diagnostics.md`).
