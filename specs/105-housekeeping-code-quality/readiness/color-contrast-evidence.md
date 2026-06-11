# Color Contrast Evidence

PASS: every enforced foreground/background pairing meets its required WCAG ratio in both themes (SC-001).

## light theme

| Foreground | Background | Role | Foreground color | Background color | Measured | Required | Result |
|------------|------------|------|------------------|------------------|----------|----------|--------|
| foreground | background | Text | #1f2937ff | #f8fafcff | 14.03:1 | 4.50:1 | PASS |
| accent | background | Text | #2563ebff | #f8fafcff | 4.94:1 | 4.50:1 | PASS |
| danger | background | Text | #b91c1cff | #f8fafcff | 6.18:1 | 4.50:1 | PASS |
| muted | background | Text | #64748bff | #f8fafcff | 4.55:1 | 4.50:1 | PASS |
| accent | background | GraphicOrUi | #2563ebff | #f8fafcff | 4.94:1 | 3.00:1 | PASS |

## dark theme

| Foreground | Background | Role | Foreground color | Background color | Measured | Required | Result |
|------------|------------|------|------------------|------------------|----------|----------|--------|
| foreground | background | Text | #f1f5f9ff | #111827ff | 16.19:1 | 4.50:1 | PASS |
| accent | background | Text | #60a5faff | #111827ff | 6.98:1 | 4.50:1 | PASS |
| danger | background | Text | #ff9592ff | #111827ff | 8.42:1 | 4.50:1 | PASS |
| muted | background | Text | #94a3b8ff | #111827ff | 6.92:1 | 4.50:1 | PASS |
| accent | background | GraphicOrUi | #60a5faff | #111827ff | 6.98:1 | 3.00:1 | PASS |

- regenerate: ./fake.sh build -t RefreshSurfaceBaselines (after a DTCG token edit)
- gate: ./fake.sh build -t ContrastCheck
- failure-class: sub-threshold-shipped-token

## Independent validation / regression protection (US1, SC-005)

The gate is independently falsifiable through the DTCG single source only:

1. Drop a validated pairing below threshold — e.g. set `dark.danger` near
   `dark.background` in `src/Controls/design-tokens.tokens.json`.
2. `./fake.sh build -t RefreshSurfaceBaselines` regenerates `DesignTokens.fs`.
3. `./fake.sh build -t ContrastCheck` then FAILS, naming the pairing, the measured
   ratio, and the required ratio (see the `## Failing rows` section on a failing run).
4. Restoring an accessible value (e.g. a Radix ramp step) makes the gate PASS again.