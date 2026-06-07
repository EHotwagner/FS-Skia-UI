# Quickstart: Build, Preview, and Publish the Docs Site

Runbook for the FsDocs documentation site. Use the `fsdocs-*` skills for the
authoring loop; this is the operational path.

## One-time setup (per machine)

```bash
# Restore the pinned fsdocs-tool from .config/dotnet-tools.json
dotnet tool restore
```

`GenerateDocumentationFile=true` is already set repo-wide in
`Directory.Build.props`, so XML doc emission is on. The `fsdocs-setup` skill owns
adding the tool pin, the `FsDocs*` properties (site root / source link / theme /
`FsDocsWarnOnMissingDocs`), and the `.gitignore` entries (`output/`, `.fsdocs/`,
`tmp/`).

## Author

- **API docs**: add `///` comments on the member's **`.fsi`** declaration (not the
  `.fs` — for signatured modules the compiler emits docs from the signature).
  Use the `fsdocs-api-doc` skill; ignore its generic "edits `.fs`" wording for
  this repo.
- **Architecture / governance / controls-design prose**: Markdown under
  `docs/**` via `fsdocs-technical`. Every architecture page ends with the
  strengths/weaknesses + pros/cons analysis.
- **Examples**: literate `.fsx` under `docs/examples/` via `fsdocs-examples`.

## Build & preview locally

```bash
# Full strict build (what CI runs): evaluates .fsx, fails on warnings/missing docs
dotnet fsdocs build --strict
# -> output/  (open output/index.html)

# Live-reload while authoring
dotnet fsdocs watch
```

If a `Docs` FAKE target is added (see research.md R4), `./fake.sh build -t Docs`
runs the same strict build. Capture the log to
`specs/076-fsdocs-documentation-site/readiness/logs/fsdocs-build.txt`.

> Malformed `///` XML fails `dotnet build` *before* fsdocs runs, because the repo
> sets `TreatWarningsAsErrors=true` (FS3390 → error). Write well-formed XML.

## Verify the contract is preserved (FR-004 / SC-007)

```bash
# Prove doc comments did not move any surface baseline
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t PerPackageSurfaceDiff
# Save evidence -> readiness/surface-baseline-unchanged.md
```

## Route the change (authoritative gate set)

```bash
./fake.sh build -t Route            # prints tier + minimal gate list for THIS diff
./fake.sh build -t Route --enforce  # also fails if required evidence is missing
# Save -> readiness/logs/route.txt
```

Expected routing for a mixed diff: `.fsi` doc comments → `package-surface`
(`PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff`); `docs/**`
content → `docs-only` (`EvidenceGraph`, needs `readiness/validation-contract.md`);
`src/**/*.fs` (none expected) → inner-loop `Dev`. Run **only** the gates Route
prints, FAKE targets sequentially.

## Publish to GitHub Pages

Publishing is automated — push to `main` triggers `.github/workflows/docs.yml`,
which restores the tool, runs `dotnet fsdocs build --strict`, uploads `output/` as
the Pages artifact, and deploys via `actions/deploy-pages`. No generated output is
committed.

```bash
# Manual run if needed
gh workflow run docs.yml
```

Published site: `https://ehotwagner.github.io/FS-Skia-UI/`. Save the run URL (or
`readiness/logs/pages-deploy.txt`) as SC-005/SC-006 evidence.

## Acceptance smoke (maps to success criteria)

1. Open `output/index.html` → reach consumer / contributor / speckit entry in ≤ 2
   clicks (SC-008).
2. Open a known public type's API page → non-empty summary; cross-link to its
   architecture page resolves (SC-001, FR-011).
3. Open each architecture page → architecture body + closing analysis with both
   sides (SC-002).
4. Open the governance section → touchpoints mapped to speckit phases + usage
   guidance (SC-003).
5. Open the typed-control/Penpot section → token-to-control flow + speckit phase
   placement (SC-004).
6. Surface baselines unchanged (SC-007); every required `.fsx` evaluated (SC-009).
