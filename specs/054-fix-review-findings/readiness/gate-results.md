# Escalated Maintainer-Verify Gate Results — Feature 054 (T019–T021)

`Route` escalated to `tier=agent-ready` with the gate list below. FAKE-backed
targets were run **sequentially, never concurrently** ([[fake-build-constraints]]).
Aggregate results are recorded as non-authoritative; the one race-like
environmental failure was rerun in focused isolation as the authoritative result.

| Gate | Result | Log |
|---|---|---|
| `Route` / `Route --enforce` | `tier=agent-ready`, all required artifacts present, `Status: Ok` | logs/route.txt, logs/route-enforce.txt |
| `Dev` | `Status: Ok` — full build (FS3261-clean, escape hatch removed) + all test suites incl. `Governance.Tests` 349/349 (behaviour preserved, FR-006) | logs/dev.txt |
| `GeneratedGuidanceCheck` | `Status: Ok` | logs/generated-guidance-check.txt |
| `TemplateCheck` | `Status: Ok` — includes the strengthened pin-parity assertion (FR-003) | logs/template-check.txt |
| `GeneratedProductCheck` | `Status: Ok` (authoritative rerun; see flake note) | logs/generated-product-check.txt |
| `TemplateDrift` | `Status: Ok` | logs/template-drift.txt |
| `EvidenceGraph` (T020) | `verdict=ok` — acyclic, 0 `[S]`/`[S*]`/unaccepted synthetic | logs/evidence-graph.txt, task-graph.md |
| `EvidenceAudit` (T021) | **PASS** — 0 unaccepted-synthetic, 0 auto-synthetic, 0 late-SEH, 0 diagnostics | logs/evidence-audit.txt, evidence-audit.md |

## Non-authoritative aggregate / focused rerun (GeneratedProductCheck)

The first `GeneratedProductCheck` aggregate run reported `Status: Failure` from
`SkiaViewer.Tests`. Diagnosis: the host advertises **both** `WAYLAND_DISPLAY` and
`DISPLAY` (DualDisplay); the graphics stack tried to load `libdecor-gtk.so`,
which cannot init in this container, crashing the **test host on teardown** —
*after* the suite's assertions passed (the log shows `SkiaViewer.Tests` Passed
48/48 and 39/39 across the aggregate before the abort). This is the documented
environmental flake the 049 X11-forcing addresses, not a regression from this
feature (which touches only `build/Governance/**` + the template `#r` pin).

- **Focused rerun (authoritative):**
  `env -u WAYLAND_DISPLAY GDK_BACKEND=x11 SDL_VIDEODRIVER=x11 dotnet test tests/SkiaViewer.Tests/...`
  → **Passed 48/48**, no `libdecor` crash, no abort
  (logs/skiaviewer-tests-focused-rerun.txt).
- **Full gate rerun (authoritative):** `GeneratedProductCheck` rerun with the
  same X11-forced environment → `Status: Ok`.

The aggregate failure is recorded as **non-authoritative**; the focused/forced-X11
result is the authoritative verdict.
