# Governance risk levels (feature 112)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls (ControlRuntime, internal seam) + FS.Skia.UI.Controls.Elmish (live renderRetained seam)
public-api-impact=no public signature change; new INTERNAL `RuntimeStampResult` type + `applyRuntimeVisualStateTargeted`/`runtimeStampFor` vals in ControlRuntime.fsi; `RuntimeStateTouchedNodeCount` is internal
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; dispatch OUTCOMES byte-identical, FR-008 — only the per-frame visual-state STAMP mechanism changes)
route-tier=agent-ready (controls-public-surface)

## Risk classification

- **small** — a test-only edit that does not touch any `.fsi`: focused validation is `Dev` plus the
  affected test list.
- **medium** — the `ControlRuntime` internal seam + the live wiring: focused validation is
  `RefreshSurfaceBaselines` + `Dev` + the per-package surface diff.
- **broad** — the full escalated controls-public-surface set Route prints, required because the Controls
  package `.fsi` surface changes (a new internal type + vals move the per-package baseline).
  **broad validation** is mandatory before merge. Non-authoritative aggregate results are advisory only
  in [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is the
  focused per-target rerun.

THIS feature is **broad** (a Controls package `.fsi` surface change).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

Run `./fake.sh build -t Route` against the real diff and obey its printed minimal list. The Controls
`.fsi` change escalates to **controls-public-surface**; Route prints the controls-public-surface set
(`Dev`, the package/per-package surface diffs, `FsiTranscripts`, the controls catalog/doc/interaction/
rendering checks, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`,
`EvidenceAudit`). `RefreshSurfaceBaselines` regenerates the per-package Controls surface (gains the
internal `RuntimeStampResult` type + the two vals); the top-level public Controls surface baseline is
unchanged (the seam is internal).

## Required evidence per risk level

- **required evidence** (broad / Controls `.fsi`): recaptured per-package Controls surface baseline +
  the new type/vals' `///` docs (doc-preservation gate); the top-level public surface baseline shows no
  delta.
- **required evidence** (US1 touched count): a hover/focus move touches the affected identities +
  ancestors (« N) and a no-change frame touches 0 (`Feature112TouchedCountTests`, SC-001/SC-003/SC-006).
- **required evidence** (US2 parity): the targeted stamp's scene equals the full-tree oracle's over
  hover/focus/press transitions, incl. the route-selection helper (`Feature112TargetedStampParityTests`,
  SC-002), and consumer-set precedence is preserved (`Feature112PrecedenceTests`, SC-004).
- **required evidence** (byte-identity): at-rest rendered output + geometry byte-identical — the standing
  Scene-parity golden suite under `Dev` ([byte-identity-authority.md](./byte-identity-authority.md),
  FR-008/SC-005).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
