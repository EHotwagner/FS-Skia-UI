# Quickstart: Focus, Keyboard Traversal & Input Routing (E4)

A maintainer's path through the E4 focus model — what to build, in what order, and how to prove it.

## What E4 adds (one sentence)

A deterministic **tab order** + **keyboard traversal** + **focused-control key delivery** for all
controls, generalizing E1's text seam — all driven by the existing `AccessibilityMetadata`, with
focus surviving re-renders via E2 identity and indicated through E3's `Focused` style.

## Build order (Spec → FSI → Tests → Implement)

1. **Sketch `Focus.fsi`** (contracts/focus-model.md) and exercise it in FSI:
   ```fsharp
   #r "FS.Skia.UI.Controls.dll"
   open FS.Skia.UI.Controls
   let order = Focus.order myView
   Focus.traverse order None Next            // first focusable stop
   Focus.route buttonKeyboard "Enter" false false   // Activate
   Focus.route buttonKeyboard "Tab"   true  false   // Traverse Next
   ```
2. **Write failing-first tests** in `Controls.Tests`: tab-order, traversal (cyclic wrap), the
   FsCheck purity/totality/determinism property (≥1000), routing (Button activates once;
   Slider/RadioGroup navigates), the E1-text-seam regression, focus-stability over the live
   retained path, and the R1 correction (a focusable activation-only Button is valid; Tab is not
   consumed by a default control).
3. **Apply Research R1**: correct `Accessibility.defaultFor` (Tab out of `NavigationKeys`,
   intra-control arrows in per role) and relax `Accessibility.validate` (focusable need not carry
   `NavigationKeys`). `.fs` only — `Accessibility.fsi` unchanged.
4. **Implement `Focus.fs`**: `order` (pre-order walk → keep focusable → stable sort by
   `(FocusOrder ?? +∞, docIndex)`), `traverse` (index ± 1 mod n, None→first/last, stale recovery),
   `route` (membership tests then Tab test → `KeyRouting`). Plain folds / list walks; no SRTP /
   reflection / type providers / custom operators.
5. **Implement `routeFocusedKey`** in `Controls.Elmish` (contracts/key-routing-surface.md): resolve
   the focused control over the retained tree, normalize `ViewerKey`, run the E1 text seam first,
   then `Focus.route`, emit authored activation/navigation messages or a `FocusControl` traversal
   message, else fall through. Wire it into `runInteractiveApp` before `host.MapKey`.
6. **Recapture baselines**: `RefreshSurfaceBaselines` + `PerPackageSurface.captureCurrent`
   (controls-public-surface + Controls.Elmish package-surface + per-package + cross-package).

## Representative verification set (mechanism + representative, NOT catalog-wide)

- **`Button`** — activation control: Space/Enter fires its authored activation binding once.
- **`Slider`** / **`RadioGroup`** — navigation control: arrows fire its value-change/selection.
- **a text control** — proves the E1 keystroke/composition seam is preserved unchanged.

A keyboard retrofit of all 52 typed views is **out of scope** (a separate fitness pass).

## Validate (run `Route` first; run only what it prints)

Public `src/Controls/*.fsi` (the new `Focus.fsi`) + `src/Controls.Elmish/*.fsi` edits escalate to
the controls-public-surface / package-surface rules — the serialized order applies:

```
./fake.sh build -t Route          # authoritative tier + gate list for THIS diff
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

FAKE-backed targets run **sequentially** (shared `.fake` state). `ContrastCheck` applies only if the
focus indicator introduces a new token-derived color (it should not — it resolves through E3's
`Focused` style).

## Evidence (under `specs/094-focus-keyboard-traversal/readiness/`)

`us1-tab-traversal.md`, `us2-focused-key-delivery.md`, `us2-text-seam-preserved.md`,
`us3-focus-stability.md`, `us3-focus-indicator.md`, `sc006-determinism-property.md`,
`sc007-validate-order.md`, `responds-proof.md`, `fsi-transcript.md`, `surface-baselines.md`.
All real: deterministic reducer / route-probe results, the live retained path for stability, the
reused E1 `captureRespondsProof` for the responds-proof, and `Accessibility.validate` over the
representative view. No synthetic evidence planned.

## The one trap to avoid

`Accessibility.defaultFor` today puts `["Tab"; "Shift+Tab"]` in **every** focusable control's
`NavigationKeys` and `validate` requires it. If you keep that, FR-007 ("a control's `NavigationKeys`
consumption wins") makes every control consume Tab and **global traversal never fires**. Fix it
first (R1): traversal keys are engine-level; `NavigationKeys` is intra-control arrows only.
