# SC-006 — Generated consumers stay governed via the packaged engine (no Python)

**Task:** T028. **Command chain:** `./fake.sh build -t PackLocal` →
`./fake.sh build -t GeneratedProductCheck` (→ TemplateCheck). All FAKE-backed,
run sequentially.

## Result: PASS

- `PackLocal` packs the published governance library
  **`FS.Skia.UI.Build` 0.1.45-preview.1** into the local feed
  (`~/.local/share/nuget-local/FS.Skia.UI.Build.0.1.45-preview.1.nupkg`) — see
  `local-packages.md` (now lists the new package identity with no product/runtime
  package affected).
- `GeneratedProductCheck` → **Status: Ok**. It packs + installs the template,
  instantiates all profiles, restores from the local feed, and runs each
  generated project's `EvidenceGraph` / `EvidenceAudit` gates **in-process**.

## In-process consumption proof (FR-013, SC-006)

Every generated profile's `readiness/evidence-audit.md` reports:

```
authority=in-process-engine
engine=FS.Skia.UI.Build.Evidence
status=ok
verdict=PASS
total-blockers=0
```

Captured copies under `generated-evidence-reports/` for all five profiles
(`app-source`, `app-package`, `governed-source`, `headless-scene-source`,
`sample-pack-source`). The generated `build.fsx` resolves the engine via
`#r "nuget: FS.Skia.UI.Build, 0.1.45-preview.1"` and calls
`Engine.runGraph` / `Engine.runAudit`.

## No copied Python / run-audit.sh

`find artifacts/generated-products/043-foundations-evidence-engine -name run-audit.sh`
returns **nothing** — the generated projects carry no `run-audit.sh` and no
`*.py` evidence script. The retained data file
`.specify/extensions/evidence/audit-patterns.yml` (read by the diff-scan) is
still copied. Both copy paths now exclude the scripts directory:

- `.template.config/template.json` `.specify/` source `exclude` gains
  `extensions/evidence/scripts/**`.
- `build.fsx` `copySpecKitInstall` `copyDirectoryExcept` gains
  `extensions/evidence/scripts/`.

The generated-product scan's required-file list swaps
`.specify/extensions/evidence/scripts/bash/run-audit.sh` →
`.specify/extensions/evidence/audit-patterns.yml`.
