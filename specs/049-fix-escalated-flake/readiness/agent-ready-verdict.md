# Agent-ready verdict (T014)

**Verdict: agent-ready.** An autonomous agent operator can obtain a trustworthy escalated
verdict from a **single** run of the path on the standard headless host, with **no** manual
environment-variable setup and **no** focused rerun (FR-001 / FR-002 / FR-006 / SC-001 /
SC-003).

## Basis (single-run escalated evidence, T013)

- `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` each exited 0
  in one sequential run with no manual `env -u WAYLAND_DISPLAY` prefix
  (`readiness/aggregate-hang-diagnostics.md`, `readiness/logs/*.log`).
- No `libdecor-gtk.so` teardown crash: the GUI/viewer tests pass their assertions **and are
  reported as passing** (`logs/test.txt` exit-code=0; focused control
  `logs/skiaviewer-control.log` 48/48, exit 0) (FR-004).
- No graphics-init stall: `GeneratedProductCheck` completed within its normal envelope
  (~94s); the previously observed ~20-minute hang did not recur (FR-003 / SC-002).
- Determinism is guaranteed by the pure dual-display guard (12/12 unit + FsCheck property
  tests in `GraphicsEnvironmentTests.fs`), not by repetition (SC-001).

## Standing invariants confirmed

- `git diff --stat -- 'src/**'` is empty — product runtime and `.fsi` byte-unchanged
  (SC-004, Tier 2); `readiness/runtime-untouched`-equivalent proof recorded in
  `target-metadata.md`.
- No FAKE target added/removed/renamed; `validation.contract.yml` / `TargetMetadata` /
  `TargetMetadataDrift` unchanged (C6, `target-metadata.md`).
- Real failures are not masked: a nonzero child exit is still reported as a failure with its
  code propagated (spawn-contract test; C5 / FR-008 / SC-006).

## Residual / operator note

Genuinely race-like failures unrelated to the graphics-backend flake class are still rerun
in focused isolation. One operational caveat (from T013): let `./fake.sh` rebuild the
compiled front-end before trusting a verdict — a stale `dotnet run` build that predates the
spawn-edge change can still reproduce the crash.
