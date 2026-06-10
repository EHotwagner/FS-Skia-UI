# SC-006 — the contrast gate remains the sole contrast authority

The resolver introduces **no** second/parallel contrast policy (FR-007). Contrast
stays governed by the existing `ContrastCheck` gate over its documented
`(foreground, background, role)` pairing set.

## Evidence

- **`ContrastCheck` PASS** — `./fake.sh build -t ContrastCheck` → `Status: Ok`.
  The auto-generated report `readiness/color-contrast-evidence.md` records every
  validated pairing in both themes (measured vs required, pass/fail). No migrated
  control's **default** styling regresses its contrast result — the no-class
  default render is byte-identical to the procedural baseline (SC-003), so its
  foreground/background pairing is unchanged.
- **`DesignTokenDrift` PASS** — the new `success`/`warning` tokens are generated
  from the DTCG single source; `DesignTokens.fs` is a byte-identical regeneration
  (no inline literal bypasses the gate, FR-008).
- **Insufficient `Custom` still resolves (no silent drop)** —
  `Feature093StyleResolverTests` proves an unknown/free-form `Custom` class
  resolves to a concrete value (identity delta), never an exception or a drop;
  whether that concrete value is contrast-sufficient is the existing gate's call,
  not the resolver's.

## Result

PASS — `ContrastCheck`/`DesignTokenDrift` remain the authority; the resolver
neither duplicates nor bypasses them.
