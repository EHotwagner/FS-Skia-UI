# Data Model: Governance Skew & Doc-Check Hardening

This feature is a pure text-analysis governance hardening; it introduces no new stateful entities
and no new persisted record types. The "entities" below are the governance values whose *inputs* or
*derivation* change.

## Referenced symbol (skew input)

- **Definition:** an `FS.Skia.UI.*`-rooted token (`open` leaf or fully-qualified trailing symbol)
  extracted from generated `template/base/src` + `template/base/tests` source by
  `PackageSkew.referencedSymbols`.
- **Shape (unchanged):** `(symbol: string) * (file: string)` pairs.
- **Change:** the *source text* is now **comment-stripped** before extraction (FR-001). Tokens that
  appear only inside `//` / `///` / `(* *)` comments no longer contribute. A token in both a comment
  and live code still contributes via its live-code occurrence.

## Captured per-package surface

- **Definition:** the `Set<string>` of declaration-site symbol names per tracked package, persisted
  as `readiness/per-package-surface/<PackageId>.fsi.txt` and aggregated into the pinned surface that
  `PackageSkew.detectSkew` validates references against.
- **Shape (unchanged):** normalized, comment-stripped concatenation of the package's `.fsi` files;
  `surfaceSymbols` derives the symbol set.
- **Change (FR-002):** `captureCurrent` enumerates `*.fsi` **recursively** under the package source
  dir, so `FS.Skia.UI.Controls` now includes `src/Controls/Widgets/*.fsi` (the typed front door).
  `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` is regenerated **additively** — symbols
  are added (typed-front-door members + their namespace segments incl. `Typed`), none removed.
  Validation rule: ordering deterministic (sort by relative path) so the baseline is stable across
  runs/platforms.

## Skew finding (`PackageSkewFinding`)

- **Definition / shape (unchanged):** `{ Symbol; File; PinnedVersion; LocalVersion }` from
  `EvidenceFormatSchema`. Produced when a referenced symbol resolves nowhere in the pinned surface.
- **Change:** none structurally. After the fix the *population* shrinks for false positives
  (comment-only tokens, typed-front-door references) while genuinely-absent symbols still produce a
  finding (FR-003). Report `readiness/package-skew.md` expected `status=clean`, `findings=0`.

## Doc-preservation signal

- **Definition:** the package-agnostic evidence that the API-reference generator carries `///`
  summaries through into each `docs/api-surface/<PackageId>.md`.
- **Old signal (removed):** presence of the placeholder boilerplate sentence "Public contract type
  exposed by this FS.Skia.UI package." in Scene/Testing references.
- **New signal (FR-004):** for **every** tracked package reference, the embedded "## Curated
  Signatures" body contains ≥1 `///`-prefixed line whose summary is **not** a placeholder
  (classified by `ControlsDocCoverage.isPlaceholderSummary`). Optional corroboration:
  `xml-summary-count > 0`.
- **Validation rule (FR-005):** the check MUST FAIL when a reference body carries zero `///`
  summary lines (proven by a red-before fixture).

## Non-entities (explicitly unchanged)

- No product `Model` / `Msg` / `Effect` / `Cmd<Msg>` — no MVU surface is involved (pure analyses).
- No product `.fsi` signature shape change. No `validation.contract.yml` / gate change.
- No template, generated-project, or dependency record change.
