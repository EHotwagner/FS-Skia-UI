# Contract impact — zero `.fsi` / zero surface obligation (feature 115, T005, FR-003)

## Asserted obligation

This feature changes **no contract**. The following MUST report **no delta** after every applied bump:

- **`.fsi` signature files** — zero edits; no public/internal/private visibility change.
- **public surface baselines** — top-level and per-package surface baselines unchanged.
- **goldens** — Scene-parity / catalog / corpus goldens unchanged (no rendered-output or geometry change).
- **sample contracts** — no sample/`controls`-catalog contract change.
- **generated product output** — a freshly generated `dotnet new fs-skia-ui` project's contents are
  unchanged by the safe bumps (FR-006).

## Enforcing assertion

The surface/golden/generated-product gates are the enforcing mechanism, not this prose:

- `Dev` (build + Expecto/FsCheck suites + Scene-parity goldens) — a behavior or output change turns a
  currently-green golden red.
- `PackageSurfaceCheck` / `PerPackageSurfaceDiff` / surface baselines (run iff Route prints them) — a
  `.fsi`/surface delta fails them. For THIS diff Route did **not** escalate to the controls-public-surface
  tier (no `src/**` edit), so the surface-diff gates are not in the printed set; the zero-`.fsi` claim is
  guaranteed structurally (no `.fs`/`.fsi` file is touched at all) and confirmed by `git diff --stat`
  showing no `*.fs`/`*.fsi` path.
- `GeneratedProductCheck` / `TemplateCheck` — a generated-project restore/build/content change fails them.
- `EvidenceAudit` — must report `verdict=PASS` with zero synthetic markers.

## Why no `.fsi` can change

Feature 115 edits only `Directory.Packages.props` (version strings), `.specify/init-options.json` (a
recorded version string), `template/**` pins (only if needed for consistency — version strings), and
`docs/`/`specs/` evidence. **No `src/**/*.fs` or `*.fsi` file is in the diff**, so a signature change is
structurally impossible for the safe bumps. A held bump is adopted only if it is byte-clean with **no
source change** — i.e. if adopting it would require any `.fs`/`.fsi` edit, it is deferred and reverted
(FR-004/FR-005), preserving the zero-`.fsi` obligation.
